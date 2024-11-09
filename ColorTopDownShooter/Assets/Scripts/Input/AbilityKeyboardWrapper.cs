using mytest2.Character.Abilities;
using UnityEngine;
using UnityEngine.UI;

namespace mytest2.UI.InputSystem
{
    public class AbilityKeyboardWrapper : MonoBehaviour
    {
        public AbilityTypes AbilityType;
        public Text Text_AbilityAmmo;

        public void UpdateAbilityAmmo(int ammoAmount)
        {
            Text_AbilityAmmo.text = ammoAmount.ToString();
        }

        public void Select()
        {

        }
    }
}
