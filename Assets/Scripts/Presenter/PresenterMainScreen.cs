using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PresenterMainScreen : MonoBehaviour
{
    public void ChoosingTypeTasks(int value)
    {
        Debug.Log(value.ToString());
    }

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

    [SerializeField] private InputField nameTask;
    [SerializeField] private InputField timeDeadlineTask;
    [SerializeField] private InputField dataDeadlineTask;
    [SerializeField] private InputField timeExecutionTask;
    [SerializeField] private InputField timeImportance;
    [SerializeField] private Toggle fixedTask;
    [SerializeField] private Toggle waiting;
    public GameObject prefabTask;
    public RectTransform contentListTask;
    private string titleTask = "";
    private string deadline = "";
    private string timeExecution = "";
    private string importance = "";
    private bool taskFixed = false;
    private int idCounter = 0;

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

    public void AddTask()
    {
        titleTask = nameTask.text.ToString();
        deadline = timeDeadlineTask.text.ToString() + dataDeadlineTask.text.ToString();
        timeExecution = timeExecutionTask.text.ToString();
        importance = timeImportance.text.ToString();
        taskFixed = fixedTask.isOn;
        StartCoroutine(GetItems(results => OnRecevedItem(results)));
        PaneOpeningRegulation();
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
        
        view.title.text = model.title;
        view.deadline.text = model.deadline;
        view.timeExecution.text = model.timeExecution;
        view.importance.text = model.importance;
        view.id.text = model.id;

        view.buttonTask.onClick.AddListener(() =>
        {
            PrintId(view.id.text + "Task");
        });
    }

    IEnumerator GetItems(System.Action<ItemListModel> callback)
    {
        yield return new WaitForSeconds(0);
        var results = new ItemListModel();
        results.title = titleTask;
        results.deadline = deadline;
        results.timeExecution = timeExecution;
        results.importance = importance;

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

        public ItemListView(Transform rootView, bool fixedTask)
        {
            Transform taskView = rootView.Find("View").Find("Content");
            id = rootView.Find("ID").GetComponent<Text>();
            title = taskView.Find("title").GetComponent<Text>();
            deadline = taskView.Find("deadline").GetComponent<Text>();
            timeExecution = taskView.Find("executionTime").GetComponent<Text>();
            importance = taskView.Find("importance").GetComponent<Text>();
            taskFixedIcon = taskView.Find("lock").GetComponent<Image>();
            buttonTask = rootView.Find("View").Find("Content").GetComponent<Button>();
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
