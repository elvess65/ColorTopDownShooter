using System.Collections.Generic;

namespace mytest2.Character.Container
{
    /// <summary>
    /// Контейнер для всех щитов в сцене
    /// </summary>
    public class ShieldDataContainer
    {
        private List<Shield.Shield> m_ActiveShields;

        public int ShieldsCount
        { get { return m_ActiveShields.Count; } }

        public ShieldDataContainer()
        {
            m_ActiveShields = new List<Shield.Shield>();
        }

        public void AddShield(Shield.Shield shield)
        {
            m_ActiveShields.Add(shield);
        }

        public void RemoveShield(Shield.Shield shield)
        {
            if (m_ActiveShields.Contains(shield))
                m_ActiveShields.Remove(shield);
        }

        public Shield.Shield GetShield(int index)
        {
            return m_ActiveShields[index];
        }
    }
}
