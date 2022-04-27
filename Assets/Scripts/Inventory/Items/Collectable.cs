using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;

public class Collectable : Item
{

    public override void Equip()
    {
        
    }

    public void Drop()
    {
        gameObject.transform.SetParent(null);
    }
}
