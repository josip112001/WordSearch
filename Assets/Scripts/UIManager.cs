using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class UIManager : MonoBehaviour {

    public static UIManager instance=null;

    //main screen buttons
    public Button questsButton;
    public Button quickPlayButton;
    public Button dailyPuzzleButton;
    public Text whatHappednedText;
    public Button settingsButton;
    public Button storeButton;
    public Button facebookLoginButton;
    public GameObject facebookLoginReward;

    public GameObject TopUIPrefab;
    public GameObject buttons;                  //empty object that holds all buttons on main screen
    public GameObject settingsPanelPrefab;
    public GameObject storePanelPrefab;
    public GameObject questsPanel;

    void OnEnable()
    {
        SocialManager.OnFacebookLoggedIn += OnFacebookLoggedIn;
        SceneManager.sceneLoaded += OnSceneWasLoaded;
        PlayerPreferences.OnCoinsAdded += OnCoinsAdded;
    }

    void OnDisable()
    {
        SocialManager.OnFacebookLoggedIn -= OnFacebookLoggedIn;
        SceneManager.sceneLoaded += OnSceneWasLoaded;

    }

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }        
    }

    void Start()
    {
        questsButton.onClick.AddListener(() => questsPanel.GetComponent<QuestsPanel>().ShowQuestsPanel());
        quickPlayButton.onClick.AddListener(() => SceneManager.LoadScene("PlayScreen"));
        dailyPuzzleButton.onClick.AddListener(() => SceneManager.LoadScene("PlayScreen"));
    }

    void OnSceneWasLoaded(Scene scene, LoadSceneMode mode)
    {
        /*GameObject TopUI = Instantiate(TopUIPrefab, Vector3.zero, Quaternion.identity) as GameObject;
        TopUI.transform.SetParent(GameObject.Find("Canvas").transform);
        TopUI.GetComponent<RectTransform>().offsetMax = new Vector2(2.0f,2.0f);
        TopUI.GetComponent<RectTransform>().offsetMin = new Vector2(-2.0f,1.0f);
        TopUI.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);*/

        if (scene.name == "MainScreen")
        {
            //check if player is logged in with facebook and then show/hide facebook login button
            if (PlayerPreferences.facebookIsLoggedIn)
            {
                facebookLoginButton.gameObject.SetActive(false);
                facebookLoginReward.gameObject.SetActive(false);
            }

            string sufix;
            if (System.DateTime.Now.ToString("dd") == "1")
            {
                sufix = "ST";
            }
            else if (System.DateTime.Now.ToString("dd") == "2")
            {
                sufix = "ND";
            }
            else
            {
                sufix = "TH";
            }
            whatHappednedText.text = "WHAT HAPPENED ON " + System.DateTime.Now.ToString("MMMM" + ", " + "dd") + sufix;
        }
    }

    public void ShowSettingsPanel(bool value)
    {
        GameObject settingsPanel=null;
        if (value == true)
        {
            if (SceneManager.GetActiveScene().name == "MainScreen")
            {
                buttons.SetActive(false);
            }
            settingsPanel = Instantiate(settingsPanelPrefab, Vector3.zero, Quaternion.identity) as GameObject;
            settingsPanel.transform.parent = GameObject.Find("Canvas").transform;
            settingsPanel.GetComponent<RectTransform>().offsetMax = new Vector2(-25.0f, -100.0f);
            settingsPanel.GetComponent<RectTransform>().offsetMin = new Vector2(25.0f, 50.0f);
            settingsPanel.gameObject.name = "SettingsPanel";
            settingsPanel.GetComponentInChildren<Button>().onClick.AddListener(() => ShowSettingsPanel(false));
        }
        else
        {
            if (SceneManager.GetActiveScene().name == "MainScreen")
            {
                buttons.SetActive(true);
            }        
        }
    }

    public void ShowStorePanel(bool value)
    {
        GameObject storePanel = null;
        if (value == true)
        {
            if (SceneManager.GetActiveScene().name == "MainScreen")
            {
                buttons.SetActive(false);
            }
            storePanel = Instantiate(storePanelPrefab, Vector3.zero, Quaternion.identity) as GameObject;
            storePanel.transform.parent = GameObject.Find("Canvas").transform;
            storePanel.GetComponent<RectTransform>().offsetMax = new Vector2(-25.0f, -100.0f);
            storePanel.GetComponent<RectTransform>().offsetMin = new Vector2(25.0f, 50.0f);
            storePanel.gameObject.name = "StorePanel";
            storePanel.GetComponentInChildren<Button>().onClick.AddListener(() => ShowStorePanel(false));
        }
        else
        {
            if (SceneManager.GetActiveScene().name == "MainScreen")
            {
                buttons.SetActive(true);
            }
        }
    }

    public void FacebookLogin()
    {
        SocialManager.instance.FacebookLogin();
    }

    private void OnFacebookLoggedIn()
    {
        //update username and profile picture
        TopUI.instance.userName.text = PlayerPreferences.userName;
        TopUI.instance.profilePicture.GetComponent<Image>().overrideSprite = PlayerPreferences.profilePicture;

        //reward player with coins
        PlayerPreferences.instance.AddCoins(50);

        facebookLoginButton.GetComponent<Button>().interactable = false;
        facebookLoginReward.SetActive(false);
    }

    private void OnCoinsAdded()
    {
        TopUI.instance.coins.text = PlayerPreferences.coins.ToString();
    }
}
