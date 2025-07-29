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
    public class MainMenu : Scene
    {
       // public Color menuBackColor = Color.LightSteelBlue;
        float totalMS=0f;

        public MainMenu(Rectangle sceneRectangle) :base(WTFHelper.SCENES.MAIN_MENU, sceneRectangle)
        {
            int btnInterval = 13;
            int btnHeight = 120;
            int btnWidth = 613;

            AddComponent(new Label("LABEL1", "TEST", new Rectangle(sceneRectangle.Left, sceneRectangle.Top, 200, 100), DrawHelper.spriteFont, Color.Red, AlignXY.RIGHT_BOTTOM));

            //left
            AddComponent( new Button("TEST1", "TEST LAB", new Rectangle((int)App.screenBounds.Left + btnInterval, (int)App.screenBounds.Top + btnInterval * 1 + btnHeight * 0, btnWidth, btnHeight)));
            AddComponent(new Button("TEST2", "TEST2", new Rectangle((int)App.screenBounds.Left + btnInterval, (int)App.screenBounds.Top + btnInterval * 2 + btnHeight * 1, btnWidth, btnHeight)));
            AddComponent(new Button("TEST3", "TEST3", new Rectangle((int)App.screenBounds.Left + btnInterval, (int)App.screenBounds.Top + btnInterval * 3 + btnHeight * 2, btnWidth, btnHeight)));
            AddComponent(new Button("TEST4", "TEST4", new Rectangle((int)App.screenBounds.Left + btnInterval, (int)App.screenBounds.Top + btnInterval * 4 + btnHeight * 3, btnWidth, btnHeight)));
            AddComponent(new Button("TEST5", "TEST5", new Rectangle((int)App.screenBounds.Left + btnInterval, (int)App.screenBounds.Top + btnInterval * 5 + btnHeight * 4, btnWidth, btnHeight)));
            AddComponent(new Button("MODAL EMPTY", "MODAL EMPTY", new Rectangle((int)App.screenBounds.Left + btnInterval, (int)App.screenBounds.Top + btnInterval * 6 + btnHeight * 5, btnWidth, btnHeight)));
            AddComponent(new Button("MODAL OK", "MODAL OK", new Rectangle((int)App.screenBounds.Left + btnInterval, (int)App.screenBounds.Top + btnInterval * 7 + btnHeight * 6, btnWidth, btnHeight)));
            AddComponent(new Button("MODAL OK/NO", "MODA OK/NO", new Rectangle((int)App.screenBounds.Left + btnInterval, (int)App.screenBounds.Top + btnInterval * 8 + btnHeight * 7, btnWidth, btnHeight)));

            //center
            AddComponent(new Button("TEST CENTER", "TEST CENTER", new Rectangle((int)App.screenCenter.X - btnWidth / 2, (int)App.screenBounds.Top + btnInterval * 5 + btnHeight * 4, btnWidth, btnHeight)));

            //right
            
            /*AddButton("APP1", "CONVEYOR",           new Rectangle((int)App.screenBounds.Right - btnInterval - btnWidth, (int)App.screenBounds.Top + btnInterval*1 + btnHeight * 0, btnWidth, btnHeight),
               DrawHelper.GetTexture(),DrawHelper.GetTexture());*/
            AddComponent(new Button("START", "SELECT LVL", new Rectangle((int)App.screenBounds.Right - btnInterval - btnWidth, (int)App.screenBounds.Top + btnInterval * 2 + btnHeight * 1, btnWidth, btnHeight)));
            /*AddButton("LEVEL_EDITOR", "EDIT LVL",   new Rectangle((int)App.screenBounds.Right - btnInterval - btnWidth, (int)App.screenBounds.Top + btnInterval*3 + btnHeight * 2, btnWidth, btnHeight),
               DrawHelper.GetTexture(),DrawHelper.GetTexture());*/

        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            //TODO HERE
            totalMS += 16.666f;
            (guiObjects["LABEL1"] as Label).Text = ((int)totalMS/1000).ToString();
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            //TODO HERE
          //  DrawHelper.DrawRectagle(spriteBatch, Color.Gray, 10, 10, 50, 20, 2);
        }
        public override void GUIStateChanged(GuiObject sender)
        {
            base.GUIStateChanged(sender);
            //TODO HERE

            //Нажата и отпущена кнопка меню
            if (sender.state == Button.State.Released)
            {
                switch (sender.Name)
                {
                    case "START":

                        App.GoToScene(WTFHelper.SCENES.LEVEL_SELECTOR);
                        break;
                    case "LEVEL_EDITOR":
                        App.GoToScene(WTFHelper.SCENES.LEVEL_EDITOR);
                        break;
                    case "TEST1":
                        App.GoToScene(WTFHelper.SCENES.TEST1);
                        break;
                    case "TEST2":
                        App.GoToScene(WTFHelper.SCENES.TEST2);
                        break;
                    case "TEST3":
                        App.GoToScene(WTFHelper.SCENES.TEST3);
                        break;
                    case "TEST4":
                        App.GoToScene(WTFHelper.SCENES.TEST4);
                        break;
                    case "TEST5":
                        App.GoToScene(WTFHelper.SCENES.TEST5);
                        break;
                    case "MODAL EMPTY":
                        App.GoToScene(WTFHelper.SCENES.MODAL_TEXT,
                            Engine.Scene.Scenes.Modal.ModalType.EMPTY,
                            sceneRect: new Rectangle(App.screenBounds.Center.X - 500, App.screenBounds.Center.Y - 300, 1000, 600),
                            param: "The first row in the lbl.\nThe second row in the lbl.");
                        break;
                    case "MODAL OK":
                        App.GoToScene(WTFHelper.SCENES.MODAL_TEXT,
                            Engine.Scene.Scenes.Modal.ModalType.OK,
                            sceneRect: new Rectangle(App.screenBounds.Center.X - 500, App.screenBounds.Center.Y - 300, 1000, 600),
                            param: "OK TEST");
                        break;
                    case "MODAL OK/NO":
                        App.GoToScene(WTFHelper.SCENES.MODAL_TEXT,
                            Engine.Scene.Scenes.Modal.ModalType.OK_NO,
                            sceneRect: new Rectangle(App.screenBounds.Center.X - 500, App.screenBounds.Center.Y - 300, 1000, 600),
                            param: "OK/NO TEST");
                        break;
                }
            }
        }
    }
}
