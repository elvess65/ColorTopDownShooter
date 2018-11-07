using mytest2.Character.Abilities;
using mytest2.Utils;
using mytest2.Utils.Pool;
using UnityEngine;

namespace mytest2.Character.Shield
{
    /// <summary>
    /// Щит
    /// </summary>
    public class ShieldObject : PoolObject
    {
        public System.Action OnTimeElapsed;

        private bool m_IsActive = false;
        private float m_FinishTime;

        public void Init(AbilityTypes type, Vector3 origin, float radius, float angle, int existTimeMiliseconds)
        {
            //Внешний вид щита
            transform.localScale = new Vector3(radius * 2, radius * 2, radius * 2);

            //Таймер щита
            m_FinishTime = Time.time + existTimeMiliseconds / 1000f;
            m_IsActive = true;
        }

        void Update()
        {
            if (m_IsActive)
            {
                if (Time.time >= m_FinishTime)
                {
                    m_IsActive = false;

                    Disable();

                    if (OnTimeElapsed != null)
                        OnTimeElapsed();
                }
            }
        }
    }
}
