using System.Collections;
using System.Collections.Generic;
using mytest2.Character.Abilities;
using mytest2.Utils.Pool;
using UnityEngine;

namespace mytest2.Character.Shield
{
    /// <summary>
    /// Визуальное отображение щита
    /// </summary>
    public class ShieldVisuals : PoolObject
    {
        public System.Action OnTimeElapsed;

        private bool m_IsActive = false;
        private float m_FinishTime;
		private Coroutine m_AppearAnimationRoutine = null;
		private Coroutine m_DissapearAnimationRoutine = null;
		private List<ShieldVisualsParticle> m_Particles = new List<ShieldVisualsParticle>();

		private const float m_APPEAR_ANIMATION_DELAY = 0.1f;
		private const float m_DISSAPEAR_ANIMATION_DELAY = 0.2f;

        public void Init(AbilityTypes type, Vector3 position, Vector3 origin, float radius, float angle, int existTimeMiliseconds)
        {
            //Внешний вид щита
			transform.position = position;

            //Таймер щита
            m_FinishTime = Time.time + existTimeMiliseconds / 1000f;
			m_IsActive = true;

			CreateParticles(type, position, origin, angle, radius);
			PlayAppearAnimation ();
        }

		void PlayAppearAnimation()
		{
			m_AppearAnimationRoutine = StartCoroutine (PlayParticleAnimationWithDelay (m_APPEAR_ANIMATION_DELAY, true));
		}

		void PlayDissapearAnimation()
		{
			m_DissapearAnimationRoutine = StartCoroutine (PlayParticleAnimationWithDelay (m_DISSAPEAR_ANIMATION_DELAY, false));
		}

		void CreateParticles(AbilityTypes type, Vector3 position, Vector3 origin, float angle, float radius)
		{
			int step = 20;
			float angleBetweenOriginAndForward = Vector3.Angle(Vector3.forward, origin);
			float dot = Vector3.Dot(Vector3.right, origin);
			float sign = Mathf.Sign(dot);

			Debug.Log("Shield angle: " + angle + " OriginAndForward Angle: " + angleBetweenOriginAndForward + " Dot: " + dot);

			int startAngle = (int)((angleBetweenOriginAndForward + angle) * sign);
			int finishAngle = startAngle + (int)(angle * 2 * -sign);

			Debug.Log("Start angle: " + startAngle + " Finish angle: " + finishAngle + " " + Mathf.CeilToInt(angle * 2 / step));


			for (int i = startAngle; sign > 0 ? i > finishAngle : i < finishAngle; i += step * -(int)sign)
			{
				float scale = 1f;
				Vector3 pos = PositionOnCircle(position, radius, i);

				ShieldVisualsParticle ob = PoolManager.GetObject (GameManager.Instance.PrefabLibrary.GetAbilityShieldParticlePrefab(type)) as ShieldVisualsParticle;
				ob.transform.localScale = new Vector3(scale, scale, scale);
				ob.transform.position = pos;
				ob.transform.rotation = Quaternion.LookRotation (pos - position);
				m_Particles.Add(ob);
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

		//TODO: Path animation name as parametr to play appear and dissapear animation
		IEnumerator PlayParticleAnimationWithDelay(float delay, bool isAppearAnimation)
		{
			//Индекс центрального сегмента
			int mIndex = m_Particles.Count / 2; 								
			//Анимация центрального сегмента
			if (isAppearAnimation)
				m_Particles[mIndex].PlayAppearAnimation(); 
			else 
				m_Particles[mIndex].PlayDissapearAnimation(); 

			//Задержка между анимациями
			yield return new WaitForSeconds(delay);

			//Начальный индекс второй стороны
			int j = mIndex + 1;
			//Начальный индекс правой стороны
			for (int i = mIndex - 1; i >= 0; i--)
			{
				//Анимация первой стороны
				if (isAppearAnimation)
					m_Particles[i].PlayAppearAnimation(); 
				else 
					m_Particles[i].PlayDissapearAnimation(); 

				//Анимация второй стороны
				if (j < m_Particles.Count) 
				{
					if (isAppearAnimation)
						m_Particles [j].PlayAppearAnimation ();  
					else 
						m_Particles [j].PlayDissapearAnimation ();  
				}
				//Увеличение индекса второй стороны (паралельный цыкл)
				j++;

				//Задержка между анимациями
				yield return new WaitForSeconds(delay);
			}

			//Окончание анимация
			if (isAppearAnimation) 
			{
				if (m_AppearAnimationRoutine != null)
					m_AppearAnimationRoutine = null;
			}
			else 
			{
			 	if (m_DissapearAnimationRoutine != null)
					m_DissapearAnimationRoutine = null;

				//После окончания анимации выключить щит
				for (int i = 0; i < m_Particles.Count; i++)
					m_Particles [i].Disable ();

				m_Particles.Clear ();

				//Выключить объкт
				Disable();
			}
		}
			
		void Update()
		{
			if (m_IsActive)
			{
				if (Time.time >= m_FinishTime)
				{
					m_IsActive = false;

					if (m_AppearAnimationRoutine != null)
						StopCoroutine (m_AppearAnimationRoutine);

					PlayDissapearAnimation ();

					if (OnTimeElapsed != null)
						OnTimeElapsed();
				}
			}
		}
    }
}
