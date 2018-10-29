using UnityEngine;

namespace mytest2.Character.Movement
{
    /// <summary>
    /// Передвижение при помощи CharacterController
    /// </summary>
    public class Movement_CharacterController : MonoBehaviour, iMovement
    {
        public float MoveSpeed = 3;
        public float RotationSpeed = 5;

        private CharacterController m_CharacterController;

        public void Init()
        {
            m_CharacterController = transform.GetComponent<CharacterController>();
        }

        public void Move(Vector3 mDir)
        {
            m_CharacterController.SimpleMove(mDir * MoveSpeed);
        }

        public void Rotate(float angle)
        {
            Quaternion targetRot = Quaternion.AngleAxis(angle, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * RotationSpeed);
        }
    }
}
