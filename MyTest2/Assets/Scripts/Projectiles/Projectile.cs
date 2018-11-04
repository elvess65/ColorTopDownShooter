using mytest2.Utils.Pool;
using UnityEngine;
using mytest2.Character.Collisions;

namespace mytest2.Projectiles
{
    /// <summary>
    /// Обычный снярад, который может лететь по прямой
    /// </summary>
	[RequireComponent(typeof(TriggerCollisionController))]
    public class Projectile : PoolObject
    {
        private float m_Speed = 10;
        private float m_MaxSQRDist = 100;
        private Vector3 m_Dir;
        private Vector3 m_LaunchPos;
        private bool m_IsActive = false;
		private TriggerCollisionController m_CollisionController;

        public void Launch(Vector2 dir)
        {
			if (m_CollisionController == null) 
			{
				m_CollisionController = GetComponent<TriggerCollisionController> ();
				m_CollisionController.OnTriggerEnterEvent = CollisionWithAnythingHandler;
			}
				
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

		void CollisionWithAnythingHandler(Collider other)
		{
			Disable ();
		}
    }
}
