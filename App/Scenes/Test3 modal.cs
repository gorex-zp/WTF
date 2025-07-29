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
    public class Test3 : Scene
    {
        public Test3(Rectangle sceneRectangle) : base(WTFHelper.SCENES.TEST1, sceneRectangle)
        {

            
            Label l = new Label("LABEL1", "TEST", new Rectangle(sceneRectangle.Left, sceneRectangle.Top, 200, 100), DrawHelper.spriteFontBig, Color.Red, AlignXY.RIGHT_BOTTOM);
            Button b1 = new Button("OK", "Ok", new Rectangle(sceneRectangle.Left + 20, sceneRectangle.Bottom - 120, (sceneRectangle.Width - 60) / 2, 100));
            Button b2 = new Button("NO", "No", new Rectangle(sceneRectangle.Right - (sceneRectangle.Width - 60) / 2-20, sceneRectangle.Bottom - 120, (sceneRectangle.Width - 60) / 2, 100));
            AddComponent(l);
            AddComponent(b1);
            AddComponent(b2);
        }
        public override void GUIStateChanged(GuiObject sender)
        {
            base.GUIStateChanged(sender);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            //l.Draw(spriteBatch, WTFHelper.DRAW_LAYER.GUI.F());

            spriteBatch.Draw(DrawHelper.GetTexture(), sceneRectangle, Color.FromNonPremultiplied(150, 100, 100, WTFHelper.alpha));
            DrawHelper.DrawRectagle(spriteBatch, Color.FromNonPremultiplied(200, 200, 200, WTFHelper.alpha), sceneRectangle, 4);

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
              }*/
        }

        public override void Touch(Point touch, ButtonState touchState, bool isPressedMove)
        {
            base.Touch(touch, touchState, isPressedMove);
            App.GoToPrevScene();
            //App.GoToScene(App.prev, true);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            int stepTimeInMs = 3000;

            double stepProgress = 1.0 / stepTimeInMs * (gameTime.TotalGameTime.TotalMilliseconds% stepTimeInMs);
        }
    }
}
