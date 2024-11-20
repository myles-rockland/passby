using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PassBy
{
    [System.Serializable]
    public class Passerby
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public Avatar Avatar { get; set; }

        public Passerby() 
        {
            ID = -1;
            Avatar = new Avatar();
        }
    }
}
