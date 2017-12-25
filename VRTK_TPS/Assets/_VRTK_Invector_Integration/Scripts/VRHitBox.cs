using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// The copy of vHitBox script. Look at comments to see the changes
/// </summary>
public class VRHitBox : MonoBehaviour {

    /// <summary>
    /// The OnTriggerEnter method have a bug. it may call at the next frame after collision.
    /// The RaycastEnter more complex in calculations.
    /// </summary>
    public enum TriggerMode { Collider, Raycast }

    [HideInInspector] public VRMeleeAttackObject attackObject;
    public Collider trigger;
    public int damagePercentage = 100;
    [Tooltip("The Trigger method have a bug. It may call at the next frame after collision. The Raycast more complex in calculations.")]
    public TriggerMode enterMode = TriggerMode.Raycast;

    private List<Collider> raycastHitedColliders = new List<Collider>();

    void OnDrawGizmos()
    {
        trigger = gameObject.GetComponent<Collider>();

        if (!trigger) trigger = gameObject.AddComponent<BoxCollider>();
        //Changed gizmos color
        Color color = Color.cyan;
        color.a = 0.6f;
        Gizmos.color = color;
        if (!Application.isPlaying && trigger && !trigger.enabled) trigger.enabled = true;
        if (trigger && trigger.enabled)
        {
            if (trigger as BoxCollider)
            {
                BoxCollider box = trigger as BoxCollider;

                var sizeX = transform.lossyScale.x * box.size.x;
                var sizeY = transform.lossyScale.y * box.size.y;
                var sizeZ = transform.lossyScale.z * box.size.z;
                Matrix4x4 rotationMatrix = Matrix4x4.TRS(box.bounds.center, transform.rotation, new Vector3(sizeX, sizeY, sizeZ));
                Gizmos.matrix = rotationMatrix;
                Gizmos.DrawCube(Vector3.zero, Vector3.one);
            }
        }
    }

    void Start()
    {
        trigger = GetComponent<Collider>();
        if (!trigger) trigger = gameObject.AddComponent<BoxCollider>();
        if (trigger)
        {
            trigger.isTrigger = true;
            trigger.enabled = false;
        }
        
    }

    private void Update()
    {
        if (enterMode == TriggerMode.Raycast)
        {
            BoxCollider box = trigger as BoxCollider;

            var sizeX = transform.lossyScale.x * box.size.x / 2;
            var sizeY = transform.lossyScale.y * box.size.y / 2;
            var sizeZ = transform.lossyScale.z * box.size.z / 2;

            Collider[] hitedColiders = Physics.BoxCastAll(transform.position, new Vector3(sizeX, sizeY, sizeZ), Vector3.forward, transform.rotation, 0).Select((hit) => hit.collider).ToArray();

            List<Collider> tempCollection = new List<Collider>();

            //foreach (var collider in raycastHitedColliders)
            for (int i = raycastHitedColliders.Count -1; i >=0; i--)
            {
                Collider collider = raycastHitedColliders[i];
                if (hitedColiders.Contains(collider))
                {
                    OnStay(collider);
                }else
                {
                    OnExit(collider);
                    raycastHitedColliders.Remove(collider);
                }
            }


            foreach (var collider in hitedColiders)
            {
                if (!raycastHitedColliders.Contains(collider))
                {
                    raycastHitedColliders.Add(collider);
                    OnEnter(collider);
                }
            }

        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (enterMode != TriggerMode.Collider)
            return;

        OnEnter(other);
    }

    void OnTriggerStay(Collider other)
    {
        if (enterMode != TriggerMode.Collider)
            return;

        OnStay(other);
    }

    void OnTriggerExit(Collider other)
    {
        if (enterMode != TriggerMode.Collider)
            return;

        OnExit(other);
    }


    private void OnEnter(Collider other)
    {
       // Debug.Log("Enter" + other.gameObject.name);

        if (TriggerCondictions(other))
        {
            if (attackObject != null)
            {
                attackObject.OnHit(this, other);
            }
        }
    }

    private void OnExit(Collider other)
    {
      //  Debug.Log("Exit" + other.gameObject.name);
    }

    private void OnStay(Collider other)
    {
      //  Debug.Log("Stay" + other.gameObject.name);
    }




    bool TriggerCondictions(Collider other)
    {
        return ((attackObject != null && (attackObject.meleeManager == null || other.gameObject != attackObject.meleeManager.gameObject)));
    }
}
