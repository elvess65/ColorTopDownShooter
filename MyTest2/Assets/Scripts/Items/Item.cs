using mytest2.Character.Abilities;
using UnityEngine;

namespace mytest2.Items
{
    /// <summary>
    /// Предмет, который можно подобрать
    /// </summary>
    public class Item : MonoBehaviour
    {
        public AbilityTypes Type;

        public void Pick()
        {
            Destroy(gameObject);
        }
    }
}
