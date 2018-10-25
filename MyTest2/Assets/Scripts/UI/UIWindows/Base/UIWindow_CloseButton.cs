using UnityEngine;
using UnityEngine.UI;

namespace mytest2.UI.Windows
{
    /// <summary>
    /// Окно с кнопкой Закрыть
    /// </summary>
    public abstract class UIWindow_CloseButton : UIWindow_Base
    {
        public Button Button_Close;

        private bool m_InputIsLocked = false;

        public override void Hide()
        {
            LockInput(true);

            base.Hide();
        }

        public override void HideByEscape()
        {
            if (m_InputIsLocked)
                return;

            base.HideByEscape();
        }


        protected override void Init()
        {
            if (Button_Close != null)
                Button_Close.onClick.AddListener(ButtonClose_PressHandler);

            LockInput(true);
        }

        protected void LockInput(bool inputIsLocked)
        {
            m_InputIsLocked = inputIsLocked;

            Button_Close.enabled = !inputIsLocked;

            Color curColor = Button_Close.image.color;
            float alpha = inputIsLocked ? 0.5f : 1f;
            Button_Close.image.color = new Color(curColor.r, curColor.g, curColor.b, alpha);
        }

        protected override void ShowAnimation_Finished()
        {
            base.ShowAnimation_Finished();

            LockInput(false);
        }


        void ButtonClose_PressHandler()
        {
            if (m_InputIsLocked)
                return;

            Hide();
        }
    }
}