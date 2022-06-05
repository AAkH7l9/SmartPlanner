using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckBoxWaiting : MonoBehaviour
{
    [SerializeField] private InputField importance;
    [SerializeField] private InputField timeExecution;
    [SerializeField] private InputField deadline;
    [SerializeField] private Color color;
    [SerializeField] private Color colorActive;
    public void clickCheckBox(bool checkBox)
    {
        if (checkBox)
        {
            importance.image.color = colorActive;
            timeExecution.image.color = colorActive;
            deadline.image.color = colorActive;
        }
        else
        {
            importance.image.color = color;
            timeExecution.image.color = color;
            deadline.image.color = color;
        }
    }
}
