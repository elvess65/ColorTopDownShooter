using mytest2.Character.Abilities;
using UnityEngine;

namespace mytest2.UI.InputSystem
{
    /// <summary>
    /// Надстройка над джойстиком способностей
    /// </summary>
    public class AbilityVirtualJoystickWrapper : VirtualJoystickWrapper
    {
        public System.Action<AbilityTypes> OnAbilitySelect;

        public AbilityTypes AbilityType;

        private bool m_AbilitySelected = false;
        private Vector3 m_TouchStartMousePos;
        private const float m_SQR_DISTANCE_TO_USE_ABILITY = 200;

        protected override void HandleTouchStart()
        {
            base.HandleTouchStart();

            //Являеться ли способность этого джойстика выделенной
            m_AbilitySelected = AbilityType == GameManager.Instance.GameState.SelectedAbilityController.CurAbilityType;

            if (OnAbilitySelect != null)
                OnAbilitySelect(AbilityType);

            m_TouchStartMousePos = Input.mousePosition;
        }

        protected override void HandleDrag()
        {
            base.HandleDrag();

            //Если способность этого джойстика не выделенна
            if (!m_AbilitySelected && MouseDeltaIsEnough())
            {
                m_AbilitySelected = true;
                SelectAbility(AbilityType);
            }
        }

        protected override void HandleTouchEnd()
        {
            //Если не достаточн дистанции для совершения дейтсвия - обнулить направление и выделить способность
            if (!MouseDeltaIsEnough())
            {
                m_JoystickPosition = Vector2.zero;
                SelectAbility(AbilityType);
            }
            
            base.HandleTouchEnd();
        }


        void SelectAbility(AbilityTypes abilityType)
        {
            GameManager.Instance.GameState.SelectedAbilityController.SelectAbility(abilityType);
        }

        bool MouseDeltaIsEnough()
        {
            Vector3 delta = Input.mousePosition - m_TouchStartMousePos;
            return delta.sqrMagnitude >= m_SQR_DISTANCE_TO_USE_ABILITY;
        }
    }
}
