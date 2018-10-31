using System.Collections.Generic;
using UnityEngine;

namespace mytest2.Character.Abilities
{
    /// <summary>
    /// Контроллер способностей персонажа (задает какими способностями персонаж может владеть и их использование)
    /// </summary>
    public class CreatureAbilityController : MonoBehaviour
    {
        [Tooltip("Доступные способности")]
        public AbilityTypes[] Abilities;

        private Dictionary<AbilityTypes, CreatureAbility> m_Abilities;

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
        /// Использовать способность
        /// </summary>
        /// <param name="type">Тип способности</param>
        /// <param name="dir">Направление способности</param>
        /// <returns>true если способность была применена</returns>
        public bool UseAbility(AbilityTypes type, Vector2 dir)
        {
            if (m_Abilities.ContainsKey(type))
            {
                CreatureAbility ability = m_Abilities[type];
                if (ability.HasAmmo && !ability.IsCooldown) //Если у способности есть заряды и она не в откате
                {
                    ability.Use();

                    //TODO: Create effect

                    DataAbility abilityData = GameManager.Instance.GameState.DataTableAbilities.GetAbilityData(type);
                    Debug.Log("Using ability: " + type + " Dir: " + dir + " " + abilityData.Damage + " " +
                                                                                abilityData.CooldownMiliseconds + " " +
                                                                                abilityData.Stamina);

                    return true;
                }
                else if (!ability.HasAmmo)
                    Debug.LogWarning("No ammo");
                else 
                    Debug.LogWarning("Is in cooldown");
            }

            return false;
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
    }

    /// <summary>
    /// Способность, которая принадлежит персонажу
    /// </summary>
    public class CreatureAbility
    {
        private int m_Ammo;
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
            get { return m_Ammo > 0; }
        }

        public CreatureAbility(AbilityTypes type)
        {
            Type = type;
            m_Ammo = 2;
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
            m_Ammo += ammoAmount;
        }


        void Cooldown()
        {
            m_UseTime = Time.time;
        }

        void ReduceAmmo()
        {
            m_Ammo -= 1;
            if (m_Ammo < 0)
                m_Ammo = 0;
        }
    }
}
