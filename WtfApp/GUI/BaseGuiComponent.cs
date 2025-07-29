using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

namespace WtfApp.GUI
{
    public enum AlignXY : int { LEFT_TOP, LEFT_CENTER, LEFT_BOTTOM, CENTER_TOP, CENTER_CENTER, CENTER_BOTTOM, RIGHT_TOP, RIGHT_CENTER, RIGHT_BOTTOM }
    public enum BorderStyle : int { SOLID, TEXTURE, NONE }

    public abstract class BaseGuiComponent
    {
        
        protected Rectangle _rectangle;
        public string _name { get; protected set; }
        public Color borderColor;
        
        public BorderStyle borderStyle { get; set; }

        public abstract void Draw(SpriteBatch spriteBatch, float layer);
        public abstract void Update(GameTime gameTime);
        public abstract bool Touch(Point touch, ButtonState touchState, bool isPressedMove);

    }

    public static class GuiExtension
    {
        public static Button GetButton(this List<Button> buttonList, string buttonName)
        {
            foreach (var button in buttonList)
            {
                if (button.Name == buttonName)
                    return button;
            }
            return null;
        }
    }

}
