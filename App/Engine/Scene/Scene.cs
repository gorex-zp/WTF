using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using WtfApp.GUI;

namespace WtfApp.Engine.Scene
{
    public class Scene
    {
        public enum SceneState
        {
            Running,Paused,Stoped,Background
        }
        public SceneState state;
        public WTFHelper.SCENES sceneType { get; private set; }
        protected Rectangle sceneRectangle { get; set; }
        public Dictionary<string, GuiObject> guiObjects = new Dictionary<string, GuiObject>();
        //public Dictionary<string,Button> sceneButtons = new Dictionary<string, Button>();

        public bool isDebug = false;
        public string debugStr = "";
        public bool hidePrevious = true;
        

        protected Scene(WTFHelper.SCENES sceneType, Rectangle sceneRectangle, bool hidePrevious = true)
        {
            this.guiObjects = new Dictionary<string, GuiObject>();
            this.sceneType = sceneType;
            this.sceneRectangle = sceneRectangle;
            this.hidePrevious = hidePrevious;
            this.state = SceneState.Stoped;
        }

        public void AddComponent(GuiObject guiObject)
        {
            switch (guiObject.objectType)
            {
                case GuiObjectType.BUTTON: (guiObject as Button).buttonStateChanged += GUIStateChanged; break;
            }
            guiObjects.Add(guiObject.Name, guiObject);
        }

        /*public void AddButton(string name,string text, Rectangle rectangle, Texture2D defaultTexture = null, Texture2D pressedTexture = null)
        {
            Button button = new Button(name, text, rectangle, defaultTexture ??DrawHelper.GetTexture(), pressedTexture ??DrawHelper.GetTexture());
            button.buttonStateChanged += GUIStateChanged;
            sceneButtons.Add(name,button);
        }*/

        public virtual void Touch(Point touch, ButtonState touchState, bool isPressedMove)
        {
            if (state != SceneState.Background)
            {
                for(int i=guiObjects.Count-1;i>=0;i--)
                {
                    if (guiObjects.ElementAt(i).Value.objectType == GuiObjectType.BUTTON
                        && guiObjects.ElementAt(i).Value.Touch(touch, touchState, isPressedMove))
                        break;
                }
                /*foreach (var guiObj in guiObjects.Values)
                {
                    if (guiObj.Touch(touch, touchState, isPressedMove))
                        break;
                }*/
                /* for(int i= guiObjects.Count-1;i>=0;i--)
                 {
                     if (guiObjects[i].Touch(touch, touchState, isPressedMove))
                         break;
                 }*/
            }
        }

        public virtual void Update(GameTime gameTime)
        {
            foreach (var guiObj in guiObjects.Values)
            {
                guiObj.Update(gameTime);
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            foreach (var guiObj in guiObjects.Values)
            {
                guiObj.Draw(spriteBatch, WTFHelper.DRAW_LAYER.GUI.F());
            }

            if(isDebug)
            {
                spriteBatch.DrawString(DrawHelper.spriteFont, debugStr,
                    new Vector2(sceneRectangle.Right - DrawHelper.spriteFont.MeasureString(debugStr).X - 5, 1),
                    Color.FromNonPremultiplied(0, 255, 0, WTFHelper.alpha * 20),
                     0, Vector2.Zero, Vector2.One, SpriteEffects.None, WTFHelper.DRAW_LAYER.ABSOLUTE_FRONT.F());
            }
        }

        public virtual void GUIStateChanged(GuiObject sender)
        {

        }
    }
}

