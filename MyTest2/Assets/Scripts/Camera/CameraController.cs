using mytest2.Effects.PostProcessing;
using mytest2.UI.InputSystem;
using System.Collections;
using UnityEngine;

namespace mytest2.CameraSystem
{
    [RequireComponent(typeof(CameraAligningBehaviour))]
    [RequireComponent(typeof(CameraRotationBehaviour))]
    [RequireComponent(typeof(CameraFollowingBehaviour))]
    [RequireComponent(typeof(CameraFocusingBehaviour))]
    public class CameraController : MonoBehaviour
    {
        public System.Action OnCameraArrived;
        public System.Action OnRotationFinished;

        public Vector3 InitCameraOffset = new Vector3(0, 12, -10);

        private CameraAligningBehaviour m_AligningBehaviour;
        private CameraRotationBehaviour m_RotationBehaviour;
        private CameraFollowingBehaviour m_FollowingBehaviour;
        private CameraFocusingBehaviour m_FocusingBehaviour;

        private Transform m_Target;
        private Vector3 m_CachedOffset;

        //Focusing some time 
        private float m_FocusingTime;
        private System.Action m_OnFocusingFinished;
        private System.Action m_OnFocusDelayFinished;
        private System.Action m_OnFocusDelayStarted;
        //Rotation
        private float m_Angle;
        private float m_Speed;
        private bool m_ClockWise;
        private bool m_UnlockInputOnRotationFinished;

        void Awake()
        {
            m_AligningBehaviour = GetComponent<CameraAligningBehaviour>();
            m_RotationBehaviour = GetComponent<CameraRotationBehaviour>();
            m_FollowingBehaviour = GetComponent<CameraFollowingBehaviour>();
            m_FocusingBehaviour = GetComponent<CameraFocusingBehaviour>();
        }

        public void Init(Transform target)
        {
            m_Target = target;

            AlignToInitOffset();
        }

        /// <summary>
        /// Выровнять камеру согласно начальному отступу
        /// </summary>
        void AlignToInitOffset()
        {
            m_AligningBehaviour.OnFinished += FollowTarget;
            m_AligningBehaviour.OnFinished += OnCameraArrived;
            m_AligningBehaviour.AlignToOffset(m_Target, InitCameraOffset);
        }

        /// <summary>
        /// Начать следовать за целью, закешировав отступ
        /// </summary>
        void FollowTarget()
        {
            m_FollowingBehaviour.Follow(m_Target, CacheOffset());
        }


        /// <summary>
        /// Переместить камеру на некоторое время на объект, а затем вернуть на предыдущую цель
        /// </summary>
        /// <param name="target">Объект для фокусировки</param>
        /// <param name="focusingTime">Время фокусировки на объекте</param>
        /// <param name="onFocusingFinished">Окончания фокусировки (камера вернулась на предыдущую цель)</param>
        /// <param name="onFocusDelayFinished">Окончание задержки фокусировки (камера начинает возвращаться на предыдущую цель)</param>
        /// <param name="onFocusDelayStarted">Начало задержки фокусировки (камера окончила движение к цели)</param>
        public void FocusSomeTimeAt(Transform target, float focusingTime, System.Action onFocusingFinished, System.Action onFocusDelayFinished, System.Action onFocusDelayStarted)
        {
            m_FollowingBehaviour.PauseFollowing();

            m_FocusingTime = focusingTime;
            m_OnFocusingFinished = onFocusingFinished;
            m_OnFocusDelayFinished = onFocusDelayFinished;
            m_OnFocusDelayStarted = onFocusDelayStarted;

            m_FocusingBehaviour.OnFinished += FocusedOnTargetHandler;
            m_FocusingBehaviour.MoveToWithOffset(target, CacheOffset());   //Кеш оффсета на момент начала движения (если было вращение камеры)
        }

        void FocusedOnTargetHandler()
        {
            StartCoroutine(WaitFocusDelay());
        }

        IEnumerator WaitFocusDelay()
        {
            if (m_OnFocusDelayStarted != null)
                m_OnFocusDelayStarted();

            yield return new WaitForSeconds(m_FocusingTime);

            if (m_OnFocusDelayFinished != null)
                m_OnFocusDelayFinished();

            m_FocusingBehaviour.OnFinished += FocusingFinishedHandler;
            m_FocusingBehaviour.MoveToWithOffset(m_Target, m_CachedOffset);
        }

        void FocusingFinishedHandler()
        {
            if (m_OnFocusingFinished != null)
                m_OnFocusingFinished();

            m_FollowingBehaviour.ContinueFollowing();
        }


        public void RotateAroundTarget(float angle, float speed, bool clockwise, bool unlockInputOnRotationFinished)
        {
            m_Angle = angle;
            m_Speed = speed;
            m_ClockWise = clockwise;
            m_UnlockInputOnRotationFinished = unlockInputOnRotationFinished;

            Debug.LogWarning("GameManager.Instance.GameState.Player.PauseAnimations(true)");
            InputManager.Instance.InputIsEnabled = false;
            PostProcessingController.Instance.DecreaseSaturation();

            //Если нужно вращать камеру, а она все еще следует за персонажем подписаться на событие окончания движения
            if (m_FollowingBehaviour.IsMoving)
                m_FollowingBehaviour.OnFinished += RotateCamera;
            else
                RotateCamera();
        }

        void RotateCamera()
        {
            m_FollowingBehaviour.PauseFollowing();
            m_RotationBehaviour.OnFinished += RotationFinishedHandler;
            m_RotationBehaviour.OnFinished += OnRotationFinished;
            m_RotationBehaviour.RotateAroundBy(m_Target, m_Angle, m_Speed, m_ClockWise);
        }

        void RotationFinishedHandler()
        {
            Debug.LogWarning("GameManager.Instance.GameState.Player.PauseAnimations(false);");
            PostProcessingController.Instance.NormalizeSaturation();

            if (m_UnlockInputOnRotationFinished)
                InputManager.Instance.InputIsEnabled = true;

            m_FollowingBehaviour.Follow(m_Target, CacheOffset());
        }



        void LateUpdate()
        {
            if (!GameManager.Instance.IsActive && m_Target == null)
                return;

            m_AligningBehaviour.UpdateBehaviour();
            m_RotationBehaviour.UpdateBehaviour();
            m_FollowingBehaviour.UpdateBehaviour();
            m_FocusingBehaviour.UpdateBehaviour();


        }

        Vector3 CacheOffset()
        {
            m_CachedOffset = transform.position - m_Target.transform.position;
            return m_CachedOffset;
        }
    }
}
