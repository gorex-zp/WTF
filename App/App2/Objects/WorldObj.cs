using System;
using System.Collections.Generic;
using System.Text;

namespace WtfApp.App2.Objects
{
    public class WorldObj
    {
        bool isHasGas = true;
        //GasType;
        float GasTemperature;
        public Ground ground;
        public List<SurfaceObj> surfaceObjs;
        public FAir air;

        public void CreateObj(params object[] param)
        {
            air = new FAir();
            air.CreateNormalAir();
            //ground = new List<Ground>(Enum.GetValues(typeof(Ground)).Length);
        }
    }
}
