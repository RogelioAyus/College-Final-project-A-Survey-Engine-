using UnityEngine;
using TMPro; // Use UnityEngine.UI if using normal Text
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using static ForCreatePanel;

public class PreFabScript : MonoBehaviour
{
    public Image[] stars; // Assign 5 star Image objects in the Inspector
    public Sprite emptyStar;
    public Sprite halfStar;
    public Sprite fullStar;

    public Image mapSet;

    public TextMeshProUGUI topicText;
    public TextMeshProUGUI personText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI textEmail;



    public void SetTheText(string topicString, string personString, string desString, string email, float rating, string longitude, string latitude)
    {
        topicText.text = topicString;
        personText.text = personString;
        descriptionText.text = desString;
        textEmail.text = email;
        SetRating(rating);

        string url = $"https://api.mapbox.com/styles/v1/mapbox/streets-v11/static/pin-l+ff0000({longitude},{latitude})/{longitude},{latitude},14/242x242?access_token=" + accessToken;
        StartCoroutine(LoadMap(url));
        StartCoroutine(GetAddressFromCoordinates(longitude, latitude));
    }
    public void SetRating(float ratingOutOf10)
    {
        float starRating = ratingOutOf10 / 2f; // Convert to 0–5

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
    public RawImage mapImage;

    // Replace with your own coordinates and access token
    float latitude;
    float longitude;
    private string accessToken = "pk.eyJ1Ijoicm9nZWxpb2F5dXMiLCJhIjoiY21hZ3M2d2ppMDRkdzJtcHk2a2s2Zmt4YyJ9.gc5tfkTPgn25LCnhOOK8vA";
    void Start()
    {
        
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
            mapImage.texture = texture;
        }
    }

    public TMP_Text locationDisplay;
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
    }
