using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace mytest2.Character.Collisions
{
	public class ProjectileCollisionController : TriggerCollisionController 
	{
		public override void HandleEnterCollision(Collider other)
		{
			if (!GameManager.Instance.IsActive)
				return;

			string objTag = other.tag;
			/*switch (objTag)
	        {
	            case m_ACTIONFIELD_INTERACTABLE:
	                other.GetComponent<iInteractable>().Interact();
	                break;
	            case m_OBJECT_ENEMY:
	                GetComponent<PlayerController>().DestroyPlayer(); 
	                other.GetComponent<EnemyController>().TakeDamage(transform);
	                break;
	            case m_RESTART:
	                GameManager.Instance.RestartRound();
	                break;
	        }*/
		}

		public override void HandlerExitCollision(Collider other)
		{
		}
	}
}
