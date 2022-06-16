using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckBoxWaiting : MonoBehaviour
{
    [SerializeField] private GameObject starImportance;
    [SerializeField] private GameObject starTimeExecution;
    [SerializeField] private GameObject starDataDeadline;
    [SerializeField] private GameObject starTimeDeadline;

    public void clickCheckBox(bool checkBox)
    {
        starImportance.SetActive(!checkBox);
        starTimeExecution.SetActive(!checkBox);
        starDataDeadline.SetActive(!checkBox);
        starTimeDeadline.SetActive(!checkBox);
    }
}
