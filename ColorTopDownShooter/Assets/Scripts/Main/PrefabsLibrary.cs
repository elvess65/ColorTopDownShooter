using mytest2.Character;
using mytest2.Character.Abilities;
using mytest2.Character.Shield;
using mytest2.Items;
using mytest2.Projectiles;
using mytest2.UI.Controllers;
using mytest2.UI.Controllers3D;
using mytest2.Utils.Pool;
using System.Collections.Generic;
using UnityEngine;

namespace mytest2.Main
{
    public class PrefabsLibrary : MonoBehaviour
    {
        public CreatureController PlayerPrefab;
        public CreatureController EnemyPrefab;
		[Header("UI")]
        public UIPlayerActionDirectionController UIAbilityDirectionPrefab;
        public UICooldownController UIJoystickCooldownPrefab;
        public UICooldownController UIKeyboardCooldownPrefab;
        public UIHealthBarController UIHealthBarPrefab;
        public UIHealthBarSegment UIHealthBarSegmentPrefab;
        public PoolObject UIShieldRadiusPrefab;
		[Header("Other")]
		public ShieldVisuals ShieldVisualsPrefab;
		[Header("Abilities")]
        public AbilityProjectilePrefab[] AbilityProjectilePrefabs;
        public AbilityShieldParticlePrefab[] AbilityShieldParticlePrefabs;
        public AbilityItemPrefab[] AbilityItemPrefabs;

        private Dictionary<AbilityTypes, AbilityProjectilePrefab> m_AbilityProjectiles;
        private Dictionary<AbilityTypes, AbilityShieldParticlePrefab> m_AbilityShieldParticles;
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
            m_AbilityShieldParticles = new Dictionary<AbilityTypes, AbilityShieldParticlePrefab>();
            for (int i = 0; i < AbilityShieldParticlePrefabs.Length; i++)
            {
                if (!m_AbilityShieldParticles.ContainsKey(AbilityShieldParticlePrefabs[i].Type))
                    m_AbilityShieldParticles.Add(AbilityShieldParticlePrefabs[i].Type, AbilityShieldParticlePrefabs[i]);
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
		public ShieldVisualsParticle GetAbilityShieldParticlePrefab(AbilityTypes type)
        {
			if (m_AbilityShieldParticles.ContainsKey(type))
                return m_AbilityShieldParticles[type].Prefab;

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
        public class AbilityShieldParticlePrefab
        {
            public AbilityTypes Type;
			public ShieldVisualsParticle Prefab;
        }
        [System.Serializable]
        public class AbilityItemPrefab
        {
            public AbilityTypes Type;
            public Item Prefab;
        }
    }
}
