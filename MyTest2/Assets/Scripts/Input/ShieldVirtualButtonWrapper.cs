using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace mytest2.UI.InputSystem
{
    public class ShieldVirtualButtonWrapper : VirtualButtonWrapper
    {
        public Image Image_Angle;
        Vector2 shieldOrigin;

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);

            Vector2 dir = eventData.position - new Vector2(transform.position.x, transform.position.y);

            Vector3 lookRotationEuler = Quaternion.LookRotation(dir.normalized).eulerAngles;
            Debug.Log(lookRotationEuler);
            Image_Angle.transform.eulerAngles = new Vector3(Image_Angle.transform.localEulerAngles.x, Image_Angle.transform.localEulerAngles.y, lookRotationEuler.z);

            shieldOrigin = dir.normalized;
            Debug.Log(dir.normalized);

            if (InputManager.Instance.OnShieldInputStart != null)
                InputManager.Instance.OnShieldInputStart(dir.normalized);
        }

        public override void OnDrag(PointerEventData eventData)
        {
            base.OnDrag(eventData);
            Vector2 dir = eventData.position - new Vector2(transform.position.x, transform.position.y);
            float m_ShieldAngle = Vector3.Angle(shieldOrigin, dir.normalized);

            if (InputManager.Instance.OnShieldInputUpdate != null)
                InputManager.Instance.OnShieldInputUpdate(dir.normalized);
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);

            Vector2 dir = eventData.position - new Vector2(transform.position.x, transform.position.y);

            if (InputManager.Instance.OnShieldInputEnd != null)
                InputManager.Instance.OnShieldInputEnd();
        }
    }
}
