using mytest2.Character;
using mytest2.Character.Abilities;
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
        public UIPlayerActionDirectionController UIAbilityDirectionPrefab;
        public UIJoystickCooldownController UIJoystickCooldownPrefab;
        public UIHealthBarController UIHealthBarPrefab;
        public UIHealthBarSegment UIHealthBarSegmentPrefab; 
        public AbilityProjectile[] AbilityProjectilePrefabs;

        private Dictionary<AbilityTypes, AbilityProjectile> m_AbilityProjectiles;

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
        }


        public Projectile GetAbilityProjectilePrefab(AbilityTypes type)
        {
            if (m_AbilityProjectiles.ContainsKey(type))
                return m_AbilityProjectiles[type].ProjectilePrefab;

            return null;
        }


        [System.Serializable]
        public class AbilityProjectile
        {
            public AbilityTypes Type;
            public Projectile ProjectilePrefab;
        }

    }
}
