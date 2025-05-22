using UnityEngine;
using TMPro;
using System.Collections;

public class TrashBag : MonoBehaviour
{
    public GameObject TrashBagPickedUp;
    public GameObject TrashBagObject;
    public AudioSource PickUpSound;
    public AudioSource ThrowTrash;

    private PlayerMovement playerMovement;

    public bool isHoldingTrashBag;
    private int trashCount = 0;
    private const int maxTrashCapacity = 3;

    public GameObject cavnas;
    public TextMeshProUGUI trashCountText;

    public Animator anim;

    public Animator hint;

    private void Start()
    {
        cavnas.SetActive(false);
        isHoldingTrashBag = false;
        UpdateTrashCountUI();
        TrashBagPickedUp.SetActive(false);
        playerMovement = FindObjectOfType<PlayerMovement>();

    }

    public void PickUpTrashBag()
    {
        TrashBagPickedUp.SetActive(true);
        cavnas.SetActive(true);
        TrashBagObject.SetActive(true);
        PickUpSound.Play();
        isHoldingTrashBag = true;

        playerMovement.heldItem = TrashBagPickedUp;
    }

    public void DropTrashBag()
    {
        TrashBagPickedUp.SetActive(false);
        cavnas.SetActive(false);
        TrashBagObject.SetActive(false);
        ThrowTrash.Play();
        isHoldingTrashBag = false;
        EmptyTrashBag();

        playerMovement.heldItem = null;
    }

    public bool AddTrash()
    {
        if (trashCount < maxTrashCapacity)
        {
            anim.SetTrigger("Add");
            trashCount++;
            UpdateTrashCountUI(); 
            return true;
        }
        else
        {
            hint.SetBool("ISFULL", true);
            StartCoroutine(HideHintAfterDelay(2f));
            Debug.Log("TrashBag pe³ny!");
            return false;
        }
    }

    public int GetTrashCount()
    {
        return trashCount;
    }

    public void EmptyTrashBag()
    {
        trashCount = 0;
        UpdateTrashCountUI();  
    }

    private void UpdateTrashCountUI()
    {
        if (trashCountText != null)
        {
            trashCountText.text = "Trash In Bag: " + trashCount + "/" + maxTrashCapacity;
        }
    }

    private IEnumerator HideHintAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        hint.SetBool("ISFULL", false);
    }
}
