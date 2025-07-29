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

namespace WtfApp.App3
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

            GUIContainer guiCon = new GUIContainer("CON1",this,
                new Rectangle(50, 50, 1820, 800),
                Color.FromNonPremultiplied(50,50,50,255),
                GUIContainer.GUIDirection.HORIZONTAL,
                GUIContainer.GUITypePosition.AUTO_POS_AUTO_SIZE,
                30);
            guiCon.preferColCount = 10;
            guiCon.contentVerticalAlign = GUIContainer.ContentVAlign.TOP;
            //guiCon.contentVerticalAlign = GUIContainer.ContentAlign.AUTOINTERVAL;
            guiCon.contentMultiLine = GUIContainer.ContentMultiLine.POSSIBLE;
            AddComponent(guiCon);

            for (int i = 0,indexLvl=1; indexLvl < maxLevelNum; indexLvl++, i++)
            {
                var b = new Button("LEVEL." + indexLvl.ToString(), indexLvl.ToString(),
                    new Rectangle(App.screenBounds.Center.X - (btnSize + btnInterval) * btnCountInRow / 2 + btnInterval / 2 + i % btnCountInRow * (btnSize + btnInterval),
                    App.screenBounds.Top + btnInterval + (int)Math.Ceiling(i / btnCountInRow * 1.0) * (btnSize + btnInterval), btnSize, btnSize),
                    DrawHelper.GetTexture(), DrawHelper.GetTexture());
                AddComponent(b);
                guiCon.AddGuiObj(b);

                /*Button btn=new Button("EDIT."+indexLvl.ToString(), "E",
                    new Rectangle(App.screenBounds.Center.X - (btnSize + btnInterval) * btnCountInRow / 2 + btnInterval / 2 + i % btnCountInRow * (btnSize + btnInterval)+btnSize-editBtnSize,
                    App.screenBounds.Top + btnInterval + (int)Math.Ceiling(i / btnCountInRow * 1.0) * (btnSize + btnInterval), editBtnSize, editBtnSize),
                   DrawHelper.GetTexture(),DrawHelper.GetTexture());

                btn.borderStyle = BorderStyle.SOLID;
                btn.borderColor = Color.FromNonPremultiplied(150,150,150,WTFHelper.alpha);

                AddComponent(btn);*/
            }
            Button btn = new Button("NEW", "NEW",
                new Rectangle(App.screenBounds.Center.X - (btnSize + btnInterval) * btnCountInRow / 2 + btnInterval / 2 + (maxLevelNum - 1) % btnCountInRow * (btnSize + btnInterval),
                App.screenBounds.Top + btnInterval + (int)Math.Ceiling((maxLevelNum - 1) / btnCountInRow * 1.0) * (btnSize + btnInterval), btnSize, btnSize),
               DrawHelper.GetTexture(), DrawHelper.GetTexture());
            AddComponent(btn);

            guiCon.AddGuiObj(btn);
            AddComponent(new Button("BACK", "BACK", new Rectangle(App.screenBounds.Right-320, App.screenBounds.Bottom-170, 300, 150)));
        }

        public override void GUIStateChanged(GuiObject sender)
        {
            base.GUIStateChanged(sender);
            if (sender.objectType == GuiObjectType.BUTTON)
            {
                if (sender.state == Button.State.Released)
                {
                    if (sender.Name.StartsWith("LEVEL"))
                    {
                        SaveLoadLevel sll = SaveLoadLevel.LoadLevel(int.Parse(sender.Name.Split('.')[1].ToString()));
                        App.GoToScene(WTFHelper.SCENES.APP3, param: sll);
                    }
                    else if (sender.Name.StartsWith("EDIT"))
                    {
                        SaveLoadLevel sll = SaveLoadLevel.LoadLevel(int.Parse(sender.Name.Split('.')[1].ToString()));
                        App.GoToScene(WTFHelper.SCENES.APP3_LEVEL_EDITOR, param: sll);
                    }
                    else if (sender.Name == "NEW")
                    {
                        App.GoToScene(WTFHelper.SCENES.APP3_LEVEL_EDITOR);
                    }
                    else if (sender.Name == "BACK")
                    {
                        App.GoToScene(WTFHelper.SCENES.MAIN_MENU);
                    }
                }
                else if( sender.state == Button.State.PressedHold)
                {
                    if (sender.Name.StartsWith("LEVEL"))
                    {
                        SaveLoadLevel sll = SaveLoadLevel.LoadLevel(int.Parse(sender.Name.Split('.')[1].ToString()));
                        App.GoToScene(WTFHelper.SCENES.APP3_LEVEL_EDITOR, param: sll);
                        /*SaveLoadLevel sll = SaveLoadLevel.LoadLevel(int.Parse(sender.Name.Split('.')[1].ToString()));
                        App.GoToScene(WTFHelper.SCENES.APP3, param: sll);*/
                    }
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
