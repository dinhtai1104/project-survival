using Cysharp.Threading.Tasks;
using Game.GameActor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Game.Handler
{
    public class HealthBarHandler : MonoBehaviour
    {
        public static HealthBarHandler Instance;
        public Dictionary<ActorBase, HealthBarBase> healthBars = new Dictionary<ActorBase, HealthBarBase>();

        public Dictionary<int, HealthBarBase> healthBarGroups = new Dictionary<int, HealthBarBase>();
        [SerializeField]
        HealthBarConfig[] healthBarRefs;

        private Dictionary<ECharacterType, AssetReference> dictionary = new Dictionary<ECharacterType, AssetReference>();

        CancellationTokenSource cancellation;

        [System.Serializable]
        private struct HealthBarConfig 
        {
            public ECharacterType type;
            public AssetReference reference;
        }

        // Start is called before the first frame update
        void Start()
        {
            Instance = this;
            foreach( var config in healthBarRefs)
            {
                dictionary.Add(config.type, config.reference);
            }
        }
        private void OnEnable()
        {
            Messenger.AddListener<bool>(EventKey.GameClear, OnGameClear);
            Messenger.AddListener<int, int>(EventKey.GameStart, OnGameStart);

            Messenger.AddListener<ActorBase, bool,int>(EventKey.ActorSpawn, OnActorSpawn);
            ActorBase.onDie += OnDie;

            cancellation = new CancellationTokenSource();

            SpawnHealthBarFromStack().Forget();

        }

        private void OnGameClear(bool arg1)
        {
            spawnIndex = 0;
            //HealthBarBase playerHealthBar = healthBars.ContainsKey(Game.Controller.Instance.gameController.GetMainActor()) ? healthBars[Game.Controller.Instance.gameController.GetMainActor()] : null;
            //foreach (var bar in healthBars)
            //{
            //    if (bar.Value != playerHealthBar)
            //    {
            //        bar.Value.Hide();
            //    }
            //}

            //healthBars.Clear();

            //healthBars.Add(Game.Controller.Instance.gameController.GetMainActor(), playerHealthBar);

        }

        private void OnDestroy()
        {
            Messenger.RemoveListener<bool>(EventKey.GameClear, OnGameClear);
            Messenger.RemoveListener<ActorBase, bool, int>(EventKey.ActorSpawn, OnActorSpawn);
            Messenger.RemoveListener<int, int>(EventKey.GameStart, OnGameStart);
            ActorBase.onDie -= OnDie;

            if (cancellation != null)
            {
                cancellation.Cancel();
                cancellation.Dispose();
                cancellation = null;
            }
        }

        
        private void OnDisable()
        {
            Messenger.RemoveListener<bool>(EventKey.GameClear, OnGameClear);
            Messenger.RemoveListener<ActorBase, bool, int>(EventKey.ActorSpawn, OnActorSpawn);
            Messenger.RemoveListener<int, int>(EventKey.GameStart, OnGameStart);

            ActorBase.onDie -= OnDie;

        }
        int spawnIndex = 0;
        Stack<UniTask> spawnTasks = new Stack<UniTask>();

        private void OnActorSpawn(ActorBase actor, bool active,int group)
        {
            Logger.Log("OnActorSpawn " + actor.gameObject.name + " " + actor.gameObject.GetInstanceID());
            if (!actor.IsUseHealBar)
            {
                return;
            }
            spawnTasks.Push(Spawn(actor,active,actor.GetCharacterType()==ECharacterType.Player));

            async UniTask Spawn(ActorBase actor, bool active, bool pernament)
            {
                HealthBarBase healthBar = null;

                healthBar=(await Pool.GameObjectSpawner.Instance.GetAsync(dictionary[actor.GetCharacterType()].RuntimeKey.ToString(),pernament?Pool.EPool.Pernament:Pool.EPool.Temporary)).GetComponent<HealthBarBase>();
                Logger.Log("->>>>>>SPAWN healthbar "+healthBar.gameObject.name+" "+actor.gameObject.name+" "+actor.gameObject.GetInstanceID());
                healthBar.SetUp(actor, actor.healthBarPlace.position - actor.GetPosition());
                healthBar.SetActive(active);
                healthBars.Add(actor, healthBar);
                Messenger.Broadcast(EventKey.OnHealthBarSpawn, healthBar, actor);
            }
        }
        private void OnDie(ActorBase actor, ActorBase attacker)
        {
            //Debug.Log("ACTOR die " + actor.gameObject.name);
            if (!healthBars.ContainsKey(actor)) return;
            HealthBarBase healthBar = healthBars[actor];
            healthBar.Hide();

            healthBars.Remove(actor);
        }

        private void OnGameStart(int dungeon, int stage)
        {
            
        }

        async UniTask SpawnHealthBarFromStack()
        {
            while (true)
            {
                while (spawnTasks.Count > 0)
                {
                    await spawnTasks.Pop();
                }
                await UniTask.DelayFrame(60,cancellationToken:cancellation.Token);
            }
        }


        public HealthBarBase Get(ActorBase actorBase)
        {
            if (!healthBars.ContainsKey(actorBase)) return null;
            return healthBars[actorBase];
        }
    }
}