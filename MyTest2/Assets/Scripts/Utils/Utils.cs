namespace mytest2.Utils
{
    public struct InterpolationData<T>
    {
        public float TotalTime;
        public T From;
        public T To;

        private float m_CurTime;
        private bool m_Perform;

        public bool IsStarted
        {
            get { return m_Perform; }
        }
        public float Progress
        {
            get { return m_CurTime / TotalTime; }
        }

        public InterpolationData(float totalTime)
        {
            m_Perform = false;
            m_CurTime = 0;
            TotalTime = totalTime;
            From = default(T);
            To = default(T);
        }

        public void Increment()
        {
            m_CurTime += UnityEngine.Time.deltaTime;
        }

        public void Start()
        {
            m_CurTime = 0;
            m_Perform = true;
        }

        public void Stop()
        {
            m_Perform = false;
        }

        /// <summary>
        /// Проверяет превышено ли время ожидания (не выключает)
        /// </summary>
        /// <returns>The overtime.</returns>
        public bool Overtime()
        {
            return m_CurTime >= TotalTime;
        }
    }
}
