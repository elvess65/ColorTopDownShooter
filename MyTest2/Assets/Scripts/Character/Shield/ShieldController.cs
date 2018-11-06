using UnityEngine;

namespace mytest2.Character.Shield
{
    /// <summary>
    /// Контроллер щита персонажа
    /// </summary>
    public class ShieldController : MonoBehaviour
    {
        public System.Action OnShieldCreated;

        public float ShieldRadius = 2;

        public void CreateShield(Vector3 position, Vector3 origin, float angle)
        {
            Shield shield = new Shield(position, origin, ShieldRadius, angle);
            GameManager.Instance.GameState.SceneObjectsController.AddShield(shield);
        }
    }

    /// <summary>
    /// Програмное представление щита
    /// </summary>
    public class Shield
    {
        public Vector3 Position
        { get; private set; }
        public Vector3 Origin
        { get; private set; }
        public float Radius
        { get; private set; }
        public float Angle
        { get; private set; }

        private float m_SQRRadius;

        public Shield(Vector3 position, Vector3 origin, float radius, float angle)
        {
            Position = position;
            Origin = origin;
            Radius = radius;
            Angle = angle;

            m_SQRRadius = radius * radius;
        }

        public bool Intersects(Vector3 pos)
        {
            Vector3 dirToTarget = pos - Position;
            float sqrDist = dirToTarget.sqrMagnitude;
            if (sqrDist <= m_SQRRadius)
            {
                float angleToTarget = Vector3.Angle(Origin, dirToTarget);
                return angleToTarget <= Angle;
            }

            return false;
        }
    }
}
