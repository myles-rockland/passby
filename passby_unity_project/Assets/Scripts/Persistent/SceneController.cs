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
            }
        }
    }
}
