using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class
CheckBoxFixedTime : MonoBehaviour
{
    [SerializeField]  private Text inputField;
    public void clickCheckBox(bool checkBox)
    {
        if (checkBox)
        {
            inputField.text = "Время начала";
        }
        else
        {
            inputField.text = "Дедлайн";
        }
    }
}
