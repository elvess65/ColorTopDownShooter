using mytest2.Character.Dodging;
using mytest2.Character.Movement;
using UnityEngine;

namespace mytest2.Character
{
    /// <summary>
    /// Общий класс для всех существ в игре
    /// </summary>
    public abstract class CreatureController : MonoBehaviour
    {
        protected iMovement m_MoveController;
        protected iDodging m_DodgeController;
        //TODO
        //Health
        //Attack
        //Move

        protected virtual void Start()
        {
            InitializeControllers();
            SubscribeForInputEvents();
            SubscribeForControllerEvents();
        }

        protected void InitializeControllers()
        {
            m_MoveController = GetComponent<iMovement>();
            m_MoveController.Init();

            m_DodgeController = GetComponent<iDodging>();
            m_DodgeController.Init();
        }

        public abstract void Move(Vector3 dir);
        public abstract void Dodge(Vector2 dir);

        protected virtual void SubscribeForInputEvents()
        { }

        protected virtual void SubscribeForControllerEvents()
        { }
    }
}
