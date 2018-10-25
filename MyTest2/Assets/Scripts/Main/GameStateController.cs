using mytest2.Character;
using UnityEngine;

namespace mytest2.Main
{
    public class GameStateController : MonoBehaviour
    {
        private GameOverController m_GameOverController;
        private CreatureController m_Player;

        public CreatureController Player
        {
            get { return m_Player; }
            set { m_Player = value; }
        }

        public void GameOver()
        {
            m_GameOverController.GameOver();
        }

        void Start()
        {
            m_GameOverController = GetComponent<GameOverController>();
        }
    }
}
