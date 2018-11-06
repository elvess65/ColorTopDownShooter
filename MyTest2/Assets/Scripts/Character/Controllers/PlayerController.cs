using mytest2.Character.Abilities;
using mytest2.UI.Controllers3D;
using mytest2.UI.InputSystem;
using mytest2.Utils.Pool;
using UnityEngine;

namespace mytest2.Character
{
    public class PlayerController : CreatureController
    {
        private UIPlayerActionDirectionController m_UIActionDirectionController;
		private Vector3 m_TargetMoveDir = Vector3.zero;
        private float m_TargetRotAngle;

        private static AbilityTypes m_CurAbilityType = AbilityTypes.None;

        public static AbilityTypes SelectedAbility
        { 
            get { return m_CurAbilityType; }
        }

        protected override void Update()
        {
            base.Update();

			//Передвижение и вращение если персонаж не уклоняеться или не вращаеться к направлению способности (применение)
			if (!m_DodgeController.IsDodging && !m_IsRotating2Ability)
            {
                m_MoveController.Move(m_TargetMoveDir);
                m_MoveController.Rotate(m_TargetRotAngle);
            }
        }

        /// <summary>
        /// Сохранить данные для передвижения (вызов реализации происходит в Update и за нее отвечает компонент iMovement)
        /// </summary>
        /// <param name="dir"></param>
        public override void Move(Vector3 dir)
        {
            //Кэш направления передвижения
            m_TargetMoveDir = dir;

            //Если игрок хочет переместиться
            if (m_TargetMoveDir != Vector3.zero)
            {
                //Вращение в направлении движения
                m_TargetRotAngle = Mathf.Atan2(m_TargetMoveDir.x, m_TargetMoveDir.z) * Mathf.Rad2Deg;
            }
        }


        /// <summary>
        /// Нажатие на джойстик уклона
        /// </summary>
        void DodgeInputTouchStart(Vector2 dir)
        {
            ShowUIActionDirectionController(AbilityTypes.None);
        }

        /// <summary>
        /// Перемещение джойстика уклона 
        /// </summary>
        void DodgeInputDrag(Vector2 dir)
        {
            UpdateUIActionDirectionController(dir);
        }

        /// <summary>
        /// Окончание нажатия на джойстик уклона
        /// </summary>
        /// <param name="dir"></param>
        void DodgeInputTouchEnd(Vector2 dir)
        {
            //Спрятать указатель направления
            HideUIActionDirectionController();
        }


        /// <summary>
        /// Персонаж начал выполнять уклон
        /// </summary>
        void DodgeStartedHandler()
        {
            Debug.Log("DodgeStarted");
            //TODO: Translate to animation
        }

        /// <summary>
        /// Персонаж закончил выполнять уклон
        /// </summary>
        void DodgeFinishedHandler()
        {
            Debug.Log("DodgeFinished");
            //TODO: Translate to animation
        }


        /// <summary>
        /// Активация способности
        /// </summary>
        /// <param name="type">Тип способности</param>
        void AbilityInputActivate(AbilityTypes type)
        {
            ShowUIActionDirectionController(type);
        }

        /// <summary>
        /// Выбор способности
        /// </summary>
        /// <param name="abilityType">Тип способности</param>
        void AbilityInputSelect(AbilityTypes abilityType)
        {
            if (m_CurAbilityType != abilityType)
            {
                m_CurAbilityType = abilityType;
                GameManager.Instance.UIManager.SelectAbilityJoystick(abilityType);
            }
        }

        /// <summary>
        /// Перемещение джойстика способности 
        /// </summary>
        void AbilityInputDrag(Vector2 dir)
        {
            UpdateUIActionDirectionController(dir);
        }

        /// <summary>
        /// окончание нажатия на джойстик способности
        /// </summary>
        /// <param name="dir">Направление способности</param>
        void AbilityInputTouchEnd(Vector2 dir)
        {
            //Если нельзя использовать способность длина вектора 0 (если способность была только выделена, но не использована)
            if (dir.sqrMagnitude > 0)
                TryUseAbility(SelectedAbility, dir);

            //Спрятать указатель направления
            HideUIActionDirectionController();
        }

        /// <summary>
        /// Обновить состояние джойстика способности (откат)
        /// </summary>
        /// <param name="type">Тип способности</param>
        void AbilityUseHandler(AbilityTypes type)
        {
            DataAbility abilityData = GameManager.Instance.GameState.DataTableAbilities.GetAbilityData(type);
            GameManager.Instance.UIManager.CooldownAbilityJoystick(type, abilityData.CooldownMiliseconds); 
        }

        /// <summary>
        /// Обносить количество заряжов способности
        /// </summary>
        /// <param name="type">Тип способности</param>
        /// <param name="ammoAmmount">Количество зарядов</param>
        void AbilityUpdateAmmoHandler(AbilityTypes type, int ammoAmmount)
        {
            GameManager.Instance.UIManager.UpdateAbilityAmmo(type, ammoAmmount);
        }

        /// <summary>
        /// Вывести состояние силы на UI
        /// </summary>
        /// <param name="progress"></param>
        void StaminaUpdateHandler(float progress)
        {
            GameManager.Instance.UIManager.StaminaController.SetState(progress);
        }

        /// <summary>
        /// Смена состояния ввода
        /// </summary>
        /// <param name="state"></param>
        void InputStatusChangeHandler(bool state)
        {

        }


        public static Vector3 m_ShieldOrigin;
        Vector3 m_InputBound;
        Vector3 m_AutoBound;
        Vector3 m_PerpendicularToOrigin;
        Vector2 m_PerpendicularToOrigin2D;
        public static float m_AngleBetweenOriginAndBound;

        void OnShieldInputStart(Vector2 dirToTarget)
        {
            //Вектор начала щита
            m_ShieldOrigin = new Vector3(dirToTarget.x, 0, dirToTarget.y);
            //Перпендикуляр к началу щита 2D
            m_PerpendicularToOrigin2D = new Vector2(dirToTarget.y, -dirToTarget.x);
            //Перпендикуляр к началу щита 
            m_PerpendicularToOrigin = new Vector3(m_PerpendicularToOrigin2D.x, 0, m_PerpendicularToOrigin2D.y);
            //Граница щита, которая автоматически сдвигаеться
            m_AutoBound = m_ShieldOrigin;
        }
        void OnShieldInputUpdate(Vector2 dirToTarget)
        {
            //Текущая граница контролируемая вводом
            m_InputBound = new Vector3(dirToTarget.x, 0, dirToTarget.y);
            //Угол между началом щита и текущей границей
            m_AngleBetweenOriginAndBound = Vector3.Angle(m_ShieldOrigin, m_InputBound);

            Debug.Log(m_AngleBetweenOriginAndBound);

            //Сравнение Dot с пермендикуляром к началу щита для определения знака угла 
            float dot = Vector2.Dot(m_PerpendicularToOrigin2D, dirToTarget);
            float dir = Mathf.Sign(dot);

            //Автоматическое смещение границы
            m_AutoBound = Quaternion.Euler(0, -m_AngleBetweenOriginAndBound * dir, 0) * m_ShieldOrigin;
        }

        void OnShieldInputEnd()
        {
            m_ShieldController.CreateShield(transform.position, m_ShieldOrigin, m_AngleBetweenOriginAndBound);
        }

        private void OnDrawGizmos()
        {
            //Радиус щита
            Gizmos.DrawWireSphere(transform.position, m_ShieldController.ShieldRadius);

            //Начало щита
            Gizmos.color = Color.white;
            Gizmos.DrawLine(transform.position, transform.position + m_ShieldOrigin * m_ShieldController.ShieldRadius);

            //Перпендикуляр к началу щита
            Gizmos.color = Color.black;
            Gizmos.DrawLine(transform.position, transform.position + m_PerpendicularToOrigin * m_ShieldController.ShieldRadius);

            //Автоматическая граница
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + m_AutoBound * m_ShieldController.ShieldRadius);

            //Граница контролируемая вводом
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, transform.position + m_InputBound * m_ShieldController.ShieldRadius);
        }

        //Инициализация
        protected override void SubscribeForInputEvents()
        {
			base.SubscribeForInputEvents ();

#if UNITY_EDITOR
            if (InputManager.Instance.PreferVirtualJoystickInEditor)
                SubscribeForJoystickEvents();
            else
            {
                InputManager.Instance.KeyboardInput.OnMove += Move;
            
                InputManager.Instance.KeyboardInput.OnDodgeStart += DodgeInputTouchStart;
                InputManager.Instance.KeyboardInput.OnDodgeDrag += DodgeInputDrag;
                InputManager.Instance.KeyboardInput.OnDodge += Dodge;
            }
#else
            SubscribeForJoystickEvents();
#endif

            InputManager.Instance.OnInputStateChange += InputStatusChangeHandler;
            InputManager.Instance.OnInputStart += OnShieldInputStart;
            InputManager.Instance.OnInputUpdate += OnShieldInputUpdate;
            InputManager.Instance.OnInputEnd += OnShieldInputEnd;
        }
        protected override void SubscribeForControllerEvents()
        {
			base.SubscribeForControllerEvents ();

            m_DodgeController.OnDodgeStarted += DodgeStartedHandler;
            m_DodgeController.OnDodgeFinished += DodgeFinishedHandler;

            m_StaminaController.OnStaminaUpdate += StaminaUpdateHandler;
            m_AbilityController.OnAbilityUse += AbilityUseHandler;
            m_AbilityController.OnUpdateAmmo += AbilityUpdateAmmoHandler;
        }
        protected override void FinishInitialization()
        {
			base.FinishInitialization ();

            m_AbilityController.SetAndCallUpdateAmmoEventForAllAbilities();
        }
        private void SubscribeForJoystickEvents()
        {
            //Передвижение
            InputManager.Instance.VirtualJoystickInput.OnMove += Move;

            //Уклон
            InputManager.Instance.VirtualJoystickInput.DodgeJoystickWrapper.OnJoystickTouchStart += DodgeInputTouchStart;
            InputManager.Instance.VirtualJoystickInput.DodgeJoystickWrapper.OnJoystickMove += DodgeInputDrag;
            InputManager.Instance.VirtualJoystickInput.DodgeJoystickWrapper.OnJoystickTouchEnd += Dodge;
            InputManager.Instance.VirtualJoystickInput.DodgeJoystickWrapper.OnJoystickTouchEnd += DodgeInputTouchEnd;

            //Способности
            for (int i = 0; i < InputManager.Instance.VirtualJoystickInput.AbilityJoystickWrappers.Length; i++)
            {
                InputManager.Instance.VirtualJoystickInput.AbilityJoystickWrappers[i].OnAbilityActivate += AbilityInputActivate;
                InputManager.Instance.VirtualJoystickInput.AbilityJoystickWrappers[i].OnAbilitySelect += AbilityInputSelect;
                InputManager.Instance.VirtualJoystickInput.AbilityJoystickWrappers[i].OnJoystickTouchEnd += AbilityInputTouchEnd;
                InputManager.Instance.VirtualJoystickInput.AbilityJoystickWrappers[i].OnJoystickMove += AbilityInputDrag;
            }
        }

        //Обработка
        protected override void HandleUseAbility (AbilityTypes type, Vector2 dir)
		{
			base.HandleUseAbility (type, dir);

			m_TargetRotAngle = m_CachedAbilityAngle;
		}

        //UI направления дейтсвия
        void ShowUIActionDirectionController(AbilityTypes type)
        {
            m_UIActionDirectionController = PoolManager.GetObject(GameManager.Instance.PrefabLibrary.UIAbilityDirectionPrefab) as UIPlayerActionDirectionController;
            m_UIActionDirectionController.transform.position = transform.position;
            m_UIActionDirectionController.Init(transform, type);
        }
        void UpdateUIActionDirectionController(Vector2 dir)
        {
            if (m_UIActionDirectionController != null && m_UIActionDirectionController.IsEnabled)
                m_UIActionDirectionController.SetDirection(dir);
        }
        void HideUIActionDirectionController()
        {
            if (m_UIActionDirectionController != null && m_UIActionDirectionController.IsEnabled)
                m_UIActionDirectionController.Disable();
        }
    }
}
