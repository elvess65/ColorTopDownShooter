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

        private float m_TargetAngleToUseAbility = -1;
        private Vector2 m_AbilityDir;
        private const float m_DELTA_TO_ANGLE = 0.1f;
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
            m_AbilityDir = dir;
            m_TargetAngleToUseAbility = Mathf.Atan2(dir.x, dir.y) * Mathf.Rad2Deg;

            /*if (!m_DodgeController.IsDodging)
            {
                if (m_StaminaController.HasEnoughStamina(GameManager.Instance.GameState.DataTableAbilities.GetAbilityData(type).Stamina))
                {
                    if (m_AbilityController.UseAbility(type, dir))
                        m_StaminaController.ReduceStamina(GameManager.Instance.GameState.DataTableAbilities.GetAbilityData(type).Stamina);
                }
                else
                    Debug.LogWarning("Not enought stamina");
            }*/
        }
    

        protected virtual void Start()
        {
            InitializeControllers();
            SubscribeForInputEvents();
            SubscribeForControllerEvents();
            FinishInitialization();
        }
        protected virtual void Update()
        {
            if (m_TargetAngleToUseAbility > -1)
            {
                if (m_TargetAngleToUseAbility > 0)
                {
                    m_MoveController.Rotate(m_TargetAngleToUseAbility);

                    if (DeltaToDir(m_AbilityDir) < m_DELTA_TO_ANGLE)
                    {
                        Debug.Log("Rotation finished");
                        m_TargetAngleToUseAbility = -1;
                    }
                }
            }
            
            Debug.Log(Vector2.Angle(new Vector2(transform.forward.x, transform.forward.z), m_AbilityDir));
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
        private float DeltaToDir(Vector2 dir)
        {
            Vector2 transformV2 = new Vector2(transform.forward.x, transform.forward.z);
            return Vector2.Angle(new Vector2(transform.forward.x, transform.forward.z), dir);
        }
    }
}
