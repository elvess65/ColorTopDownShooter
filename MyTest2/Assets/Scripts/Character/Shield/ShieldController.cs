using mytest2.Character.Abilities;
using mytest2.Utils.Pool;
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

        public void CreateShield(Vector3 position, Vector3 origin, float angle, AbilityTypes type, int senderTeamID)
        {
            ShieldObject shieldObj = PoolManager.GetObject(GameManager.Instance.PrefabLibrary.GetAbilityShieldPrefab(type)) as ShieldObject;
            shieldObj.transform.position = position;
            shieldObj.Init(type, origin, ShieldRadius, angle, 1000);

            Shield shield = new Shield(position, origin, ShieldRadius, angle, type, senderTeamID, shieldObj);
            GameManager.Instance.GameState.DataContainerController.ShieldContainer.AddShield(shield);
            //Create shield object
            //Set timer
            //Assign onTimerFinish event
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
        public AbilityTypes Type
        { get; private set; }
        public int SenderTeamID
        { get; private set; }
        public ShieldObject ObjectReference
        { get; private set; }

        private float m_SQRRadius;

        public Shield(Vector3 position, Vector3 origin, float radius, float angle, AbilityTypes type, int senderTeamID, ShieldObject objReference)
        {
            Position = position;
            Origin = origin;
            Radius = radius;
            Angle = angle;
            Type = type;
            SenderTeamID = senderTeamID;
            ObjectReference = objReference;

            m_SQRRadius = radius * radius;
        }

        /// <summary>
        /// Пересекаеться ли позиция с щитом
        /// </summary>
        /// <param name="pos">Позиция для проверки</param>
        public bool Intersects(Vector3 pos)
        {
            //Направление от позиции в щиту
            Vector3 dirToTarget = pos - Position;
            //Квадрат расстояния (для оптимизации)
            float sqrDist = dirToTarget.sqrMagnitude;
            //Если квадрат расстояния до от цели к центру щита меньше чем кадрат радиуса щита - внутри щита. Просчитать угол
            if (sqrDist <= m_SQRRadius)
            {
                //Угол между центром щита и направлением от позиции щиту
                float angleToTarget = Vector3.Angle(Origin, dirToTarget);

                //Для попадания угол под которым находиться позиция должен быть меньше угла щита
                return angleToTarget <= Angle;
            }

            return false;
        }
    }
}
