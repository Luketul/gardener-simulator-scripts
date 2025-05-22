using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;

public class GardenManager : MonoBehaviour
{
    public static GardenManager Instance { get; private set; }

    public int trashCount = 0;
    public int trashToCompleteLevel;

    public int ToyInSandboxCount = 0;
    public int ToyInSandboxToCompleteLevel;

    public int wateredFlowersCount = 0;
    public int wateredFlowersToCompleteLevel;

    public int sprayedFlowersCount = 0;
    public int sprayedFlowersToCompleteLevel;

    public int wateredBushCount = 0;
    public int wateredBushToCompleteLevel;

    public AudioSource success;
    public Button GnomeButton;
    public Button finishLevelButton;

    public TextMeshProUGUI trashCounterText;
    public TextMeshProUGUI toyinsandboxCounterText;
    public TextMeshProUGUI flowerCounterText;
    public TextMeshProUGUI sprayerCounterText;
    public TextMeshProUGUI bushCounterText;
    public TextMeshProUGUI gnomeTaskText; 
    public TextMeshProUGUI goToCarText;

    public AudioSource CarDoorClose;




    private bool gnomePlaced = false;
    public GameObject hud;
    public Animator transitionAnimator; 
    public string mainMenuSceneName = "PlayerProfile";



    //check 


    public Animator trashCheckAnimator;
    public Animator toyCheckAnimator;
    public Animator flowerCheckAnimator;
    public Animator sprayerCheckAnimator;
    public Animator bushCheckAnimator;
    public Animator checkGnome;



    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        finishLevelButton.interactable = false;
        finishLevelButton.onClick.AddListener(FinishLevel);



    }

    private void Start()
    {
        transitionAnimator.SetTrigger("Open");
        gnomeTaskText.gameObject.SetActive(false);
        goToCarText.gameObject.SetActive(false);

        UpdateTrashCounterUI();
        UpdateToyCounterUI();
        UpdateFlowerCounterUI();
        UpdateSprayedFlowerCounterUI();
        UpdateBushCounterUI();

        CarDoorClose.Play();
    }



    public void AddWateredFlower()
    {
        wateredFlowersCount++;
        UpdateFlowerCounterUI();
        success.Play();
    }

    public void AddSprayedFlower()
    {
        sprayedFlowersCount++;
        UpdateSprayedFlowerCounterUI();
        success.Play();
    }

    public void AddTrash(int amount)
    {
        if (amount > 0)
        {
            trashCount += amount;
            UpdateTrashCounterUI();
            success.Play();
        }
    }

    public void AddToy()
    {
        ToyInSandboxCount++;
        UpdateToyCounterUI();
    }

    public void SubtractToy()
    {
        ToyInSandboxCount--;
        UpdateToyCounterUI();
    }

    public void AddWateredBush()
    {
        wateredBushCount++;
        UpdateBushCounterUI();
        success.Play();
    }

    private void UpdateTrashCounterUI()
    {
        if (trashCounterText != null)
        {
            trashCounterText.text = $"Thrown Away Garbage: {trashCount}/{trashToCompleteLevel}";

            if (trashCount >= trashToCompleteLevel && trashCounterText.fontStyle != FontStyles.Strikethrough)
            {
                MarkTaskAsCompleted(trashCounterText);
                trashCheckAnimator.SetTrigger("Open");
            }
            RearrangeTasks();
        }
    }

    private void UpdateToyCounterUI()
    {
        if (toyinsandboxCounterText != null)
        {
            toyinsandboxCounterText.text = $"Toys In Sandbox: {ToyInSandboxCount}/{ToyInSandboxToCompleteLevel}";

            if (ToyInSandboxCount >= ToyInSandboxToCompleteLevel && toyinsandboxCounterText.fontStyle != FontStyles.Strikethrough)
            {
                MarkTaskAsCompleted(toyinsandboxCounterText);
                toyCheckAnimator.SetTrigger("Open");
            }
            RearrangeTasks();
        }
    }

    private void UpdateFlowerCounterUI()
    {
        if (flowerCounterText != null)
        {
            flowerCounterText.text = $"Watered Flowers: {wateredFlowersCount}/{wateredFlowersToCompleteLevel}";

            if (wateredFlowersCount >= wateredFlowersToCompleteLevel && flowerCounterText.fontStyle != FontStyles.Strikethrough)
            {
                MarkTaskAsCompleted(flowerCounterText);
                flowerCheckAnimator.SetTrigger("Open");
            }
            RearrangeTasks();
        }
    }

    private void UpdateSprayedFlowerCounterUI()
    {
        if (sprayerCounterText != null)
        {
            sprayerCounterText.text = $"Sprayed Flowers: {sprayedFlowersCount}/{sprayedFlowersToCompleteLevel}";

            if (sprayedFlowersCount >= sprayedFlowersToCompleteLevel && sprayerCounterText.fontStyle != FontStyles.Strikethrough)
            {
                MarkTaskAsCompleted(sprayerCounterText);
                sprayerCheckAnimator.SetTrigger("Open");
            }
            RearrangeTasks();
        }
    }

    private void UpdateBushCounterUI()
    {
        if (bushCounterText != null)
        {
            bushCounterText.text = $"Watered Bush: {wateredBushCount}/{wateredBushToCompleteLevel}";

            if (wateredBushCount >= wateredBushToCompleteLevel && bushCounterText.fontStyle != FontStyles.Strikethrough)
            {
                MarkTaskAsCompleted(bushCounterText);
                bushCheckAnimator.SetTrigger("Open");
            }
            RearrangeTasks();
        }
    }




    private void RearrangeTasks()
    {
        float yOffset = 175;
        float startY = 255;

        TextMeshProUGUI[] texts = { trashCounterText, toyinsandboxCounterText, flowerCounterText, sprayerCounterText, bushCounterText, gnomeTaskText, goToCarText };

        float currentY = startY;
        bool allTasksCompleted = true;

        // List to hold completed tasks that should go to the bottom
        List<TextMeshProUGUI> completedTasks = new List<TextMeshProUGUI>();
        List<TextMeshProUGUI> uncompletedTasks = new List<TextMeshProUGUI>();

        foreach (var text in texts)
        {
            // Check if the text element is active and if the task is completed
            if (text != null && text.gameObject.activeSelf)
            {
                // If the task is completed, add it to the completedTasks list, otherwise add to uncompletedTasks
                if (IsTaskCompleted(text))
                {
                    completedTasks.Add(text); // Mark completed task for later
                }
                else
                {
                    uncompletedTasks.Add(text); // Uncompleted tasks remain in the task list
                }
            }
        }

        // Rearrange uncompleted tasks normally
        foreach (var text in uncompletedTasks)
        {
            text.rectTransform.anchoredPosition = new Vector2(text.rectTransform.anchoredPosition.x, currentY);
            currentY -= yOffset;  // Adjust position for uncompleted task
            allTasksCompleted = false;
        }

        // Place completed tasks at the bottom (after uncompleted ones)
        foreach (var completedTask in completedTasks)
        {
            completedTask.rectTransform.anchoredPosition = new Vector2(completedTask.rectTransform.anchoredPosition.x, currentY);
            currentY -= yOffset;  // Adjust position for completed task
        }

        // If all tasks are completed, fade out completed tasks and show the gnome task
        if (allTasksCompleted && !gnomePlaced)
        {
            // Fade out all completed tasks
            StartCoroutine(FadeOutCompletedTasks(completedTasks));
        }
        else if (gnomePlaced)
        {
            if (gnomeTaskText != null)
            {
                gnomeTaskText.gameObject.SetActive(false);
            }

            if (goToCarText != null)
            {
                goToCarText.gameObject.SetActive(true);
                goToCarText.text = "Go to car to finish the level";
                goToCarText.rectTransform.anchoredPosition = new Vector2(goToCarText.rectTransform.anchoredPosition.x, startY);
            }
        }
    }

    // Coroutine to fade out completed tasks
    private IEnumerator FadeOutCompletedTasks(List<TextMeshProUGUI> completedTasks)
    {
        float fadeDuration = 0.7f;  // Adjust fade duration if needed
        foreach (var task in completedTasks)
        {
            // Fade out each completed task
            StartCoroutine(FadeOutCoroutine(task));
            yield return new WaitForSeconds(fadeDuration);  // Wait for the fade to complete
        }

        // After fading out, show the gnome task
        if (gnomeTaskText != null)
        {
            gnomeTaskText.gameObject.SetActive(true);
            gnomeTaskText.text = "Place the gnome in the garden";
            gnomeTaskText.rectTransform.anchoredPosition = new Vector2(gnomeTaskText.rectTransform.anchoredPosition.x, 255);  // Adjust position if needed
            GnomeButton.interactable = true;  // Enable the Gnome button
        }
    }

    private void MarkTaskAsCompleted(TextMeshProUGUI textElement)
    {
        textElement.fontStyle = FontStyles.Strikethrough; // Dodaje przekreœlenie
    }


    // The existing FadeOutCoroutine (used for fading individual tasks)
    private IEnumerator FadeOutCoroutine(TextMeshProUGUI textElement)
    {
        float duration = 0.7f;
        float elapsed = 0f;
        Color originalColor = textElement.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
            textElement.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        textElement.gameObject.SetActive(false);
    }


    // Helper method to check if a task is completed based on its text element
    private bool IsTaskCompleted(TextMeshProUGUI textElement)
    {
        if (textElement == trashCounterText && trashCount >= trashToCompleteLevel)
            return true;
        if (textElement == toyinsandboxCounterText && ToyInSandboxCount >= ToyInSandboxToCompleteLevel)
            return true;
        if (textElement == flowerCounterText && wateredFlowersCount >= wateredFlowersToCompleteLevel)
            return true;
        if (textElement == sprayerCounterText && sprayedFlowersCount >= sprayedFlowersToCompleteLevel)
            return true;
        if (textElement == bushCounterText && wateredBushCount >= wateredBushToCompleteLevel)
            return true;

        return false;  // If no condition is met, the task is not completed
    }

    public void GnomePlaced()
    {
        gnomePlaced = true;
        finishLevelButton.interactable = true;
        RearrangeTasks();

        if (checkGnome != null)
        {
            checkGnome.SetTrigger("Open");
        }
    }

    public void FinishLevel()
    {
        if (AllTasksCompleted())
        {

            string sceneName = SceneManager.GetActiveScene().name;
            int currentProfile = PlayerPrefs.GetInt("CurrentProfile", 1);

            if (sceneName == "Garden1")
            {
                PlayerPrefs.SetInt("Level2Unlocked_" + currentProfile, 1);
                PlayerPrefs.SetInt("LevelCompleted", 1);
            }
            else if (sceneName == "Garden2")
            {
                PlayerPrefs.SetInt("Level2Completed", 1);
            }

            PlayerPrefs.Save();
            StartCoroutine(LoadMenuAfterAnimation());
        }
    }


    private IEnumerator LoadMenuAfterAnimation()
    {
        transitionAnimator.SetTrigger("Start");

        if (hud != null)
        {
            hud.SetActive(false);
        }

        yield return new WaitForSeconds(1f);

        SceneManager.LoadScene(mainMenuSceneName);
    }

    public bool AllTasksCompleted()
    {
        finishLevelButton.interactable = true; 

        return trashCount >= trashToCompleteLevel &&
               ToyInSandboxCount >= ToyInSandboxToCompleteLevel &&
               wateredFlowersCount >= wateredFlowersToCompleteLevel &&
               sprayedFlowersCount >= sprayedFlowersToCompleteLevel &&
               wateredBushCount >= wateredBushToCompleteLevel &&
               gnomePlaced;
    }
}




