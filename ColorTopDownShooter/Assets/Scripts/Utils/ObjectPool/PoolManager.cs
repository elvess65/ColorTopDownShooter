using System.Collections.Generic;
using UnityEngine;

namespace mytest2.Utils.Pool
{
    /// <summary>
    /// Контроллер пулла
    /// </summary>
    public class PoolManager
    {
        private static GameObject m_PoolParent;
        private static Dictionary<int, ObjectPool> m_Pools = new Dictionary<int, ObjectPool>();

        public static GameObject PoolParent
        {
            get
            {
                if (m_PoolParent == null)
                {
                    m_PoolParent = new GameObject();
                    m_PoolParent.name = "DisabledPoolObjects";
                    m_PoolParent.transform.position = new Vector3(1000, 1000, 1000);
                }

                return m_PoolParent;
            }
        }

        /// <summary>
        /// Используеться для инициализации определенными объектами (вызываеться один раз)
        /// </summary>
        public static void InitializePoolWithObjects(PoolObject source, int count)
        {
            GetPool(source.GetHashCode()).InitializeObjects(source, count);
        }

        /// <summary>
        /// Обнулить пулл
        /// </summary>
        public static void Reset()
        {
            try
            {
                foreach (ObjectPool pool in m_Pools.Values)
                    pool.Reset();
            }
            catch (System.Exception e)
            {
                Debug.LogError("POOL. PoolManager: CATHING EXCEPTION. ERROR RESETING POOL: " + e.Message + ". POOL LISЕ IS NULL: " + (m_Pools == null));
            }

            m_Pools.Clear();
        }

        /// <summary>
        /// Создать объект или взять из пулла
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static PoolObject GetObject(PoolObject source)
        {
            try
            {
                return GetPool(source.GetHashCode()).GetObject(source);
            }
            catch (System.Exception e)
            {
                Debug.LogError("Error " + e);
            }

            return null;
        }


        static ObjectPool GetPool(int objectName)
        {
            if (m_Pools.ContainsKey(objectName))
                return m_Pools[objectName];

            ObjectPool pool = new ObjectPool();
            m_Pools.Add(objectName, pool);

            return pool;
        }

        /*
        #if USE_POOL
            PoolManager.Instance.GetObject();
        #else
        #endif
        */
    }
}
