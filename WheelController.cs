using UnityEngine;

public class RadialMenuAnimatorController : MonoBehaviour
{
    private Animator radialMenuAnimator;

    void Start()
    {
        radialMenuAnimator = GetComponent<Animator>();
    }

    public void OpenMenu()
    {
        if (radialMenuAnimator != null)
        {
            radialMenuAnimator.SetBool("Open", true);
            radialMenuAnimator.SetBool("Close", false);
        }
    }

    public void CloseMenu()
    {
        if (radialMenuAnimator != null)
        {
            radialMenuAnimator.SetBool("Open", false);
            radialMenuAnimator.SetBool("Close", true);
        }
    }
}
