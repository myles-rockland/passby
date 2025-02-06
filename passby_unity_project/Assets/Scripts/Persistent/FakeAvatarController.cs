using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace PassBy
{
    public class FakeAvatarController : MonoBehaviour
    {
        Passerby Bonnie;
        Passerby Benson;
        Passerby Jake;
        List<Passerby> FakePassersby;
        public static FakeAvatarController Instance { get; private set; }
        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject); // Prevent duplicates
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        private void Start()
        {
            // Initialise variables
            Bonnie = new Passerby();
            Benson = new Passerby();
            Jake = new Passerby();
            FakePassersby = new List<Passerby>();

            // Set Bonnie passerby
            Bonnie.Name = "Bonnie";
            Bonnie.ID = -1; // ID shouldn't really matter for fake passersby, so it can be negative.
            Bonnie.Avatar.BodyType = "pink_body_circle";
            Bonnie.Avatar.LeftHandColour = "pink_hand_open";
            Bonnie.Avatar.RightHandColour = "pink_hand_open";

            // Set Benson passerby
            Benson.Name = "Benson";
            Benson.ID = -2;
            Benson.Avatar.BodyType = "pink_body_circle";
            Benson.Avatar.LeftHandColour = "red_hand_closed";
            Benson.Avatar.RightHandColour = "red_hand_closed";

            // Set Jake passerby
            Jake.Name = "Jake";
            Jake.ID = -3;
            Jake.Avatar.BodyType = "yellow_body_circle";
            Jake.Avatar.LeftHandColour = "hand_yellow_closed";
            Jake.Avatar.RightHandColour = "hand_yellow_closed";

            // Add fake passersby to list
            FakePassersby.Add(Bonnie);
            FakePassersby.Add(Benson);
            FakePassersby.Add(Jake);
        }
        public Passerby GetRandomFakePasserby()
        {
            // Return a random fake passerby from the list
            int index = Random.Range(0, FakePassersby.Count);
            return FakePassersby[index];
        }
    }
}
