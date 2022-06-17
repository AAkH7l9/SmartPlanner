using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Newtonsoft.Json;
using Backend;

public class PresenterCalendar : MonoBehaviour
{
    TaskManager taskManager;
    [SerializeField] private GameObject prefabWeek;
    [SerializeField] private RectTransform contentCalendar;
    [SerializeField] private Text textMonth;
    [SerializeField] private Text textYear;
    [SerializeField] private Text dateFreeTime;
    [SerializeField] private Text freeTime;

    string[] listMonth = new string[12] { "€нварь", "февраль", "март", "апрель", "май", "июнь", "июль", "август", "сент€брь", "окт€брь", "но€брь", "декабрь" };
    string[] listMonthWithEnd = new string[12] { "€нвар€", "феврал€", "марта", "апрел€", "ма€", "июн€", "июл€", "августа", "сент€бр€", "окт€бр€", "но€бр€", "декабр€" };
    int submittedDay = 1;
    DateTime selectedDate = DateTime.Now;
    int submittedMonth = DateTime.Now.Month;
    int submittedYear = DateTime.Now.Year;
    int furstDayMonthOfWeek = (int)new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).DayOfWeek;
    int lastDayMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month + 1, 1).AddDays(-1).Day;


    public void Start()
    {
        taskManager = TaskManager.GetInstance();
        FillData();
        RefreshCalendar();
        Debug.Log(submittedMonth);
    }

    private void FillData()
    {
        textYear.text = DateTime.Now.Year.ToString();
        textMonth.text = listMonth[DateTime.Now.Month-1];
    }

    public void onClickBackMonth()
    {
        if (WhatMonth(textMonth.text) == 0)
        {
            textMonth.text = listMonth[11];
            textYear.text = (int.Parse(textYear.text) - 1).ToString();
        }
        else
        {
            textMonth.text = listMonth[WhatMonth(textMonth.text) - 1];
        }
        submittedMonth = WhatMonth(textMonth.text)+1;
        
        furstDayMonthOfWeek = (int)new DateTime(DateTime.Now.Year, submittedMonth, 1).DayOfWeek;
        lastDayMonth = new DateTime(DateTime.Now.Year, submittedMonth + 1, 1).AddDays(-1).Day;
        RefreshCalendar();
        Debug.Log(submittedMonth);
    }

    public void onClickNextMonth()
    {
        if (WhatMonth(textMonth.text) == 11)
        {
            textMonth.text = listMonth[0];
            textYear.text = (int.Parse(textYear.text) + 1).ToString();
        }
        else
        {
            textMonth.text = listMonth[WhatMonth(textMonth.text) + 1];
        }
        submittedMonth = WhatMonth(textMonth.text)+1;
        furstDayMonthOfWeek = (int)new DateTime(DateTime.Now.Year, submittedMonth, 1).DayOfWeek;
        lastDayMonth = new DateTime(DateTime.Now.Year, submittedMonth + 1, 1).AddDays(-1).Day;
        RefreshCalendar();
        Debug.Log(submittedMonth);
    }
    private int WhatMonth(string month)
    {
        for (int i = 0; i < listMonth.Length; i++)
        {
            if (listMonth[i] == month)
            {
                return i;
            }
        }
        return -1;
    }

    private DayType WhatDayType(int numberDay)
    {
        DateTime day = new DateTime(submittedYear, submittedMonth, numberDay);
        if (day < DateTime.Now)
        {
            return DayType.Past;
        }
        if (day == selectedDate)
        {
            return DayType.Selected;
        }
        if ((day >= DateTime.Now) && (day < selectedDate))
        {
            return DayType.Considered;
        }
        return DayType.Normal;
    }
    private void RefreshCalendar()
    {
        ClearListTasks();

        if (furstDayMonthOfWeek == 0)
        {
            furstDayMonthOfWeek = 7;
        }
        Day[] weekNumber = new Day[7] {
                new Day(0, DayType.Normal),
                new Day(0, DayType.Normal),
                new Day(0, DayType.Normal),
                new Day(0, DayType.Normal),
                new Day(0, DayType.Past),
                new Day(0, DayType.Normal),
                new Day(0, DayType.Normal)};
        for (int i = furstDayMonthOfWeek - 1; i < weekNumber.Length; i++)
        {
            weekNumber[i].numberDay = submittedDay;
            weekNumber[i].dayType = WhatDayType(submittedDay);
            submittedDay++;
        }
        StartCoroutine(GetItems(results => OnRecevedItem(results, weekNumber), weekNumber));


        Debug.Log(lastDayMonth);
        for (int i = 0; submittedDay <= lastDayMonth; i++)
        {
            weekNumber = new Day[7] { 
                new Day(0, DayType.Normal),
                new Day(0, DayType.Normal), 
                new Day(0, DayType.Normal),
                new Day(0, DayType.Normal),
                new Day(0, DayType.Normal),
                new Day(0, DayType.Normal),
                new Day(0, DayType.Normal)};
            for (int j = 0; j < weekNumber.Length; j++)
            {
                if(submittedDay <= lastDayMonth)
                {
                    weekNumber[j].numberDay = submittedDay;
                    weekNumber[j].dayType = WhatDayType(submittedDay);
                }
                submittedDay++;
            }
            StartCoroutine(GetItems(results => OnRecevedItem(results, weekNumber), weekNumber));
        }
    }

    void ClearListTasks()
    {
        foreach (Transform child in contentCalendar)
        {
            Destroy(child.gameObject);
        }

        submittedDay = 1;
    }

    private void OnChooseDay(int day)
    {
        selectedDate = new DateTime(int.Parse(textYear.text), WhatMonth(textMonth.text)+1, day) ;
        RefreshCalendar();
        double freeMinutes = taskManager.GetFreeTime(selectedDate).TotalMinutes;
        dateFreeTime.text = "ѕо " + selectedDate.Day.ToString() + " " + listMonthWithEnd[selectedDate.Month - 1 ] + " свободно";
        freeTime.text = (int)freeMinutes/60 - 24 + "ч. " + (int)freeMinutes%60 + "мин.";
    }

    void OnRecevedItem(ItemListModel item, Day[] weekNumber)
    {
        var instance = Instantiate(prefabWeek.gameObject);
        instance.transform.SetParent(contentCalendar, false);
        InitializeItemView(instance, item, weekNumber);
    }

    [SerializeField] private Color colorSelected;
    [SerializeField] private Color colorPast;
    [SerializeField] private Color colorConsidered;

    void InitializeItemView(GameObject viewGameObject, ItemListModel model, Day[] weekNumber)
    {
        ItemListView view = new ItemListView(viewGameObject.transform);

        switch (weekNumber[0].dayType)
        {
            case DayType.Normal:
                break;
            case DayType.Past:
                view.textMon.color = colorPast;
                view.buttonMon.image.color = colorPast; 
                break;

            case DayType.Selected:
                view.textMon.color = colorSelected;
                view.buttonMon.image.color = colorSelected;
                break;

            case DayType.Considered:
                view.textMon.color = colorConsidered;
                view.buttonMon.image.color = colorConsidered;
                break;

            default:
                break;
        }

        switch (weekNumber[1].dayType)
        {
            case DayType.Normal:
                break;
            case DayType.Past:
                view.textTue.color = colorPast;
                view.buttonTue.image.color = colorPast;
                break;

            case DayType.Selected:
                view.textTue.color = colorSelected;
                view.buttonTue.image.color = colorSelected;
                break;

            case DayType.Considered:
                view.textTue.color = colorConsidered;
                view.buttonTue.image.color = colorConsidered;
                break;

            default:
                break;
        }

        switch (weekNumber[2].dayType)
        {
            case DayType.Normal:
                break;
            case DayType.Past:
                view.textWed.color = colorPast;
                view.buttonWed.image.color = colorPast;
                break;

            case DayType.Selected:
                view.textWed.color = colorSelected;
                view.buttonWed.image.color = colorSelected;
                break;

            case DayType.Considered:
                view.textWed.color = colorConsidered;
                view.buttonWed.image.color = colorConsidered;
                break;

            default:
                break;
        }

        switch (weekNumber[3].dayType)
        {
            case DayType.Normal:
                break;
            case DayType.Past:
                view.textThu.color = colorPast;
                view.buttonThu.image.color = colorPast;
                break;

            case DayType.Selected:
                view.textThu.color = colorSelected;
                view.buttonThu.image.color = colorSelected;
                break;

            case DayType.Considered:
                view.textThu.color = colorConsidered;
                view.buttonThu.image.color = colorConsidered;
                break;

            default:
                break;
        }

        switch (weekNumber[4].dayType)
        {
            case DayType.Normal:
                break;
            case DayType.Past:
                view.textFri.color = colorPast;
                view.buttonFri.image.color = colorPast;
                break;

            case DayType.Selected:
                view.textFri.color = colorSelected;
                view.buttonFri.image.color = colorSelected;
                break;

            case DayType.Considered:
                view.textFri.color = colorConsidered;
                view.buttonFri.image.color = colorConsidered;
                break;

            default:
                break;
        }

        switch (weekNumber[5].dayType)
        {
            case DayType.Normal:
                break;
            case DayType.Past:
                view.textSat.color = colorPast;
                view.buttonSat.image.color = colorPast;
                break;

            case DayType.Selected:
                view.textSat.color = colorSelected;
                view.buttonSat.image.color = colorSelected;
                break;

            case DayType.Considered:
                view.textSat.color = colorConsidered;
                view.buttonSat.image.color = colorConsidered;
                break;

            default:
                break;
        }
        switch (weekNumber[6].dayType)
        {
            case DayType.Normal:
                break;
            case DayType.Past:
                view.textSun.color = colorPast;
                view.buttonSun.image.color = colorPast;
                break;

            case DayType.Selected:
                view.textSun.color = colorSelected;
                view.buttonSun.image.color = colorSelected;
                break;

            case DayType.Considered:
                view.textSun.color = colorConsidered;
                view.buttonSun.image.color = colorConsidered;
                break;

            default:
                break;
        }
        
        view.textMon.text = model.numberMon;
        view.textTue.text = model.numberTue;
        view.textWed.text = model.numberWed; 
        view.textThu.text = model.numberThu;
        view.textFri.text = model.numberFri;
        view.textSat.text = model.numberSat;
        view.textSun.text = model.numberSun;

        if(weekNumber[0].numberDay == 0)
        {
            view.buttonMon.gameObject.SetActive(false);
        }
        if (weekNumber[1].numberDay == 0)
        {
            view.buttonTue.gameObject.SetActive(false);
        }
        if (weekNumber[2].numberDay == 0)
        {
            view.buttonWed.gameObject.SetActive(false);
        }
        if (weekNumber[3].numberDay == 0)
        {
            view.buttonThu.gameObject.SetActive(false);
        }
        if (weekNumber[4].numberDay == 0)
        {
            view.buttonFri.gameObject.SetActive(false);
        }
        if (weekNumber[5].numberDay == 0)
        {
            view.buttonSat.gameObject.SetActive(false);
        }
        if (weekNumber[6].numberDay == 0)
        {
            view.buttonSun.gameObject.SetActive(false);
        }



        view.buttonMon.onClick.AddListener(() =>
        {
            OnChooseDay(int.Parse(view.textMon.text));
        });

        view.buttonTue.onClick.AddListener(() =>
        {
            OnChooseDay(int.Parse(view.textTue.text));
        });

        view.buttonWed.onClick.AddListener(() =>
        {
            OnChooseDay(int.Parse(view.textWed.text));
        });

        view.buttonThu.onClick.AddListener(() =>
        {
            OnChooseDay(int.Parse(view.textThu.text));
        });

        view.buttonFri.onClick.AddListener(() =>
        {
            OnChooseDay(int.Parse(view.textFri.text));
        });

        view.buttonSat.onClick.AddListener(() =>
        {
            OnChooseDay(int.Parse(view.textSat.text));
        });

        view.buttonSun.onClick.AddListener(() =>
        {
            OnChooseDay(int.Parse(view.textSun.text));
        });

    }



    IEnumerator GetItems(System.Action<ItemListModel> callback, Day[] weekNumber)
    {
        var results = new ItemListModel();
        results.numberMon = weekNumber[0].numberDay.ToString();
        results.numberTue = weekNumber[1].numberDay.ToString();
        results.numberWed = weekNumber[2].numberDay.ToString();
        results.numberThu = weekNumber[3].numberDay.ToString();
        results.numberFri = weekNumber[4].numberDay.ToString();
        results.numberSat = weekNumber[5].numberDay.ToString();
        results.numberSun = weekNumber[6].numberDay.ToString();
        

        callback(results);

        yield return results;
    }

    public class ItemListView
    {
        public Text textMon;
        public Button buttonMon;
        public Text textTue;
        public Button buttonTue;
        public Text textWed;
        public Button buttonWed;
        public Text textThu;
        public Button buttonThu;
        public Text textFri;
        public Button buttonFri;
        public Text textSat;
        public Button buttonSat;
        public Text textSun;
        public Button buttonSun;

        public ItemListView(Transform rootView)
        {
            buttonMon = rootView.Find("Mon").GetComponent<Button>();
            textMon = buttonMon.transform.Find("Text").GetComponent<Text>();

            buttonTue = rootView.Find("Tue").GetComponent<Button>();
            textTue = buttonTue.transform.Find("Text").GetComponent<Text>();

            buttonWed = rootView.Find("Wed").GetComponent<Button>();
            textWed = buttonWed.transform.Find("Text").GetComponent<Text>();
            
            buttonThu = rootView.Find("Thu").GetComponent<Button>();
            textThu = buttonThu.transform.Find("Text").GetComponent<Text>();
            
            buttonFri = rootView.Find("Fri").GetComponent<Button>();
            textFri = buttonFri.transform.Find("Text").GetComponent<Text>();
            
            buttonSat = rootView.Find("Sat").GetComponent<Button>();
            textSat = buttonSat.transform.Find("Text").GetComponent<Text>();

            buttonSun = rootView.Find("Sun").GetComponent<Button>();
            textSun = buttonSun.transform.Find("Text").GetComponent<Text>();

        }
    }

    public class ItemListModel
    {
        public string numberMon;
        public string numberTue;
        public string numberWed;
        public string numberThu;
        public string numberFri;
        public string numberSat;
        public string numberSun;
    }
}

public class Day
{
    public int numberDay;
    public DayType dayType;

    public Day() { }

    public Day(int _numberDay, DayType _dayType)
    {
        numberDay = _numberDay;
        dayType = _dayType;
    }
}

public enum DayType
{
    Normal, 
    Past,
    Selected,
    Considered
}
