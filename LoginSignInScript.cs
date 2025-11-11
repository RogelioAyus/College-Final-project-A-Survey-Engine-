using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;

public class LoginSignInScript : MonoBehaviour
{

    public ForCreatePanel sendDataUser;
    public SendGetRequest TakeCareUser;
    public TMP_InputField usernameLogin;
    public TMP_InputField passwordLogin;

    public TMP_InputField usernameSignin;
    public TMP_InputField emailSignin;
    public TMP_InputField passwordSignin;

    public GameObject PanelSignin;
    public GameObject PanelLogin;

    public TMP_InputField nameCreate;
    public TMP_InputField emailCreate;

    string usernameFetch;
    string passwordFetch;

    public TMP_Text textDisplay;

    public void clickToSign()
    {
        PanelSignin.SetActive(true);
    }
    public void clickCancelSignin()
    {
        PanelSignin.SetActive(false);
    }

    public void clickSignIn()
    {
        StartCoroutine(SendRequest("http://"+ TakeCareUser.stringIP +"/project1/addLogin.php?username="+ usernameSignin.text +"&password=" + passwordSignin.text + "&email=" + emailSignin.text));
    }

    public void clickLogIn()
    {
        StartCoroutine(FetchUsers("http://" + TakeCareUser.stringIP + "/project1/fetchLogin.php"));
    }



    // Coroutine to send the GET request
    private IEnumerator SendRequest(string url)
    {
        UnityWebRequest request = UnityWebRequest.Get(url);

        // Send the request and wait for the response
        yield return request.SendWebRequest();

        // Check for errors in the response
        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Data sent successfully: " + request.downloadHandler.text);
        }
        else
        {
            Debug.Log("Error sending data: " + request.error);
        }
        usernameSignin.text = "";
        passwordSignin.text = "";
        emailSignin.text = "";
        PanelSignin.SetActive(false);
    }


    IEnumerator FetchUsers(string searchQuery)
    {
        bool ifFound = false;
        UnityWebRequest www = UnityWebRequest.Get("http://" + TakeCareUser.stringIP + "/project1/fetchLogin.php");
        yield return www.SendWebRequest();
        
        string json = www.downloadHandler.text;
        UserListWrapper userList = JsonUtility.FromJson<UserListWrapper>(json);

        // Access data
        foreach (UserObj user in userList.users)
        {
            if (user.username == usernameLogin.text)
            {
                ifFound = true;
                if (user.password == passwordLogin.text)
                {
                    textDisplay.text = "Creditential Success";
                    sendDataUser.updateFetching(user.username, user.password, user.email);
                    PanelLogin.SetActive(false);
                    nameCreate.text = user.username;
                    emailCreate.text = user.email;
                }
            }
        }
        if (ifFound == true)
        {
            textDisplay.text = "Creditential invalid";
        }
        else
        {
            textDisplay.text = "Creditential invalid.";
        }
        usernameLogin.text = "";
        passwordLogin.text = "";
    }
}

[System.Serializable]
public class UserObj
{
    public string username;
    public string password;
    public string email;

}

[System.Serializable]
public class UserListWrapper
{
    public UserObj[] users;
}