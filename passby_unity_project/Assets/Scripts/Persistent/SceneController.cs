using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace PassBy
{
    public class SceneController : MonoBehaviour
    {
        public static SceneController Instance { get; private set; }
        public GameObject cellPrefab;
        public GameObject frCellPrefab;
        private GameObject selectedAvatar;
        private Passerby displayedPasserby;
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject); // Prevent duplicates
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        void Start()
        {
            if(SaveController.Instance.SaveExists())
            {
                SaveController.Instance.Load();
                SceneManager.LoadSceneAsync("MainHub", LoadSceneMode.Additive); // Load MainHub scene on top of Persistent scene
            }
            else
            {
                SceneManager.LoadSceneAsync("AvatarCreation", LoadSceneMode.Additive); // Load AvatarCreation scene on top of Persistent scene
            }
        }

        public Passerby GetDisplayedPasserby()
        {
            return displayedPasserby;
        }

        public void LoadScene(string sceneName)
        {
            SceneManager.LoadSceneAsync(sceneName);
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Debug.Log("OnSceneLoaded: " + scene.name);
            if (scene.name == "MainHub")
            {
                SceneManager.SetActiveScene(scene);
                PlayerController.Instance.StartGetNearbyPlayersPeriodically();
                FirstTimeLoad();
                FillAvatarCollection();
                PlayerController.Instance.StartGetFriendRequestsPeriodically();
            }
        }

        public void FirstTimeLoad()
        {
            // Edit displayed avatar with new sprites of selected avatar
            GameObject displayedAvatar = GameObject.Find("Displayed Avatar");
            displayedPasserby = PlayerController.Instance.Passerby;

            // Body
            Image bodyImage = displayedAvatar.transform.GetChild(0).GetComponent<Image>();
            string playerAvatarBody = PlayerController.Instance.Passerby.Avatar.BodyType;
            bodyImage.sprite = Resources.Load<Sprite>("Art/kenney_shape-characters/PNG/Default/" + playerAvatarBody);

            // Left Hand
            Image leftHandImage = displayedAvatar.transform.GetChild(2).GetComponent<Image>();
            string playerAvatarLeftHand = PlayerController.Instance.Passerby.Avatar.LeftHandColour;
            leftHandImage.sprite = Resources.Load<Sprite>("Art/kenney_shape-characters/PNG/Default/" + playerAvatarLeftHand);

            // Right Hand
            Image rightHandImage = displayedAvatar.transform.GetChild(3).GetComponent<Image>();
            string playerAvatarRightHand = PlayerController.Instance.Passerby.Avatar.RightHandColour;
            rightHandImage.sprite = Resources.Load<Sprite>("Art/kenney_shape-characters/PNG/Default/" + playerAvatarRightHand);

            // Edit avatar details text to welcome the player
            TMP_Text details = GameObject.Find("Avatar Details Text").GetComponent<TMP_Text>(); // .Find() needs to be replaced!
            details.text = $"Welcome, {PlayerController.Instance.Passerby.Name}";
        }

        public void FillAvatarCollection()
        {
            // Edit displayed avatar with new sprites of selected avatar
            GameObject displayedAvatar = GameObject.Find("Displayed Avatar");
            // Body
            Image bodyImage = displayedAvatar.transform.GetChild(0).GetComponent<Image>();
            // Left Hand
            Image leftHandImage = displayedAvatar.transform.GetChild(2).GetComponent<Image>();
            // Right Hand
            Image rightHandImage = displayedAvatar.transform.GetChild(3).GetComponent<Image>();


            // Display collected avatars on screen...
            if (SceneManager.GetActiveScene().name == "MainHub")
            {
                Debug.Log("In the MainHub scene!");
                GameObject scrollViewContent = GameObject.Find("/Canvas/Avatar Scroll View/Viewport/Content");

                // Edit passersby counter text with number of passersby in collection
                TMP_Text passersbyCounter = GameObject.Find("Passersby Text").GetComponent<TMP_Text>(); // .Find() needs to be replaced!
                passersbyCounter.text = $"Passersby: {PlayerController.Instance.GetPasserbyCollection().Count}";

                // Destroy children (cells in grid that contain avatars) before adding new avatars
                foreach (Transform child in scrollViewContent.transform)
                {
                    Destroy(child.gameObject);
                }

                // Add all avatars from collection
                for (int i = 0; i < PlayerController.Instance.GetPasserbyCollection().Count; i++)
                {
                    Passerby passerby = PlayerController.Instance.GetPasserbyCollection()[i];

                    // Grid Cell from prefab
                    GameObject gridCell = Instantiate(cellPrefab, scrollViewContent.transform);
                    gridCell.name = $"Grid Cell {i}";
                    Button cellButton = gridCell.AddComponent<Button>();

                    // Add outline if currently selected
                    if(displayedPasserby == passerby)
                    {
                        // Turn on outlines on cellButton.gameObject's avatar (child 0)
                        GameObject avatarObject = cellButton.gameObject.transform.GetChild(0).gameObject;
                        // Body outline
                        Outline existingBodyOutline = avatarObject.transform.GetChild(0).GetComponent<Outline>();
                        existingBodyOutline.enabled = true;
                        // Left hand outline
                        Outline existingLeftHandOutline = avatarObject.transform.GetChild(2).GetComponent<Outline>();
                        existingLeftHandOutline.enabled = true;
                        // Right hand outline
                        Outline existingRightHandOutline = avatarObject.transform.GetChild(3).GetComponent<Outline>();
                        existingRightHandOutline.enabled = true;
                        // Set new selected avatar
                        selectedAvatar = avatarObject;
                        displayedPasserby = passerby;
                    }

                    // Add onClick listener to grid cell
                    cellButton.onClick.AddListener(() =>
                    {
                        // Turn off outlines on currently selected avatar
                        if (selectedAvatar != null)
                        {
                            // Body outline
                            Outline bodyOutline = selectedAvatar.transform.GetChild(0).GetComponent<Outline>();
                            bodyOutline.enabled = false;
                            // Left hand outline
                            Outline leftHandOutline = selectedAvatar.transform.GetChild(2).GetComponent<Outline>();
                            leftHandOutline.enabled = false;
                            // Right hand outline
                            Outline rightHandOutline = selectedAvatar.transform.GetChild(3).GetComponent<Outline>();
                            rightHandOutline.enabled = false;
                        }

                        // Turn on outlines on cellButton.gameObject's avatar (child 0)
                        GameObject avatar = cellButton.gameObject.transform.GetChild(0).gameObject;
                        // Body outline
                        Transform body = avatar.transform.GetChild(0);
                        Outline newBodyOutline = body.GetComponent<Outline>();
                        newBodyOutline.enabled = true;
                        // Left hand outline
                        Transform leftHand = avatar.transform.GetChild(2);
                        Outline newLeftHandOutline = leftHand.GetComponent<Outline>();
                        newLeftHandOutline.enabled = true;
                        // Right hand outline
                        Transform rightHand = avatar.transform.GetChild(3);
                        Outline newRightHandOutline = rightHand.GetComponent<Outline>();
                        newRightHandOutline.enabled = true;
                        // Set new selected avatar
                        selectedAvatar = avatar;
                        displayedPasserby = passerby;

                        // Edit displayed avatar with new sprites of selected avatar
                        bodyImage.sprite = body.GetComponent<Image>().sprite;
                        leftHandImage.sprite = leftHand.GetComponent<Image>().sprite;
                        rightHandImage.sprite = rightHand.GetComponent<Image>().sprite;

                        // Edit avatar details text with details of now selected avatar
                        TMP_Text details = GameObject.Find("Avatar Details Text").GetComponent<TMP_Text>(); // .Find() needs to be replaced!
                        //details.text = $"Name: {passerby.Name}\nInterests: ?\nPassed by: ? times"; // Replace ? with actual details
                        details.text = $"Name: {passerby.Name}"; // Temporary change to remove interests and passby counter for poster.

                        // Change friend request button based on friend status/friend request status
                        GameObject sendButton = GameObject.Find("/Canvas/Friend Request Buttons/Friend Request Button");
                        GameObject sentButton = GameObject.Find("/Canvas/Friend Request Buttons/Disabled Friend Request Button");
                        GameObject alreadyFriendsButton = GameObject.Find("/Canvas/Friend Request Buttons/Already Friends Button");

                        // Set all buttons to false
                        sendButton.SetActive(false);
                        sentButton.SetActive(false);
                        alreadyFriendsButton.SetActive(false);

                        // Set respective button to true based on friend/friend request status
                        if(passerby.ID >= 0) // Only real players can be friends... not fake ones
                        {
                            if (PlayerController.Instance.GetFriendsList().Contains(passerby))
                            {
                                alreadyFriendsButton.SetActive(true);
                            }
                            else if (PlayerController.Instance.GetOutgoingFriendRequests().Contains(passerby.ID))
                            {
                                sentButton.SetActive(true);
                            }
                            else
                            {
                                sendButton.SetActive(true);
                            }
                        }
                    });

                    // Canvas avatar from grid cell prefab
                    GameObject avatar = gridCell.transform.GetChild(0).gameObject;
                    avatar.name = $"{passerby.Name}'s Avatar";
                    RectTransform avatarRectTransform = avatar.GetComponent<RectTransform>();
                    avatarRectTransform.localScale = new Vector3(220.0f, 220.0f, 220.0f);

                    // Body
                    Image body = avatar.transform.GetChild(0).GetComponent<Image>();
                    string avatarBody = passerby.Avatar.BodyType;
                    body.sprite = Resources.Load<Sprite>("Art/kenney_shape-characters/PNG/Default/" + avatarBody);
                    // Left Hand
                    Image leftHand = avatar.transform.GetChild(2).GetComponent<Image>();
                    string avatarLeftHand = passerby.Avatar.LeftHandColour;
                    leftHand.sprite = Resources.Load<Sprite>("Art/kenney_shape-characters/PNG/Default/" + avatarLeftHand);
                    // Right Hand
                    Image rightHand = avatar.transform.GetChild(3).GetComponent<Image>();
                    string avatarRightHand = passerby.Avatar.RightHandColour;
                    rightHand.sprite = Resources.Load<Sprite>("Art/kenney_shape-characters/PNG/Default/" + avatarRightHand);

                    // Avatar name tag from grid cell prefab
                    TMP_Text nameTag = gridCell.transform.GetChild(1).GetComponent<TMP_Text>();
                    nameTag.text = passerby.Name;

                    // Friend icon from grid cell prefab
                    GameObject friendIcon = gridCell.transform.GetChild(2).gameObject;
                    // Activate or deactivate based on presence in friends list
                    friendIcon.SetActive(PlayerController.Instance.GetFriendsList().Contains(passerby)); // Probably won't work, need to compare IDs rather than passerby references...
                }
            }
        }

        public void FillFriendRequestsMenu()
        {
            // Delete all current pending friend requests from friend requests scroll view
            GameObject frScrollView = GameObject.FindWithTag("Friend Requests Menu");

            foreach (Transform child in frScrollView.transform)
            {
                Destroy(child.gameObject);
            }

            // Add all passersby from IDs in friend requests
            List<Passerby> pendingFriendPassersby = new List<Passerby>();
            foreach(Passerby passerby in PlayerController.Instance.GetPasserbyCollection())
            {
                if (PlayerController.Instance.GetIncomingFriendRequests().Contains(passerby.ID))
                {
                    pendingFriendPassersby.Add(passerby);
                }
            }

            // Add all pending friend requests to friend requests scroll view
            for (int i = 0; i < pendingFriendPassersby.Count; i++)
            {
                Passerby passerby = pendingFriendPassersby[i];

                // Friend Request Cell from prefab
                GameObject frCell = Instantiate(frCellPrefab, frScrollView.transform);
                frCell.name = $"Friend Request Cell {i}";

                // Set the avatar
                GameObject avatar = frCell.transform.GetChild(0).gameObject;
                avatar.name = $"{passerby.Name}'s Avatar";
                // Body
                Image body = avatar.transform.GetChild(0).GetComponent<Image>();
                string avatarBody = passerby.Avatar.BodyType;
                body.sprite = Resources.Load<Sprite>("Art/kenney_shape-characters/PNG/Default/" + avatarBody);
                // Left Hand
                Image leftHand = avatar.transform.GetChild(2).GetComponent<Image>();
                string avatarLeftHand = passerby.Avatar.LeftHandColour;
                leftHand.sprite = Resources.Load<Sprite>("Art/kenney_shape-characters/PNG/Default/" + avatarLeftHand);
                // Right Hand
                Image rightHand = avatar.transform.GetChild(3).GetComponent<Image>();
                string avatarRightHand = passerby.Avatar.RightHandColour;
                rightHand.sprite = Resources.Load<Sprite>("Art/kenney_shape-characters/PNG/Default/" + avatarRightHand);

                // Set the name tag
                TMP_Text nameTag = frCell.transform.GetChild(1).GetComponent<TMP_Text>();
                nameTag.text = passerby.Name;

                // Add button listener to decline the friend request
                Button declineButton = frCell.transform.GetChild(2).GetComponent<Button>();
                declineButton.onClick.AddListener(() =>
                {
                    // Start ProcessFriendRequest coroutine
                    StartCoroutine(RespondToFriendRequest(passerby, false));

                    // Delete parent cell and everything under it
                    Destroy(frCell);
                });

                // Add button listener to accept the friend request
                Button acceptButton = frCell.transform.GetChild(3).GetComponent<Button>();
                acceptButton.onClick.AddListener(() =>
                {
                    // Start ProcessFriendRequest coroutine
                    StartCoroutine(RespondToFriendRequest(passerby, true));

                    // Add passerby to friends list
                    PlayerController.Instance.GetFriendsList().Add(passerby);

                    // Update avatar collection with new icon
                    FillAvatarCollection();

                    // Delete parent cell and everything under it
                    Destroy(frCell);
                });
            }
        }

        public IEnumerator RespondToFriendRequest(Passerby sender, bool accepted)
        {
            // Create JSON data for request body
            Dictionary<string, object> playerData = new Dictionary<string, object> {
                { "sender_id", sender.ID },
                { "recipient_id", PlayerController.Instance.Passerby.ID },
                { "accepted", accepted }
            };

            string playerJsonData = JsonConvert.SerializeObject(playerData);

            // Send POST request to get nearby players
            string contentType = "application/json";
            using (UnityWebRequest request = UnityWebRequest.Post($"{PlayerController.Instance.GetServerURL()}/respond_to_friend_request", playerJsonData, contentType))
            {
                yield return request.SendWebRequest();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError("Error processing friend request: " + request.error);
                }
                else
                {
                    string jsonResponse = request.downloadHandler.text;
                    Debug.Log(jsonResponse);
                    SaveController.Instance.Save();
                }
            }
        }
    }
}
