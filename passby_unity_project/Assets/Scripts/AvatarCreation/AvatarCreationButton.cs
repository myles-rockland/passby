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

        public void OnClick()
        {
            PlayerController.Instance.SetName(inputField);
            PlayerController.Instance.StartGeneratePlayerId();
            SceneController.Instance.LoadScene("MainHub");
        }
    }
}
