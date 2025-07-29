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
    public enum BorderStyle : int { NONE, SOLID, TEXTURE }
    public enum GuiObjectType : int { BUTTON, CONTAINER, LABEL }

    public abstract class GuiObject
    {
        public enum State
        {
            Default,
            Disabled,
            Pressed,
            Released,
            PressedHold
        }
        public State state { get; protected set; } = State.Default;

        public GuiObjectType objectType;
        //область позиционирования объекта
        public Rectangle _rectangle;
        /*public Rectangle Rectangle
        {
            get { return _rectangle; }
            set { _rectangle = value; }
        }*/

        //цвет рамки границы области позиционирования
        public Color borderColor { get; set; }
        //стиль рамки
        public BorderStyle borderStyle { get; set; }
        //толщина линии рамки
        public int borderWidth { get; set; }

        //название/id объекта
        public string _name { get; protected set; }
        public string Name
        {
            get { return _name; }
        }

        //задействован ли элемент
        public bool _enabled { get; set; }
        public bool Enabled
        {
            get { return _enabled; }
            set { _enabled = value; }
        }
        public bool Disable()
        {
            if (this.state != State.Disabled)
            {
                this.state = State.Disabled;
                return true;
            }
            return false;
        }
        public bool Enable()
        {
            if (this.state != State.Default)
            {
                this.state = State.Default;
                return true;
            }
            return false;
        }

        public GuiObject(string name, GuiObjectType guiObjectType)
        {
            this._name = name;
            this.objectType = guiObjectType;
        }

        public abstract void Draw(SpriteBatch spriteBatch, float layer);
        public abstract void Update(GameTime gameTime);
        public abstract bool Touch(Point touch, ButtonState touchState, bool isPressedMove);
    }
    /*
    //класс для методов расширений
    public static class GuiExtension
    {
        // получение объекта по имени/id
        public static Button GetButton(this List<GuiObject> buttonList, string buttonName)
        {
            foreach (var button in buttonList)
            {
                if (button.Name == buttonName)
                    return button;
            }
            return null;
        }
    }
    */
}
