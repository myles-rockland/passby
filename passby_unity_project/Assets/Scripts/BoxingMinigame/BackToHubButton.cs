using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PassBy
{
    public class BackToHubButton : MonoBehaviour
    {
        public void OnClick()
        {
            SceneController.Instance.LoadScene("MainHub");
        }
    }
}
