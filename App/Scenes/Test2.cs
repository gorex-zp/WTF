using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using WtfApp.GUI;
using WtfApp.Engine.Scene;

namespace WtfApp.Scenes
{
    public class Test2 : Scene
    {
        Label l;
        public Test2(Rectangle sceneRectangle) : base(WTFHelper.SCENES.TEST1, sceneRectangle)
        {
            l = new Label("LABEL1", "TEST", new Rectangle(0, 0, 200, 100), DrawHelper.spriteFont, Color.Red, AlignXY.RIGHT_BOTTOM);
            AddComponent(new Button("BACK", "BACK", new Rectangle(App.screenBounds.Right - 320, App.screenBounds.Bottom - 170, 300, 150)));
        }
        public override void GUIStateChanged(GuiObject sender)
        {
            if (sender.state == Button.State.Released)
                switch (sender.Name)
                {
                    case "BACK":
                        App.GoToScene(WTFHelper.SCENES.MAIN_MENU);
                        break;
                }

            base.GUIStateChanged(sender);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            l.Draw(spriteBatch, WTFHelper.DRAW_LAYER.GUI.F());

            //spriteBatch.DrawString(DrawHelper.spriteFont, testObj.stepProgress.ToString(), new Vector2(100, 700), Color.Black);
            /*float multiScale = 0.3f;
              Color lerpColor = Color.White;

              for(int i=0;i<25;i++)
              {
                  spriteBatch.Draw(DrawHelper.GetTexture(), new Rectangle(i * 70, 5, 70, 70), Color.FromNonPremultiplied(i * 10, i * 10, i * 10,255));
                  spriteBatch.Draw(DrawHelper.GetTexture(), new Rectangle(i * 70, 75, 70, 70), Color.FromNonPremultiplied(i * 10, 0, 0, 255));
                  spriteBatch.Draw(DrawHelper.GetTexture(), new Rectangle(i * 70, 145, 70, 70), Color.FromNonPremultiplied(0, i * 10, 0, 255));
                  spriteBatch.Draw(DrawHelper.GetTexture(), new Rectangle(i * 70, 215, 70, 70), Color.FromNonPremultiplied(0, 0, i * 10, 255));

                  spriteBatch.Draw(DrawHelper.GetTexture(), new Rectangle(i * 70, 285, 70, 70), Color.Multiply(Color.FromNonPremultiplied(i * 10, i * 10, i * 10, 255), multiScale));
                  spriteBatch.Draw(DrawHelper.GetTexture(), new Rectangle(i * 70, 355, 70, 70), Color.Multiply(Color.FromNonPremultiplied(i * 10, 0, 0, 255), multiScale));
                  spriteBatch.Draw(DrawHelper.GetTexture(), new Rectangle(i * 70, 425, 70, 70), Color.Multiply(Color.FromNonPremultiplied(0, i * 10, 0, 255), multiScale));
                  spriteBatch.Draw(DrawHelper.GetTexture(), new Rectangle(i * 70, 495, 70, 70), Color.Multiply(Color.FromNonPremultiplied(0, 0, i * 10, 255), multiScale));
              }
              */
        }

        public override void Touch(Point touch, ButtonState touchState, bool isPressedMove)
        {
            base.Touch(touch, touchState, isPressedMove);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            int stepTimeInMs = 3000;

            double stepProgress = 1.0 / stepTimeInMs * (gameTime.TotalGameTime.TotalMilliseconds% stepTimeInMs);
        }
    }
}
