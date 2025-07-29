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

namespace WtfApp.Engine.Scene.Scenes
{
    public class Modal: Scene
    {
        public enum ModalType:int { EMPTY, OK, OK_NO, OK_CANCEL }
        public enum ModalResult:int { OK, NO, CANCEL }

        public ModalResult modalResult=(ModalResult)(-1);
        public ModalType modalType = (ModalType)(-1);

        private int btnInterval = 20;
        private Point btnSize;

        public Modal(string labelText,Rectangle sceneRectangle, ModalType modalType) : base(WTFHelper.SCENES.MODAL_TEXT, sceneRectangle)
        {
            this.modalType = modalType; 
            btnSize = new Point((sceneRectangle.Width - btnInterval*3)/2, 100);

            Label l = new Label("LABEL", labelText, new Rectangle(sceneRectangle.Left, sceneRectangle.Top, sceneRectangle.Width, sceneRectangle.Height-(modalType== ModalType.EMPTY?0 : btnSize.Y-btnInterval)), DrawHelper.spriteFontBig, Color.DarkSlateGray, AlignXY.RIGHT_BOTTOM);
            AddComponent(l);
            switch (modalType)
            {
                case ModalType.OK:
                    AddComponent(new Button("OK", "Ok", new Rectangle(sceneRectangle.Center.X-btnSize.X/2, sceneRectangle.Bottom - btnSize.Y - btnInterval, btnSize.X, btnSize.Y)));
                    break;
                case ModalType.OK_NO:
                    AddComponent(new Button("OK", "Ok", new Rectangle(sceneRectangle.Left + btnInterval, sceneRectangle.Bottom - btnSize.Y - btnInterval, btnSize.X, btnSize.Y)));
                    AddComponent(new Button("NO", "No", new Rectangle(sceneRectangle.Right -btnSize.X - btnInterval, sceneRectangle.Bottom -btnSize.Y-btnInterval, btnSize.X, btnSize.Y)));
                    break;
                case ModalType.OK_CANCEL:
                    AddComponent(new Button("OK", "Ok", new Rectangle(sceneRectangle.Left + btnInterval, sceneRectangle.Bottom - btnSize.Y - btnInterval, btnSize.X, btnSize.Y)));
                    AddComponent(new Button("CANCEL", "CANCEL", new Rectangle(sceneRectangle.Right - btnSize.X - btnInterval, sceneRectangle.Bottom - btnSize.Y - btnInterval, btnSize.X, btnSize.Y)));
                    break;
            }   
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(DrawHelper.GetTexture(), sceneRectangle, Color.FromNonPremultiplied(150, 170, 200, WTFHelper.alpha));
            DrawHelper.DrawRectagle(spriteBatch, Color.FromNonPremultiplied(200, 220, 255, WTFHelper.alpha), sceneRectangle, 9);

            //draw gui elements
            base.Draw(spriteBatch);
        }

        public override void Touch(Point touch, ButtonState touchState, bool isPressedMove)
        {
            base.Touch(touch, touchState, isPressedMove);

            if (modalType == ModalType.EMPTY)
            {
                App.GoToPrevScene();
            }
            //App.GoToScene(App.prev, true);
        }
        public override void GUIStateChanged(GuiObject sender)
        {
            if (sender.state == Button.State.Released)
                switch (sender.Name)
                {
                    case "OK":
                        modalResult = ModalResult.OK;
                        App.GoToPrevScene();
                        break;
                    case "NO":
                        modalResult = ModalResult.NO;
                        App.GoToPrevScene();
                        break;
                    case "CANCEL":
                        modalResult = ModalResult.CANCEL;
                        App.GoToPrevScene();
                        break;
                }
            base.GUIStateChanged(sender);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            int stepTimeInMs = 3000;
            double stepProgress = 1.0 / stepTimeInMs * (gameTime.TotalGameTime.TotalMilliseconds % stepTimeInMs);
        }
    }
}
