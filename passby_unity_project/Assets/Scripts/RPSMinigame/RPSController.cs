using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
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
        GameObject rockButton, paperButton, scissorsButton;
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
        int winStreak;
        int p1Choice, p2Choice;
        SpriteRenderer p2RightHand;

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
            // Load save, including rps data
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
                p2RightHand = p2Avatar.transform.GetChild(3).GetComponent<SpriteRenderer>();
                string player2RightHand = p2Passerby.Avatar.RightHandColour;
                p2RightHand.sprite = Resources.Load<Sprite>("Art/kenney_shape-characters/PNG/Default/" + player2RightHand);

                // Enable P2 Avatar
                p2Avatar.SetActive(true);

                // Enable Start button
                startButton.SetActive(true);
            }
        }

        public void SetChoice(int c)
        {
            p1Choice = c;
            p2Choice = Random.Range(0, 3); // 0 = Rock, 1 = Paper, 2 = Scissors
        }

        public void StartCountdown()
        {
            StartCoroutine(Countdown());
        }

        IEnumerator Countdown()
        {
            middleText.text = "Rock...";
            yield return new WaitForSeconds(0.4f);
            middleText.text = "Paper...";
            yield return new WaitForSeconds(0.4f);
            middleText.text = "Scissors...";
            yield return new WaitForSeconds(0.4f);
            middleText.text = "Shoot!!";
            ChangeHands();
            yield return new WaitForSeconds(1);
            CheckForWin();
            //yield return null;
        }

        void ChangeHands()
        {
            // Set P1's left hand
            SpriteRenderer p1LeftHand = p1Avatar.transform.GetChild(2).GetComponent<SpriteRenderer>();
            string player1LeftHand = PlayerController.Instance.Passerby.Avatar.LeftHandColour;

            switch (player1LeftHand)
            {
                case "blue_hand_closed":
                    if(p1Choice == 0)
                        p1LeftHand.sprite = Resources.Load<Sprite>("Art/kenney_shape-characters/PNG/Default/blue_hand_closed");
                    else if(p1Choice == 1)
                        p1LeftHand.sprite = Resources.Load<Sprite>("Art/kenney_shape-characters/PNG/Default/blue_hand_open");
                    else
                        p1LeftHand.sprite = Resources.Load<Sprite>("Art/kenney_shape-characters/PNG/Default/blue_hand_peace");
                    break;
                case "green_hand_closed":
                    if (p1Choice == 0)
                        p1LeftHand.sprite = Resources.Load<Sprite>("Art/kenney_shape-characters/PNG/Default/green_hand_closed");
                    else if (p1Choice == 1)
                        p1LeftHand.sprite = Resources.Load<Sprite>("Art/kenney_shape-characters/PNG/Default/green_hand_open");
                    else
                        p1LeftHand.sprite = Resources.Load<Sprite>("Art/kenney_shape-characters/PNG/Default/green_hand_peace");
                    break;
                case "hand_yellow_closed":
                    if (p1Choice == 0)
                        p1LeftHand.sprite = Resources.Load<Sprite>("Art/kenney_shape-characters/PNG/Default/hand_yellow_closed");
                    else if (p1Choice == 1)
                        p1LeftHand.sprite = Resources.Load<Sprite>("Art/kenney_shape-characters/PNG/Default/hand_yellow_open");
                    else
                        p1LeftHand.sprite = Resources.Load<Sprite>("Art/kenney_shape-characters/PNG/Default/hand_yellow_peace");
                    break;
                case "pink_hand_closed":
                    if (p1Choice == 0)
                        p1LeftHand.sprite = Resources.Load<Sprite>("Art/kenney_shape-characters/PNG/Default/pink_hand_closed");
                    else if (p1Choice == 1)
                        p1LeftHand.sprite = Resources.Load<Sprite>("Art/kenney_shape-characters/PNG/Default/pink_hand_open");
                    else
                        p1LeftHand.sprite = Resources.Load<Sprite>("Art/kenney_shape-characters/PNG/Default/pink_hand_peace");
                    break;
                case "purple_hand_closed":
                    if (p1Choice == 0)
                        p1LeftHand.sprite = Resources.Load<Sprite>("Art/kenney_shape-characters/PNG/Default/purple_hand_closed");
                    else if (p1Choice == 1)
                        p1LeftHand.sprite = Resources.Load<Sprite>("Art/kenney_shape-characters/PNG/Default/purple_hand_open");
                    else
                        p1LeftHand.sprite = Resources.Load<Sprite>("Art/kenney_shape-characters/PNG/Default/purple_hand_peace");
                    break;
                case "red_hand_closed":
                    if (p1Choice == 0)
                        p1LeftHand.sprite = Resources.Load<Sprite>("Art/kenney_shape-characters/PNG/Default/red_hand_closed");
                    else if (p1Choice == 1)
                        p1LeftHand.sprite = Resources.Load<Sprite>("Art/kenney_shape-characters/PNG/Default/red_hand_open");
                    else
                        p1LeftHand.sprite = Resources.Load<Sprite>("Art/kenney_shape-characters/PNG/Default/red_hand_peace");
                    break;
                default:
                    Debug.LogError("Couldn't find valid asset name for player 1 left hand");
                    break;
            }

            // Set P2's right hand
            string player2RightHand = p2RightHand.sprite.name;
            Debug.Log($"player2RightHand sprite name: {player2RightHand}");

            switch (player2RightHand)
            {
                case "blue_hand_closed":
                    if (p2Choice == 0)
                        p2RightHand.sprite = Resources.Load<Sprite>("Art/kenney_shape-characters/PNG/Default/blue_hand_closed");
                    else if (p2Choice == 1)
                        p2RightHand.sprite = Resources.Load<Sprite>("Art/kenney_shape-characters/PNG/Default/blue_hand_open");
                    else
                        p2RightHand.sprite = Resources.Load<Sprite>("Art/kenney_shape-characters/PNG/Default/blue_hand_peace");
                    break;
                case "green_hand_closed":
                    if (p2Choice == 0)
                        p2RightHand.sprite = Resources.Load<Sprite>("Art/kenney_shape-characters/PNG/Default/green_hand_closed");
                    else if (p2Choice == 1)
                        p2RightHand.sprite = Resources.Load<Sprite>("Art/kenney_shape-characters/PNG/Default/green_hand_open");
                    else
                        p2RightHand.sprite = Resources.Load<Sprite>("Art/kenney_shape-characters/PNG/Default/green_hand_peace");
                    break;
                case "hand_yellow_closed":
                    if (p2Choice == 0)
                        p2RightHand.sprite = Resources.Load<Sprite>("Art/kenney_shape-characters/PNG/Default/hand_yellow_closed");
                    else if (p2Choice == 1)
                        p2RightHand.sprite = Resources.Load<Sprite>("Art/kenney_shape-characters/PNG/Default/hand_yellow_open");
                    else
                        p2RightHand.sprite = Resources.Load<Sprite>("Art/kenney_shape-characters/PNG/Default/hand_yellow_peace");
                    break;
                case "pink_hand_closed":
                    if (p2Choice == 0)
                        p2RightHand.sprite = Resources.Load<Sprite>("Art/kenney_shape-characters/PNG/Default/pink_hand_closed");
                    else if (p2Choice == 1)
                        p2RightHand.sprite = Resources.Load<Sprite>("Art/kenney_shape-characters/PNG/Default/pink_hand_open");
                    else
                        p2RightHand.sprite = Resources.Load<Sprite>("Art/kenney_shape-characters/PNG/Default/pink_hand_peace");
                    break;
                case "purple_hand_closed":
                    if (p2Choice == 0)
                        p2RightHand.sprite = Resources.Load<Sprite>("Art/kenney_shape-characters/PNG/Default/purple_hand_closed");
                    else if (p2Choice == 1)
                        p2RightHand.sprite = Resources.Load<Sprite>("Art/kenney_shape-characters/PNG/Default/purple_hand_open");
                    else
                        p2RightHand.sprite = Resources.Load<Sprite>("Art/kenney_shape-characters/PNG/Default/purple_hand_peace");
                    break;
                case "red_hand_closed":
                    if (p2Choice == 0)
                        p2RightHand.sprite = Resources.Load<Sprite>("Art/kenney_shape-characters/PNG/Default/red_hand_closed");
                    else if (p2Choice == 1)
                        p2RightHand.sprite = Resources.Load<Sprite>("Art/kenney_shape-characters/PNG/Default/red_hand_open");
                    else
                        p2RightHand.sprite = Resources.Load<Sprite>("Art/kenney_shape-characters/PNG/Default/red_hand_peace");
                    break;
                default:
                    Debug.LogError("Couldn't find valid asset name for player 2 right hand");
                    break;
            }
        }

        void CheckForWin()
        {
            switch (p1Choice)
            {
                case 0:
                    if(p2Choice == 0) // Draw
                    {
                        // Go again, retoggle the buttons on
                        middleText.text = string.Empty;
                        ToggleRPSButtons(true);
                    }
                    else if (p2Choice == 1) // Rock < Paper, p1 loses
                    {
                        middleText.text = "Defeat...";
                        backToHubButton.SetActive(true);
                        winStreak = 0;
                        winStreakText.text = $"Win Streak: {winStreak}";
                        SaveController.Instance.Save();
                    }
                    else // Rock > Scissors, p1 wins
                    {
                        // P1 wins
                        middleText.text = "Victory!";
                        backToHubButton.SetActive(true);
                        winStreak++;
                        winStreakText.text = $"Win Streak: {winStreak}";
                        SaveController.Instance.Save();
                    }
                    break;
                case 1:
                    if (p2Choice == 1) // Draw
                    {
                        // Go again, retoggle the buttons on
                        middleText.text = string.Empty;
                        ToggleRPSButtons(true);
                    }
                    else if (p2Choice == 2) // Paper < Scissors, p1 loses
                    {
                        middleText.text = "Defeat...";
                        backToHubButton.SetActive(true);
                        winStreak = 0;
                        winStreakText.text = $"Win Streak: {winStreak}";
                        SaveController.Instance.Save();
                    }
                    else // Paper > Rock, p1 wins
                    {
                        // P1 wins
                        middleText.text = "Victory!";
                        backToHubButton.SetActive(true);
                        winStreak++;
                        winStreakText.text = $"Win Streak: {winStreak}";
                        SaveController.Instance.Save();
                    }
                    break;
                case 2:
                    if (p2Choice == 2) // Draw
                    {
                        // Go again, retoggle the buttons on
                        middleText.text = string.Empty;
                        ToggleRPSButtons(true);
                    }
                    else if (p2Choice == 0) // Scissors < Rock, p1 loses
                    {
                        middleText.text = "Defeat...";
                        backToHubButton.SetActive(true);
                        winStreak = 0;
                        winStreakText.text = $"Win Streak: {winStreak}";
                        SaveController.Instance.Save();
                    }
                    else // Scissors > Paper, p1 wins
                    {
                        // P1 wins
                        middleText.text = "Victory!";
                        backToHubButton.SetActive(true);
                        winStreak++;
                        winStreakText.text = $"Win Streak: {winStreak}";
                        SaveController.Instance.Save();
                    }
                    break;
                default:
                    Debug.LogError($"p1Choice has an invalid value: {p1Choice}");
                    backToHubButton.SetActive(true);
                    break;
            }
        }

        public void ToggleRPSButtons(bool active)
        {
            rockButton.SetActive(active);
            paperButton.SetActive(active);
            scissorsButton.SetActive(active);
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
