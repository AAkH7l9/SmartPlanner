using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollViewAdapter : MonoBehaviour
{
    public RectTransform prefab;
    public RectTransform content;

    public void UpdateItems()
    {
        StartCoroutine(GetItems( results => OnRecevedItem(results)));
    }

    void OnRecevedItem (ItemListModel item)
    {
            var instance = GameObject.Instantiate(prefab.gameObject) as GameObject;
            instance.transform.SetParent(content, false);
            InitializeItemView(instance, item);
    }

    void InitializeItemView(GameObject viewGameObject, ItemListModel model)
    {
        ItemListView view = new ItemListView(viewGameObject.transform);
        view.title.text = model.title;
        view.deadline.text = model.deadline;
        view.timeExecution.text = model.timeExecution;
    }

    IEnumerator GetItems( System.Action<ItemListModel> callback)
    {
        yield return new WaitForSeconds(0);
        var results = new ItemListModel();
        results.title = "Задача";
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

        public ItemListView (Transform rootView)
        {
            title = rootView.Find("title").GetComponent<Text>(); 
            deadline = rootView.Find("deadline").GetComponent<Text>();
            timeExecution = rootView.Find("executionTime").GetComponent<Text>();
            Parameters = rootView.Find("parameters").GetComponent<Button>();
        }
    }

    public class ItemListModel
    {
        public string title;
        public string deadline;
        public string timeExecution; 
    }
}
