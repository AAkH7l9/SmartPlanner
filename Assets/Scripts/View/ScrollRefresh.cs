using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollRefresh : MonoBehaviour
{
    [SerializeField] private RectTransform content;
    public void Refresh()
    {
        if(content.position.y < 3)
        {
            Debug.Log("Обновить");
            content.position = new Vector3(content.position.x, 4f);
        }
    }
}
