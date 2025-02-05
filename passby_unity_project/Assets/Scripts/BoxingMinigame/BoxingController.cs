using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PassBy
{
    public class BoxingController : MonoBehaviour
    {
        public static BoxingController Instance { get; private set; }
        [SerializeField]
        Slider p1HealthBarSlider;
        [SerializeField]
        Slider p2HealthBarSlider;
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
        [SerializeField]
        List<AudioClip> punchSfxs;
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

            // If player has a passerby in collection...
            if (PlayerController.Instance.passerbyCollection.Count > 0) 
            {
                // Set player2 to first passerby in collection
                Passerby p2Passerby = PlayerController.Instance.passerbyCollection[0];

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
            if (gameRunning)
            {
                // Check game end conditions via both players' health. If 0 or less, output whether player 1 wins or loses.
                if (p1HealthBarSlider.value <= 0 && p2HealthBarSlider.value <= 0)
                {
                    Debug.LogWarning("Players drew"); // Unsure how to treat this case... maybe win streak is unaffected?
                    middleText.text = "Draw";
                    gameRunning = false;
                    backToHubButton.SetActive(true);
                }
                else if (p1HealthBarSlider.value <= 0)
                {
                    // P1 loses
                    middleText.text = "Defeat...";
                    gameRunning = false;
                    backToHubButton.SetActive(true);
                    winStreak = 0;
                    winStreakText.text = $"Win Streak: {winStreak}";
                    SaveController.Instance.Save();
                }
                else if (p2HealthBarSlider.value <= 0)
                {
                    // P1 wins
                    middleText.text = "Victory!";
                    gameRunning = false;
                    backToHubButton.SetActive(true);
                    winStreak++;
                    winStreakText.text = $"Win Streak: {winStreak}";
                    SaveController.Instance.Save();
                }

                // Check inputs
                if (Input.touchCount > 0)
                {
                    Touch touch = Input.GetTouch(0);
                    if (touch.phase == TouchPhase.Began)
                    {
                        p2HealthBarSlider.value--;
                        // Play random punch sfx
                        int index = Random.Range(0, punchSfxs.Count);
                        AudioClip punchSfx = punchSfxs[index];
                        AudioSource audioSource = GetComponent<AudioSource>();
                        audioSource.clip = punchSfx;
                        audioSource.Play();
                    }
                }
            }
        }

        public void StartCountdown()
        {
            StartCoroutine(Countdown());
        }

        IEnumerator Countdown()
        {
            middleText.text = "Tap in 3";
            yield return new WaitForSeconds(1);
            middleText.text = "Tap in 2";
            yield return new WaitForSeconds(1);
            middleText.text = "Tap in 1";
            yield return new WaitForSeconds(1);
            middleText.text = "Tap!!";
            gameRunning = true;
            StartCoroutine(DecrementP1Health());
            //yield return null;
        }

        IEnumerator DecrementP1Health()
        {
            while (gameRunning)
            {
                float waitTime = Random.Range((float)5/30, (float)6/30);
                yield return new WaitForSeconds(waitTime);
                p1HealthBarSlider.value--;
            }
        }

        public void Save(ref BoxingData boxingData)
        {
            boxingData.winStreak = winStreak;
        }
        public void Load(BoxingData boxingData)
        {
            winStreak = boxingData.winStreak;
        }
    }

    [System.Serializable]
    public struct BoxingData
    {
        public int winStreak;
    }
}
