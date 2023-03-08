using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill
{
    public string name;
    public string groapName;

    public int value;
    public int ID;
    public int matematicalOperation;

    public Skill(string name, int value, int ID, string groapName = null, int matematicalOpretion = 0)
    {
        this.name = name;
        this.groapName = groapName;
        this.value = value;
        this.ID = ID;
        this.matematicalOperation = matematicalOpretion;
    }
}
