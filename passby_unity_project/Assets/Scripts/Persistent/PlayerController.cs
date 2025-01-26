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
        public static PlayerController Instance { get; private set; }
        public Passerby Passerby { get; private set; }
        public List<Passerby> passerbyCollection;
        UnityEvent nearbyPlayerFound;
        string serverUrl = "http://10.254.95.248:5000"; // 10.86.77.80 at home // 10.254.95.248 on campus

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject); // Prevent duplicates
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        private void Start()
        {
            Passerby = new Passerby();
            passerbyCollection = new List<Passerby>();
            nearbyPlayerFound = new UnityEvent();
            nearbyPlayerFound.AddListener(onNearbyPlayerFound);
        }

        public int GetId() { return Passerby.ID; }
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
                    Passerby.ID = playerIdDict["player_id"];
                    Debug.Log($"Player id generated successfully (value is now {Passerby.ID}).");
                    SaveController.Instance.Save();
                }
            }
        }

        public void StartGetNearbyPlayersPeriodically()
        {
            StartCoroutine(GetNearbyPlayersPeriodically());
        }

        public IEnumerator GetNearbyPlayersPeriodically()
        {
            while (Passerby.ID < 0)
            {
                yield return null;
            }
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
                { "player_id", Passerby.ID },
                { "latitude", Input.location.lastData.latitude },
                { "longitude", Input.location.lastData.longitude }
            };

            string playerJsonData = JsonConvert.SerializeObject(playerData);

            // Send POST request to get nearby players
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

                        // Add each passerby to the player's collection. Can possibly be simplified with a Set data structure?
                        List<int> collectedPasserbyIds = new List<int>();
                        foreach (Passerby passerby in passerbyCollection)
                        {
                            collectedPasserbyIds.Add(passerby.ID);
                        }

                        bool newPasserbyCollected = false;
                        foreach (Passerby passerby in nearbyPlayers.Values)
                        {

                            if(!collectedPasserbyIds.Contains(passerby.ID))
                            {
                                passerbyCollection.Add(passerby);
                                newPasserbyCollected = true;
                                Debug.Log($"Added {passerby.Name} to collection!");
                            }
                        }

                        if (newPasserbyCollected)
                        {
                            nearbyPlayerFound.Invoke();
                            NotificationController.Instance.SendPassbyNotification("New PasserBy!", "You passed by someone"); // Should specify who using their name. Could also be several people at once
                        }

                        SaveController.Instance.Save();
                    }
                }
            }
            yield break;
        }

        private void onNearbyPlayerFound()
        {
            SceneController.Instance.FillAvatarCollection();
        }

        public void Save(ref PlayerData playerData)
        {
            playerData.Passerby = Passerby;
            playerData.PasserbyCollection = passerbyCollection;
        }
        
        public void Load(PlayerData playerData)
        {
            Passerby = playerData.Passerby;
            passerbyCollection = playerData.PasserbyCollection;
        }
    }

    [System.Serializable]
    public struct PlayerData
    {
        public Passerby Passerby;
        public List<Passerby> PasserbyCollection;
    }
}
