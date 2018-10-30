using mytest2.Character;
using mytest2.Character.Abilities;
using UnityEngine;

namespace mytest2.Main
{
    public class GameStateController : MonoBehaviour
    {
        private GameOverController m_GameOverController;
        private DataTableAbilities m_DataTableAbilities;
        private SelectedAbilityController m_SelectedAbilityController;
        private CreatureController m_Player;

        public CreatureController Player
        {
            get { return m_Player; }
            set { m_Player = value; }
        }
        public DataTableAbilities DataTableAbilities
        {
            get { return m_DataTableAbilities; }
        }
        public SelectedAbilityController SelectedAbilityController
        {
            get { return m_SelectedAbilityController; }
        }

        public void GameOver()
        {
            m_GameOverController.GameOver();
        }

        void Start()
        {
            m_GameOverController = GetComponent<GameOverController>();
            m_DataTableAbilities = GetComponent<DataTableAbilities>();
            m_SelectedAbilityController = GetComponent<SelectedAbilityController>();
        }
    }
}
