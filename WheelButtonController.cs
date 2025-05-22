using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WheelButtonController : MonoBehaviour
{

    public int ID;
    private Animator anim;
    public AudioSource HowerSound;
    void Start()
    {
        anim = GetComponent<Animator>();
        HowerSound.Stop();
    }

    public void HowerEnter()
    {
        anim.SetBool("Hover", true);
        HowerSound.Play();

    }

    public void HowerExit()
    {
        anim.SetBool("Hover", false);
    }
}
