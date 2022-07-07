using System;
using System.Collections.Generic;
using System.Text;

namespace DotnetOsrsApiWrapper
{
    public class Skill : IPlayerInfoProperty
    {
        //do not re-order this or it will break it all
        public string Name { get; set;}
        public int Rank { get; set; }
        public int Level { get; set; }
        public int Experience { get; set; }
    }
}
