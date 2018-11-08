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
        private System.Action<AbilityTypes> m_OnSelectAbilityVisuals;
        private System.Action<AbilityTypes, int> m_OnAbilityUpdateAmmo;

        [Header("Animation Controllers")]
        public BaseUIAnimationController MoveJoystickAnimationController;
        public BaseUIAnimationController DodgeJoystickAnimationController;
        public BaseUIAnimationController[] AbilityJoysticksAnimationController;
        public BaseUIAnimationController[] AbilityKeyboardAnimationController;
        [Header("UI Controllers")]
        public UIStaminaController StaminaController;
        public UIAbilitySelectionController JoystickSelectionController;
        public UIAbilitySelectionController KeyboardSelectionController;

        public UIWindowsManager WindowManager
        {
            get; private set;
        }

        void Start()
        {
            WindowManager = GetComponent<UIWindowsManager>();

#if UNITY_EDITOR
            if (InputManager.Instance.PreferVirtualJoystickInEditor)
                SubscribeForVirtualJoystick();
            else
            {
                for (int i = 0; i < AbilityKeyboardAnimationController.Length; i++)
                    InputManager.Instance.OnInputStateChange += AbilityKeyboardAnimationController[i].PlayAnimation;

                m_OnSelectAbilityVisuals = KeyboardSelectAbility;
                m_OnAbilityUpdateAmmo = KeyboardUpdateAbilityAmmo;
            }
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

        public UIWindow_Base ShowWindow(UIWindow_Base source)
        {
            return WindowManager.ShowWindow(source);
        }

        //Select
        public void SelectAbilityVisuals(AbilityTypes type)
        {
            m_OnSelectAbilityVisuals(type);
        }

        void VirtualJoystickSelectAbility(AbilityTypes type)
        {
            AbilityVirtualJoystickWrapper targetJoystick = InputManager.Instance.VirtualJoystickInput.GetAbilityJoystick(type);
            if (targetJoystick != null)
                JoystickSelectionController.Select(targetJoystick.transform);
        }

        void KeyboardSelectAbility(AbilityTypes type)
        {
            AbilityKeyboardWrapper targetKeyboard = InputManager.Instance.KeyboardInput.GetAbilityKeyboard(type);
            if (targetKeyboard != null)
                KeyboardSelectionController.Select(targetKeyboard.transform);
        }

        //Update ammo
        public void UpdateAbilityAmmo(AbilityTypes type, int ammoAmount)
        {
            m_OnAbilityUpdateAmmo(type, ammoAmount);
        }

        void VirtualJoystickUpdateAbilityAmmo(AbilityTypes type, int ammoAmount)
        {
            AbilityVirtualJoystickWrapper targetJoystick = InputManager.Instance.VirtualJoystickInput.GetAbilityJoystick(type);
            if (targetJoystick != null)
                targetJoystick.UpdateAbilityAmmo(ammoAmount);
        }

        void KeyboardUpdateAbilityAmmo(AbilityTypes type, int ammoAmount)
        {
            AbilityKeyboardWrapper targetKeyboard = InputManager.Instance.KeyboardInput.GetAbilityKeyboard(type);
            if (targetKeyboard != null)
                targetKeyboard.UpdateAbilityAmmo(ammoAmount);
        }


        void SubscribeForVirtualJoystick()
        {
            InputManager.Instance.OnInputStateChange += MoveJoystickAnimationController.PlayAnimation;
            InputManager.Instance.OnInputStateChange += DodgeJoystickAnimationController.PlayAnimation;

            for (int i = 0; i < AbilityJoysticksAnimationController.Length; i++)
                InputManager.Instance.OnInputStateChange += AbilityJoysticksAnimationController[i].PlayAnimation;

            m_OnSelectAbilityVisuals = VirtualJoystickSelectAbility;
            m_OnAbilityUpdateAmmo = VirtualJoystickUpdateAbilityAmmo;
        }
    }
}
