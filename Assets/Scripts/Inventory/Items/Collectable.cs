using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;

public class Collectable : Item
{
    public override void Equip()
    {
        var success = HandleEquip();
        if (success)
        {
            gameObject.GetComponentInParent<PlayerStateMachine>().heldItem = gameObject.GetComponent<Item>();
            gameObject.SetActive(!gameObject.activeSelf);
            gameObject.transform.parent.Find("FlashlightRig").GetComponent<Rig>().weight = gameObject.activeSelf ? 1 : 0;
        }
    }

    public void Drop()
    {
        gameObject.transform.SetParent(null);
    }
}
