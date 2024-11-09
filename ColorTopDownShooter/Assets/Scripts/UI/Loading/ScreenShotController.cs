using UnityEngine;

namespace mytest2.UI.Loading
{
    /// <summary>
    /// Скриншот того, что видит камера
    /// </summary>
    public class ScreenShotController : MonoBehaviour
    {
        public UnityEngine.Camera RenderCamera;
        public UnityEngine.Camera UIRenderCamera;

        public RenderTexture GetRenderTexture()
        {
            RenderTexture rt = new RenderTexture(Screen.width, Screen.height, 16, RenderTextureFormat.ARGB32);
            rt.Create();

            RenderCamera.targetTexture = rt;
            RenderCamera.Render();
            RenderCamera.targetTexture = null;

            return rt;
        }

        public RenderTexture GetUIRenderTexture()
        {
            RenderTexture rt = new RenderTexture(Screen.width, Screen.height, 16, RenderTextureFormat.ARGB32);
            rt.Create();

            UIRenderCamera.targetTexture = rt;
            UIRenderCamera.Render();
            UIRenderCamera.targetTexture = null;

            return rt;
        }
    }
}
