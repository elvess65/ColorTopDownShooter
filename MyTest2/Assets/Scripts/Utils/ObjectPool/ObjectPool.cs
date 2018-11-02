using System.Collections.Generic;
using UnityEngine;

namespace mytest2.Utils.Pool
{
    /// <summary>
    /// Пулл объектов
    /// </summary>
    public class ObjectPool
    {
        private Stack<PoolObject> m_Pool;

        public ObjectPool()
        {
            m_Pool = new Stack<PoolObject>();
        }

        /// <summary>
        /// Инициализация пулла объектами
        /// </summary>
        /// <param name="source">Source</param>
        /// <param name="count">Count</param>
        public void InitializeObjects(PoolObject source, int count)
        {
            for (int i = 0; i < count; i++)
            {
                PoolObject obj = MonoBehaviour.Instantiate(source);
                obj.OnDisable += AddToPool;
                obj.Disable();
            }
        }

        /// <summary>
        /// Удаление всех объектов из пулла
        /// </summary>
        public void Reset()
        {
            try
            {
                foreach (PoolObject ob in m_Pool)
                {
                    if (ob != null)
                        MonoBehaviour.Destroy(ob.gameObject);
                    else
                        Debug.LogError("POOL. ObjectPool: ERROR DESTOYING POOL OBJECT " + (ob == null));
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("POOL. ObjectPool: CATHING EXCEPTION. ERROR RESETING POOL: " + e.Message);
            }

            m_Pool.Clear();
        }

        /// <summary>
        /// Получить объект из пулла или создать недостающий
        /// </summary>
        /// <returns>The object.</returns>
        /// <param name="source">Source.</param>
        public PoolObject GetObject(PoolObject source)
        {
            PoolObject result = null;
            if (m_Pool.Count == 0)
            {
                result = MonoBehaviour.Instantiate(source);
                result.OnDisable += AddToPool;
            }
            else
                result = m_Pool.Pop();

            if (result != null)
            {
                result.Enable();
            }
            else
            {
                Debug.LogError("ERROR WHILE GETTING POOL OBJECT! SUPPOSEDLY: Error UnityEngine.MissingReferenceException. Source is null: " + (source == null) + " Result is null: " + (result == null) + " Source name: " + (source != null ? source.gameObject.name : " is null"));

                result = MonoBehaviour.Instantiate(source);
                result.OnDisable += AddToPool;
            }

            return result;
        }

        void AddToPool(PoolObject ob)
        {
            m_Pool.Push(ob);
        }
    }
}
