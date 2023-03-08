using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BookLoader
{
    public static string SAVINGPATH = Application.persistentDataPath;
    public static string SUFFIX = ".molk";
    public static void LoadNewBook(string bookName)
    {
        char SLASH = '/';
#if UNITY_EDITOR
        SLASH = '\\';
#endif
        string[] bookParts = File.ReadAllText(SAVINGPATH + SLASH + bookName + SUFFIX).Split('~');
        //string bookName = bookParts[0];

        if (!Directory.Exists(SAVINGPATH + SLASH + bookName)) Directory.CreateDirectory(SAVINGPATH + SLASH + bookName);
        int pageCounter = 0;
        for (int i = 1; i < bookParts.Length - 4; i += 2)
        {
            File.WriteAllText(SAVINGPATH + SLASH + bookName + SLASH + "page-" + bookParts[i].Split('ˇ')[0] + ".random", bookParts[i]);
            File.WriteAllText(SAVINGPATH + SLASH + bookName + SLASH + "path-" + bookParts[i].Split('ˇ')[0] + ".random", bookParts[i + 1]);
            pageCounter++;
        }

        List<string> checkpoints = LoadCheckpoints(bookParts[bookParts.Length - 3].Split('|')[0], SAVINGPATH + SLASH + bookName + SLASH);
        List<string> endings = LoadEndings(bookParts[bookParts.Length - 3].Split('|')[1], SAVINGPATH + SLASH + bookName + SLASH);
        List<Skill> skills = LoadSkills(bookParts[bookParts.Length - 2], SAVINGPATH + SLASH + bookName + SLASH);
        List<Artefact> artefacts = LoadArtefacts(bookParts[bookParts.Length - 1], SAVINGPATH + SLASH + bookName + SLASH);

        File.WriteAllText(SAVINGPATH + SLASH + bookName + SLASH + "playerStats.random", bookParts[bookParts.Length - 2] + '#' + bookParts[bookParts.Length - 1]);

        //Player player = new Player(skills, artefacts);
    }
    private static List<string> LoadCheckpoints(string jsonCheckpoints, string savingPath)
    {
        List<string> checkpoints = new List<string>();
        File.WriteAllText(savingPath + "checkpoints.random", jsonCheckpoints);

        foreach (string checkpoint in jsonCheckpoints.Split('$'))
        {
            if (checkpoint.Length > 1) checkpoints.Add(checkpoint);
        }

        return checkpoints;
    }
    private static List<string> LoadEndings(string jsonEndings, string savingPath)
    {
        List<string> endings = new List<string>();
        File.WriteAllText(savingPath + "endings.random", jsonEndings);

        foreach (string ending in jsonEndings.Split('$'))
        {
            if (ending.Length > 1) endings.Add(ending);
        }

        return endings;
    }
    private static List<Skill> LoadSkills(string JsonSkills, string savingPath)
    {
        //if (!File.Exists(savingPath + "skills.random")) File.Create(savingPath + "skills.random");
        File.WriteAllText(savingPath + "skills.random", JsonSkills);
 
        List<Skill> skills = new List<Skill>();
        foreach(string skill in JsonSkills.Split('$'))
        {
            if (skill.Length < 3) continue;
            //Debug.Log(skill);
            Skill currentSkill = JsonUtility.FromJson<Skill>(skill);
            skills.Add(currentSkill);
        }
        //foreach(Skill das in skills)
        //{
        //    Debug.Log(das.name + " " + das.matematicalOpretion + " " + das.value + " " + das.ID + " " + das.groapName);
        //}
        return skills;
    }
    private static List<Artefact> LoadArtefacts(string JsonArtefacts, string savingPath)
    {
        //if (!File.Exists(savingPath + "artefacts.random")) File.Create(savingPath + "artefacts.random");
        File.WriteAllText(savingPath + "artefacts.random", JsonArtefacts);

        List<Artefact> artefacts = new List<Artefact>();
        foreach (string artefact in JsonArtefacts.Split('$'))
        {
            if (artefact.Length < 3) continue;
            //Debug.Log(artefact);
            Artefact currentArtefact = JsonUtility.FromJson<Artefact>(artefact);
            artefacts.Add(currentArtefact);
        }
        //foreach (Artefact das in artefacts)
        //{
        //    Debug.Log(das.name + " " + das.isActive + " " + das.ID + " " + das.category);
        //}
        return artefacts;
    }
}
