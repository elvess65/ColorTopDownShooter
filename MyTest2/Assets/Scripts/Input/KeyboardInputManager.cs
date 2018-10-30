using UnityEngine;

namespace mytest2.UI.InputSystem
{
    public class KeyboardInputManager : BaseInputManager
    {
        public System.Action<Vector2> OnDodgeStart;
        public System.Action<Vector2> OnDodgeDrag;
        public System.Action<Vector2> OnDodge;

        public override void UpdateInput()
        {
            Vector2 dir = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            if (OnMove != null)
                OnMove(new Vector3(dir.x, 0, dir.y));

            if (Input.GetKeyDown(KeyCode.Space))
            {
                Vector2 dodgeDir = Input.mousePosition - new Vector3(Screen.width / 2, Screen.height / 2, Input.mousePosition.z);
                if (OnDodgeStart != null)
                    OnDodgeStart(dodgeDir.normalized);
            }
            else if (Input.GetKey(KeyCode.Space))
            {
                Vector2 dodgeDir = Input.mousePosition - new Vector3(Screen.width / 2, Screen.height / 2, Input.mousePosition.z);
                if (OnDodgeDrag != null)
                    OnDodgeDrag(dodgeDir.normalized);
            }
            else if (Input.GetKeyUp(KeyCode.Space))
            {
                Vector2 dodgeDir = Input.mousePosition - new Vector3(Screen.width / 2, Screen.height / 2, Input.mousePosition.z);
                if (OnDodge != null)
                    OnDodge(dodgeDir.normalized);
            }
        }
    }
}
