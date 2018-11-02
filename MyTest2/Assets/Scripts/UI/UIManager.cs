using mytest2.Character.Abilities;
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

        private UIJoystickSelectionController m_JoystickSelectionController;

        void Start()
        {
            WindowManager = GetComponent<UIWindowsManager>();
            m_JoystickSelectionController = GetComponent<UIJoystickSelectionController>();

            //Animation Controller
#if UNITY_EDITOR
            if (InputManager.Instance.PreferVirtualJoystickInEditor)
                SubscribeForVirtualJoystick();
#else
            SubscribeForVirtualJoystick();
#endif
        }

        public void CooldownAbilityJoystick(AbilityTypes type, float timeMiliseconds)
        {
            AbilityVirtualJoystickWrapper targetJoystick = InputManager.Instance.VirtualJoystickInput.GetAbilityJoystick(type);
            if (targetJoystick != null)
            {
                UIJoystickCooldownController joystickCooldownController = Utils.Pool.PoolManager.GetObject(GameManager.Instance.PrefabLibrary.UIJoystickCooldownPrefab) as UIJoystickCooldownController;
                RectTransform joystickRectTransform = targetJoystick.GetComponent<RectTransform>();
                RectTransform cooldownControllerRectTransform = joystickCooldownController.GetComponent<RectTransform>();

                cooldownControllerRectTransform.SetParent(joystickRectTransform.parent);
                cooldownControllerRectTransform.anchorMin = joystickRectTransform.anchorMin;
                cooldownControllerRectTransform.anchorMax = joystickRectTransform.anchorMax;
                cooldownControllerRectTransform.offsetMin = joystickRectTransform.offsetMin;
                cooldownControllerRectTransform.offsetMax = joystickRectTransform.offsetMax;

                joystickCooldownController.Cooldown(timeMiliseconds);
            }
        }

        public void UpdateAbilityAmmo(AbilityTypes type, int ammoAmount)
        {
            AbilityVirtualJoystickWrapper targetJoystick = InputManager.Instance.VirtualJoystickInput.GetAbilityJoystick(type);
            if (targetJoystick != null)
                targetJoystick.UpdateAbilityAmmo(ammoAmount);
        }

        public void SelectAbilityJoystick(AbilityTypes type)
        {
            AbilityVirtualJoystickWrapper targetJoystick = InputManager.Instance.VirtualJoystickInput.GetAbilityJoystick(type);
            if (targetJoystick != null)
                m_JoystickSelectionController.Select(targetJoystick.transform);
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
