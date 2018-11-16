using UnityEngine;

namespace mytest2.Character.Abilities
{
    public class AbilityFocusAssistant : MonoBehaviour
    {
        private float m_AbilityLengthSQR;

        public void InitForAbility(AbilityTypes type)
        {
            float abilityLength = GameManager.Instance.GameState.DataTableAbilities.GetAbilityData(type).Length;
            m_AbilityLengthSQR = abilityLength * abilityLength;
        }

        public Vector2 GetFocusedDir(AbilityTypes type, Vector2 originDir)
        {
            for (int i = 0; i < GameManager.Instance.Enemies.Count; i++)
            {
                Vector3 dirToEnemy = GameManager.Instance.Enemies[i].transform.position - transform.position;
                if (dirToEnemy.sqrMagnitude <= m_AbilityLengthSQR)
                    return new Vector2(dirToEnemy.x, dirToEnemy.z).normalized;
            }

            return originDir;
        }
    }
}
