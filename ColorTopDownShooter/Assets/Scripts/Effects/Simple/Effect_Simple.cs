using UnityEngine;

namespace mytest2.Effects
{
    /// <summary>
    /// Включает или выключает объект
    /// </summary>
    public class Effect_Simple : Effect_Base
    {
        [Header("References")]
        public Effect_Base[] ActivateEffects;
        public Effect_Base[] DeactivateEffects;
        public GameObject EffectObj;

        public override void Activate()
        {
            base.Activate();

            EffectObj.SetActive(true);

            //Включить эффекты при активации (Выключаться должны сами)
            for (int i = 0; i < ActivateEffects.Length; i++)
                ActivateEffects[i].Activate();
        }

        public override void Deactivate()
        {
            EffectObj.SetActive(false);

            //Включить эффекты при деактивации (Выключаться должны сами)
            for (int i = 0; i < DeactivateEffects.Length; i++)
                DeactivateEffects[i].Activate();
        }

        protected override void PerformAutodetectEffects()
        {
            EffectObj = gameObject;
        }
    }
}
