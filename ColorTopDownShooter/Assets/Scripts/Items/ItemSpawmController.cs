using mytest2.Character.Abilities;
using mytest2.Utils.Pool;
using UnityEngine;

namespace mytest2.Items
{
    /// <summary>
    /// Контроллер создания предметов
    /// </summary>
    public class ItemSpawmController : MonoBehaviour
    {
        public void SpawnItems()
        {
            Vector3 center = GameManager.Instance.GameState.Player.transform.position;
            for (int i = 0; i < (int)AbilityTypes.None; i++)
            {
                Vector3 pos = PositionOnCircle(center, 5.0f, i * 45);
                Item item = PoolManager.GetObject(GameManager.Instance.PrefabLibrary.GetAbilityItemPrefab((AbilityTypes)i)) as Item;
                item.transform.position = pos;
            }
        }

        Vector3 PositionOnCircle(Vector3 center, float radius, int angle)
        {
            float ang = angle;
            Vector3 pos;
            pos.x = center.x + radius * Mathf.Sin(ang * Mathf.Deg2Rad);
            pos.y = center.y;
            pos.z = center.z + radius * Mathf.Cos(ang * Mathf.Deg2Rad);
            return pos;
        }
    }
}
