using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path : MonoBehaviour
{
    public int ID;
    public string connectingFile;
    public string storyChoiseText;
    public Conditions conditions = new Conditions();
    public Effects effects = new Effects();

    public void SetupPath(MyPathPrefab prefab)
    {
        ID = prefab.ID;
        connectingFile = prefab.connectingFile;
        storyChoiseText = prefab.storyChoiseText;
    }
    //public void DestroyMe(List<GameObject> pathsList)
    //{
    //    pathsList.Remove(gameObject);
    //    conditions.skillActions.Clear();
    //    conditions.artefactActions.Clear();
    //    effects.skillActions.Clear();
    //    effects.artefactActions.Clear();
    //    Destroy(gameObject);
    //    GameObject.Find("PageManager").GetComponent<CurrentPageManager>().UpdateObjectList(pathsList);
    //}
    public void SetName(string name)
    {
        this.name = name;
    }
    public void SetID(int ID)
    {
        this.ID = ID;
    }
    public string GetTypeName()
    {
        return "Path";
    }
}
public class Conditions
{
    public List<Skill> skillActions = new List<Skill>();
    public List<Artefact> artefactActions = new List<Artefact>();
}
public class Effects
{
    public List<Skill> skillActions = new List<Skill>();
    public List<Artefact> artefactActions = new List<Artefact>();
}
public class MyPathPrefab
{
    public int ID;
    public string connectingFile;
    public string storyChoiseText;
}
