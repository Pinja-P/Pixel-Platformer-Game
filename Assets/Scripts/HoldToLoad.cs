using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class HoldToLoad : MonoBehaviour
{
    public float holdDownDuration = 1f;
    public Image fillCircle;

    private float holdTimer = 0;
    private bool isHolding = false;

    public static event Action OnHoldComplete;
    

    void Update()
    {
        if (isHolding)
        {
            holdTimer += Time.deltaTime;
            fillCircle.fillAmount = holdTimer / holdDownDuration;

            if (holdTimer >= holdDownDuration)
            {
                // load next level
                OnHoldComplete.Invoke();
                ResetHold();
            }
        }
    }

    public void OnHold(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            isHolding = true;
        }
        else if (context.canceled)
        {
            ResetHold();
        }
    }

    private void ResetHold()
    {
        isHolding = false;
        holdTimer = 0;
        fillCircle.fillAmount = 0;
    }
}
