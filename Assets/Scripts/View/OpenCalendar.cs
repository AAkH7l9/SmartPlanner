using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenCalendar : MonoBehaviour
{
    public GameObject panel;
    [SerializeField] private Animation calendar;
    [SerializeField] private AnimationClip openCalendar;
    [SerializeField] private AnimationClip closeCalendar;

    [SerializeField] private Animation btnClosePanel;
    [SerializeField] private AnimationClip openbtnClosePanel;
    [SerializeField] private AnimationClip closebtnClosePanel;

    public void OpenPanel()
    {
        calendar.clip = openCalendar;
        btnClosePanel.clip = openbtnClosePanel;
        panel.SetActive(true);
        calendar.Play();
        btnClosePanel.Play();
    }

    public void ClosePanelAnim()
    {
        btnClosePanel.clip = closebtnClosePanel;
        calendar.clip = closeCalendar;
        btnClosePanel.Play();
        calendar.Play();
    }

    public void ClosePanel()
    {
        panel.SetActive(false);
    }
}
