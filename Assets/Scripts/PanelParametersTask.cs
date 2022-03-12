using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelParametersTask : MonoBehaviour
{
    public GameObject contentDatainput;
    private bool click = false;
    public InputField unputNameTask;
    public static string nameTaskText = "123"; 

    public void PaneOpeningRegulation()
    {
        if (!click)
        {
            contentDatainput.SetActive(true);
            click = true;
        }
        else
        {
            contentDatainput.SetActive(false);
            click = false;
        }
    }

    public string SetName()
    {
        nameTaskText = unputNameTask.text;
        return nameTaskText;
    }

}
