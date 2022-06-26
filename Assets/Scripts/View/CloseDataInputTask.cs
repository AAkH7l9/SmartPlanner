using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseDataInputTask : MonoBehaviour
{
    [ SerializeField] private GameObject contentDataInput;
    public void ClosePanelDatainput()
    {
        contentDataInput.SetActive(false);
    }
}
