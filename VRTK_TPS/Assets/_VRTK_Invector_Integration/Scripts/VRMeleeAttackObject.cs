using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using Invector.EventSystems;
using Invector;
using VRTK;

public class VRMeleeAttackObject : MonoBehaviour
{
    public vDamage damage;
    public List<VRHitBox> hitBoxes;
    public int damageModifier;
    [HideInInspector] public bool canApplyDamage;
    [HideInInspector] public OnHitEnterVR onDamageHit;
    [HideInInspector] public UnityEvent onEnableDamage, onDisableDamage;
    [HideInInspector] public VRMeleeManager meleeManager;
    
    [Tooltip("Modification to Damage. If power less min value the damage will not be sended. If value more than max will sended max damage. I other case will calculate damage")]
    [MinMaxSlider(0, 500)]
    public Vector2 power;
    public float powerMult = 1;
    private float MaxHapticValue = 500f;

    protected virtual void Start()
    {
        if (hitBoxes.Count > 0)
        {
            /// inicialize the hitBox properties
            foreach (VRHitBox hitBox in hitBoxes)
                hitBox.attackObject = this;
        }
        else
            enabled = false;
    }

    /// <summary>
    /// Set Active all hitBoxes of the MeleeAttackObject
    /// </summary>
    /// <param name="value"> active value</param>  
    public virtual void SetActiveDamage(bool value)
    {
        canApplyDamage = value;
        for (int i = 0; i < hitBoxes.Count; i++)
        {
            var hitCollider = hitBoxes[i];
            hitCollider.trigger.enabled = value;
        }
        if (value)
            onEnableDamage.Invoke();
        else onDisableDamage.Invoke();
    }

    /// <summary>
    /// Call Back of hitboxes
    /// </summary>
    /// <param name="hitBox">vHitBox object</param>
    /// <param name="other">target Collider</param>

    public void OnHit(VRHitBox hitBox, Collider other)
    {
        if (meleeManager == null)
            return;

        VRTK_ControllerReference controllerReference = VRTK_ControllerReference.GetControllerReference(meleeManager.GetControllerGameObject(this));
        var collisionForce = VRTK_DeviceFinder.GetControllerVelocity(controllerReference).magnitude * powerMult;

        if (collisionForce < power.x)
            return;

        float percPowerValue = collisionForce > power.y ? 1 : ((collisionForce - power.x) / (power.y - power.x));

        VRTK_ControllerHaptics.TriggerHapticPulse(controllerReference, MaxHapticValue * percPowerValue, 0.03f, 0.01f);

        //Check  first contition for hit 
        if (canApplyDamage && (meleeManager != null && other.gameObject != meleeManager.gameObject))
        {
            var inDamage = false;
            
            //check if meleeManager exist and apply  his hitProperties  to this
            List<string> _hitDamageTags = meleeManager.hitDamageTags;

            /// Damage Conditions
            if (_hitDamageTags == null || _hitDamageTags.Count == 0)
                inDamage = true;
            else if (_hitDamageTags.Contains(other.tag))
                inDamage = true;
            
            if (inDamage)
            {
                VRHitInfo hitInfo = new VRHitInfo(this, hitBox, other, hitBox.transform.position);

                // Creating new vDamage depending on hit power
                vDamage oldDamage = new vDamage(hitInfo.attackObject.damage);
                hitInfo.attackObject.damage.damageValue = Mathf.RoundToInt(hitInfo.attackObject.damage.damageValue * percPowerValue);

                if (inDamage == true)
                {
                    /// If meleeManager 
                    /// call onDamageHit to control damage values
                    /// and  meleemanager will call the ApplyDamage after to filter the damage
                    /// if meleeManager is null
                    /// The damage will be  directly applied
                    /// Finally the OnDamageHit event is called
	                if (meleeManager)
                        meleeManager.OnDamageHit(hitInfo);
                    else
                    {
                        // Creating new vDamage depending on hit power

                        vDamage newDamage = new vDamage(damage);
                        newDamage.sender = transform;
                        newDamage.damageValue = Mathf.RoundToInt(newDamage.damageValue * percPowerValue);
                        ApplyDamage(hitBox, other, newDamage);
                    }
                    if (onDamageHit != null)
                    onDamageHit.Invoke(hitInfo);
                }
                
                hitInfo.attackObject.damage = oldDamage;
            }
        }
    }


    /// <summary>
    /// Apply damage to target collider (TakeDamage,damage))
    /// </summary>
    /// <param name="hitBox">vHitBox object</param>
    /// <param name="other">collider target</param>
    /// <param name="damage"> damage</param>
    public void ApplyDamage(VRHitBox hitBox, Collider other, vDamage damage)
    {
        vDamage _damage = new vDamage(damage);
        _damage.receiver = other.transform;
        _damage.damageValue = (int)Mathf.RoundToInt(((float)(damage.damageValue + damageModifier) * (((float)hitBox.damagePercentage) * 0.01f)));
        //_damage.sender = transform;
        _damage.hitPosition = hitBox.transform.position;
        other.gameObject.ApplyDamage(_damage, meleeManager.fighter);

    }
}


public class OnHitEnterVR : UnityEvent<VRHitInfo> { }

public class VRHitInfo
{
    public VRMeleeAttackObject attackObject;
    public VRHitBox hitBox;
    public Vector3 hitPoint;
    public Collider targetCollider;
    public VRHitInfo(VRMeleeAttackObject attackObject, VRHitBox hitBox, Collider targetCollider, Vector3 hitPoint)
    {
        this.attackObject = attackObject;
        this.hitBox = hitBox;
        this.targetCollider = targetCollider;
        this.hitPoint = hitPoint;
    }
}