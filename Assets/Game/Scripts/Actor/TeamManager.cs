using Engine;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// Manage team members and relationship between them
    /// </summary>
    [System.Serializable]
    public class TeamManager
    {
        private Dictionary<int, TeamModel> _teams;

#if UNITY_EDITOR
        [SerializeField] private List<TeamModel> _teamModels = new List<TeamModel>();
#endif

        public TeamManager()
        {
            _teams = new Dictionary<int, TeamModel>();
        }

        public void AddTeam(TeamModel teamModel)
        {
            _teams.Add(teamModel.TeamId, teamModel);

#if UNITY_EDITOR
            _teamModels.Add(teamModel);
#endif
        }

        public TeamModel GetTeamModel(int teamId)
        {
            if (_teams.ContainsKey(teamId))
            {
                return _teams[teamId];
            }

            return TeamModel.NullModel;
        }

        public void AddActor(int teamId, ActorBase actor)
        {
            if (!_teams.ContainsKey(teamId))
            {
                Debug.LogError("Team does not exist " + teamId);
                return;
            }

            // Add relationship
            foreach (var team in _teams.Values)
            {
                if (team.IsEnemy(teamId))
                {
                    team.AddEnemy(actor);
                }
                else if (team.IsAlly(teamId))
                {
                    team.AddAlly(actor);
                }
            }
        }

        public void RemoveActor(int teamId, ActorBase actor)
        {
            if (!_teams.ContainsKey(teamId))
            {
                Debug.LogError("Team does not exist " + teamId);
                return;
            }

            // Remove relationship
            foreach (var team in _teams.Values)
            {
                if (team.IsEnemy(teamId))
                {
                    team.RemoveEnemy(actor);
                }
                else if (team.IsAlly(teamId))
                {
                    team.RemoveAlly(actor);
                }
            }
        }
    }
}