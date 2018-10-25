using UnityEngine;

namespace mytest2.UI.Animations
{
    /// <summary>
    /// Контролирует анимацию изменения размера
    /// </summary>
    public class WindowSizeAnimationController : OneValueAnimationController
    {
        [Header("Link")]
        public RectTransform m_RectTransform;

        protected override void SetTargetPosition()
        {
            m_TargetPosition = m_RectTransform.localScale.x;
        }

        protected override void ApplyPosition(float position)
        {
            m_RectTransform.localScale = new Vector3(position, position, m_RectTransform.localScale.z);
        }
    }
}
