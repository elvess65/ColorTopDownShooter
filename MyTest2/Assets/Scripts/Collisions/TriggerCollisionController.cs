using UnityEngine;

namespace mytest2.Character.Collisions
{
    /// <summary>
    /// Контроллер обработки взаимодействия с триггером (OnTriggerEnter)
    /// </summary>
	public class TriggerCollisionController : MonoBehaviour
	{
		public System.Action<Collider> OnTriggerEnterEvent;

		void OnTriggerEnter(Collider other)
		{
			if (OnTriggerEnterEvent != null)
				OnTriggerEnterEvent (other);
		}
	}
}
