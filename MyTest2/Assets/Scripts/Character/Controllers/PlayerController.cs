using mytest2.Character.Abilities;
using mytest2.UI.Controllers3D;
using mytest2.UI.InputSystem;
using mytest2.Utils.Pool;
using UnityEngine;

namespace mytest2.Character
{
    public class PlayerController : CreatureController
    {
        private AbilityFocusAssistant m_AbilityFocusAssistant;
        private UIPlayerActionDirectionController m_UIActionDirectionController;
		private Vector3 m_TargetMoveDir = Vector3.zero;
        private Vector2 m_LastMoveDir2D;
        private float m_TargetRotAngle;
        //Shield
        private float m_ShieldAngle;                //Для создания щита
        private Vector3 m_ShieldOrigin;             //Для создания щита
        private Vector3 m_InputBound;               //Для просчета угла
        private Vector3 m_AutoBound;                //Для отрисовки Gizmo
        private Vector3 m_PerpendicularToOrigin;    //Для отрисовки Gizmo 
        private Vector2 m_PerpendicularToOrigin2D;  //Для отрисовки Gizmo (скорее всего)

        protected override void Update()
        {
            base.Update();

			//Передвижение и вращение если персонаж не уклоняеться или не вращаеться к направлению способности (применение)
			if (m_State == States.Normal)
            {
                //Debug.Log(m_LastMoveDir2D);
                m_MoveController.Move(m_TargetMoveDir);
                m_MoveController.Rotate(m_TargetRotAngle);
            }
        }

        /// <summary>
        /// Сохранить данные для передвижения (вызов реализации происходит в Update и за нее отвечает компонент iMovement)
        /// </summary>
        /// <param name="dir"></param>
        public override void Move(Vector2 dir)
        {
            //Кэш направления передвижения
            m_TargetMoveDir = new Vector3(dir.x, 0, dir.y);

            //Если игрок хочет переместиться
            if (m_TargetMoveDir != Vector3.zero)
            {
                m_LastMoveDir2D = dir;

                //Вращение в направлении движения
                m_TargetRotAngle = Mathf.Atan2(m_TargetMoveDir.x, m_TargetMoveDir.z) * Mathf.Rad2Deg;
            }
        }

        #region Dodge Handlers
        /// <summary>
        /// Нажатие на джойстик уклона
        /// </summary>
        void DodgeInputTouchStart(Vector2 dir)
        {
            //ShowUIActionDirectionController(AbilityTypes.None);
        }

        /// <summary>
        /// Перемещение джойстика уклона 
        /// </summary>
        void DodgeInputDrag(Vector2 dir)
        {
            //UpdateUIActionDirectionController(dir);
        }

        /// <summary>
        /// Окончание нажатия на джойстик уклона
        /// </summary>
        /// <param name="dir"></param>
        void DodgeInputTouchEnd(Vector2 dir)
        {
            if (m_State != States.Normal)
            {
                //TODO: Возможно следует прерывать вращение для применения способности если игрок хочет сделать уклон
                Debug.Log("Cant dodge: Cur state is " + m_State);
                return;
            }

            //Спрятать указатель направления
            //HideUIActionDirectionController();

            //Два варианта
            //1 Уклон в сторону последнего передвижения

            Dodge(m_LastMoveDir2D);
            //2 Уклон в выбранную сторону
            //Dodge(dir);
        }
        #endregion
        #region Ability Handlers
        /// <summary>
        /// Активация способности
        /// </summary>
        /// <param name="type">Тип способности</param>
        void AbilityInputActivate(AbilityTypes type)
        {
            m_AbilityFocusAssistant.InitForAbility(type);
            ShowUIActionDirectionController(type);
        }

        /// <summary>
        /// Выбор способности
        /// </summary>
        /// <param name="abilityType">Тип способности</param>
        void AbilityInputSelect(AbilityTypes abilityType)
        {
            SelectAbility(abilityType);
        }

        /// <summary>
        /// Перемещение джойстика способности 
        /// </summary>
        void AbilityInputDrag(Vector2 dir)
        {
            dir = m_AbilityFocusAssistant.GetFocusedDir(m_SelectedAbility, dir);
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
            {
                dir = m_AbilityFocusAssistant.GetFocusedDir(m_SelectedAbility, dir);

                TryUseAbility(SelectedAbility, dir);
            }

            //Спрятать указатель направления
            HideUIActionDirectionController();
        }

        #region Attack
        void AttackInputTouchStart(Vector2 screenPos)
        {
            //ShowUIActionDirectionController(m_SelectedAbility);
            m_AbilityFocusAssistant.InitForAbility(m_SelectedAbility);
        }

        void AttackInputDrag(Vector2 screenPos)
        {
            //UpdateUIActionDirectionController(dir.normalized);
        }

        void AttackInputTouchEnd(Vector2 screenPos)
        {
            //Направление способности по-умолчанию - вперед 
            Vector2 abilityDir = new Vector2(transform.forward.x, transform.forward.z);
            //Коректировка направления способности
            abilityDir = m_AbilityFocusAssistant.GetFocusedDir(m_SelectedAbility, abilityDir);

            TryUseAbility(SelectedAbility, abilityDir);

            //HideUIActionDirectionController();
        }
        #endregion

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
        /// Обновить количество зарядов способности
        /// </summary>
        /// <param name="type">Тип способности</param>
        /// <param name="ammoAmmount">Количество зарядов</param>
        void AbilityUpdateAmmoHandler(AbilityTypes type, int ammoAmmount)
        {
            GameManager.Instance.UIManager.UpdateAbilityAmmo(type, ammoAmmount);
        }

        /// <summary>
        /// Выделить способность
        /// </summary>
        /// <param name="abilityType"></param>
        public override void SelectAbility(AbilityTypes abilityType)
        {
            if (m_SelectedAbility != abilityType)
                GameManager.Instance.UIManager.SelectAbilityVisuals(abilityType);

            base.SelectAbility(abilityType);
        }
        #endregion
        #region Shield Handlers
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

            m_ShieldController.ShowShieldUI(m_ShieldOrigin);
        }

        void OnShieldInputUpdate(Vector2 dirToTarget)
        {
            //Текущая граница контролируемая вводом
            m_InputBound = new Vector3(dirToTarget.x, 0, dirToTarget.y);
            //Угол между началом щита и текущей границей
            m_ShieldAngle = Vector3.Angle(m_ShieldOrigin, m_InputBound);

            //Сравнение Dot с пермендикуляром к началу щита для определения знака угла 
            float dot = Vector2.Dot(m_PerpendicularToOrigin2D, dirToTarget);
            float dir = Mathf.Sign(dot);

            m_ShieldController.UpdateShieldUI(m_ShieldAngle);

            //Автоматическое смещение границы
            m_AutoBound = Quaternion.Euler(0, -m_ShieldAngle * dir, 0) * m_ShieldOrigin;
        }

        void OnShieldInputEnd()
        {
            m_ShieldController.HideShieldUI();
            CreateShield(m_ShieldOrigin, m_ShieldAngle, m_SelectedAbility);
        }

        private void OnDrawGizmos()
        {
            if (m_ShieldController != null)
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
        }
        #endregion
        #region Other Handlers
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
        /// <param name="inputIsEnabled"></param>
        void InputStatusChangeHandler(bool inputIsEnabled)
        {
            SelectAbility(AbilityTypes.Blue);
        }
        #endregion

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
                InputManager.Instance.KeyboardInput.OnDodge += DodgeInputTouchEnd;

                InputManager.Instance.KeyboardInput.OnAbilityActivate += AbilityInputActivate;
                InputManager.Instance.KeyboardInput.OnAbilitySelect += AbilityInputSelect;
                InputManager.Instance.KeyboardInput.OnAbilityEnd += AbilityInputTouchEnd;
                InputManager.Instance.KeyboardInput.OnAbilityMove += AbilityInputDrag;
            }
#else
            SubscribeForJoystickEvents();
#endif

            InputManager.Instance.OnInputStateChange += InputStatusChangeHandler;
            InputManager.Instance.OnShieldInputStart += OnShieldInputStart;
            InputManager.Instance.OnShieldInputUpdate += OnShieldInputUpdate;
            InputManager.Instance.OnShieldInputEnd += OnShieldInputEnd;
        }
        protected override void SubscribeForControllerEvents()
        {
			base.SubscribeForControllerEvents ();

            m_StaminaController.OnStaminaUpdate += StaminaUpdateHandler;
            m_AbilityController.OnAbilityUse += AbilityUseHandler;
            m_AbilityController.OnUpdateAmmo += AbilityUpdateAmmoHandler;
        }
        protected override void FinishInitialization()
        {
			base.FinishInitialization ();

            //Подписаться на событие обновления зарядов способностей и вызвать это событие
            m_AbilityController.SetAndCallUpdateAmmoEventForAllAbilities();
            //По умолчанию последнее направление движения - назад относительно персонажа
            m_LastMoveDir2D = new Vector2(transform.forward.x, -transform.forward.z);
            //Создать предметы для подъема
            GameManager.Instance.GameState.ItemSpawnController.SpawnItems();
            //Помощь в наведении способностей
            m_AbilityFocusAssistant = GetComponent<AbilityFocusAssistant>();
        }
        private void SubscribeForJoystickEvents()
        {
            //Передвижение
            InputManager.Instance.VirtualJoystickInput.OnMove += Move;

            //Уклон
            /*InputManager.Instance.VirtualJoystickInput.DodgeJoystickWrapper.OnJoystickTouchStart += DodgeInputTouchStart;
            InputManager.Instance.VirtualJoystickInput.DodgeJoystickWrapper.OnJoystickMove += DodgeInputDrag;
            InputManager.Instance.VirtualJoystickInput.DodgeJoystickWrapper.OnJoystickTouchEnd += DodgeInputTouchEnd;*/

            InputManager.Instance.VirtualJoystickInput.AttackButtonWrapper.OnButtonTouchStart += AttackInputTouchStart;
            InputManager.Instance.VirtualJoystickInput.AttackButtonWrapper.OnButtonMove += AttackInputDrag;
            InputManager.Instance.VirtualJoystickInput.AttackButtonWrapper.OnButtonTouchEnd += AttackInputTouchEnd;

            InputManager.Instance.VirtualJoystickInput.DodgeButtonWrapper.OnButtonTouchStart += DodgeInputTouchEnd;

            //Способности
            /*for (int i = 0; i < InputManager.Instance.VirtualJoystickInput.AbilityJoystickWrappers.Length; i++)
            {
                InputManager.Instance.VirtualJoystickInput.AbilityJoystickWrappers[i].OnAbilityActivate += AbilityInputActivate;
                InputManager.Instance.VirtualJoystickInput.AbilityJoystickWrappers[i].OnAbilitySelect += AbilityInputSelect;
                InputManager.Instance.VirtualJoystickInput.AbilityJoystickWrappers[i].OnJoystickTouchEnd += AbilityInputTouchEnd;
                InputManager.Instance.VirtualJoystickInput.AbilityJoystickWrappers[i].OnJoystickMove += AbilityInputDrag;
            }*/

            for (int i = 0; i < InputManager.Instance.VirtualJoystickInput.AbilityButtonWrappers.Length; i++)
            {
                InputManager.Instance.VirtualJoystickInput.AbilityButtonWrappers[i].OnAbilitySelect += AbilityInputSelect;
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
            {
                m_UIActionDirectionController.SetDirection(dir);
            }
        }
        void HideUIActionDirectionController()
        {
            if (m_UIActionDirectionController != null && m_UIActionDirectionController.IsEnabled)
                m_UIActionDirectionController.Disable();
        }
    }
}
