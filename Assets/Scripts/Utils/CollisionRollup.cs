using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollisionRollup : MonoBehaviour
{
    public int scrollSpeed = 1;

    private bool collisionFlag = false;
    private float scrollUp = 0;
    private float scrollDown = 1;

    private void Update()
    {
        if (scrollUp > 0)
        {
            scrollUp -= 0.001f * scrollSpeed;
            transform.parent.gameObject.GetComponent<ScrollRect>().verticalNormalizedPosition = scrollUp;
        }// scroll upwards
        if (scrollDown < 1)
        {
            scrollUp += 0.001f * scrollSpeed;
            transform.parent.gameObject.GetComponent<ScrollRect>().verticalNormalizedPosition = scrollUp;
        }// scroll downwards
    }


    private void OnTriggerStay(Collider other)
    {
        if (!Input.GetMouseButton(0) && !collisionFlag)
        {
            //Debug.Log("TriggeredStay");

            //Debug.Log(transform.parent.gameObject.GetComponent<ScrollRect>().verticalNormalizedPosition);
            //transform.parent.gameObject.GetComponent<ScrollRect>().verticalNormalizedPosition = 0;
            scrollUp = transform.parent.gameObject.GetComponent<ScrollRect>().verticalNormalizedPosition;

            collisionFlag = true;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        //if (Input.GetMouseButtonDown(0))
        //Debug.Log("TriggeredEnter");
    }
    private void OnTriggerExit(Collider other)
    {
        //if (Input.GetMouseButtonDown(0))
        Debug.Log("TriggeredExit");

        collisionFlag = false;
    }
    private void ScrollToTop()
    {
        scrollUp = transform.parent.gameObject.GetComponent<ScrollRect>().verticalNormalizedPosition;
    }
    private void ScrollToBottom()
    {
        scrollDown = transform.parent.gameObject.GetComponent<ScrollRect>().verticalNormalizedPosition;
    }
}
