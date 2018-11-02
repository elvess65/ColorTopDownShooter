using mytest2.Character.Abilities;
using mytest2.Utils.Pool;
using UnityEngine;

namespace mytest2.UI.Controllers3D
{
    /// <summary>
    /// Контроллер объекта, который задает отображает стрелку направления способности
    /// </summary>
    public class UIPlayerActionDirectionController : PoolObject
    {
        public MeshRenderer Renderer;

        private Quaternion m_TargetRot;
        private Transform m_Parent;

        private Material m_Material;
        private const float m_SPEED = 10f;

        public void Init(Transform parent, AbilityTypes type)
        {
            m_Parent = parent;
            m_TargetRot = m_Parent.rotation;

            if (m_Material == null)
            {
                m_Material = new Material(Renderer.sharedMaterial);
                Renderer.sharedMaterial = m_Material;
            }

            switch(type)
            {
                case AbilityTypes.Blue:
                    m_Material.color = Color.blue;
                    break;
                case AbilityTypes.Green:
                    m_Material.color = Color.green;
                    break;
                case AbilityTypes.Red:
                    m_Material.color = Color.red;
                    break;
                case AbilityTypes.Violet:
                    m_Material.color = Color.magenta;
                    break;
                case AbilityTypes.Yellow:
                    m_Material.color = Color.yellow;
                    break;
                default:
                    m_Material.color = Color.white;
                    break;
            }
        }

        public void SetDirection(Vector2 dir)
        {
            float targetRotAngle = Mathf.Atan2(dir.x, dir.y) * Mathf.Rad2Deg;
            m_TargetRot = Quaternion.AngleAxis(targetRotAngle, Vector3.up);
        }


        void Update()
        {
            if (m_Parent != null)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, m_TargetRot, Time.deltaTime * m_SPEED);
                transform.position = m_Parent.position;
            }
        }
    }
}
