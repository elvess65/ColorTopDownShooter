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
        public int ShieldExistsTimeMiliseconds = 1500;

        public void CreateShield(Vector3 position, Vector3 origin, float angle, AbilityTypes type, int senderTeamID)
        {
            //Создать визуальное отображение щита
            ShieldObject shieldObj = PoolManager.GetObject(GameManager.Instance.PrefabLibrary.GetAbilityShieldPrefab(type)) as ShieldObject;
            shieldObj.transform.position = position;

            //Создать програмное представление щита
            Shield shield = new Shield(position, origin, ShieldRadius, angle, type, senderTeamID, shieldObj);

            //Задать событие на уничтожение щита
            shieldObj.OnTimeElapsed = () => 
            {
                GameManager.Instance.GameState.DataContainerController.ShieldContainer.RemoveShield(shield);
            };

            CreateShieldVisuals(position, origin, angle);

            //Добавить програмное представление щита в список активных щитов
            GameManager.Instance.GameState.DataContainerController.ShieldContainer.AddShield(shield);
            //Инициализировать визуальное обображение щита
            shieldObj.Init(type, origin, ShieldRadius, angle, ShieldExistsTimeMiliseconds);
        }

        void CreateShieldVisuals(Vector3 position, Vector3 origin, float angle)
        {
            int step = 15;
            float angle1 = Vector3.Angle(Vector3.forward, origin);
            Debug.Log(angle + " " + (angle * 2 / step) + " " + angle1 + " " + Vector3.Dot(Vector3.right, origin));
            Vector3 center = GameManager.Instance.GameState.Player.transform.position;
            int startAngle = -(int)(angle1 + angle);
            int finishAngle = startAngle + (int)angle * 2;
            for (int i = startAngle; i < finishAngle; i += step)
            {
                Vector3 pos = PositionOnCircle(center, ShieldRadius, i);
                float scale = 0.5f;
                GameObject ob = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                ob.transform.localScale = new Vector3(scale, scale, scale);
                ob.transform.position = pos;
            }
        }

        Vector3 PositionOnCircle(Vector3 center, float radius, int angle)
        {
            float ang = angle;
            Vector3 pos;
            pos.x = center.x + radius * Mathf.Sin(ang * Mathf.Deg2Rad);
            pos.y = center.y;
            pos.z = center.z + radius * Mathf.Cos(ang * Mathf.Deg2Rad);
            return pos;
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
