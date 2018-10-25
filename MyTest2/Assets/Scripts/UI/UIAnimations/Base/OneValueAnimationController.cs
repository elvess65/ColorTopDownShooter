namespace mytest2.UI.Animations
{
    public abstract class OneValueAnimationController : UIAnimationController<float>
    {
        protected override float GetTotalDistance()
        {
            return m_TargetPosition - StartPosition;
        }

        protected override float GetShowValue(float t)
        {
            return StartPosition + CurveShow.Evaluate(t) * m_TotalDistance;
        }

        protected override float GetHideValue(float t)
        {
            return m_TargetPosition - CurveHide.Evaluate(t) * m_TotalDistance;
        }
    }
}
