using UnityEngine;

namespace mytest2.UI.InputSystem
{
    public class VirtualJoystickInputManager : BaseInputManager
    {
        public string MoveJoystickName = "MainJoystick";
        public string DodgeJoystickName = "MainJoystick";

        public override void UpdateInput()
        {
            Vector2 movePosition = UltimateJoystick.GetPosition(MoveJoystickName);
            if (OnMove != null)
                OnMove(new Vector3(movePosition.x, 0, movePosition.y));

            Vector2 dodgePosition = UltimateJoystick.GetPosition(DodgeJoystickName);
            if (OnDodge != null)
                OnDodge(new Vector3(dodgePosition.x, 0, dodgePosition.y));
        }

        protected override void Start()
        {
            base.Start();

            Debug.LogWarning("ButtonJump.onClick.AddListener(ButtonJump_PressHandler);");
        }
    }
}
