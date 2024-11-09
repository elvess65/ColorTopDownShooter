using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using mytest2.Utils.Pool;

namespace mytest2.Character.Shield
{
	/// <summary>
	/// Визуальное отображение одной частицы щита
	/// </summary>
	public class ShieldVisualsParticle : PoolObject 
	{
		public void PlayAppearAnimation()
		{
			transform.position += new Vector3 (0, 0.5f, 0);
		}

		public void PlayDissapearAnimation()
		{
			transform.position -= new Vector3 (0, 0.5f, 0);
		}
	}
}
