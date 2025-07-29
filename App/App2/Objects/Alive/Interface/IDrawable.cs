using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WtfApp.App2.Objects.Alive.Interface
{
    interface IDrawable
    {
        void Draw(SpriteBatch spriteBatch, Point cellSize);
    }
}
