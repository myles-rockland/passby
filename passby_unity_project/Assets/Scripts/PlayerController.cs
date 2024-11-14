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
        string serverUrl = "http://10.86.73.162:5000"; // 10.86.73.162

        void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
        private void Start()
        {
            id = -1;
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
            while (id < 0)
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
                { "player_id", id },
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
                        NotificationController notificationController = notificationControllerObject.GetComponent<NotificationController>();
                        notificationController.SendPassbyNotification("New PasserBy!", "You passed by someone"); // Should specify who using their name. Could also be several people at once

                        // Add each passerby to the player's collection
                        bool newPasserbyCollected = false;
                        foreach (Passerby passerby in nearbyPlayers.Values)
                        {
                            if(!passerbyCollection.Contains(passerby))
                            {
                                passerbyCollection.Add(passerby);
                                newPasserbyCollected = true;
                                Debug.Log($"Added {passerby.Name} to collection!");
                            }
                        }
                        if (newPasserbyCollected)
                        {
                            nearbyPlayerFound.Invoke();
                        }
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
                    Debug.LogError("Couldn't find Content object");
                else
                    Debug.Log("Found the Content object");

                for (int i = 0; i < passerbyCollection.Count; i++)
                {
                    Passerby passerby = passerbyCollection[i];

                    // Grid Cell Object
                    GameObject gridCell = new GameObject($"Grid Cell {scrollViewContent.transform.childCount}");
                    gridCell.transform.SetParent(scrollViewContent.transform, false);
                    gridCell.AddComponent<RectTransform>();
                    gridCell.AddComponent<Mask>();

                    // Avatar Object
                    GameObject avatar = new GameObject($"{passerby.Name}'s Avatar");
                    avatar.transform.SetParent(gridCell.transform, false);
                    RectTransform avatarRectTransform =  avatar.AddComponent<RectTransform>();
                    avatarRectTransform.localScale = new Vector3(220.0f, 220.0f, 220.0f);
                    gridCell.AddComponent<Mask>();

                    // Body Object
                    GameObject body = new GameObject("Body");
                    body.transform.SetParent(avatar.transform, false);
                    SpriteRenderer bodyRenderer = body.AddComponent<SpriteRenderer>();
                    Sprite avatarBody = Resources.Load<Sprite>("Art/kenney_shape-characters/PNG/Default/" + passerby.Avatar.BodyType);
                    if (avatarBody == null)
                        Debug.LogError("Couldn't find avatarBody resource");
                    else
                        bodyRenderer.sprite = avatarBody;

                    // Face Object
                    GameObject face = new GameObject("Face");
                    face.transform.SetParent(avatar.transform, false);
                    SpriteRenderer faceRenderer = face.AddComponent<SpriteRenderer>();
                    faceRenderer.sortingOrder = 1;
                    Sprite avatarFace = Resources.Load<Sprite>("Art/kenney_shape-characters/PNG/Default/face_a");
                    if (avatarFace == null)
                        Debug.LogError("Couldn't find avatarFace resource");
                    else
                        faceRenderer.sprite = avatarFace;

                    // Left Hand Object
                    GameObject leftHand = new GameObject("Left Hand");
                    leftHand.transform.SetParent(avatar.transform, false);
                    leftHand.transform.localPosition = new Vector3(-0.6f, -0.2f, 0f);
                    SpriteRenderer leftHandRenderer = leftHand.AddComponent<SpriteRenderer>();
                    leftHandRenderer.flipX = true;
                    leftHandRenderer.flipY = true;
                    Sprite avatarLeftHand = Resources.Load<Sprite>("Art/kenney_shape-characters/PNG/Default/" + passerby.Avatar.LeftHandColour);
                    if (avatarLeftHand == null)
                        Debug.LogError("Couldn't find left hand resource");
                    else
                        leftHandRenderer.sprite = avatarLeftHand;

                    // Right Hand Object
                    GameObject rightHand = new GameObject("Right Hand");
                    rightHand.transform.SetParent(avatar.transform, false);
                    rightHand.transform.localPosition = new Vector3(0.6f, -0.2f, 0f);
                    SpriteRenderer rightHandRenderer = rightHand.AddComponent<SpriteRenderer>();
                    rightHandRenderer.flipY = true;
                    Sprite avatarRightHand = Resources.Load<Sprite>("Art/kenney_shape-characters/PNG/Default/" + passerby.Avatar.RightHandColour);
                    if (avatarRightHand == null)
                        Debug.LogError("Couldn't find left hand resource");
                    else
                        rightHandRenderer.sprite = avatarRightHand;
                }
            }
        }
    }
}
