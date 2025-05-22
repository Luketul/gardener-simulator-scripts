using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;

public class LawnMowerController : MonoBehaviour
{
    public Transform lawnMower;
    public Transform lawnMowerCamera;
    public Transform playerCamera;
    public Transform player;
    public Transform playerSpawnPoint;
    public Transform lawnMowerTarget;

    public float turnAngle = 30f;
    public float lawnMowerSpeed = 5f;

    private CharacterController characterController;
    private MeshRenderer playerMeshRenderer;
    private bool isUsingLawnMower = false;
    private Quaternion lastPlayerRotation;
    private Vector3 lastPlayerPosition;

    public AudioSource mowerSound;
    public float fadeOutDuration = 2.0f;

    private PlayerMovement playerMovement;
    private Coroutine fadeOutCoroutine;
    private Coroutine smoothLoopCoroutine;

    public float loopStart = 1.0f;
    public float loopEnd = 5.0f;

    public Terrain terrain;
    public float grassCutRadius = 2f;
    public int grassDetailIndex = 0;

    public int totalGrass = 0;
    public int grassLeft = 0;
    public float grassCutPercentage = 0f;

    public TextMeshProUGUI GrassCutHUD;
    public GameObject interactHUD;

    public Outline mowerOutline;

    public Slider GrassSlider;

    public GameObject Celownik;

    private void Start()
    {
        if (playerCamera.GetComponent<AudioListener>() == null)
        {
            playerCamera.gameObject.AddComponent<AudioListener>();
        }

        if (lawnMowerCamera.GetComponent<AudioListener>() == null)
        {
            lawnMowerCamera.gameObject.AddComponent<AudioListener>();
        }

        characterController = player.GetComponent<CharacterController>();
        playerMeshRenderer = player.GetComponent<MeshRenderer>();

        playerMovement = player.GetComponent<PlayerMovement>();
        lawnMowerCamera.gameObject.SetActive(false);

        if (mowerSound != null)
        {
            mowerSound.loop = false;
        }

        GrassCutHUD.gameObject.SetActive(false);
        interactHUD.SetActive(false);
        GrassSlider.gameObject.SetActive(false);

        if (mowerOutline != null)
        {
            mowerOutline.enabled = false; 
        }

        InitializeGrassCount();

        if (GrassSlider != null)
        {
            GrassSlider.value = 0f;
        }

        InitializeGrassCount();
    }

    private void InitializeGrassCount()
    {
        TerrainData terrainData = terrain.terrainData;
        int[,] details = terrainData.GetDetailLayer(0, 0, terrainData.detailWidth, terrainData.detailHeight, grassDetailIndex);

        for (int x = 0; x < details.GetLength(0); x++)
        {
            for (int z = 0; z < details.GetLength(1); z++)
            {
                totalGrass += details[x, z];
            }
        }

        grassLeft = totalGrass;
    }

    private void Update()
    {
        CheckForLawnMowerInteraction();

        if (isUsingLawnMower)
        {
            ControlLawnMower();
        }

        ShowInteractPrompt();
    }

    private void CheckForLawnMowerInteraction()
    {
        if (playerMovement.heldItem != null)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (isUsingLawnMower)
            {
                StopUsingLawnMower();
            }
            else
            {
                TryStartUsingLawnMower();
            }
        }
    }

    private void TryStartUsingLawnMower()
    {
        Ray ray = new Ray(playerCamera.position, playerCamera.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 3f))
        {
            if (hit.transform == lawnMower)
            {
                StartUsingLawnMower();
            }
        }
    }

    public void StartUsingLawnMower()
    {
        Celownik.SetActive(false);

        GrassSlider.gameObject.SetActive(true);
        GrassCutHUD.gameObject.SetActive(true);

        playerCamera.GetComponent<AudioListener>().enabled = false;
        lawnMowerCamera.GetComponent<AudioListener>().enabled = true;

        isUsingLawnMower = true;
        playerMovement.isControllingLawnMower = true;
        playerMovement.enabled = false;

        lastPlayerPosition = player.position;
        lastPlayerRotation = player.rotation;

        if (playerSpawnPoint != null)
        {
            Vector3 spawnPosition = playerSpawnPoint.position;
            spawnPosition.y = Mathf.Max(spawnPosition.y, Terrain.activeTerrain.SampleHeight(spawnPosition)); 
            player.position = spawnPosition;
        }

        player.SetParent(lawnMower);

        playerCamera.gameObject.SetActive(false);
        lawnMowerCamera.gameObject.SetActive(true);
        characterController.enabled = false;

        if (playerMeshRenderer != null)
        {
            playerMeshRenderer.enabled = false;
        }

        mowerSound.Play();
        mowerSound.volume = 0.02f;

        if (fadeOutCoroutine != null)
        {
            StopCoroutine(fadeOutCoroutine);
        }

        if (smoothLoopCoroutine != null)
        {
            StopCoroutine(smoothLoopCoroutine);
        }
        smoothLoopCoroutine = StartCoroutine(SmoothLoop());

        interactHUD.SetActive(false);
        if (mowerOutline != null) mowerOutline.enabled = false;
    }


    public void ControlLawnMower()
    {
        float moveX = Input.GetAxis("Vertical");
        float turn = -Input.GetAxis("Horizontal");

        Vector3 moveDirection = lawnMower.right * moveX;
        moveDirection.y = 0;

        lawnMower.Translate(moveDirection * lawnMowerSpeed * Time.deltaTime, Space.World);

        float rotationZ = turn * turnAngle * Time.deltaTime;
        lawnMower.Rotate(Vector3.forward * rotationZ);

        CutGrass();
    }

    public void StopUsingLawnMower()
    {

        Celownik.SetActive(true);
        GrassSlider.gameObject.SetActive(false);
        GrassCutHUD.gameObject.SetActive(false);
        interactHUD.gameObject.SetActive(false);

        playerCamera.GetComponent<AudioListener>().enabled = true;
        lawnMowerCamera.GetComponent<AudioListener>().enabled = false;

        isUsingLawnMower = false;
        playerMovement.isControllingLawnMower = false;
        playerMovement.enabled = true; 

        player.SetParent(null);

        if (playerSpawnPoint != null)
        {
            Vector3 spawnPosition = playerSpawnPoint.position;
            spawnPosition.y = Mathf.Max(spawnPosition.y, Terrain.activeTerrain.SampleHeight(spawnPosition)); 
            player.position = spawnPosition; 
        }

        if (lawnMowerTarget != null)
        {
            Vector3 targetDirection = (lawnMowerTarget.position - player.position).normalized;
            if (targetDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
                player.rotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0);
            }
        }

        lawnMowerCamera.gameObject.SetActive(false);
        playerCamera.gameObject.SetActive(true);
        characterController.enabled = true;

        if (playerMeshRenderer != null)
        {
            playerMeshRenderer.enabled = true;
        }

        if (smoothLoopCoroutine != null)
        {
            StopCoroutine(smoothLoopCoroutine);
        }

        if (mowerSound.isPlaying)
        {
            fadeOutCoroutine = StartCoroutine(FadeOutMowerSound());
        }
    }




    private void ShowInteractPrompt()
    {
        Ray ray = new Ray(playerCamera.position, playerCamera.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 2f) && hit.transform == lawnMower && !isUsingLawnMower)
        {
            interactHUD.gameObject.SetActive(true);
            if (mowerOutline != null) mowerOutline.enabled = true;
        }
        else
        {
            interactHUD.gameObject.SetActive(false);
            if (mowerOutline != null) mowerOutline.enabled = false;
        }
    }

    private void CutGrass()
    {
        Vector3 mowerPosition = lawnMower.position;
        TerrainData terrainData = terrain.terrainData;

        int terrainPosX = Mathf.FloorToInt((mowerPosition.x / terrainData.size.x) * terrainData.detailWidth);
        int terrainPosZ = Mathf.FloorToInt((mowerPosition.z / terrainData.size.z) * terrainData.detailHeight);

        int grassRadius = Mathf.FloorToInt((grassCutRadius / terrainData.size.x) * terrainData.detailWidth);

        int[,] details = terrainData.GetDetailLayer(terrainPosX - grassRadius, terrainPosZ - grassRadius, grassRadius * 2, grassRadius * 2, grassDetailIndex);

        int grassCut = 0;

        for (int x = 0; x < details.GetLength(0); x++)
        {
            for (int z = 0; z < details.GetLength(1); z++)
            {
                float distance = Vector2.Distance(new Vector2(x, z), new Vector2(grassRadius, grassRadius));
                if (distance < grassRadius && details[z, x] > 0)
                {
                    grassCut += details[z, x];
                    details[z, x] = 0;
                }
            }
        }

        terrainData.SetDetailLayer(terrainPosX - grassRadius, terrainPosZ - grassRadius, grassDetailIndex, details);

        grassLeft -= grassCut;
        grassCutPercentage = ((float)(totalGrass - grassLeft) / totalGrass) * 100f;

        UpdateHUD();
    }

    private void UpdateHUD()
    {
        if (GrassCutHUD != null)
        {
            GrassCutHUD.text = "Grass Cut: " + grassCutPercentage.ToString("F2") + "%";
        }

        if (GrassSlider != null)
        {
            GrassSlider.value = grassCutPercentage / 100f;
        }
    }

    private IEnumerator FadeOutMowerSound()
    {
        float startVolume = mowerSound.volume;

        float currentTime = 0;
        while (currentTime < fadeOutDuration)
        {
            currentTime += Time.deltaTime;
            mowerSound.volume = Mathf.Lerp(startVolume, 0, currentTime / fadeOutDuration);
            yield return null;
        }

        mowerSound.Stop();
    }

    private IEnumerator SmoothLoop()
    {
        while (mowerSound.isPlaying)
        {
            if (mowerSound.time >= loopEnd)
            {
                mowerSound.time = loopStart;
            }
            yield return null;
        }
    }
}
