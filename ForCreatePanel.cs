using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ForCreatePanel : MonoBehaviour
{
    public string usernameFetch;
    public string passwordFetch;
    public string emailFetch;

    public SendGetRequest TakeCareUser;
    public Image[] stars;

    public Sprite emptyStar;
    public Sprite halfStar;
    public Sprite fullStar;

    public TMP_InputField inputTopic;
    public TMP_InputField inputName;
    public TMP_InputField inputDescription;
    public TMP_InputField inputEmail;
    public Slider inputSlider;

    public TMP_InputField inputCoordinate;
    string lat;
    string lon;
    public TMP_Text locationDisplay;

    public RawImage webRawImage;

    public void updateFetching(string a, string b, string c)
    {
        usernameFetch = a;
        passwordFetch = b;
        emailFetch = c;
    }
    void Start()
    {
        updateSlider(0);
    }

    // Update is called once per frame

    [System.Serializable]
    public class Feature
    {
        public string place_name;
    }

    [System.Serializable]
    public class GeocodeResponse
    {
        public Feature[] features;
    }
    IEnumerator GetAddressFromCoordinates(string longitude, string latitude)
    {
        string accessToken = "pk.eyJ1Ijoicm9nZWxpb2F5dXMiLCJhIjoiY21hZ3M2d2ppMDRkdzJtcHk2a2s2Zmt4YyJ9.gc5tfkTPgn25LCnhOOK8vA";
        string url = $"https://api.mapbox.com/geocoding/v5/mapbox.places/{longitude},{latitude}.json?access_token={accessToken}";

        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Reverse geocoding failed: " + www.error);
        }
        else
        {
            string json = www.downloadHandler.text;
            GeocodeResponse result = JsonUtility.FromJson<GeocodeResponse>(json);

            if (result.features != null && result.features.Length > 0)
            {
                locationDisplay.text = result.features[0].place_name;
            }
            else
            {
                locationDisplay.text = "No address found.";
            }


        }
    }


    public void clickLoadMap()
    {
        SplitCoordinates();
        string accessToken = "pk.eyJ1Ijoicm9nZWxpb2F5dXMiLCJhIjoiY21hZ3M2d2ppMDRkdzJtcHk2a2s2Zmt4YyJ9.gc5tfkTPgn25LCnhOOK8vA";
        string url = $"https://api.mapbox.com/styles/v1/mapbox/streets-v11/static/pin-l+ff0000(125.1316,8.0036)/{lon},{lat},14/700x700?access_token=" + accessToken;
        StartCoroutine(LoadMap(url));
        StartCoroutine(GetAddressFromCoordinates(lon,lat));
    }

    public void SplitCoordinates()
    {
        string input = inputCoordinate.text.Trim();

        // Split by comma
        string[] parts = input.Split(',');

        if (parts.Length == 2)
        {
            lat = parts[0].Trim();
            lon = parts[1].Trim();

        }
        else
        {
            Debug.LogWarning("Invalid format! Use: latitude, longitude");
        }
    }

    IEnumerator LoadMap(string url)
    {

        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Map load failed: " + www.error);
        }
        else
        {
            Texture2D texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
            webRawImage.texture = texture;
        }
    }

    public void cancelClick()
    {
        clearData();
        
    }

    public void clearData()
    {
        inputTopic.text = "";
        inputDescription.text = "";
        //inputName.text = "";
        //inputEmail.text = "";
        inputSlider.value = 0;
        gameObject.SetActive(false);
        inputCoordinate.text = "";
    }

    public void acceptClick()
    {
        SendData(inputName.text, inputDescription.text, inputTopic.text, inputEmail.text, inputSlider.value, lon, lat);
        clearData();
        TakeCareUser.updateUI();

    }

    public void updateSlider(float value)
    {
        float starRating = value / 2f; // Convert to 0–5

        for (int i = 0; i < stars.Length; i++)
        {
            if (starRating >= i + 1)
            {
                stars[i].sprite = fullStar;
            }
            else if (starRating >= i + 0.5f)
            {
                stars[i].sprite = halfStar;
            }
            else
            {
                stars[i].sprite = emptyStar;
            }
        }
    }

    public void SendData(string name, string description, string topic, string email, float rating, string lon1, string lat1)
    {
        // Prepare the URL with query parameters
        string url = "http://"+ TakeCareUser.stringIP +"/project1/add.php";
        url += "?name=" + UnityWebRequest.EscapeURL(name);
        url += "&description=" + UnityWebRequest.EscapeURL(description);
        url += "&topic=" + UnityWebRequest.EscapeURL(topic);
        url += "&email=" + UnityWebRequest.EscapeURL(email);
        url += "&rate=" + rating;
        url += "&long=" + lon1;
        url += "&lati=" + lat1;
        Debug.Log(url);

        // Start the coroutine to send the request
        StartCoroutine(SendRequest(url));
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
    }

}
