using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class HintsManager : MonoBehaviour {

    [SerializeField]
    private Button useHintButton;           //button in bottom UI that we use for showing a hint to player
                                            //one random word that is still in list of words that are not found flashes

    void Start()
    {
        useHintButton.onClick.AddListener(UseHint);
    }

    private void UseHint()
    {
        //only allow showing hints if player have hint credits avilable
        if (PlayerPreferences.hints > 0)
        {
            //subtract one hint credit from player hint credits
            PlayerPreferences.hints--;

            TopUI.instance.RefreshTopUI();

            //show hint (one random word that is still not found yet flashes)
            ShowHint();
        }
        else
        {
            Debug.Log("Player dont have hint credits available");
            //show store window and focus on buying hints
        }
    }

    /// <summary>
    /// this method will play a particle effect on all cells that are occupied with random word from list of words that are still not found yet
    /// </summary>
    private void ShowHint()
    {
        int rndIndex = Random.Range(0, LayoutManager.instance.wordsInWordsPanel.Count);
        string randomWord = LayoutManager.instance.wordsInWordsPanel[rndIndex].GetComponentInChildren<Text>().text;

        Debug.Log("random word for hint is: " + randomWord);

        foreach (KeyValuePair<string, int[,]> pair in LayoutManager.instance.finalDictionaryOfInsertedWords)
        {
            //random word is found in dictionary
            if (pair.Key == randomWord)
            {
                //go through an array of positions where word is inserted and start hint effect on each cell that is occupied with letters of that word
                for (int i = 0; i < pair.Key.Length; i++)
                {
                    GameObject.Find("Cell" + pair.Value[i, 0] + pair.Value[i, 1]).GetComponent<Cell>().showHintEffectOnCell = true;
                    //this will need to be improved - make better visual effect
                    float alpha = (float)i/pair.Key.Length;
                    Vector4 color = new Vector4(1.0f,0.0f,1.0f,1.0f-(alpha/2));
                    StartCoroutine(GameObject.Find("Cell" + pair.Value[i, 0] + pair.Value[i, 1]).GetComponent<Cell>().ShowingHintEffect(color));
                    //
                }
                break;
            }
        }
    }
}
