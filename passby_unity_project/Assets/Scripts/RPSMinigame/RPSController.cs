using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PassBy
{
    public class RPSController : MonoBehaviour
    {
        public static RPSController Instance { get; private set; }
        [SerializeField]
        TMP_Text winStreakText;
        [SerializeField]
        TMP_Text middleText;
        [SerializeField]
        GameObject backToHubButton;
        [SerializeField]
        GameObject startButton;
        [SerializeField]
        GameObject p1Avatar;
        [SerializeField]
        GameObject p2Avatar;
        bool gameRunning;
        int winStreak;

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
            // Load save, including boxing data
            SaveController.Instance.Load();

            // Set win streak text
            winStreakText.text = $"Win Streak: {winStreak}";

            // Set P1 Avatar to player's avatar
            // Body
            SpriteRenderer p1Body = p1Avatar.transform.GetChild(0).GetComponent<SpriteRenderer>();
            string player1Body = PlayerController.Instance.Passerby.Avatar.BodyType;
            p1Body.sprite = Resources.Load<Sprite>("Art/kenney_shape-characters/PNG/Default/" + player1Body);
            // Left Hand
            SpriteRenderer p1LeftHand = p1Avatar.transform.GetChild(2).GetComponent<SpriteRenderer>();
            string player1LeftHand = PlayerController.Instance.Passerby.Avatar.LeftHandColour;
            p1LeftHand.sprite = Resources.Load<Sprite>("Art/kenney_shape-characters/PNG/Default/" + player1LeftHand);
            // Right Hand
            SpriteRenderer p1RightHand = p1Avatar.transform.GetChild(3).GetComponent<SpriteRenderer>();
            string player1RightHand = PlayerController.Instance.Passerby.Avatar.RightHandColour;
            p1RightHand.sprite = Resources.Load<Sprite>("Art/kenney_shape-characters/PNG/Default/" + player1RightHand);

            // Get the active passerby queue
            Queue<Passerby> activePasserbyQueue = PlayerController.Instance.GetActivePasserbyQueue();
            // If player has a passerby in their active queue...
            if (activePasserbyQueue.Count > 0) 
            {
                // Set player2 to first passerby in the queue, and remove that passerby from the queue
                Passerby p2Passerby = activePasserbyQueue.Dequeue();

                // Set P2 Avatar to passerby's avatar
                // Body
                SpriteRenderer p2Body = p2Avatar.transform.GetChild(0).GetComponent<SpriteRenderer>();
                string player2Body = p2Passerby.Avatar.BodyType;
                p2Body.sprite = Resources.Load<Sprite>("Art/kenney_shape-characters/PNG/Default/" + player2Body);
                // Left Hand
                SpriteRenderer p2LeftHand = p2Avatar.transform.GetChild(2).GetComponent<SpriteRenderer>();
                string player2LeftHand = p2Passerby.Avatar.LeftHandColour;
                p2LeftHand.sprite = Resources.Load<Sprite>("Art/kenney_shape-characters/PNG/Default/" + player2LeftHand);
                // Right Hand
                SpriteRenderer p2RightHand = p2Avatar.transform.GetChild(3).GetComponent<SpriteRenderer>();
                string player2RightHand = p2Passerby.Avatar.RightHandColour;
                p2RightHand.sprite = Resources.Load<Sprite>("Art/kenney_shape-characters/PNG/Default/" + player2RightHand);

                // Enable P2 Avatar
                p2Avatar.SetActive(true);

                // Enable Start button
                startButton.SetActive(true);
            }
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        IEnumerator Countdown()
        {
            middleText.text = "Rock...";
            yield return new WaitForSeconds(1);
            middleText.text = "Paper...";
            yield return new WaitForSeconds(1);
            middleText.text = "Scissors...";
            yield return new WaitForSeconds(1);
            middleText.text = "Shoot!!";
            gameRunning = true;
            // Check for win?
            //yield return null;
        }

        public void Save(ref RPSData boxingData)
        {
            boxingData.winStreak = winStreak;
        }
        public void Load(RPSData boxingData)
        {
            winStreak = boxingData.winStreak;
        }
    }

    [System.Serializable]
    public struct RPSData
    {
        public int winStreak;
    }
}
