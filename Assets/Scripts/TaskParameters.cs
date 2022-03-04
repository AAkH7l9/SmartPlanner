using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskParameters : MonoBehaviour
{
    public RectTransform prefab;
    void Start()
    {

    }

    void Update()
    {
        
    }

    public void ChangUpdateParameters()
    {
        print(prefab);
        prefab.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
    }
}
