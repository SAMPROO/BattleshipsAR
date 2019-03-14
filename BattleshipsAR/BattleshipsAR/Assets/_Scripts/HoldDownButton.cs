using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HoldDownButton : MonoBehaviour
{
    public Image timerDisplay;
    public float holdTime = 1f;

    float timer = 0;
    bool isHeldDown = false;

    public bool Ready()
    {
        if (isHeldDown)
        {
            timer += Time.deltaTime;

            timerDisplay.fillAmount = timer / holdTime;

            if (timer >= holdTime)
            {
                timerDisplay.fillAmount = 0;
                timer = 0;
                isHeldDown = false;

                return true;
            }
        }

        return false;
    }

    public void ButtonDown()
    {
        isHeldDown = true;
    }

    public void ButtonUp()
    {
        timer = 0;
        timerDisplay.fillAmount = 0;

        isHeldDown = false;
    }
}
