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

    public class Button : BaseGuiComponent
    {
        public enum State
        {
            Default,
            Pressed,
            Released,
            PressedHold
        }

        private Vector2 _textSize;
        private int _touchID;
        private double _pressedTime;

        public Color classicButtonTextColor;
        public Color classicButtonDefaultColor;
        public Color classicButtonPressedColor;


        public string  Name
        {
            get { return _name; }
           // private set { _name = value; }
        }

        private string _text;
        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                _textSize = DrawHelper.spriteFont.MeasureString(_text);
            }
        }
        public State state { get; private set; }
        private Dictionary<State, Texture2D> _textures;

        public Button(string name,string text,Rectangle rectangle, Texture2D defaultTexture=null, Texture2D pressedTexture = null)
        {

            this.borderColor = Color.FromNonPremultiplied(100, 255, 100, WTFHelper.alpha);
            this.classicButtonDefaultColor = Color.FromNonPremultiplied(60, 60, 60, WTFHelper.alpha);
            ChangeDefaultColor(this.classicButtonDefaultColor);
            //this.classicButtonPressedColor = Color.FromNonPremultiplied(100, 100, 100, WTFHelper.alpha);
            //this.classicButtonTextColor = Color.FromNonPremultiplied(90, 90, 90, WTFHelper.alpha);

            this.borderStyle = BorderStyle.NONE;
            this._pressedTime = 0.0;
            this._name = name;
            this.Text = text;
            _rectangle = rectangle;
            _textures = new Dictionary<State, Texture2D>
            {
                { State.Default, defaultTexture??DrawHelper.emptyTexture },
                { State.Pressed, pressedTexture??DrawHelper.emptyTexture },
                { State.PressedHold, pressedTexture??DrawHelper.emptyTexture }
            };
            //if(defaultTexture == null && pressedTexture==null && releasedTexture==null){ }
        }

        public void ChangeDefaultColor(Color color)
        {
            this.classicButtonDefaultColor = color;
            this.classicButtonPressedColor = Color.Multiply(color, 0.3f);
            this.classicButtonTextColor = Color.Multiply(color, 1.5f);
        }

        public delegate void ButtonStateChanged(Button sender);
        public event ButtonStateChanged buttonStateChanged;

        public override bool Touch(Point touch,ButtonState touchState,bool isPressedMove)
        {
            if (touchState == ButtonState.Pressed && isPressedMove==false)
            {
                if (_rectangle.Contains(touch))
                {
                    state = State.Pressed;
                    buttonStateChanged?.Invoke(this);
                    return true;
                }
            }
            else if(touchState == ButtonState.Released && state == State.Pressed)
            {
                if (_rectangle.Contains(touch))
                {
                    state = State.Released;
                    buttonStateChanged?.Invoke(this);
                }
                else
                    state = State.Default;
            }
            return false;
        }

        public override void Update(GameTime gameTime)
        {
            if(state==State.Pressed)
            {
                if (_pressedTime >= 1000)
                {
                    state = State.PressedHold;
                    buttonStateChanged?.Invoke(this);
                    state = State.Pressed;
                    _pressedTime = 0.0;
                }
                _pressedTime += gameTime.ElapsedGameTime.TotalMilliseconds;
            }
            else
            {
                _pressedTime = 0.0;
            }
        }
        // Make sure Begin is called on s before you call this function
        public override void Draw(SpriteBatch spriteBatch, float layer)
        {
            switch (state)
            {
                case State.Released: 
                case State.Default:
                    spriteBatch.Draw(_textures[State.Default],destinationRectangle: _rectangle, color: classicButtonDefaultColor, layerDepth: layer);
                    if (!string.IsNullOrEmpty(Text))
                        spriteBatch.DrawString(DrawHelper.spriteFont, _text, _rectangle.Center.ToVector2() - _textSize / 2, classicButtonTextColor, 0, Vector2.Zero,Vector2.One, SpriteEffects.None, layer + layer/10);
                    break;
                /*case State.Hover:
                    spriteBatch.Draw(_textures[state], _rectangle, WTFHelper.buttonHoverColor);
                    if (!string.IsNullOrEmpty(text))
                        spriteBatch.DrawString(DrawHelper.spriteFont, _text, _rectangle.Center.ToVector2() - _textSize / 2, WTFHelper.buttonTextColor);
                    break;*/
                case State.PressedHold:
                case State.Pressed:
                    spriteBatch.Draw(_textures[state], destinationRectangle: _rectangle, color: classicButtonPressedColor, layerDepth: layer);
                    if (!string.IsNullOrEmpty(Text))
                        spriteBatch.DrawString(DrawHelper.spriteFont, _text, _rectangle.Center.ToVector2() - _textSize / 2, classicButtonTextColor, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, layer + layer/10);
                    break;
                //case State.Released: spriteBatch.Draw(_textures[state], _rectangle, WTFHelper.buttonReleassedColor); break;
            }

            switch(borderStyle)
            {
                case BorderStyle.SOLID: DrawHelper.DrawRectagle(spriteBatch, borderColor, _rectangle, (int)(1/Main.screenScale),layer+ layer/10); break;
            }
        }
    }
}
