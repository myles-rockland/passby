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
        bool gameRunning;

        // Start is called before the first frame update
        void Start()
        {
            // Reset game state
            // Reset players' health
            // Start countdown
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
