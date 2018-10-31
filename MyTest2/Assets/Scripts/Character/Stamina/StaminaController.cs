using UnityEngine;

namespace mytest2.Character.Abilities
{
    /// <summary>
    /// Котроллер силы персонажа (задает количество силы и тратит ее на определенные действия)
    /// </summary>
    public class StaminaController : MonoBehaviour
    {
        public System.Action<float> OnStaminaUpdate;

        public int Stamina = 10;
        public float IncreasePerSecond = 1;

        private float m_CurStamina = 0;

        public void Init()
        {
            m_CurStamina = Stamina;
        }

        /// <summary>
        /// Уменьшить количество силы
        /// </summary>
        /// <param name="amount">На сколько уменьшить количество силы</param>
        public void ReduceStamina(int amount)
        {
            m_CurStamina = Mathf.Clamp(m_CurStamina - amount, 0, Stamina);
        }

        /// <summary>
        /// Хватает ли силы для совершения действия
        /// </summary>
        /// <param name="amount">Количество силы для действия</param>
        /// <returns>true - если силы хватает</returns>
        public bool HasEnoughStamina(int amount)
        {
            return m_CurStamina >= amount;
        }
        

        void Update()
        {
            if (m_CurStamina < Stamina)
            {
                m_CurStamina += IncreasePerSecond * Time.deltaTime;

                if (OnStaminaUpdate != null)
                    OnStaminaUpdate(m_CurStamina / Stamina);

                if (m_CurStamina >= Stamina)
                    m_CurStamina = Stamina;
            }
        }
    }
}
