using UnityEngine;

namespace mytest2.Character
{
    public class EnemyController : CreatureController
    {
        public float DetectDisatnce = 10;
        public float AttackDistance = 5;

        private float m_LastAttackTime = float.NegativeInfinity;
        private float m_TimeBetweenAttack = 2;

        public override void Move(Vector2 dir)
        {
        }


        protected override void Update()
        {
            base.Update();

            Vector3 vecToPlayer = GameManager.Instance.GameState.Player.transform.position - transform.position;
            float distToPlayer = vecToPlayer.magnitude;

            if (distToPlayer <= DetectDisatnce && distToPlayer > AttackDistance)
            {
                float angleToPlayer = Mathf.Atan2(vecToPlayer.x, vecToPlayer.z) * Mathf.Rad2Deg;
                m_MoveController.Rotate(angleToPlayer);
            }
            else if (distToPlayer <= AttackDistance)
            {
                Vector3 vecToPlayerNormalized = vecToPlayer.normalized;
                Vector2 vecToPlayer2D = new Vector2(vecToPlayerNormalized.x, vecToPlayerNormalized.z);

                if (Time.time - m_LastAttackTime >= m_TimeBetweenAttack)
                {
                    TryUseAbility(m_AbilityController.Abilities[0], vecToPlayer2D);
                    m_LastAttackTime = Time.time;
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, DetectDisatnce);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, AttackDistance);

            Gizmos.DrawLine(transform.position, GameManager.Instance.GameState.Player.transform.position);
        }
    }
}
