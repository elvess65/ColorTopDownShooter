using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace mytest2.UI.InputSystem
{
    /// <summary>
    /// Надстройка над кнопкой щита
    /// </summary>
    public class ShieldVirtualButtonWrapper : VirtualButtonWrapper
    {
        public System.Action<Vector2> OnShieldInputStart;
        public System.Action<Vector2> OnShieldInputUpdate;
        public System.Action OnShieldInputEnd;

        public Image Image_Angle;

        private Vector2 m_ShieldOrigin;
        private float m_InitEulerZ;
        private float m_InitAngleOffset;
        private const float m_INIT_FILL = 0.01f;

        private void Start()
        {
            m_InitAngleOffset = 360 / m_INIT_FILL;
            Image_Angle.enabled = false;
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);

            Image_Angle.fillAmount = m_INIT_FILL;
            Image_Angle.enabled = true;
            Image_Angle.color = GameManager.Instance.GameState.AbilityColorController.GetAbilityColor(GameManager.Instance.GameState.Player.SelectedAbility);

            //Выктор направления щита
            m_ShieldOrigin = (eventData.position - new Vector2(transform.position.x, transform.position.y)).normalized;
            //Вектор направления щита в 3D - для вращения (Quaternion.LookRotation принимает Vector3)
            Vector3 origin3D = new Vector3(m_ShieldOrigin.x, 0, m_ShieldOrigin.y);
            //Вращение в направлении щита
            Vector3 lookRotationEuler = Quaternion.LookRotation(origin3D).eulerAngles;

            //Кеш оси (для смещения при изменении угла щита)
            m_InitEulerZ = -lookRotationEuler.y;
            //Вращение 
            Image_Angle.transform.eulerAngles = new Vector3(Image_Angle.transform.localEulerAngles.x, Image_Angle.transform.localEulerAngles.y, m_InitEulerZ + m_InitAngleOffset);

            if (OnShieldInputStart != null)
                OnShieldInputStart(m_ShieldOrigin);
        }

        public override void OnDrag(PointerEventData eventData)
        {
            base.OnDrag(eventData);

            //Текущее направление 
            Vector2 dir = (eventData.position - new Vector2(transform.position.x, transform.position.y)).normalized;
            //Угол щита
            float m_ShieldAngle = Vector3.Angle(m_ShieldOrigin, dir);

            //Заполнение 
            Image_Angle.fillAmount = (m_ShieldAngle * 2) / 360f;
            //Вращение
            Image_Angle.transform.eulerAngles = new Vector3(Image_Angle.transform.localEulerAngles.x, Image_Angle.transform.localEulerAngles.y, m_InitEulerZ + m_ShieldAngle);

            if (OnShieldInputUpdate != null)
                OnShieldInputUpdate(dir);
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);

            //Обнулить 
            Image_Angle.enabled = false;

            if (OnShieldInputEnd != null)
                OnShieldInputEnd();
        }
    }
}
