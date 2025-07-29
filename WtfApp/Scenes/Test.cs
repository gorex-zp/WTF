using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Gui;
using MonoGame.Extended.Gui.Controls;
using WtfApp.GUI;

namespace WtfApp.Scenes
{
    public class Test : Scene
    {
       

    
        public Test(Rectangle sceneRectangle) : base(WTFHelper.SCENES.TEST, sceneRectangle)
        {
            GuiExtendet
        }
        public override void ButtonStateChanged(Button sender)
        {
            base.ButtonStateChanged(sender);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }

        public override void Touch(Point touch, ButtonState touchState, bool isPressedMove)
        {
            base.Touch(touch, touchState, isPressedMove);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
}
