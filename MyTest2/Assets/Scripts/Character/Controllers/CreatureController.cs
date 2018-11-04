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

        private const float m_DELTA_ANGLE_TO_DIR = 0.1f;
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
        public void TryUseAbility(AbilityTypes type, Vector2 dir)
        {
            if (!m_DodgeController.IsDodging)
            {
                if (m_StaminaController.HasEnoughStamina(GameManager.Instance.GameState.DataTableAbilities.GetAbilityData(type).Stamina))
                {
					if (m_AbilityController.CanUseABility (type)) 
						HandleUseAbility (type, dir);
                }
                else
                    Debug.LogWarning("Not enought stamina");
            }
        }
    
		protected bool m_IsRotating2Ability = false;
		protected float m_CachedAbilityAngle;
		private Vector2 m_CachedAbilityDir;
		private System.Action m_OnRotation2AbilityFinished;


        protected virtual void Start()
        {
            InitializeControllers();
            SubscribeForInputEvents();
            SubscribeForControllerEvents();
            FinishInitialization();
        }
        protected virtual void Update()
        {
			HandleRotatation2AbilityDir ();
        }
        protected virtual void SubscribeForInputEvents() { }
        protected virtual void SubscribeForControllerEvents()
        {
			m_AbilityController.OnAbilityUse += (AbilityTypes type) => 
			{
				m_StaminaController.ReduceStamina(GameManager.Instance.GameState.DataTableAbilities.GetAbilityData(type).Stamina);
			};
		}
        protected virtual void FinishInitialization() { }
		protected virtual void HandleUseAbility(AbilityTypes type, Vector2 dir)
		{
			m_OnRotation2AbilityFinished = () => 
			{
				m_AbilityController.TryUseAbility (type, dir);
			};

			m_CachedAbilityDir = dir;
			m_CachedAbilityAngle = Mathf.Atan2(dir.x, dir.y) * Mathf.Rad2Deg;

			m_IsRotating2Ability = true;
		}


		private void HandleRotatation2AbilityDir()
		{
			if (m_IsRotating2Ability)
			{
				m_MoveController.Rotate(m_CachedAbilityAngle);

				if (DeltaAngleToDir(m_CachedAbilityDir) < m_DELTA_ANGLE_TO_DIR)
				{
					Debug.Log("Rotation finished");
					m_IsRotating2Ability = false;

					if (m_OnRotation2AbilityFinished != null)
						m_OnRotation2AbilityFinished ();
				}
			}
		}

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
        private float DeltaAngleToDir(Vector2 dir)
        {
            Vector2 transformV2 = new Vector2(transform.forward.x, transform.forward.z);
            return Vector2.Angle(new Vector2(transform.forward.x, transform.forward.z), dir);
        }
    }
}
