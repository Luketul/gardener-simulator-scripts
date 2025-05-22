using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PauseGame : MonoBehaviour
{
    public GameObject menu;
    public static bool isGamePaused = false;
    public Animator anim;
    public GameObject hud;

    private void Start()
    {
        menu.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        anim.SetTrigger("Open");
    }

    void Awake()
    {
        if (FindObjectsOfType<PauseGame>().Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isGamePaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        Time.timeScale = 1;
        menu.SetActive(false);
        isGamePaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void Pause()
    {
        Time.timeScale = 0;
        menu.SetActive(true);
        isGamePaused = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Exit()
    {
        StartCoroutine(LoadMenuAfterAnimation());
        anim.SetTrigger("Open");
    }

    private IEnumerator LoadMenuAfterAnimation()
    {
        Time.timeScale = 1;
        anim.SetTrigger("Start");  

        if (hud != null)
        {
            hud.SetActive(false);
        }

        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);

        SceneManager.LoadScene("PlayerProfile");
    }
}
