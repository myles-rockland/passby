using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace PassBy
{
    public class SceneController : MonoBehaviour
    {
        public static SceneController Instance { get; private set; }
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
            SceneManager.LoadSceneAsync("AvatarCreation", LoadSceneMode.Additive); // Load AvatarCreation scene on top of Persistent scene
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
                    Debug.LogError("Couldn't find Content object");
                else
                    Debug.Log("Found the Content object");

                for (int i = 0; i < PlayerController.Instance.passerbyCollection.Count; i++)
                {
                    Passerby passerby = PlayerController.Instance.passerbyCollection[i];

                    // Grid Cell Object
                    GameObject gridCell = new GameObject($"Grid Cell {scrollViewContent.transform.childCount}");
                    gridCell.transform.SetParent(scrollViewContent.transform, false);
                    gridCell.AddComponent<RectTransform>();
                    gridCell.AddComponent<Mask>();

                    // Avatar Object
                    GameObject avatar = new GameObject($"{passerby.Name}'s Avatar");
                    avatar.transform.SetParent(gridCell.transform, false);
                    RectTransform avatarRectTransform = avatar.AddComponent<RectTransform>();
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
