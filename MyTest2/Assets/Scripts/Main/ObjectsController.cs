using mytest2.Character.Shield;
using System.Collections.Generic;
using UnityEngine;

namespace mytest2.Main
{
    /// <summary>
    /// Контроллер динамических объектов внутри сцены
    /// </summary>
    public class ObjectsController : MonoBehaviour
    {
        private List<Shield> m_ActiveShields;

        public int ShieldsCount
        {
            get
            {
                if (m_ActiveShields == null)
                    return 0;

                return m_ActiveShields.Count;
            }
        }

        public void AddShield(Shield shield)
        {
            if (m_ActiveShields == null)
                m_ActiveShields = new List<Shield>();

            m_ActiveShields.Add(shield);
        }

        public void RemoveShield(Shield shield)
        {
            if (m_ActiveShields.Contains(shield))
                m_ActiveShields.Remove(shield);
        }

        public Shield GetShield(int index)
        {
            return m_ActiveShields[index];
        }
    }
}
