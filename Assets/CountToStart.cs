using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CountToStart : MonoBehaviour {

    public Text countToStartText;

    void Start()
    {
        StartCoroutine(Count());
    }

    private IEnumerator Count()
    {
        countToStartText.text = "3 ...";
        yield return new WaitForSeconds(1.0f);
        countToStartText.text = "2 ...";
        yield return new WaitForSeconds(1.0f);
        countToStartText.text = "1 ...";
        yield return new WaitForSeconds(1.0f);
        this.gameObject.SetActive(false);

        GameManager.instance.gameTime = 0.0f;
        GameManager.instance.gameStarted = true;
    }
}
