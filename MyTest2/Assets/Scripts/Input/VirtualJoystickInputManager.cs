using UnityEngine;

namespace mytest2.UI.InputSystem
{
    public class VirtualJoystickInputManager : BaseInputManager
    {
        public string MainJoystickName = "MainJoystick";
        public UnityEngine.UI.Button ButtonJump; 

        public override void UpdateInput()
        {
            Vector2 jPosition = UltimateJoystick.GetPosition(MainJoystickName);

            if (OnMove != null)
                OnMove(new Vector3(jPosition.x, 0, jPosition.y));

#if UNITY_EDITOR
            if (Input.GetKeyUp(KeyCode.Space))
            {
                if (OnJump != null)
                    OnJump();
            }
#endif
        }

        protected override void Start()
        {
            base.Start();

            Debug.LogWarning("ButtonJump.onClick.AddListener(ButtonJump_PressHandler);");
        }

        void ButtonJump_PressHandler()
        {
            if (OnJump != null)
                OnJump();
        }
    }
}
