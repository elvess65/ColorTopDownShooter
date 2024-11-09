using System.Collections;
using UnityEngine;

namespace mytest2.Utils.Pool
{
    /// <summary>
    /// Объект пулла
    /// </summary>
    public class PoolObject : MonoBehaviour
    {
        public System.Action<PoolObject> OnDisable;

        private bool m_IsEnabled = false;

        public bool IsEnabled
        {
            get { return m_IsEnabled; }
        }

        /// <summary>
        /// Вызываеться при получении объекта с пулла
        /// </summary>
        public void Enable()
        {
            if (gameObject != null)
                gameObject.SetActive(true);

            m_IsEnabled = true;
        }

        /// <summary>
        /// Отключение объекта с задержкой
        /// </summary>
        /// <param name="timeToDisable"></param>
        public void Disable(float timeToDisable)
        {
#if USE_POOL
            if (timeToDisable > 0)
                StartCoroutine(DisableDelay(timeToDisable));
            else
                DisableObject();
#else
            if (gameObject != null)
                Destroy(gameObject, timeToDisable);
#endif
        }

        /// <summary>
        /// Немедленное отключение объекта
        /// </summary>
        public void Disable()
        {
#if USE_POOL
            DisableObject();
#else
            if (gameObject != null)
                Destroy(gameObject);
#endif
        }


        protected virtual void DisableObject()
        {
            if (this == null)
                return;

            gameObject.SetActive(false);
            transform.SetParent(PoolManager.PoolParent.transform);
            m_IsEnabled = false;

            if (OnDisable != null)
                OnDisable(this);
        }

        IEnumerator DisableDelay(float time)
        {
            yield return new WaitForSeconds(time);

            DisableObject();
        }
    }
}