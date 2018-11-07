using mytest2.Character.Abilities;
using mytest2.Utils.Pool;
using UnityEngine;

namespace mytest2.Character.Shield
{
    /// <summary>
    /// Щит
    /// </summary>
    public class ShieldObject : PoolObject
    {
        public void Init(AbilityTypes type, Vector3 origin, float radius, float angle, int existTimeMiliseconds)
        {
            transform.localScale = new Vector3(radius * 2, radius * 2, radius * 2);
            Destroy(gameObject, existTimeMiliseconds / 1000f);
        }


        void Update()
        {

        }
    }
}
