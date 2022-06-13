using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskSlides : MonoBehaviour
{
    [SerializeField]  private RectTransform content;
    public void slideControler()
    {
        if (content.position.x > 0.65)
        {
            OneButtonLeft();
        }
        if (content.position.x < -1.3)
        {
            TwoButtonRight();
        }
    }
    private void OneButtonLeft()
    {
        content.position = new Vector3 (1, content.position.y);
    }

    public void TwoButtonRight()
    {
        content.position = new Vector3(-2, content.position.y);
    }
}
