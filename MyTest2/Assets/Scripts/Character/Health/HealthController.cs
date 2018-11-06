using mytest2.Character.Abilities;
using mytest2.UI.Controllers3D;
using mytest2.Utils.Pool;
using System.Collections.Generic;
using UnityEngine;

namespace mytest2.Character.Health
{
    public class HealthController : MonoBehaviour
    {
        public System.Action<AbilityTypes, int> OnTakeDamage;
        public System.Action OnDestroy;
        public System.Action<AbilityTypes> OnWrongAbility;

        public Transform HealthBarSpawnPoint;
        public HealthSegment[] HealthData;

        private UIHealthBarController m_UIHealthBarController;
        private Dictionary<AbilityTypes, HealthSegment> m_HealthData;

        public void Init()
        {
            //Инициализировать данные
            m_HealthData = new Dictionary<AbilityTypes, HealthSegment>();
            for (int i = 0; i < HealthData.Length; i++)
            {
                if (!m_HealthData.ContainsKey(HealthData[i].Type))
                {
                    HealthData[i].Init();
                    m_HealthData.Add(HealthData[i].Type, HealthData[i]);
                }
            }

            //Инициализировать UI
            m_UIHealthBarController = PoolManager.GetObject(GameManager.Instance.PrefabLibrary.UIHealthBarPrefab) as UIHealthBarController;
            m_UIHealthBarController.transform.position = HealthBarSpawnPoint.position;
            m_UIHealthBarController.Init(transform, m_HealthData);
        }

        /// <summary>
        /// Нанести урон персонажу
        /// </summary>
        public void TakeDamage(AbilityTypes type, int damage)
        {
            HealthSegment healthSegment = GetSegmentForTakeDamage(type);
            if (healthSegment != null)
            {
                //Нанести  урон
                healthSegment.TakeDamage(damage);

                //Обновить UI
                m_UIHealthBarController.UpdateUI(type, healthSegment.CurHealth);

                //Персонаж уничтожен
                if (CreatureIsDestroyed())
                {
                    if (OnDestroy != null)
                        OnDestroy();
                }
                else //Нанесен урон
                {
                    if (OnTakeDamage != null)
                        OnTakeDamage(healthSegment.Type, healthSegment.CurHealth);
                }
            }
            else //Урон не подходящей способностью
            {
                if (OnWrongAbility != null)
                    OnWrongAbility(type);
            }
        }

        /// <summary>
        /// Получить сегмент хп, которому наноситься урон
        /// </summary>
        HealthSegment GetSegmentForTakeDamage(AbilityTypes type)
        {
            if (m_HealthData.ContainsKey(AbilityTypes.None))
                return m_HealthData[AbilityTypes.None];
            else if (m_HealthData.ContainsKey(type))
                return m_HealthData[type];

            return null;
        }

        bool CreatureIsDestroyed()
        {
            int emptySegments = 0;
            foreach (HealthSegment s in m_HealthData.Values)
            {
                if (s.IsEmpty)
                    emptySegments++;
            }

            return emptySegments == m_HealthData.Count;
        }

        /// <summary>
        /// Представление одного сегмента хп
        /// </summary>
        [System.Serializable]
        public class HealthSegment
        {
            public AbilityTypes Type;
            public int Health;

            public int CurHealth
            { get; private set; }
            public bool IsEmpty
            {
                get { return CurHealth <= 0; }  
            }

            public void Init()
            {
                CurHealth = Health;
            }

            public void TakeDamage(int damage)
            {
                CurHealth -= damage;

                if (CurHealth < 0)
                    CurHealth = 0;
            }
        }
    }
}
