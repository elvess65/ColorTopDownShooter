using UnityEngine;

namespace mytest2.Utils
{
    public class Timer : MonoBehaviour
    {
        public delegate void TimeElapsedHandler(Timer sender);
        public event TimeElapsedHandler OnStep;
        public event TimeElapsedHandler OnTotalTimeElapsed;

        private float m_StartSecond;
        private float m_CurrentTime = 0;
        private float m_CurrentStep = 0;
        private float m_StepVal = 1;
        private bool m_TimerEnable = false;
        private bool m_Pausable = false;
        private static float m_StartPauseTime = 0;

        public float CurTime
        {
            get { return m_CurrentTime; }
            set { m_CurrentTime = value; }
        }

        public void Init(float second)
        {
            m_StartSecond = second;
            m_CurrentTime = second;
        }

        public void Init(float second, bool pausable)
        {
            m_Pausable = pausable;
            Init(second);
        }

        public void Init(float second, float stepVal)
        {
            Init(second);
            m_StepVal = stepVal;
        }

        public void SetStep(float newStep)
        {
            m_StepVal = newStep;
        }

        public void Step()
        {
            m_CurrentTime -= m_StepVal;

            if (OnStep != null)
                OnStep(this);

            if (m_CurrentTime <= 0)
            {
                if (OnTotalTimeElapsed != null)
                    OnTotalTimeElapsed(this);

                StopCountdown();
            }
        }

        public void ResetEvents()
        {
            OnStep = null;
            OnTotalTimeElapsed = null;
        }

        public void ResetCountdown()
        {
            m_CurrentTime = m_StartSecond;
        }

        public void StartCountdown()
        {
            m_TimerEnable = true;
        }

        public void StopCountdown()
        {
            m_TimerEnable = false;
        }

        void FixedUpdate()
        {
            if (m_TimerEnable)
            {
                m_CurrentStep += Time.fixedDeltaTime;
                if (m_CurrentStep >= m_StepVal)
                {
                    Step();
                    m_CurrentStep -= m_StepVal;
                }
            }
        }

        private void OnApplicationPause(bool state)
        {
            if (!m_Pausable && m_TimerEnable)
            {
                if (state)
                    m_StartPauseTime = Time.realtimeSinceStartup;
                else
                {
                    m_CurrentTime = Mathf.Max(0, Mathf.RoundToInt(m_CurrentTime - (Time.realtimeSinceStartup - m_StartPauseTime)));

                    if (OnStep != null)
                        OnStep(this);

                    if (m_CurrentTime <= 0)
                    {
                        StopCountdown();

                        if (OnTotalTimeElapsed != null)
                            OnTotalTimeElapsed(this);
                    }
                    else
                        StartCountdown();
                }
            }
        }
    }
}