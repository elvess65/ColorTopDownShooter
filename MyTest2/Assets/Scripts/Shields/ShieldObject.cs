using System.Collections;
using System.Collections.Generic;
using mytest2.Character.Abilities;
using mytest2.Utils.Pool;
using UnityEngine;

namespace mytest2.Character.Shield
{
    /// <summary>
    /// Щит
    /// </summary>
    public class ShieldObject : PoolObject
    {
        public System.Action OnTimeElapsed;

        private bool m_IsActive = false;
        private float m_FinishTime;

        public void Init(AbilityTypes type, Vector3 position, Vector3 origin, float radius, float angle, int existTimeMiliseconds)
        {
            //Внешний вид щита
            transform.localScale = new Vector3(radius * 2, radius * 2, radius * 2);

            //Таймер щита
            m_FinishTime = Time.time + existTimeMiliseconds / 1000f;
            //m_IsActive = true;

            StartCoroutine(Create(position, origin, angle, radius));
        }

		IEnumerator Create(Vector3 position, Vector3 origin, float angle, float radius)
		{
			int step = 15;
			float angleBetweenOriginAndForward = Vector3.Angle(Vector3.forward, origin);
			float dot = Vector3.Dot(Vector3.right, origin);
			float sign = Mathf.Sign(dot);

			Debug.Log("Shield angle: " + angle + " OriginAndForward Angle: " + angleBetweenOriginAndForward + " Dot: " + dot);

			int startAngle = (int)((angleBetweenOriginAndForward + angle) * sign);
			int finishAngle = startAngle + (int)(angle * 2 * -sign);

			Debug.Log("Start angle: " + startAngle + " Finish angle: " + finishAngle + " " + Mathf.CeilToInt(angle * 2 / step));

			List<GameObject> obj = new List<GameObject>();
			for (int i = startAngle; sign > 0 ? i > finishAngle : i < finishAngle; i += step * -(int)sign)
			{
				float scale = 0.5f;
				Vector3 pos = PositionOnCircle(position, radius, i);

				GameObject ob = GameObject.CreatePrimitive(PrimitiveType.Sphere);
				ob.transform.localScale = new Vector3(scale, scale, scale);
				ob.transform.position = pos;
				obj.Add(ob);
			}

			int mIndex = obj.Count / 2;
			
			obj[mIndex].transform.localScale *= 0.5f;
            yield return new WaitForSeconds(0.1f);
			int j = mIndex + 1;
			for (int i = mIndex - 1; i >= 0; i--)
			{
				obj[i].transform.localScale *= 0.3f;
				if (j < obj.Count)
					obj[j].transform.localScale *= 0.3f;
				j++;
				yield return new WaitForSeconds(0.1f);
			}
		}

		Vector3 PositionOnCircle(Vector3 center, float radius, int angle)
		{
			float ang = angle;
			Vector3 pos;
			pos.x = center.x + radius * Mathf.Sin(ang * Mathf.Deg2Rad);
			pos.y = center.y;
			pos.z = center.z + radius * Mathf.Cos(ang * Mathf.Deg2Rad);
			return pos;
		}

        void Update()
        {
            if (m_IsActive)
            {
                if (Time.time >= m_FinishTime)
                {
                    m_IsActive = false;

                    Disable();

                    if (OnTimeElapsed != null)
                        OnTimeElapsed();
                }
            }
        }
    }
}
