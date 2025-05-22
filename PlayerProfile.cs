using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;

public class PlayerProfile : MonoBehaviour
{
    public Button Level1Button;
    public Button Level2Button;
    public Button BackToMenuButton;
    public TMP_Text GardenerNameText;
    public TMP_Text CoinsText;

    public GameObject IntroductionPanel;
    public Button ContinueButton;

    public GameObject ShopPanel;
    public Button OpenShopButton;
    public Button CloseShopButton;

    public GameObject JobsPanel;
    public Button OpenJobsButton;
    public Button CloseJobsButton;

    public Button NewJobs;
    public Button CompletedJobs;
    public GameObject NewJobsPanel;
    public GameObject CompletedJobspanel;
    public Button BackToJobsButton;


    private int currentProfile;
    private int coins;


    //panele po ukonczonych poziomach
    public GameObject LevelCompletedPanel;
    public Button CloseLevelCompletedButton;

    public GameObject Level2CompletedPanel;
    public Button CloseLevel2CompletedButton;


    private void Start()
    {
        currentProfile = PlayerPrefs.GetInt("CurrentProfile", 1);

        string gardenerName = PlayerPrefs.GetString("GardenerName_" + currentProfile, "Unknown");
        GardenerNameText.text = "Hello, " + gardenerName + " !";

        if (!PlayerPrefs.HasKey("Coins_" + currentProfile))
        {
            PlayerPrefs.SetInt("Coins_" + currentProfile, 30);
            PlayerPrefs.Save();
        }

        coins = PlayerPrefs.GetInt("Coins_" + currentProfile, 30);
        UpdateCoinsUI();

        bool isLevel2Unlocked = PlayerPrefs.GetInt("Level2Unlocked_" + currentProfile, 0) == 1;
        Level1Button.interactable = true;
        Level2Button.interactable = isLevel2Unlocked;

        if (PlayerPrefs.GetInt("FirstTime_" + currentProfile, 1) == 1)
        {
            IntroductionPanel.SetActive(true);
            PlayerPrefs.SetInt("FirstTime_" + currentProfile, 0);
            PlayerPrefs.Save();
        }
        else
        {
            IntroductionPanel.SetActive(false);
        }


        LevelCompletedPanel.SetActive(false);
        Level2CompletedPanel.SetActive(false);




        //panele do ukoñczenia poziomu
        // ukoñczenie poziomu 1 (Garden1)
        {
            coins = PlayerPrefs.GetInt("Coins_" + currentProfile, 30);
            UpdateCoinsUI();



            // --- Garden1 (Level 1) ---
            if (PlayerPrefs.GetInt("LevelCompleted", 0) == 1)
            {
                LevelCompletedPanel.SetActive(true);

                CloseLevelCompletedButton.onClick.AddListener(() =>
                {
                    int reward = 50;
                    coins += reward;

                    PlayerPrefs.SetInt("Coins_" + currentProfile, coins);
                    PlayerPrefs.SetInt("LevelCompleted", 0); // Zresetuj po klikniêciu
                    PlayerPrefs.DeleteKey("CompletedMissionName");
                    PlayerPrefs.Save();

                    UpdateCoinsUI();
                    LevelCompletedPanel.SetActive(false);
                });
            }

            // --- Garden2 (Level 2) ---
            if (PlayerPrefs.GetInt("Level2Completed", 0) == 1)
            {
                Level2CompletedPanel.SetActive(true);

                CloseLevel2CompletedButton.onClick.AddListener(() =>
                {
                    int reward = 100;
                    coins += reward;

                    PlayerPrefs.SetInt("Coins_" + currentProfile, coins);
                    PlayerPrefs.SetInt("Level2Completed", 0); // Zresetuj po klikniêciu
                    PlayerPrefs.Save();

                    UpdateCoinsUI();
                    Level2CompletedPanel.SetActive(false);
                });
            }
        }






        Level1Button.onClick.AddListener(() => LoadLevel("Garden1"));
        Level2Button.onClick.AddListener(() => LoadLevel("Garden2"));
        BackToMenuButton.onClick.AddListener(BackToMenu);
        ContinueButton.onClick.AddListener(HideIntroductionPanel);

        OpenShopButton.onClick.AddListener(OpenShop);
        CloseShopButton.onClick.AddListener(CloseShop);

        OpenJobsButton.onClick.AddListener(OpenJobs);
        CloseJobsButton.onClick.AddListener(CloseJobs);
        BackToJobsButton.onClick?.AddListener(BackToJobs);




        ShopPanel.SetActive(false);
        CloseShopButton.gameObject.SetActive(false);

        JobsPanel.SetActive(false);
        CloseJobsButton.gameObject.SetActive(false);

        CompletedJobspanel.SetActive(false);
        NewJobsPanel.SetActive(false);
        NewJobs.gameObject.SetActive(false);
        CompletedJobs.gameObject.SetActive(false);
        BackToJobsButton.gameObject.SetActive(false);

    }

    void RemoveDontDestroyOnLoadObjects()
    {
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            if (obj.scene.name == "DontDestroyOnLoad")
            {
                Destroy(obj);
            }
        }
    }

    public void LoadLevel(string sceneName)
    {
        RemoveDontDestroyOnLoadObjects();
        SceneManager.LoadScene(sceneName);
        Invoke("UpdateLighting", 0.5f);
    }

    public void BackToMenu()
    {
        CloseJobsButton.gameObject.SetActive(false);
        CloseShopButton.gameObject.SetActive(false);
        SceneManager.LoadScene("MainMenu");
    }

    public void EarnCoins(int amount)
    {
        coins += amount;
        PlayerPrefs.SetInt("Coins_" + currentProfile, coins);
        PlayerPrefs.Save();
        UpdateCoinsUI();
    }

    public bool SpendCoins(int amount)
    {
        if (coins >= amount)
        {
            coins -= amount;
            PlayerPrefs.SetInt("Coins_" + currentProfile, coins);
            PlayerPrefs.Save();
            UpdateCoinsUI();
            return true;
        }
        return false;
    }

    private void UpdateCoinsUI()
    {
        CoinsText.text = "Coins: " + coins;
    }


    public void UnlockLevel2()
    {
        PlayerPrefs.SetInt("Level2Unlocked_" + currentProfile, 1);
        PlayerPrefs.Save();
        Level2Button.interactable = true;
    }

    private void HideIntroductionPanel()
    {
        IntroductionPanel.SetActive(false);
    }

    public void OpenShop()
    {
        ShopPanel.SetActive(true);
        OpenShopButton.gameObject.SetActive(false);
        OpenJobsButton.gameObject.SetActive(false);
        BackToMenuButton.gameObject.SetActive(false);
        CloseShopButton.gameObject.SetActive(true);
    }

    public void CloseShop()
    {
        ShopPanel.SetActive(false);
        OpenShopButton.gameObject.SetActive(true);
        OpenJobsButton.gameObject.SetActive(true);
        BackToMenuButton.gameObject.SetActive(true);
        CloseShopButton.gameObject.SetActive(false);
    }

    public void OpenJobs()
    {
        JobsPanel.SetActive(true);
        OpenShopButton.gameObject.SetActive(false);
        OpenJobsButton.gameObject.SetActive(false);
        BackToMenuButton.gameObject.SetActive(false);
        CloseJobsButton.gameObject.SetActive(true);

        NewJobs.gameObject.SetActive(true);
        CompletedJobs.gameObject.SetActive(true);  
    }

    public void CloseJobs()
    {
        JobsPanel.SetActive(false);
        OpenShopButton.gameObject.SetActive(true);
        OpenJobsButton.gameObject.SetActive(true);
        BackToMenuButton.gameObject.SetActive(true);
        CloseJobsButton.gameObject.SetActive(false);
        NewJobs.gameObject.SetActive(false);
        CompletedJobs.gameObject.SetActive(false);

        NewJobs.gameObject.SetActive(false);
        CompletedJobs.gameObject.SetActive(false);

        CompletedJobspanel.SetActive(false);
        NewJobsPanel.SetActive(false);
    }

    public void OpenNewJobs()
    {
        NewJobsPanel.SetActive(true);
        BackToJobsButton.gameObject.SetActive(true);
        CloseJobsButton.gameObject.SetActive(false);

        //jobs buttons
        NewJobs.gameObject.SetActive(false);
        CompletedJobs.gameObject.SetActive(false);

    }

    public void OpenCompletedJobs()
    {
        CompletedJobspanel.SetActive(true);
        BackToJobsButton.gameObject.SetActive(true);
        CloseJobsButton.gameObject.SetActive(false);

        //jobs buttons
        NewJobs.gameObject.SetActive(false);
        CompletedJobs.gameObject.SetActive(false);

    }

    public void BackToJobs()
    {
        BackToJobsButton.gameObject.SetActive(false);
        CompletedJobspanel.SetActive(false);
        NewJobsPanel.SetActive(false);
        CloseJobsButton.gameObject.SetActive(true);

        NewJobs.gameObject.SetActive(true);
        CompletedJobs.gameObject.SetActive(true);

    }
}
