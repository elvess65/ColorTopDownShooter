using mytest2.Character.Movement;
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
#if UNITY_EDITOR
            if (InputManager.Instance.PreferVirtualJoystickInEditor)
                InputManager.Instance.VirtualJoystickInput.OnMove += Move;
            else
                InputManager.Instance.KeyboardInput.OnMove += Move;
#else
            InputManager.Instance.VirtualJoystickInput.OnMove += MoveInDir;
#endif

            InputManager.Instance.OnInputStateChange += InputStatusChangeHandler;
        }

        void Update()
        {
            m_MoveController.Move(m_MoveDir);
            m_MoveController.Rotate(m_TargetRotAngle);
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

        public override void Dodge(Vector3 dir)
        {

        }

        void InputStatusChangeHandler(bool state)
        {

        }
    }
}
