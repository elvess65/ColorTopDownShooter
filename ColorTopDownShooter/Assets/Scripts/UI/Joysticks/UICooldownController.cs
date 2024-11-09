using mytest2.Utils;
using mytest2.Utils.Pool;
using UnityEngine;
using UnityEngine.UI;

namespace mytest2.UI.Controllers
{
    /// <summary>
    /// Контроллер UI - визуальное отображение отката способности, а так же блокировка нажатия (игрок использовал способность)
    /// </summary>
    [RequireComponent(typeof(Timer))]
    public class UICooldownController : PoolObject
    {
        public Text Text_Cooldown;
        public Image Image_Cooldown;

        private Timer m_Timer;
        private InterpolationData<float> m_LerpData;

        public void Cooldown(float timeMiliseconds)
        {
            float timeSeconds = timeMiliseconds / 1000f;

            UpdateLeftTime(timeSeconds);

            m_LerpData = new InterpolationData<float>(timeSeconds);
            m_LerpData.From = 1;
            m_LerpData.To = 0;

            if (m_Timer == null)
            {
                m_Timer = GetComponent<Timer>();

                m_Timer.OnStep += TimerStep_Handler;
                m_Timer.OnTotalTimeElapsed += TimerElapsed_Handler;
            }

            m_Timer.Init(timeSeconds);
            m_Timer.StartCountdown();

            m_LerpData.Start();
        }


        void Update()
        {
            if (m_LerpData.IsStarted)
            {
                m_LerpData.Increment();

                float progress = Mathf.Lerp(m_LerpData.From, m_LerpData.To, m_LerpData.Progress);
                UpdateLeftTimeImage(progress);

                if (m_LerpData.Overtime())
                {
                    m_LerpData.Stop();
                    UpdateLeftTimeImage(m_LerpData.To);
                }
            }
        }

        void UpdateLeftTimeImage(float progress)
        {
            Image_Cooldown.fillAmount = progress;
        }

        void UpdateLeftTime(float time)
        {
            Text_Cooldown.text = time.ToString();
        }

        void TimerStep_Handler(Timer sender)
        {
            UpdateLeftTime(sender.CurTime);
        }

        void TimerElapsed_Handler(Timer sender)
        {
            Disable();
        }
    }
}
