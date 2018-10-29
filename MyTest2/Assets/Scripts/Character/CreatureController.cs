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
        //TODO
        //Health
        //Attack
        //Move

        protected virtual void Start()
        {
            InitializeControllers();
        }

        protected void InitializeControllers()
        {
            m_MoveController = GetComponent<iMovement>();
            m_MoveController.Init();
        }

        public abstract void Move(Vector3 dir);
        public abstract void Dodge(Vector3 dir);
    }
}
