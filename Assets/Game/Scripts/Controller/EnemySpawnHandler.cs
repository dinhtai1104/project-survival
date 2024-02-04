using Cysharp.Threading.Tasks;
using Game.GameActor;
using Game.Pool;
using JetBrains.Annotations;
using Sirenix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;


namespace Game.Level
{
    [System.Serializable]
    public class Wave
    {
        public bool isActived;
        public int wave;
        public float startTime;
        public int timer;
        CancellationTokenSource cancellation;
        bool isPaused;
        public Wave(int wave)
        {
            this.wave = wave;
            cancellation = new CancellationTokenSource();
            startTime = Time.time;
        }

        public async UniTask<Wave> SetTimer(int timer)
        {
            isPaused = false;
            this.timer = timer;
            isActived = true;
            await Run();
            isActived = false;
            return this;
        }

        public async UniTask Run()
        {
            int time = timer;
            while (time > 0)
            {
                time--;
                Messenger.Broadcast(EventKey.WaveCountDown, time);
                if (isPaused)
                {
                    await UniTask.WaitUntil(() => !isPaused, cancellationToken: cancellation.Token);
                }
                await UniTask.Delay(1000, DelayType.DeltaTime, cancellationToken: cancellation.Token);
            }
        }
        public void Pause()
        {
            isPaused = true;
        }
        public void Continue()
        {
            isPaused = false;
        }
        public void Destroy()
        {
            if (cancellation != null)
            {
                cancellation.Cancel();
                cancellation.Dispose();
                cancellation = null;
            }
        }
    }
    public class EnemySpawnHandler : MonoBehaviour
    {
        public const string ENEMY_ADDRESS = "Enemy/{0}.prefab";

        protected GroupNpcSpawn groupSpawnPoints;
        public DungeonRoomEntity levelSpawnConfig;
        public int currentWave = 0;
        protected CancellationTokenSource cancellation;
        public List<ActorBase> enemies = new List<ActorBase>();
        protected bool isGameReady = false;

        private float lastTimeActorDie=0;
        float bonusCheckTime = 1;

        private Wave currentWaveTimer;
        private void OnEnable()
        {
            currentWaveTimer = null;
            cancellation = new CancellationTokenSource();
            GameController.onStageStart -= OnStageStart;
            ActorBase.onDie -= OnActorDie;
            Messenger.RemoveListener(EventKey.EnemyAllySpawn, OnAllySpawn);

        }
        private void OnDisable()
        {
            if (cancellation != null)
            {
                cancellation.Cancel();
            }
            GameController.onStageStart -= OnStageStart;
            ActorBase.onDie -= OnActorDie;
            Messenger.RemoveListener(EventKey.EnemyAllySpawn, OnAllySpawn);
        }
        public void Destroy()
        {
            ClearAll();
        }
        public void ClearAll()
        {
            foreach(var actor in enemies)
            {
                actor.SetActive(false);
            }
            enemies.Clear();
        }
        //setup enemy generator, using list of spawn points in the currentMap and a level config
        public virtual void SetUp(GroupNpcSpawn npcPoints, DungeonRoomEntity roomConfig)
        {
            this.groupSpawnPoints = npcPoints;
            this.levelSpawnConfig = roomConfig;
          //clear
            enemies.Clear();
            isGameReady = false;
            currentWave = 0;
            //setup event
            GameController.onStageStart -= OnStageStart;
            ActorBase.onDie -= OnActorDie;
            GameController.onStageStart += OnStageStart;
            ActorBase.onDie += OnActorDie;

            Messenger.AddListener(EventKey.EnemyAllySpawn, OnAllySpawn);

            // spawn first wave, but doesn't active behaviours
            SpawnWave(levelSpawnConfig.Waves[currentWave]);
        }

        // on game start. spawn first wave
        public void OnStageStart()
        {
            isGameReady = true;
            foreach (var enemy in enemies)
            {
                enemy.StartBehaviours();
            }
            // check if this level has any enemy => call instant game clear
            if (enemies.Count==0)
            {
                Messenger.Broadcast<bool>(EventKey.GameClear, true);
            }
        }
        private void OnAllySpawn()
        {
            bonusCheckTime = 1;
        }

        // call this when any of the actor die, then check for enemy
        protected virtual void OnActorDie(ActorBase actor, ActorBase attacker)
        {
            if (enemies.Contains(actor))
            {
                enemies.Remove(actor);
                lastTimeActorDie = Time.time;
            }
            CancelInvoke();
            currentWaveTimer?.Pause();
            Invoke(nameof(CheckClearEnemy), 0.5f+ bonusCheckTime);
        }

        // after a enemy kill, delay check clear
        protected virtual void CheckClearEnemy()
        {
            Logger.Log("CheckClearEnemy");
            //if there is no enemy
            if (enemies.Count == 0)
            {
                Logger.Log("=>CheckClearEnemy "+(currentWaveTimer!=null)+ " "+currentWave);
                //if current wave is loaded but enemies haven't spawned yet.
                if (currentWaveTimer!=null &&Time.time - currentWaveTimer.startTime > 2)
                {
                    Logger.Log("CheckClearEnemy 1");
                    ClearCurrentWave();
                    SpawnNextWave();
                }
                else if (currentWaveTimer == null && currentWave == 0)
                {
                    Logger.Log("CheckClearEnemy 2");
                    SpawnNextWave();
                }
                
               
            }
            else
            {
                currentWaveTimer?.Continue();
            }
            bonusCheckTime = 0;
        }
        protected void ClearCurrentWave()
        {
            Logger.Log("CLEAR CURRENT WAVE ");
            if (currentWaveTimer != null)
            {
                Logger.Log(" =>>>>>CURRENT WAVE "+currentWaveTimer.wave);

                currentWaveTimer.Destroy();
                currentWaveTimer = null;
            }
        }
        protected void SpawnNextWave()
        {
            currentWave++;
            Logger.Log("..... spawn next wave :" + currentWave+ " >= " + levelSpawnConfig.Waves.Count);
            //last wave
            if (currentWave >= levelSpawnConfig.Waves.Count )
            {

                //true if there is no enemy
                Messenger.Broadcast<bool>(EventKey.GameClear, levelSpawnConfig.Waves[currentWave - 1].EnemiesWave.Count==1 && string.IsNullOrEmpty(levelSpawnConfig.Waves[currentWave-1].EnemiesWave[0].Enemy));
            }
            //spawn new wave
            else
            {
                SpawnWave(levelSpawnConfig.Waves[currentWave]);
            }
        }
        protected virtual void SpawnWave(DungeonWaveEntity wave)
        {
            Logger.Log("Spawn wave " + currentWave);
            Game.Pool.GameObjectSpawner.Instance.ClearPool(EPool.Projectile, true);
            Messenger.Broadcast(EventKey.WaveStart, currentWave,levelSpawnConfig.Waves.Count);
            foreach (var enemySpawnConfig in wave.EnemiesWave)
            {
                Spawn(enemySpawnConfig).Forget();
            }


            //if wave has timer and not the last wave
            if ( currentWave < levelSpawnConfig.Waves.Count)
            {
                currentWaveTimer = new Wave(currentWave);
                Logger.Log("ADD TMER");
                if (currentWave < levelSpawnConfig.Waves.Count-1)
                {
                    currentWaveTimer.SetTimer((int)wave.DelayToWave).ContinueWith(OnWaveTimeOut).Forget();
                }

            }
           
        }
        void OnWaveTimeOut(Wave wave)
        {
            ClearCurrentWave();
            SpawnNextWave();
        }
       
        private async UniTask Spawn(EnemyWaveEntity enemySpawnConfig)
        {
            if (string.IsNullOrEmpty(enemySpawnConfig.Enemy)) return;
            int levelEnemy = enemySpawnConfig.RoomLinked.StageLinked.LevelEnemyStage;
            //delay then start spawn
            //await UniTask.Delay((1000 * enemySpawnConfig.Delay).ToInt(), cancellationToken: cancellation.Token);
            var enemyTotal = 1;
            for (int i = 0; i < enemyTotal; i++) {


                Character enemy = await SpawnSingle(enemySpawnConfig.Enemy, levelEnemy, groupSpawnPoints.GetSpawnPoint(enemySpawnConfig.SpawnPoint).Position,enemySpawnConfig.Delay,usePortal:currentWave>0);

                //await UniTask.Delay(enemySpawnConfig.spawnSpacing, cancellationToken: cancellation.Token);
            }
        }

     

        IStatGroup GetEnemyStat(string id, int enemyLevel)
        {
            var entity = DataManager.Base.Enemy.Dictionary[id];
            StatGroup stats = new StatGroup();
            stats.AddStat(StatKey.Level, enemyLevel);
            stats.AddStat(StatKey.Hp, 0);
            stats.AddStat(StatKey.SpeedMove, 0);
            stats.AddStat(StatKey.Dmg, 0);
            stats.AddStat(StatKey.ReduceDmg, 0);

            entity.GetStats(stats);
            stats.CalculateStats();

            var valueLevelDmgGrow = enemyLevel * entity.DmgGrown;
            var valueLevelHpGrow = enemyLevel * entity.HpGrown;
            stats.AddModifier(StatKey.Dmg, new StatModifier(EStatMod.Flat, valueLevelDmgGrow), this);
            stats.AddModifier(StatKey.Hp, new StatModifier(EStatMod.Flat, valueLevelHpGrow), this);
            return stats;
        }
        
        public virtual async UniTask<Character> SpawnSingle(string enemyId,int level,Vector2 position,float delay=0, bool isStartBehaviourNow = true,bool usePortal=false, string vfx_spawn = "",int healthBarGroup=-1)
        {
            if (!DataManager.Base.Enemy.Dictionary.ContainsKey(enemyId)) return null;

           
            //Logger.Log("GET:" + enemyId);
            var enemyEntity = DataManager.Base.Enemy.Dictionary[enemyId];

            var obj = await GameObjectSpawner.Instance.GetAsync(string.Format(ENEMY_ADDRESS,enemyId));
            Character enemy = obj.GetComponent<Character>();

            if (usePortal)
            {
                if (!vfx_spawn.IsNullOrWhitespace())
                {
                    Messenger.Broadcast(EventKey.ActorSpawnPortalWithCustomVFX, enemy.GetCharacterType(), position, vfx_spawn);
                }
                else
                {
                    Messenger.Broadcast(EventKey.ActorSpawnPortal, enemy.GetCharacterType(), position);
                }
                await UniTask.Delay(1500, cancellationToken:cancellation.Token);
            }

            enemy.gameObject.name = enemyId;
            await enemy.SetUp(GetEnemyStat(enemyId, level));
            enemy.Tagger.AddTags(enemyEntity.MonsterTags);
            enemy.SetPosition(position);
            enemy.SetActive(true);

            await UniTask.Yield(cancellation.Token);


         

            // Add body type and body size
            enemy.GetTransform().localScale = Vector3.one * enemyEntity.BodySize;
            enemy.GetRigidbody().bodyType = enemyEntity.BodyType;

            if (enemyEntity.BodyType == RigidbodyType2D.Kinematic)
            {
                enemy.GetRigidbody().velocity = Vector3.zero;
                enemy.MoveHandler.Locked = true;
            }

            enemy.PropertyHandler.AddProperty(EActorProperty.Vunerable, 1);


            Messenger.Broadcast(EventKey.ActorSpawn, (ActorBase)enemy, true,healthBarGroup);

            //get first skill
            int skillId = 0;
            while (enemy.SkillEngine.GetSkill(skillId) == null && skillId<=5)
            {
                skillId++;
            }

            if(enemy.SkillEngine.GetSkill(skillId)!=null)
                enemy.SkillEngine.GetSkill(skillId).SetCoolDown(Mathf.Max(0,enemy.SkillEngine.GetSkill(skillId).GetCoolDown()-delay));

            enemies.Add(enemy);

            DelayActiveEnemy(enemy, isStartBehaviourNow,usePortal?1:0);

            return enemy;
        }
        async UniTask DelayActiveEnemy(ActorBase enemy,bool isStartBehaviourNow,float delay=0)
        {
            await UniTask.Delay((int)(delay * 1000), cancellationToken: cancellation.Token);
            //
            if (isGameReady && isStartBehaviourNow)
            {
                enemy.StartBehaviours();
            }
        }
        public void ClearAllEnemy()
        {
            //cancellation.Cancel();
            while(enemies.Count > 0)
            {
                var enemy = enemies[enemies.Count - 1];
                if (enemy != null && enemy.IsActived)
                {
                    enemy.GetHit(new DamageSource
                    {
                        _damage = new Stat(10000000000),
                        Attacker = GameController.Instance.GetMainActor(),
                        Defender = enemy,
                        _damageType = EDamage.HeadShot,
                    }, enemy);
                }
                enemies.Remove(enemy);
            }
            ClearAll();
        }
    }
   
}

