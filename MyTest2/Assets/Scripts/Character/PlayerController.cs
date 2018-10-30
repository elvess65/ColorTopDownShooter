using mytest2.Character.Abilities;
using mytest2.UI.InputSystem;
using UnityEngine;

namespace mytest2.Character
{
    public class PlayerController : CreatureController
    {
        private Vector3 m_MoveDir = Vector3.zero;
        private float m_TargetRotAngle;

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
        /// Начать выполнять уклон (за реализацию уклона отвечает компонент iDodging)
        /// </summary>
        /// <param name="dir"></param>
        public override void Dodge(Vector2 dir)
        {
            m_DodgeController.Dodge(dir);
        }

        /// <summary>
        /// Нажатие на джойстик уклона
        /// </summary>
        void DodgeInputStart(Vector2 dir)
        {
            //TODO: Translate to skill selection
        }

        /// <summary>
        /// Перемещение джойстика уклона 
        /// </summary>
        void DodgeInputDrag(Vector2 dir)
        {
            //TODO: Translate to skill selection
        }

        /// <summary>
        /// Персонаж начал выполнять уклон
        /// </summary>
        void DodgeStarted()
        {
            Debug.Log("DodgeStarted");
            //TODO: Translate to animation
        }

        /// <summary>
        /// Персонаж закончил выполнять уклон
        /// </summary>
        void DodgeFinished()
        {
            Debug.Log("DodgeFinished");
            //TODO: Translate to animation
        }

   
        void AbilityInputTouchEnd(Vector2 dir)
        {
            if (dir.sqrMagnitude > 0)
                UseAbility(GameManager.Instance.GameState.SelectedAbilityController.CurAbilityType, dir);
        }


        protected override void SubscribeForInputEvents()
        {
#if UNITY_EDITOR
            if (InputManager.Instance.PreferVirtualJoystickInEditor)
                SubscribeForJoystickEvents();
            else
            {
                InputManager.Instance.KeyboardInput.OnMove += Move;
            
                InputManager.Instance.KeyboardInput.OnDodgeStart += DodgeInputStart;
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
            m_DodgeController.OnDodgeStarted += DodgeStarted;
            m_DodgeController.OnDodgeFinished += DodgeFinished;
        }

        void SubscribeForJoystickEvents()
        {
            //Передвижение
            InputManager.Instance.VirtualJoystickInput.OnMove += Move;

            //Уклон
            InputManager.Instance.VirtualJoystickInput.DodgeJoystickWrapper.OnJoystickTouchStart += DodgeInputStart;
            InputManager.Instance.VirtualJoystickInput.DodgeJoystickWrapper.OnJoystickMove += DodgeInputDrag;
            InputManager.Instance.VirtualJoystickInput.DodgeJoystickWrapper.OnJoystickTouchEnd += Dodge;

            //Способности
            for (int i = 0; i < InputManager.Instance.VirtualJoystickInput.AbilityJoystickWrappers.Length; i++)
                InputManager.Instance.VirtualJoystickInput.AbilityJoystickWrappers[i].OnJoystickTouchEnd += AbilityInputTouchEnd;
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
