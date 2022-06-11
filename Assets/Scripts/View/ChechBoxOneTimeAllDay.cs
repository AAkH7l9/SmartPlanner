using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChechBoxOneTimeAllDay : MonoBehaviour
{
    [SerializeField] private Text NameDay1;
    [SerializeField] private GameObject inputFieldDay2;
    [SerializeField] private GameObject inputFieldDay3;
    [SerializeField] private GameObject inputFieldDay4;
    [SerializeField] private GameObject inputFieldDay5;
    [SerializeField] private GameObject inputFieldDay6;
    [SerializeField] private GameObject inputFieldDay7;
    [SerializeField] private RectTransform buttonSave;
    public void clickCheckBox(bool checkBox)
    {
        if (checkBox)
        {
            NameDay1.text = "Ежедневно";
            inputFieldDay2.SetActive(false);
            inputFieldDay3.SetActive(false);
            inputFieldDay4.SetActive(false);
            inputFieldDay5.SetActive(false);
            inputFieldDay6.SetActive(false);
            inputFieldDay7.SetActive(false);
        }
        else
        {
            NameDay1.text = "Понедельник";
            inputFieldDay2.SetActive(true);
            inputFieldDay3.SetActive(true);
            inputFieldDay4.SetActive(true);
            inputFieldDay5.SetActive(true);
            inputFieldDay6.SetActive(true);
            inputFieldDay7.SetActive(true);
        }
    }
}
