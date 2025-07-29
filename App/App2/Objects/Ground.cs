using System;
using System.Collections.Generic;
using System.Text;

namespace WtfApp.App2.Objects
{
   
    public class Ground
    {
        public enum GroundType
        {
            Earth,
            Stone,
            Clay,
            Sand
        }

        public GroundType groundType;

        public Ground(GroundType groundType)
        {
            this.groundType = groundType;
        }
    }
    
}
