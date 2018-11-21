using mytest2.Utils.Pool;
using UnityEngine;
using Werewolf.StatusIndicators.Components;

namespace mytest2.UI.Controllers3D
{
    /// <summary>
    /// Отображение выбора направления действия щита
    /// </summary>
    [RequireComponent(typeof(FollowTransformController))]
    public class UIShieldController : PoolObject
    {
        public Cone IndicatorController;

        private FollowTransformController m_Follow;
        private const float m_INDICATOR_SCALE = 2.3f;

        public void ShowUI(Transform followTransform, Vector3 origin, float shieldRadius)
        {
            if (m_Follow == null)
                m_Follow = GetComponent<FollowTransformController>();

            //Переместить объект в позицию игрока
            transform.position = followTransform.position;

            //Начать следовать за игроком
            m_Follow.Init(followTransform);

            //Размер индикатора
            IndicatorController.Scale = m_INDICATOR_SCALE * shieldRadius;
            //Включить индикатор
            IndicatorController.gameObject.SetActive(true);

            //Вращение индикатора 
            Vector3 lookRotationEuler = Quaternion.LookRotation(origin).eulerAngles;
            transform.eulerAngles = new Vector3(transform.localEulerAngles.x, lookRotationEuler.y, transform.localEulerAngles.z);

            Debug.Log("Shield dir " + origin + " Shield euler: " + lookRotationEuler);

            //Обнулить угол
            IndicatorController.Angle = 0;
        }

        public void UpdateUI(float angle)
        {
            IndicatorController.Angle = angle * 2;
        }

        public void HideUI()
        {
            IndicatorController.gameObject.SetActive(false);
            m_Follow.StopFollowing();
        }
    }
}
