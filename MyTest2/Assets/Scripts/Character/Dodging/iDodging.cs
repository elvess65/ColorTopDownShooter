using UnityEngine;

namespace mytest2.Character.Dodging
{
    /// <summary>
    /// Интерфейс для всех типов уклонений
    /// </summary>
    public interface iDodging
    {
        event System.Action OnDodgeStarted;
        event System.Action OnDodgeFinished;

        bool IsDodging { get; }

        void Init();
        void Dodge(Vector2 dir);
    }
}
