using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Artefact
{
    public string name;
    public string category;

    public bool isActive;

    public int ID;

    public Artefact(string name, bool isActive, int ID, string category = null)
    {
        this.name = name;
        this.category = category;
        this.isActive = isActive;
        this.ID = ID;
    }
}
