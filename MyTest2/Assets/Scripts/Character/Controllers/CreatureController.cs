using mytest2.Character.Abilities;
using mytest2.Character.Collisions;
using mytest2.Character.Dodging;
using mytest2.Character.Health;
using mytest2.Character.Movement;
using mytest2.Character.Shield;
using mytest2.Items;
using mytest2.Projectiles;
using UnityEngine;

namespace mytest2.Character
{
    /// <summary>
    /// Общий класс для всех существ в игре
    /// </summary>
    [RequireComponent(typeof(iMovement))]
    [RequireComponent(typeof(iDodging))]
    [RequireComponent(typeof(AbilityController))]
    [RequireComponent(typeof(StaminaController))]
    [RequireComponent(typeof(HealthController))]
    [RequireComponent(typeof(TriggerCollisionController))]
    [RequireComponent(typeof(ShieldController))]
    public abstract class CreatureController : MonoBehaviour
    {
        public int TeamID;

        protected iMovement m_MoveController;
        protected iDodging m_DodgeController;
        protected AbilityController m_AbilityController;
        protected StaminaController m_StaminaController;
        protected HealthController m_HealthController;
        protected TriggerCollisionController m_CollisionController;
        protected ShieldController m_ShieldController;

        protected bool m_IsRotating2Ability = false;
        protected float m_CachedAbilityAngle;
        private Vector2 m_CachedAbilityDir;
        private System.Action m_OnRotation2AbilityFinished;
        private const float m_DELTA_ANGLE_TO_DIR = 0.1f;


        public abstract void Move(Vector2 dir);

        /// <summary>
        /// Начать выполнять уклон (за реализацию уклона отвечает компонент iDodging)
        /// </summary>
        /// <param name="dir">Направление уклона в 2D координатах</param>
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
        /// <param name="dir">Направление способности в 2D координатах</param>
        public void TryUseAbility(AbilityTypes type, Vector2 dir)
        {
            if (!m_DodgeController.IsDodging)
            {
                if (m_StaminaController.HasEnoughStamina(GameManager.Instance.GameState.DataTableAbilities.GetAbilityData(type).Stamina))
                {
                    if (m_AbilityController.CanUseABility(type))
                        HandleUseAbility(type, dir);
                }
                else
                    Debug.LogWarning("Not enought stamina");
            }
            else
                Debug.Log("Cant use ability: Is dodging");
        }
    
        /// <summary>
        /// Нанести урон
        /// </summary>
        public void TakeDamage(AbilityTypes type, int damage)
        {
            m_HealthController.TakeDamage(type, damage);
        }

        /// <summary>
        /// Создать щит
        /// </summary>
        /// <param name="position">Позиция, где находиться игрок, который создал щит</param>
        /// <param name="origin">Ветор направления щита</param>
        /// <param name="angle">Угол щита</param>
        /// <param name="type">Тип щита</param>
        public void CreateShield(Vector3 origin, float angle, AbilityTypes type)
        {
            m_ShieldController.CreateShield(transform.position, origin, angle, type, TeamID);
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
			HandleRotatation2AbilityDir ();
        }
        //Инициализация
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

            m_HealthController = GetComponent<HealthController>();
            m_HealthController.Init();

            m_ShieldController = GetComponent<ShieldController>();
            m_ShieldController.Init();

            m_CollisionController = GetComponent<TriggerCollisionController>();
        }                 //Инициализация всех контроллеров
        protected virtual void SubscribeForInputEvents() { }    //Подписаться на события ввода
        protected virtual void SubscribeForControllerEvents()   //Подписаться на события контроллеров
        {
            //Способности
			m_AbilityController.OnAbilityUse += (AbilityTypes type) => 
			{
				m_StaminaController.ReduceStamina(GameManager.Instance.GameState.DataTableAbilities.GetAbilityData(type).Stamina);
			};

            //ХП
            m_HealthController.OnDestroy += HandleDestroyCreature;
            m_HealthController.OnWrongAbility += HandleWrongAbilityDamage;

            //Взаимодейтсвие с триггером
            m_CollisionController.OnTriggerEnterEvent = HandleTriggerEnter;
        }
        protected virtual void FinishInitialization() { }       //Окончание инициализации
        //Обработка
		protected virtual void HandleUseAbility(AbilityTypes type, Vector2 dir)
		{
            //Начать вращение в направлении применения способности

            //Событие окончания вращения
			m_OnRotation2AbilityFinished = () => 
			{
				m_AbilityController.TryUseAbility (type, dir, TeamID);
			};

            //Направление способности и угол, на который надо повернуться, для применения
			m_CachedAbilityDir = dir;
			m_CachedAbilityAngle = Mathf.Atan2(dir.x, dir.y) * Mathf.Rad2Deg;

            //Начать вращение в направлении способности
			m_IsRotating2Ability = true;
		}
        protected virtual void HandleTriggerEnter(Collider collider)
        {
            Projectile projectile = collider.GetComponent<Projectile>();
            if (projectile != null)
                TakeDamage(projectile.Type, GameManager.Instance.GameState.DataTableAbilities.GetAbilityData(projectile.Type).Damage);

            Item item = collider.GetComponent<Item>();
            if (item != null)
            {
                m_AbilityController.AddAmmo(item.Type, 1);
                item.Pick();
            }
        }
        private void HandleRotatation2AbilityDir()
        {
            if (m_IsRotating2Ability)
            {
                m_MoveController.Rotate(m_CachedAbilityAngle);

                if (DeltaAngleToDir(m_CachedAbilityDir) < m_DELTA_ANGLE_TO_DIR)
                {
                    m_IsRotating2Ability = false;

                    if (m_OnRotation2AbilityFinished != null)
                        m_OnRotation2AbilityFinished();
                }
            }
        }

        protected virtual void HandleDestroyCreature()
        {
            Debug.Log("Is destroyed");
        }
        protected virtual void HandleWrongAbilityDamage(AbilityTypes type)
        {
            Debug.Log("Wrong ability: " + type);
        }

        private float DeltaAngleToDir(Vector2 dir)
        {
            return Vector2.Angle(new Vector2(transform.forward.x, transform.forward.z), dir);
        }
    }
}
