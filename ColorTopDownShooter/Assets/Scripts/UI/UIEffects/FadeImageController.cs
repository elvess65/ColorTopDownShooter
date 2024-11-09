using UnityEngine;
using UnityEngine.UI;

namespace mytest2.UI.Effects
{
    /// <summary>
    /// Image которая может уходить в прозрачность и выходить из нее (фон для загрузки уровня)
    /// </summary>
    public class FadeImageController : MonoBehaviour
    {
        public System.Action OnFadeAnimationComplete;

        protected Image m_FadeImage;
        protected Utils.InterpolationData<Color> m_LerpData;
        protected float m_FadeTime = 1;
        protected float m_InitAlpha = 1;

        protected virtual void Awake()
        {
            m_FadeImage = GetComponent<Image>();

            //Начальное значение цвета
            Color color = m_FadeImage.color;
            color.a = m_InitAlpha;
            m_FadeImage.color = color;
        }

        void Update()
        {
            if (m_LerpData.IsStarted)
            {
                m_LerpData.Increment();
                m_FadeImage.color = Color.Lerp(m_LerpData.From, m_LerpData.To, m_LerpData.Progress);
                if (m_LerpData.Overtime())
                {
                    m_LerpData.Stop();
                    m_FadeImage.color = m_LerpData.To;

                    if (OnFadeAnimationComplete != null)
                    {
                        OnFadeAnimationComplete();
                        OnFadeAnimationComplete = null;
                    }
                }
            }
        }


        public virtual void FadeIn()
        {
            OnFadeAnimationComplete += () => m_FadeImage.enabled = false;
            Fade(0);
        }

        public virtual void FadeOut()
        {
            m_FadeImage.enabled = true;
            Fade(1);
        }

        void Fade(float alpha)
        {
            m_FadeImage.enabled = true;
            m_LerpData = new Utils.InterpolationData<Color>(m_FadeTime);
            m_LerpData.From = m_FadeImage.color;
            m_LerpData.To = m_LerpData.From;
            m_LerpData.To.a = alpha;
            m_LerpData.Start();
        }
    }
}
