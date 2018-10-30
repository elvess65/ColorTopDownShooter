using UnityEngine;

namespace mytest2.Character.Abilities
{
    /// <summary>
    /// Таблица с параметрами способностей
    /// </summary>
    public class DataTableAbilities : MonoBehaviour
    {
        public DataAbility[] AbilitiesData;

        public DataAbility GetAbilityData(AbilityTypes type)
        {
            DataAbility result = AbilitiesData[0];
            for (int i = 0; i < AbilitiesData.Length; i++)
            {
                if (AbilitiesData[i].Type.Equals(type))
                    result = AbilitiesData[i];
            }

            return result;
        }
    }

    public enum AbilityTypes
    {
        Red,
        Green,
        Blue,
        Yellow,
        Violet,
        None
    }

    /// <summary>
    /// Контейнер с данными конкретной способности
    /// </summary>
    [System.Serializable]
    public struct DataAbility
    {
        public AbilityTypes Type;
        public int Damage;
        public int Stamina;
        public float CooldownMiliseconds;
    }
}
