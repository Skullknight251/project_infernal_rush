using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Weapon;

public class Spell : Item, IHasSkill
{
    public SpellSO spellSO;
    public Skill swapSpellButtonPrefab;
    public List<SkillSO> skillListSO;
    public float spellCastingHoldThreshold = 0f;
    public List<SkillSO> SkillListSO => skillListSO;
    public bool charging;
    public float chargingTime;
    public float maxChargingTime;
    public int chargingPlusDamage;
    public float chargingMaxDamage;
    public float castSpellManaCost;
    private float currentChargingManaCost;
    private SpriteRenderer spriteRenderer;

    protected virtual void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Start()
    {
    }

    public virtual void CastSpell(PlayerMovement playerMovement)
    {
        HideSpellVisual();
        StartCoroutine(CastSpellCoroutine());
    }

    public IEnumerator CastSpellCoroutine()
    {
        yield return new WaitForSeconds(1f);
        ShowSpellVisual();
    }

    public void HideSpellVisual()
    {
        if (spriteRenderer != null) spriteRenderer.enabled = false;
    }

    public void ShowSpellVisual()
    {
        if (spriteRenderer != null) spriteRenderer.enabled = true;
    }

    public void DoDamage(Enemy enemy, int damage)
    {
        enemy.TakeDamage(damage);
    }

    public void TryCastSpellCharging()
    {
        if (GameInput.Instance.IsSpellCasting && Player.Instance.GetMana() >= castSpellManaCost)
        {
            if (!charging)
            {
                charging = true;

                if (AimSkill.Instance != null)
                {
                    AimSkill.Instance.StartAim();
                }
            }

            chargingTime += Time.deltaTime;
            chargingTime = Mathf.Min(chargingTime, maxChargingTime);

            chargingPlusDamage = Mathf.FloorToInt(chargingTime / maxChargingTime * chargingMaxDamage);
        }
        else
        {
            if (charging)
            {
                charging = false;
                chargingTime = 0f;
                chargingPlusDamage = 0;
            }
        }
    }

    public void ConsumeManaSlowly(float manaCost)
    {
        float targetManaCost = chargingTime / maxChargingTime * manaCost;
        float manaCostThisFrame = targetManaCost - currentChargingManaCost;

        if (manaCostThisFrame > 0f)
        {
            Player.Instance.DecreaseMana(manaCostThisFrame);
        }

        currentChargingManaCost = targetManaCost;
    }

    public void ConsumeManaFast(float manaCost)
    {
        Player.Instance.DecreaseMana(manaCost);
    }
}
