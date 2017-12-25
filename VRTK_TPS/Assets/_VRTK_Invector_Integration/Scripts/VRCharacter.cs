using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Invector.CharacterController;

public class VRCharacter : vCharacter
{
    public VRHUDController[] huds;

    private void Start()
    {
        currentHealth = maxHealth;
        onDead.AddListener(SetDeadPostEffect);
        onReceiveDamage.AddListener(SetHitPostEffect);
    }

    public override void ChangeStamina(int value) { }

    public override void ChangeMaxStamina(int value) { }

    public override void TakeDamage(vDamage damage, bool hitReaction = true)
    {
        if (damage != null)
            currentHealth -= damage.damageValue;

        onReceiveDamage.Invoke(damage);

        if (currentHealth <= 0)
            onDead.Invoke(gameObject);
    }

    private void Update()
    {
        foreach( var hud in huds)
            hud.UpdateHUD(this);
    }

    private void SetHitPostEffect(vDamage damage)
    {
        PostProcessingController.Instance.SetEffect(PostProcessingController.EffectList.hit, 0.5f);
    }

    private void SetDeadPostEffect(GameObject go)
    {
        PostProcessingController.Instance.SetEffect(PostProcessingController.EffectList.dead);
    }
}
    
