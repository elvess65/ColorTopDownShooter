using UnityEngine;

namespace mytest2.CameraSystem
{
    public class CameraAligningBehaviour : MonoBehaviour
    {
        public System.Action OnFinished;

        public float AnimationTime = 1;

        private Transform m_Target;
        private Utils.InterpolationData<Vector3> m_PositionLerpData;
        private Utils.InterpolationData<Quaternion> m_RotationLerpData;

        private void Awake()
        {
            m_PositionLerpData = new Utils.InterpolationData<Vector3>(AnimationTime);
            m_RotationLerpData = new Utils.InterpolationData<Quaternion>();
        }

        public void AlignToOffset(Transform target, Vector3 offset)
        {
            m_Target = target;

            //От текущей позиции до позиции цели с учетом смещения
            m_PositionLerpData.From = transform.position;
            m_PositionLerpData.To = m_Target.transform.position + offset;

            //От текущего вращения
            m_RotationLerpData.From = transform.rotation;
            //m_RotationLerpData.To = Quaternion.LookRotation(m_Target.transform.position - transform.position);

            m_PositionLerpData.Start();
        }

        public void UpdateBehaviour()
        {
            if (m_PositionLerpData.IsStarted)
            {
                m_PositionLerpData.Increment();

                transform.position = Vector3.Slerp(m_PositionLerpData.From, m_PositionLerpData.To, m_PositionLerpData.Progress);

                //Вращение в направлении к цели
                m_RotationLerpData.To = Quaternion.LookRotation(m_Target.transform.position - transform.position);
                transform.rotation = Quaternion.Lerp(m_RotationLerpData.From, m_RotationLerpData.To, m_PositionLerpData.Progress);

                if (m_PositionLerpData.Overtime())
                {
                    m_PositionLerpData.Stop();
                    transform.position = m_PositionLerpData.To;
                    transform.rotation = m_RotationLerpData.To;

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
