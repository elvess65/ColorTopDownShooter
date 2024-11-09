using mytest2.Character.Abilities;
using UnityEngine;
using UnityEngine.UI;

namespace mytest2.UI.InputSystem
{
    /// <summary>
    /// Надстройка над джойстиком способностей
    /// </summary>
    public class AbilityVirtualJoystickWrapper : VirtualJoystickWrapper
    {
        public System.Action<AbilityTypes> OnAbilityActivate;
        public System.Action<AbilityTypes> OnAbilitySelect;

        public AbilityTypes AbilityType;
        public Text Text_AbilityAmmo;

        private bool m_AbilityIsActivated = false;
        private bool m_AbilitySelected = false;
        private Vector3 m_TouchStartMousePos;
        private const float m_SQR_DISTANCE_TO_USE_ABILITY = 200;


        public void UpdateAbilityAmmo(int ammoAmount)
        {
            Text_AbilityAmmo.text = ammoAmount.ToString();
        }


        protected override void HandleTouchStart()
        {
            base.HandleTouchStart();

            //Являеться ли способность этого джойстика выделенной
            m_AbilitySelected = AbilityType == GameManager.Instance.GameState.Player.SelectedAbility;
            m_AbilityIsActivated = false;

            m_TouchStartMousePos = Input.mousePosition;
        }

        protected override void HandleDrag()
        {
            base.HandleDrag();

            //Если дистанция, которую прошел джойстик достаточна
            if (MouseDeltaIsEnough())
            {
                //Если способность этого джойстика не выделенна
                if (!m_AbilitySelected)
                {
                    m_AbilitySelected = true;

                    if (OnAbilitySelect != null)
                        OnAbilitySelect(AbilityType);
                }

                if (!m_AbilityIsActivated)
                {
                    m_AbilityIsActivated = true;

                    if (OnAbilityActivate != null)
                        OnAbilityActivate(AbilityType);
                }
            }
        }

        protected override void HandleTouchEnd()
        {
            //Если не достаточн дистанции для совершения дейтсвия - обнулить направление и выделить способность
            if (!MouseDeltaIsEnough())
            {
                m_JoystickPosition = Vector2.zero;

                if (OnAbilitySelect != null)
                    OnAbilitySelect(AbilityType);
            }
            
            base.HandleTouchEnd();
        }


        bool MouseDeltaIsEnough()
        {
            Vector3 delta = Input.mousePosition - m_TouchStartMousePos;
            return delta.sqrMagnitude >= m_SQR_DISTANCE_TO_USE_ABILITY;
        }
    }
}
