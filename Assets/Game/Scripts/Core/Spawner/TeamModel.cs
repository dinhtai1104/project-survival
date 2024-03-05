using System;
using System.Collections.Generic;
using Engine;
using FrogunnerGames.Inspector;
using UnityEngine;

namespace Framework
{
    [Serializable]
    public class TeamModel
    {
        public static readonly TeamModel NullModel = new TeamModel(-1, LayerMask.NameToLayer("Default"));

        [SerializeField]
        private int _teamId;

        [SerializeField]
        private int _layer;

        [SerializeField]
        private LayerMask _allyLayerMask;

        [SerializeField]
        private LayerMask _enemyLayerMask;

        [SerializeField]
        private List<Actor> _allies;

        [SerializeField]
        private List<Actor> _enemies;

        private readonly HashSet<int> _enemyLookup;
        private readonly HashSet<int> _allyLookUp;

        public IList<Actor> Allies { get { return _allies; } }
        public IList<Actor> Enemies { get { return _enemies; } }

        public int TeamId { get { return _teamId; } }
        public int Layer { get { return _layer; } }
        public LayerMask EnemyLayerMask { get { return _enemyLayerMask; } }
        public LayerMask AllyLayerMask { get { return _allyLayerMask; } }

        public TeamModel(int teamId, int layer)
        {
            _teamId = teamId;
            _layer = layer;
            _allies = new List<Actor>();
            _enemies = new List<Actor>();
            _enemyLookup = new HashSet<int>();
            _allyLookUp = new HashSet<int>();

            AddAllyTeam(this);
        }

        public void AddEnemyTeam(TeamModel teamModel)
        {
            if (!_enemyLookup.Contains(teamModel.TeamId))
            {
                _enemyLookup.Add(teamModel.TeamId);
                _enemyLayerMask = _enemyLayerMask.AddLayer(teamModel.Layer);
            }
        }

        public void AddAllyTeam(TeamModel teamModel)
        {
            if (_allyLookUp.Contains(teamModel.TeamId)) return;
            _allyLookUp.Add(teamModel.TeamId);
            _allyLayerMask = _allyLayerMask.AddLayer(teamModel.Layer);
        }

        public void AddAlly(Actor actor)
        {
            _allies.Add(actor);
        }

        public void RemoveAlly(Actor actor)
        {
            _allies.Remove(actor);
        }

        public void AddEnemy(Actor actor)
        {
            _enemies.Add(actor);
        }

        public void RemoveEnemy(Actor actor)
        {
            _enemies.Remove(actor);
        }

        public bool IsEnemy(int teamId)
        {
            return _enemyLookup.Contains(teamId);
        }

        public bool IsAlly(int teamId)
        {
            return _allyLookUp.Contains(teamId);
        }
    }
}