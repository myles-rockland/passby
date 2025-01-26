using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using Newtonsoft.Json;

namespace PassBy
{
    public class SaveController : MonoBehaviour
    {
        public static SaveController Instance { get; private set; }
        public SaveData saveData;
        string savePath;

        [System.Serializable]
        public struct SaveData
        {
            // Add more Data classes as necessary
            public PlayerData playerData;
            public BoxingData boxingData;
        }

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

        private void Start()
        {
            savePath = Application.persistentDataPath + "/save.sav";
            saveData = new SaveData();
        }

        public bool SaveExists()
        {
            return File.Exists(savePath);
        }

        public void Save()
        {
            HandleSaveData();

            File.WriteAllText(savePath, JsonConvert.SerializeObject(saveData, Formatting.Indented));
        }

        public void HandleSaveData() // Add to this method when other objects with data require saving
        {
            // Add more calls to objects with save methods here as necessary
            PlayerController.Instance.Save(ref saveData.playerData);
            if(BoxingController.Instance != null)
            {
                BoxingController.Instance.Save(ref saveData.boxingData);
            }
        }

        public void Load()
        {
            string saveFileContent = File.ReadAllText(savePath);

            saveData = JsonConvert.DeserializeObject<SaveData>(saveFileContent);

            HandleLoadData();
        }

        public void HandleLoadData() // Add to this method when other objects with data require loading
        {
            // Add more calls to objects with load methods here as necessary
            PlayerController.Instance.Load(saveData.playerData);
            if(BoxingController.Instance != null)
            {
                BoxingController.Instance.Load(saveData.boxingData);
            }
            // Add some way to load the boxing data
            // Need an instance of the boxing controller to access its Save and Load methods
            // But currently, the instance only exists when the boxing scene is loaded
        }
    }
}
