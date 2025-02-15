using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PassBy
{
    public class CityController : MonoBehaviour
    {
        public static CityController Instance { get; private set; }
        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject); // Prevent duplicates
                return;
            }

            Instance = this;
        }
        // Start is called before the first frame update
        void Start()
        {
            // Load save, including deco data
            SaveController.Instance.Load();
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
