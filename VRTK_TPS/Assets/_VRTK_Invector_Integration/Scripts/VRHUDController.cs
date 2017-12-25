using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VRHUDController : MonoBehaviour {

    public Slider healthSlider;




    public virtual void UpdateHUD(VRCharacter vrc)
    {
        UpdateSliders(vrc);
    }

    void UpdateSliders(VRCharacter vrc)
    {
        if (vrc.maxHealth != healthSlider.maxValue)
        {
            healthSlider.maxValue = Mathf.Lerp(healthSlider.maxValue, vrc.maxHealth, 2f * Time.fixedDeltaTime);
            healthSlider.onValueChanged.Invoke(healthSlider.value);
        }
        healthSlider.value = Mathf.Lerp(healthSlider.value, vrc.currentHealth, 2f * Time.fixedDeltaTime);
    }

}
