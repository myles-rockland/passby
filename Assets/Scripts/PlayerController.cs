using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Newtonsoft.Json;

namespace PassBy
{
    public class PlayerController : MonoBehaviour
    {
        private int id;
        private new string name;
        public GameObject inputField;

        public int GetId() { return id; }
        public string GetName() { return name; }
        public void SetName()
        {
            name = inputField.GetComponent<TMP_InputField>().text;
        }
        public void StartGeneratePlayerId()
        {
            StartCoroutine(GeneratePlayerId());
        }

        public IEnumerator GeneratePlayerId()
        {
            // Send a request to the web server for a unique player id
            // https://docs.unity3d.com/2022.3/Documentation/ScriptReference/Networking.UnityWebRequest.Post.html

            // Create dictionaries
            Dictionary<string, float> location = new Dictionary<string, float> {
                { "latitude", Input.location.lastData.latitude },
                { "longitude", Input.location.lastData.longitude }
            };

            string bodyColour = GameObject.Find("Body").GetComponent<SpriteRenderer>().sprite.name; // need a better way to get these values...
            string leftHandColour = GameObject.Find("Left Hand").GetComponent<SpriteRenderer>().sprite.name;
            string rightHandColour = GameObject.Find("Right Hand").GetComponent<SpriteRenderer>().sprite.name;

            Dictionary<string, string> avatar = new Dictionary<string, string> {
                //{ "bodyShape", bodyShape },
                { "bodyColour", bodyColour },
                { "leftHandColour", leftHandColour },
                { "rightHandColour", rightHandColour }
            };

            // Create JSON data
            Dictionary<string, object> playerData = new Dictionary<string, object> {
                { "name", name },
                { "avatar", avatar },
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
                    Debug.Log("Player id updated successfully.");
                }
            }
        }

        IEnumerator Start()
        {
            // Check if the user has location service enabled.
            if (!Input.location.isEnabledByUser)
                Debug.Log("Location not enabled on device or app does not have permission to access location");

            // Starts the location service.
            Input.location.Start();

            // Waits until the location service initializes
            int maxWait = 20;
            while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
            {
                yield return new WaitForSeconds(1);
                maxWait--;
            }

            // If the service didn't initialize in 20 seconds this cancels location service use.
            if (maxWait < 1)
            {
                Debug.Log("Timed out");
                yield break;
            }

            // If the connection failed this cancels location service use.
            if (Input.location.status == LocationServiceStatus.Failed)
            {
                Debug.LogError("Unable to determine device location");
                yield break;
            }
            else
            {
                // If the connection succeeded, this retrieves the device's current location and displays it in the Console window.
                Debug.Log("Location: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude + " " + Input.location.lastData.timestamp);
            }
        }
    }
}
