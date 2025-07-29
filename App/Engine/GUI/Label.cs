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
    class Label : GuiObject
    {
        AlignXY textAlign;

        private Rectangle _rectangle;
        private Vector2 _textSize;
        private string _text;
        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                _textSize = _font.MeasureString(_text);
            }
        }

        private SpriteFont _font;
        public Color borderColor;
        public bool isDrawBorder;
        public Color textColor;
        public Color classicButtonTextColor;

        public Label(string name, string text, Rectangle rectrectangle, SpriteFont font, Color textColor, AlignXY textAlign = AlignXY.LEFT_TOP):base(name,GuiObjectType.LABEL)
        {
            this._font = font;
            this.Text = text;
            this.textAlign = textAlign;
            this._rectangle = rectrectangle;
            this.textColor = textColor;

            this.isDrawBorder = false;
            this.borderColor = Color.FromNonPremultiplied(100, 255, 100, WTFHelper.alpha);
        }

        public override void Draw(SpriteBatch spriteBatch, float layer)
        {
            switch (borderStyle)
            {
                case BorderStyle.NONE: break;
                case BorderStyle.SOLID: DrawHelper.DrawRectagle(spriteBatch, borderColor, _rectangle, 3,layer); break;
            }

            if (!string.IsNullOrEmpty(Text))
                spriteBatch.DrawString(_font, _text, _rectangle.Center.ToVector2() - _textSize / 2, textColor, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, layer + layer/10);
        }

       public override void Update(GameTime gameTime)
        {
            //throw new NotImplementedException();
        }

        public override bool Touch(Point touch, ButtonState touchState, bool isPressedMove)
        {
            //throw new NotImplementedException();
            return false;
        }
    }
}
