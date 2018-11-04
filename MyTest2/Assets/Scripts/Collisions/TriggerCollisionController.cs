using UnityEngine;

namespace mytest2.Character.Collisions
{
	public abstract class TriggerCollisionController : MonoBehaviour
	{
		public System.Action<Collider> OnTriggerEnterEvent;

	    public abstract void HandleEnterCollision(Collider other);

	    public abstract void HandlerExitCollision(Collider other);


		void OnTriggerEnter(Collider other)
		{
			HandleEnterCollision (other);

			if (OnTriggerEnterEvent != null)
				OnTriggerEnterEvent (other);
		}
	}
}
