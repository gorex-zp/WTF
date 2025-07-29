using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using System;
using System.Xml.Serialization;

namespace WtfApp
{
    public class WorldObj : BaseObject
    {
        //public WTFHelper.OBJ_MOVE_DIRECTION objMoveDir = WTFHelper.OBJ_MOVE_DIRECTION.NONE;
        public WTFHelper.WORLD_OBJ_TYPE objType;
        public bool isUser;

        public WorldObj(WTFHelper.WORLD_OBJ_TYPE objType,
            int x,
            int y,
            WTFHelper.OBJ_DRAW_DIRECTION? objDirection = WTFHelper.OBJ_DRAW_DIRECTION.TOP,
            bool isUser = false,
            double stepFullTimeInSec = 1.0):base(WTFHelper.worldObjTextures[objType.ToString()], x,y)
        {
            /*if (!System.Enum.IsDefined(typeof(WTFHelper.WORLD_OBJ_TYPE), objType))
                throw new System.Exception("WRONG WORLD OBJECT TYPE");*/
            // base.objColor = Color.FromNonPremultiplied(50, 50, 50, 255);
            this.objType = objType;
            this.isBlocked = objType.isBlocked();
            this.isUser = isUser;
            this.xPos = x;
            this.yPos = y;
            this.objDrawDir = objDirection?? WTFHelper.OBJ_DRAW_DIRECTION.TOP;          
        }

      /*  public WorldObj(GraphicsDevice graphicsDevice,
            string name,
            int x,
            int y,
            bool isBlocked = false,
            bool isUser = false,
            WTFHelper.OBJ_DIRECTION objDirection=WTFHelper.OBJ_DIRECTION.TOP,
            int size = 20,
            double stepFullTimeInSec = 1.0)
        {
            if (!WTFHelper.WORLD_OBJ_NAME.Any(s => s.Equals(name, System.StringComparison.OrdinalIgnoreCase)))
                throw new System.Exception("WRONG WORLD OBJECT NAME");

            this.isBlocked = isBlocked;
            this.isUser = isUser;
            this.xPos = x;
            this.yPos = y;
            this.name = name;
            this.size = size;
            this.objDirection = objDirection;
        }    */

        public override void Update(double stepProgress)
        {
            base.Update(stepProgress);
        }
        public override void NewStep()
        {
            base.NewStep();
            isBlocked = objType.isBlocked();
        }
        public override void Draw(SpriteBatch spriteBatch, float layer)
        {
            base.Draw(spriteBatch,  layer);
        }
    }
}
