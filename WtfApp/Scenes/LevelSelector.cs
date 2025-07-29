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
    public class LevelSelector : Scene
    {
        public  LevelSelector( Rectangle sceneRectangle) : base(WTFHelper.SCENES.LEVEL_SELECTOR, sceneRectangle)
        {
            int maxLevelNum=SaveLoadLevel.GetMaxSavedLvl();
            int btnCountInRow = 8;
            int btnInterval = 20;
            int btnSize = 210;
            int editBtnSize = 70;

            for (int i = 0,indexLvl=1; indexLvl < maxLevelNum; indexLvl++, i++)
            {
                AddButton("LEVEL."+indexLvl.ToString(), indexLvl.ToString(),
                    new Rectangle(Main.screenBounds.Center.X - (btnSize + btnInterval) * btnCountInRow / 2 + btnInterval / 2 + i % btnCountInRow * (btnSize + btnInterval),
                    Main.screenBounds.Top + btnInterval + (int)Math.Ceiling(i / btnCountInRow * 1.0) * (btnSize + btnInterval), btnSize, btnSize),
                    DrawHelper.emptyTexture, DrawHelper.emptyTexture);

                AddButton("EDIT."+indexLvl.ToString(), "E",
                    new Rectangle(Main.screenBounds.Center.X - (btnSize + btnInterval) * btnCountInRow / 2 + btnInterval / 2 + i % btnCountInRow * (btnSize + btnInterval)+btnSize-editBtnSize,
                    Main.screenBounds.Top + btnInterval + (int)Math.Ceiling(i / btnCountInRow * 1.0) * (btnSize + btnInterval), editBtnSize, editBtnSize),
                    DrawHelper.emptyTexture, DrawHelper.emptyTexture);

                Button button = sceneButtons.GetButton("EDIT." + indexLvl.ToString());
                if (button != null)
                {
                    button.borderStyle = BorderStyle.SOLID;
                    button.borderColor = Color.FromNonPremultiplied(150,150,150,WTFHelper.alpha);
                }
            }
            AddButton("NEW", "NEW",
                new Rectangle(Main.screenBounds.Center.X - (btnSize + btnInterval) * btnCountInRow / 2 + btnInterval / 2 + (maxLevelNum - 1) % btnCountInRow * (btnSize + btnInterval),
                Main.screenBounds.Top + btnInterval + (int)Math.Ceiling((maxLevelNum - 1) / btnCountInRow * 1.0) * (btnSize + btnInterval), btnSize, btnSize),
                DrawHelper.emptyTexture, DrawHelper.emptyTexture);

            AddButton("BACK", "BACK", new Rectangle(Main.screenBounds.Right-320, Main.screenBounds.Bottom-220, 300, 180));

            sceneButtons.ToString();
        }

        public override void ButtonStateChanged(Button sender)
        {
            base.ButtonStateChanged(sender);
            if (sender.state == Button.State.Released)
            {

                if (sender.Name.StartsWith("LEVEL"))
                {
                    SaveLoadLevel sll = SaveLoadLevel.LoadLevel(int.Parse(sender.Name.Split('.')[1].ToString()));
                    Main.GoToScene(WTFHelper.SCENES.GAME, sll);
                }
                else if (sender.Name.StartsWith("EDIT"))
                {
                    SaveLoadLevel sll = SaveLoadLevel.LoadLevel(int.Parse(sender.Name.Split('.')[1].ToString()));
                    Main.GoToScene(WTFHelper.SCENES.LEVEL_EDITOR, sll);
                }
                else if (sender.Name == "NEW")
                {
                    Main.GoToScene(WTFHelper.SCENES.LEVEL_EDITOR);
                }
                else if (sender.Name == "BACK")
                {
                    Main.GoToScene(WTFHelper.SCENES.MAIN_MENU);
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }

        public override void Touch(Point touch, ButtonState touchState, bool isPressedMove)
        {
            base.Touch(touch, touchState, isPressedMove);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
}
