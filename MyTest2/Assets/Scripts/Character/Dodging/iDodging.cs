using UnityEngine;

namespace mytest2.Character.Dodging
{
    /// <summary>
    /// Интерфейс для всех типов уклонений
    /// </summary>
    public interface iDodging
    {
        int Stamina { get; }

        event System.Action OnDodgeStarted;
        event System.Action OnDodgeFinished;

        void Init();
        void Dodge(Vector2 dir);
    }
}
