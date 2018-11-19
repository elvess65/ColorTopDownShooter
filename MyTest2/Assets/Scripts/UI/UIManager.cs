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
        private System.Action<AbilityTypes, float> m_OnAbilityCooldown;

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
                m_OnAbilityCooldown = CooldownKeyboard;
            }
#else
            SubscribeForVirtualJoystick();
#endif
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
            AbilityVirtualButtonWrapper targetJoystick = InputManager.Instance.VirtualJoystickInput.GetAbilityJoystick(type);
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
            AbilityVirtualButtonWrapper targetJoystick = InputManager.Instance.VirtualJoystickInput.GetAbilityJoystick(type);
            if (targetJoystick != null)
                targetJoystick.UpdateAbilityAmmo(ammoAmount);
        }

        void KeyboardUpdateAbilityAmmo(AbilityTypes type, int ammoAmount)
        {
            AbilityKeyboardWrapper targetKeyboard = InputManager.Instance.KeyboardInput.GetAbilityKeyboard(type);
            if (targetKeyboard != null)
                targetKeyboard.UpdateAbilityAmmo(ammoAmount);
        }

        //Cooldown
        public void CooldownAbilityJoystick(AbilityTypes type, float timeMiliseconds)
        {
            m_OnAbilityCooldown(type, timeMiliseconds);
        }

        void CooldownJoystick(AbilityTypes type, float timeMiliseconds)
        {
            AbilityVirtualButtonWrapper targetJoystick = InputManager.Instance.VirtualJoystickInput.GetAbilityJoystick(type);
            if (targetJoystick != null)
            {
                UICooldownController joystickCooldownController = Utils.Pool.PoolManager.GetObject(GameManager.Instance.PrefabLibrary.UIJoystickCooldownPrefab) as UICooldownController;
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

        void CooldownKeyboard(AbilityTypes type, float timeMiliseconds)
        {
            AbilityKeyboardWrapper targetKeyboard = InputManager.Instance.KeyboardInput.GetAbilityKeyboard(type);
            if (targetKeyboard != null)
            {
                UICooldownController keyboardCooldownController = Utils.Pool.PoolManager.GetObject(GameManager.Instance.PrefabLibrary.UIKeyboardCooldownPrefab) as UICooldownController;
                RectTransform keyboardRectTransform = targetKeyboard.GetComponent<RectTransform>();
                RectTransform cooldownControllerRectTransform = keyboardCooldownController.GetComponent<RectTransform>();

                cooldownControllerRectTransform.SetParent(keyboardRectTransform.parent);
                cooldownControllerRectTransform.anchorMin = keyboardRectTransform.anchorMin;
                cooldownControllerRectTransform.anchorMax = keyboardRectTransform.anchorMax;
                cooldownControllerRectTransform.offsetMin = keyboardRectTransform.offsetMin;
                cooldownControllerRectTransform.offsetMax = keyboardRectTransform.offsetMax;

                keyboardCooldownController.Cooldown(timeMiliseconds);
            }
        }


        void SubscribeForVirtualJoystick()
        {
            InputManager.Instance.OnInputStateChange += MoveJoystickAnimationController.PlayAnimation;
            InputManager.Instance.OnInputStateChange += DodgeJoystickAnimationController.PlayAnimation;

            for (int i = 0; i < AbilityJoysticksAnimationController.Length; i++)
                InputManager.Instance.OnInputStateChange += AbilityJoysticksAnimationController[i].PlayAnimation;

            m_OnSelectAbilityVisuals = VirtualJoystickSelectAbility;
            m_OnAbilityUpdateAmmo = VirtualJoystickUpdateAbilityAmmo;
            m_OnAbilityCooldown = CooldownJoystick;
        }
    }
}
