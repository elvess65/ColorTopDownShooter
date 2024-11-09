using mytest2.Character.Abilities;
using mytest2.Utils.Pool;
using UnityEngine;

namespace mytest2.Items
{
    /// <summary>
    /// Предмет, который можно подобрать
    /// </summary>
    public class Item : PoolObject
    {
        public AbilityTypes Type;

        public void Pick()
        {
            Disable();
        }
    }
}
