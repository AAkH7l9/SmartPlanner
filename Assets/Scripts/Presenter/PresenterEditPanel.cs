using UnityEngine;
using Backend;
using UnityEngine.UI;

using System.Text.RegularExpressions;

public class PresenterEditPanel : MonoBehaviour
{

    private void Start()
    {
        taskManager = TaskManager.GetInstance();
        FillDate();
    }

    TaskManager taskManager;

    [SerializeField] private Toggle sameTimeAllDays;

    [SerializeField] private InputField day1;
    [SerializeField] private InputField day2;
    [SerializeField] private InputField day3;
    [SerializeField] private InputField day4;
    [SerializeField] private InputField day5;
    [SerializeField] private InputField day6;
    [SerializeField] private InputField day7;

    public void SaveDate()
    {
        if (sameTimeAllDays.isOn)
        {
            if (CorrectnessDateDay())
            {
                string[] blokingTime = new string[] { 
                    day1.text, day1.transform.Find("InputFieldDayTimeEnd").GetComponent<InputField>().text,
                    day1.text, day1.transform.Find("InputFieldDayTimeEnd").GetComponent<InputField>().text,
                    day1.text, day1.transform.Find("InputFieldDayTimeEnd").GetComponent<InputField>().text,
                    day1.text, day1.transform.Find("InputFieldDayTimeEnd").GetComponent<InputField>().text,
                    day1.text, day1.transform.Find("InputFieldDayTimeEnd").GetComponent<InputField>().text,
                    day1.text, day1.transform.Find("InputFieldDayTimeEnd").GetComponent<InputField>().text,
                    day1.text, day1.transform.Find("InputFieldDayTimeEnd").GetComponent<InputField>().text};
                taskManager.SetBlockingTime( blokingTime);
            }
        }
        else
        {
            if (CorrectnessDate7Days())
            {
                string[] blokingTime = new string[] {
                    day1.text, day1.transform.Find("InputFieldDayTimeEnd").GetComponent<InputField>().text,
                    day2.text, day2.transform.Find("InputFieldDayTimeEnd").GetComponent<InputField>().text,
                    day3.text, day3.transform.Find("InputFieldDayTimeEnd").GetComponent<InputField>().text,
                    day4.text, day4.transform.Find("InputFieldDayTimeEnd").GetComponent<InputField>().text,
                    day5.text, day5.transform.Find("InputFieldDayTimeEnd").GetComponent<InputField>().text,
                    day6.text, day6.transform.Find("InputFieldDayTimeEnd").GetComponent<InputField>().text,
                    day7.text, day7.transform.Find("InputFieldDayTimeEnd").GetComponent<InputField>().text};
                taskManager.SetBlockingTime(blokingTime);
            }
        }
        
    }
    private bool CorrectnessDateDay()
    {
        bool correct = true;

        day1.image.color = Color.white;

        if (!Regex.Match(day1.text, @"\d{2}:\d{2}").Success)
        {
            correct = false;
            day1.image.color = Color.red;
        }
        if (!Regex.Match(day1.transform.Find("InputFieldDayTimeEnd").GetComponent<InputField>().text, @"\d{2}:\d{2}").Success)
        {
            correct = false;
            day1.image.color = Color.red;
        }

        return correct;
    }
    private bool CorrectnessDate7Days()
    {
        bool correct = true;

        day1.image.color = Color.white;
        day2.image.color = Color.white;
        day3.image.color = Color.white;
        day4.image.color = Color.white;
        day5.image.color = Color.white;
        day6.image.color = Color.white;
        day7.image.color = Color.white;

        if (!Regex.Match(day1.text, @"\d{2}:\d{2}").Success)
        {
            correct = false;
            day1.image.color = Color.red;
        }
        if (!Regex.Match(day1.transform.Find("InputFieldDayTimeEnd").GetComponent<InputField>().text, @"\d{2}:\d{2}").Success)
        {
            correct = false;
            day1.image.color = Color.red;
        }

        if (!Regex.Match(day2.text, @"\d{2}:\d{2}").Success)
        {
            correct = false;
            day2.image.color = Color.red;
        }
        if (!Regex.Match(day2.transform.Find("InputFieldDayTimeEnd").GetComponent<InputField>().text, @"\d{2}:\d{2}").Success)
        {
            correct = false;
            day2.image.color = Color.red;
        }

        if (!Regex.Match(day3.text, @"\d{2}:\d{2}").Success)
        {
            correct = false;
            day3.image.color = Color.red;
        }
        if (!Regex.Match(day3.transform.Find("InputFieldDayTimeEnd").GetComponent<InputField>().text, @"\d{2}:\d{2}").Success)
        {
            correct = false;
            day3.image.color = Color.red;
        }

        if (!Regex.Match(day4.text, @"\d{2}:\d{2}").Success)
        {
            correct = false;
            day4.image.color = Color.red;
        }
        if (!Regex.Match(day4.transform.Find("InputFieldDayTimeEnd").GetComponent<InputField>().text, @"\d{2}:\d{2}").Success)
        {
            correct = false;
            day4.image.color = Color.red;
        }

        if (!Regex.Match(day5.text, @"\d{2}:\d{2}").Success)
        {
            correct = false;
            day5.image.color = Color.red;
        }
        if (!Regex.Match(day5.transform.Find("InputFieldDayTimeEnd").GetComponent<InputField>().text, @"\d{2}:\d{2}").Success)
        {
            correct = false;
            day5.image.color = Color.red;
        }

        if (!Regex.Match(day6.text, @"\d{2}:\d{2}").Success)
        {
            correct = false;
            day6.image.color = Color.red;
        }
        if (!Regex.Match(day6.transform.Find("InputFieldDayTimeEnd").GetComponent<InputField>().text, @"\d{2}:\d{2}").Success)
        {
            correct = false;
            day6.image.color = Color.red;
        }

        if (!Regex.Match(day7.text, @"\d{2}:\d{2}").Success)
        {
            correct = false;
            day7.image.color = Color.red;
        }
        if (!Regex.Match(day7.transform.Find("InputFieldDayTimeEnd").GetComponent<InputField>().text, @"\d{2}:\d{2}").Success)
        {
            correct = false;
            day7.image.color = Color.red;
        }

        return correct;
    }

    private void FillDate()
    {
        string[] timeAllDay = taskManager.GetBlockingTime();
        
        bool sameTime = false;
        for (int i = 0; i < timeAllDay.Length-2; i++)
        {
            if (timeAllDay[i] == timeAllDay[i + 2])
            {
                sameTime = true;
            }
            else
            {
                sameTime = false;
                break;
            }
        }    
        sameTimeAllDays.isOn = sameTime ;
        if (sameTimeAllDays.isOn)
        {
            FillDay(day1, timeAllDay[0], timeAllDay[1]);
        }
        else
        {
            FillDay(day1, timeAllDay[0], timeAllDay[1]);
            FillDay(day2, timeAllDay[2], timeAllDay[3]);
            FillDay(day3, timeAllDay[4], timeAllDay[5]);
            FillDay(day4, timeAllDay[6], timeAllDay[7]);
            FillDay(day5, timeAllDay[8], timeAllDay[9]);
            FillDay(day6, timeAllDay[10], timeAllDay[11]);
            FillDay(day7, timeAllDay[12], timeAllDay[13]);
        }
    }

    public void FillDateView()
    {
        string[] timeAllDay = taskManager.GetBlockingTime();

        if (sameTimeAllDays.isOn)
        {
            FillDay(day1, timeAllDay[0], timeAllDay[1]);
        }
        else
        {
            FillDay(day1, timeAllDay[0], timeAllDay[1]);
            FillDay(day2, timeAllDay[2], timeAllDay[3]);
            FillDay(day3, timeAllDay[4], timeAllDay[5]);
            FillDay(day4, timeAllDay[6], timeAllDay[7]);
            FillDay(day5, timeAllDay[8], timeAllDay[9]);
            FillDay(day6, timeAllDay[10], timeAllDay[11]);
            FillDay(day7, timeAllDay[12], timeAllDay[13]);
        }
    }

    private void FillDay(InputField day, string startTime, string endTime)
    {
        day.text = startTime;
        day.transform.Find("InputFieldDayTimeEnd").GetComponent<InputField>().text = endTime;
    }

}
