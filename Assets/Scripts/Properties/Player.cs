using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public List<Skill> mySkills = new List<Skill>();
    public List<Artefact> myArtefacts = new List<Artefact>();

    public Player(List<Skill> mySkills, List<Artefact> myArtefacts)
    {
        this.mySkills = mySkills;
        this.myArtefacts = myArtefacts;
    }
}
