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

    public class Button : GuiObject
    {
        private Vector2 _textSize;
        private double _pressedTime;

        public Color classicButtonTextColor;
        public Color classicButtonDefaultColor;
        public Color classicButtonPressedColor;

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

        private Dictionary<State, Texture2D> _textures;

        public Button(string name,string text,Rectangle rectangle, Texture2D defaultTexture=null, Texture2D disabledTexture= null, Texture2D pressedTexture = null):base(name,GuiObjectType.BUTTON)
        {
            this.borderColor = Color.FromNonPremultiplied(100, 255, 100, 120);
            this.classicButtonDefaultColor = Color.FromNonPremultiplied(100, 100, 120, 255);
            ChangeDefaultColor(this.classicButtonDefaultColor);
            //this.classicButtonPressedColor = Color.FromNonPremultiplied(100, 100, 100, WTFHelper.alpha);
            //this.classicButtonTextColor = Color.FromNonPremultiplied(90, 90, 90, WTFHelper.alpha);

            this.borderStyle = BorderStyle.NONE;
            this._pressedTime = 0.0;
            this.Text = text;
            _rectangle = rectangle;
            _textures = new Dictionary<State, Texture2D>
            {
                { State.Default, defaultTexture??DrawHelper.GetTexture() },
                { State.Disabled, disabledTexture??DrawHelper.GetTexture() },
                { State.Pressed, pressedTexture??DrawHelper.GetTexture() },
                { State.PressedHold, pressedTexture??DrawHelper.GetTexture() }
            };
            //if(defaultTexture == null && pressedTexture==null && releasedTexture==null){ }
        }

        public void ChangeDefaultColor(Color color)
        {
            this.classicButtonDefaultColor = color;
            this.classicButtonPressedColor = Color.Multiply(color, 0.3f);
            this.classicButtonTextColor = Color.Multiply(color, 2.5f);
        }

        public delegate void GUIStateChanged(GuiObject sender);
        public event GUIStateChanged buttonStateChanged;

        public override bool Touch(Point touch,ButtonState touchState,bool isPressedMove)
        {
            if (this.state != State.Disabled)
            {
                if (touchState == ButtonState.Pressed && isPressedMove == false)
                {
                    if (_rectangle.Contains(touch))
                    {
                        state = State.Pressed;
                        buttonStateChanged?.Invoke(this);
                        return true;
                    }
                }
                else if (touchState == ButtonState.Released && state == State.Pressed)
                {
                    if (_rectangle.Contains(touch))
                    {
                        state = State.Released;
                        buttonStateChanged?.Invoke(this);
                    }
                    else
                        state = State.Default;
                }
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
                    spriteBatch.Draw(_textures[State.Default], destinationRectangle: _rectangle, color: classicButtonDefaultColor/*, layerDepth: layer*/);
                    if (!string.IsNullOrEmpty(Text))
                        spriteBatch.DrawString(DrawHelper.spriteFont, _text, _rectangle.Center.ToVector2() - _textSize / 2, classicButtonTextColor, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, layer + layer / 10);
                    break;
                case State.Disabled:
                    spriteBatch.Draw(_textures[State.Default], destinationRectangle: _rectangle, color: Color.Multiply(classicButtonDefaultColor,0.5f)/*, layerDepth: layer*/);
                    if (!string.IsNullOrEmpty(Text))
                        spriteBatch.DrawString(DrawHelper.spriteFont, _text, _rectangle.Center.ToVector2() - _textSize / 2, Color.Multiply(classicButtonTextColor,0.5f), 0, Vector2.Zero, Vector2.One, SpriteEffects.None, layer + layer / 10);
                    break;
                case State.Default:
                    spriteBatch.Draw(_textures[State.Default],destinationRectangle: _rectangle, color: classicButtonDefaultColor/*, layerDepth: layer*/);
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
                    spriteBatch.Draw(_textures[state], destinationRectangle: _rectangle, color: classicButtonPressedColor/*, layerDepth: layer*/);
                    if (!string.IsNullOrEmpty(Text))
                        spriteBatch.DrawString(DrawHelper.spriteFont, _text, _rectangle.Center.ToVector2() - _textSize / 2, classicButtonTextColor, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, layer + layer/10);
                    break;
                //case State.Released: spriteBatch.Draw(_textures[state], _rectangle, WTFHelper.buttonReleassedColor); break;
            }

            switch(borderStyle)
            {
                case BorderStyle.SOLID: DrawHelper.DrawRectagle(spriteBatch, borderColor, _rectangle, (int)(1/App.screenScale),layer+ layer/10); break;
            }
        }
    }
}