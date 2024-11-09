using UnityEngine;

namespace mytest2.CameraSystem
{
    public class CameraFollowingBehaviour : MonoBehaviour
    {
        public System.Action OnFinished;
        public float Speed = 10;

        private Vector3 m_Offset;
        private Vector3 m_TargetPos;
        private Transform m_Target;
        private bool m_IsActive = false;
        private const float m_STOP_DISATNCE = 0.01f;

        public bool IsMoving
        {
            get { return Vector3.Distance(transform.position, m_TargetPos) > m_STOP_DISATNCE; }
        }

        void Awake()
        {
            m_Offset = Vector3.zero;
        }

        /// <summary>
        /// Начать следовать за объектом с указаным отступом
        /// </summary>
        /// <param name="target">Объект, за которым следовать</param>
        /// <param name="offset">Отступ от объекта за которым необходимо следовать</param>
        public void Follow(Transform target, Vector3 offset)
        {
            m_Offset = offset;
            m_Target = target;

            m_IsActive = true;
        }

        /// <summary>
        /// Остановить следование за объектом, сохранив объект и отступ
        /// </summary>
        public void PauseFollowing()
        {
            m_IsActive = false;
        }

        /// <summary>
        /// Продолжить следование за объектом, за которым следовали раньше
        /// </summary>
        public void ContinueFollowing()
        {
            if (m_Target == null)
                return;

            m_IsActive = true;
        }

        public void UpdateBehaviour()
        {
            if (m_IsActive)
            {
                m_TargetPos = m_Target.transform.position + m_Offset;

                if (IsMoving)
                    transform.position = Vector3.Slerp(transform.position, m_TargetPos, Speed * Time.deltaTime);
                else
                {
                    if (OnFinished != null)
                    {
                        OnFinished();
                        OnFinished = null;
                    }
                }
            }
        }
    }
}
