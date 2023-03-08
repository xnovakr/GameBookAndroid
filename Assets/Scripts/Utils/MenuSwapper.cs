using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSwapper : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject favouriteBooks;
    public GameObject library;
    public GameObject shop;
    public GameObject settings;
    public GameObject credits;

    private GameObject menuToMove;

    public float transitionTime = 1;
    public float screenWidth;

    public int movementSpeed = 10;
    private int movementDirection = 0;

    private void Awake()
    {
        screenWidth = Screen.width;
    }

    private void Update()
    {
        //if (movementSpeed < 0 && menuToMove)
        //{
        //    if (mainMenu.transform.position.x > -500) mainMenu.transform.position += new Vector3(movementSpeed * movementDirection, 0, 0);
        //    if (menuToMove.transform.position.x <= 500) menuToMove.transform.position += new Vector3(movementSpeed * movementDirection, 0, 0);
        //}//going to main menu

        //if (movementSpeed > 0 && menuToMove)
        //{
        //    if (mainMenu.transform.position.x < 0) mainMenu.transform.position += new Vector3(movementSpeed * movementDirection, 0, 0);
        //    if (menuToMove.transform.position.x >= 0) menuToMove.transform.position += new Vector3(movementSpeed * movementDirection, 0, 0);
        //}//going to sub menu
    }

    public void BackToMenu(GameObject activeScene)
    {
        if (!activeScene)
        {
            Debug.Log("Curret scene wasnt found.");
            return;
        }
        menuToMove = activeScene;
        movementDirection = 1;
        StopAllCoroutines();
        StartCoroutine(ChangeMenuAnimation(activeScene.GetComponent<RectTransform>(), screenWidth*2));
        StartCoroutine(ChangeMenuAnimation(mainMenu.GetComponent<RectTransform>(), 0));
    }
    public void SetSubMenu(GameObject sceneToActivate)
    {
        if (!sceneToActivate)
        {
            Debug.Log("Scene to activate wasnt found.");
            return;
        }
        menuToMove = sceneToActivate;
        movementDirection = -1;
        StopAllCoroutines();
        StartCoroutine(ChangeMenuAnimation(sceneToActivate.GetComponent<RectTransform>(), 0));
        StartCoroutine(ChangeMenuAnimation(mainMenu.GetComponent<RectTransform>(), -screenWidth*2));
    }
    private IEnumerator ChangeMenuAnimation(RectTransform menuContainer, float posX)
    {
        Vector3 newPos = new Vector3(posX, 0, 0);
        float elapsed = 0f;
        Vector3 oldPos = menuContainer.anchoredPosition3D;

        while (elapsed <= transitionTime)
        {
            elapsed += Time.deltaTime;
            Vector3 currentPos = Vector3.Lerp(oldPos, newPos, elapsed / transitionTime);
            menuContainer.anchoredPosition3D = currentPos;
            yield return null;
        }
    }
    public void SwapSideMenu()
    {
        movementDirection = -1;
        StopAllCoroutines();
        float posToGo;
        if (this.GetComponent<RectTransform>().anchoredPosition.x > 0) posToGo = -this.GetComponent<RectTransform>().sizeDelta.x / 2;
        else posToGo = this.GetComponent<RectTransform>().sizeDelta.x / 2;
        StartCoroutine(ChangeMenuAnimation(this.GetComponent<RectTransform>(), posToGo));
    }
}
