using UnityEngine;

namespace mytest2.UI.InputSystem
{
    [RequireComponent(typeof(KeyboardInputManager))]
    [RequireComponent(typeof(VirtualJoystickInputManager))]
    public class InputManager : MonoBehaviour
    {
        public static InputManager Instance;

        public System.Action<bool> OnInputStateChange;

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
                m_Input.UpdateInput();

            if (Input.GetKeyDown(KeyCode.L))
                InputIsEnabled = false;

            if (Input.GetKeyDown(KeyCode.U))
                InputIsEnabled = true;
        }
    }

    public abstract class BaseInputManager : MonoBehaviour
    {
        public System.Action<Vector2> OnMove;

        protected virtual void Start()
        { }

        public abstract void UpdateInput();
		public virtual int GetShieldInputButton()
		{
			return 1;
		}
    }
}
