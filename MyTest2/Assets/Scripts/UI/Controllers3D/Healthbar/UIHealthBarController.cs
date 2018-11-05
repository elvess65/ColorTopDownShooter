using mytest2.Character.Abilities;
using mytest2.Utils.Pool;
using System.Collections.Generic;
using UnityEngine;

namespace mytest2.UI.Controllers3D
{
    public class UIHealthBarController : PoolObject
    {
        public RectTransform SegmentParent;

        private float m_InitSize = 3;
        private float m_StepDelta = 0.5f; 

        private Transform m_Parent;
        private Dictionary<AbilityTypes, UIHealthBarSegment> m_Segments;

        public void Init(Transform parent, Dictionary<AbilityTypes, Character.Health.HealthController.HealthSegment> healthData)
        {
            m_Parent = parent;

            m_Segments = new Dictionary<AbilityTypes, UIHealthBarSegment>();
            int segments = 0;
            foreach(Character.Health.HealthController.HealthSegment segmentData in healthData.Values)
            {
                UIHealthBarSegment segment = PoolManager.GetObject(GameManager.Instance.PrefabLibrary.UIHealthBarSegmentPrefab) as UIHealthBarSegment;
                RectTransform segmentRectTransform = segment.GetComponent<RectTransform>();
                segmentRectTransform.SetParent(SegmentParent);
                segmentRectTransform.anchoredPosition = Vector2.zero;
                segmentRectTransform.localEulerAngles = Vector3.zero;

                float scale = m_InitSize - m_StepDelta * segments;
                segmentRectTransform.localScale = new Vector3(scale, scale, scale);

                segment.Init(segmentData.Type, segmentData.Health);
                m_Segments.Add(segmentData.Type, segment);

                segments++;
            }
        }

        void Update()
        {
            if (m_Parent != null)
            {
                transform.position = m_Parent.position;
            }
        }
    }
}
