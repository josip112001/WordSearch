using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StorePanel : MonoBehaviour {

    public Animator animator;
    public Button closeButton;

    void Start()
    {
        closeButton.onClick.AddListener(() =>
        {
            animator.SetTrigger("disable");
            Destroy(this.gameObject, 0.5f);
        });
    }
}
