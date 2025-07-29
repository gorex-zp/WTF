using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Xml.Serialization;

namespace WtfApp.App1
{
    public abstract class BaseObject
    {

        public enum ROTATE_DIRRECTION
        {
            LEFT = -1, NONE = 0, RIGHT = 1
        }
        private Texture2D Texture;
        public WTFHelper.OBJ_MOVE_DIRECTION objMoveDir = WTFHelper.OBJ_MOVE_DIRECTION.NONE;
        public bool isMoveDirectionSet = false;
        private ROTATE_DIRRECTION rotateDir;
        protected Point rotateAt;      
        protected Texture2D texture
        {
            get
            {
                return Texture;
            }
            set
            {
                Texture = value;
               // textureRotated = DrawHelper.SaveAsRotatedTexture2D(Texture, true);
            }
        }
        public double stepProgress = 0.0;
        public int xPos = 0;
        public int yPos = 0;
        public Rectangle drawRect;
        public bool isBlocked = false;
        public int size = MainG.cellSize;
        public Color objColor = WTFHelper.objColor;
        public WTFHelper.OBJ_DRAW_DIRECTION objDrawDir = WTFHelper.OBJ_DRAW_DIRECTION.TOP;
        private Vector2 _offsetDraw;

        public BaseObject(Texture2D texture,int xPos, int yPos)
        {
            this.xPos = xPos;
            this.yPos = yPos;
            this.texture = texture;
            this.drawRect = new Rectangle(
                                    (int)((xPos - objMoveDir.X()) * size + size / 2),
                                    (int)((yPos - objMoveDir.Y()) * size + size / 2),
                                    size, size);
            this._offsetDraw = Vector2.Zero;
            this.rotateAt = Point.Zero;
            this.rotateDir = ROTATE_DIRRECTION.NONE;
        }

        public void ChangeOffsetDraw(Vector2 newOffsetDraw)
        {
            drawRect.Offset(Vector2.Negate(_offsetDraw ));
            drawRect.Offset(newOffsetDraw);
            _offsetDraw = newOffsetDraw;
        }

        public virtual void Update( double stepProgress)
        {
            this.stepProgress = stepProgress;
            if (rotateDir != ROTATE_DIRRECTION.NONE)
            {
                drawRect.X = (int)((rotateAt.X + (xPos - rotateAt.X) * Math.Cos(MathHelper.PiOver2 * stepProgress) - (yPos - rotateAt.Y) * Math.Sin(MathHelper.PiOver2 * stepProgress * (double)rotateDir)) * size + size / 2);
                drawRect.Y = (int)((rotateAt.Y + (yPos - rotateAt.Y) * Math.Cos(MathHelper.PiOver2 * stepProgress) + (xPos - rotateAt.X) * Math.Sin(MathHelper.PiOver2 * stepProgress * (double)rotateDir)) * size + size / 2);
            }
            else
            {
                drawRect.X = (int)((xPos * size + size / 2) + (objMoveDir.X() * size * stepProgress));
                drawRect.Y = (int)((yPos * size + size / 2) + (objMoveDir.Y() * size * stepProgress));
            }
            drawRect.Offset(_offsetDraw);
            //this.drawRect.Offset(size * (float)curCallProgress, size * (float)curCallProgress);
        }

        public virtual void Rotate(Point rotateAroundPoint,ROTATE_DIRRECTION rotateDir)
        { 
            this.rotateDir = rotateDir;
            this.rotateAt = rotateAroundPoint;
        }
        public virtual void Move(WTFHelper.OBJ_MOVE_DIRECTION moveDir)
        {
            objMoveDir=moveDir;
        }
        
        public virtual void NewStep()
        {            
            //изменение направления отрисовки
            this.objDrawDir += (int)rotateDir;
            if ((int)objDrawDir < 1)
                objDrawDir = WTFHelper.OBJ_DRAW_DIRECTION.LEFT;
            else if((int)objDrawDir > 4)
                objDrawDir = WTFHelper.OBJ_DRAW_DIRECTION.TOP;

            if(rotateDir!=ROTATE_DIRRECTION.NONE)
            {
                int oldX = xPos;
                this.xPos = rotateAt.X - (rotateAt.Y - yPos) * (rotateDir == ROTATE_DIRRECTION.RIGHT ? -1 : 1);
                this.yPos = rotateAt.Y - (rotateAt.X - oldX) * (rotateDir == ROTATE_DIRRECTION.LEFT ? -1 : 1);
            }

            this.xPos += objMoveDir.X();
            this.yPos += objMoveDir.Y();

            //не обнуляем для сохранения состояния "не поворчивать если уже был поворот на этом поворотном блоке)
            //this.rotateAt = Point.Zero;
            this.rotateDir = ROTATE_DIRRECTION.NONE;
            this.objMoveDir = WTFHelper.OBJ_MOVE_DIRECTION.NONE;
        }

        public virtual void Draw(SpriteBatch spriteBatch, float layer)
        {
            spriteBatch.Draw(
                    texture,
                    drawRect,
                    null,
                    objColor,
                    (MathHelper.PiOver2 * ((float)objDrawDir - 1)) + MathHelper.PiOver2 * (int)rotateDir * (float)stepProgress,
                    new Vector2(WTFHelper.textureSize / 2),
                    SpriteEffects.None,
                    layer);

         /*   spriteBatch.Draw((objDrawDir.X() != 0 ? textureRotated : texture), drawRect, null, objColor,0,
                    new Vector2(WTFHelper.textureSize / 2),
                    (objDrawDir.Y() > 0 ? SpriteEffects.FlipVertically : (objDrawDir.X() < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None)),1f);*/
            
        }
    }
}
