using mytest2.Character.Abilities;
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
        protected CreatureAbilityController m_AbilityController;
        //TODO
        //Health
        //Attack

        public abstract void Move(Vector3 dir);
        public abstract void Dodge(Vector2 dir);

        public virtual void UseAbility(AbilityTypes type, Vector2 dir)
        {
            m_AbilityController.UseAbility(type, dir);
        }
    
        protected virtual void Start()
        {
            InitializeControllers();
            SubscribeForInputEvents();
            SubscribeForControllerEvents();
        }
        protected virtual void SubscribeForInputEvents()
        { }
        protected virtual void SubscribeForControllerEvents()
        { }

        protected void InitializeControllers()
        {
            m_MoveController = GetComponent<iMovement>();
            m_MoveController.Init();

            m_DodgeController = GetComponent<iDodging>();
            m_DodgeController.Init();

            m_AbilityController = GetComponent<CreatureAbilityController>();
            m_AbilityController.Init();
        }
    }
}
