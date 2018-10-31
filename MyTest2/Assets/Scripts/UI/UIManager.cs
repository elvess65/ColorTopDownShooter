using mytest2.UI.Animations;
using mytest2.UI.Controllers;
using mytest2.UI.InputSystem;
using mytest2.UI.Windows;
using UnityEngine;

namespace mytest2.UI
{
    public class UIManager : MonoBehaviour
    {
        [Header("Animation Controllers")]
        public BaseUIAnimationController MoveJoystickAnimationController;
        public BaseUIAnimationController DodgeJoystickAnimationController;
        public BaseUIAnimationController[] AbilityJoysticksAnimationController;
        [Header("UI Controllers")]
        public UIStaminaController StaminaController;

        public UIWindowsManager WindowManager
        {
            get; private set;
        }

        void Start()
        {
            WindowManager = GetComponent<UIWindowsManager>();

            //Animation Controller
#if UNITY_EDITOR
            if (InputManager.Instance.PreferVirtualJoystickInEditor)
                SubscribeForVirtualJoystick();
#else
            SubscribeForVirtualJoystick();
#endif
        }

        public UIWindow_Base ShowWindow(UIWindow_Base source)
        {
            return WindowManager.ShowWindow(source);
        }

        void SubscribeForVirtualJoystick()
        {
            InputManager.Instance.OnInputStateChange += MoveJoystickAnimationController.PlayAnimation;
            InputManager.Instance.OnInputStateChange += DodgeJoystickAnimationController.PlayAnimation;

            for (int i = 0; i < AbilityJoysticksAnimationController.Length; i++)
                InputManager.Instance.OnInputStateChange += AbilityJoysticksAnimationController[i].PlayAnimation;
        }
    }
}
