using UnityEngine;

namespace mytest2.UI.Controllers3D
{
	/// <summary>
	/// Следование за объектом 
	/// </summary>
    public class FollowTransformController : MonoBehaviour
    {
		public float m_FollowSpeed = 10f;

		private Transform m_Parent;
		private Vector3 m_CreationOffset;

		public void Init(Transform parent)
		{
			//Оффсет на момент создания (создать объект в слоте, но он следует на целью)
			m_CreationOffset = transform.position - parent.position;
			m_Parent = parent;
		}

        public void StopFollowing()
        {
            m_Parent = null;
        }

        void LateUpdate()
        {
			if (m_Parent != null) 
			{
				transform.position = m_Parent.position + m_CreationOffset;
			}
        }
    }
}
