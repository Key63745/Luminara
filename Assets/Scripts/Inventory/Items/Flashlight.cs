using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;

public class Flashlight : Item
{
    [SerializeField] GameObject lightSource;

    public override void Equip()
    {
        gameObject.SetActive(!gameObject.activeSelf);
        gameObject.transform.parent.Find("FlashlightRig").GetComponent<Rig>().weight = gameObject.activeSelf ? 1 : 0;
    }
    
    public void Toggle(InputAction.CallbackContext context)
    {
        lightSource.SetActive(!lightSource.activeSelf);
    }
}
