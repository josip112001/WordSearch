using UnityEngine;
using UnityEngine.UI;
using Facebook.Unity;
using System.Collections;
using System.Collections.Generic;

public class SocialManager : MonoBehaviour {

    public static SocialManager instance = null;

    public delegate void FacebookLoggedIn();
    public static event FacebookLoggedIn OnFacebookLoggedIn;


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

        if (!FB.IsInitialized)
        {
            // Initialize the Facebook SDK
            FB.Init(InitCallback, OnHideUnity);
        }
        else
        {
            // Already initialized, signal an app activation App Event
            FB.ActivateApp();
        }
    }

    private void InitCallback()
    {
        if (FB.IsInitialized)
        {
            // Signal an app activation App Event
            FB.ActivateApp();
        }
        else {
            Debug.Log("Failed to Initialize the Facebook SDK");
        }
    }

    private void OnHideUnity(bool isGameShown)
    {
        if (!isGameShown)
        {
            // Pause the game - we will need to hide
            Time.timeScale = 0;
        }
        else {
            // Resume the game - we're getting focus again
            Time.timeScale = 1;
        }
    }

    public void FacebookLogin()
    {
        var perms = new List<string>() { "public_profile", "email", "user_friends" };
        FB.LogInWithReadPermissions(perms, AuthCallback);
    }

    private void AuthCallback(ILoginResult result)
    {
        if (FB.IsLoggedIn)
        {
            // AccessToken class will have session details
            var aToken = Facebook.Unity.AccessToken.CurrentAccessToken;
            // Print current access token's User ID
            Debug.Log(aToken.UserId);
            // Print current access token's granted permissions
            foreach (string perm in aToken.Permissions)
            {
                Debug.Log(perm);
            }

            //show the username
            FB.API("/me?fields=first_name", HttpMethod.GET, SetUsername);
            FB.API("/me/picture?type=square&height=128&width=128", HttpMethod.GET, SetProfilePicture);

            

        }
        else {
            Debug.Log(result.Error);
        }
    }

    private void SetUsername(IResult result)
    {
        if (result.Error == null)
        {
            PlayerPreferences.userName = result.ResultDictionary["first_name"].ToString();
        }
        else
        {           
            Debug.Log(result.Error);
        }
    }

    private void SetProfilePicture(IGraphResult result)
    {
        if (result.Texture != null)
        {
            PlayerPreferences.profilePicture= Sprite.Create(result.Texture, new Rect(0, 0, 128, 128), new Vector2());
        }

        //trigger facebookLoggedIn event
        if (OnFacebookLoggedIn != null)
        {
            PlayerPreferences.facebookIsLoggedIn = true;
            OnFacebookLoggedIn();
        }
    }

    /*
    access token: EAADZBhWNvmLEBAMeeNb8pQxxlwGN91pWZB5wKeJOPjZCpAAmEdTCnRW2DBd3CkaeBAR9k6GK7zZADgxe5gcgo5vZBYv8nSx7gdZBfCAkJ9W8SZCC1tQQZBMmms3c2mq9ujXUNZB2I7fsL9vejy3RRtJT7WpZAjPISDZAG4ZD
    */
}

