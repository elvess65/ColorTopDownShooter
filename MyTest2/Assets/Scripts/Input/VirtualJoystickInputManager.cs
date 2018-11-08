using mytest2.Character.Abilities;
using System.Collections.Generic;
using UnityEngine;

namespace mytest2.UI.InputSystem
{
    public class VirtualJoystickInputManager : BaseInputManager
    {
        public string MoveJoystickName = "MainJoystick";

        [Header("Virtual Joystick Wrappers")]
        public VirtualJoystickWrapper DodgeJoystickWrapper;
        [Header(" - Ability Wrappers")]
        public AbilityVirtualJoystickWrapper[] AbilityJoystickWrappers;

        private Dictionary<AbilityTypes, AbilityVirtualJoystickWrapper> m_JoystickWrappers; //Словарь создан для более удобного доступа к джойстикам способностей

        public AbilityVirtualJoystickWrapper GetAbilityJoystick(AbilityTypes type)
        {
            if (m_JoystickWrappers.ContainsKey(type))
                return m_JoystickWrappers[type];

            return null;
        }

        public override void UpdateInput()
        {
            Vector2 movePosition = UltimateJoystick.GetPosition(MoveJoystickName);
            if (OnMove != null)
                OnMove(movePosition);
        }

        protected override void Start()
        {
            base.Start();

            //Создать словать джойстиков способностей
            m_JoystickWrappers = new Dictionary<AbilityTypes, AbilityVirtualJoystickWrapper>();
            for (int i = 0; i < AbilityJoystickWrappers.Length; i++)
            {
                if (!m_JoystickWrappers.ContainsKey(AbilityJoystickWrappers[i].AbilityType))
                    m_JoystickWrappers.Add(AbilityJoystickWrappers[i].AbilityType, AbilityJoystickWrappers[i]);
            }

#if UNITY_EDITOR
            if (InputManager.Instance.PreferVirtualJoystickInEditor)
                InitializeWrappers();
#else
            InitializeWrappers();
#endif
        }

        void InitializeWrappers()
        {
            DodgeJoystickWrapper.Init();

            for (int i = 0; i < AbilityJoystickWrappers.Length; i++)
                AbilityJoystickWrappers[i].Init();
        }
    }
}
