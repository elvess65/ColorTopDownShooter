using UnityEngine;

public class PlayerCollisionController : CollisionController
{
    private const string m_ACTIONFIELD_INTERACTABLE =    "Intaractable";
    private const string m_OBJECT_ENEMY =                "Enemy";
    private const string m_RESTART =                     "Restart";

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
        if (!GameManager.Instance.IsActive)
            return;

        string objTag = other.tag;
        /*switch (objTag)
        {
            case m_ACTIONFIELD_INTERACTABLE:
                other.GetComponent<iExitableFromInteractionArea>().ExitFromInteractableArea();
                break;
        }*/
    }
}
