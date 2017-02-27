using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TopUI : MonoBehaviour {

    public static TopUI instance = null;

    public GameObject profilePicture;
    public Text userName;
    public Text stars;
    public Text coins;
    public Text hints;

    private float topUIWidth;
    private float topUIHeight;

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

	void Start ()
    {
        Debug.Log("width of TOpUI is: " + GetComponent<RectTransform>().rect.width);

        //calculte the size of each object in TopUI depending on height/width of TopUI
        topUIWidth = GetComponent<RectTransform>().rect.width;
        topUIHeight = GetComponent<RectTransform>().rect.height;
        RefreshTopUI();
	}

    public void RefreshTopUI()
    {
        userName.text = PlayerPreferences.userName;
        stars.text = PlayerPreferences.stars.ToString();
        coins.text = PlayerPreferences.coins.ToString();
        hints.text = PlayerPreferences.hints.ToString();
    }
}
