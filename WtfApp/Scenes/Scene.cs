using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using WtfApp.GUI;


namespace WtfApp.Scenes
{
    public class Scene
    {
        public WTFHelper.SCENES sceneType { get; private set; }
        protected Rectangle sceneRectangle { get; set; }
        public List<Button> sceneButtons = new List<Button>();

        public bool isActive = true;
        public bool isDebug = false;
        public bool isPause = false;

        protected Scene(WTFHelper.SCENES sceneType,Rectangle sceneRectangle)
        {
            this.sceneType = sceneType;
            this.sceneRectangle = sceneRectangle;
        }

        public void AddButton(string name,string text, Rectangle rectangle, Texture2D defaultTexture = null, Texture2D pressedTexture = null)
        {
            Button button = new Button(name, text, rectangle, defaultTexture ?? DrawHelper.emptyTexture, pressedTexture ?? DrawHelper.emptyTexture);
            button.buttonStateChanged += ButtonStateChanged;
            sceneButtons.Add(button);
        }
        public virtual void ButtonStateChanged(Button sender)
        {

        }
        
        public virtual void Touch(Point touch,ButtonState touchState, bool isPressedMove)
        {
            if (isActive)
            {
                for(int i=sceneButtons.Count-1;i>=0;i--)
                {
                    if (sceneButtons[i].Touch(touch, touchState, isPressedMove))
                        break;
                }
            }
        }

        public virtual void Update(GameTime gameTime)
        {
            foreach (var button in sceneButtons)
            {
                button.Update(gameTime);
            }       
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            foreach (var button in sceneButtons)
            {
                button.Draw(spriteBatch,WTFHelper.DRAW_LAYER.GUI.F());
            }
        }
    }
}
