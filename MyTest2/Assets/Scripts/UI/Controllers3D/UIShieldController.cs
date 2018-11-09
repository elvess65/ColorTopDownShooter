using mytest2.Utils.Pool;
using UnityEngine;
using Werewolf.StatusIndicators.Components;

namespace mytest2.UI.Controllers3D
{
    public class UIShieldController : PoolObject
    {
        public Cone IndicatorController;

        public void Init(Transform followTransform, Vector3 origin)
        {
            if (!gameObject.activeSelf)
                gameObject.SetActive(true);

            transform.position = followTransform.position;

            Vector3 lookRotationEuler = Quaternion.LookRotation(origin).eulerAngles;
            transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, lookRotationEuler.y, transform.localEulerAngles.z);
            IndicatorController.Angle = 0;
        }

        public void UpdateUI(float angle)
        {
            IndicatorController.Angle = angle * 2;
        }

        public void HideUI()
        {
            gameObject.SetActive(false);
        }
    }
}
