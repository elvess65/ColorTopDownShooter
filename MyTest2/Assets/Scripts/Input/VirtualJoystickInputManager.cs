using mytest2.Character.Abilities;
using System.Collections.Generic;
using UnityEngine;

namespace mytest2.UI.InputSystem
{
    public class VirtualJoystickInputManager : BaseInputManager
    {
        public string MoveJoystickName = "MainJoystick";

        public VirtualButtonWrapper AttackButtonWrapper;
        public VirtualButtonWrapper DodgeButtonWrapper;
        public ShieldVirtualButtonWrapper ShieldButtonWrapper;
        [Header("Ability")]
        public AbilityVirtualButtonWrapper[] AbilityButtonWrappers;

        private Dictionary<AbilityTypes, AbilityVirtualButtonWrapper> m_AbilityWrappers; //Словарь создан для более удобного доступа к способностям


        public AbilityVirtualButtonWrapper GetAbilityJoystick(AbilityTypes type)
        {
            if (m_AbilityWrappers.ContainsKey(type))
                return m_AbilityWrappers[type];

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
            m_AbilityWrappers = new Dictionary<AbilityTypes, AbilityVirtualButtonWrapper>();
            for (int i = 0; i < AbilityButtonWrappers.Length; i++)
            {
                if (!m_AbilityWrappers.ContainsKey(AbilityButtonWrappers[i].AbilityType))
                    m_AbilityWrappers.Add(AbilityButtonWrappers[i].AbilityType, AbilityButtonWrappers[i]);
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
            /*DodgeJoystickWrapper.Init();

            for (int i = 0; i < AbilityJoystickWrappers.Length; i++)
                AbilityJoystickWrappers[i].Init();*/
        }
    }
}
