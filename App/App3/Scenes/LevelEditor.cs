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
    public class LevelEditor : Scene
    {
        public Color menuBackColor = Color.DimGray;
        private int lvlId = 0;
        private int worldSizeX=8;
        private int worldSizeY=8;

        private WorldObj[,] worldObjects;
        private List<WTFHelper.WORLD_OBJ_TYPE> availableUserWorldObj;
        private WTFHelper.WORLD_OBJ_TYPE _selectedWorldObj;

        private double menuPartWidth = 0.2;
        private double worldPartWidth = 0.8;
        //смещение объектов отрисовки исходя из зума/перетаскивания
        private Vector2 offsetDraw = Vector2.Zero;
        private Rectangle menuBtnRect;
        private Rectangle worldMenuBtnRect;
        private Rectangle gameAreaRect;
        private Rectangle worldObjectsRect;
       
        public LevelEditor(Rectangle sceneRectangle, SaveLoadLevel sll):this(sceneRectangle)
        {
            if (sll != null)
            {
                this.lvlId = sll.levelId;
                this.worldSizeX = sll.levelSizeX;
                this.worldSizeY = sll.levelSizeY;
                this.availableUserWorldObj = sll.availableUserWorldObj.ToList();
                this.worldObjects = sll.GetNormalizeWorldArray();

                ResizeWorldArray(0, 0);
            }
        }

        public LevelEditor(Rectangle sceneRectangle) : base(WTFHelper.SCENES.MAIN_MENU, sceneRectangle)
        {
            worldObjects = new WorldObj[worldSizeY,worldSizeX];
            availableUserWorldObj = new List<WTFHelper.WORLD_OBJ_TYPE>();
            int worldBtnItemSize = 96;
            int countWorldItemsInCol = 6;
            int buttonsHeight = 72;
            int buttonsInterval = 10;

            gameAreaRect = new Rectangle(App.screenBounds.Left,
                                         App.screenBounds.Top,
                                         (int)(App.screenBounds.Width * worldPartWidth),
                                         (int)(App.screenBounds.Height));

            worldObjectsRect = new Rectangle(gameAreaRect.Center.X - worldSizeX * App3.cellSize / 2,
                                             gameAreaRect.Center.Y - worldSizeY * App3.cellSize / 2,
                                             App3.cellSize * worldSizeX, App3.cellSize * worldSizeY);

            worldMenuBtnRect = new Rectangle(App.screenBounds.Width - (int)(App.screenBounds.Width * menuPartWidth),
                                        App.screenBounds.Top,
                                        (int)(App.screenBounds.Width * menuPartWidth),
                                        (int)(App.screenBounds.Height * 0.55));

            menuBtnRect = new Rectangle(App.screenBounds.Width - (int)(App.screenBounds.Width* menuPartWidth),
                                        App.screenBounds.Height - (int)(App.screenBounds.Height*0.45),
                                        (int)(App.screenBounds.Width * menuPartWidth),
                                        (int)(App.screenBounds.Height * 0.45));

            offsetDraw.X = worldObjectsRect.Left - gameAreaRect.Left;
            offsetDraw.Y = worldObjectsRect.Top - gameAreaRect.Top;

            GUIContainer guiContainer = new GUIContainer("CON1", this, worldMenuBtnRect, Color.Black, GUIContainer.GUIDirection.VERTICAL, GUIContainer.GUITypePosition.AUTO_POS_AUTO_SIZE, 20);
            guiContainer.contentHorizontalAlign = GUIContainer.ContentHAlign.LEFT;
            AddComponent(guiContainer);
            for (int i=0;i<WTFHelper.worldObjTextures.Count;i++)
            {
                Button btn = new Button(WTFHelper.worldObjTextures.ElementAt(i).Key,"", Rectangle.Empty, WTFHelper.worldObjTextures.ElementAt(i).Value, WTFHelper.worldObjTextures.ElementAt(i).Value);
                guiContainer.AddGuiObj(btn);
                /*if (availableUserWorldObj.Count(t => t.ToString() == WTFHelper.worldObjTextures.ElementAt(i).Key) > 0)
                    btn.borderStyle = BorderStyle.SOLID;

                btn.ChangeDefaultColor(Color.FromNonPremultiplied(255, 255, 255, WTFHelper.alpha));*/
                AddComponent(btn);
            }
            AddComponent(new Button("SAVE", "SAVE", new Rectangle(menuBtnRect.X, menuBtnRect.Y, menuBtnRect.Width, buttonsHeight)));
            AddComponent(new Button("TEST", "TEST", new Rectangle(menuBtnRect.X, menuBtnRect.Y+ (buttonsHeight+buttonsInterval), menuBtnRect.Width, buttonsHeight)));
            AddComponent(new Button("BACK", "BACK", new Rectangle(menuBtnRect.X, menuBtnRect.Y+ (buttonsHeight + buttonsInterval)*2, menuBtnRect.Width, buttonsHeight)));

            AddComponent(new Button("W-", "W-", new Rectangle(menuBtnRect.Left, menuBtnRect.Bottom- (buttonsHeight + buttonsInterval) * 2, menuBtnRect.Width/2- buttonsInterval/2, buttonsHeight)));
            AddComponent(new Button("W+", "W+", new Rectangle(menuBtnRect.Right - menuBtnRect.Width/2+ buttonsInterval/2, menuBtnRect.Bottom - (buttonsHeight + buttonsInterval)*2, menuBtnRect.Width/2- buttonsInterval/2, buttonsHeight)));
            AddComponent(new Button("H-", "H-", new Rectangle(menuBtnRect.Left, menuBtnRect.Bottom - buttonsHeight-buttonsInterval, menuBtnRect.Width/2- buttonsInterval/2, buttonsHeight)));
            AddComponent(new Button("H+", "H+", new Rectangle(menuBtnRect.Right- menuBtnRect.Width/2+ buttonsInterval/2, menuBtnRect.Bottom - buttonsHeight - buttonsInterval, menuBtnRect.Width/2- buttonsInterval/2, buttonsHeight)));
        }

        public bool ResizeWorldArray(int offsetW, int offsetH)
        {
            if (worldSizeX + offsetW > 1 && worldSizeY + offsetH > 1
                && worldSizeX + offsetW < 30 && worldSizeY + offsetH < 20)
            {

                WorldObj[,] tmpWorldObjects = new WorldObj[worldSizeY + offsetH, worldSizeX + offsetW];

                worldSizeX += offsetW;
                worldSizeY += offsetH;

                worldObjectsRect = new Rectangle(gameAreaRect.Center.X - worldSizeX * App3.cellSize / 2,
                                                 gameAreaRect.Center.Y - worldSizeY * App3.cellSize / 2,
                                                 App3.cellSize * worldSizeX, App3.cellSize * worldSizeY);


                offsetDraw.X = worldObjectsRect.Left - gameAreaRect.Left;
                offsetDraw.Y = worldObjectsRect.Top - gameAreaRect.Top;

                for (int y = 0; y < worldSizeY; y++)
                {
                    for (int x = 0; x < worldSizeX; x++)
                    {
                        tmpWorldObjects[y, x] = (y >= worldObjects.GetLength(0) || x >= worldObjects.GetLength(1)) ? null : worldObjects[y, x]; //(offsetW + offsetH) > 0 ? null: worldObjects[y, x];
                        tmpWorldObjects[y, x]?.ChangeOffsetDraw(offsetDraw);
                    }
                }
                worldObjects = tmpWorldObjects;

                return true;
            }
            else
                return false;
        }

        public override void Touch(Point touch, ButtonState touchState, bool isPressedMove)
        {
            base.Touch(touch, touchState, isPressedMove);
            //TODO HERE
            if( worldObjectsRect.Contains(touch))
            {
                double doubleTouchedWorldX = (touch.X - offsetDraw.X) / App3.cellSize;
                double doubleTouchedWorldY = (touch.Y - offsetDraw.Y) / App3.cellSize;
                int touchedWorldX = (int)Math.Floor(doubleTouchedWorldX);
                int touchedWorldY = (int)Math.Floor(doubleTouchedWorldY);

                if (_selectedWorldObj != WTFHelper.WORLD_OBJ_TYPE.EMPTY)
                {
                    if (worldObjects[touchedWorldY, touchedWorldX] == null)
                    {
                        worldObjects[touchedWorldY, touchedWorldX] = new WorldObj(_selectedWorldObj, touchedWorldX, touchedWorldY);
                        worldObjects[touchedWorldY, touchedWorldX].ChangeOffsetDraw(offsetDraw);
                    }
                    //поворот существуещего объекта в направлении движения "касания"
                    else if (isPressedMove)
                    {
                        if (Math.Abs(doubleTouchedWorldX - touchedWorldX - 0.5) > Math.Abs(doubleTouchedWorldY - touchedWorldY - 0.5))
                        {
                            if (doubleTouchedWorldX > touchedWorldX + 0.5)
                                worldObjects[touchedWorldY, touchedWorldX].objDrawDir = WTFHelper.OBJ_DRAW_DIRECTION.RIGHT;
                            else
                                worldObjects[touchedWorldY, touchedWorldX].objDrawDir = WTFHelper.OBJ_DRAW_DIRECTION.LEFT;
                        }
                        else
                        {
                            if (doubleTouchedWorldY > touchedWorldY + 0.5)
                                worldObjects[touchedWorldY, touchedWorldX].objDrawDir = WTFHelper.OBJ_DRAW_DIRECTION.BOTTOM;
                            else
                                worldObjects[touchedWorldY, touchedWorldX].objDrawDir = WTFHelper.OBJ_DRAW_DIRECTION.TOP;
                        }
                    }
                }
                else if (worldObjects[touchedWorldY, touchedWorldX] != null && _selectedWorldObj == WTFHelper.WORLD_OBJ_TYPE.EMPTY)
                {
                    worldObjects[touchedWorldY, touchedWorldX] = null;
                }            
            }
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
            for (int y = 0; y < worldSizeY; y++)
            {
                for (int x = 0; x < worldSizeX; x++)
                {
                    if(worldObjects[y,x]!=null)
                    {
                        worldObjects[y, x].Draw(spriteBatch,WTFHelper.DRAW_LAYER.WORLD_OBJ.F());
                    }
                    DrawHelper.DrawRectagle(spriteBatch, Color.FromNonPremultiplied(50, 50, 50, WTFHelper.alpha),
                        (int)offsetDraw.X + x * App3.cellSize,
                        (int)offsetDraw.Y + y * App3.cellSize,
                        App3.cellSize, App3.cellSize, 5);
                }
            }
        }
        public override void GUIStateChanged(GuiObject sender)
        {
            base.GUIStateChanged(sender);
            //TODO HERE
            if (sender.objectType == GuiObjectType.BUTTON)
            {
                WTFHelper.WORLD_OBJ_TYPE clickedWordObjMenuItem;
                if (Enum.TryParse(sender.Name, out clickedWordObjMenuItem))
                {
                    if (sender.state == Button.State.Released)
                    {
                        _selectedWorldObj = clickedWordObjMenuItem;
                    }
                    else if (sender.state == Button.State.PressedHold)
                    {
                        if (sender.borderStyle == BorderStyle.NONE)
                            sender.borderStyle = BorderStyle.SOLID;
                        else
                            sender.borderStyle = BorderStyle.NONE;

                        int index = availableUserWorldObj.IndexOf(clickedWordObjMenuItem);
                        if (sender.borderStyle != BorderStyle.NONE && index == -1)
                        {
                            availableUserWorldObj.Add(clickedWordObjMenuItem);
                        }
                        else if (sender.borderStyle == BorderStyle.NONE && index != -1)
                        {
                            availableUserWorldObj.RemoveAt(index);
                        }

                        /*if (sender.text == "")
                            sender.text = "+";
                        else
                            sender.text = "";*/
                    }
                }
                else if (sender.state == Button.State.Released)
                {
                    switch (sender.Name)
                    {
                        case "W+": ResizeWorldArray(1, 0); break;
                        case "W-": ResizeWorldArray(-1, 0); break;
                        case "H+": ResizeWorldArray(0, 1); break;
                        case "H-": ResizeWorldArray(0, -1); break;
                        case "SAVE":
                            SaveLoadLevel sll = new SaveLoadLevel();
                            sll.SaveLevel(lvlId == 0 ? SaveLoadLevel.GetMaxSavedLvl() : lvlId, worldSizeX, worldSizeY, worldObjects, availableUserWorldObj);
                            // App.GoToScene(WTFHelper.SCENES.GAME);
                            break;
                        case "BACK":
                            App.GoToScene(WTFHelper.SCENES.APP3_LEVEL_SELECTOR);
                            break;

                            /*     case "LEVEL_EDITOR":
                                     if (sender.state == Button.State.Released)
                                     {
                                         //App.GoToScene(WTFHelper.SCENES.LEVEL_EDITOR);
                                     }
                                     break;*/
                    }
                }
            }
        }
    }
}
