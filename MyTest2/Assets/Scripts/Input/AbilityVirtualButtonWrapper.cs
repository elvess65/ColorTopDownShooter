using mytest2.Character.Abilities;
using UnityEngine.EventSystems;

namespace mytest2.UI.InputSystem
{
    /// <summary>
    /// Надстройка над кнопкой способностей
    /// </summary>
    public class AbilityVirtualButtonWrapper : VirtualButtonWrapper
    {
        public System.Action<AbilityTypes> OnAbilitySelect;

        public AbilityTypes AbilityType;

        public void UpdateAbilityAmmo(int ammoAmount)
        {
            //Text_AbilityAmmo.text = ammoAmount.ToString();
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);

            if (OnAbilitySelect != null)
                OnAbilitySelect(AbilityType);
        }
    }
}
