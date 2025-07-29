using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace WtfApp.App3.Objects
{
    class Player : BaseObject
    {
       /* public static Texture2D GetPlayerTexture(GraphicsDevice gd, Point size)
        {
            Texture2D texture = new Texture2D(gd, size.X, size.Y);

            float percentWidth = 0.2f;
            int halfLineWidth = (int)((size.X * percentWidth)/2);

            Color[] data = new Color[size.X*size.Y];
            for (int pixel = 0; pixel < data.Length; pixel++)
            {
                if (pixel%size.Y >= (size.X / 2 - halfLineWidth) && pixel % size.Y <= (size.X / 2 + halfLineWidth))
                    data[pixel] = Color.YellowGreen;
                else
                    data[pixel] = Color.Transparent;
            }
            texture.SetData(data);
            return texture;
        }*/

        public Player(Texture2D texture, int xPos, int yPos) : base(texture, xPos, yPos)
        {

        }

        public override void Update(double stepProgress)
        {
            base.Update(stepProgress);
        }
        public override void Draw(SpriteBatch spriteBatch, float layer)
        {
            base.Draw(spriteBatch, layer);
        }
    }
}
