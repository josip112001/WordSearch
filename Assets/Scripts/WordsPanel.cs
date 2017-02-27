using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WordsPanel : MonoBehaviour {

    public GridLayoutGroup gridLayoutGruop;

    void Start()
    {
        gridLayoutGruop.cellSize=new Vector2((GetComponent<RectTransform>().rect.width-55)/4, (GetComponent<RectTransform>().rect.height - 80) / 3);
        for (int i = 0; i < transform.childCount; i++)
        {
            Debug.Log(transform.GetChild(i).gameObject.name);
            Debug.Log("cell size is: " + gridLayoutGruop.cellSize);
            transform.GetChild(i).GetChild(0).GetComponent<RectTransform>().sizeDelta =new Vector2 ((float)0.85 * gridLayoutGruop.cellSize.x, gridLayoutGruop.cellSize.y);
        }        
    }
}
