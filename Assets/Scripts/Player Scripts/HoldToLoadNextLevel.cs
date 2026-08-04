using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class HoldToLoadNextLevel : MonoBehaviour
{
    public float holdDuration = 1f; //how long to hold for next level
    public Image fillCircle;

    private float holdTimer = 0;
    private bool isHolding = false;

    public static event Action OnHoldComplete;


    void Update()
    {
        if (isHolding)
        {
            holdTimer += Time.deltaTime;
            fillCircle.fillAmount = holdTimer / holdDuration;

            if(holdTimer >= holdDuration )
            {
                //LOAD NEXT LEVEL
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

    public void ResetHold()
    {
        isHolding = false;
        holdTimer = 0;
        fillCircle.fillAmount = 0;
    }
}
