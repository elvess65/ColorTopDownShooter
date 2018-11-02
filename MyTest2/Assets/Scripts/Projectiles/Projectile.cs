using mytest2.Utils.Pool;
using UnityEngine;

namespace mytest2.Projectiles
{
    /// <summary>
    /// Обычный снярад, который может лететь по прямой
    /// </summary>
    public class Projectile : PoolObject
    {
        private float m_Speed = 10;
        private float m_MaxSQRDist = 100;
        private Vector3 m_Dir;
        private Vector3 m_LaunchPos;
        private bool m_IsActive = false;

        public void Launch(Vector2 dir)
        {
            m_Dir = new Vector3(dir.x, 0, dir.y);
            m_LaunchPos = transform.position;
            m_IsActive = true;
        }

        protected override void DisableObject()
        {
            m_IsActive = false;

            base.DisableObject();
        }


        void Update()
        {
            if (m_IsActive)
            {
                transform.Translate(m_Dir * m_Speed * Time.deltaTime);

                if ((transform.position - m_LaunchPos).sqrMagnitude > m_MaxSQRDist)
                    Disable();
            }
        }
    }
}
