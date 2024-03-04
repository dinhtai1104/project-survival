using FrogunnerGames.Inspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TypeReferences;
using UnityEngine;

namespace SceneManger
{
    [CreateAssetMenu(fileName = "SceneData", menuName = "ScriptableObjects/Scene Data")]
    public class SceneData : ScriptableObject
    {
        [SerializeField, SceneSelection]
        private string _sceneName;

        [SerializeField, ClassExtends(typeof(BaseSceneController))]
        private ClassTypeReference _sceneType;

        [SerializeField]
        private GameObject _loadingScenePrefab;

        [SerializeField]
        private GameObject _enterTransitionPrefab;

        [SerializeField]
        private GameObject _exitTransitionPrefab;

        public string SceneName { get { return _sceneName; } }
        public Type SceneType { get { return _sceneType; } }
        public GameObject LoadingScenePrefab { get { return _loadingScenePrefab; } }
        public GameObject EnterTransitionPrefab { get { return _enterTransitionPrefab; } }
        public GameObject ExitTransitionPrefab { get { return _exitTransitionPrefab; } }
    }
}
