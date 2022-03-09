using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskParameters : MonoBehaviour
{
    public RectTransform prefabTask;
    public RectTransform prefabDataInputTask;
    public RectTransform content;
    private bool click = false;
    void Start()
    {

    }

    void Update()
    {
        
    }

    public void ChangUpdateParameters()
    {
        if (!click)
        {
            StartCoroutine(GetItems(results => OnRecevedItem(results)));
            click = true;
        }
        else
        {
            print("net");
            click = false;
        }

    }

    void OnRecevedItem(ItemListModel item)
    {
        var instance = GameObject.Instantiate(prefabDataInputTask.gameObject) as GameObject;
        instance.transform.SetParent(content, false);
        InitializeItemView(instance, item);
    }

    void InitializeItemView(GameObject viewGameObject, ItemListModel model)
    {
        ItemListView view = new ItemListView(viewGameObject.transform);
    }

    IEnumerator GetItems(System.Action<ItemListModel> callback)
    {
        yield return new WaitForSeconds(0);
        var results = new ItemListModel();

        callback(results);
    }

    public class ItemListView
    {

        public ItemListView(Transform rootView)
        {

        }
    }

    public class ItemListModel
    {
        public string title;
        public string deadline;
        public string timeExecution;
    }
}


