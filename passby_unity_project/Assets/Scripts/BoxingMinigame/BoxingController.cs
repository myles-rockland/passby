using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PassBy
{
    public class BoxingController : MonoBehaviour
    {
        [SerializeField]
        Slider p1HealthBarSlider;
        [SerializeField]
        Slider p2HealthBarSlider;
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

        // Start is called before the first frame update
        void Start()
        {
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
                SpriteRenderer p2Body = p1Avatar.transform.GetChild(0).GetComponent<SpriteRenderer>();
                string player2Body = p2Passerby.Avatar.BodyType;
                p2Body.sprite = Resources.Load<Sprite>("Art/kenney_shape-characters/PNG/Default/" + player2Body);
                // Left Hand
                SpriteRenderer p2LeftHand = p1Avatar.transform.GetChild(2).GetComponent<SpriteRenderer>();
                string player2LeftHand = p2Passerby.Avatar.LeftHandColour;
                p2LeftHand.sprite = Resources.Load<Sprite>("Art/kenney_shape-characters/PNG/Default/" + player2LeftHand);
                // Right Hand
                SpriteRenderer p2RightHand = p1Avatar.transform.GetChild(3).GetComponent<SpriteRenderer>();
                string player2RightHand = p2Passerby.Avatar.RightHandColour;
                p2RightHand.sprite = Resources.Load<Sprite>("Art/kenney_shape-characters/PNG/Default/" + player2RightHand);

                // Enable Start button
                startButton.SetActive(true);

                // Enable P2 Avatar
                p2Avatar.SetActive(true);
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (gameRunning)
            {
                // Check both players' health. If 0 or less, output whether player 1 wins or loses.
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
                }
                else if (p2HealthBarSlider.value <= 0)
                {
                    // P1 wins
                    middleText.text = "Victory!";
                    gameRunning = false;
                    backToHubButton.SetActive(true);
                }
                if (Input.touchCount > 0)
                {
                    Touch touch = Input.GetTouch(0);
                    if (touch.phase == TouchPhase.Began)
                    {
                        p2HealthBarSlider.value--;
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
    }
}
