using UnityEngine;

namespace mytest2.Character.Abilities
{
    /// <summary>
    /// Отвечает за текущую выделенную способность
    /// </summary>
    public class SelectedAbilityController : MonoBehaviour
    {
        private AbilityTypes m_CurAbilityType = AbilityTypes.None;

        public AbilityTypes CurAbilityType
        {
            get { return m_CurAbilityType; }
        }

        public void SelectAbility(AbilityTypes type)
        {
            if (m_CurAbilityType != type)
            {
                m_CurAbilityType = type;
                GameManager.Instance.UIManager.SelectAbilityJoystick(type);
            }
        }
    }
}
