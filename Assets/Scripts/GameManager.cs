using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

    //defining the event that will be thrown once the word is verified
    public delegate void LetterVerified();
    public static event LetterVerified OnWordVerified;

    public static GameManager instance;
    public static int gridSize=9;
    public static int totalNumberOfWords=3;
    public static string selectedString="";
    public Text selectedLettersText;
    public List<GameObject> selectedCells;
    public List<string> generatedWords;

    public GameObject linePrefab;
    private List<Color> lineColors;

    [SerializeField]
    private GameObject countToStartPanel;
    public Text wordsFoundText;
    public bool gameStarted;
    public float gameTime;
    public Text timerText;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        lineColors = new List<Color> { new Vector4(0,0,0,0.3f),new Vector4(0,0,1f,0.3f),new Vector4(0, 1f, 1f, 0.3f), new Vector4(0.5f, 0.5f, 0.5f, 0.3f), new Vector4(0, 1f, 0f, 0.3f), new Vector4(1f, 0, 1f, 0.3f), new Vector4(1f, 0, 0f, 0.3f), new Vector4(1f, 0.92f, 0.016f, 0.3f) };
        //SceneManager.LoadScene("PlayScreen");
        selectedLettersText.text = "";

        UpdateWordCountText(totalNumberOfWords, totalNumberOfWords);
        timerText.text = "00:00";
        countToStartPanel.SetActive(true);      
    }

    void Update()
    {
        if (gameStarted)
        {
            gameTime += Time.deltaTime;
            string minSec = string.Format("{0}:{1:00}", (int)gameTime / 60, (int)gameTime % 60);
            timerText.text = minSec;
        }
    }

    public void SelectGridSize(int size)
    {
        gridSize = size;
        SceneManager.LoadScene("PlayScreen");
    }

    //this method will be called from cell class each time player stops selecting the cells(writing the word); OnMouseUp event Method
    public void VerifyWord()
    {
        //when the written word is verified, trigger the event, so every cell can reset
        if (OnWordVerified != null)
        {
            OnWordVerified();
        }

        for (int i = 0; i < generatedWords.Count; i++)
        {
            if (selectedString == generatedWords[i])
            {
                generatedWords.RemoveAt(i);

                UpdateWordCountText(generatedWords.Count , totalNumberOfWords);

                //removing the word from words panel; letting player know which words he have already found
                Destroy(LayoutManager.instance.wordsInWordsPanel[i].gameObject);
                LayoutManager.instance.wordsInWordsPanel.RemoveAt(i);

                //draw a line connecting all selected cells
                GameObject line = Instantiate(linePrefab, selectedCells[0].transform.position, Quaternion.identity) as GameObject;
                line.gameObject.name = "Line";
                line.GetComponent<LineRenderer>().SetPosition(0, selectedCells[0].transform.position);
                line.GetComponent<LineRenderer>().SetPosition(1, selectedCells[selectedCells.Count-1].transform.position);

                int rnd = Random.Range(0, lineColors.Count);
                line.GetComponent<LineRenderer>().SetColors(lineColors[rnd], lineColors[rnd]);

                //check if player have found all words
                if (generatedWords.Count == 0)
                {
                    Debug.Log("all words found");
                    LevelCleared();
                }
                break;
            }
        }

        //empty the list of selected cells
        selectedCells.Clear();      
    }

    public void UpdateWordCountText(int numberOfWordsLeft, int numberOfTotalWords)
    {
        wordsFoundText.text = (numberOfTotalWords-numberOfWordsLeft).ToString() + "/" + numberOfTotalWords.ToString();
    }

    private void LevelCleared()
    {
        gameStarted = false;
        Debug.Log("game finished in time: " + gameTime);           
    }
}
