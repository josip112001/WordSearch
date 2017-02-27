using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Cell : MonoBehaviour {

    public bool occupied = false;
    public string letter;
    public Text letterText;
    public bool showHintEffectOnCell=false;

    void OnEnable()
    {
        GameManager.OnWordVerified += Reset;
    }

    void OnDisable()
    {
        GameManager.OnWordVerified -= Reset;
    }

    void OnMouseDown()
    {
        this.GetComponent<Image>().color = new Vector4(0f,0f,0.6f,0.3f);
        GameManager.selectedString += letter;
        GameManager.instance.selectedLettersText.text = GameManager.selectedString;
        GameManager.instance.selectedCells.Add(this.gameObject);      
    }

    void OnMouseEnter()
    {
        if (Input.GetMouseButton(0) == true && !GameManager.instance.selectedCells.Contains(this.gameObject))
        {
            this.GetComponent<Image>().color = new Vector4 (0f,0f,0.6f,0.3f);
            GameManager.selectedString += letter;
            GameManager.instance.selectedLettersText.text = GameManager.selectedString;
            GameManager.instance.selectedCells.Add(this.gameObject);           
        }
    }

    void OnMouseUp()
    {
        Debug.Log("this word is written: " + GameManager.selectedString);
        GameManager.instance.VerifyWord();
        GameManager.selectedString = "";
        GameManager.instance.selectedLettersText.text = GameManager.selectedString;

    }

    //reset the color of cell once the written word has been verified
    void Reset()
    {
         GetComponent<Image>().color = Color.white; 
    }

    //this ienumerator is used for changing the font of cell letter - this way we simulate effect of hint showing
    public IEnumerator ShowingHintEffect(Vector4 color)
    {
        yield return null;
        GetComponent<Image>().color = color;

    }
}
