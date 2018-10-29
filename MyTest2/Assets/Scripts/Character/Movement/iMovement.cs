using UnityEngine;

namespace mytest2.Character.Movement
{
    /// <summary>
    /// Интерфейс для всех типов передвижений
    /// </summary>
    public interface iMovement
    {
        void Init();
        void Move(Vector3 mDir);
        void Rotate(float angle);
    }
}
