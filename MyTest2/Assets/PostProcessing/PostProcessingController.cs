using mytest2.UI.InputSystem;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;
using Wilberforce.FinalVignette;

namespace mytest2.Effects.PostProcessing
{
    public class PostProcessingController : MonoBehaviour
    {
        public static PostProcessingController Instance;

        public PostProcessingBehaviour Behaviour;
        public Camera ColorGradingCamera;
        public FinalVignetteCommandBuffer Vignette;

        private enum PostEffectsTypes
        {
            Saturation,
            PostExposure,
            Vignette
        }

        private Dictionary<PostEffectsTypes, AnimationBehaviour> m_AnimationBehaviours;
        private List<AnimationBehaviour> m_ActiveAnimationBehaviours;
        private PostProcessingProfile m_Profile;
        private Utils.InterpolationData<float> m_SaturationLerpData;

        //Saturation
        private const float m_SATURATION_DECREASED = 0.5f;
        private const float m_SATURATION_NORMALIZED = 1f;
        private const float m_SATURATION_FADE_TIME = 1f;
        //PostExposure
        private const float m_POSTEXPOSURE_DECREASED = -3f;
        private const float m_POSTEXPOSURE_NORMALIZED = 0f;
        private const float m_POSTEXPOSURE_FADE_TIME = 1f;
        //Vignette 
        private float m_VignetteInnerValue;
        private float m_VignetteOuterValue;
        private const float m_VIGNETTE_FADE_TIME = 1;

        private void Awake()
        {
            Instance = this;
        }

        void Start()
        {
            m_AnimationBehaviours = new Dictionary<PostEffectsTypes, AnimationBehaviour>();
            m_ActiveAnimationBehaviours = new List<AnimationBehaviour>();

            //Создать новый профиль и начать его использовать
            m_Profile = ScriptableObject.CreateInstance<PostProcessingProfile>();
            Behaviour.profile = m_Profile;

            ///Color grading
            m_Profile.colorGrading = new ColorGradingModel();

            //Изменить тип Tonemapper на Tonemapper.None
            ColorGradingModel.Settings colorGradingModelSettings = m_Profile.colorGrading.settings;
            colorGradingModelSettings.tonemapping.tonemapper = ColorGradingModel.Tonemapper.None;
            m_Profile.colorGrading.settings = colorGradingModelSettings;

            //Saturation
            AnimationBehaviour behaviour = new SaturationAnimationBehaviour(m_Profile, m_SATURATION_FADE_TIME);
            behaviour.OnAnimationFinished += AnimationFinishedHandler;
            m_AnimationBehaviours.Add(PostEffectsTypes.Saturation, behaviour);

            //Expose
            behaviour = new ExposeAnimationBehaviour(m_Profile, m_POSTEXPOSURE_FADE_TIME);
            behaviour.OnAnimationFinished += AnimationFinishedHandler;
            m_AnimationBehaviours.Add(PostEffectsTypes.PostExposure, behaviour);

            ///Vignette
            behaviour = new VignetteAnimationBehaviour(null, m_VIGNETTE_FADE_TIME, Vignette, Vignette.VignetteInnerValue, Vignette.VignetteOuterValue);
            behaviour.OnAnimationFinished += AnimationFinishedHandler;
            m_AnimationBehaviours.Add(PostEffectsTypes.Vignette, behaviour);

            InputManager.Instance.OnInputStateChange += VignetteOnInputState;
        }

        void Update()
        {
            if (m_ActiveAnimationBehaviours.Count > 0)
            {
                for (int i = 0; i < m_ActiveAnimationBehaviours.Count; i++)
                    m_ActiveAnimationBehaviours[i].Update();
            }
        }


        public void DecreaseSaturation()
        {
            Enable();

            //Включить обработку эффектов
            if (!Behaviour.enabled)
                Behaviour.enabled = true;

            //Включить Color grading
            if (!m_Profile.colorGrading.enabled)
                m_Profile.colorGrading.enabled = true;

            //Включить камеру
            if (!ColorGradingCamera.enabled)
                ColorGradingCamera.enabled = true;

            StratAnimation(m_SATURATION_DECREASED, PostEffectsTypes.Saturation);
        }

        public void NormalizeSaturation()
        {
            Enable();

            StratAnimation(m_SATURATION_NORMALIZED, PostEffectsTypes.Saturation, ColorGradingNormalizeAnimationFinished);
        }


        public void DecreasePostExposure()
        {
            Enable();

            //Включить обработку эффектов
            if (!Behaviour.enabled)
                Behaviour.enabled = true;

            //Включить Color grading
            if (!m_Profile.colorGrading.enabled)
                m_Profile.colorGrading.enabled = true;

            //Включить камеру
            if (!ColorGradingCamera.enabled)
                ColorGradingCamera.enabled = true;

            StratAnimation(m_POSTEXPOSURE_DECREASED, PostEffectsTypes.PostExposure);
        }

        public void NormalizePostExposure()
        {
            Enable();

            StratAnimation(m_POSTEXPOSURE_NORMALIZED, PostEffectsTypes.PostExposure, ColorGradingNormalizeAnimationFinished);
        }

        void ColorGradingNormalizeAnimationFinished(AnimationBehaviour behaviour)
        {
            behaviour.OnAnimationFinished -= ColorGradingNormalizeAnimationFinished;

            //Выключить Color grading
            m_Profile.colorGrading.enabled = false;

            //Выключить камеру
            if (ColorGradingCamera.enabled)
                ColorGradingCamera.enabled = false;

            //Выключить обработку эффектов
            if (Behaviour.enabled)
                Behaviour.enabled = false;
        }


        public void ShowVignette()
        {
            Enable();

            if (!Vignette.enabled)
                Vignette.enabled = true;

            StratAnimation(1, PostEffectsTypes.Vignette);
        }

        public void HideVignette()
        {
            Enable();

            StratAnimation(-1, PostEffectsTypes.Vignette, VignetteHideAnimationFinished);
        }

        void VignetteHideAnimationFinished(AnimationBehaviour behaviour)
        {
            behaviour.OnAnimationFinished -= VignetteHideAnimationFinished;
            Vignette.enabled = false;
        }

        void VignetteOnInputState(bool inputEnableState)
        {
            if (inputEnableState)
                HideVignette();
            else
                ShowVignette();
        }


        void StratAnimation(float targetValue, PostEffectsTypes type, Action<AnimationBehaviour> animationFinishedHandler = null)
        {
            AnimationBehaviour behaviour = m_AnimationBehaviours[type];

            //Событие окончания анимации
            if (animationFinishedHandler != null)
                behaviour.OnAnimationFinished += animationFinishedHandler;

            //Добавить эффект в список активных
            m_ActiveAnimationBehaviours.Add(behaviour);

            //Начать анимацию
            behaviour.StartAnimation(targetValue);
        }

        void AnimationFinishedHandler(AnimationBehaviour behaviour)
        {
            m_ActiveAnimationBehaviours.Remove(behaviour);
            Disable();
        }


        void Enable()
        {
            if (!enabled)
                enabled = true;
        }

        void Disable()
        {
            //if (m_ActiveAnimationBehaviours.Count == 0)
            //    enabled = false;
        }


        abstract class AnimationBehaviour
        {
            public Action<AnimationBehaviour> OnAnimationFinished;

            protected PostProcessingProfile m_Profile;
            protected Utils.InterpolationData<float> m_LerpData;

            public AnimationBehaviour(PostProcessingProfile profile, float totalTime)
            {
                //Профиль
                m_Profile = profile;

                //Базовые данные об анимации
                m_LerpData = new Utils.InterpolationData<float>();
                m_LerpData.TotalTime = totalTime;
            }

            public virtual void StartAnimation(float targetValue)
            {
                m_LerpData.Start();
            }

            public void Update()
            {
                if (m_LerpData.IsStarted)
                {
                    m_LerpData.Increment();
                    Animate();

                    if (m_LerpData.Overtime())
                    {
                        m_LerpData.Stop();

                        if (OnAnimationFinished != null)
                            OnAnimationFinished(this);
                    }
                }
            }


            protected float LerpResult()
            {
                return Mathf.Lerp(m_LerpData.From, m_LerpData.To, m_LerpData.Progress);
            }

            protected abstract void Animate();
        }

        class SaturationAnimationBehaviour : AnimationBehaviour
        {
            public SaturationAnimationBehaviour(PostProcessingProfile profile, float totalTime) : base(profile, totalTime)
            {
            }

            public override void StartAnimation(float targetValue)
            {
                m_LerpData.From = m_Profile.colorGrading.settings.basic.saturation;
                m_LerpData.To = targetValue;

                base.StartAnimation(targetValue);
            }

            protected override void Animate()
            {
                ColorGradingModel.Settings colorGradingModelSettings = m_Profile.colorGrading.settings;
                colorGradingModelSettings.basic.saturation = LerpResult();
                m_Profile.colorGrading.settings = colorGradingModelSettings;
            }
        }

        class ExposeAnimationBehaviour : AnimationBehaviour
        {
            public ExposeAnimationBehaviour(PostProcessingProfile profile, float totalTime) : base(profile, totalTime)
            {
            }

            public override void StartAnimation(float targetValue)
            {
                m_LerpData.From = m_Profile.colorGrading.settings.basic.postExposure;
                m_LerpData.To = targetValue;

                base.StartAnimation(targetValue);
            }

            protected override void Animate()
            {
                ColorGradingModel.Settings colorGradingModelSettings = m_Profile.colorGrading.settings;
                colorGradingModelSettings.basic.postExposure = LerpResult();
                m_Profile.colorGrading.settings = colorGradingModelSettings;
            }
        }

        class VignetteAnimationBehaviour : AnimationBehaviour
        {
            private FinalVignetteCommandBuffer m_Vignette;
            private float m_VignetteInnerValue;
            private float m_VignetteOuterValue;
            protected Utils.InterpolationData<float> m_OuterValueLerpData;

            public VignetteAnimationBehaviour(PostProcessingProfile profile, float totalTime,
                                              FinalVignetteCommandBuffer vignette, float innerValue, float outerValue) : base(profile, totalTime)
            {
                m_Vignette = vignette;
                m_VignetteInnerValue = innerValue;
                m_VignetteOuterValue = outerValue;

                m_OuterValueLerpData = new Utils.InterpolationData<float>();
                m_OuterValueLerpData.TotalTime = totalTime;
            }

            public override void StartAnimation(float targetValue)
            {
                //targetValue < 0 - decrease, > 0 - normalize
                m_LerpData.From = m_Vignette.VignetteInnerValue;
                m_LerpData.To = targetValue < 0 ? 1 : m_VignetteInnerValue;

                m_OuterValueLerpData.From = m_Vignette.VignetteOuterValue;
                m_OuterValueLerpData.To = targetValue < 0 ? 1 : m_VignetteOuterValue;

                base.StartAnimation(targetValue);
            }

            protected override void Animate()
            {
                m_Vignette.VignetteInnerValue = LerpResult();


                m_Vignette.VignetteOuterValue = Mathf.Lerp(m_OuterValueLerpData.From, m_OuterValueLerpData.To, m_LerpData.Progress);
            }
        }
    }
}
