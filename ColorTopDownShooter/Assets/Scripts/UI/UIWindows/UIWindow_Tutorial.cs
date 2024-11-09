using mytest2.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace mytest2.UI.Windows
{
    /// <summary>
    /// Реализация окна обучения
    /// </summary>
    public class UIWindow_Tutorial : UIWindow_TimerBeforeClose
    {
        [Space(10)]
        public Image Icon;

        public void InitWithType(UIWindowsTutorialLibrary.TutorialWindowTypes type)
        {
            Text_Main.text = LocalizationManager.GetText(type.ToString());
            Icon.sprite = UIWindowsTutorialLibrary.Instance.GetWindowSpriteByType(type);
        }
    }
}
