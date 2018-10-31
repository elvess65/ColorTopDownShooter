using UnityEngine;
using UnityEngine.UI;

namespace mytest2.UI.Controllers
{
    /// <summary>
    /// Контроллер UI - отображения силы персонажа
    /// </summary>
    public class UIStaminaController : MonoBehaviour
    {
        public Image Image_FG;

        public void SetState(float progressState)
        {
            Image_FG.fillAmount = progressState;
        }
    }
}
