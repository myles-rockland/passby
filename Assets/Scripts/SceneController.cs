using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PassBy
{
    public class SceneController : MonoBehaviour
    {
        public GameObject playerControllerObject;
        private PlayerController playerController;
        public GameObject locationControllerObject;
        private LocationController locationController;


        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;

            playerController = playerControllerObject.GetComponent<PlayerController>();
            locationController = locationControllerObject.GetComponent<LocationController>();
        }

        public void LoadScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Debug.Log("OnSceneLoaded: " + scene.name);
            playerController.StartGetNearbyPlayersPeriodically();
        }
    }
}
