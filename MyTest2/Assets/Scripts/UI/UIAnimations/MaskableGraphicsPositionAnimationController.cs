using UnityEngine;

namespace mytest2.UI.Animations
{
    /// <summary>
    /// Контролирует анимацию расположения MaskableGraphic(картинка, текст)
    /// </summary>
    public class MaskableGraphicsPositionAnimationController : VectorAnimationController
    {
        [Header("Link")]
        public RectTransform Target;

        protected override void SetTargetPosition()
        {
            //Получить текущую позицию кнопки
            m_TargetPosition = Target.anchoredPosition;
        }

        protected override void ApplyPosition(Vector3 position)
        {
            //Задать позицию кнопки
            Target.anchoredPosition = position;
        }
    }
}
