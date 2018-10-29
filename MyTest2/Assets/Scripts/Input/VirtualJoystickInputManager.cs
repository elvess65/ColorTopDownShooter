using UnityEngine;

namespace mytest2.UI.InputSystem
{
    public class VirtualJoystickInputManager : BaseInputManager
    {
        public string MoveJoystickName = "MainJoystick";
        public string DodgeJoystickName = "MainJoystick";

        public override void UpdateInput()
        {
            Vector2 jPosition = UltimateJoystick.GetPosition(MoveJoystickName);

            if (OnMove != null)
                OnMove(new Vector3(jPosition.x, 0, jPosition.y));
        }

        protected override void Start()
        {
            base.Start();

            Debug.LogWarning("ButtonJump.onClick.AddListener(ButtonJump_PressHandler);");
        }
    }
}
