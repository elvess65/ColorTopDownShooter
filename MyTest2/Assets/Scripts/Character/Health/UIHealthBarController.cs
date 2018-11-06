using mytest2.Character.Abilities;
using mytest2.Utils.Pool;
using System.Collections.Generic;
using UnityEngine;

namespace mytest2.UI.Controllers3D
{
	/// <summary>
	/// Контроллер объекта, который отображает количество ХП у персонажа
	/// </summary>
	[RequireComponent(typeof(FollowTransformController))]
    public class UIHealthBarController : PoolObject
    {
        public RectTransform SegmentParent;

        private float m_InitSize = 3;
        private float m_StepDelta = 0.5f; 

        private Transform m_Parent;
		private FollowTransformController m_FollowController;
        private Dictionary<AbilityTypes, UIHealthBarSegment> m_Segments;

        public void Init(Transform parent, Dictionary<AbilityTypes, Character.Health.HealthController.HealthSegment> healthData)
        {
			//Follow
			if (m_FollowController == null)
				m_FollowController = GetComponent<FollowTransformController> ();

			m_FollowController.Init (parent);

			//Healthbar
			int segments = 0;
            m_Segments = new Dictionary<AbilityTypes, UIHealthBarSegment>();
            foreach(Character.Health.HealthController.HealthSegment segmentData in healthData.Values)
            {
				//Create segment
                UIHealthBarSegment segment = PoolManager.GetObject(GameManager.Instance.PrefabLibrary.UIHealthBarSegmentPrefab) as UIHealthBarSegment;

				//Position segment
                RectTransform segmentRectTransform = segment.GetComponent<RectTransform>();
                segmentRectTransform.SetParent(SegmentParent);
                segmentRectTransform.anchoredPosition = Vector3.zero;
				segmentRectTransform.localPosition = Vector3.zero;
                segmentRectTransform.localEulerAngles = Vector3.zero;

				//Scale segment
                float scale = m_InitSize - m_StepDelta * segments;
                segmentRectTransform.localScale = new Vector3(scale, scale, scale);

				//Init segment
                segment.Init(segmentData.Type, segmentData.Health);

				//Other
                m_Segments.Add(segmentData.Type, segment);
                segments++;
            }
        }

		public void UpdateUI(AbilityTypes type, int currentHealth)
		{
            if (m_Segments.ContainsKey(type))
                m_Segments[type].UpdateUI(currentHealth);
		}
    }
}
