
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaskSlides : MonoBehaviour
{
    [SerializeField]  private RectTransform content;
    bool taskFixed = false;
    public void slideControler()
    {
        if (content.position.x > 0.9 || (taskFixed && content.position.x > 0.5))
        {
            content.position = new Vector3(1, content.position.y);
            taskFixed = true;
        }
        if (content.position.x < -1.2 || (taskFixed && content.position.x < -1) )
        {
            content.position = new Vector3(-2, content.position.y);
            taskFixed = true;
        }
        
        if(content.position.x < 0.3 && content.position.x > -0.5)
        {
            taskFixed = false;
        }
    }
}

