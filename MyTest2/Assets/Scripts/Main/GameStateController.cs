using mytest2.Character;
using mytest2.Character.Abilities;
using UnityEngine;

namespace mytest2.Main
{
    public class GameStateController : MonoBehaviour
    {
        private GameOverController m_GameOverController;
        private DataTableAbilities m_DataTableAbilities;

        public CreatureController Player
        {
            get; set;
        }
        public DataTableAbilities DataTableAbilities
        {
            get; private set;
        }

        public void GameOver()
        {
            m_GameOverController.GameOver();
        }

        void Start()
        {
            m_GameOverController = GetComponent<GameOverController>();
            DataTableAbilities = GetComponent<DataTableAbilities>();
        }
    }
}
