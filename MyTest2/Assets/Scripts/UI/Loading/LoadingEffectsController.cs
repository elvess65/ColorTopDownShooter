using mytest2.Effects;
using UnityEngine;
using UnityEngine.UI;

namespace mytest2.UI.Loading
{
    /// <summary>
    /// Эффект перехода из начальной сцены (вывод скриншота экрана)
    /// </summary>
    public class LoadingEffectsController : MonoBehaviour
    {
        public System.Action OnAnimationComplete;

        public RawImage BackgroundImage;
        public RawImage UIBackgroundImage;
        public Image TitleImage;
        public Effect_Base[] DisableEffect;

        private Color m_InitColor;
        private Color m_TargetColor;
        private bool m_IsLerpingColor;
        private float m_CurTime = 0;
        private float m_TotalTime = 2;

        public void ShowBackgroundEffect(RenderTexture texture, RenderTexture uitexture)
        {
            BackgroundImage.texture = texture;
            UIBackgroundImage.texture = uitexture;

            m_InitColor = BackgroundImage.color;
            m_TargetColor = m_InitColor;
            m_TargetColor.a = 1;

            m_CurTime = 0;
            m_IsLerpingColor = true;


            for (int i = 0; i < DisableEffect.Length; i++)
            {
                if (DisableEffect[i] != null)
                    DisableEffect[i].Deactivate();
            }
        }

        void Update()
        {
            if (m_IsLerpingColor)
            {
                m_CurTime += Time.deltaTime;
                BackgroundImage.color = Color.Lerp(m_InitColor, m_TargetColor, m_CurTime / m_TotalTime);
                UIBackgroundImage.color = Color.Lerp(m_InitColor, m_TargetColor, m_CurTime / m_TotalTime);

                if (m_CurTime >= m_TotalTime)
                {
                    m_IsLerpingColor = false;

                    if (OnAnimationComplete != null)
                        OnAnimationComplete();
                }
            }
        }
    }
}