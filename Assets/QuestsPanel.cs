using UnityEngine;
using System.Collections;

public class QuestsPanel : MonoBehaviour {

    public Animator animator;

    public void ShowQuestsPanel()
    {
        animator.SetTrigger("enable");
    }
}
