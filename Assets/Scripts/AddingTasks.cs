using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddingTasks : MonoBehaviour
{
    public GameObject prefabTask;
    public RectTransform contentListTask;

    public void AddingTask()
    {
        StartCoroutine(GetItems(results => OnRecevedItem(results)));
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
        results.title = PanelParametersTask.nameTaskText;
        results.deadline = "дедлайн";
        results.timeExecution = "время";

        callback(results);
    }

    public class ItemListView
    {
        public Text title;
        public Text deadline;
        public Text timeExecution;
        public Button Parameters;

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

