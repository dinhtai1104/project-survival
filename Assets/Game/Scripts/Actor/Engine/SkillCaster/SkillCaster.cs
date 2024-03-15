using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Engine
{
    public class SkillCaster : MonoBehaviour, ISkillCaster
    {
        [ShowInInspector, ReadOnly] private List<ISkill> m_ActiveSkills;
        [ShowInInspector, ReadOnly] private ISkill[] m_AllSkills;
        private ISkill m_CurrentSkill;
        private Dictionary<int, ISkill> m_AllSkillDict;
        [ShowInInspector, ReadOnly] private Queue<ISkill> m_SkillQueue;
        [ShowInInspector, ReadOnly] private List<ISkill> m_AvailableSkills;

        public bool IsLocked { get; set; }

        public ActorBase Owner { get; private set; }

        public ISkill CurrentSkill
        {
            get { return m_CurrentSkill; }
        }

        public IEnumerable<ISkill> ActiveSkills
        {
            get { return m_ActiveSkills; }
        }

        public IEnumerable<ISkill> AllSkills
        {
            get { return m_AllSkills; }
        }

        public bool HasWaitingSkills
        {
            get { return m_SkillQueue.Count > 0; }
        }

        public bool HasAvailableSkill
        {
            get { return m_AvailableSkills.Count > 0; }
        }

        public bool IsBusy
        {
            get
            {
                if (m_SkillQueue.Count > 0) return true;

                foreach (var skill in m_AllSkills)
                {
                    if (skill.IsExecuting && skill.LockOtherSkill && !skill.Interruptible)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        public bool IsExecuting
        {
            get
            {
                foreach (var skill in m_AllSkills)
                {
                    if (skill.IsExecuting)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        public void Init(ActorBase actor)
        {
            Owner = actor;
            m_SkillQueue = new Queue<ISkill>();
            m_AllSkills = GetComponentsInChildren<ISkill>();
            m_AllSkillDict = new Dictionary<int, ISkill>(6);
            m_ActiveSkills = new List<ISkill>(3);
            m_AvailableSkills = new List<ISkill>(3);

            for (int i = 0; i < transform.childCount; ++i)
            {
                Transform child = transform.GetChild(i);

                if (!child.gameObject.activeInHierarchy) continue;

                var skill = child.GetComponent<ISkill>();

                if (skill != null)
                {
                    if (!skill.AutoCast && !skill.IsLocked)
                    {
                        m_ActiveSkills.Add(skill);
                    }

                    m_AllSkillDict.Add(skill.Id, skill);
                }
            }

            foreach (var skill in m_AllSkills)
            {
                skill.Init(Owner);
            }
        }

        public void OnUpdate()
        {
            for (int i = 0; i < m_AllSkills.Length; ++i)
            {
                m_AllSkills[i].OnUpdate();
            }

            if (IsLocked) return;

            UpdateAvailableSkills();

            if (m_CurrentSkill != null)
            {
                if (!m_CurrentSkill.IsExecuting)
                {
                    m_CurrentSkill = null;
                }
            }
            else
            {
                if (m_SkillQueue.Count <= 0)
                {
                    return;
                }

                m_CurrentSkill = m_SkillQueue.Dequeue();

                if (m_CurrentSkill != null)
                {
                    m_CurrentSkill.Cast();
                }
            }
        }

        public ISkill GetSkillById(int id)
        {
            return m_AllSkillDict.ContainsKey(id) ? m_AllSkillDict[id] : null;
        }

        public bool AddSkill(ISkill skill)
        {
            return false;
        }

        public bool RemoveSkill(ISkill skill)
        {
            return false;
        }

        public void UpdateAvailableSkills()
        {
            if (m_ActiveSkills != null)
            {
                for (int i = 0; i < m_ActiveSkills.Count; ++i)
                {
                    ISkill skill = m_ActiveSkills[i];

                    if (skill.CanCast && !m_AvailableSkills.Contains(skill))
                    {
                        // if (Owner.AI) {
                        if (!skill.Ignore)
                        {
                            m_AvailableSkills.Add(skill);
                        }

                        // } else {
                        //     m_AvailableSkills.Add(skill);
                        // }
                    }
                }
            }

            if (m_AvailableSkills != null)
            {
                for (int i = m_AvailableSkills.Count - 1; i >= 0; --i)
                {
                    ISkill skill = m_AvailableSkills[i];
                    if (!skill.CanCast || skill.Ignore)
                    {
                        m_AvailableSkills.RemoveAt(i);
                    }
                }
            }
        }

        [Button]
        public bool CastSkillById(int id)
        {
            if (!m_AllSkillDict.ContainsKey(id))
            {
                return false;
            }

            ISkill skill = m_AllSkillDict[id];

            if (!skill.CanCast)
            {
                return false;
            }

            if (m_SkillQueue.Contains(skill))
            {
                return false;
            }

            if (m_CurrentSkill != null)
            {
                if (m_CurrentSkill.Id == -1 && skill.Id != -1)
                {
                    InterruptCurrentSkill();
                }
                else
                {
                    if (m_CurrentSkill.GetType() == typeof(MultiTaskSkill) && skill.Priority < m_CurrentSkill.Priority)
                    {
                        var multiTaskSkill = m_CurrentSkill as MultiTaskSkill;
                        if (multiTaskSkill != null)
                        {
                            InterruptCurrentSkill();
                        }
                    }
                }
            }

            if (IsBusy)
            {
                return false;
            }

            //GameCore.Event.Fire(this, RequestCastSkillEventArgs.Create(Owner, skill));
            m_SkillQueue.Enqueue(skill);
            return true;
        }

        public void ForceCastSkillById(int id)
        {
            if (!m_AllSkillDict.ContainsKey(id))
            {
                return;
            }

            ISkill skill = m_AllSkillDict[id];

            if (skill.IsLocked)
            {
                return;
            }

            //GameCore.Event.Fire(this, RequestCastSkillEventArgs.Create(Owner, skill));
            m_SkillQueue.Enqueue(skill);
        }

        public void SetLockSkill(int id, bool isLocked)
        {
            if (m_AllSkillDict.ContainsKey(id))
            {
                m_AllSkillDict[id].IsLocked = isLocked;
            }
            else
            {
                Debug.Log(gameObject.name + " SetLockSkill id does not exists " + id);
            }
        }

        public bool HasSkillId(int id)
        {
            return m_AllSkillDict.ContainsKey(id);
        }

        public void InterruptCurrentSkill()
        {
            if (m_CurrentSkill == null || !m_CurrentSkill.Interruptible) return;
            m_CurrentSkill.Stop();
            m_CurrentSkill = null;
        }

        public void InterruptAllSkills()
        {
            m_SkillQueue.Clear();
            InterruptCurrentSkill();
        }

        public void ResetAllSkills()
        {
            foreach (var skill in m_AllSkills)
            {
                skill.Reset();
            }
        }

        public bool CastRandomAvailableSkill()
        {
            if (IsLocked || IsBusy || m_ActiveSkills.Count == 0 || m_AvailableSkills.Count == 0)
            {
                return false;
            }

            ISkill skill = null;

            if (m_AvailableSkills.Count > 1)
            {
                int index = Random.Range(0, m_AvailableSkills.Count);
                skill = m_AvailableSkills[index];
            }
            else
            {
                skill = m_AvailableSkills[0];
            }

            return CastSkillById(skill.Id);
        }
    }
}