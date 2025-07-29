using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using WtfApp.GUI;

namespace WtfApp.Scenes
{
    public class MainMenu : Scene
    {
        public Color menuBackColor = Color.DimGray;       

        public MainMenu(Rectangle sceneRectangle) :base(WTFHelper.SCENES.MAIN_MENU, sceneRectangle)
        {
            AddButton("START", "START", new Rectangle((int)Main.screenCenter.X - 200, (int)Main.screenCenter.Y - 140, 400, 120),
                DrawHelper.emptyTexture, DrawHelper.emptyTexture);
            AddButton("LEVEL_EDITOR", "EDIT LVL", new Rectangle((int)Main.screenCenter.X - 200, (int)Main.screenCenter.Y + 20, 400, 120),
                DrawHelper.emptyTexture, DrawHelper.emptyTexture);
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            //TODO HERE
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            //TODO HERE
          //  DrawHelper.DrawRectagle(spriteBatch, Color.Gray, 10, 10, 50, 20, 2);
        }
        public override void ButtonStateChanged(Button sender)
        {
            base.ButtonStateChanged(sender);
            //TODO HERE
/*#if DEBUG
            if(sender.state==Button.State.PressedHold)
                System.Diagnostics.Debug.Write(this.sceneType.ToString()+":ButtonEvent["+sender.name+";"+sender.state.ToString()+";]\n");
#endif*/

            switch (sender.Name)
            {
                case "START":
                    if(sender.state==Button.State.Released)
                    {
                        Main.GoToScene(WTFHelper.SCENES.GAME);
                    }
                    break;
                case "LEVEL_EDITOR":
                    if (sender.state == Button.State.Released)
                    {
                        Main.GoToScene(WTFHelper.SCENES.LEVEL_EDITOR);
                    }
                    break;
            }
        }
    }
}
