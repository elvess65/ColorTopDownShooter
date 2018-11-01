﻿using mytest2.Character.Abilities;
using mytest2.UI.Controllers3D;
using mytest2.UI.InputSystem;
using UnityEngine;

namespace mytest2.Character
{
    public class PlayerController : CreatureController
    {
        private UIPlayerActionDirectionController m_UIActionDirectionController;
        private Vector3 m_MoveDir = Vector3.zero;
        private float m_TargetRotAngle;

        private static AbilityTypes m_CurAbilityType = AbilityTypes.None;

        public static AbilityTypes SelectedAbility
        {
            get { return m_CurAbilityType; }
        }

        void Update()
        {
            //Передвижение и вращение если персонаж не уклоняеться
            if (!m_DodgeController.IsDodging)
            {
                m_MoveController.Move(m_MoveDir);
                m_MoveController.Rotate(m_TargetRotAngle);
            }
        }

        /// <summary>
        /// Сохранить данные для передвижения (вызов реализации происходит в Update и за нее отвечает компонент iMovement)
        /// </summary>
        /// <param name="dir"></param>
        public override void Move(Vector3 dir)
        {
            //Кэш направления передвижения
            m_MoveDir = dir;

            //Если игрок хочет переместиться
            if (m_MoveDir != Vector3.zero)
            {
                //Вращение в направлении движения
                m_TargetRotAngle = Mathf.Atan2(m_MoveDir.x, m_MoveDir.z) * Mathf.Rad2Deg;
            }
        }

        /// <summary>
        /// Нажатие на джойстик уклона
        /// </summary>
        void DodgeInputTouchStart(Vector2 dir)
        {
            m_UIActionDirectionController = Instantiate(GameManager.Instance.PrefabLibrary.UIAbilityDirectionPrefab, transform.position, Quaternion.identity);
            m_UIActionDirectionController.Init(transform, AbilityTypes.None);
        }

        /// <summary>
        /// Перемещение джойстика уклона 
        /// </summary>
        void DodgeInputDrag(Vector2 dir)
        {
            if (m_UIActionDirectionController != null)
                m_UIActionDirectionController.SetDirection(dir);
        }

        void DodgeInputTouchEnd(Vector2 dir)
        {
            if (m_UIActionDirectionController != null)
                Destroy(m_UIActionDirectionController.gameObject);
        }

        /// <summary>
        /// Персонаж начал выполнять уклон
        /// </summary>
        void DodgeStartedHandler()
        {
            Debug.Log("DodgeStarted");
            //TODO: Translate to animation
        }

        /// <summary>
        /// Персонаж закончил выполнять уклон
        /// </summary>
        void DodgeFinishedHandler()
        {
            Debug.Log("DodgeFinished");
            //TODO: Translate to animation
        }


        /// <summary>
        /// Активация способности
        /// </summary>
        /// <param name="type">Тип способности</param>
        void AbilityInputActivate(AbilityTypes type)
        {
            m_UIActionDirectionController = Instantiate(GameManager.Instance.PrefabLibrary.UIAbilityDirectionPrefab, transform.position, Quaternion.identity);
            m_UIActionDirectionController.Init(transform, type);
        }

        /// <summary>
        /// Выбор способности
        /// </summary>
        /// <param name="abilityType">Тип способности</param>
        void AbilityInputSelect(AbilityTypes abilityType)
        {
            if (m_CurAbilityType != abilityType)
            {
                m_CurAbilityType = abilityType;
                GameManager.Instance.UIManager.SelectAbilityJoystick(abilityType);
            }
        }

        /// <summary>
        /// Перемещение джойстика способности 
        /// </summary>
        void AbilityInputDrag(Vector2 dir)
        {
            if (m_UIActionDirectionController != null)
                m_UIActionDirectionController.SetDirection(dir);
        }

        /// <summary>
        /// окончание нажатия на джойстик способности
        /// </summary>
        /// <param name="dir">Направление способности</param>
        void AbilityInputTouchEnd(Vector2 dir)
        {
            //Если нельзя использовать способность длина вектора 0 (если способность была только выделена, но не использована)
            if (dir.sqrMagnitude > 0)
                UseAbility(SelectedAbility, dir);

            if (m_UIActionDirectionController != null)
                Destroy(m_UIActionDirectionController.gameObject);
        }

        /// <summary>
        /// Обновить состояние джойстика способности (откат)
        /// </summary>
        /// <param name="type"></param>
        void AbilityUseHandler(AbilityTypes type)
        {
            DataAbility abilityData = GameManager.Instance.GameState.DataTableAbilities.GetAbilityData(type);
            GameManager.Instance.UIManager.CooldownAbilityJoystick(type, abilityData.CooldownMiliseconds);
        }


        /// <summary>
        /// Вывести состояние силы на UI
        /// </summary>
        /// <param name="progress"></param>
        void StaminaUpdateHandler(float progress)
        {
            GameManager.Instance.UIManager.StaminaController.SetState(progress);

   
        }


        protected override void SubscribeForInputEvents()
        {
#if UNITY_EDITOR
            if (InputManager.Instance.PreferVirtualJoystickInEditor)
                SubscribeForJoystickEvents();
            else
            {
                InputManager.Instance.KeyboardInput.OnMove += Move;
            
                InputManager.Instance.KeyboardInput.OnDodgeStart += DodgeInputTouchStart;
                InputManager.Instance.KeyboardInput.OnDodgeDrag += DodgeInputDrag;
                InputManager.Instance.KeyboardInput.OnDodge += Dodge;
            }
#else
            SubscribeForJoystickEvents();
#endif

            InputManager.Instance.OnInputStateChange += InputStatusChangeHandler;
        }

        protected override void SubscribeForControllerEvents()
        {
            m_DodgeController.OnDodgeStarted += DodgeStartedHandler;
            m_DodgeController.OnDodgeFinished += DodgeFinishedHandler;

            m_StaminaController.OnStaminaUpdate += StaminaUpdateHandler;
            m_AbilityController.OnAbilityUse += AbilityUseHandler;
        }


        void SubscribeForJoystickEvents()
        {
            //Передвижение
            InputManager.Instance.VirtualJoystickInput.OnMove += Move;

            //Уклон
            InputManager.Instance.VirtualJoystickInput.DodgeJoystickWrapper.OnJoystickTouchStart += DodgeInputTouchStart;
            InputManager.Instance.VirtualJoystickInput.DodgeJoystickWrapper.OnJoystickMove += DodgeInputDrag;
            InputManager.Instance.VirtualJoystickInput.DodgeJoystickWrapper.OnJoystickTouchEnd += Dodge;
            InputManager.Instance.VirtualJoystickInput.DodgeJoystickWrapper.OnJoystickTouchEnd += DodgeInputTouchEnd;

            //Способности
            for (int i = 0; i < InputManager.Instance.VirtualJoystickInput.AbilityJoystickWrappers.Length; i++)
            {
                InputManager.Instance.VirtualJoystickInput.AbilityJoystickWrappers[i].OnAbilityActivate += AbilityInputActivate;
                InputManager.Instance.VirtualJoystickInput.AbilityJoystickWrappers[i].OnAbilitySelect += AbilityInputSelect;
                InputManager.Instance.VirtualJoystickInput.AbilityJoystickWrappers[i].OnJoystickTouchEnd += AbilityInputTouchEnd;
                InputManager.Instance.VirtualJoystickInput.AbilityJoystickWrappers[i].OnJoystickMove += AbilityInputDrag;
            }
        }

        /// <summary>
        /// Смена состояния ввода
        /// </summary>
        /// <param name="state"></param>
        void InputStatusChangeHandler(bool state)
        {

        }
    }
}
