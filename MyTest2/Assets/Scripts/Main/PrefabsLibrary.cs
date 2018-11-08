using mytest2.Character;
using mytest2.Character.Abilities;
using mytest2.Character.Shield;
using mytest2.Items;
using mytest2.Projectiles;
using mytest2.UI.Controllers;
using mytest2.UI.Controllers3D;
using System.Collections.Generic;
using UnityEngine;

namespace mytest2.Main
{
    public class PrefabsLibrary : MonoBehaviour
    {
        public CreatureController PlayerPrefab;
        public CreatureController EnemyPrefab;
        //UI Prefabs
        public UIPlayerActionDirectionController UIAbilityDirectionPrefab;
        public UIJoystickCooldownController UIJoystickCooldownPrefab;
        public UIHealthBarController UIHealthBarPrefab;
        public UIHealthBarSegment UIHealthBarSegmentPrefab; 
        //Ability prefabs
        public AbilityProjectilePrefab[] AbilityProjectilePrefabs;
        public AbilityShieldPrefab[] AbilityShieldPrefabs;
        public AbilityItemPrefab[] AbilityItemPrefabs;

        private Dictionary<AbilityTypes, AbilityProjectilePrefab> m_AbilityProjectiles;
        private Dictionary<AbilityTypes, AbilityShieldPrefab> m_AbilityShields;
        private Dictionary<AbilityTypes, AbilityItemPrefab> m_AbilityItems;

        void Awake()
        {
            InitializeDictionaries();
        }

        void InitializeDictionaries()
        {
            //Projectiles
            m_AbilityProjectiles = new Dictionary<AbilityTypes, AbilityProjectilePrefab>();
            for (int i = 0; i < AbilityProjectilePrefabs.Length; i++)
            {
                if (!m_AbilityProjectiles.ContainsKey(AbilityProjectilePrefabs[i].Type))
                    m_AbilityProjectiles.Add(AbilityProjectilePrefabs[i].Type, AbilityProjectilePrefabs[i]);
            }

            //Shields
            m_AbilityShields = new Dictionary<AbilityTypes, AbilityShieldPrefab>();
            for (int i = 0; i < AbilityShieldPrefabs.Length; i++)
            {
                if (!m_AbilityShields.ContainsKey(AbilityShieldPrefabs[i].Type))
                    m_AbilityShields.Add(AbilityShieldPrefabs[i].Type, AbilityShieldPrefabs[i]);
            }

            //Items
            m_AbilityItems = new Dictionary<AbilityTypes, AbilityItemPrefab>();
            for (int i = 0; i < AbilityItemPrefabs.Length; i++)
            {
                if (!m_AbilityItems.ContainsKey(AbilityItemPrefabs[i].Type))
                    m_AbilityItems.Add(AbilityItemPrefabs[i].Type, AbilityItemPrefabs[i]);
            }
        }


        public Projectile GetAbilityProjectilePrefab(AbilityTypes type)
        {
            if (m_AbilityProjectiles.ContainsKey(type))
                return m_AbilityProjectiles[type].Prefab;

            return null;
        }
        public ShieldObject GetAbilityShieldPrefab(AbilityTypes type)
        {
            if (m_AbilityShields.ContainsKey(type))
                return m_AbilityShields[type].Prefab;

            return null;
        }
        public Item GetAbilityItemPrefab(AbilityTypes type)
        {
            if (m_AbilityItems.ContainsKey(type))
                return m_AbilityItems[type].Prefab;

            return null;
        }

        [System.Serializable]
        public class AbilityProjectilePrefab
        {
            public AbilityTypes Type;
            public Projectile Prefab;
        }
        [System.Serializable]
        public class AbilityShieldPrefab
        {
            public AbilityTypes Type;
            public ShieldObject Prefab;
        }
        [System.Serializable]
        public class AbilityItemPrefab
        {
            public AbilityTypes Type;
            public Item Prefab;
        }
    }
}
