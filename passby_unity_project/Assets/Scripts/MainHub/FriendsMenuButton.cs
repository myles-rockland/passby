using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PassBy
{
    public class FriendsMenuButton : MonoBehaviour
    {
        [SerializeField]
        private GameObject minigameFade;
        [SerializeField]
        private GameObject friendsMenuFade;
        public void OnClick()
        {
            minigameFade.SetActive(false);
            friendsMenuFade.SetActive(!friendsMenuFade.activeInHierarchy);
            if (friendsMenuFade.activeInHierarchy)
            {
                SceneController.Instance.FillFriendRequestsMenu();
            }
        }
    }
}
