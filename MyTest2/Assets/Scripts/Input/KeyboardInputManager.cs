using mytest2.Character;
using mytest2.Character.Abilities;
using System.Collections.Generic;
using UnityEngine;

namespace mytest2.UI.InputSystem
{
    public class KeyboardInputManager : BaseInputManager
    {
        //Abilities
        public System.Action<AbilityTypes> OnAbilityActivate;
        public System.Action<AbilityTypes> OnAbilitySelect;
        public System.Action<Vector2> OnAbilityMove;
        public System.Action<Vector2> OnAbilityEnd;
        //Dodge
        public System.Action<Vector2> OnDodgeStart;
        public System.Action<Vector2> OnDodgeDrag;
        public System.Action<Vector2> OnDodge;

        public AbilityKeyboardWrapper[] AbilityKeyboardWrappers;

        private float m_AbilityPressTime;
        private bool m_AbilityIsActivated = false;
        private bool m_AbilitySelected = false;

        private Dictionary<AbilityTypes, AbilityKeyboardWrapper> m_KeyboardWrappers; //Словарь создан для более удобного доступа к иконкам способностей
        private const float m_PRES_TIME_TO_ACTIVATE_ABILITY = 0.5f;


        public AbilityKeyboardWrapper GetAbilityKeyboard(AbilityTypes type)
        {
            if (m_KeyboardWrappers.ContainsKey(type))
                return m_KeyboardWrappers[type];

            return null;
        }

        public override void UpdateInput()
        {
            Vector2 dir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            if (OnMove != null)
                OnMove(dir);

            ProcessDodgeKey(KeyCode.Space);

            ProcessAbilityKey(KeyCode.Alpha1, AbilityTypes.Red);
            ProcessAbilityKey(KeyCode.Alpha2, AbilityTypes.Green);
            ProcessAbilityKey(KeyCode.Alpha3, AbilityTypes.Blue);
            ProcessAbilityKey(KeyCode.Alpha4, AbilityTypes.Yellow);
            ProcessAbilityKey(KeyCode.Alpha5, AbilityTypes.Violet);
        }

        protected override void Start()
        {
            base.Start();

            //Создать словать иконок способностей
            m_KeyboardWrappers = new Dictionary<AbilityTypes, AbilityKeyboardWrapper>();
            for (int i = 0; i < AbilityKeyboardWrappers.Length; i++)
            {
                if (!m_KeyboardWrappers.ContainsKey(AbilityKeyboardWrappers[i].AbilityType))
                    m_KeyboardWrappers.Add(AbilityKeyboardWrappers[i].AbilityType, AbilityKeyboardWrappers[i]);
            }
        }


        void ProcessDodgeKey(KeyCode key)
        {
            if (Input.GetKeyDown(key))
            {
                Vector2 dodgeDir = GetMouseScreenDir();
                if (OnDodgeStart != null)
                    OnDodgeStart(dodgeDir.normalized);
            }
            else if (Input.GetKey(key))
            {
                Vector2 dodgeDir = GetMouseScreenDir();
                if (OnDodgeDrag != null)
                    OnDodgeDrag(dodgeDir.normalized);
            }
            else if (Input.GetKeyUp(key))
            {
                Vector2 dodgeDir = GetMouseScreenDir();
                if (OnDodge != null)
                    OnDodge(dodgeDir.normalized);
            }
        }

        void ProcessAbilityKey(KeyCode key, AbilityTypes type)
        {
            if (Input.GetKeyDown(key))
            {
                m_AbilitySelected = type == PlayerController.SelectedAbility;
                m_AbilityIsActivated = false;

                m_AbilityPressTime = Time.time;
            }
            else if (Input.GetKey(key))
            {
                float timeSincePress = Time.time - m_AbilityPressTime;
                if (timeSincePress >= m_PRES_TIME_TO_ACTIVATE_ABILITY)
                {
                    //Если способность не выделенна
                    if (!m_AbilitySelected)
                    {
                        m_AbilitySelected = true;

                        if (OnAbilitySelect != null)
                            OnAbilitySelect(type);
                    }

                    if (!m_AbilityIsActivated)
                    {
                        m_AbilityIsActivated = true;

                        if (OnAbilityActivate != null)
                            OnAbilityActivate(type);
                    }
                    else
                    {
                        Vector2 abilityDir = GetMouseScreenDir();
                        if (OnAbilityMove != null)
                            OnAbilityMove(abilityDir.normalized);
                    }
                }
            }
            else if (Input.GetKeyUp(key))
            {
                float timeSincePress = Time.time - m_AbilityPressTime;
                if (timeSincePress >= m_PRES_TIME_TO_ACTIVATE_ABILITY)
                {
                    Vector2 abilityDir = GetMouseScreenDir();
                    if (OnAbilityEnd != null)
                        OnAbilityEnd(abilityDir.normalized);
                }
                else
                {
                    if (OnAbilitySelect != null)
                        OnAbilitySelect(type);
                }
            }
        }

        Vector2 GetMouseScreenDir()
        {
            return Input.mousePosition - new Vector3(Screen.width / 2, Screen.height / 2, Input.mousePosition.z);
        }
    }
}
