using UnityEngine;

public class HUDController : MonoBehaviour
{
    public Animator hudAnimator;
    private bool isHudOpen = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (!isHudOpen)
            {
                hudAnimator.SetBool("Open", true);
                hudAnimator.SetBool("Close", false);
                isHudOpen = true;
            }
            else
            {
                hudAnimator.SetBool("Open", false);
                hudAnimator.SetBool("Close", true);
                isHudOpen = false;
            }
        }
    }
}
