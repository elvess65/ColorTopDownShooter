using UnityEngine;

namespace mytest2.UI.InputSystem
{
    /// <summary>
    /// Надстройка над обычным джойстиком
    /// </summary>
    public class VirtualJoystickWrapper : MonoBehaviour
    {
        public System.Action<Vector2> OnJoystickTouchStart;
        public System.Action<Vector2> OnJoystickMove;
        public System.Action<Vector2> OnJoystickTouchEnd;

        protected Vector2 m_JoystickPosition;
        private UltimateJoystick m_Joystick;
        private bool m_PrevTouchState = false;
        private bool m_IsActive = false;

        public void Init()
        {
            string joystickName = GetComponent<UltimateJoystick>().joystickName;
            m_Joystick = UltimateJoystick.GetJoystick(joystickName);

            m_IsActive = true;
        }

        void HandleInput(bool curTouchState)
        {
            if (curTouchState && !m_PrevTouchState) //Если сейчас джойстик нажат, а на предыдущем кадре не был (произошло нажатие)
                HandleTouchStart();
            else if (curTouchState && m_PrevTouchState) //Если сейчас джойстик нажат и был нажат на предыдущем кадре (зажали)
                HandleDrag();
            else if (!curTouchState && m_PrevTouchState) //Если джойстик не нажат, а на предыдущем кадре был (перестали нажимать)
                HandleTouchEnd();
        }

        void Update()
        {
            if (m_IsActive)
            {
                bool curTouchState = m_Joystick.GetJoystickState();
                HandleInput(curTouchState);
                m_PrevTouchState = curTouchState;
            }
        }


        protected virtual void HandleTouchStart()
        {
            Vector2 jPosition = m_Joystick.GetPosition();
            if (OnJoystickTouchStart != null)
                OnJoystickTouchStart(jPosition);
        }

        protected virtual void HandleDrag()
        {
            m_JoystickPosition = m_Joystick.GetPosition();
            if (OnJoystickMove != null)
                OnJoystickMove(m_JoystickPosition);
        }

        protected virtual void HandleTouchEnd()
        {
            if (OnJoystickTouchEnd != null)
                OnJoystickTouchEnd(m_JoystickPosition);
        } 
    }
}
