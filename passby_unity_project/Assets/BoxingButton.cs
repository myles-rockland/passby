using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PassBy
{
    public class BoxingButton : MonoBehaviour
    {
        public void OnClick()
        {
            SceneController.Instance.LoadScene("BoxingMinigame");
        }
    }
}
