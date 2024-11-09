using UnityEngine;

namespace mytest2.CameraSystem
{
    public class CameraFocusingBehaviour : MonoBehaviour
    {
        public System.Action OnFinished;

        public float AnimationTime = 1;

        private Utils.InterpolationData<Vector3> m_LerpData;

        void Awake()
        {
            m_LerpData = new Utils.InterpolationData<Vector3>(AnimationTime);
        }

        public void MoveToWithOffset(Transform target, Vector3 offset)
        {
            m_LerpData.From = transform.position;
            m_LerpData.To = target.position + offset;
            m_LerpData.Start();
        }

        public void UpdateBehaviour()
        {
            if (m_LerpData.IsStarted)
            {
                m_LerpData.Increment();
                transform.position = Vector3.Lerp(m_LerpData.From, m_LerpData.To, m_LerpData.Progress);

                if (m_LerpData.Overtime())
                {
                    m_LerpData.Stop();

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
