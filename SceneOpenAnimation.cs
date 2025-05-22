using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneOpenAnimation : MonoBehaviour
{
    public Animator open;
    void Start()
    {
        open.SetTrigger("Open");
    }

}
