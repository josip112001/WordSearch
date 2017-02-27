using UnityEngine;
using System.Collections;

public class PlayerPreferences : MonoBehaviour {

    public static PlayerPreferences instance = null;

    public delegate void CoinsAdded();
    public static event CoinsAdded OnCoinsAdded;

    public static bool facebookIsLoggedIn;
    public static string userName;
    public static Sprite profilePicture;
    public static int stars;
    public static int coins;
    public static int hints;
        

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
        LoadPreferences();

        //
        hints = 10;
        //
    }

    void OnApplicationQuit()
    {
        SavePreferences();
    }

    public void AddCoins(int value)
    {
        coins += value;

        if (OnCoinsAdded != null)
        {
            OnCoinsAdded();
        }
    }

    public void SavePreferences()
    {
        PlayerPrefs.SetInt("FacebookLoggedIn", ConvertToInt(facebookIsLoggedIn));
        PlayerPrefs.SetInt("Stars", stars);
        PlayerPrefs.SetInt("Coins", coins);
        PlayerPrefs.SetInt("Hints", hints);
    }

    public void LoadPreferences()
    {
        facebookIsLoggedIn = ConvertToBool(PlayerPrefs.GetInt("FacebookLoggedIn"));
        stars = PlayerPrefs.GetInt("Stars");
        coins = PlayerPrefs.GetInt("Coins");
        hints = PlayerPrefs.GetInt("Hints");
    }

    #region Helper Methods
    /// <summary>
    /// converting int value saved in player preferences to bool value
    /// bool values cannot be saved in player preferences, so they must be converted to some other type 
    /// 1-yes; 0-no
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    private bool ConvertToBool(int value)
    {
        if (value == 1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// converting bool value to int value so we can save it in player preferences
    /// bool's cannot be saved to player preferences
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    private int ConvertToInt(bool value)
    {
        if (value)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }
    #endregion
}
