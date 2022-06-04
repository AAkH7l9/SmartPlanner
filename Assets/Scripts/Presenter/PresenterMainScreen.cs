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

    /*public Text title;
    public Text deadline;
    public Text timeExecution;*/
    public GameObject contentDatainput;
    private bool click = false;

    public void PaneOpeningRegulation()
    {
        if (!click)
        {
            contentDatainput.SetActive(true);
            click = true;
        }
        else
        {
            contentDatainput.SetActive(false);
            click = false;
        }
    }

    [SerializeField] private InputField nameTask;
    public GameObject prefabTask;
    public RectTransform contentListTask;
    private string titleTask = "";
    private string deadline = "";
    private string timeExecution = "";


    public void AddTask()
    {
        titleTask = nameTask.text.ToString();
        deadline = "";
        timeExecution = "";
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
        ItemListView view = new ItemListView(viewGameObject.transform);
        view.title.text = model.title;
        view.deadline.text = model.deadline;
        view.timeExecution.text = model.timeExecution;
    }

    IEnumerator GetItems(System.Action<ItemListModel> callback)
    {
        yield return new WaitForSeconds(0);
        var results = new ItemListModel();
        results.title = titleTask;
        results.deadline = deadline;
        results.timeExecution = timeExecution;

        callback(results);
    }

    public class ItemListView
    {
        public Text title;
        public Text deadline;
        public Text timeExecution;

        public ItemListView(Transform rootView)
        {
            title = rootView.Find("title").GetComponent<Text>();
            deadline = rootView.Find("deadline").GetComponent<Text>();
            timeExecution = rootView.Find("executionTime").GetComponent<Text>();
        }
    }

    public class ItemListModel
    {
        public string title;
        public string deadline;
        public string timeExecution;
    }
}
