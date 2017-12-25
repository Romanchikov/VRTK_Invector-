using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using Invector;
using Invector.EventSystems;

/// <summary>
/// This class is copy of vMeleeManager class with some modification 
/// I remove:
/// - Recoil 
/// - Changing body mambers
/// - Stamina 
/// 
/// In my case I don't need all this things
/// </summary>
public class VRMeleeManager : vMonoBehaviour{

     public static VRMeleeManager Inctance{ get; private set; }
    
    public vDamage defaultDamage = new vDamage(10);
    public VRMeleeAttackObject leftHand, rightHand;
    public List<string> hitDamageTags = new List<string>() { "Enemy" };
    public VRMeleeWeapon leftWeapon, rightWeapon;
    public VROnHitEvent onDamageHit;

    [Range(0, 100)]
    public int defaultDefenseRate = 50;
    [Range(0, 180)]
    public float defaultDefenseRange = 90;

    [HideInInspector]
    public vIMeleeFighter fighter;
    private int damageMultiplier;
    private bool ignoreDefense;
    private string attackName;
    

    protected virtual void Awake()
    {
        if (Inctance != null)
            DestroyImmediate(this);

        Inctance = this;
    }
    
    // Make Attack always on
    protected virtual void Start()
    {
        leftHand.meleeManager = this;
        rightHand.meleeManager = this;

        this.DoWithDelay(() =>
        {
            CheeckLeftWeapon();
            CheeckRightWeapon();
        }, 0.1f);
    }

    /// <summary>
    /// Return controller gameobject where was placed attackObject
    /// </summary>
    /// <returns></returns>
    public virtual GameObject GetControllerGameObject(VRMeleeAttackObject attackObject)
    {
        if ((rightWeapon != null && attackObject == rightWeapon) || attackObject == rightHand)
            return rightHand.gameObject;
        
        if ((leftWeapon != null && attackObject == leftWeapon) || attackObject == leftHand)
            return leftHand.gameObject;

        return null;
    }

    /// <summary>
    /// Listener of Damage Event
    /// </summary>
    /// <param name="hitInfo"></param>
    public virtual void OnDamageHit(VRHitInfo hitInfo)
    {
        vDamage damage = new vDamage(hitInfo.attackObject.damage);
        damage.sender = transform;
        if (attackName != string.Empty) damage.attackName = this.attackName;
        if (ignoreDefense) damage.ignoreDefense = ignoreDefense;
        /// Calc damage with multiplier 
        /// and Call ApplyDamage of attackObject 
        damage.damageValue *= damageMultiplier > 1 ? damageMultiplier : 1;
        hitInfo.attackObject.ApplyDamage(hitInfo.hitBox, hitInfo.targetCollider, damage);
        onDamageHit.Invoke(hitInfo);
    }

    public virtual void SetLeftWeapon(GameObject weaponObject)
    {
        if (weaponObject)
        {
            leftWeapon = weaponObject.GetComponent<VRMeleeWeapon>();
            if (leftWeapon)
                leftWeapon.meleeManager = this;
            
        }else if(leftWeapon != null)
            leftWeapon = null;

        CheeckLeftWeapon();
    }

    public virtual void SetRightWeapon(GameObject weaponObject)
    {
        if (weaponObject)
        {
            rightWeapon = weaponObject.GetComponent<VRMeleeWeapon>();
            if (rightWeapon)
                rightWeapon.meleeManager = this;
        } else if (rightWeapon != null)
            rightWeapon = null;

        CheeckRightWeapon();
    }


    protected virtual void CheeckLeftWeapon() { CheeckWeapon(leftHand, leftWeapon); }

    protected virtual void CheeckRightWeapon() { CheeckWeapon(rightHand, rightWeapon); }

    void CheeckWeapon(VRMeleeAttackObject hand, VRMeleeAttackObject weapon)
    {
        // if weapon is active 
        if (weapon != null && weapon.canApplyDamage)
            return;

        // if weapon not active but not null
        if (weapon != null)
        {
            weapon.SetActiveDamage(true);
            hand.SetActiveDamage(false);
        }
        else if (!hand.canApplyDamage)
            hand.SetActiveDamage(true);
    }
}

[Serializable]
public class VROnHitEvent : UnityEngine.Events.UnityEvent<VRHitInfo> { }

