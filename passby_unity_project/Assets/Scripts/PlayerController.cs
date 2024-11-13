using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Newtonsoft.Json;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace PassBy
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField]
        private GameObject notificationControllerObject;
        private int id;
        private Passerby Passerby;
        private List<Passerby> passerbyCollection;
        UnityEvent nearbyPlayerFound;

        void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
        private void Start()
        {
            id = 0;
            Passerby = new Passerby();
            passerbyCollection = new List<Passerby>();
            nearbyPlayerFound = new UnityEvent();
            nearbyPlayerFound.AddListener(onNearbyPlayerFound);
        }

        public int GetId() { return id; }
        public string GetName() { return Passerby.Name; }
        public void SetName(TMP_InputField inputField)
        {
            Passerby.Name = inputField.text;
        }
        public List<Passerby> GetPassersby()
        {
            return passerbyCollection;
        }

        public void StartGeneratePlayerId()
        {
            StartCoroutine(GeneratePlayerId());
        }

        public IEnumerator GeneratePlayerId() // Send a request to the web server for a unique player id
        {
            // Create dictionaries
            Dictionary<string, float> location = new Dictionary<string, float> {
                { "latitude", Input.location.lastData.latitude },
                { "longitude", Input.location.lastData.longitude }
            };

            string bodyType = GameObject.Find("Body").GetComponent<SpriteRenderer>().sprite.name; // need a better way to get these values...
            string leftHandColour = GameObject.Find("Left Hand").GetComponent<SpriteRenderer>().sprite.name;
            string rightHandColour = GameObject.Find("Right Hand").GetComponent<SpriteRenderer>().sprite.name;

            Passerby.Avatar.BodyType = bodyType;
            Passerby.Avatar.LeftHandColour = leftHandColour;
            Passerby.Avatar.RightHandColour = rightHandColour;

            // Create JSON data
            Dictionary<string, object> playerData = new Dictionary<string, object> {
                { "Name", Passerby.Name },
                { "Avatar", Passerby.Avatar },
                { "location", location }
            };

            string playerJsonData = JsonConvert.SerializeObject(playerData);

            // Send POST request to get unique id
            string serverUrl = "http://10.86.73.162:5000"; // 10.86.73.162 // This needs to be some static ip, or an ip that can be calculated to guarantee a server connection...
            string contentType = "application/json";
            using (UnityWebRequest request = UnityWebRequest.Post($"{serverUrl}/generate_player_id", playerJsonData, contentType))
            {
                yield return request.SendWebRequest();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError("Error getting player id: " + request.error);
                }
                else
                {
                    string jsonResponse = request.downloadHandler.text;
                    Dictionary<string, int> playerIdDict = JsonConvert.DeserializeObject<Dictionary<string, int>>(jsonResponse);
                    id = playerIdDict["player_id"];
                    Debug.Log($"Player id generated successfully (value is now {id}).");
                }
            }
        }

        public void StartGetNearbyPlayersPeriodically()
        {
            StartCoroutine(GetNearbyPlayersPeriodically());
        }

        public IEnumerator GetNearbyPlayersPeriodically()
        {
            while (true)
            {
                yield return StartCoroutine(GetNearbyPlayers());
                yield return new WaitForSecondsRealtime(7.5f);
            }
        }

        public IEnumerator GetNearbyPlayers() // Send a request to the web server for nearby players
        {
            // Create JSON data
            Dictionary<string, object> playerData = new Dictionary<string, object> {
                { "player_id", id },
                { "latitude", Input.location.lastData.latitude },
                { "longitude", Input.location.lastData.longitude }
            };

            string playerJsonData = JsonConvert.SerializeObject(playerData);

            // Send POST request to get nearby players
            string serverUrl = "http://10.86.73.162:5000"; // 10.86.73.162 // This needs to be some static ip, or an ip that can be calculated to guarantee a server connection...
            string contentType = "application/json";
            using (UnityWebRequest request = UnityWebRequest.Post($"{serverUrl}/get_nearby_players", playerJsonData, contentType))
            {
                yield return request.SendWebRequest();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError("Error getting nearby players: " + request.error);
                }
                else
                {
                    string jsonResponse = request.downloadHandler.text;
                    Debug.Log("Nearby players: " + jsonResponse);
                    Dictionary<int, Passerby> nearbyPlayers = JsonConvert.DeserializeObject<Dictionary<int, Passerby>>(jsonResponse);
                    if (nearbyPlayers.Count > 0)
                    {
                        Debug.Log("Nearby players successfully found.");
                        NotificationController notificationController = notificationControllerObject.GetComponent<NotificationController>();
                        notificationController.SendPassbyNotification("New PasserBy!", "You passed by someone"); // Should specify who using their name. Could also be several people at once
                        
                        // Add each passerby to the player's collection
                        foreach (Passerby passerby in nearbyPlayers.Values)
                        {
                            passerbyCollection.Add(passerby);
                            Debug.Log($"Added {passerby.Name} to collection!");
                        }
                        nearbyPlayerFound.Invoke();
                    }
                }
            }
            yield break;
        }

        private void onNearbyPlayerFound()
        {
            // Display collected avatars on screen...
            if(SceneManager.GetActiveScene().name == "MainHub")
            {
                Debug.Log("In the MainHub scene!");
                GameObject scrollViewContent = GameObject.Find("/Canvas/Scroll View/Viewport/Content");
                if (scrollViewContent == null)
                    Debug.LogError("Couldn't find Content object?");
                else
                    Debug.Log("Found the Content object");

                for (int i = 0; i < passerbyCollection.Count; i++)
                {
                    Passerby passerby = passerbyCollection[i];

                    GameObject gridCell = new GameObject($"Grid Cell {i}");
                    gridCell.transform.SetParent(scrollViewContent.transform, false);
                    gridCell.AddComponent<RectTransform>();

                    GameObject avatar = new GameObject($"{passerby.Name}'s Avatar");
                    avatar.transform.SetParent(gridCell.transform, false);
                    avatar.AddComponent<RectTransform>();

                    Image image = avatar.AddComponent<Image>();
                    Sprite avatarBody = Resources.Load<Sprite>("Art/kenney_shape-characters/PNG/Default/" + passerby.Avatar.BodyType);
                    if (avatarBody == null)
                        Debug.LogError("Couldn't find avatarBody resource?");
                    else
                        image.sprite = avatarBody;
                }
            }
        }
    }
}
