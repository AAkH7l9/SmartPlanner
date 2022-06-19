using System;
using System.Collections;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
        ClearListTasks();
        for (int i = 0; i < listTasks.Length; i++)
        {
            StartCoroutine(GetItems(results => OnRecievedItem(results, listTasks[i]), listTasks[i]));
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
            animPanelAddTask.clip = animClosePanelEditTask;
            animButtonAddTask.clip = animClosePanel;
            animButtonAddTask.Play();
            animPanelAddTask.Play();
            contentDatainput.SetActive(false);
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

    public void AddTask()
    {
        if (textButtonCreateTask.text == "Сохранить изменения")
        {
            Task task = taskManager.GetTaskById(int.Parse(id.text));
            if (CorrectnessDateTask())
            {
                string dateString = dataDeadlineTask.text;
                string timeString = timeDeadlineTask.text;

                Debug.Log(dateString);
                Debug.Log(timeString);
                DateTime deadLine = DateTime.ParseExact(dateString + " " + timeString, "dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture);
                
                TaskStatus statusTask = TaskStatus.Relevant;
                if (waiting.isOn)
                {
                    statusTask = TaskStatus.Awaiting;
                }

                task.Name = nameTask.text;
                task.TimeInMinutes = int.Parse(timeExecutionTask.text);
                task.DataDeadline = deadLine;
                task.Importance = int.Parse(timeImportance.text);
                task.IsEnoughTime = true;
                task.IsFixed = fixedTask.isOn;
                task.Status = statusTask;


                taskManager.EditTask(task);
            }

            

        }
        else
        {
            if (CorrectnessDateTask())
            {
                string dateString = dataDeadlineTask.text;
                string timeString = timeDeadlineTask.text;
                Debug.Log(dateString);
                Debug.Log(timeString);

                DateTime deadLine = DateTime.ParseExact(dateString + " " + timeString, "dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture);

                TaskStatus statusTask = TaskStatus.Relevant;
                if (waiting.isOn)
                {
                    statusTask = TaskStatus.Awaiting;
                }
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
            }
        }
        RefreshListButton();
    }

    private bool CorrectnessDateTask()
    {
        /*bool correctData = true;
        Regex regex = new Regex(@"\d");
        Debug.Log("Работает");
        if (nameTask.text == "")
        {
            correctData = false;
            //Добавить изменения цвета у названия задачи
        }
        if( regex.IsMatch(timeDeadlineTask.text))
        {
            Debug.Log("Работает аывваывыа");
        }
        return correctData;*/
        return true;
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
    }

    private void FillDateFormTask(Task task)
    {
        nameTask.text = task.Name;
        if (task.DataDeadline.Minute.ToString().Length == 1)
        {
            timeDeadlineTask.text = task.DataDeadline.Hour.ToString() + ":0" + task.DataDeadline.Minute.ToString();
        }
        else
        {
            timeDeadlineTask.text = task.DataDeadline.Hour.ToString() + ":" + task.DataDeadline.Minute.ToString();
        }
        Debug.Log(task.DataDeadline.Month.ToString());
        if (task.DataDeadline.Month.ToString().Length == 1)
        {
            dataDeadlineTask.text = task.DataDeadline.Day.ToString() + ".0" + task.DataDeadline.Month.ToString() + "." + task.DataDeadline.Year.ToString();
        }
        else
        {
            dataDeadlineTask.text = task.DataDeadline.Day.ToString() + "." + task.DataDeadline.Month.ToString() + "." + task.DataDeadline.Year.ToString();
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
        
        switch (status)
        {
            case TaskStatus.Relevant:
                usingSpriteTask = imageActive;
                break;
            case TaskStatus.Awaiting:
                usingSpriteTask = imageWaiting;
                break;
            case TaskStatus.Overdue:
                usingSpriteTask = imageOveride;
                viewGameObject.transform.Find("View").Find("Content").Find("deadlineText").GetComponent<Text>().text = "Надо было начать до";
                OnClickNotification();
                break;
            case TaskStatus.Done:
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
                taskManager.SetTaskStatusById(int.Parse(view.id.text), TaskStatus.Done);
            }
            else
            {
                taskManager.SetTaskStatusById(int.Parse(view.id.text), TaskStatus.Relevant);
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
            if (status == TaskStatus.Awaiting) { }
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
        results.deadline = task.Beginning.ToString();
        results.timeExecution = task.TimeInMinutes.ToString();
        results.importance = task.Importance.ToString();
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
