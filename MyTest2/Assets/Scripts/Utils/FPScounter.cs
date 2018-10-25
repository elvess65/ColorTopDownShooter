using UnityEngine;

namespace mytest2.Utils
{
    /// <summary>
    /// Вспомогательный класс, для отображения кол-ва кадров в секунду.
    /// </summary>
    public class FPScounter : MonoBehaviour 
	{
		private Rect fpsLblRect;
		private bool showFPS = true;
		private string strFPS;
		private float fpsAccum   = 0; 		// FPS accumulated over the interval
		private int   fpaFrames  = 0; 		// Frames drawn over the interval
		private float fpsTimeleft = 0.5f; 	// Left time for current interval
	
		// Use this for initialization
		void Start () 
		{
            fpsLblRect = new Rect(Screen.width - 80, Screen.height / 2, 80.0f, 20.0f);	
			#if (!UNITY_EDITOR) 
			if (!Debug.isDebugBuild)
				showFPS = false;
			#endif
		}
		// Update is called once per frame
		void Update () 
		{
			// FPS counter
			if(showFPS)
			{
	 			fpsTimeleft -= Time.deltaTime;
	    		fpsAccum += Time.timeScale/Time.deltaTime;
	    		++fpaFrames;
			    if( fpsTimeleft <= 0.0f )
				{
					strFPS = System.String.Format("{0:F1} FPS", fpsAccum / fpaFrames);
					//hud.SetFPS(strFPS);
	    	       	fpsTimeleft = 0.5f;
	        		fpsAccum = 0.0f;
	        		fpaFrames = 0;
				}
			}
		}
		void OnGUI()
		{
			if(showFPS)
				GUI.Label(fpsLblRect, strFPS);
		}
		public void ShowFPS(bool state)
		{
			showFPS = state;
		}
	}
}
