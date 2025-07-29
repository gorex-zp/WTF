using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using WtfApp.GUI;
using WtfApp.Classes;
using WtfApp.Engine.Scene;

namespace WtfApp.App3
{
    public class App3 : Scene
    {
        public List<RectangleF> DEBUGRECTS;

        public static int worldSizeX = 0;
        public static int worldSizeY = 0;
        public static int cellSize = 130;
        public static double stepTimeInMs = 2000;
        public static double stepCountToNewBlock = 2;
        public double stepCurrentTime = 0.0;
        public double currentStepCountToNewBlock = 0;
        public double stepProgress = 0.0;
        public double curUpdateProgress = 0.0;
        public static bool isPaused = false;
        public static bool isWorldClean = true;
        //curGameWeldId mast be >0
        public static int curGameWeldId = 1;
        public static int score = 0;
        public WTFHelper.WORLD_OBJ_TYPE selectedWorldObj;
        public bool isStoped = false; //change to TRUE

        private double menuPartWidth = 0.2;
        private double worldPartWidth = 0.8;
        private Vector2 offsetDraw = Vector2.Zero;
        private Rectangle menuBtnRect;
        private Rectangle worldMenuBtnRect;
        private float worldMenuBtnScale = 1.2f;
        private Rectangle gameAreaRect;
        private Rectangle worldObjectsRect;
        private WTFHelper.MENU_BUTTON_ALIGN menuBtnAlign = WTFHelper.MENU_BUTTON_ALIGN.CENTER;

        //public static Rectangle worldFieldRect = new Rectangle(5, 5, 200, 200);
        //public static Rectangle menuFieldRect = new Rectangle(210, 5, 50, 200);
        //public static Rectangle advFieldRect = new Rectangle(210, 5, 50, 200);

        public WTFHelper.WORLD_OBJ_TYPE[] availableWorldObjects;
        public WorldObj[,] worldObjects { get; private set; }
        public GeneratedObj[,] generatedObjects { get; private set; }
        private GeneratedObj[,] _tmpGeneratedObjects { get; set; }

        public App3(Rectangle sceneRectangle,SaveLoadLevel sll) : base(WTFHelper.SCENES.APP3, sceneRectangle)
        {
            DEBUGRECTS = new List<RectangleF>();

            worldSizeX = sll.levelSizeX;
            worldSizeY = sll.levelSizeY;

            this.sceneRectangle = sceneRectangle;
            this.worldObjects = new WorldObj[worldSizeY, worldSizeX];
            this.generatedObjects = new GeneratedObj[worldSizeY, worldSizeX];
            this._tmpGeneratedObjects = new GeneratedObj[worldSizeY, worldSizeX];

            /*инф. о доступных для выбора элементов*/
            int worldItemSize = 96;
            //кол-во в столбце
            int countWorldItemsInCol = 5;

            /*инф о кнопках управления*/
            int buttonsHeight = 88;
            int buttonsInterval = 10;

            gameAreaRect = new Rectangle(App.screenBounds.Left,
                                         App.screenBounds.Top,
                                         (int)(App.screenBounds.Width * worldPartWidth),
                                         (int)(App.screenBounds.Height));

            worldObjectsRect = new Rectangle(gameAreaRect.Center.X - worldSizeX * App3.cellSize / 2,
                                             gameAreaRect.Center.Y - worldSizeY * App3.cellSize / 2,
                                             App3.cellSize * worldSizeX, App3.cellSize * worldSizeY);

            worldMenuBtnRect = new Rectangle(sceneRectangle.Width - (int)(sceneRectangle.Width * menuPartWidth),
                                        sceneRectangle.Top,
                                        (int)(sceneRectangle.Width * menuPartWidth),
                                        (int)(sceneRectangle.Height * 0.55));

            menuBtnRect = new Rectangle(sceneRectangle.Width - (int)(sceneRectangle.Width * menuPartWidth),
                                        sceneRectangle.Height - (int)(sceneRectangle.Height * 0.45),
                                        (int)(sceneRectangle.Width * menuPartWidth),
                                        (int)(sceneRectangle.Height * 0.45));

            offsetDraw.X = worldObjectsRect.Left - gameAreaRect.Left;
            offsetDraw.Y = worldObjectsRect.Top - gameAreaRect.Top;

            //загрузка объектов мира
            for (int i = 0; i < sll.worldObjects.Length; i++)
            {
             
                AddWorldObject(sll.worldObjects[i].x, sll.worldObjects[i].y, sll.worldObjects[i].type, sll.worldObjects[i].dir);
                worldObjects[sll.worldObjects[i].y, sll.worldObjects[i].x].ChangeOffsetDraw(offsetDraw);
            }

            for (int y = 0; y < worldSizeY; y++)
            {
                for (int x = 0; x < worldSizeX; x++)
                {
                    if (worldObjects[y, x] == null)
                    {
                        AddWorldObject(x, y, (WTFHelper.WORLD_OBJ_TYPE.EMPTY), WTFHelper.OBJ_DRAW_DIRECTION.TOP);
                        worldObjects[y, x].ChangeOffsetDraw(offsetDraw);
                    }
                }
            }
            //загрузка достпных пользователю объектов
            availableWorldObjects = sll.availableUserWorldObj;
            GUIContainer guiContainer = new GUIContainer("CON1", this, worldMenuBtnRect, Color.Black, GUIContainer.GUIDirection.VERTICAL, GUIContainer.GUITypePosition.AUTO_POS_AUTO_SIZE, 20);
            guiContainer.contentHorizontalAlign = GUIContainer.ContentHAlign.LEFT;
            //guiContainer.preferRowCount = 0;
            //guiContainer.contentVerticalAlign = GUIContainer.ContentVAlign.TOP;
            AddComponent(guiContainer);
            for (int i = 0; i < availableWorldObjects.Length; i++)
            {

                /* if (i == countWorldItemsInCol)
                 {
                    /*AddButton("DELETE", "",
                     new Rectangle(worldMenuBtnRect.Left,
                     worldMenuBtnRect.Top + countWorldItemsInCol * worldItemSize, worldItemSize, worldItemSize),
                     WTFHelper.worldObjTextures[(int)WTFHelper.worldObjTextures, WTFHelper.worldObjTextures[(int)availableWorldObjects[i]]);
                     sceneButtons[i].borderColor = Color.Yellow;*/

                Button btn = new Button(availableWorldObjects[i].ToString(), "", Rectangle.Empty,
                        WTFHelper.worldObjTextures[availableWorldObjects[i].ToString()], WTFHelper.worldObjTextures[availableWorldObjects[i].ToString()]);
                    btn.borderColor = Color.Yellow;
                    btn.ChangeDefaultColor( Color.FromNonPremultiplied(255,255,255,WTFHelper.alpha));

                guiContainer.AddGuiObj(btn);
                AddComponent(btn);

            }
            //AddButton("START_BTN", "START", new Rectangle(menuBtnRect.X, menuBtnRect.Y, menuBtnRect.Width, buttonsHeight));

            AddComponent(new Button("START_BTN", "STOP", new Rectangle(menuBtnRect.X, menuBtnRect.Y, menuBtnRect.Width, buttonsHeight)));

            AddComponent(new Button("PAUSE_BTN", "PAUSE", new Rectangle(menuBtnRect.X, menuBtnRect.Y + (buttonsHeight + buttonsInterval), menuBtnRect.Width, buttonsHeight)));
            AddComponent(new Button("CLEAR_BTN", "CLEAR", new Rectangle(menuBtnRect.X, menuBtnRect.Y + (buttonsHeight + buttonsInterval) * 2, menuBtnRect.Width, buttonsHeight)));
            AddComponent(new Button("BACK_BTN", "BACK", new Rectangle(menuBtnRect.X, menuBtnRect.Y + (buttonsHeight + buttonsInterval) * 3, menuBtnRect.Width, buttonsHeight)));

            AddComponent(new Button("S-", "S-", new Rectangle(menuBtnRect.Left, menuBtnRect.Bottom - (buttonsHeight + buttonsInterval) , (int)(menuBtnRect.Width *0.3), buttonsHeight)));
            AddComponent(new Button("S+", "S+", new Rectangle(menuBtnRect.Right - (int)(menuBtnRect.Width*0.3) , menuBtnRect.Bottom - (buttonsHeight + buttonsInterval) , (int)(menuBtnRect.Width *0.3), buttonsHeight)));
        }


        public bool CordInWorldAreaRange(int curX, int curY, WTFHelper.OBJ_MOVE_DIRECTION dir)
        {
            switch (dir)
            {
                case WTFHelper.OBJ_MOVE_DIRECTION.TOP:
                    if (curY - 1 >= 0)
                        return true;
                    break;
                case WTFHelper.OBJ_MOVE_DIRECTION.RIGHT:
                    if (curX + 1 <= worldSizeX - 1)
                        return true;
                    break;
                case WTFHelper.OBJ_MOVE_DIRECTION.BOTTOM:
                    if (curY + 1 <= worldSizeY - 1)
                        return true;
                    break;
                case WTFHelper.OBJ_MOVE_DIRECTION.LEFT:
                    if (curX - 1 >= 0)
                        return true;
                    break;
            }
            return false;
        }

        public GeneratedObj GetNextGeneratedObjOnDirection(int curX, int curY, WTFHelper.OBJ_MOVE_DIRECTION dir)
        {
            switch (dir)
            {
                case WTFHelper.OBJ_MOVE_DIRECTION.TOP:
                    if (curY - 1 >= 0)
                        return generatedObjects[curY - 1, curX];
                    break;
                case WTFHelper.OBJ_MOVE_DIRECTION.RIGHT:
                    if (curX + 1 <= worldSizeX - 1)
                        return generatedObjects[curY, curX + 1];
                    break;
                case WTFHelper.OBJ_MOVE_DIRECTION.BOTTOM:
                    if (curY + 1 <= worldSizeY - 1)
                        return generatedObjects[curY + 1, curX];
                    break;
                case WTFHelper.OBJ_MOVE_DIRECTION.LEFT:
                    if (curX - 1 >= 0)
                        return generatedObjects[curY, curX - 1];
                    break;
            }
            return null;
        }
        public WorldObj GetNextWorldObjOnDirection(int curX, int curY, WTFHelper.OBJ_DRAW_DIRECTION dir)
        {
            switch (dir)
            {
                case WTFHelper.OBJ_DRAW_DIRECTION.TOP:
                    if (curY - 1 >= 0)
                        return worldObjects[curY - 1, curX];
                    break;
                case WTFHelper.OBJ_DRAW_DIRECTION.RIGHT:
                    if (curX + 1 <= worldSizeX - 1)
                        return worldObjects[curY, curX + 1];
                    break;
                case WTFHelper.OBJ_DRAW_DIRECTION.BOTTOM:
                    if (curY + 1 <= worldSizeY - 1)
                        return worldObjects[curY + 1, curX];
                    break;
                case WTFHelper.OBJ_DRAW_DIRECTION.LEFT:
                    if (curX - 1 >= 0)
                        return worldObjects[curY, curX - 1];
                    break;
            }
            return null;
        }
        public void AddWorldObject(int x, int y, WTFHelper.WORLD_OBJ_TYPE type, WTFHelper.OBJ_DRAW_DIRECTION dir)
        {
            if (x >= worldSizeX || y >= worldSizeY || x < 0 || y < 0)
                throw new Exception("X/Y OUT OF WORLD SIZE RANGE");

            worldObjects[y, x] = new WorldObj(
                type,
                x,
                y,
                dir);
        }


        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            //TODO HERE
            if (state == SceneState.Paused)
            {
                stepCurrentTime += gameTime.ElapsedGameTime.TotalMilliseconds;
                stepProgress = 1 / stepTimeInMs * stepCurrentTime;
                System.Diagnostics.Debug.WriteLine(stepProgress);

                //инкрементируем текущий счетчик шага

                if (stepCurrentTime >= stepTimeInMs)
                {
                    stepCurrentTime = 0.0;
                    stepProgress = 0.0;
                    currentStepCountToNewBlock++;
                    //обнуляем счетчик текущих шагов
                    if (currentStepCountToNewBlock == stepCountToNewBlock)
                        currentStepCountToNewBlock = 0;

                    //вызываем NewStep для каждого объекта где убираем инф. с пред. шага
                    for (int y = 0; y < worldSizeY; y++)
                    {
                        for (int x = 0; x < worldSizeX; x++)
                        {
                            worldObjects[y, x]?.NewStep();
                            generatedObjects[y, x]?.NewStep();
                            /*//оставляем IsRotated если он есть и блок на ходится на повортном блоке
                            if(generatedObjects[y,x]?.isRotated==true &&
                                (worldObjects[y,x]?.objType==WTFHelper.WORLD_OBJ_TYPE.ROTATE || worldObjects[y, x]?.objType == WTFHelper.WORLD_OBJ_TYPE.ROTATEBACK))
                                generatedObjects[y, x].isRotated = true;
                            else
                                generatedObjects[y, x]?.isRotated ??= false;*/
                        }
                    }
                }

                for (int y = 0; y < worldSizeY; y++)
                {
                    for (int x = 0; x < worldSizeX; x++)
                    {
                        worldObjects[y, x]?.Update(stepProgress);
                        generatedObjects[y, x]?.Update(stepProgress);
                    }
                }
            }
        }

       
        public override void Touch(Point touch, ButtonState touchState, bool isPressedMove)
        {
            base.Touch(touch, touchState, isPressedMove);

            if (worldObjectsRect.Contains(touch))
            {
                double doubleTouchedWorldX = (touch.X - offsetDraw.X) / App3.cellSize;
                double doubleTouchedWorldY = (touch.Y - offsetDraw.Y) / App3.cellSize;
                int touchedWorldX = (int)Math.Floor(doubleTouchedWorldX);
                int touchedWorldY = (int)Math.Floor(doubleTouchedWorldY);

                
                if (selectedWorldObj != WTFHelper.WORLD_OBJ_TYPE.EMPTY && isStoped==true)
                {
                    if (worldObjects[touchedWorldY, touchedWorldX]?.objType== WTFHelper.WORLD_OBJ_TYPE.EMPTY)
                    {
                        worldObjects[touchedWorldY, touchedWorldX] = new WorldObj(selectedWorldObj, touchedWorldX, touchedWorldY,null,true);
                        worldObjects[touchedWorldY, touchedWorldX].ChangeOffsetDraw(offsetDraw);
                    }
                    //поворот существуещего объекта в направлении движения "касания"
                    else if (isPressedMove && worldObjects[touchedWorldY, touchedWorldX].isUser==true)
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

                    else if (worldObjects[touchedWorldY, touchedWorldX].isUser==true && selectedWorldObj == WTFHelper.WORLD_OBJ_TYPE.EMPTY)
                    {
                        AddWorldObject(touchedWorldX, touchedWorldY, (WTFHelper.WORLD_OBJ_TYPE.EMPTY), WTFHelper.OBJ_DRAW_DIRECTION.TOP);
                    }
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            //TODO HERE
            /* for (int y = 0; y < worldSizeY; y++)
             {
                 for (int x = 0; x < worldSizeX; x++)
                 {
                     DrawHelper.DrawRectagle(spriteBatch,
                         Color.FromNonPremultiplied(20, 20, 20, 255),
                         cellSize * x,
                         cellSize * y,
                         cellSize, cellSize, 3);
                     DrawHelper.DrawRectagle(spriteBatch, 
                         Color.FromNonPremultiplied(40, 40, 40, 255),
                         cellSize * x + 1,
                         cellSize * y + 1,
                         cellSize - 2, cellSize - 2, 1);
                 }
             }*/
            //Draw world && generated

            for (int y = 0; y < worldSizeY; y++)
            {
                for (int x = 0; x < worldSizeX; x++)
                {
                    worldObjects[y, x]?.Draw(spriteBatch,WTFHelper.DRAW_LAYER.WORLD_OBJ.F());
                    generatedObjects[y, x]?.Draw(spriteBatch, WTFHelper.DRAW_LAYER.GENERATED_OBJ.F());
                }
            }

            if (isDebug == true)
            {
                for (int i = 0; i < DEBUGRECTS.Count; i++)
                {
                    spriteBatch.Draw(DrawHelper.GetTexture(),destinationRectangle: DEBUGRECTS[i].GetRectangle(), color: Color.FromNonPremultiplied(255, 255, 255, WTFHelper.alpha)/*, layerDepth: WTFHelper.DRAW_LAYER.DEBUG.F()*/);
                }
                isPaused = true;

                string debugStr = $"CurStepTime: {Math.Round(stepCurrentTime)}\n" +
                                  $"CurStepTimeToNewBlock: {Math.Round(currentStepCountToNewBlock)}";
 
                spriteBatch.DrawString(DrawHelper.spriteFont, debugStr, new Vector2(1100, 0), Color.FromNonPremultiplied(60, 60, 60, 255),0,Vector2.Zero,Vector2.One,SpriteEffects.None, WTFHelper.DRAW_LAYER.ABSOLUTE_FRONT.F());
            }
        }
        public override void GUIStateChanged(GuiObject sender)
        {
            base.GUIStateChanged(sender);
            //TODO HERE
            switch (sender.objectType)
            {
                case GuiObjectType.BUTTON:
                    Button btn = sender as Button;
                    if (btn.state == Button.State.Released)
                    {
                        //выбираем блок, или обработка кнопок
                        WTFHelper.WORLD_OBJ_TYPE clickedWordObjMenuItem;
                        if (Enum.TryParse<WTFHelper.WORLD_OBJ_TYPE>(btn.Name, out clickedWordObjMenuItem))
                        {
                            if (selectedWorldObj != clickedWordObjMenuItem)
                                selectedWorldObj = clickedWordObjMenuItem;
                            else
                                selectedWorldObj = WTFHelper.WORLD_OBJ_TYPE.EMPTY;
                        }
                        else
                        {
                            switch (btn.Name)
                            {
                                case "START_BTN":
                                    if (isStoped)
                                    {
                                        btn.Text = "STOP";
                                        this.state = SceneState.Running;
                                    }
                                    else
                                    {
                                        for (int y = 0; y < worldSizeY; y++)
                                        {
                                            for (int x = 0; x < worldSizeX; x++)
                                            {
                                                generatedObjects[y, x] = null;
                                            }
                                        }
                                        stepCurrentTime = 0.0;
                                        this.state = SceneState.Stoped;
                                        btn.Text = "START";
                                    }
                                    break;
                                case "PAUSE_BTN":
                                    this.state = SceneState.Paused;
                                    break;
                                case "BACK_BTN":
                                    App.GoToScene(WTFHelper.SCENES.APP3_LEVEL_SELECTOR);
                                    break;
                                case "CLEAR_BTN":
                                    if (isStoped)
                                    {
                                        for (int y = 0; y < worldSizeY; y++)
                                        {
                                            for (int x = 0; x < worldSizeX; x++)
                                            {
                                                if (worldObjects[y, x].isUser == true)
                                                {
                                                    worldObjects[y, x] = new WorldObj(WTFHelper.WORLD_OBJ_TYPE.EMPTY, x, y);
                                                }
                                            }
                                        }
                                    }
                                    break;
                                case "S+":
                                    if (stepTimeInMs >= 250)
                                    {
                                        //stepCurrentTime -= 475 * stepCurrentTime / stepTimeInMs;
                                        //stepTimeInMs -= 475;
                                        stepCurrentTime /= 2;
                                        stepTimeInMs /= 2;
                                        btn.classicButtonDefaultColor = WTFHelper.buttonDefaultColor;
                                    }
                                    else
                                    {
                                        btn.classicButtonDefaultColor = WTFHelper.buttonDisabledColor;
                                    }
                                    break;
                                case "S-":
                                    if (stepTimeInMs <= 2000)
                                    {
                                        //stepCurrentTime += 475 * stepCurrentTime / stepTimeInMs;
                                        //stepTimeInMs += 475;
                                        stepCurrentTime *= 2;
                                        stepTimeInMs *= 2;
                                        btn.classicButtonDefaultColor = WTFHelper.buttonDefaultColor;
                                    }
                                    else
                                    {
                                        btn.classicButtonDefaultColor = WTFHelper.buttonDisabledColor;
                                    }
                                    break;
                            }
                        }
                    }
                    break;
            }
        }
    }
}
