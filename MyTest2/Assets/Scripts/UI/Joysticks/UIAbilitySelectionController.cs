using UnityEngine;

namespace mytest2.UI.Controllers
{
    /// <summary>
    /// Контроллер UI - визуальное отображение выделение джойстика (игрок выделяет способность)
    /// </summary>
    public class UIAbilitySelectionController : MonoBehaviour
    {
		public RectTransform SelectionObj;

        private void Start()
        {
			SelectionObj.gameObject.SetActive(false);
        }

        public void Select(Transform joystickTransform)
        {
			SelectionObj.gameObject.SetActive(true);
			SelectionObj.SetParent(joystickTransform);
			SelectionObj.anchoredPosition = Vector3.zero;
        }
    }
}
