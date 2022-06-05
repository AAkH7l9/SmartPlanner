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
    }

    IEnumerator GetItems(System.Action<ItemListModel> callback)
    {
        yield return new WaitForSeconds(0);
        var results = new ItemListModel();
        results.title = titleTask;
        results.deadline = deadline;
        results.timeExecution = timeExecution;
        results.importance = importance;

        callback(results);
    }

    public class ItemListView
    {
        public Text title;
        public Text deadline;
        public Text timeExecution;
        public Text importance;
        public Image taskFixedIcon;

        public ItemListView(Transform rootView, bool fixedTask)
        {
            title = rootView.Find("title").GetComponent<Text>();
            deadline = rootView.Find("deadline").GetComponent<Text>();
            timeExecution = rootView.Find("executionTime").GetComponent<Text>();
            importance = rootView.Find("importance").GetComponent<Text>();
            taskFixedIcon = rootView.Find("Image").GetComponent<Image>();
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
    }
}