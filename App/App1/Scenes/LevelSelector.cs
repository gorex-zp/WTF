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

namespace WtfApp.App1
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
                AddComponent(new Button("LEVEL."+indexLvl.ToString(), indexLvl.ToString(),
                    new Rectangle(App.screenBounds.Center.X - (btnSize + btnInterval) * btnCountInRow / 2 + btnInterval / 2 + i % btnCountInRow * (btnSize + btnInterval),
                    App.screenBounds.Top + btnInterval + (int)Math.Ceiling(i / btnCountInRow * 1.0) * (btnSize + btnInterval), btnSize, btnSize),
                   DrawHelper.GetTexture(),DrawHelper.GetTexture()));

                Button btn=new Button("EDIT."+indexLvl.ToString(), "E",
                    new Rectangle(App.screenBounds.Center.X - (btnSize + btnInterval) * btnCountInRow / 2 + btnInterval / 2 + i % btnCountInRow * (btnSize + btnInterval)+btnSize-editBtnSize,
                    App.screenBounds.Top + btnInterval + (int)Math.Ceiling(i / btnCountInRow * 1.0) * (btnSize + btnInterval), editBtnSize, editBtnSize),
                   DrawHelper.GetTexture(),DrawHelper.GetTexture());
                btn.ChangeDefaultColor(Color.LightSlateGray);

                btn.borderStyle = BorderStyle.SOLID;
                btn.borderColor = Color.FromNonPremultiplied(150,150,150,WTFHelper.alpha);

                AddComponent(btn);
            }
            AddComponent(new Button("NEW", "NEW",
                new Rectangle(App.screenBounds.Center.X - (btnSize + btnInterval) * btnCountInRow / 2 + btnInterval / 2 + (maxLevelNum - 1) % btnCountInRow * (btnSize + btnInterval),
                App.screenBounds.Top + btnInterval + (int)Math.Ceiling((maxLevelNum - 1) / btnCountInRow * 1.0) * (btnSize + btnInterval), btnSize, btnSize),
               DrawHelper.GetTexture(),DrawHelper.GetTexture()));

            AddComponent(new Button("BACK", "BACK", new Rectangle(App.screenBounds.Right-320, App.screenBounds.Bottom-170, 300, 150)));
        }

        public override void GUIStateChanged(GuiObject sender)
        {
            //base.GUIStateChanged(sender);
            if (sender.objectType==GuiObjectType.BUTTON && sender.state == Button.State.Released)
            {
                if (sender.Name.StartsWith("LEVEL"))
                {
                    SaveLoadLevel sll = SaveLoadLevel.LoadLevel(int.Parse(sender.Name.Split('.')[1].ToString()));
                    App.GoToScene(WTFHelper.SCENES.GAME,param: sll);
                }
                else if (sender.Name.StartsWith("EDIT"))
                {
                    SaveLoadLevel sll = SaveLoadLevel.LoadLevel(int.Parse(sender.Name.Split('.')[1].ToString()));
                    App.GoToScene(WTFHelper.SCENES.LEVEL_EDITOR,param: sll);
                }
                else if (sender.Name == "NEW")
                {
                    App.GoToScene(WTFHelper.SCENES.LEVEL_EDITOR);
                }
                else if (sender.Name == "BACK")
                {
                    App.GoToScene(WTFHelper.SCENES.MAIN_MENU);
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
