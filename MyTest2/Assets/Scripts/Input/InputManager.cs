using UnityEngine;

namespace mytest2.UI.InputSystem
{
    [RequireComponent(typeof(KeyboardInputManager))]
    [RequireComponent(typeof(VirtualJoystickInputManager))]
    public class InputManager : MonoBehaviour
    {
        public static InputManager Instance;

        public System.Action<bool> OnInputStateChange;
        public System.Action<Vector2> OnShieldInputStart;
        public System.Action<Vector2> OnShieldInputUpdate;
        public System.Action OnShieldInputEnd;

        public bool PreferVirtualJoystickInEditor = false;

        private BaseInputManager m_Input;
        private bool m_InputState = false;

        public VirtualJoystickInputManager VirtualJoystickInput
        {
            get; private set;
        }
        public KeyboardInputManager KeyboardInput
        {
            get; private set;
        }   
        public bool InputIsEnabled
        {
            get { return m_InputState; }
            set
            {
                if (m_InputState != value)
                {
                    m_InputState = value;

                    if (OnInputStateChange != null)
                        OnInputStateChange(m_InputState);
                }
            }
        }

        void Awake()
        {
            Instance = this;

            VirtualJoystickInput = GetComponent<VirtualJoystickInputManager>();
            KeyboardInput = GetComponent<KeyboardInputManager>();
        }

        void Start()
        {
#if UNITY_EDITOR
            if (PreferVirtualJoystickInEditor)
                m_Input = VirtualJoystickInput;
            else
                m_Input = KeyboardInput;
#else
            m_Input = VirtualJoystickInput;
#endif
        }

        void Update()
        {
            if (GameManager.Instance.IsActive && m_InputState)
            {
                m_Input.UpdateInput();

                HandleShieldInput();
            }

            if (Input.GetKeyDown(KeyCode.L))
                InputIsEnabled = false;

            if (Input.GetKeyDown(KeyCode.U))
                InputIsEnabled = true;
        }


        void HandleShieldInput()
        {
            if (Input.GetMouseButtonDown(1) && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            {
                Vector2 dirFromCenterToMouse = GetDirFromScreenCenterToMouse();
                if (OnShieldInputStart != null)
                    OnShieldInputStart(dirFromCenterToMouse.normalized);
            }

            if(Input.GetMouseButton(1))
            {;
                Vector2 dirFromCenterToMouse = GetDirFromScreenCenterToMouse();
                if (OnShieldInputUpdate != null)
                    OnShieldInputUpdate(dirFromCenterToMouse.normalized);
            }

            if (Input.GetMouseButtonUp(1))
            {
                if (OnShieldInputEnd != null)
                    OnShieldInputEnd();
            }
        }

        Vector2 GetDirFromScreenCenterToMouse()
        {
            Vector2 mousePos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            Vector2 screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);
            Vector2 dirFromCenterToMouse = mousePos - screenCenter;

            return dirFromCenterToMouse;
        }
    }

    public abstract class BaseInputManager : MonoBehaviour
    {
        public System.Action<Vector2> OnMove;

        protected virtual void Start()
        { }

        public abstract void UpdateInput();
    }
}
