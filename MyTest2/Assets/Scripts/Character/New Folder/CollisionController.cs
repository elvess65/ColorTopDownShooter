using UnityEngine;

public abstract class CollisionController : MonoBehaviour
{
    public abstract void HandleEnterCollision(Collider other);

    public abstract void HandlerExitCollision(Collider other);
}
