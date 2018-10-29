using mytest2.UI.Animations;
using mytest2.UI.InputSystem;
using mytest2.UI.Windows;
using UnityEngine;
using UnityEngine.UI;

namespace mytest2.UI
{
    public class UIManager : MonoBehaviour
    {
        [Header("Buttons")]
        public Button Button_Assist;
        public Button Button_Weapon;
        [Header("Animation Controllers")]
        public BaseUIAnimationController JoystickAnimationController;
        public BaseUIAnimationController AssistantButtonAnimationController;
        public BaseUIAnimationController WeaponButtonAnimationController;
        public BaseUIAnimationController CompassAnimationController;
        public BaseUIAnimationController JumpButtonAnimationController;
        [Header("Animation Controllers Init settings")]
        public bool ShowAssistantButton = true;
        public bool ShowWeaponButton = true;
        public bool ShowCompass = true;
        public bool ShowJumpButton = true;

        private UIWindowsManager m_WindowsManager;

        public UIWindowsManager WindowManager
        {
            get { return m_WindowsManager; }
        }

        void Start()
        {
            m_WindowsManager = GetComponent<UIWindowsManager>();

            //Animation Controller
            if (ShowAssistantButton && AssistantButtonAnimationController != null)
                InputManager.Instance.OnInputStateChange += AssistantButtonAnimationController.PlayAnimation;

            if (ShowWeaponButton && WeaponButtonAnimationController != null)
                InputManager.Instance.OnInputStateChange += WeaponButtonAnimationController.PlayAnimation;

            if (ShowCompass && CompassAnimationController != null)
                InputManager.Instance.OnInputStateChange += CompassAnimationController.PlayAnimation;

#if UNITY_EDITOR
            if (InputManager.Instance.PreferVirtualJoystickInEditor)
                SubscribeForVirtualJoystick();
#else
            SubscribeForVirtualJoystick();
#endif
        }

        public UIWindow_Base ShowWindow(UIWindow_Base source)
        {
            return m_WindowsManager.ShowWindow(source);
        }

        void SubscribeForVirtualJoystick()
        {
            Debug.LogWarning("InputManager.Instance.OnInputStateChange += JoystickAnimationController.PlayAnimation;");
            //InputManager.Instance.OnInputStateChange += JoystickAnimationController.PlayAnimation;

            if (ShowJumpButton && JumpButtonAnimationController != null)
                InputManager.Instance.OnInputStateChange += JumpButtonAnimationController.PlayAnimation;
        }
    }
}
