using UnityEngine;

namespace mytest2.Effects
{
    /// <summary>
    /// Базовый класс для всех эффектов
    /// </summary>
    public abstract class Effect_Base : MonoBehaviour
    {
        [Tooltip("Автоопределение ссылок на эффекты")]
        public bool AutoDetect = false;

        [Header("AutoDesctruct")]
        [Tooltip("Автоматическое выключение эффекта после активации")]
        public bool AutoDestructAfterActivation = false;
        [Tooltip("Задержка перед автоматическим выключением")]
        public float AutoDestructSec = 5;

        protected virtual void Start()
        {
            if (AutoDetect)
                PerformAutodetectEffects();
        }

        public virtual void Activate()
        {
            if (AutoDestructAfterActivation)
                LaunchAutoDestruct();
        }

        public abstract void Deactivate();


        public void ForceAutoDestruct()
        {
            ForceAutoDestruct(AutoDestructSec);
        }

        public void ForceAutoDestruct(float time)
        {
            if (AutoDestructAfterActivation)
                return;

            AutoDestructSec = time;
            LaunchAutoDestruct();
        }

        protected void LaunchAutoDestruct()
        {
            Destroy(gameObject, AutoDestructSec);
        }


        protected virtual void PerformAutodetectEffects()
        {
        }
    }
}
