using UnityEngine;

namespace mytest2.Character.Container
{
    /// <summary>
    /// Контроллер всех контейнеров данных в игре
    /// </summary>
    public class DataContainerController : MonoBehaviour
    {
        public ShieldDataContainer ShieldContainer;

        private void Start()
        {
            ShieldContainer = new ShieldDataContainer();
        }
    }
}
