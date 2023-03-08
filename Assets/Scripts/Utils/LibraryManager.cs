using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LibraryManager : MonoBehaviour
{
    private const string FAVOURITEBOOKSFILE = "favouriteBooks.chache";
    private const string READINGFILE = "reading.chache";
    private const string BOOKSUFFIX = "molk";
    private const char SEPARATOR = '$';

    public GameObject bookPrefab;
    public GameObject library;
    public GameObject libraryOfFavourite;
    public GameObject chechk;

    public List<string> bookFolders;
    public List<string> bookFiles;
    public List<string> favouriteBooks;

    public string savingPath; //app persistent path
    private char SLASH = '/';

    private string lastRedBook;
    private string lastRedPage;

    private void Awake()
    {
#if UNITY_EDITOR
        SLASH = '\\';
#endif
        if (!library) library = GameObject.Find("Library");
        savingPath = Application.persistentDataPath;
        if (File.Exists(savingPath + SLASH + FAVOURITEBOOKSFILE)) LoadFavouriteBooks();
        if (File.Exists(savingPath + SLASH + READINGFILE))
        {
            LoadReadingCache();
            GameObject.Find("ButtonContinueReading").GetComponent<Button>().interactable = true;
        }
        else
        {
            GameObject.Find("ButtonContinueReading").GetComponent<Button>().interactable = false;
        }
        LoadBooks();
        //File.Create(savingPath + SLASH + "Saving here.txt");

    }
    public void LoadBookFolders()
    {
        string[] folders = Directory.GetDirectories(savingPath);
        foreach (string folder in folders)
        {
            string folderName = folder.Split(SLASH)[folder.Split(SLASH).Length - 1];
            bookFolders.Add(folderName);
        }
    }
    public void LoadBookFiles()
    {
        string[] files = Directory.GetFiles(savingPath);
        foreach (string file in files)
        {
            string fileName = file.Split(SLASH)[file.Split(SLASH).Length - 1];
            if (fileName.Split('.')[1] != BOOKSUFFIX) continue;
            bookFiles.Add(fileName);
        }
        bookPrefab.transform.parent = library.transform.Find("BooksMask").transform.Find("BooksContent").transform;
    }
    public void LoadBooks()
    {
        LoadBookFiles();
        LoadBookFolders();
        //bookFiles.Add("");
        foreach (string file in bookFiles)
        {
            CreateBook(file, library.transform.Find("BooksMask").transform.Find("BooksContent").transform);
        }

        //chechk.GetComponent<Text>().text = library.transform.Find("BooksMask").transform.Find("BooksContent").transform.childCount.ToString();
    }
    public void CreateBook(string bookName, Transform parent, bool copyOfBook = false)
    {
        //if (bookName.Split('.')[1] != BOOKSUFFIX) return;
        GameObject tempBook = GameObject.Instantiate(bookPrefab, parent);
        //tempBook.transform.position = Vector3.zero;
        //tempBook.transform.localScale = Vector3.one;
        tempBook.SetActive(true);

        GameObject book = tempBook.transform.Find("Book").gameObject;

        Text bookNameField = book.transform.Find("BookName").GetComponent<Text>();
        bookName = bookName.Split('.')[0];
        bookNameField.text = bookName;
        tempBook.transform.name = bookName;

        Button buttonAddToFav = tempBook.transform.Find("ButtonAddToFav").GetComponent<Button>();
        Button buttonDownload = tempBook.transform.Find("ButtonDownload").GetComponent<Button>();// not implemented yet
        Button buttonExtract = tempBook.transform.Find("ButtonExtract").GetComponent<Button>();
        Button buttonUpdate = tempBook.transform.Find("ButtonUpdate").GetComponent<Button>(); // not implemented yet
        Button buttonDelete = tempBook.transform.Find("ButtonDelete").GetComponent<Button>();
        Button buttonRead = tempBook.transform.Find("ButtonRead").GetComponent<Button>();

        buttonAddToFav.onClick.AddListener(delegate { AddBookToFav(bookName, buttonAddToFav); });
        buttonDownload.onClick.AddListener(delegate { });
        buttonExtract.onClick.AddListener(delegate { ExtractBook(bookName); buttonExtract.gameObject.SetActive(false); buttonRead.gameObject.SetActive(true); buttonDelete.gameObject.SetActive(true); });
        buttonUpdate.onClick.AddListener(delegate { });
        buttonDelete.onClick.AddListener(delegate { DeleteBook(bookName); buttonDelete.gameObject.SetActive(false); buttonExtract.gameObject.SetActive(true); });
        buttonRead.onClick.AddListener(delegate { ReadBook(bookName); });
        buttonDownload.gameObject.SetActive(false);
        buttonUpdate.gameObject.SetActive(false);

        if (bookFolders.Contains(bookName))
        {
            buttonExtract.gameObject.SetActive(false);
        }//if there is folder with extracted book
        else
        {
            buttonRead.gameObject.SetActive(false);
        }//if there is no folder with extracted book
        if (favouriteBooks.Contains(bookName))
        {
            buttonAddToFav.onClick.RemoveAllListeners();
            buttonAddToFav.onClick.AddListener(delegate { RemoveFromFav(bookName, buttonAddToFav); });
            buttonAddToFav.transform.Find("NotFav").gameObject.SetActive(false);
            buttonAddToFav.transform.Find("Fav").gameObject.SetActive(true);
            if (!copyOfBook) CreateBook(bookName, libraryOfFavourite.transform.Find("BooksMask").Find("BooksContent").transform, true); //crating copy for favourites if it isnt already created
        }
    }
    public void ReadBook(string bookName)
    {
        if (!Directory.Exists(savingPath + SLASH + bookName)) return;
        PlayerPrefs.SetString("bookName", bookName);

        SceneManager.LoadScene("Game");
    }
    public void ContinueReading()
    {
        PlayerPrefs.SetInt("continueReading", 1);

        SceneManager.LoadScene("Game");
    }
    public void AddBookToFav(string bookName, Button favouriteAdd)
    {
        favouriteBooks.Add(bookName);
        SaveFavouriteBooks();
        CreateBook(bookName, libraryOfFavourite.transform.Find("BooksMask").Find("BooksContent").transform, true);
        favouriteAdd.onClick.RemoveAllListeners();
        favouriteAdd.onClick.AddListener(delegate { RemoveFromFav(bookName, favouriteAdd); });
        favouriteAdd.transform.Find("NotFav").gameObject.SetActive(false);
        favouriteAdd.transform.Find("Fav").gameObject.SetActive(true);
    }
    public void RemoveFromFav(string bookName, Button favouriteAdd)
    {
        favouriteBooks.Remove(bookName);
        SaveFavouriteBooks();

        if (libraryOfFavourite.transform.Find("BooksMask").Find("BooksContent").transform.Find(bookName).gameObject)
        {
            Destroy(libraryOfFavourite.transform.Find("BooksMask").Find("BooksContent").transform.Find(bookName).gameObject);
        }

        favouriteAdd.onClick.RemoveAllListeners();
        favouriteAdd.onClick.AddListener(delegate { AddBookToFav(bookName, favouriteAdd); });
        favouriteAdd.transform.Find("NotFav").gameObject.SetActive(true);
        favouriteAdd.transform.Find("Fav").gameObject.SetActive(false);
    }
    public void DeleteBook(string bookName)
    {
        if (!Directory.Exists(savingPath + SLASH + bookName)) return;
        foreach (string file in Directory.GetFiles(savingPath + SLASH + bookName)) File.Delete(file);
        Directory.Delete(savingPath + SLASH + bookName);
        UpdateBook(bookName);
    }
    public void UpdateBook(string bookName)
    {
        GameObject tempBook = GameObject.Find(bookName);

        Button buttonAddToFav = tempBook.transform.Find("ButtonAddToFav").GetComponent<Button>();
        Button buttonDownload = tempBook.transform.Find("ButtonDownload").GetComponent<Button>();// not implemented yet
        Button buttonExtract = tempBook.transform.Find("ButtonExtract").GetComponent<Button>();
        Button buttonUpdate = tempBook.transform.Find("ButtonUpdate").GetComponent<Button>(); // not implemented yet
        Button buttonDelete = tempBook.transform.Find("ButtonDelete").GetComponent<Button>();
        Button buttonRead = tempBook.transform.Find("ButtonRead").GetComponent<Button>();

        buttonAddToFav.onClick.AddListener(delegate { AddBookToFav(bookName, buttonAddToFav); });
        buttonDownload.onClick.AddListener(delegate { });
        buttonExtract.onClick.AddListener(delegate { BookLoader.LoadNewBook(bookName); });
        buttonUpdate.onClick.AddListener(delegate { });
        buttonDelete.onClick.AddListener(delegate { DeleteBook(bookName); });
        buttonRead.onClick.AddListener(delegate { });
        buttonDownload.gameObject.SetActive(false);
        buttonUpdate.gameObject.SetActive(false);

        if (bookFolders.Contains(bookName))
        {
            buttonExtract.gameObject.SetActive(false);
        }//if there is folder with extracted book
        else
        {
            buttonRead.gameObject.SetActive(false);
        }//if there is no folder with extracted book

    }
    public void ExtractBook(string bookName)
    {
        BookLoader.LoadNewBook(bookName);
    }
    public void LoadFavouriteBooks()
    {
        favouriteBooks.Clear();
        string localSavingPath = savingPath + SLASH + FAVOURITEBOOKSFILE;
        string loadedText = File.ReadAllText(localSavingPath);
        string[] loadedFavouriteBooks = loadedText.Split(SEPARATOR);
        foreach(string loadedBook in loadedFavouriteBooks)
        {
            if (loadedBook.Length < 1) continue;
            favouriteBooks.Add(loadedBook);
        }
    }
    public void SaveFavouriteBooks()
    {
        string textToSave = "";
        string localSavingPath = savingPath + SLASH + FAVOURITEBOOKSFILE;
        foreach(string book in favouriteBooks)
        {
            if (book.Length < 1) continue;
            if (textToSave.Length > 1) textToSave += SEPARATOR;
            textToSave += book;
        }
        File.WriteAllText(localSavingPath, textToSave);
    }
    public void LoadReadingCache()
    {
        string loadedText = File.ReadAllText(savingPath + SLASH + READINGFILE);
        string[] loadedCache = loadedText.Split(SEPARATOR);
        lastRedBook = loadedCache[0];
        lastRedPage = loadedCache[1];
        PlayerPrefs.SetString("lastRedBook", lastRedBook);
        PlayerPrefs.SetString("lastRedPage", lastRedPage);
    }
    public void QuiteGame()
    {
        Application.Quit();
    }
}
