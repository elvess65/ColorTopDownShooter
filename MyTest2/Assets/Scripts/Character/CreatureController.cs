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
        protected AbilityController m_AbilityController;
        protected StaminaController m_StaminaController;
        //TODO
        //Health
        //Attack

        public abstract void Move(Vector3 dir);

        /// <summary>
        /// Начать выполнять уклон (за реализацию уклона отвечает компонент iDodging)
        /// </summary>
        /// <param name="dir">Направление уклона</param>
        public void Dodge(Vector2 dir)
        {
            if (m_StaminaController.HasEnoughStamina(m_DodgeController.Stamina))
            {
                m_DodgeController.Dodge(dir);
                m_StaminaController.ReduceStamina(m_DodgeController.Stamina);
            }
            else
                Debug.LogWarning("Not enought stamina");
           
        }

        /// <summary>
        /// Использовать способность
        /// </summary>
        /// <param name="type">Тип способности</param>
        /// <param name="dir">направление способности</param>
        public void UseAbility(AbilityTypes type, Vector2 dir)
        {
            if (!m_DodgeController.IsDodging)
            {
                if (m_StaminaController.HasEnoughStamina(GameManager.Instance.GameState.DataTableAbilities.GetAbilityData(type).Stamina))
                {
                    if (m_AbilityController.UseAbility(type, dir))
                        m_StaminaController.ReduceStamina(GameManager.Instance.GameState.DataTableAbilities.GetAbilityData(type).Stamina);
                }
                else
                    Debug.LogWarning("Not enought stamina");
            }
        }
    
        protected virtual void Start()
        {
            InitializeControllers();
            SubscribeForInputEvents();
            SubscribeForControllerEvents();
            FinishInitialization();
        }
        protected virtual void SubscribeForInputEvents()
        { }
        protected virtual void SubscribeForControllerEvents()
        { }
        protected virtual void FinishInitialization()
        { }

        private void InitializeControllers()
        {
            m_MoveController = GetComponent<iMovement>();
            m_MoveController.Init();

            m_DodgeController = GetComponent<iDodging>();
            m_DodgeController.Init();

            m_AbilityController = GetComponent<AbilityController>();
            m_AbilityController.Init();

            m_StaminaController = GetComponent<StaminaController>();
            m_StaminaController.Init();
        }
    }
}
