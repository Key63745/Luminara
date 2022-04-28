using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;

public class Flashlight : Item
{
    [SerializeField] GameObject lightSource;
    [SerializeField] private int _power = 100;

    bool _draining = false;
    bool _blinking = false;

    public override void Equip()
    {
        HandleEquip();
        gameObject.GetComponentInParent<PlayerStateMachine>().heldItem = gameObject.GetComponent<Item>();
        gameObject.SetActive(!gameObject.activeSelf);
        gameObject.transform.parent.Find("FlashlightRig").GetComponent<Rig>().weight = gameObject.activeSelf ? 1 : 0;
        gameObject.GetComponent<PlayerInput>().enabled = gameObject.activeSelf;
    }
    
    public void Toggle(InputAction.CallbackContext context)
    {
        lightSource.SetActive(!lightSource.activeSelf);
    }

    IEnumerator DrainPower(float wait)
    {
        _draining = true;
        while (_power > 0)
        {
            yield return new WaitForSeconds(wait);
            _power -= 2;
        }
        _draining = false;
    }

    IEnumerator Blink()
    {
        _blinking = true;
        bool _blinkInProcess = false;
        while (_power < 30)
        {
            if (!_blinkInProcess)
            {
                lightSource.SetActive(false);
                _blinkInProcess = true;
                yield return new WaitForSeconds(_power / 12);
                lightSource.SetActive(true);
                yield return new WaitForSeconds(_power / 12);
                _blinkInProcess = false;
            }
        }
        _blinking = false;
    }

    private void Update()
    {
        if (gameObject.GetComponentInParent<PlayerStateMachine>())
        {
            if (!_draining)
            StartCoroutine(DrainPower(2.5f));

            if (_power < 30 && _power >= 0 && !_blinking)
            {
                StartCoroutine(Blink());
            }
            else
            {
                StopCoroutine(Blink());
            }
        }
    }
}
