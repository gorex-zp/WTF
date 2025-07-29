using System;
using System.Collections.Generic;
using System.Text;

namespace WtfApp.App2.Objects
{
    class SurfaceWater
    {
        //liters in cube
        public float volume = 0.0f;
        public float salt = 0.0f;
        public SurfaceWater(float volume)
        {
            this.volume = volume;
        }
    }
}
