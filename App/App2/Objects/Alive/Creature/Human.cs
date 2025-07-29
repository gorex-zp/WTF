using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WtfApp.App2.Objects.Alive.Creature
{
    class Human : Interface.IMovable, Interface.IDrawable 
    {
        //enum has percent value from 0 to 100, from lowest to higest rank
        /*struct PhysicalFatigueType
        {
            const float Light = 0.2f;
            const float Middle = 0.5f;
            const float Heavy = 0.7f;
        }*/
        
        public enum PhysicalFatigueType : byte
        {
            Light  = 20,
            Middle = 50,
            Heavy  = 70
        }
        private Array _physicalFatigueTypesValues = Enum.GetValues(typeof(PhysicalFatigueType));
        public PhysicalFatigueType currPhysicalFatigueType
        {
            get
            {
                foreach (var v in _physicalFatigueTypesValues)
                {
                    if ((float)v / 100f >= physicalFatigue)
                    {
                        currPhysicalFatigueType = (PhysicalFatigueType)v;
                    }
                }
                return currPhysicalFatigueType;
            }
            private set
            {
                currPhysicalFatigueType = value;
                physicalFatigue = (float)currPhysicalFatigueType / 100f;
            }
        }

        #region Movable
        public Vector2 moveTo
        {   
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }
        #endregion
        private Vector2 pos;

        //body parts/organs
        private MoveType moveType;
        private BodyParts.Body body;
        private BodyParts.Head head;
        private List<BodyParts.Hand> hands;
        private List<BodyParts.Leg> legs;

        //human state 0 - min, 1 - max
        //сытость
        private float satiety;
        //физ. усталость
        private float physicalFatigue;
        //потребность в сне
        private float cheerfulnessFatigue;

        public Human(Vector2 pos)
        {
            hands = new List<BodyParts.Hand>(2);
            legs = new List<BodyParts.Leg>(2);
            head = new BodyParts.Head();
            body = new BodyParts.Body(pos);

            for(int i=0;i< legs.Capacity;i++)
            {
                legs.Add(new BodyParts.Leg());
            }
            for (int i = 0; i < hands.Capacity; i++)
            {
                hands.Add(new BodyParts.Hand());
            }
            SetMoveType();
        }

        public void Draw(SpriteBatch spriteBatch, Point cellSize)
        {
            throw new NotImplementedException();
        }

        public void Move()
        {
            throw new NotImplementedException();
        }

        //spriteBatch.DrawLine(new Vector2(cellSize.X * x + cellSize.X / 2, cellSize.Y * y + cellSize.Y / 5), new Vector2(cellSize.X * x + cellSize.X / 2, cellSize.Y * (y + 1) - cellSize.Y / 5), Color.Black, 3);

        private void SetMoveType()
        {
            if (legs.Count != 0)
            {
                moveType = MoveType.Leg;
            }
            else if (legs.Count == 0 && hands.Count != 0)
            {
                moveType = MoveType.Hand;
            }
            else if (legs.Count == 0 && hands.Count == 0)
            {
                moveType = MoveType.Immovable;
            }  
        }
    }
}
