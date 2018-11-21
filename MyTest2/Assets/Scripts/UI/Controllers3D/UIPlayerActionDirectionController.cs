using mytest2.Character.Abilities;
using mytest2.Utils.Pool;
using UnityEngine;

namespace mytest2.UI.Controllers3D
{
    /// <summary>
    /// Контроллер объекта, который задает отображает стрелку направления способности
    /// </summary>
	[RequireComponent(typeof(FollowTransformController))]
    public class UIPlayerActionDirectionController : PoolObject
    {
        public MeshRenderer Renderer;
        public float RotationSpeed = 10;

        private bool m_IsActive = false;
        private Quaternion m_TargetRot;
        private Material m_Material;
		private FollowTransformController m_FollowController;

	    

        public void Init(Transform parent, AbilityTypes type)
        {
			//Follow
			if (m_FollowController == null)
				m_FollowController = GetComponent<FollowTransformController> ();

			m_FollowController.Init (parent);

			//Rotation
			m_TargetRot = parent.rotation;

			//Color
            if (m_Material == null)
            {
                m_Material = new Material(Renderer.sharedMaterial);
                Renderer.sharedMaterial = m_Material;
            }

            //Length
            Vector3 scale = transform.localScale;
            scale.z = GameManager.Instance.GameState.DataTableAbilities.GetAbilityData(type).Length;
            transform.localScale = scale;

            m_Material.color = GameManager.Instance.GameState.AbilityColorController.GetAbilityColor(type);

			m_IsActive = true;
        }

        public void SetDirection(Vector2 dir)
        {
            float targetRotAngle = Mathf.Atan2(dir.x, dir.y) * Mathf.Rad2Deg;
            m_TargetRot = Quaternion.AngleAxis(targetRotAngle, Vector3.up);
        }

		protected override void DisableObject ()
		{
			base.DisableObject ();

			m_IsActive = false;
		}
			
        private void Update()
        {
			if (m_IsActive)
				transform.rotation = Quaternion.Slerp(transform.rotation, m_TargetRot, Time.deltaTime * RotationSpeed);
        }
    }
}
