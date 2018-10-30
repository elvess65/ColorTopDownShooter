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

        public void UseAbility(AbilityTypes type, Vector2 dir)
        {
            if (m_Abilities.ContainsKey(type))
            {
                CreatureAbility ability = m_Abilities[type];
                ability.ReduceAmount();

                DataAbility abilityData = GameManager.Instance.GameState.DataTableAbilities.GetAbilityData(type);
                Debug.Log("Using ability: " + type + " Dir: " + dir + " " + abilityData.Damage + " " +
                                                                            abilityData.CooldownMiliseconds + " " +
                                                                            abilityData.Stamina);
            }
        }

        public void Add(AbilityTypes type)
        {
            if (m_Abilities.ContainsKey(type))
            {
                CreatureAbility ability = m_Abilities[type];
                ability.AddAmount();
            }
        }
    }

    /// <summary>
    /// Информация о способности, которая принадлежит персонажу
    /// </summary>
    public class CreatureAbility
    {
        private AbilityTypes m_Type;
        private int m_Amount;

        public AbilityTypes Type
        {
            get { return m_Type; }
        }

        public CreatureAbility(AbilityTypes type)
        {
            m_Type = type;
            m_Amount = 0;
        }

        public void ReduceAmount()
        {
            m_Amount -= 1;
            if (m_Amount < 0)
                m_Amount = 0;
        }

        public void AddAmount()
        {
            m_Amount++;
        }
    }
}
