using mytest2.Character.Abilities;
using mytest2.Utils.Pool;
using UnityEngine;
using UnityEngine.UI;

namespace mytest2.UI.Controllers3D
{
    public class UIHealthBarSegment : PoolObject
    {
        public Image Image_FG;
        [Header("Animation")]
        public float AnimationTime = 0.5f;

        private int m_MaxHealth;
        private int m_CurHealth;
        private Utils.InterpolationData<float> m_LerpData;

        public void Init(AbilityTypes type, int health)
        {
            m_LerpData = new Utils.InterpolationData<float>(AnimationTime);
            m_MaxHealth = health;

            switch (type)
            {
                case AbilityTypes.Blue:
                    Image_FG.color = Color.blue;
                    break;
                case AbilityTypes.Green:
                    Image_FG.color = Color.green;
                    break;
                case AbilityTypes.Red:
                    Image_FG.color = Color.red;
                    break;
                case AbilityTypes.Violet:
                    Image_FG.color = Color.magenta;
                    break;
                case AbilityTypes.Yellow:
                    Image_FG.color = Color.yellow;
                    break;
                default:
                    Image_FG.color = Color.white;
                    break;
            }
        }

		public void UpdateUI(int currentHealth)
		{
            m_CurHealth = currentHealth;

            float progress = (float)currentHealth / m_MaxHealth;
            m_LerpData.From = Image_FG.fillAmount;
            m_LerpData.To = progress;
            m_LerpData.Start();
        }
        
        void Update()
        {
            if (m_LerpData.IsStarted)
            {
                m_LerpData.Increment();
                Image_FG.fillAmount = Mathf.Lerp(m_LerpData.From, m_LerpData.To, m_LerpData.Progress);

                if (m_LerpData.Overtime())
                {
                    m_LerpData.Stop();
                    Image_FG.fillAmount = m_LerpData.To;
                }
            }
        }
    }
}
