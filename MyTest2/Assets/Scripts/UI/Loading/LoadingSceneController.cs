using UnityEngine;

namespace mytest2.UI.Loading
{
    /// <summary>
    /// Обработка ввода для загрузки сцены (переход из начальной сцены)
    /// </summary>
    public class LoadingSceneController : MonoBehaviour
    {
        private ScreenShotController m_ScreenShotController;
        private LoadingEffectsController m_LoadingEffectsController;
        private bool m_IsLoading = false;

        void Start()
        {
            m_ScreenShotController = GetComponent<ScreenShotController>();
            m_LoadingEffectsController = GetComponent<LoadingEffectsController>();
            m_LoadingEffectsController.OnAnimationComplete += AnimationCompleteHandler;
        }

        void AnimationCompleteHandler()
        {
            LevelLoader.Instance.LoadNextLevel();
        }

        void Update()
        {
            if (Input.GetMouseButtonUp(0) && !m_IsLoading)
            {
                m_IsLoading = true;
                m_LoadingEffectsController.ShowBackgroundEffect(m_ScreenShotController.GetRenderTexture(),
                                                                m_ScreenShotController.GetUIRenderTexture());
            }
        }
    }
}
