using UnityEngine;
using UnityEngine.UI;

namespace mytest2.UI.Animations
{
    /// <summary>
    /// Контролирует анимацию расположения джойстика
    /// </summary>
    public class JoystickAnimationController : OneValueAnimationController
    {
        [Header("Link")]
        public UltimateJoystick Joystick;
        public Image m_RaycastImage;

        protected override void SetTargetPosition()
        {
            //Получить текущую позицию джойстика
            m_TargetPosition = Joystick.customSpacing_X;

            m_RaycastImage.raycastTarget = false;
            Joystick.enabled = false;
        }

        protected override void ApplyPosition(float position)
        {
            Joystick.customSpacing_X = position;
            Joystick.UpdatePositioning();
        }

        protected override void ShowStarted()
        {
            base.ShowStarted();

            Joystick.EnableJoystick();
            Joystick.enabled = false;
        }

        protected override void ShowFinished()
        {
            base.ShowFinished();

            m_RaycastImage.raycastTarget = true;
            Joystick.enabled = true;
        }

        protected override void HideStarted()
        {
            base.HideStarted();

            Joystick.ResetJoystick();

            m_RaycastImage.raycastTarget = false;
            Joystick.enabled = false;
        }

        protected override void HideFinished()
        {
            base.HideFinished();

            Joystick.DisableJoystick();
        }
    }
}
