using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenPanelEditApp : MonoBehaviour
{
    public GameObject panel;

    public void OpenPanel()
    {
        panel.SetActive(true);
    }

    public void ClosePanel()
    {
        panel.SetActive(false);
    }
}