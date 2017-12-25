using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK.Examples;
using VRTK;


public class InvectorVRWeapon : Sword {

    

    public override void Grabbed(VRTK_InteractGrab grabbingObject)
    {
        base.Grabbed(grabbingObject);
     
        if (VRTK_DeviceFinder.GetControllerLeftHand() == grabbingObject.gameObject)
            VRMeleeManager.Inctance.SetLeftWeapon(gameObject);
        
        if (VRTK_DeviceFinder.GetControllerRightHand() == grabbingObject.gameObject)
            VRMeleeManager.Inctance.SetRightWeapon(gameObject);
            
    }

    public override void Ungrabbed(VRTK_InteractGrab previousGrabbingObject)
    {
        base.Ungrabbed(previousGrabbingObject);
        
        if (VRTK_DeviceFinder.GetControllerLeftHand() == previousGrabbingObject.gameObject)
            VRMeleeManager.Inctance.SetLeftWeapon(null);

        if (VRTK_DeviceFinder.GetControllerRightHand() == previousGrabbingObject.gameObject)
            VRMeleeManager.Inctance.SetRightWeapon(null);
    }
}
