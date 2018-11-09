using System.Collections;
using System.Collections.Generic;
using mytest2.Character.Abilities;
using mytest2.UI.Controllers3D;
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
        public bool CreateSplatOnStart = false;

        private UIShieldController m_ShieldUI;

        public void ShowShieldUI(Vector3 origin)
        {
            if (m_ShieldUI == null)
                m_ShieldUI = PoolManager.GetObject(GameManager.Instance.PrefabLibrary.UIShieldRadiusPrefab) as UIShieldController;

            m_ShieldUI.Init(transform, origin);
        }

        public void UpdateShieldUI(float angle)
        {
            m_ShieldUI.UpdateUI(angle);
        }

        public void HideShieldUI()
        {
            m_ShieldUI.HideUI();
        }

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

            //Добавить програмное представление щита в список активных щитов
            GameManager.Instance.GameState.DataContainerController.ShieldContainer.AddShield(shield);

            //Инициализировать визуальное обображение щита
            shieldObj.Init(type, position, origin, ShieldRadius, angle, ShieldExistsTimeMiliseconds);
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
