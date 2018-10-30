using mytest2.UI.InputSystem;
using UnityEngine;

namespace mytest2.Character
{
    public class PlayerController : CreatureController
    {
        private Vector3 m_MoveDir = Vector3.zero;
        private float m_TargetRotAngle;

        protected override void Start()
        {
            base.Start();

            //Подписаться на события
            SubscribeForInputEvents();
        }

        void Update()
        {
            if (!m_DodgeController.IsDodging)
            {
                m_MoveController.Move(m_MoveDir);
                m_MoveController.Rotate(m_TargetRotAngle);
            }
        }


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

        public override void Dodge(Vector2 dir)
        {
            m_DodgeController.Dodge(dir);
        }

        void DodgeInputStart(Vector2 dir)
        {
            //TODO: Translate to skill selection
        }

        void DodgeInputDrag(Vector2 dir)
        {
            //TODO: Translate to skill selection
        }

        void DodgeStarted()
        {
            Debug.Log("DodgeStarted");
            //TODO: Translate to animation
        }

        void DodgeFinished()
        {
            Debug.Log("DodgeFinished");
            //TODO: Translate to animation
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
            InputManager.Instance.VirtualJoystickInput.OnMove += Move;

            InputManager.Instance.VirtualJoystickInput.DodgeJoystickWrapper.OnJoystickTouchStart += DodgeInputStart;
            InputManager.Instance.VirtualJoystickInput.DodgeJoystickWrapper.OnJoystickMove += DodgeInputDrag;
            InputManager.Instance.VirtualJoystickInput.DodgeJoystickWrapper.OnJoystickTouchEnd += Dodge;
        }

        
        void InputStatusChangeHandler(bool state)
        {

        }
    }
}
