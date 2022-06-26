using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenPanelEditApp : MonoBehaviour
{
    public GameObject panel;
    [SerializeField] private Animation panelEdit;
    [SerializeField] private AnimationClip openPanelEdit;
    [SerializeField] private AnimationClip closePanelEdit;

    [SerializeField] private Animation btnClosePanel;
    [SerializeField] private AnimationClip openbtnClosePanel;
    [SerializeField] private AnimationClip closebtnClosePanel;
    public void OpenPanel()
    {
        panelEdit.clip = openPanelEdit;
        btnClosePanel.clip = openbtnClosePanel;
        panel.SetActive(true);
        panelEdit.Play();
        btnClosePanel.Play();
    }

    public void ClosePanelAnim()
    {
        btnClosePanel.clip = closebtnClosePanel;
        panelEdit.clip = closePanelEdit;
        panelEdit.Play();
        btnClosePanel.Play();
    }

    public void ClosePanel()
    {
        panel.SetActive(false);
    }
}
