using UnityEngine;

namespace mytest2.UI.Controllers
{
    /// <summary>
    /// Контроллер UI - визуальное отображение выделение джойстика (игрок выделяет способность)
    /// </summary>
    public class UIJoystickSelectionController : MonoBehaviour
    {
        public GameObject SelectionObj;

        private void Start()
        {
            SelectionObj.SetActive(false);
        }

        public void Select(Transform joystickTransform)
        {
            SelectionObj.SetActive(true);
            SelectionObj.transform.position = joystickTransform.position;
        }
    }
}
