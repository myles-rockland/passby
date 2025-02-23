using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PassBy
{
    public class MinigamesButton : MonoBehaviour
    {
        [SerializeField]
        private GameObject minigameFade;
        [SerializeField]
        private GameObject friendsMenuFade;
        public void OnClick()
        {
            friendsMenuFade.SetActive(false);
            minigameFade.SetActive(!minigameFade.activeInHierarchy);
        }
    }
}
