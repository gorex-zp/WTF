using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;

namespace WtfApp.App3
{
    public class GeneratedObj: BaseObject
    {
        public WTFHelper.GENERATED_OBJ_TYPE objType;
        public bool isRotated = false;
        public bool isProcessed = false;
        public int weldId = 0;
        // public bool isUser;

        public GeneratedObj(WTFHelper.GENERATED_OBJ_TYPE objType,
            int x,
            int y,
            bool isBlocked = false,
            bool isUser = false,
            WTFHelper.OBJ_DRAW_DIRECTION objDirection = WTFHelper.OBJ_DRAW_DIRECTION.TOP) : base(WTFHelper.generatedObjTextures[objType.ToString()],x,y)
        {
            //this.objColor = Color.FromNonPremultiplied(50, 50, 50, 255);
            this.objType=objType;
            this.isBlocked = isBlocked;
            //this.isUser = isUser;
            //this.objType = objType;
            this.objDrawDir = objDirection;
        }

        public override void Update(double stepProgress)
        {
            base.Update(stepProgress);
        }

        public override void Rotate(Point rotateAroundPoint, ROTATE_DIRRECTION rotateDir)
        {
            base.Rotate(rotateAroundPoint, rotateDir);
            this.isRotated = true;
        }

        public override void NewStep()
        {
            if (rotateAt.X == xPos && rotateAt.Y == yPos)
                this.isRotated = true;
            else
                this.isRotated = false;

            base.NewStep();
           // isRotated = false;
            isBlocked = false;
            isProcessed = false;
            isMoveDirectionSet = false;
        }

        public override void Draw(SpriteBatch spriteBatch, float layer)
        {
            base.Draw(spriteBatch, layer);
            //spriteBatch.Draw(WTFHelper.generatedObjTextures[(int)objType],
            //    new Rectangle(xPos * size, yPos * size, size, size),
            //    null,
            //    WTFHelper.objColor,
            //    90f * (byte)objDrawDir,
            //    Vector2.Zero,
            //    SpriteEffects.None,
            //    1f);
        }
    }
}
