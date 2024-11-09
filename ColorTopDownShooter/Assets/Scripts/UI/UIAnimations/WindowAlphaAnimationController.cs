using UnityEngine;

namespace mytest2.UI.Animations
{
    /// <summary>
    /// Контролирует анимацию изменения прозрачности для CanvasGroup
    /// </summary>
    public class WindowAlphaAnimationController : OneValueAnimationController
    {
        [Header("Link")]
        public CanvasGroup Group;

        protected override void SetTargetPosition()
        {
            m_TargetPosition = Group.alpha;
        }

        protected override void ApplyPosition(float position)
        {
            Group.alpha = position;
        }
    }
}
