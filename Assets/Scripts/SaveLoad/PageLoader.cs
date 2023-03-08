using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
#if PLATFORM_ANDROID
using UnityEngine.Android;
#endif

public class PageLoader : MonoBehaviour
{
    private const char PATHSEPARATOR = '#';
    private const char PROPERTYESSEPARATOR = '|';
    private const char GROAPSEPARATOR = '@';
    public const char SEPARATOR = '$';

    public const string SKILLS = "skills";
    public const string ARTEFACTS = "artefacs";
    public const string PATHS = "paths-";
    private const string CONDITIONS = "Conditions";
    private const string EFFECTS = "Effects";

    private const string READINGFILE = "reading.chache";

    private char SLASH = '/';

    public GameObject playerManager;
    public GameObject storyChoises;
    public GameObject deadEndWarning;
    public GameObject theEndButtonPrefab;

    public GameObject storyChoisePrefab;

    public Text titleText;
    public Text storyText;

    List<string> pageNames = new List<string>();
    List<string> checkpoints = new List<string>();
    List<string> endings = new List<string>();

    string savingPath;
    string bookName;
    string pageName;
    string lastCheckpoint = "prologue";

    float scrollRectVericalPosition;

    private void Awake()
    {
#if UNITY_EDITOR
        SLASH = '\\';
#endif
        pageNames.Add("prologue");
        bookName = PlayerPrefs.GetString("bookName");
        savingPath = Application.persistentDataPath + SLASH + bookName + SLASH;
        LoadEndings();
        LoadCheckpoints();
        LoadPlayer();
        if (!PlayerPrefs.HasKey("continueReading"))
        {
            LoadPage(pageNames[0]);
        }
        else
        {
            PlayerPrefs.DeleteKey("continueReading");
            Debug.Log("ContinueReading " + PlayerPrefs.GetInt("continueReading"));
            LoadReadingCache();
            LoadPage(pageName);
            GameObject.Find("TextContainer").GetComponent<ScrollRect>().verticalNormalizedPosition = scrollRectVericalPosition;
        }
    }
    private void LateUpdate()
    {

        Canvas.ForceUpdateCanvases();
        GameObject.Find("Content").GetComponent<VerticalLayoutGroup>().enabled = false;
        GameObject.Find("Content").GetComponent<ContentSizeFitter>().enabled = false;
        GameObject.Find("Content").GetComponent<VerticalLayoutGroup>().enabled = true;
        GameObject.Find("Content").GetComponent<ContentSizeFitter>().enabled = true;
    }
    public void LoadPage(string pageName)
    {
        this.pageName = pageName;
        foreach (Transform child in storyChoises.transform)
        {
            Destroy(child.gameObject);
        }
        //loading page text
        string textToLoad = File.ReadAllText(savingPath + "page-" + pageName + ".random");
        string title = textToLoad.Split('ˇ')[1];
        string story = textToLoad.Split('ˇ')[2];
        titleText.text = title;
        storyText.text = story;

        if (checkpoints.Contains(pageName)) lastCheckpoint = pageName;

        //end of the story mechanic
        if (endings.Contains(pageName))
        {
            GameObject temp = GameObject.Instantiate(theEndButtonPrefab, storyChoises.transform);
            if (File.Exists(Application.persistentDataPath + SLASH + READINGFILE)) File.Delete(Application.persistentDataPath + SLASH + READINGFILE);
            temp.GetComponent<Button>().onClick.AddListener(delegate { BackToMenu(true); });
            return;
        }
        //loading paths
        string pathsToLoad = File.ReadAllText(savingPath + "path-" + pageName + ".random");
        string[] paths = pathsToLoad.Split(PATHSEPARATOR);
        foreach (string path in paths)
        {
            if (path.Length < 3) continue;
            GameObject currentPathObject = GameObject.Instantiate(storyChoisePrefab, storyChoises.transform);
            MyPathPrefab currentPath = JsonUtility.FromJson<MyPathPrefab>(path.Split(GROAPSEPARATOR)[0]);
            currentPathObject.GetComponent<Path>().SetupPath(currentPath);
            string conditions = path.Split(GROAPSEPARATOR)[1];
            string effects = path.Split(GROAPSEPARATOR)[2];
            bool isDeadEnd = false;
            if (path.Split(GROAPSEPARATOR).Length >= 4) isDeadEnd = bool.Parse(path.Split(GROAPSEPARATOR)[3]);

            storyChoises.transform.position -= new Vector3(0, 100, 0);

            LoadCondtions(currentPathObject, conditions);
            LoadEffects(currentPathObject, effects);

            currentPathObject.transform.Find("Text").GetComponent<Text>().text = currentPath.storyChoiseText;


            if (isDeadEnd)
            {
                currentPathObject.GetComponent<Button>().onClick.AddListener(delegate { SuddenDeath(); });
                continue;
            }

            currentPathObject.GetComponent<Button>().onClick.AddListener(delegate { LoadPage(currentPath.connectingFile.Split('-')[1]); });
            //checking for selected conditions for path
            if (CheckConditions(currentPathObject.GetComponent<Path>().conditions, playerManager.GetComponent<Player>()))
            {
                currentPathObject.SetActive(false);
                continue;
            }
            //setting up effects for current path

            foreach (Skill skill in currentPathObject.GetComponent<Path>().effects.skillActions)
            {
                foreach (Skill playerSkill in playerManager.GetComponent<Player>().mySkills)
                {
                    if (playerSkill.name == skill.name)
                    {
                        if (skill.matematicalOperation == 0) currentPathObject.GetComponent<Button>().onClick.AddListener(delegate { playerSkill.value += skill.value; });
                        else currentPathObject.GetComponent<Button>().onClick.AddListener(delegate { playerSkill.value -= skill.value; Debug.Log(playerSkill.value); });
                        break;
                    }
                }
            }
            foreach (Artefact artefacts in currentPathObject.GetComponent<Path>().effects.artefactActions)
            {
                foreach (Artefact playerArtefact in playerManager.GetComponent<Player>().myArtefacts)
                {
                    if (playerArtefact.name == artefacts.name)
                    {
                        currentPathObject.GetComponent<Button>().onClick.AddListener(delegate { playerArtefact.isActive = artefacts.isActive; });
                        break;
                    }
                }
            }
            Canvas.ForceUpdateCanvases();
            GameObject.Find("Content").GetComponent<VerticalLayoutGroup>().enabled = false;
            GameObject.Find("Content").GetComponent<ContentSizeFitter>().enabled = false;
            GameObject.Find("Content").GetComponent<VerticalLayoutGroup>().enabled = true;
            //GameObject.Find("Content").GetComponent<ContentSizeFitter>().enabled = true;
        }
    }
    public void SuddenDeath()
    {
        deadEndWarning.SetActive(true);

        deadEndWarning.transform.Find("ButtonLoadCheckpoint").GetComponent<Button>().onClick.AddListener(delegate { LoadLastCheckpoint(); });
        deadEndWarning.transform.Find("ButtonMenu").GetComponent<Button>().onClick.AddListener(delegate { BackToMenu(true); deadEndWarning.SetActive(false); SaveReadingCache(lastCheckpoint, 0f); });
    }
    public void LoadLastCheckpoint()
    {
        SaveReadingCache(lastCheckpoint, 0f);
        deadEndWarning.SetActive(false);
        LoadPage(lastCheckpoint);
    }
    public void LoadCondtions(GameObject path, string conditions)
    {
        if (conditions.Length < 1) return;
        string jsonSkills = conditions.Split(PROPERTYESSEPARATOR)[0];
        string jsonArtefacts = conditions.Split(PROPERTYESSEPARATOR)[1];
        LoadSkillActions(path, jsonSkills);
        LoadArtefactActions(path, jsonArtefacts);
    }
    public void LoadEffects(GameObject path, string conditions)
    {
        if (conditions.Length < 1) return;
        string jsonSkills = conditions.Split(PROPERTYESSEPARATOR)[0];
        string jsonArtefacts = conditions.Split(PROPERTYESSEPARATOR)[1];
        LoadSkillActions(path, jsonSkills);
        LoadArtefactActions(path, jsonArtefacts);
    }
    public void LoadSkillActions(GameObject path, string skills)
    {
        string[] jsonSkills = skills.Split(SEPARATOR);
        // goning thrue all json save files
        foreach (string json in jsonSkills)
        {
            if (json.Length < 3) continue;// if file is too short to be json file continue in cyclus
            Skill skill = JsonUtility.FromJson<Skill>(json);
            if (skill.groapName == CONDITIONS)
            {
                path.GetComponent<Path>().conditions.skillActions.Add(skill);
            }
            else if (skill.groapName == EFFECTS)
            {
                //Debug.Log(json);
                path.GetComponent<Path>().effects.skillActions.Add(skill);
                //Debug.Log(skill.matematicalOperation);
            }
        }
    }
    public void LoadArtefactActions(GameObject path, string artefacts)
    {
        string[] jsonArtefacts = artefacts.Split(SEPARATOR);
        // goning thrue all json save files
        foreach (string json in jsonArtefacts)
        {
            if (json.Length < 3) continue;// if file is too short to be json file continue in cyclus
            Artefact artefact = JsonUtility.FromJson<Artefact>(json);
            //Debug.Log(json);
            if (artefact.category == CONDITIONS)
            {
                path.GetComponent<Path>().conditions.artefactActions.Add(artefact);
            }
            else if (artefact.category == EFFECTS)
            {
                path.GetComponent<Path>().effects.artefactActions.Add(artefact);
            }
        }
    }
    public void LoadPlayer()
    {
        string playerStats = File.ReadAllText(savingPath + "playerStats.random");
        string playerSkills = playerStats.Split('#')[0];
        string playerArtefacts = playerStats.Split('#')[1];

        Player currPlayer = playerManager.GetComponent<Player>();
        currPlayer.mySkills = new List<Skill>();
        currPlayer.myArtefacts = new List<Artefact>();

        foreach (string skill in playerSkills.Split(SEPARATOR))
        {
            if (skill.Length < 3) continue;
            playerManager.GetComponent<Player>().mySkills.Add(JsonUtility.FromJson<Skill>(skill));
        }
        foreach (string artefact in playerArtefacts.Split(SEPARATOR))
        {
            if (artefact.Length < 3) continue;
            currPlayer.myArtefacts.Add(JsonUtility.FromJson<Artefact>(artefact));
        }
    }
    public bool CheckConditions(Conditions conditions, Player player)
    {
        foreach (Skill skill in conditions.skillActions)
        {
            foreach (Skill playerSkill in player.mySkills)
            {
                if (playerSkill.name == skill.name)
                {
                    if (playerSkill.value < skill.value) return true;
                    break;
                }
            }

        }
        foreach (Artefact artefacts in conditions.artefactActions)
        {
            foreach (Artefact playerArtefact in player.myArtefacts)
            {
                if (playerArtefact.name == artefacts.name)
                {
                    if (playerArtefact.isActive != artefacts.isActive) return true;
                    break;
                }
            }
        }
        return false;
    }
    public void BackToMenu(bool isDeadEnding = false)
    {
        if (!isDeadEnding) SaveReadingCache();
        SceneManager.LoadScene("MainMenu");
    }
    public void SaveReadingCache(string checkpoint = null, float readingProgress = -1f)
    {
        string savedPage = pageName;
        if (checkpoint != null) savedPage = checkpoint;
        if (readingProgress == -1) readingProgress = GameObject.Find("TextContainer").GetComponent<ScrollRect>().verticalNormalizedPosition;
        string textToSave = bookName + SEPARATOR + savedPage + SEPARATOR + readingProgress;
        File.WriteAllText(Application.persistentDataPath + SLASH + READINGFILE, textToSave);
    }
    public void LoadReadingCache()
    {
        string[] loadedCache = File.ReadAllText(Application.persistentDataPath + SLASH + READINGFILE).Split(SEPARATOR);
        bookName = loadedCache[0]; // bookName
        pageName = loadedCache[1]; // pageName
        scrollRectVericalPosition = float.Parse(loadedCache[2]); // scrollRectVericalPosition
    }
    public void LoadCheckpoints()
    {
        if (!File.Exists(BookLoader.SAVINGPATH + SLASH + bookName + SLASH + "checkpoints.random")) return;
        string checkpointsToLoad = File.ReadAllText(BookLoader.SAVINGPATH + SLASH + bookName + SLASH + "checkpoints.random");
        foreach (string checkpoint in checkpointsToLoad.Split('$'))
        {
            checkpoints.Add(checkpoint);
        }
    }
    public void LoadEndings()
    {
        if (!File.Exists(BookLoader.SAVINGPATH + SLASH + bookName + SLASH + "endings.random")) return;
        string endingsToLoad = File.ReadAllText(BookLoader.SAVINGPATH + SLASH + bookName + SLASH + "endings.random");
        foreach (string ending in endingsToLoad.Split('$'))
        {
            endings.Add(ending);
        }
    }
}