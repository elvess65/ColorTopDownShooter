using mytest2.UI.Effects;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace mytest2.UI.Loading
{
    /// <summary>
    /// Загрузка сцены (затемнение экрана при загрузке)
    /// </summary>
    public class LevelLoader : MonoBehaviour
    {
        public static LevelLoader Instance;

        public Slider ProgressBar;
        public FadeImageController FadeImage;

        private AsyncOperation m_AsyncOperation;

        void Awake()
        {
            Instance = this;
        }

        void Start()
        {
            if (ProgressBar != null)
                ProgressBar.gameObject.SetActive(false);

            FadeImage.FadeIn();
        }

        public void LoadLevel(int sceneIndex)
        {
            StartCoroutine(LoadLevelAsync(sceneIndex));
        }

        public void LoadNextLevel()
        {
            int nextLevelIndex = SceneManager.GetActiveScene().buildIndex + 1;
            if (nextLevelIndex > SceneManager.sceneCountInBuildSettings - 1)
                nextLevelIndex = 0;

            LoadLevel(nextLevelIndex);
        }

        public void RestartRound()
        {
            LoadLevel(SceneManager.GetActiveScene().buildIndex);
        }

        IEnumerator LoadLevelAsync(int index)
        {
            if (ProgressBar != null)
                ProgressBar.gameObject.SetActive(true);

            m_AsyncOperation = SceneManager.LoadSceneAsync(index);
            m_AsyncOperation.allowSceneActivation = false;
            while (m_AsyncOperation.progress < 0.9f)
            {
                if (ProgressBar != null)
                {
                    float progress = Mathf.Clamp01(m_AsyncOperation.progress / 0.9f);
                    ProgressBar.value = progress;
                }

                yield return null;
            }

            if (ProgressBar != null)
                ProgressBar.value = 1;

            FadeImage.OnFadeAnimationComplete += LoadingStageFinished;
            FadeImage.FadeOut();
        }

        void LoadingStageFinished()
        {
            m_AsyncOperation.allowSceneActivation = true;
        }
    }
}
