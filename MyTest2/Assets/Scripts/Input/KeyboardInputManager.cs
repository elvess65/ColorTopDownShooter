using UnityEngine;

namespace mytest2.UI.InputSystem
{
    public class KeyboardInputManager : BaseInputManager
    {
        public override void UpdateInput()
        {
            Vector2 dir = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

            if (OnMove != null)
                OnMove(new Vector3(dir.x, 0, dir.y));
        }
    }
}
