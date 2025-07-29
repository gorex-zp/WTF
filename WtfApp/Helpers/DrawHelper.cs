using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WtfApp
{
    /*
     * Содержит поля и методы использующиеся в/для ф. Draw(), но не имющих зависимостей для текущего проекта.
     */

    static class  DrawHelper
    {
        public static Texture2D emptyTexture;
        public static SpriteFont spriteFont;

        public static Vector2 xPlus = new Vector2(1, 0);
        public static Vector2 xMinus = new Vector2(-1, 0);
        public static Vector2 yPlus = new Vector2(0, 1);
        public static Vector2 yMinus = new Vector2(0, -1);

        public struct RectagleF
        {

        }

        //много жрёт
        public static Texture2D SaveAsRotatedTexture2D(Texture2D input, bool dirRight)
        {
            Texture2D flipped = new Texture2D(input.GraphicsDevice, input.Width, input.Height);
            Color[] data = new Color[input.Width * input.Height];
            Color[] flipped_data = new Color[data.Length];
            input.GetData<Color>(data);

            for (int y = 0; y < input.Height; y++)
            { 
                for (int x = 0; x < input.Width; x++)
                {              
                    if (dirRight)
                    {
                        flipped_data[(x + 1) * input.Width - y - 1] = data[x + (y * input.Width)];
                    }
                    else
                    {
                        flipped_data[((input.Width - x - 1) * input.Width) + y] = data[x + (y * input.Width)];
                    }
                    /*int index = 0;
                    if (     horizontal && vertical)
                        index = input.Width - 1 - x + (input.Height - 1 - y) * input.Width;
                    else if (horizontal && !vertical)
                        index = input.Width - 1 - x + y * input.Width;
                    else if (!horizontal && vertical)
                        index = x + (input.Height - 1 - y) * input.Width;
                    else if (!horizontal && !vertical)
                        index = x + y * input.Width;
                        
                    flipped_data[x + y * input.Width] = data[index];*/
                }
            }
            flipped.SetData<Color>(flipped_data);

            data = null;
            flipped_data = null;

            return flipped;
        }
        public static void DrawRectagle(SpriteBatch spriteBatch, Color color, Rectangle rect, int borderWidth)
        {
            DrawRectagle(spriteBatch, color, rect.X, rect.Y, rect.Width, rect.Height, borderWidth);
        }
        public static void DrawRectagle(SpriteBatch spriteBatch, Color color, Rectangle rect, int borderWidth, float layer)
        {
            DrawRectagle(spriteBatch, color, rect.X, rect.Y, rect.Width, rect.Height, borderWidth,layer);
        }
        public static void DrawRectagle(SpriteBatch spriteBatch, Color color, int x,int y, int width, int height, int borderWidth, float layer = 0)
        {
            //int gBorder =  (borderWidth >= Math.Ceiling(height / 2.0) ? (int)Math.Ceiling(height / 2.0) : borderWidth);
            //int vBorder =  (borderWidth >= Math.Ceiling(width / 2.0)  ? (int)Math.Ceiling(width / 2.0)  : borderWidth);

            int gBorder = (borderWidth >= height / 2 ? height / 2 : borderWidth);
            int vBorder = (borderWidth >= width / 2? width / 2 : borderWidth);
            
            spriteBatch.Draw(emptyTexture, destinationRectangle: new Rectangle(x, y, width, gBorder), color: color,layerDepth: layer);                                     //верхняя
            spriteBatch.Draw(emptyTexture, destinationRectangle: new Rectangle(x, y+gBorder, vBorder, height- gBorder*2), color: color, layerDepth: layer);                 //левая
            spriteBatch.Draw(emptyTexture, destinationRectangle: new Rectangle(x+width-vBorder, y+gBorder, vBorder, height-gBorder*2), color: color, layerDepth: layer);    //правая
            spriteBatch.Draw(emptyTexture, destinationRectangle: new Rectangle(x, y+height- gBorder, width, gBorder), color: color, layerDepth: layer);                     //нижняя
        }

        public static VertexPositionNormalTexture[] GetCube(float posCX, float posCY, float posCZ, float size)
        {

            /*
              z+  y+
              |  /
              | /
      x- _ _ _|/_ _ _ x+
              /
             /
            / 
           y-
            */
            VertexPositionNormalTexture[] floorVerts = new VertexPositionNormalTexture[36];

            //низ
            floorVerts[0].Position = new Vector3(posCX-size/2, posCY-size/2, posCZ-size/2);
            floorVerts[1].Position = new Vector3(posCX - size / 2, posCY + size / 2, posCZ - size / 2);
            floorVerts[2].Position = new Vector3(posCX + size / 2, posCY - size / 2, posCZ - size / 2);

            floorVerts[3].Position = floorVerts[1].Position;
            floorVerts[4].Position = new Vector3(posCX + size / 2, posCY + size / 2, posCZ - size / 2);
            floorVerts[5].Position = floorVerts[2].Position;

            //верх
            floorVerts[6].Position = new Vector3(posCX - size / 2, posCY - size / 2, posCZ + size / 2);
            floorVerts[7].Position = new Vector3(posCX - size / 2, posCY + size / 2, posCZ + size / 2);
            floorVerts[8].Position = new Vector3(posCX + size / 2, posCY - size / 2, posCZ + size / 2);

            floorVerts[9].Position = floorVerts[7].Position;
            floorVerts[10].Position = new Vector3(posCX + size / 2, posCY + size / 2, posCZ + size / 2);
            floorVerts[11].Position = floorVerts[8].Position;
           
            //лево
            floorVerts[12].Position = new Vector3(posCX - size / 2, posCY - size / 2, posCZ - size / 2);
            floorVerts[13].Position = new Vector3(posCX - size / 2, posCY + size / 2, posCZ - size / 2);
            floorVerts[14].Position = new Vector3(posCX - size / 2, posCY - size / 2, posCZ + size / 2);

            floorVerts[15].Position = floorVerts[13].Position;
            floorVerts[16].Position = new Vector3(posCX - size / 2, posCY + size / 2, posCZ + size / 2);
            floorVerts[17].Position = floorVerts[14].Position;

            //право
            floorVerts[18].Position = new Vector3(posCX + size / 2, posCY - size / 2, posCZ - size / 2);
            floorVerts[19].Position = new Vector3(posCX + size / 2, posCY + size / 2, posCZ - size / 2);
            floorVerts[20].Position = new Vector3(posCX + size / 2, posCY - size / 2, posCZ + size / 2);

            floorVerts[21].Position = floorVerts[19].Position;
            floorVerts[22].Position = new Vector3(posCX + size / 2, posCY + size / 2, posCZ + size / 2);
            floorVerts[23].Position = floorVerts[20].Position;

            //задняя стенка
            floorVerts[24].Position = new Vector3(posCX - size * 2, posCY + size / 2, posCZ - size / 2);
            floorVerts[25].Position = new Vector3(posCX + size / 2, posCY + size / 2, posCZ - size / 2);
            floorVerts[26].Position = new Vector3(posCX - size / 2, posCY + size / 2, posCZ + size / 2);

            floorVerts[27].Position = floorVerts[25].Position;
            floorVerts[28].Position = new Vector3(posCX + size / 2, posCY + size / 2, posCZ + size / 2);
            floorVerts[29].Position = floorVerts[26].Position;

            //передняя стенка
            floorVerts[30].Position = new Vector3(posCX - size / 2, posCY - size / 2, posCZ - size / 2);
            floorVerts[31].Position = new Vector3(posCX + size / 2, posCY - size / 2, posCZ - size / 2);
            floorVerts[32].Position = new Vector3(posCX - size / 2, posCY - size / 2, posCZ + size / 2);

            floorVerts[33].Position = floorVerts[31].Position;
            floorVerts[34].Position = new Vector3(posCX + size / 2, posCY - size / 2, posCZ + size / 2);
            floorVerts[35].Position = floorVerts[32].Position;
            return floorVerts;
        }
    }
}
