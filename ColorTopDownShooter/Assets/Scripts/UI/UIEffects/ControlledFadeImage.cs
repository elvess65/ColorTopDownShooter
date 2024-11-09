using System.Collections;
using UnityEngine;

namespace mytest2.UI.Effects
{
    public class ControlledFadeImage : FadeImageController
    {
        [Tooltip("Для спрайтов, которые могут плавно появляться")]
        public bool AutoFadeOut = false;
        public float FadeTime = 3;
        public float InitAlpha = 0;
        [Header("Fade delay")]
        public float FadeOutDelay = 1;
        public float FadeInDelay = 1;

        protected override void Awake()
        {
            if (AutoFadeOut)
            {
                m_FadeTime = FadeTime;
                m_InitAlpha = InitAlpha;
            }

            base.Awake();
        }

        void Start()
        {
            if (AutoFadeOut)
                FadeOut();
        }


        public override void FadeIn()
        {
            StartCoroutine(WaitFadeInDelay());
        }

        public override void FadeOut()
        {
            StartCoroutine(WaitFadeOutDelay());
        }

        IEnumerator WaitFadeInDelay()
        {
            yield return new WaitForSeconds(FadeOutDelay);
            base.FadeIn();
        }

        IEnumerator WaitFadeOutDelay()
        {
            yield return new WaitForSeconds(FadeOutDelay);
            base.FadeOut();
        }
    }
}
