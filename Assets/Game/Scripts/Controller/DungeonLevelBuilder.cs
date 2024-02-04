using com.assets.loader.addressables;
using com.assets.loader.core;
using Cysharp.Threading.Tasks;
using Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Game.Level
{
    public class DungeonLevelBuilder : LevelBuilderBase
    {
        Map map;
        BackGround backGround;
        Portal portal;

        public override GroupNpcSpawn GetNpcSpawns()
        {
            return map.groupNpcSpawn;
        }

        public override void Destroy()
        {
            if(map!=null)
                map?.gameObject?.SetActive(false);
            if(backGround!=null)
                backGround?.gameObject?.SetActive(false);
            if(portal!=null)    
               portal?.gameObject?.SetActive(false);

         
        }
        public override async UniTask<Map> SetUp(string mapId)
        {

         

            map = (await Game.Pool.GameObjectSpawner.Instance.GetAsync(mapId)).GetComponent<Map>();
            map.gameObject.SetActive(true);
         
            return map;
        }
        public override async UniTask<BackGround> SetUpBackGround(string backGroundId)
        {
            backGround = (await Game.Pool.GameObjectSpawner.Instance.GetAsync( backGroundId)).GetComponent<BackGround>();
            backGround.gameObject.SetActive(true);
            return backGround;
        }
        public override async UniTask<Portal> SetUpPortal(bool isBuffPortal)
        {
            portal = (await Game.Pool.GameObjectSpawner.Instance.GetAsync(isBuffPortal ? "BuffPortal" : "NormalPortal")).GetComponent<Portal>();
            var hit = Physics2D.RaycastAll(map.portalPoint.position + Vector3.up, Vector3.down, 4, layerMask: LayerMask.GetMask("Ground"));
            foreach(var h in hit)
            {
                if (h.collider.CompareTag("Ground"))
                {
                    portal.transform.position = h.point;
                    break;
                }
            }

            portal.gameObject.SetActive(true);
            return portal;
        }
        public override Map CurrentMap()
        {
            return map;
        }
    }
}