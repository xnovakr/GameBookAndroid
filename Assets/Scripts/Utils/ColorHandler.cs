using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorHandler : MonoBehaviour
{
    public FlexibleColorPicker fcp;
    private Material material;

    public Color textColor;
    private Color backgroundColor;
    private Color currentColor;

    public GameObject test;
    public GameObject colorSwitch;
    public GameObject sizeSettings;
    public GameObject currentManager;
    private Cache cacheObject = new Cache();

    int fontSize = 0;

    bool background;
    bool text;

    private void Start()
    {
        background = colorSwitch.transform.Find("ToggleBackground").GetComponent<Toggle>().isOn;
        text = colorSwitch.transform.Find("ToggleText").GetComponent<Toggle>().isOn;
        if (Cache.CheckCacheFile())
        {
            Debug.Log("loading Cache");
            cacheObject = cacheObject.LoadCache();
            backgroundColor = cacheObject.backgroundColor;
            textColor = cacheObject.fontColor;
            fontSize = cacheObject.fontSize;


            test.transform.Find("TextExample").GetComponent<Text>().fontSize = fontSize;
            test.transform.Find("TextExample").GetComponent<Text>().color = textColor;
            test.transform.Find("BackgroundExample").GetComponent<Image>().color = backgroundColor;
        }
        else
        {
            backgroundColor = test.transform.Find("BackgroundExample").GetComponent<Image>().color;
            textColor = test.transform.Find("TextExample").GetComponent<Text>().color;
            fontSize = test.transform.Find("TextExample").GetComponent<Text>().fontSize;

            cacheObject.backgroundColor = backgroundColor;
            cacheObject.fontColor = textColor;
            cacheObject.fontSize = fontSize;
        }
        if (background) currentColor = backgroundColor;
        else if (text) currentColor = textColor;
        fcp.color = currentColor;

        sizeSettings.transform.Find("TextSize").GetComponent<Text>().text = fontSize.ToString();

        colorSwitch.transform.Find("ToggleBackground").GetComponent<Toggle>().onValueChanged.AddListener(delegate { SwapSwitch(); });
        colorSwitch.transform.Find("ToggleText").GetComponent<Toggle>().onValueChanged.AddListener(delegate { SwapSwitch(); });
    }
    public void SwapSwitch()
    {
        colorSwitch.transform.Find("ToggleBackground").GetComponent<Toggle>().onValueChanged.RemoveAllListeners();
        colorSwitch.transform.Find("ToggleText").GetComponent<Toggle>().onValueChanged.RemoveAllListeners();
        
        if(background == colorSwitch.transform.Find("ToggleBackground").GetComponent<Toggle>().isOn)
        colorSwitch.transform.Find("ToggleBackground").GetComponent<Toggle>().isOn = !colorSwitch.transform.Find("ToggleBackground").GetComponent<Toggle>().isOn;
        if (text == colorSwitch.transform.Find("ToggleText").GetComponent<Toggle>().isOn)
            colorSwitch.transform.Find("ToggleText").GetComponent<Toggle>().isOn = !colorSwitch.transform.Find("ToggleText").GetComponent<Toggle>().isOn;

        background = colorSwitch.transform.Find("ToggleBackground").GetComponent<Toggle>().isOn;
        text = colorSwitch.transform.Find("ToggleText").GetComponent<Toggle>().isOn;

        if (background) currentColor = backgroundColor;
        if (text) currentColor = textColor;
        fcp.color = currentColor;

        colorSwitch.transform.Find("ToggleBackground").GetComponent<Toggle>().onValueChanged.AddListener(delegate { SwapSwitch(); });
        colorSwitch.transform.Find("ToggleText").GetComponent<Toggle>().onValueChanged.AddListener(delegate { SwapSwitch(); });
    }
    public void ChangeFontSize(int value)
    {
        fontSize += value;
        test.transform.Find("TextExample").GetComponent<Text>().fontSize = fontSize;
        sizeSettings.transform.Find("TextSize").GetComponent<Text>().text = fontSize.ToString();
        cacheObject.fontSize = fontSize;
        cacheObject.SaveCache(cacheObject);
    }
    private void Update()
    {
        //apply color of this script to the FCP whenever it is changed by the user
        //if (internalColor != externalColor)
        //{
        //    fcp.color = externalColor;
        //    internalColor = externalColor;
        //    Debug.Log(externalColor);
        //}

        //font resizing test


        /*
        if (fontSize != test.transform.Find("SizeOfTextSetting").Find("TextExample").GetComponent<Text>().fontSize)
        {
            fontSize = test.transform.Find("SizeOfTextSetting").Find("TextExample").GetComponent<Text>().fontSize;
            test.transform.Find("SizeOfTextSetting").Find("TextSize").GetComponent<Text>().text = fontSize.ToString();
            cacheObject.fontSize = fontSize;
            cacheObject.SaveCache(cacheObject);
        }
        if (currentColor != fcp.color)
        {
            currentColor = fcp.color;
            if (background)
            {
                backgroundColor = currentColor;
                cacheObject.backgroundColor = backgroundColor;
                test.transform.Find("SizeOfTextSetting").Find("Background").GetComponent<Image>().color = fcp.color;
                test.transform.Find("ColorSelectorSwitch").Find("Background").GetComponent<Image>().color = fcp.color;
            }
            if (text)
            {
                cacheObject.fontColor = textColor;
                textColor = currentColor;
                test.transform.Find("SizeOfTextSetting").Find("Text").GetComponent<Text>().color = fcp.color;
                test.transform.Find("SizeOfTextSetting").Find("TextSize").GetComponent<Text>().color = fcp.color;
                test.transform.Find("SizeOfTextSetting").Find("TextExample").GetComponent<Text>().color = fcp.color;

                test.transform.Find("ColorSelectorSwitch").Find("ToggleBackground").Find("TextToggleBackground").GetComponent<Text>().color = fcp.color;
                test.transform.Find("ColorSelectorSwitch").Find("ToggleText").Find("TextToggleText").GetComponent<Text>().color = fcp.color;
            }
            cacheObject.SaveCache(cacheObject);
        }
        */

        if (currentColor != fcp.color)
        {
            currentColor = fcp.color;
            if (background)
            {
                backgroundColor = currentColor;
                cacheObject.backgroundColor = backgroundColor;
                test.transform.Find("BackgroundExample").GetComponent<Image>().color = fcp.color;
            }
            if (text)
            {
                cacheObject.fontColor = textColor;
                textColor = currentColor;
                test.transform.Find("TextExample").GetComponent<Text>().color = fcp.color;
            }
            cacheObject.SaveCache(cacheObject);
        }



        //extract color from the FCP and apply it to the object material
        //material.color = fcp.color;
    }
}
