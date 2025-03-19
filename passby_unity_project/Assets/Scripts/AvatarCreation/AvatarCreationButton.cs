using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace PassBy
{
    public class AvatarCreationButton : MonoBehaviour
    {
        [SerializeField]
        TMP_InputField inputField;
        [SerializeField]
        Animator connectionWarningAnimator;

        public void OnClick()
        {
            StartCoroutine(SetPlayerDetails());
        }

        IEnumerator SetPlayerDetails()
        {
            connectionWarningAnimator.SetTrigger("FadeOut"); // FadeOut if needed
            yield return new WaitForSeconds(0.5f);

            // Set player details
            Debug.Log("Setting player details");
            PlayerController.Instance.SetName(inputField);
            PlayerController.Instance.StartGeneratePlayerId();
            SaveController.Instance.Save();

            yield return new WaitForSeconds(0.5f); // Give the game time to connect to the server and generate a player id... race condition?

            if (PlayerController.Instance.Passerby.ID < 0)
            {
                connectionWarningAnimator.ResetTrigger("FadeOut");
                connectionWarningAnimator.SetTrigger("FadeIn");
            }
            else
            {
                SceneController.Instance.LoadScene("MainHub");
            }
        }
    }
}
