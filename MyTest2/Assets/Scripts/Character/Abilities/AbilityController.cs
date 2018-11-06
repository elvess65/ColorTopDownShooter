using mytest2.Projectiles;
using mytest2.Utils.Pool;
using System.Collections.Generic;
using UnityEngine;

namespace mytest2.Character.Abilities
{
    /// <summary>
    /// Контроллер способностей персонажа (задает какими способностями персонаж может владеть и их использование)
    /// </summary>
    public class AbilityController : MonoBehaviour
    {
        public System.Action<AbilityTypes> OnAbilityUse; 
        public System.Action<AbilityTypes, int> OnUpdateAmmo;

        public Transform AbilitySpawnPoint;
        
        [Tooltip("Доступные способности")]
        public AbilityTypes[] Abilities;

        private Dictionary<AbilityTypes, CreatureAbility> m_Abilities;
		public static int ABILITY_CODE_COOLDOWN = -1;
		public static int ABILITY_CODE_NO_AMMO = -2;
		public static int ABILITY_CAN_USE = 0;

        public void Init()
        {
            m_Abilities = new Dictionary<AbilityTypes, CreatureAbility>();
            for (int i = 0; i < Abilities.Length; i++)
            {
                if (!m_Abilities.ContainsKey(Abilities[i]))
                {
                    CreatureAbility ability = new CreatureAbility(Abilities[i]);
                    m_Abilities.Add(Abilities[i], ability);
                }
            }
        }

        /// <summary>
        /// Задать всем способностям событие для обновления зарядов и вызвать это событие (вызываеться на старте игроком - обновить UI)
        /// </summary>
        public void SetAndCallUpdateAmmoEventForAllAbilities()
        {
            foreach (CreatureAbility ability in m_Abilities.Values)
            {
                ability.OnUpdateAmmo += OnUpdateAmmo;
                OnUpdateAmmo(ability.Type, ability.AmmoAmmount);
            }
        }

        /// <summary>
        /// Использовать способность
        /// </summary>
        /// <param name="type">Тип способности</param>
        /// <param name="dir">Направление способности</param>
        /// <returns>true если способность была применена</returns>
        public void TryUseAbility(AbilityTypes type, Vector2 dir)
        {
            //Если у персонажа есть способность
            if (m_Abilities.ContainsKey(type))
            {
                CreatureAbility ability = m_Abilities[type];
			
				int abilityCode = CanUseABility (ability);
				if (abilityCode == ABILITY_CAN_USE) 
				{
					ability.Use ();

					LaunchProjectile (type, dir);

					if (OnAbilityUse != null)
						OnAbilityUse (type);
				} 
				else 
				{
					Debug.Log ("CANT USE ABILITY. REASON " + abilityCode);
				}
            }
        }

        /// <summary>
        /// Добавить определенное количество зарядов конкретной способности
        /// </summary>
        /// <param name="type">Тип способности</param>
        /// <param name="ammoAmount">Количество зарядов</param>
        public void AddAmmo(AbilityTypes type, int ammoAmount)
        {
            if (m_Abilities.ContainsKey(type))
                m_Abilities[type].AddAmmo(ammoAmount);
        }

		/// <summary>
		/// Можно ли использовать способность (хватает патронов и способность не в откате)
		/// </summary>
		public bool CanUseABility(AbilityTypes type)
		{
			//Если у персонажа есть способность
			if (m_Abilities.ContainsKey (type)) 
				return CanUseABility (m_Abilities [type]) == ABILITY_CAN_USE;

			return false;
		}


		/// <summary>
		/// Внутреняя функция проверки возможности использовать способность с получением типов причин отказа
		/// </summary>
		int CanUseABility(CreatureAbility ability)
		{
			//Если у способности нет зарядов и она не в откате
			if (!ability.HasAmmo) 
			{
				Debug.LogWarning ("No ammo");
				return ABILITY_CODE_NO_AMMO;
			} 
			else if (ability.IsCooldown) //Если способность в откате
			{
				Debug.LogWarning ("Is in cooldown");
				return ABILITY_CODE_COOLDOWN;
			}

			return ABILITY_CAN_USE;;
		}

		/// <summary>
		/// Заупск снаряда способности
		/// </summary>
		/// <param name="type">Type.</param>
		/// <param name="dir">Dir.</param>
        void LaunchProjectile(AbilityTypes type, Vector2 dir)
        {
            Projectile projectile = PoolManager.GetObject(GameManager.Instance.PrefabLibrary.GetAbilityProjectilePrefab(type)) as Projectile;
            projectile.transform.position = AbilitySpawnPoint.position;
            projectile.Launch(dir, type);
        }
    }

    /// <summary>
    /// Способность, которая принадлежит персонажу
    /// </summary>
    public class CreatureAbility
    {
        public System.Action<AbilityTypes, int> OnUpdateAmmo;
        public System.Action<AbilityTypes> OnCooldown;

        private float m_UseTime = float.NegativeInfinity;

        public AbilityTypes Type
        {
            get; private set;
        }
        public bool IsCooldown
        {
            get
            {
                float leftTimeSeconds = Time.time - m_UseTime;
                return (leftTimeSeconds * 1000) < GameManager.Instance.GameState.DataTableAbilities.GetAbilityData(Type).CooldownMiliseconds;
            }
        }
        public bool HasAmmo
        {
            get { return AmmoAmmount > 0; }
        }
        public int AmmoAmmount
        {
            get; private set;
        }

        public CreatureAbility(AbilityTypes type)
        {
            Type = type;
            AmmoAmmount = 5;
        }

        /// <summary>
        /// Использовать способность (уменьшить количество зарядов и начат откат)
        /// </summary>
        public void Use()
        {
            ReduceAmmo();
            Cooldown();
        }

        /// <summary>
        /// Добавить определенное количество зарядов
        /// </summary>
        /// <param name="ammoAmount">Количество зарядов</param>
        public void AddAmmo(int ammoAmount)
        {
            AmmoAmmount += ammoAmount;

            if (OnUpdateAmmo != null)
                OnUpdateAmmo(Type, AmmoAmmount);
        }


        void Cooldown()
        {
            m_UseTime = Time.time;

            if (OnCooldown != null)
                OnCooldown(Type);
        }

        void ReduceAmmo()
        {
            AmmoAmmount -= 1;
            if (AmmoAmmount < 0)
                AmmoAmmount = 0;

            if (OnUpdateAmmo != null)
                OnUpdateAmmo(Type, AmmoAmmount);
        }
    }
}
