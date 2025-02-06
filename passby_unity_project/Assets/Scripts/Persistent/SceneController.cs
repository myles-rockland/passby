using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace PassBy
{
    public class SceneController : MonoBehaviour
    {
        public static SceneController Instance { get; private set; }
        public GameObject cellPrefab;
        private GameObject selectedAvatar;
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
                FillAvatarCollection();
            }
        }

        public void FillAvatarCollection()
        {
            // Display collected avatars on screen...
            if (SceneManager.GetActiveScene().name == "MainHub")
            {
                Debug.Log("In the MainHub scene!");
                GameObject scrollViewContent = GameObject.Find("/Canvas/Avatar Scroll View/Viewport/Content");
                if (scrollViewContent == null)
                {
                    Debug.LogError("Couldn't find Content object");
                    return;
                }
                else
                {
                    Debug.Log("Found the Content object");
                }

                // Destroy children (cells in grid that contain avatars) before adding new avatars
                foreach (Transform child in scrollViewContent.transform)
                {
                    Destroy(child.gameObject);
                }

                // Edit displayed avatar with new sprites of selected avatar
                GameObject displayedAvatar = GameObject.Find("Displayed Avatar");

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

                for (int i = 0; i < PlayerController.Instance.GetPasserbyCollection().Count; i++)
                {
                    Passerby passerby = PlayerController.Instance.GetPasserbyCollection()[i];

                    // Grid Cell from prefab
                    GameObject gridCell = Instantiate(cellPrefab, scrollViewContent.transform);
                    gridCell.name = $"Grid Cell {i}";
                    Button cellButton = gridCell.AddComponent<Button>();
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

                        // Edit displayed avatar with new sprites of selected avatar
                        bodyImage.sprite = body.GetComponent<Image>().sprite;
                        leftHandImage.sprite = leftHand.GetComponent<Image>().sprite;
                        rightHandImage.sprite = rightHand.GetComponent<Image>().sprite;

                        // Edit TMP_Text with details of now selected avatar
                        TMP_Text details = GameObject.Find("Avatar Details Text").GetComponent<TMP_Text>(); // .Find() needs to be replaced!
                        details.text = $"Name: {passerby.Name}\nInterests: ?\nPassed by: ? times"; // Replace ? with actual details
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
                }
            }
        }
    }
}
