using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace PassBy
{
    public class FriendRequestButton : MonoBehaviour
    {
        public void StartSendFriendRequest()
        {
            StartCoroutine(SendFriendRequest());
        }
        public IEnumerator SendFriendRequest()
        {
            // Get currently displayed passerby
            Passerby recipient = SceneController.Instance.GetDisplayedPasserby();

            // Setup data to use "send_friend_request" request to server
            // Create JSON data for request body
            Dictionary<string, int> playerData = new Dictionary<string, int> {
                { "sender_id", PlayerController.Instance.Passerby.ID },
                { "recipient_id", recipient.ID }
            };

            string playerJsonData = JsonConvert.SerializeObject(playerData);

            // Send POST request to get nearby players
            string contentType = "application/json";
            using (UnityWebRequest request = UnityWebRequest.Post($"{PlayerController.Instance.GetServerURL()}/send_friend_request", playerJsonData, contentType))
            {
                yield return request.SendWebRequest();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError("Error sending friend request: " + request.error);
                }
                else
                {
                    string jsonResponse = request.downloadHandler.text;
                    Debug.Log(jsonResponse);

                    // Change button to greyed out version with "friend request sent"
                    gameObject.SetActive(false);
                    GameObject sentButton = GameObject.Find("/Canvas/Friend Request Buttons/Disabled Friend Request Button");
                    sentButton.SetActive(true);

                    SaveController.Instance.Save();
                }
            }
        }
    }
}
