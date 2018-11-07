using mytest2.Character;
using mytest2.Character.Abilities;
using mytest2.Character.Shield;
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
        public UIPlayerActionDirectionController UIAbilityDirectionPrefab;
        public UIJoystickCooldownController UIJoystickCooldownPrefab;
        public UIHealthBarController UIHealthBarPrefab;
        public UIHealthBarSegment UIHealthBarSegmentPrefab; 
        public AbilityProjectile[] AbilityProjectilePrefabs;
        public AbilityShield[] AbilityShieldPrefabs;

        private Dictionary<AbilityTypes, AbilityProjectile> m_AbilityProjectiles;
        private Dictionary<AbilityTypes, AbilityShield> m_AbilityShields;

        void Awake()
        {
            InitializeDictionaries();
        }

        void InitializeDictionaries()
        {
            m_AbilityProjectiles = new Dictionary<AbilityTypes, AbilityProjectile>();
            for (int i = 0; i < AbilityProjectilePrefabs.Length; i++)
            {
                if (!m_AbilityProjectiles.ContainsKey(AbilityProjectilePrefabs[i].Type))
                    m_AbilityProjectiles.Add(AbilityProjectilePrefabs[i].Type, AbilityProjectilePrefabs[i]);
            }

            m_AbilityShields = new Dictionary<AbilityTypes, AbilityShield>();
            for (int i = 0; i < AbilityShieldPrefabs.Length; i++)
            {
                if (!m_AbilityShields.ContainsKey(AbilityShieldPrefabs[i].Type))
                    m_AbilityShields.Add(AbilityShieldPrefabs[i].Type, AbilityShieldPrefabs[i]);
            }
        }


        public Projectile GetAbilityProjectilePrefab(AbilityTypes type)
        {
            if (m_AbilityProjectiles.ContainsKey(type))
                return m_AbilityProjectiles[type].ProjectilePrefab;

            return null;
        }

        public ShieldObject GetAbilityShieldPrefab(AbilityTypes type)
        {
            if (m_AbilityShields.ContainsKey(type))
                return m_AbilityShields[type].ProjectilePrefab;

            return null;
        }


        [System.Serializable]
        public class AbilityProjectile
        {
            public AbilityTypes Type;
            public Projectile ProjectilePrefab;
        }
        [System.Serializable]
        public class AbilityShield
        {
            public AbilityTypes Type;
            public ShieldObject ProjectilePrefab;
        }


    }
}
