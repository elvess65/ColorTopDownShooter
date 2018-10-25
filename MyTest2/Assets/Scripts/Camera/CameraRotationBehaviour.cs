using UnityEngine;

namespace mytest2.CameraSystem
{
    public class CameraRotationBehaviour : MonoBehaviour
    {
        public System.Action OnFinished;

        public float RotationSpeed = 10;
        public AnimationCurve SpeedCurve;

        private Transform m_Target;

        private float m_Speed;              //Скорость вращения (Может быть общей или указаной в RotateAroundBy)
        private float m_TargetAngle = 90;   //Угол, на который нужно повернуться
        private float m_Clockwise = 1;      //Значение, отвечающее за поворот по часовой стрелке (>0) или против (<0)
        private float m_DegressPassed = 0;  //На сколько градусов уже поврнуто, за один вызов
        private float m_Progress;           //Прогресс достижения целевого угла
        private bool m_IsActive = false;

        public void RotateAroundBy(Transform target, float targetAngle, float speed, bool clockwise)
        {
            m_Target = target;
            m_TargetAngle = targetAngle;
            m_Clockwise = clockwise ? 1 : -1;
            m_Speed = speed <= 0 ? RotationSpeed : speed;

            m_Progress = 0;
            m_DegressPassed = 0;
            m_IsActive = true;
        }

        public void UpdateBehaviour()
        {
            if (m_IsActive)
            {
                //Угол, на который надо сместиться в текущем кадре (с учетом скорости кривой)
                float angle = m_Speed * SpeedCurve.Evaluate(m_Progress) * Time.deltaTime;
                m_DegressPassed += angle;
                m_Progress = m_DegressPassed / m_TargetAngle;

                transform.RotateAround(m_Target.position, Vector3.up * m_Clockwise, angle);

                if (m_Progress >= 1)
                {
                    m_IsActive = false;

                    if (OnFinished != null)
                    {
                        OnFinished();
                        OnFinished = null;
                    }
                }
            }
        }
    }
}
