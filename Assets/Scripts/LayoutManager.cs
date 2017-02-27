using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class LayoutManager : MonoBehaviour {

    public static LayoutManager instance = null;
    //text file that holds list of all words
    public TextAsset wordpool;
    //list of all words from text file
    public List<string> words;
    //list of random words that are used for current game
    public List<string> listOfWords;
    //sorted list of words; we'll sort words by letter count starting from words with most letters in order to improve alghorithm
    //that places all words into grid; we start with longest words while there are plenty empty spaces in grid
    public List<string> sortedListOfWords;

    //dictionary that holds the list of all words placed into grid along with all the grid positions that each word occupies
    //Dictionary key(string) is actuall word and Dictionary value (two-dimensional array) represent the list of all occupied grid cells
    //array will be dimensioned in this way [word.Length,2] - i.e. array[4,0] - this represent the x value(row in grid) of 5th letter of word and array [4,1] represents y value(column) of 5th letter in words
    public Dictionary<string, int[,]> finalDictionaryOfInsertedWords;

    //two-dimensional array that holds all cells
    public static GameObject[,] cells;
    public GameObject playPanel;
    public GameObject cellPrefab;
    public GameObject wordPrefab;
    public GameObject wordsPanel;
    public List<GameObject> wordsInWordsPanel;
    private List<char> characters = new List<char> { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };

    void Start()
    {
        instance = this;

        words = wordpool.text.Split(';').ToList();
        finalDictionaryOfInsertedWords = new Dictionary<string, int[,]>();
        CreateGrid();
    }

    /// <summary>
    /// this method will read the GridSize that player has selected and depending on that will create a grid with selected number of elements
    /// </summary>
    private void CreateGrid()
    {
        cells = new GameObject[GameManager.gridSize, GameManager.gridSize];
        GridLayoutGroup gridLayoutGroup = playPanel.GetComponent<GridLayoutGroup>();

        //checking the size of PlayPanel and accordingly changing the grid size in GridLayouGroup component
        Vector2 playPanelSize =new Vector2 (playPanel.GetComponent<RectTransform>().rect.width, playPanel.GetComponent<RectTransform>().rect.height);
        Vector2 cellSize = new Vector2(((playPanelSize.x-((GameManager.gridSize-1)*gridLayoutGroup.spacing.x)-4) / GameManager.gridSize), ((playPanelSize.y - ((GameManager.gridSize - 2) * gridLayoutGroup.spacing.y)-4) / GameManager.gridSize));
        gridLayoutGroup.cellSize = cellSize; 

        for (int i = 0; i < GameManager.gridSize; i++)
        {
            for (int j = 0; j < GameManager.gridSize; j++)
            {
                GameObject cell = Instantiate(cellPrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
                cell.transform.parent = playPanel.transform;
                cell.name = "Cell" + i.ToString() + j.ToString();
                cell.transform.localScale = new Vector3(1, 1, 1);
                //cell.GetComponent<BoxCollider2D>().size = cellSize;
                cell.GetComponent<CircleCollider2D>().radius = 225 / GameManager.gridSize;
                //set font size depending on grid size
                Vector2 tempCellTextSize = new Vector2((float)(0.5 * cellSize.x),(float) 0.5 * cellSize.y);
                cell.transform.FindChild("Text").GetComponent<RectTransform>().sizeDelta = tempCellTextSize;
                //adding instantiated cell into the two-dimensional array of all cells
                cells[i, j] = cell;
            }
        }
        InsertWords();
    }

    private void CreateListOfWords()
    {
        //creating random words and inserting them into lists that we'll use to place words on grid 
        //and compare the word written by player to check if he wrote correct word
        for (int i = 0; i <GameManager.totalNumberOfWords ; i++)
        {
            int rnd = Random.Range(0, words.Count);
            listOfWords.Add(words[rnd].ToUpper());
            GameManager.instance.generatedWords.Add(words[rnd].ToUpper());

            //adding randomly created word to words panel, so player can see what words he need to find
            GameObject word = Instantiate(wordPrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
            wordsInWordsPanel.Add(word);
            word.transform.parent = wordsPanel.transform;
            word.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            word.gameObject.name = "Word" + i.ToString();
            word.GetComponentInChildren<Text>().text = words[rnd].ToUpper();
            //word.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = wordsPanel.GetComponent<GridLayoutGroup>().cellSize;
        }

        //sorting list of random words, so the word with highest number of characters is first
        //we will start to inserting a words from this list starting with word with highest number of characters
        foreach (string s in SortByLength(listOfWords))
        {
            sortedListOfWords.Add(s);
        }
    }
    
    static IEnumerable<string> SortByLength(IEnumerable<string> e)
    {
        var sorted = from s in e
                     orderby s.Length descending
                     select s;
        return sorted;
    }

    private void InsertWords()
    {
        CreateListOfWords();

        //reseting the grid
        for (int k = 0; k < GameManager.gridSize; k++)
        {
            for (int l = 0; l < GameManager.gridSize; l++)
            {
                cells[k, l].GetComponentInChildren<Text>().text = "";
                cells[k, l].GetComponent<Cell>().occupied = false;
            }
        }
        int temp = sortedListOfWords.Count;

        //inserting words from listOfWords to grid
        for (int i = 0; i < temp; i++)
        {
            string s = sortedListOfWords[0].Trim();
            bool placed = false;

            List<int> shuffledListOfDirections = CreateShuffledListOfDirections();

            while (placed == false)
            {
                //generating two random numbers that will represent random cell position in grid
                //we are going to place word starting from this cell
                int row = Random.Range(0, GameManager.gridSize);
                int column = Random.Range(0, GameManager.gridSize);
                //print("random position is: " + row.ToString() + column.ToString());

                for (int k = 0; k < shuffledListOfDirections.Count; k++)
                {
                    placed = InsertWord(s.ToUpper(), row, column, shuffledListOfDirections[k]);
                    if (placed == true)
                    {
                        break;
                    }
                }               
            }
            
            listOfWords.RemoveAt(0);
            sortedListOfWords.RemoveAt(0);
        }

        /*
        //this is just for printing out inserted words for debuging purposes
        foreach (KeyValuePair<string, int[,]> pair in finalDictionaryOfInsertedWords)
        {
            Debug.Log("word in dictionary is: " + pair.Key);
            int[,] tr = pair.Value;
            for (int o = 0; o < tr.GetLength(0); o++)
            {
                Debug.Log("letter " + pair.Key[o] + " row is: " + pair.Value[o, 0] + " and column is: " + pair.Value[o, 1]);
            }
        }
        //
        */

        PopulateEmptySpacesInGrid();
    }

    private bool InsertWord(string word, int row, int column, int orientation)
     {
        //create two-dimensional array of all cell positions where word letters were inserted
        int[,] occupiedPositionsArray = new int[word.Length, 2];

        switch (orientation)
         {            
             case 1:             //horizontal left->right
                 {
                     if (column + word.Length >= GameManager.gridSize)
                     {
                         return false;
                     }
                     else
                     {
                         for (int j = 0; j < word.Length; j++)
                         {
                            if (cells[row, column + j].GetComponent<Cell>().occupied && (cells[row, column + j].GetComponent<Cell>().letter != word[j].ToString()))
                             {
                                 return false;
                             }
                         }
                         //place the word into grid starting from starting position (row+column) in given direction
                         for (int i = 0; i < word.Length; i++)
                         {
                            cells[row, column + i].GetComponentInChildren<Text>().text = word[i].ToString();
                            //cells[row, column + i].GetComponentInChildren<Text>().color = Color.red;
                            cells[row, column + i].GetComponent<Cell>().letter = word[i].ToString();
                            cells[row, column + i].GetComponent<Cell>().occupied = true;                            
                         }
                        //create two-dimensional array of all cell positions where word letters were inserted
                        //int[,] occupiedPositions = new int[word.Length,2];
                        for (int s = 0; s < word.Length; s++)
                        {
                            occupiedPositionsArray[s, 0] = row;
                            occupiedPositionsArray[s, 1] = column + s;                              
                        }
                        //store inserted word along with cell positions in grid that each letter occupies
                        finalDictionaryOfInsertedWords.Add(word, occupiedPositionsArray);

                        return true;
                     }
                 }
             case -1:            //horizontal right->left
                 {
                     if (column - word.Length >= GameManager.gridSize || column-word.Length<0)
                     {
                         return false;
                     }
                     else
                     {
                         for (int j = 0; j < word.Length; j++)
                         {
                             if (cells[row, column - j].GetComponent<Cell>().occupied && (cells[row, column - j].GetComponent<Cell>().letter != word[j].ToString()))
                             {
                                 return false;
                             }
                         }
                         for (int i = 0; i < word.Length; i++)
                         {
                            cells[row, column - i].GetComponentInChildren<Text>().text = word[i].ToString();
                            //cells[row, column - i].GetComponentInChildren<Text>().color = Color.red;
                            cells[row, column - i].GetComponent<Cell>().letter = word[i].ToString();
                            cells[row, column - i].GetComponent<Cell>().occupied = true;
                         }

                        //create two-dimensional array of all cell positions where word letters were inserted
                        //int[,] occupiedPositions = new int[word.Length, 2];
                        for (int s = 0; s < word.Length; s++)
                        {
                            occupiedPositionsArray[s, 0] = row;
                            occupiedPositionsArray[s, 1] = column - s;
                        }
                        //store inserted word along with cell positions in grid that each letter occupies
                        finalDictionaryOfInsertedWords.Add(word, occupiedPositionsArray);

                        return true;

                     }
                 }
             case 2:             //vertical down->up
                 {
                     if (row + word.Length >= GameManager.gridSize)
                     {
                         return false;
                     }
                     else
                     {
                         for (int j = 0; j < word.Length; j++)
                         {
                             if (cells[row+j, column].GetComponent<Cell>().occupied && (cells[row+j, column].GetComponent<Cell>().letter != word[j].ToString()))
                             {
                                 return false;
                             }
                         }
                         for (int i = 0; i < word.Length; i++)
                         {
                            cells[row + i, column].GetComponentInChildren<Text>().text = word[i].ToString();
                            //cells[row + i, column].GetComponentInChildren<Text>().color = Color.red;
                            cells[row + i, column].GetComponent<Cell>().letter = word[i].ToString();
                            cells[row + i, column].GetComponent<Cell>().occupied = true;
                         }

                        //create two-dimensional array of all cell positions where word letters were inserted
                        //int[,] occupiedPositions = new int[word.Length, 2];
                        for (int s = 0; s < word.Length; s++)
                        {
                            occupiedPositionsArray[s, 0] = row+s;
                            occupiedPositionsArray[s, 1] = column;
                        }
                        //store inserted word along with cell positions in grid that each letter occupies
                        finalDictionaryOfInsertedWords.Add(word, occupiedPositionsArray);

                        return true;

                     }
                 }
             case -2:           //vertical up->down
                 {
                     if (row - word.Length <= 0)
                     {
                         return false;
                     }
                     else
                     {
                         for (int j = 0; j < word.Length; j++)
                         {
                             if (cells[row - j, column].GetComponent<Cell>().occupied && (cells[row-j, column].GetComponent<Cell>().letter != word[j].ToString()))
                             {
                                 return false;
                             }
                         }
                         for (int i = 0; i < word.Length; i++)
                         {
                            cells[row - i, column].GetComponentInChildren<Text>().text = word[i].ToString();
                            //cells[row - i, column].GetComponentInChildren<Text>().color = Color.red;
                            cells[row - i, column].GetComponent<Cell>().letter = word[i].ToString();
                            cells[row - i, column].GetComponent<Cell>().occupied = true;
                         }

                        //create two-dimensional array of all cell positions where word letters were inserted
                        //int[,] occupiedPositions = new int[word.Length, 2];
                        for (int s = 0; s < word.Length; s++)
                        {
                            occupiedPositionsArray[s, 0] = row-s;
                            occupiedPositionsArray[s, 1] = column;
                        }
                        //store inserted word along with cell positions in grid that each letter occupies
                        finalDictionaryOfInsertedWords.Add(word, occupiedPositionsArray);

                        return true;
                     }
                 }
             case 3:             //diagonal down left->up right
                 {
                     if (row + word.Length >= GameManager.gridSize || column+word.Length>=GameManager.gridSize)
                     {
                         return false;
                     }
                     else
                     {
                         for (int j = 0; j < word.Length; j++)
                         {
                             if (cells[row + j, column+j].GetComponent<Cell>().occupied && (cells[row+j, column + j].GetComponent<Cell>().letter != word[j].ToString()))
                             {
                                 return false;
                             }
                         }
                         for (int i = 0; i < word.Length; i++)
                         {
                            cells[row + i, column + i].GetComponentInChildren<Text>().text = word[i].ToString();
                            //cells[row + i, column + i].GetComponentInChildren<Text>().color = Color.red;
                            cells[row + i, column + i].GetComponent<Cell>().letter = word[i].ToString();
                            cells[row + i, column + i].GetComponent<Cell>().occupied = true;
                         }

                        //create two-dimensional array of all cell positions where word letters were inserted
                        //int[,] occupiedPositions = new int[word.Length, 2];
                        for (int s = 0; s < word.Length; s++)
                        {
                            occupiedPositionsArray[s, 0] = row+s;
                            occupiedPositionsArray[s, 1] = column + s;
                        }
                        //store inserted word along with cell positions in grid that each letter occupies
                        finalDictionaryOfInsertedWords.Add(word, occupiedPositionsArray);

                        return true;
                     }
                 }
             case -3:            //diagonal down right->up left
                 {
                     if (row - word.Length <= 0 || column - word.Length <= 0)
                     {
                         return false;
                     }
                     else
                     {
                         for (int j = 0; j < word.Length; j++)
                         {
                             if (cells[row - j, column - j].GetComponent<Cell>().occupied && (cells[row-j, column - j].GetComponent<Cell>().letter != word[j].ToString()))
                             {
                                 return false;
                             }
                         }
                         for (int i = 0; i < word.Length; i++)
                         {
                            cells[row - i, column - i].GetComponentInChildren<Text>().text = word[i].ToString();
                            //cells[row - i, column - i].GetComponentInChildren<Text>().color = Color.red;
                            cells[row - i, column - i].GetComponent<Cell>().letter = word[i].ToString();
                            cells[row - i, column - i].GetComponent<Cell>().occupied = true;
                         }

                        //create two-dimensional array of all cell positions where word letters were inserted
                        //int[,] occupiedPositions = new int[word.Length, 2];
                        for (int s = 0; s < word.Length; s++)
                        {
                            occupiedPositionsArray[s, 0] = row-s;
                            occupiedPositionsArray[s, 1] = column - s;
                        }
                        //store inserted word along with cell positions in grid that each letter occupies
                        finalDictionaryOfInsertedWords.Add(word, occupiedPositionsArray);

                        return true;
                     }
                 }
             case 4:             //diagonal up left->down right
                 {
                     if (row - word.Length <= 0 || column + word.Length >= GameManager.gridSize)
                     {
                         return false;
                     }
                     else
                     {
                         for (int j = 0; j < word.Length; j++)
                         {
                             if (cells[row - j, column + j].GetComponent<Cell>().occupied && (cells[row-j, column + j].GetComponent<Cell>().letter != word[j].ToString()))
                             {
                                 return false;
                             }
                         }
                         for (int i = 0; i < word.Length; i++)
                         {
                            cells[row - i, column + i].GetComponentInChildren<Text>().text = word[i].ToString();
                            //cells[row - i, column + i].GetComponentInChildren<Text>().color = Color.red;
                            cells[row - i, column + i].GetComponent<Cell>().letter = word[i].ToString();
                            cells[row - i, column + i].GetComponent<Cell>().occupied = true;
                         }

                        //create two-dimensional array of all cell positions where word letters were inserted
                        //int[,] occupiedPositions = new int[word.Length, 2];
                        for (int s = 0; s < word.Length; s++)
                        {
                            occupiedPositionsArray[s, 0] = row-s;
                            occupiedPositionsArray[s, 1] = column + s;
                        }
                        //store inserted word along with cell positions in grid that each letter occupies
                        finalDictionaryOfInsertedWords.Add(word, occupiedPositionsArray);

                        return true;
                     }
                 }
             case -4:             //diagonal up right->down left
                 {
                     if (row - word.Length <= 0 || column - word.Length <= 0)
                     {
                         return false;
                     }
                     else
                     {
                         for (int j = 0; j < word.Length; j++)
                         {
                             if (cells[row - j, column - j].GetComponent<Cell>().occupied && (cells[row-j, column - j].GetComponent<Cell>().letter != word[j].ToString()))
                             {
                                 return false;
                             }
                         }
                         for (int i = 0; i < word.Length; i++)
                         {
                            cells[row - i, column - i].GetComponentInChildren<Text>().text = word[i].ToString();
                            //cells[row - i, column - i].GetComponentInChildren<Text>().color = Color.red;
                            cells[row - i, column - i].GetComponent<Cell>().letter = word[i].ToString();
                            cells[row - i, column - i].GetComponent<Cell>().occupied = true;
                         }

                        //create two-dimensional array of all cell positions where word letters were inserted
                        //int[,] occupiedPositions = new int[word.Length, 2];
                        for (int s = 0; s < word.Length; s++)
                        {
                            occupiedPositionsArray[s, 0] = row-s;
                            occupiedPositionsArray[s, 1] = column - s;
                        }
                        //store inserted word along with cell positions in grid that each letter occupies
                        finalDictionaryOfInsertedWords.Add(word, occupiedPositionsArray);

                        return true;
                     }
                 }
         }

         return true;
     }

    private void PopulateEmptySpacesInGrid()
    {
        for (int i = 0; i < GameManager.gridSize; i++)
        {
            for (int j = 0; j < GameManager.gridSize; j++)
            {
                if (cells[i, j].GetComponentInChildren<Text>().text == "")
                {
                    int temp = Random.Range(0, characters.Count);
                    cells[i, j].GetComponent<Cell>().occupied = true;
                    cells[i, j].GetComponent<Cell>().letter = characters[temp].ToString();
                    cells[i, j].GetComponentInChildren<Text>().text = characters[temp].ToString() ;
                }
            }
        }
    }

    private List<int> CreateShuffledListOfDirections()
    {
        //creating a shuffled list of all possible directions for placing a word into grid
        List<int> directionsList = new List<int> { -4, -3, -2, -1, 1, 2, 3, 4 };

        //it's better to try to put words first diagonally; otherwise, most of the words are placed either horizontally either vertically
        //to improve the algorithm of placing words in grid, first four places(0-3; out of eight) in shuffled list will be shuffled list of all diagonal directions
        //the other four places (4-7) will be occupied with shuffled horizontal and vertical directions
        List<int> diagonalDirections = new List<int> { -4,4,-3,3};

        //shuffle diagonal directions
        for (int k = 0; k < diagonalDirections.Count; k++)
        {
            int temp = diagonalDirections[k];
            int randomIndex = Random.Range(k, diagonalDirections.Count);
            diagonalDirections[k] = diagonalDirections[randomIndex];
            diagonalDirections[randomIndex] = temp;
        }

        List<int> otherDirections = new List<int> { -2,2,-1,1};

        //shuffle other directions
        for (int j = 0; j < otherDirections.Count; j++)
        {
            int temp2 = otherDirections[j];
            int randomIndex2 = Random.Range(j, otherDirections.Count);
            otherDirections[j] = otherDirections[randomIndex2];
            otherDirections[randomIndex2] = temp2;
        }

        //now add two shuffled list into directions list
        for (int l = 0; l < 4; l++)
        {
            directionsList[l] = diagonalDirections[l];
            directionsList[l + 4] = otherDirections[l];
        }
        return directionsList;
    } 

    public void ResetLevel()
    {
        SceneManager.LoadScene("PlayScreen");
        /*GameObject[] temp = GameObject.FindGameObjectsWithTag("Line");
        for (int i = 0; i < temp.Length; i++)
        {
            Destroy(temp[i]);
        }
        InsertWords();*/
    }
}
