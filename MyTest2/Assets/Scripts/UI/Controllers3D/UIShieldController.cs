using mytest2.Utils.Pool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Werewolf.StatusIndicators.Components;

namespace mytest2.UI.Controllers3D
{
    public class UIShieldController : PoolObject
    {
        public Cone IndicatorController;

        public void Init(Vector3 origin)
        {
            Vector3 lookRotationEuler = Quaternion.LookRotation(origin).eulerAngles;
            transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, lookRotationEuler.y, transform.localEulerAngles.z);
            IndicatorController.Angle = 0;
        }

        public void UpdateUI(float angle)
        {
            Debug.Log(angle);
            IndicatorController.Angle = angle * 2;
        }
    }
}
