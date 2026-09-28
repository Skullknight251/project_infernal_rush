
using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    public static ButtonManager Instance { get; private set; }
    [SerializeField] private Transform weaponSwapSpot;
    [SerializeField] private Transform spellSwapSpot;
    [SerializeField] private Transform basicAttackSpot;
    [SerializeField] private Transform plungeAttackSpot;
    [SerializeField] private Transform spellCastSpot;
    [SerializeField] private Transform blockSpot;

    public Weapon currentWeapon;
    public Spell currentSpell;

    public void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        if (PlayerMovement.Instance != null)
        {
            SetCurrentWeapon(PlayerMovement.Instance.currentWeapon);
            SetCurrentSpell(PlayerMovement.Instance.currentSpell);
        }
    }

    public Skill GetBasicSkill()
    {
        return basicAttackSpot.GetComponentInChildren<Skill>();
    }
    public Skill GetPlungeSkill()
    {
        return plungeAttackSpot.GetComponentInChildren<Skill>();
    }

    public Skill GetSpellCastSkill() { 
        return spellCastSpot.GetComponentInChildren<Skill>();
    }
    void Update()
    {

    }
    private void ClearSlot(Transform slotTransform)
    {
        if (slotTransform == null) return;

        foreach (Transform child in slotTransform)
        {
            Destroy(child.gameObject);
        }
    }

    public void HandleSwapWeapon()
    {
        
        if (PlayerMovement.Instance.weapon2 != null)
        {
            ClearSlot(weaponSwapSpot);
            Instantiate(PlayerMovement.Instance.weapon2.swapWeaponButtonPrefab, weaponSwapSpot);
        }
    }
    public void HandleSwapSpell()
    {
       
        if (PlayerMovement.Instance.spell2 != null)
        {
            ClearSlot(spellSwapSpot);
            Instantiate(PlayerMovement.Instance.spell2.swapSpellButtonPrefab, spellSwapSpot);
        }
    }
    public void ButtonInit()
    {
        ClearSlot(basicAttackSpot);
        ClearSlot(plungeAttackSpot);
        ClearSlot(spellCastSpot);
        ClearSlot(blockSpot);
        if (currentWeapon != null) {
            foreach (SkillSO skillButton in currentWeapon.SkillListSO)
            {
                //Debug.Log(skillButton.name + " " + skillButton.buttonType);
                switch (skillButton.buttonType)
                {

                    case SkillSO.ButtonType.BasicAttack:
                        //Debug.Log("Basic Attack Set");
                        Instantiate(skillButton.prefab.gameObject, basicAttackSpot);
                        break;
                    case SkillSO.ButtonType.PlungeAttack:
                        //Debug.Log("Plunge Attack Set");
                        Instantiate(skillButton.prefab.gameObject, plungeAttackSpot);
                        break;
                    case SkillSO.ButtonType.Block:
                        //Debug.Log("Block Set");
                        Instantiate(skillButton.prefab.gameObject, blockSpot);
                        break;
                }
            }
        }
        if (currentSpell != null)
        {
            foreach (SkillSO skillButton in currentSpell.SkillListSO)
            {
                //Debug.Log(skillButton.name + " " + skillButton.buttonType);
                switch (skillButton.buttonType)
                {
                    case SkillSO.ButtonType.SpellSkill:
                        //Debug.Log("Spell Casting Set");
                        Instantiate(skillButton.prefab.gameObject, spellCastSpot);
                        break;
                }
            }
        }
    }

    public void SetCurrentWeapon(Weapon weapon)
    {
        if (currentWeapon == weapon) return;
        currentWeapon = weapon;
        ButtonInit();
        HandleSwapWeapon();
    }

    public void SetCurrentSpell(Spell spell)
    {
        if (currentSpell == spell) return;
        currentSpell = spell;
        ButtonInit();
        HandleSwapSpell();
    }
}
