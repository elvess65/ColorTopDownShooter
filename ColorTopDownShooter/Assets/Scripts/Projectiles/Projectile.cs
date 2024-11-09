using mytest2.Utils.Pool;
using UnityEngine;
using mytest2.Character.Collisions;
using mytest2.Character.Abilities;
using mytest2.Character.Shield;

namespace mytest2.Projectiles
{
    /// <summary>
    /// Обычный снярад, который может лететь по прямой
    /// </summary>
	[RequireComponent(typeof(TriggerCollisionController))]
    public class Projectile : PoolObject
    {
        private int m_SenderTeamID;
        private float m_Speed = 10;
        private float m_MaxSQRDist = 100;
        private Vector3 m_Dir;
        private Vector3 m_LaunchPos;
        private bool m_IsActive = false;
		private TriggerCollisionController m_CollisionController;

        public AbilityTypes Type
        { get; private set; }

        public void Launch(Vector2 dir, AbilityTypes type, int senderTeamID, float distance)
        {
			if (m_CollisionController == null) 
			{
				m_CollisionController = GetComponent<TriggerCollisionController> ();
				m_CollisionController.OnTriggerEnterEvent = CollisionWithAnythingHandler;
			}

            m_MaxSQRDist = distance * distance;
            Type = type;
            m_SenderTeamID = senderTeamID;
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
                CheckShieldCollision();

                if ((transform.position - m_LaunchPos).sqrMagnitude > m_MaxSQRDist)
                    Disable();
            }
        }

        void CheckShieldCollision()
        {
            //Проходимся по всем активным щитам в игре
            for (int i = 0; i < GameManager.Instance.GameState.DataContainerController.ShieldContainer.ShieldsCount; i++)
            {
                Shield curShield = GameManager.Instance.GameState.DataContainerController.ShieldContainer.GetShield(i);

                //Если создатель текущего щита враг, тип щита такой же как и тип снаряда и позиция снаряда пересекает щит - попадание в щит
                if (curShield.SenderTeamID != m_SenderTeamID && curShield.Type == Type && curShield.Intersects(transform.position))
                    DisableObject();
            }
        }

        void CollisionWithAnythingHandler(Collider collider)
		{
			if (collider.GetComponent<Projectile>() == null)
				Disable ();
		}
    }
}
