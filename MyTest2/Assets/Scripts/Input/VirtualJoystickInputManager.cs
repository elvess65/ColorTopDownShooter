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

        public override void UpdateInput()
        {
            Vector2 movePosition = UltimateJoystick.GetPosition(MoveJoystickName);
            if (OnMove != null)
                OnMove(new Vector3(movePosition.x, 0, movePosition.y));
        }

        protected override void Start()
        {
            base.Start();

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
