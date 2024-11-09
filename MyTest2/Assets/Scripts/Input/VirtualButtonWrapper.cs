using UnityEngine;
using UnityEngine.EventSystems;

namespace mytest2.UI.InputSystem
{
    /// <summary>
    /// Надстройка над обычной кнопкой
    /// </summary>
    public class VirtualButtonWrapper : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        public System.Action<Vector2> OnButtonTouchStart;
        public System.Action<Vector2> OnButtonMove;
        public System.Action<Vector2> OnButtonTouchEnd;

        public virtual void OnDrag(PointerEventData eventData)
        {
            if (OnButtonMove != null)
                OnButtonMove(eventData.position);
        }

        public virtual void OnPointerDown(PointerEventData eventData)
        {
            if (OnButtonTouchStart != null)
                OnButtonTouchStart(eventData.position);
        }

        public virtual void OnPointerUp(PointerEventData eventData)
        {
            if (OnButtonTouchEnd != null)
                OnButtonTouchEnd(eventData.position);
        }
    }
}
