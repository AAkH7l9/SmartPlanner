using System.Collections;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Animations;

public class PresenterMainScreen : MonoBehaviour
{
    TemporaryScript temporaryScript = new TemporaryScript();
    int typeTask = 0;
    public void Start()
    {
        PrintTasks(temporaryScript.ReturnTaskList(0));
    }

    public void PrintTasks(Task[] listTasks)
    {
        ClearListTasks();
        for (int i = 0; i < listTasks.Length; i++)
        {
            taskFixed = listTasks[i].@fixed;
            StartCoroutine(GetItems(results => OnRecevedItem(results), listTasks[i]));
        }
    }

    private bool refresh = false;
    public void RefreshList()
    {

        if (contentListTask.position.y < 3)
        {
            if (!refresh)
            {
                temporaryScript.UpdateList();
                PrintTasks(temporaryScript.ReturnTaskList(typeTask));
                refresh = true;
            }
        }
        else
        {
            refresh = false;
        }

    }

    public void ChoosingTypeTasks(int value)
    {
        typeTask = value;
        PrintTasks(temporaryScript.ReturnTaskList(typeTask));
    }

    [Header("Панель добавления задач")]
    public GameObject contentDatainput;
    public RectTransform buttonAddTask;
    private bool click = false; 

    public void PaneOpeningRegulation()
    {
        if (!click)
        {
            contentDatainput.SetActive(true);
            click = true;
            buttonAddTask.Rotate(0, 0, 45);
        }
        else
        {
            contentDatainput.SetActive(false);
            buttonAddTask.Rotate(0, 0, -45);
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
    public GameObject prefabTask;
    public RectTransform contentListTask;
    private bool taskFixed = false;
    private int idCounter = 0;

    public void AddTask()
    {
        CorrectnessDateTask();
        Debug.Log("Добавить задачу " + nameTask.text);
    }

    private bool CorrectnessDateTask()
    {
        bool correctData = true;
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
        return correctData;
    }

    private void PrintId(string id)
    {
        Debug.Log(id);
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
    }

    void ClearListTasks()
    {
        foreach (Transform child in contentListTask)
        {
            Destroy(child.gameObject);
        }
    }

    void OnRecevedItem(ItemListModel item)
    {
        var instance = Instantiate(prefabTask.gameObject);
        instance.transform.SetParent(contentListTask, false);
        InitializeItemView(instance, item);
    }

    void InitializeItemView(GameObject viewGameObject, ItemListModel model)
    {
        ItemListView view = new ItemListView(viewGameObject.transform, taskFixed);

        Animation animDestroy = viewGameObject.GetComponent<Animation>();

        view.title.text = model.title;
        view.deadline.text = model.deadline;
        view.timeExecution.text = model.timeExecution;
        view.importance.text = model.importance;
        view.id.text = model.id;

        view.buttonTask.onClick.AddListener(() =>
        {
            PrintId("Открыть редактирование задачи под номером:" + view.id.text + "");
            
        });

        view.buttonCompleted.onClick.AddListener(() =>
        {
            PrintId("Задача под номером " + view.id.text + " выполнена");
            animDestroy.Play();
        });

        view.buttonDelete.onClick.AddListener(() =>
        {
            PrintId("Задача под номером " + view.id.text + " удалена");
            animDestroy.Play();
        });

        view.buttonWaiting.onClick.AddListener(() =>
        {
            PrintId("Задача под номером " + view.id.text + " перемещена в ожидающие");
            animDestroy.Play();
        });
    }

    

    IEnumerator GetItems(System.Action<ItemListModel> callback, Task task)
    {
        yield return new WaitForSeconds(0);
        var results = new ItemListModel();
        results.title = task.name;
        results.deadline = task.dataDeadline.ToString();
        results.timeExecution = task.timeInMinutes.ToString();
        results.importance = task.importance.ToString();

        ++idCounter;
        results.id = idCounter.ToString();

        callback(results);
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

        public ItemListView(Transform rootView, bool fixedTask)
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
            
            if (!fixedTask)
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
