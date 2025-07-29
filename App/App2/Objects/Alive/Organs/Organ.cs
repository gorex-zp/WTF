using System;
using System.Collections.Generic;
using System.Text;

namespace WtfApp.App2.Objects.Alive.Organs
{
    abstract class Organ
    {
        public enum DamageType { None, Light, Middle, Heavy, Critical, Destroyed }
        public enum DiseaseType { None, Light, Middle, Heavy, Critical, Lethal }
        //public enum HealthState { Healthy, Middle, Critical,  }

        public DamageType damageType;
        public DiseaseType diseaseType;

        //1 - max health;
        public float health;

        public float agingCof = 0.00001f;
        //1 - max aging
        public float agingVal;
    }
}
