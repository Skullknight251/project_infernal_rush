using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class MobileButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{

    [SerializeField] private SkillSO.ButtonType buttonType;
    public void OnPointerDown(PointerEventData eventData)
    {
        switch (buttonType)
        {
            case SkillSO.ButtonType.Jump:
                GameInput.Instance.Jump();
                break;
            case SkillSO.ButtonType.BasicAttack:
                GameInput.Instance.BasicAttackStarted();
                break;
            case SkillSO.ButtonType.PlungeAttack:
                GameInput.Instance.OverheadAttackStarted();
                break;
            case SkillSO.ButtonType.SpellSkill:
                GameInput.Instance.CastSpellStarted();
                break;
            case SkillSO.ButtonType.Block:
                GameInput.Instance.Block();
                break;
            case SkillSO.ButtonType.SwapWeapon:
                GameInput.Instance.SwapWeapon(); 
                break;
            case SkillSO.ButtonType.SwapSpell:
                GameInput.Instance.SwapSpell();
                break;
        }
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        switch (buttonType)
        {
            case SkillSO.ButtonType.BasicAttack:
                GameInput.Instance.BasicAttackCanceled();
                break;

            case SkillSO.ButtonType.PlungeAttack:
                GameInput.Instance.OverheadAttackCanceled();
                break;

            case SkillSO.ButtonType.SpellSkill:
                GameInput.Instance.CastSpellCanceled();
                break;
        }
    }
}