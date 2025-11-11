using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using TMPro;


public class SendGetRequest : MonoBehaviour
{
    public GameObject PanelCreateUser;
    public string stringIP;
    public TMP_Dropdown filterObj;
    public int filterOpt;
    public TMP_InputField ipInput;


    void Start()
    {
        stringIP = "localhost";
        //StartCoroutine(SendData("Jeter", "{\"result\":\"yes\",\"Self\":\"no\"}","asdhefjbrhuwiresgjhbd54jnkefduhebtk"));
        StartCoroutine(FetchUsers(""));
    }

    public TMP_InputField searchBar;
    public void searchType()
    {
        DeleteAllPrefabs();
        StartCoroutine(FetchUsers(searchBar.text));
    }

    public void filterChanged(int index)
    {
        Debug.Log(filterObj.value);
        filterOpt = filterObj.value;
        updateUI();
    }

    public void IPChangingTrue()
    {
        stringIP = ipInput.text;
        Debug.Log("Changed: " + stringIP);
        
    }

    public void refreshButton()
    {
        updateUI();
    }
    public void updateUI()
    {
        
        searchType();

    }

    IEnumerator FetchUsers(string searchQuery)
    {
        UnityWebRequest www = UnityWebRequest.Get("http://" + stringIP + "/project1/fetch.php");
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Fetch Error: " + www.error);
        }
        else
        {
            string rawJson = www.downloadHandler.text;
            string wrappedJson = "{\"users\":" + rawJson + "}"; // Wrap array for JsonUtility

            UserList userList = JsonUtility.FromJson<UserList>(wrappedJson);

            if (filterOpt == 0)
            {
                userList.users.Sort((x, y) => x.name.CompareTo(y.name));
            }
            else if (filterOpt == 1)
            {
                userList.users.Sort((x, y) => y.name.CompareTo(x.name));
            }
            else if (filterOpt == 2)
            {
                userList.users.Sort((x, y) => float.Parse(y.rating).CompareTo(float.Parse(x.rating)));
            }
            List<User> filteredUsers = new List<User>();
            foreach (User u in userList.users)
            {
                if (u.name.ToLower().Contains(searchQuery.ToLower()))
                {
                    filteredUsers.Add(u);
                }
            }
            foreach (User u in filteredUsers)
            {
                Debug.Log(u.id + " " + u.name + " " + u.topic + " " + u.description + " " + u.email + " " + u.rating);
                float a = float.Parse(u.rating);
                instant_prefabs(u.topic,u.name, u.description, u.email,a,u.longitude,u.latitude);
            }
        }
    }

    public Transform contextTarget;
    public GameObject prefabThing;

    public void instant_prefabs(string name, string topic, string desc, string email,float rate, string longitude, string latitude)
    {
        GameObject newObj = Instantiate(prefabThing, contextTarget);

        PreFabScript preFabScript = newObj.GetComponent<PreFabScript>();

        // Assign text and float value
        preFabScript.SetTheText(name,topic,desc,email,rate,longitude,latitude);
    }
    public void UpdateUser(int id, string name, string topic, string description, string rating, string email)
    {
        StartCoroutine(SendUpdateRequest(id, name, topic, description, rating, email));
    }

    IEnumerator SendUpdateRequest(int id, string name, string topic, string description, string rating, string email)
    {
        string url = "http://" + stringIP +"/project1/update.php" +
                     "?id=" + UnityWebRequest.EscapeURL(id.ToString()) +
                     "&name=" + UnityWebRequest.EscapeURL(name) +
                     "&topic=" + UnityWebRequest.EscapeURL(topic) +
                     "&description=" + UnityWebRequest.EscapeURL(description) +
                     "&rating=" + UnityWebRequest.EscapeURL(rating) +
                     "&email=" + UnityWebRequest.EscapeURL(email);

        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Success: " + www.downloadHandler.text);
        }
        else
        {
            Debug.Log("Error: " + www.error);
        }
    }
    public void DeleteUser(int id)
    {
        StartCoroutine(SendDeleteRequest(id));
    }

    IEnumerator SendDeleteRequest(int id)
    {
        string url = "http://" + stringIP + "/project1/delete.php?id=" + id;

        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Success: " + www.downloadHandler.text);
        }
        else
        {
            Debug.Log("Error: " + www.error);
        }
    }

    public void clickCreate()
    {
        PanelCreateUser.SetActive(true);
    }

    public GameObject contentOfScrollView;  // Assign your ScrollView's Content GameObject here

    // Function to delete all child prefabs inside the content
    public void DeleteAllPrefabs()
    {
        // Loop through all children of the content object
        foreach (Transform child in contentOfScrollView.transform)
        {
            Destroy(child.gameObject);  // Destroy the child object
        }
    }
}

[System.Serializable]
public class User
{
    public int id;
    public string name;
    public string topic;       // This is a JSON string (e.g. {"gold":100})
    public string description;
    public string email;
    public string rating;
    public string longitude;
    public string latitude;
}

[System.Serializable]
public class UserList
{
    public List<User> users;
}