using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyTaskView : MonoBehaviour
{
    [SerializeField] private GameObject task;
    public void DestroyTask()
    {
        Destroy(task);
    }
}
