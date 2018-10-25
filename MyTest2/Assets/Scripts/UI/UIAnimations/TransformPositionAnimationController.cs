using UnityEngine;

namespace mytest2.UI.Animations
{
    /// <summary>
    /// Контролирует анимацию расположения Transform
    /// </summary>
    public class TransformPositionAnimationController : VectorAnimationController
    {
        [Header("Link")]
        public Transform Target;

        protected override void SetTargetPosition()
        {
            //Получить текущую позицию кнопки
            m_TargetPosition = Target.position;
        }

        protected override void ApplyPosition(Vector3 position)
        {
            //Задать позицию кнопки
            Target.position = position;
        }
    }
}
