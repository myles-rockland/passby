using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PassBy
{
    public class Passerby
    {
        public string Name { get; set; }
        public Avatar Avatar { get; set; }

        public Passerby() 
        {
            Avatar = new Avatar();
        }
    }
}
