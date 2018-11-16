using UnityEngine;

namespace mytest2.Character.Abilities
{
    public class AbilityFocusAssistant : MonoBehaviour
    {
        public void InitForAbility(AbilityTypes type)
        {

        }

        public Vector2 GetFocusedDir(AbilityTypes type, Vector2 originDir)
        {
            for (int i = 0; i < GameManager.Instance.Enemies.Count; i++)
            {
                Debug.Log(GameManager.Instance.Enemies[i].name);
            }

            return originDir;
        }
    }
}
