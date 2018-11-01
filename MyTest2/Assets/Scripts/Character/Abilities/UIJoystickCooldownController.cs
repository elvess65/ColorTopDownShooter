using UnityEngine;
using UnityEngine.UI;

namespace mytest2.UI.Controllers
{
    /// <summary>
    /// Контроллер UI - визуальное отображение отката способности, а так же блокировка нажатия (игрок использовал способность)
    /// </summary>
    public class UIJoystickCooldownController : MonoBehaviour
    {
        public Text Text_Cooldown;

        public void Cooldown(float timeMiliseconds)
        {
            Text_Cooldown.text = (timeMiliseconds / 1000f).ToString();
            Destroy(gameObject, timeMiliseconds / 1000f);

            //TODO: Add timer;
        }
    }
}
