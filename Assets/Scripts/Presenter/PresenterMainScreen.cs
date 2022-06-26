using System;
using System.Collections;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.Animations;
using Newtonsoft.Json;

using Backend;
using Task = Backend.Task;


public class PresenterMainScreen : MonoBehaviour
{
    TaskManager taskManager;
    TaskStatus status = TaskStatus.Relevant;
    public void Start()
    {
        if (!PlayerPrefs.HasKey("json"))
            PlayerPrefs.SetString("json", "");
        else if (!PlayerPrefs.HasKey("idcounter"))
            PlayerPrefs.SetInt("idcounter", 0);
        else if (!PlayerPrefs.HasKey("blockingtime"))
            PlayerPrefs.SetString("blockingtime", @"['17:00', '20:00', '17:00', '20:00', '17:00', '20:00', '17:00', '20:00', '17:00', '20:00', '17:00', '20:00', '17:00', '20:00',]");
        List<Task> tasks = JsonConvert.DeserializeObject<List<Task>>(PlayerPrefs.GetString("json"));
        int idcounter = PlayerPrefs.GetInt("idcounter");
        List<string> blockingTime = JsonConvert.DeserializeObject<List<string>>(PlayerPrefs.GetString("blockingtime"));
        taskManager = TaskManager.GetInstance(tasks, blockingTime, idcounter);
        PrintTasks(taskManager.GetAllRelevantTasks().ToArray());
    }

    public void PrintTasks(Task[] listTasks)
    {
        var orderedList = from task in listTasks
                          orderby task.Beginning
                          select task;

        int i = 0;
        foreach (Task task in orderedList)
        {
            listTasks[i] = task;
            i++;
        }

        ClearListTasks();
        for (int j = 0; j < listTasks.Length; j++)
        {
            StartCoroutine(GetItems(results => OnRecievedItem(results, listTasks[j]), listTasks[j]));
        }
    }

    private bool refresh = false;
    public void RefreshList()
    {

        if (contentListTask.position.y < 3)
        {
            if (!refresh)
            {
                taskManager.UpdateTasks();
                switch(status)
                {
                    case TaskStatus.Relevant:
                        PrintTasks(taskManager.GetAllRelevantTasks().ToArray());
                        break;
                    case TaskStatus.Awaiting:
                        PrintTasks(taskManager.GetAllAwaitingTasks().ToArray());
                        break;
                    case TaskStatus.Overdue:
                        PrintTasks(taskManager.GetAllOverdueTasks().ToArray());
                        break;
                    case TaskStatus.Done:
                        PrintTasks(taskManager.GetAllDoneTasks().ToArray());
                        break;
                    default:
                        break;

                }
                NewOverrideTasks();
                refresh = true;
            }
        }
        else
        {
            refresh = false;
        }

    }

    public void RefreshListButton()
    {

        taskManager.UpdateTasks();
        switch (status)
        {
            case TaskStatus.Relevant:
                PrintTasks(taskManager.GetAllRelevantTasks().ToArray());
                break;
            case TaskStatus.Awaiting:
                PrintTasks(taskManager.GetAllAwaitingTasks().ToArray());
                break;
            case TaskStatus.Overdue:
                PrintTasks(taskManager.GetAllOverdueTasks().ToArray());
                break;
            case TaskStatus.Done:
                PrintTasks(taskManager.GetAllDoneTasks().ToArray());
                break;
            default:
                break;
        }
        NewOverrideTasks();
    }

    [Header("Оповещение о просроченных задачах")]
    [SerializeField] private Text notificationText;
    [SerializeField] private Animation newNotification;
    public void OnClickNotification()
    {
        notificationText.text = "";
    }
    private void NewOverrideTasks() {
        int countOverride = taskManager.GetOverdueTasksCount();
        if  ( countOverride > 0) {
            notificationText.text = countOverride.ToString();
            newNotification.Play();
        }
        else
        {
            notificationText.text = "";
        }
    }

    public void ChoosingTypeTasks(int value)
    {
        
        switch (value)
        {
            case 0:
                status = TaskStatus.Relevant;
                PrintTasks(taskManager.GetAllRelevantTasks().ToArray());
                break;
            case 1:
                status = TaskStatus.Awaiting;
                PrintTasks(taskManager.GetAllAwaitingTasks().ToArray());
                break;
            case 2:
                status = TaskStatus.Overdue;
                PrintTasks(taskManager.GetAllOverdueTasks().ToArray());
                break;

            case 3:
                status = TaskStatus.Done;
                PrintTasks(taskManager.GetAllDoneTasks().ToArray());
                break;
            default:
                break;
        }
    }

    [Header("Панель добавления задач")]
    [SerializeField] private GameObject contentDatainput;
    [SerializeField] private RectTransform buttonAddTask;
    [Header("Анимация кнопки добавления задач")]
    [SerializeField] private Animation animButtonAddTask;
    [SerializeField] private AnimationClip animAddTask;
    [SerializeField] private AnimationClip animClosePanel;
    [Header("Анимация панели добавления задач")]
    [SerializeField] private Animation animPanelAddTask;
    [SerializeField] private AnimationClip animOpenPanelEditTask;
    [SerializeField] private AnimationClip animClosePanelEditTask;
    private bool click = false; 

    

    public void PaneOpeningRegulation()
    {
        
        if (!click)
        {
            contentDatainput.SetActive(true);
            click = true;

            animPanelAddTask.clip = animOpenPanelEditTask;
            animButtonAddTask.clip = animAddTask;
            animButtonAddTask.Play();
            animPanelAddTask.Play();
        }
        else
        {
            ClearFormTask();
            animPanelAddTask.clip = animClosePanelEditTask;
            animButtonAddTask.clip = animClosePanel;
            animButtonAddTask.Play();
            animPanelAddTask.Play();
            
            click = false;
        }
    }

    [Header("Добавление задач")]
    [SerializeField] private InputField nameTask;
    [SerializeField] private InputField timeDeadlineTask;
    [SerializeField] private InputField dataDeadlineTask;
    [SerializeField] private InputField timeExecutionTask;
    [SerializeField] private InputField timeImportance;
    [SerializeField] private Toggle fixedTask;
    [SerializeField] private Toggle waiting;
    [SerializeField] private Text textButtonCreateTask;
    [SerializeField] private Text id;
    [Header("Картинки типов задач")]
    [SerializeField] private Sprite imageActive;
    [SerializeField] private Sprite imageWaiting;
    [SerializeField] private Sprite imageOveride;
    [SerializeField] private Sprite imageCompleted;
    public GameObject prefabTask;
    public RectTransform contentListTask;

    [Header("Кнопки на задачах")]
    [SerializeField] private Sprite buttonActive;
    [SerializeField] private Sprite buttonWaiting;
    [SerializeField] private Sprite buttonDelete;
    [SerializeField] private Sprite buttonCompleted;

    public void AddTaskView()
    {
        AddTask();
    }
    private bool AddTask()
    {
        if (textButtonCreateTask.text == "Сохранить изменения")
        {
            Task task = taskManager.GetTaskById(int.Parse(id.text));
            if (waiting.isOn)
            {
                if (CorrectnessDateTaskAwaiting())
                {
                    DateTime deadLine;
                    string dateString;
                    string timeString;
                    if (timeDeadlineTask.text == "")
                    {
                        timeString = "00:00";
                    }
                    else
                    {
                        timeString = timeDeadlineTask.text;
                    }
                    if (dataDeadlineTask.text == "")
                    {
                        deadLine = DateTime.MinValue;
                    }
                    else
                    {
                        dateString = dataDeadlineTask.text;
                        deadLine = DateTime.ParseExact(dateString + " " + timeString, "dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture);
                    }




                    TaskStatus statusTask = TaskStatus.Awaiting;

                    task.Name = nameTask.text;

                    if (timeExecutionTask.text == "")
                    {
                        task.TimeInMinutes = 0;
                    }
                    else
                    {
                        task.TimeInMinutes = int.Parse(timeExecutionTask.text);
                    }

                    deadLine = deadLine.AddMinutes( task.TimeInMinutes);

                    if (fixedTask.isOn)
                    {
                        task.DataDeadline = deadLine.AddMinutes(task.TimeInMinutes);
                    }
                    else
                    {
                        task.DataDeadline = deadLine;
                    }

                    if (timeImportance.text == "")
                    {
                        task.Importance = -1;
                    }
                    else
                    {
                        task.Importance = int.Parse(timeImportance.text);
                    }
                    
                    task.IsEnoughTime = true;
                    task.IsFixed = fixedTask.isOn;
                    task.Status = statusTask;


                    taskManager.EditTask(task);
                    RefreshListButton();
                    PaneOpeningRegulation();
                    ClearFormTask();
                    return true;
                }
            }
            else
            {
                if (CorrectnessDateTask())
                {
                    string dateString = dataDeadlineTask.text;
                    string timeString = timeDeadlineTask.text;

                    DateTime deadLine = DateTime.ParseExact(dateString + " " + timeString, "dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture);

                    TaskStatus statusTask = TaskStatus.Relevant;
                    if (waiting.isOn)
                    {
                        statusTask = TaskStatus.Awaiting;
                    }

                    task.Name = nameTask.text;
                    task.TimeInMinutes = int.Parse(timeExecutionTask.text);
                    if (fixedTask.isOn)
                    {
                        task.DataDeadline = deadLine.AddMinutes(int.Parse(timeExecutionTask.text));
                    }
                    else
                    {
                        task.DataDeadline = deadLine;
                    }

                    task.Importance = int.Parse(timeImportance.text);
                    task.IsEnoughTime = true;
                    task.IsFixed = fixedTask.isOn;
                    task.Status = statusTask;


                    taskManager.EditTask(task);
                    RefreshListButton();
                    PaneOpeningRegulation();
                    ClearFormTask();
                    return true;
                }
            }
        }
        else
        {
            if (waiting.isOn)
            {
                TaskStatus statusTask = TaskStatus.Awaiting;
                if (CorrectnessDateTaskAwaiting())
                {
                    DateTime deadLine;
                    string dateString;
                    string timeString;
                    if (timeDeadlineTask.text == "")
                    {
                        timeString = "00:00";
                    }
                    else
                    {
                        timeString = timeDeadlineTask.text;
                    }
                    if (dataDeadlineTask.text == "")
                    {
                        deadLine = DateTime.MinValue;
                    }
                    else
                    {
                        dateString = dataDeadlineTask.text;
                        deadLine = DateTime.ParseExact(dateString + " " + timeString, "dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture);
                    }

                    string timeExecuton;

                    if (timeExecutionTask.text == "")
                    {
                        timeExecuton = "0";
                    }
                    else
                    {
                        timeExecuton = timeExecutionTask.text;
                    }

                    deadLine = deadLine.AddMinutes(int.Parse(timeExecuton));

                    string importanceTask;

                    if (timeImportance.text == "")
                    {
                        importanceTask = "-1";
                    }
                    else
                    {
                        importanceTask = timeImportance.text;
                    }

                    Task task = new Task(
                        nameTask.text,
                        int.Parse(timeExecuton),
                        deadLine,
                        int.Parse(importanceTask),
                        isEnoughTime: true,
                        fixedTask.isOn,
                        statusTask
                        );

                    if (fixedTask.isOn)
                    {
                        task.DataDeadline = deadLine.AddMinutes(int.Parse(timeExecutionTask.text));
                    }
                    else
                    {
                        task.DataDeadline = deadLine;
                    }

                    taskManager.AddTask(task);

                    RefreshListButton();
                    PaneOpeningRegulation();
                    ClearFormTask();
                    return true;
                }
            }
            else {
                if (CorrectnessDateTask())
                {
                    string dateString = dataDeadlineTask.text;
                    string timeString = timeDeadlineTask.text;

                    DateTime deadLine = DateTime.ParseExact(dateString + " " + timeString, "dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture);

                    TaskStatus statusTask = TaskStatus.Relevant;

                    deadLine = deadLine.AddMinutes(int.Parse(timeExecutionTask.text));

                    Task task = new Task(
                        nameTask.text,
                        int.Parse(timeExecutionTask.text),
                        deadLine,
                        int.Parse(timeImportance.text),
                        isEnoughTime: true,
                        fixedTask.isOn,
                        statusTask
                        );
                    taskManager.AddTask(task);
                    RefreshListButton();
                    PaneOpeningRegulation();
                    ClearFormTask();
                    return true;
                }
            }
            
        }
        return false;
       
    }

    private bool CorrectnessDateTask()
    {
        bool correctData = true;
        if(!Regex.Match(dataDeadlineTask.text, @"\d{2}\.\d{2}\.\d{4}").Success)
        {
            dataDeadlineTask.image.color = Color.red;
            correctData = false;
        }
        else
        {
            dataDeadlineTask.image.color = Color.white;
        }

        if (!Regex.Match(timeDeadlineTask.text, @"\d{2}:\d{2}").Success)
        {
            timeDeadlineTask.image.color = Color.red;
            correctData = false;
        }
        else
        {
            dataDeadlineTask.image.color = Color.white;
        }

        if (nameTask.text == "")
        {
            nameTask.image.color = Color.red;
            correctData = false;
        }
        else
        {
            nameTask.image.color = Color.white;
        }

        if (!Regex.Match(timeExecutionTask.text, @"[1-9]\d*").Success)
        {
            timeExecutionTask.image.color = Color.red;
            correctData = false;
        }
        else
        {
            timeExecutionTask.image.color = Color.white;
        }

        if (!Regex.Match(timeImportance.text, @"^([1-9]|10)$").Success)
        {
            timeImportance.image.color = Color.red;
            correctData = false;
        }
        else
        {
            timeImportance.image.color = Color.white;
        }

        return correctData;
    }

    private bool CorrectnessDateTaskAwaiting()
    {
        bool correctData = true;
        if (!Regex.Match(dataDeadlineTask.text, @"(\d{2}\.\d{2}\.\d{4})?").Success)
        {
            dataDeadlineTask.image.color = Color.red;
            correctData = false;
        }
        else
        {
            dataDeadlineTask.image.color = Color.white;
        }

        if (!Regex.Match(timeDeadlineTask.text, @"(\d{2}:\d{2})?").Success)
        {
            timeDeadlineTask.image.color = Color.red;
            correctData = false;
        }
        else
        {
            timeDeadlineTask.image.color = Color.white;
        }

        if (nameTask.text == "")
        {
            nameTask.image.color = Color.red;
            correctData = false;
        }
        else
        {
            nameTask.image.color = Color.white;
        }

        if (!Regex.Match(timeExecutionTask.text, @"([1-9]\d*)?").Success)
        {
            timeExecutionTask.image.color = Color.red;
            correctData = false;
        }
        else
        {
            timeExecutionTask.image.color = Color.white;
        }

        if (!Regex.Match(timeImportance.text, @"(^[1-9]$|^10$)?").Success)
        {
            timeImportance.image.color = Color.red;
            correctData = false;
        }
        else
        {
            timeImportance.image.color = Color.white;
        }

        return correctData;
    }

    public void ClearFormTask()
    {
        nameTask.text = "";
        timeDeadlineTask.text = "";
        dataDeadlineTask.text = "";
        timeExecutionTask.text = "";
        timeImportance.text = "";
        fixedTask.isOn = false;
        waiting.isOn = false;
        textButtonCreateTask.text = "Создать задачу";

        dataDeadlineTask.image.color = Color.white;
        timeDeadlineTask.image.color = Color.white;
        nameTask.image.color = Color.white;
        timeExecutionTask.image.color = Color.white;
        timeImportance.image.color = Color.white;
    }

    private void FillDateFormTask(Task task)
    {
        DateTime deadline = task.DataDeadline;
        if (task.IsFixed)
        {
            deadline = task.DataDeadline.AddMinutes(-task.TimeInMinutes);
        }
        nameTask.text = task.Name;
        if (deadline.Minute.ToString().Length == 1)
        {
            timeDeadlineTask.text = deadline.Hour.ToString() + ":0" + deadline.Minute.ToString();
        }
        else
        {
            timeDeadlineTask.text = deadline.Hour.ToString() + ":" + deadline.Minute.ToString();
        }
        if (deadline.Month.ToString().Length == 1)
        {
            dataDeadlineTask.text = deadline.Day.ToString() + ".0" + deadline.Month.ToString() + "." + deadline.Year.ToString();
        }
        else
        {
            dataDeadlineTask.text = deadline.Day.ToString() + "." + deadline.Month.ToString() + "." + deadline.Year.ToString();
        }


        timeExecutionTask.text = task.TimeInMinutes.ToString();
        timeImportance.text = task.Importance.ToString();
        fixedTask.isOn = task.IsFixed;
        if (task.Status == TaskStatus.Awaiting)
        {
            waiting.isOn = true;
        }
        else
        {
            waiting.isOn = false;
        }
        textButtonCreateTask.text = "Сохранить изменения";
        id.text = task.Id.ToString();
    }

    void ClearListTasks()
    {
        foreach (Transform child in contentListTask)
        {
            Destroy(child.gameObject);
        }
    }

    void OnRecievedItem(ItemListModel item, Task task)
    {
        var instance = Instantiate(prefabTask.gameObject);
        instance.transform.SetParent(contentListTask, false);
        InitializeItemView(instance, item, task);
    }

    void InitializeItemView(GameObject viewGameObject, ItemListModel model, Task task)
    {
        ItemListView view = new ItemListView(viewGameObject.transform, task);

        Animation animDestroy = viewGameObject.GetComponent<Animation>();
        Sprite usingSpriteTask = imageActive;
        
        if (task.IsFixed)
        {
            viewGameObject.transform.Find("View").Find("Content").Find("deadlineText").GetComponent<Text>().text = "Необходимо начать в";
        }

        switch (status)
        {
            case TaskStatus.Relevant:
                viewGameObject.transform.Find("View").Find("Content").Find("ButtonCompleted").GetComponent<Image>().sprite = buttonCompleted;
                usingSpriteTask = imageActive;
                break;
            case TaskStatus.Awaiting:
                viewGameObject.transform.Find("View").Find("Content").Find("ButtonWaiting").GetComponent<Image>().sprite = buttonActive;
                usingSpriteTask = imageWaiting;
                break;
            case TaskStatus.Overdue:
                usingSpriteTask = imageOveride;
                viewGameObject.transform.Find("View").Find("Content").Find("deadlineText").GetComponent<Text>().text = "Надо было начать до";
                if (task.IsFixed)
                {
                    viewGameObject.transform.Find("View").Find("Content").Find("deadlineText").GetComponent<Text>().text = "Надо было начать в";
                }
                OnClickNotification();
                break;
            case TaskStatus.Done:
                viewGameObject.transform.Find("View").Find("Content").Find("ButtonCompleted").GetComponent<Image>().sprite = buttonActive;
                
                usingSpriteTask = imageCompleted;
                break;
            default:
                usingSpriteTask = imageActive;
                break;
        }
        viewGameObject.transform.Find("View").Find("Content").GetComponent<Image>().sprite = usingSpriteTask;

        view.title.text = model.title;
        view.deadline.text = model.deadline;
        view.timeExecution.text = model.timeExecution;
        view.importance.text = model.importance;
        view.id.text = model.id;


        view.buttonTask.onClick.AddListener(() =>
        {
            
            Task task = taskManager.GetTaskById(int.Parse(view.id.text));
            FillDateFormTask(task);
            PaneOpeningRegulation();


        });

        view.buttonCompleted.onClick.AddListener(() =>
        {
            if (status == TaskStatus.Done)
            {
                taskManager.SetTaskStatusById(int.Parse(view.id.text), TaskStatus.Relevant);
            }
            else
            {
                taskManager.SetTaskStatusById(int.Parse(view.id.text), TaskStatus.Done);
            }
            animDestroy.Play();
        });

        view.buttonDelete.onClick.AddListener(() =>
        {
            taskManager.DeleteTaskById(int.Parse(view.id.text));
            animDestroy.Play();

        });

        view.buttonWaiting.onClick.AddListener(() =>
        {
            if (status == TaskStatus.Awaiting) {
                Task task = taskManager.GetTaskById(int.Parse(view.id.text));
                task.Status = TaskStatus.Relevant;
                FillDateFormTask(task);
                PaneOpeningRegulation();
                if (AddTask())
                {
                    contentDatainput.SetActive(false);
                    animDestroy.Play();
                }
                else
                {
                    taskManager.SetTaskStatusById(int.Parse(view.id.text), TaskStatus.Awaiting);
                }
                
            }
            else {
                taskManager.SetTaskStatusById(int.Parse(view.id.text), TaskStatus.Awaiting);
                animDestroy.Play();
            }
            
        });
    }

    

    IEnumerator GetItems(System.Action<ItemListModel> callback, Task task)
    {
        var results = new ItemListModel();
        results.title = task.Name;
        if(task.Beginning == DateTime.MinValue)
        {
            results.deadline = "";
        }
        else
        {
            results.deadline = task.Beginning.ToString();
        }
        
        if(task.TimeInMinutes == 0)
        {
            results.timeExecution = "";
        }
        else
        {
            results.timeExecution = task.TimeInMinutes.ToString();
        }

        if(task.Importance == -1)
        {
            results.importance = "";
        }
        else
        {
            results.importance = task.Importance.ToString();
        }
        
        
        results.id = task.Id.ToString();

        callback(results);
        yield return results;
    }

    public class ItemListView
    {
        public Text id;
        public Text title;
        public Text deadline;
        public Text timeExecution;
        public Text importance;
        public Image taskFixedIcon;
        public Button buttonTask;
        public Button buttonCompleted;
        public Button buttonDelete;
        public Button buttonWaiting;

        public ItemListView(Transform rootView, Task task)
        {
            Transform taskView = rootView.Find("View").Find("Content");
            id = rootView.Find("ID").GetComponent<Text>();
            buttonTask = taskView.GetComponent<Button>();
            buttonCompleted = taskView.Find("ButtonCompleted").GetComponent<Button>();
            buttonDelete = taskView.Find("ButtonDelete").GetComponent<Button>();
            buttonWaiting = taskView.Find("ButtonWaiting").GetComponent<Button>();
            title = taskView.Find("title").GetComponent<Text>();
            deadline = taskView.Find("deadline").GetComponent<Text>();
            timeExecution = taskView.Find("executionTime").GetComponent<Text>();
            importance = taskView.Find("importance").GetComponent<Text>();
            taskFixedIcon = taskView.Find("lock").GetComponent<Image>();
            
            if (!task.IsFixed)
            {
                Destroy(taskFixedIcon);
            }
            
        }
    }

    public class ItemListModel
    {
        public string title;
        public string deadline;
        public string timeExecution;
        public string importance;
        public string id;
    }

    

}
