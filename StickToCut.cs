using UnityEngine;
using System.Collections.Generic;

public class StickToCut : MonoBehaviour
{

    public Camera playerCamera;
    public LayerMask interactableLayer;
    private Shears shearsScript;
    public GameObject PressLMB; 
    public AudioSource StickCutSound;

    private Outline stickOutline;
    private GameObject currentTarget;

    public StickCutted stickCuttedScript; 

    private static List<StickToCut> allSticks = new List<StickToCut>();

    void Start()
    {
        shearsScript = FindObjectOfType<Shears>();

        stickOutline = GetComponent<Outline>();
        if (stickOutline != null)
            stickOutline.enabled = false;

        if (stickCuttedScript != null)
            stickCuttedScript.enabled = false; 

        if (PressLMB != null)
            PressLMB.SetActive(false); 

        allSticks.Add(this);

    }

    void OnDestroy()
    {
        allSticks.Remove(this);
    }

    void Update()
    {
        HandleOutline();
        HandleCutting();
    }

    private void HandleOutline()
    {

        if (shearsScript != null && shearsScript.isHoldingShears)
        {
            Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 2f, interactableLayer))
            {
                if (hit.transform.gameObject != currentTarget)
                {
                    if (currentTarget != null && stickOutline != null)
                    {
                        stickOutline.enabled = false;
                    }

                    currentTarget = hit.transform.gameObject;
                    stickOutline = currentTarget.GetComponent<Outline>();

                    if (stickOutline != null)
                    {
                        stickOutline.enabled = true;
                        if (PressLMB != null)
                            PressLMB.SetActive(true);
                    }
                }
            }
            else
            {
                if (currentTarget != null && stickOutline != null)
                {
                    stickOutline.enabled = false;
                }
                if (PressLMB != null)
                    PressLMB.SetActive(false);
                currentTarget = null;
            }
        }
        else
        {
            if (currentTarget != null && stickOutline != null)
            {
                stickOutline.enabled = false;
            }
            if (PressLMB != null)
                PressLMB.SetActive(false);
        }
    }

    private void HandleCutting()
    {
        if (Input.GetMouseButtonDown(0) && currentTarget != null && shearsScript != null && shearsScript.isHoldingShears)
        {
            StickCutSound.Play();

            Rigidbody rb = currentTarget.AddComponent<Rigidbody>();
            rb.mass = 0.5f;
            rb.angularDrag = 0.5f;

            currentTarget.layer = LayerMask.NameToLayer("StickCutted");

            StickToCut stickToCutScript = currentTarget.GetComponent<StickToCut>();
            if (stickToCutScript != null)
                stickToCutScript.enabled = false;

            StickCutted stickCutted = currentTarget.GetComponent<StickCutted>();
            if (stickCutted != null)
            {
                stickCutted.enabled = true;
            }

            DisableAllPressLMB();

            currentTarget = null;
        }
    }

    private void DisableAllPressLMB()
    {
        foreach (StickToCut stick in allSticks)
        {
            if (stick.PressLMB != null)
            {
                stick.PressLMB.SetActive(false);
            }
        }
    }
}
