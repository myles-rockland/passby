using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PassBy
{
    public class LocationController : MonoBehaviour
    {
        public static LocationController Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject); // Prevent duplicates
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        IEnumerator Start()
        {
            // Check if the user has location service enabled.
            if (!Input.location.isEnabledByUser)
                Debug.LogWarning("Location not enabled on device or app does not have permission to access location");

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
