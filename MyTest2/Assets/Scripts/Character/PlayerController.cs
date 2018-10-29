using mytest2.UI.InputSystem;
using UnityEngine;

namespace mytest2.Character
{
    public class PlayerController : CreatureController
    {
        public float MoveSpeed = 3;
        public float RotateSpeed = 5;

        private Vector3 m_MoveDir = Vector3.zero;
        private Quaternion m_TargetRot;

        protected override void Start()
        {
            //Подписаться на события
#if UNITY_EDITOR
            if (InputManager.Instance.PreferVirtualJoystickInEditor)
            {
                InputManager.Instance.VirtualJoystickInput.OnMove += MoveInDir;
            }
            else
            {
                InputManager.Instance.KeyboardInput.OnMove += MoveInDir;
            }
#else
            InputManager.Instance.VirtualJoystickInput.OnMove += MoveInDir;
#endif

            InputManager.Instance.OnInputStateChange += InputStatusChangeHandler;
        }

        void Update()
        {
            Debug.Log(m_MoveDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, m_TargetRot, Time.deltaTime * RotateSpeed);
            GetComponent<CharacterController>().SimpleMove(m_MoveDir * MoveSpeed * Time.deltaTime);
        }

        public void MoveInDir(Vector3 dir)
        {
            //Кэш направления передвижения
            m_MoveDir = dir;

            //Если игрок хочет переместиться
            if (m_MoveDir != Vector3.zero)
            {
                //Вращение в направлении движения
                float angle = Mathf.Atan2(m_MoveDir.x, m_MoveDir.z) * Mathf.Rad2Deg;
                m_TargetRot = Quaternion.AngleAxis(angle, Vector3.up);
            }
        }

        void InputStatusChangeHandler(bool state)
        {

        }
    }
}
