using mytest2.Character.Abilities;
using System.Collections.Generic;
using UnityEngine;

namespace mytest2.Main
{
    public class AbilityColorController : MonoBehaviour
    {
        public AbilityColor[] AbilityColors;

        private Dictionary<AbilityTypes, AbilityColor> m_AbilityColors;

        void Start()
        {
            m_AbilityColors = new Dictionary<AbilityTypes, AbilityColor>();
            for (int i = 0; i < AbilityColors.Length; i++)
            {
                if (!m_AbilityColors.ContainsKey(AbilityColors[i].Type))
                    m_AbilityColors.Add(AbilityColors[i].Type, AbilityColors[i]);
            }
        }

        public Color GetAbilityColor(AbilityTypes type)
        {
            if (m_AbilityColors.ContainsKey(type))
                return m_AbilityColors[type].Color;

            return Color.white;
        }

        [System.Serializable]
        public struct AbilityColor
        {
            public AbilityTypes Type;
            public Color Color;
        }
    }
}
