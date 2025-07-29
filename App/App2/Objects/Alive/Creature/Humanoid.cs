using System;
using System.Collections.Generic;
using System.Text;

namespace WtfApp.App2.Objects.Alive
{


    public abstract class Humanoid
    {
        private BodyParts.Body body;
        private BodyParts.Head head;
        private List<BodyParts.Hand> hands;
        private List<BodyParts.Leg> legs;
    }
}
