using System;
using System.Collections.Generic;
using System.Text;

namespace WtfApp.App2.Objects
{
    //only with default values 1,2,3...
    public enum GasType : byte
    {
        Nitrogen,
        Oxygen, 
        //Argon, Ar = Argon,
        CarbonDioxide, 
        Gas,//natural gas
        CarbonMonoxide
    }

    public class FAir
    {
        public static float diffusionCof = 0.001f;
        public float[] gases;

        public float humidity;
        public bool isHasGases = false;

        private static float tmp, tmp2, tmp3;

        public FAir()
        {
            Array v = Enum.GetValues(typeof(GasType));
            gases = new float[v.Length];
         
            /*for(int i=0;i< v.Length;i++)
            {
                gases[i] = 0f;
            }*/
        }

        public void AirToZero()
        {
            for (byte i = 0; i < gases.Length; i++)
            {
                gases[i] = 0f;
            }
            isHasGases = false;
        }

        public void CreateNormalAir()
        {
            AirToZero();
            isHasGases = true;
            humidity = 0.65f;
            gases[(byte)GasType.Nitrogen] = 0.78f;
            //gases[(byte)GasType.CarbonDioxide] = 0.11f;
            gases[(byte)GasType.Oxygen] = 0.22f;
        }

        public void IncreaseGasConcetrationWithOtherPercentDecrease(GasType increasedGasType, float offset)
        {
            float cofForOther = 1 / (1 - gases[(byte)increasedGasType]);
            gases[(byte)increasedGasType] += offset;

            for (byte i = 0; i < gases.Length; i++)
            {
                if (i != (byte)increasedGasType)
                {
                    gases[i] -= offset * cofForOther * gases[i];
                }
            }
        }
        public bool IncreaseGasConcetrationAndDecrease(GasType increasedGasType,GasType decreasedGasType, float offset)
        {

            if (gases[(byte)decreasedGasType] >= offset && gases[(byte)increasedGasType]+offset<=1)
            {
                gases[(byte)increasedGasType] += offset;
                gases[(byte)decreasedGasType] -= offset;
                return true;
            }
            else
                return false;

        }
        public static void  AirInterchange(FAir firstAir, FAir secondAir)
        {
            for (byte i = 0; i < firstAir.gases.Length; i++)
            {
                if (firstAir.gases[i] != 0 || secondAir.gases[i] != 0)
                {
                    tmp = firstAir.gases[i] * diffusionCof;
                    tmp2 = secondAir.gases[i] * diffusionCof;
                    firstAir.gases[i] += tmp2 - tmp;
                    secondAir.gases[i] += tmp - tmp2;
                }
            }
        }
        public static void AirInterchange(FAir mainAir, FAir firstExAir, FAir secondExAir)
        {  
            for (byte i = 0; i < mainAir.gases.Length; i++)
            {
                if (mainAir.gases[i] != 0 || firstExAir.gases[i] != 0 || secondExAir.gases[i] != 0)
                {
                    tmp = mainAir.gases[i] * diffusionCof;
                    tmp2 = firstExAir.gases[i] * diffusionCof;
                    tmp3 = secondExAir.gases[i] * diffusionCof;

                    mainAir.gases[i] += tmp2 + tmp3 - tmp - tmp;

                    firstExAir.gases[i] += tmp-tmp2;
                    secondExAir.gases[i] += tmp-tmp3;
                }
            }
        }
    }
  /*  public class Oxygen: Gas
    {
        public Oxygen():base(GasType.Oxygen){}
    }

    public abstract class Gas
    {
        public const float thermalConductivity= 0.026f;
        public readonly GasType gasType;
        public Gas(GasType gasType)
        {
            this.gasType = gasType;
        }
    }*/
}
