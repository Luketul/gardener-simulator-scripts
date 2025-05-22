using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MenuLevelSelect : MonoBehaviour
{
    public Button StartGameButton;
    public Button LoadGameButton;
    public Button OptionsButton;
    public Button CreditsButton;
    public Button QuitButton;
    public Button DeleteAllSavesButton;

    public GameObject NameInputPanel;
    public TMP_InputField NameInputField;
    public Button SubmitNameButton;
    public Button CancelNameButton;

    public GameObject SaveListPanel;
    public GameObject SaveButtonPrefab;
    public Button CancelSaveListButton;
    public Transform[] SaveSlots;

    public GameObject QuitConfirmPanel;
    public Button QuitConfirmButton;
    public Button QuitCancelButton;

    public GameObject OptionsPanel;
    public Button CancelOptionsButton;

    public GameObject CreditsPanel;
    public Button CancelCreditsButton;


    public GameObject ConfirmDeletePanel;
    public Button ConfirmDeleteSave;
    public Button CancelDeleteSave;

    private int saveToDelete;
    private GameObject saveObjectToDelete;



    private const int maxSaves = 4;

    private void Start()
    {
        StartGameButton.onClick.AddListener(ShowNameInputPanel);
        LoadGameButton.onClick.AddListener(ShowSaveListPanel);
        DeleteAllSavesButton.onClick.AddListener(DeleteAllSaves);
        QuitButton.onClick.AddListener(ShowQuitConfirmPanel);
        OptionsButton.onClick.AddListener(ShowOptionsPanel);
        CreditsButton.onClick.AddListener(ShowCreditsPanel);

        SubmitNameButton.onClick.AddListener(StartGameWithName);
        CancelNameButton.onClick.AddListener(CancelNewGame);
        CancelSaveListButton.onClick.AddListener(CancelLoadGame);

        QuitConfirmButton.onClick.AddListener(QuitGame);
        QuitCancelButton.onClick.AddListener(CancelQuit);
        CancelOptionsButton.onClick.AddListener(CancelOptions);
        CancelCreditsButton.onClick.AddListener(CancelCredits);




        NameInputPanel.SetActive(false);
        SaveListPanel.SetActive(false);
        QuitConfirmPanel.SetActive(false);
        OptionsPanel.SetActive(false);
        CreditsPanel.SetActive(false);


        ConfirmDeletePanel.SetActive(false);
        CancelDeleteSave.onClick.AddListener(CancelConfirmDeletePanel);
        ConfirmDeleteSave.onClick.AddListener(ConfirmDeleteSaveAction);
        CancelDeleteSave.onClick.AddListener(CancelConfirmDeletePanel);


    }

    private void ShowNameInputPanel()
    {
        int saveCount = PlayerPrefs.GetInt("SaveCount", 0);
        if (saveCount >= maxSaves)
        {
            Debug.Log("Osi¹gniêto maksymaln¹ liczbê zapisów!");
            return;
        }

        NameInputPanel.SetActive(true);
        HideMainButtons();
    }

    private void StartGameWithName()
    {
        string gardenerName = NameInputField.text.Trim();
        if (!string.IsNullOrEmpty(gardenerName))
        {
            int saveCount = PlayerPrefs.GetInt("SaveCount", 0);
            if (saveCount >= maxSaves)
            {
                Debug.Log("Nie mo¿na dodaæ wiêcej zapisów!");
                return;
            }

            PlayerPrefs.SetString("GardenerName_" + saveCount, gardenerName);
            PlayerPrefs.SetInt("SaveCount", saveCount + 1);
            PlayerPrefs.SetInt("CurrentProfile", saveCount);
            PlayerPrefs.Save();

            SceneManager.LoadScene("PlayerProfile");
        }
    }

    private void CancelNewGame()
    {
        NameInputPanel.SetActive(false);
        ShowMainButtons();
    }

    private void ShowSaveListPanel()
    {
        SaveListPanel.SetActive(true);
        HideMainButtons();
        RefreshSaveList();
    }

    private void CheckAndUpdateLoadButtonInteractability()
    {
        int saveCount = PlayerPrefs.GetInt("SaveCount", 0);
        LoadGameButton.interactable = saveCount > 0;
    }


    private void CreateSaveButton(int saveIndex, string name, Transform parentSlot)
    {
        GameObject newSave = Instantiate(SaveButtonPrefab, parentSlot);

        TMP_Text saveText = newSave.transform.Find("Text (TMP)").GetComponent<TMP_Text>();
        saveText.text = name;

        TMP_Text lastLoginText = newSave.transform.Find("LastLoginText").GetComponent<TMP_Text>();
        string lastLogin = PlayerPrefs.GetString("LastLogin_" + saveIndex, "Brak danych");
        lastLoginText.text = "Ostatnia gra: " + lastLogin;

        Button deleteButton = newSave.transform.Find("DeleteSaveBUTTON").GetComponent<Button>();
        deleteButton.onClick.AddListener(() => ShowDeleteConfirmPanel(saveIndex, newSave));

        Button saveButton = newSave.GetComponent<Button>();
        saveButton.onClick.AddListener(() => LoadProfile(saveIndex));
    }




    public void DeleteSave(int saveIndex, GameObject saveObject)
    {
        PlayerPrefs.DeleteKey("GardenerName_" + saveIndex);
        PlayerPrefs.DeleteKey("LastLogin_" + saveIndex); 

        int saveCount = PlayerPrefs.GetInt("SaveCount", 0);
        for (int i = saveIndex; i < saveCount - 1; i++)
        {
            string nextSave = PlayerPrefs.GetString("GardenerName_" + (i + 1), "Unknown");
            PlayerPrefs.SetString("GardenerName_" + i, nextSave);

            string nextLogin = PlayerPrefs.GetString("LastLogin_" + (i + 1), "Brak danych");
            PlayerPrefs.SetString("LastLogin_" + i, nextLogin);
        }

        PlayerPrefs.SetInt("SaveCount", saveCount - 1);
        PlayerPrefs.Save();

        Destroy(saveObject);

        CheckAndUpdateLoadButtonInteractability();
    }

    private void RefreshSaveList()
    {
        ClearSaveList();
        int saveCount = PlayerPrefs.GetInt("SaveCount", 0);

        CheckAndUpdateLoadButtonInteractability();

        for (int i = 0; i < saveCount && i < SaveSlots.Length; i++)
        {
            string name = PlayerPrefs.GetString("GardenerName_" + i, "Unknown");
            CreateSaveButton(i, name, SaveSlots[i]);
        }
    }



    private void LoadProfile(int saveIndex)
    {
        string currentDate = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm");
        PlayerPrefs.SetString("LastLogin_" + saveIndex, currentDate);
        PlayerPrefs.Save();

        PlayerPrefs.SetInt("CurrentProfile", saveIndex);
        PlayerPrefs.Save();
        SceneManager.LoadScene("PlayerProfile");
    }


    private void CancelLoadGame()
    {
        SaveListPanel.SetActive(false);
        ShowMainButtons();
    }

    private void DeleteAllSaves()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        ClearSaveList();
    }

    private void ClearSaveList()
    {
        foreach (Transform slot in SaveSlots)
        {
            if (slot.childCount > 0)
            {
                Destroy(slot.GetChild(0).gameObject);
            }
        }
    }

    private void ShowQuitConfirmPanel()
    {
        QuitConfirmPanel.SetActive(true);
        HideMainButtons();
    }

    private void QuitGame()
    {
        Application.Quit();
        Debug.Log("Gra zosta³a zamkniêta.");
    }

    private void CancelQuit()
    {
        QuitConfirmPanel.SetActive(false);
        ShowMainButtons();
    }

    private void ShowOptionsPanel()
    {
        OptionsPanel.SetActive(true);
        HideMainButtons();
    }

    private void CancelOptions()
    {
        OptionsPanel.SetActive(false);
        ShowMainButtons();
    }

    private void ShowCreditsPanel()
    {
        CreditsPanel.SetActive(true);
        HideMainButtons();
    }

    private void CancelCredits()
    {
        CreditsPanel.SetActive(false);
        ShowMainButtons();
    }

    private void ShowDeleteConfirmPanel(int saveIndex, GameObject saveObject)
    {
        saveToDelete = saveIndex;
        saveObjectToDelete = saveObject;

        ConfirmDeletePanel.SetActive(true);
    }

    private void ConfirmDeleteSaveAction()
    {
        DeleteSave(saveToDelete, saveObjectToDelete);
        ConfirmDeletePanel.SetActive(false);
    }

    private void CancelConfirmDeletePanel()
    {
        ConfirmDeletePanel.SetActive(false);
    }


    public void HideMainButtons()
    {
        StartGameButton.gameObject.SetActive(false);
        LoadGameButton.gameObject.SetActive(false);
        OptionsButton.gameObject.SetActive(false);
        CreditsButton.gameObject.SetActive(false);
        QuitButton.gameObject.SetActive(false);
    }

    public void ShowMainButtons()
    {
        StartGameButton.gameObject.SetActive(true);
        LoadGameButton.gameObject.SetActive(true);
        OptionsButton.gameObject.SetActive(true);
        CreditsButton.gameObject.SetActive(true);
        QuitButton.gameObject.SetActive(true);
    }
}
