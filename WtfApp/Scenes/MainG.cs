using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using WtfApp.GUI;
using WtfApp.Classes;

namespace WtfApp
{
    public class MainG : Scenes.Scene
    {
        public List<RectangleF> DEBUGRECTS;

        public static int worldSizeX = 0;
        public static int worldSizeY = 0;
        public static int cellSize = 104;
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

        public MainG(Rectangle sceneRectangle,SaveLoadLevel sll) : base(WTFHelper.SCENES.GAME, sceneRectangle)
        {
            DEBUGRECTS = new List<RectangleF>();

            worldSizeX = sll.levelSizeX;
            worldSizeY = sll.levelSizeY;

            this.sceneRectangle = sceneRectangle;
            this.worldObjects = new WorldObj[worldSizeY, worldSizeX];
            this.generatedObjects = new GeneratedObj[worldSizeY, worldSizeX];
            this._tmpGeneratedObjects = new GeneratedObj[worldSizeY, worldSizeX];

            int worldItemSize = 96;
            int countWorldItemsInCol = 6;
            int buttonsHeight = 88;
            int buttonsInterval = 10;

            gameAreaRect = new Rectangle(Main.screenBounds.Left,
                                         Main.screenBounds.Top,
                                         (int)(Main.screenBounds.Width * worldPartWidth),
                                         (int)(Main.screenBounds.Height));

            worldObjectsRect = new Rectangle(gameAreaRect.Center.X - worldSizeX * MainG.cellSize / 2,
                                             gameAreaRect.Center.Y - worldSizeY * MainG.cellSize / 2,
                                             MainG.cellSize * worldSizeX, MainG.cellSize * worldSizeY);

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

            for (int i = 0; i < availableWorldObjects.Length; i++)
            {
                if (i == countWorldItemsInCol)
                {
                   /* AddButton("DELETE", "",
                     new Rectangle(worldMenuBtnRect.Left,
                     worldMenuBtnRect.Top + countWorldItemsInCol * worldItemSize, worldItemSize, worldItemSize),
                     WTFHelper.worldObjTextures[(int)WTFHelper.worldObjTextures, WTFHelper.worldObjTextures[(int)availableWorldObjects[i]]);
                    sceneButtons[i].borderColor = Color.Yellow;*/
                }
                else
                {
                    AddButton(availableWorldObjects[i].ToString(), "",
                        new Rectangle(worldMenuBtnRect.Left + (int)Math.Ceiling(i / countWorldItemsInCol * 1.0) * worldItemSize,
                        worldMenuBtnRect.Top + i % countWorldItemsInCol * worldItemSize, (int)(worldItemSize*1.3), (int)(worldItemSize*1.3)),
                        WTFHelper.worldObjTextures[availableWorldObjects[i].ToString()], WTFHelper.worldObjTextures[availableWorldObjects[i].ToString()]);
                    sceneButtons[i].borderColor = Color.Yellow;
                    sceneButtons[i].ChangeDefaultColor( Color.FromNonPremultiplied(255,255,255,WTFHelper.alpha));
                }
            }

            //AddButton("START_BTN", "START", new Rectangle(menuBtnRect.X, menuBtnRect.Y, menuBtnRect.Width, buttonsHeight));
            AddButton("START_BTN", "STOP", new Rectangle(menuBtnRect.X, menuBtnRect.Y, menuBtnRect.Width, buttonsHeight));

            AddButton("PAUSE_BTN", "PAUSE", new Rectangle(menuBtnRect.X, menuBtnRect.Y + (buttonsHeight + buttonsInterval), menuBtnRect.Width, buttonsHeight));
            AddButton("CLEAR_BTN", "CLEAR", new Rectangle(menuBtnRect.X, menuBtnRect.Y + (buttonsHeight + buttonsInterval) * 2, menuBtnRect.Width, buttonsHeight));
            AddButton("BACK_BTN", "BACK", new Rectangle(menuBtnRect.X, menuBtnRect.Y + (buttonsHeight + buttonsInterval) * 3, menuBtnRect.Width, buttonsHeight));

            AddButton("S-", "S-", new Rectangle(menuBtnRect.Left, menuBtnRect.Bottom - (buttonsHeight + buttonsInterval) , (int)(menuBtnRect.Width *0.3), buttonsHeight));
            AddButton("S+", "S+", new Rectangle(menuBtnRect.Right - (int)(menuBtnRect.Width*0.3) , menuBtnRect.Bottom - (buttonsHeight + buttonsInterval) , (int)(menuBtnRect.Width *0.3), buttonsHeight));
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

        public bool RotateGeneratedObjects(int xPos, int yPos, BaseObject.ROTATE_DIRRECTION direction)
        {
            //если рядом с поворотным блоком нет препятсвие(любой другой блок)
            if ((yPos - 1) >= 0 && worldObjects[yPos - 1, xPos].isBlocked == false && (generatedObjects[yPos - 1, xPos] == null 
                || (generatedObjects[yPos, xPos].weldId != 0 && generatedObjects[yPos, xPos].weldId == generatedObjects[yPos - 1, xPos].weldId)) &&
                (yPos + 1) < worldSizeY && worldObjects[yPos + 1, xPos].isBlocked == false && (generatedObjects[yPos + 1, xPos] == null 
                || (generatedObjects[yPos, xPos].weldId != 0 && generatedObjects[yPos, xPos].weldId == generatedObjects[yPos + 1, xPos].weldId)) &&
                (xPos - 1) >= 0 && worldObjects[yPos, xPos - 1].isBlocked == false && (generatedObjects[yPos, xPos - 1] == null 
                || (generatedObjects[yPos, xPos].weldId != 0 && generatedObjects[yPos, xPos].weldId == generatedObjects[yPos, xPos - 1].weldId)) &&
                (xPos + 1) < worldSizeX && worldObjects[yPos, xPos + 1].isBlocked == false && (generatedObjects[yPos, xPos + 1] == null 
                || (generatedObjects[yPos, xPos].weldId != 0 && generatedObjects[yPos, xPos].weldId == generatedObjects[yPos, xPos + 1].weldId)))
            {

            }
            else
            {
                return false;
            }

            //если блок не сварной
            if (generatedObjects[yPos, xPos].weldId == 0)
            {
                generatedObjects[yPos, xPos].Rotate(new Point(xPos, yPos), direction);
            }
            else //если сварной - поварачиваем всю группу
            {
                //проверка блоков на возможность поворота
                for (int y = 0; y < worldSizeY; y++)
                {
                    for (int x = 0; x < worldSizeX; x++)
                    {
                        if (generatedObjects[y, x] != null && generatedObjects[y, x].weldId == generatedObjects[yPos, xPos].weldId)
                        {
                            if (CheckRotate(xPos, yPos, x, y, direction, false) == false)
                            {
                                return false;
                            }
                        }
                    }
                }
                //блокируем область на которой будет осуществлен поворот при вызове NewStep блоков
                for (int y = 0; y < worldSizeY; y++)
                {
                    for (int x = 0; x < worldSizeX; x++)
                    {
                        if (generatedObjects[y, x] != null && generatedObjects[y, x].weldId == generatedObjects[yPos, xPos].weldId)
                        {
                            if (CheckRotate(xPos, yPos, x, y, direction, true) == false)
                            {
                                return false;
                            }
                        }
                    }
                }
                //поворот и перенос генерируемых объектов на временный массив
                for (int y = 0; y < worldSizeY; y++)
                {
                    for (int x = 0; x < worldSizeX; x++)
                    {
                        if (generatedObjects[y, x] != null)
                        {
                            if (generatedObjects[y, x].weldId == generatedObjects[yPos, xPos].weldId)
                            {
                                generatedObjects[y, x].Rotate(new Point(xPos, yPos), direction);
                            }
                        }
                    }
                }
            }
            return true;
        }

        public bool MoveBlock(int x, int y, WTFHelper.OBJ_MOVE_DIRECTION dir, bool isCheckOnly = false)
        {
            //isCheckOnly=true
            if (generatedObjects[y, x] == null || generatedObjects[y, x].isBlocked == true)
            {
                return false;
            }
            //направление платформы под блоком
            WTFHelper.OBJ_DRAW_DIRECTION worldObjDir = 0;

            if (worldObjects[y, x].objType == WTFHelper.WORLD_OBJ_TYPE.PLATFORM
                || worldObjects[y, x].objType == WTFHelper.WORLD_OBJ_TYPE.START)
            {
                worldObjDir = worldObjects[y, x].objDrawDir;
            }
            //если направление движения не задано - берем с платформы
            if (dir == WTFHelper.OBJ_MOVE_DIRECTION.NONE /*&& generatedObjects[y,x].weldId != 0*/)
            {
                dir = (WTFHelper.OBJ_MOVE_DIRECTION)worldObjDir;
            }

            var nextWorldObj = GetNextWorldObjOnDirection(x, y, (WTFHelper.OBJ_DRAW_DIRECTION)dir);
            var nextGeneratedObj = GetNextGeneratedObjOnDirection(x, y, dir);

            //если блок "сварной"
            if (generatedObjects[y, x].weldId != 0 && generatedObjects[y, x].isMoveDirectionSet == false)
            {
                generatedObjects[y, x].isMoveDirectionSet = true;
                bool rezCheck = true;

                if ((y - 1) >= 0
                    && generatedObjects[y - 1, x] != null
                    && generatedObjects[y - 1, x].weldId != 0
                    && generatedObjects[y - 1, x].isMoveDirectionSet == false
                    && rezCheck == true
                    && generatedObjects[y, x].weldId == generatedObjects[y - 1, x].weldId)
                {
                    rezCheck = MoveBlock(x, y - 1, dir, isCheckOnly);
                }
                if ((y + 1) < worldSizeY
                    && generatedObjects[y + 1, x] != null
                    && generatedObjects[y + 1, x].weldId != 0
                    && generatedObjects[y + 1, x].isMoveDirectionSet == false
                    && rezCheck == true
                    && generatedObjects[y, x].weldId == generatedObjects[y + 1, x].weldId)
                {
                    rezCheck = MoveBlock(x, y + 1, dir, isCheckOnly);
                }
                if ((x - 1) >= 0
                    && generatedObjects[y, x - 1] != null
                    && generatedObjects[y, x - 1].weldId != 0
                    && generatedObjects[y, x - 1].isMoveDirectionSet == false
                    && rezCheck == true
                    && generatedObjects[y, x].weldId == generatedObjects[y, x - 1].weldId)
                {
                    rezCheck = MoveBlock(x - 1, y, dir, isCheckOnly);
                }
                if ((x + 1) < worldSizeX
                    && generatedObjects[y, x + 1] != null
                    && generatedObjects[y, x + 1].weldId != 0
                    && generatedObjects[y, x + 1].isMoveDirectionSet == false
                    && rezCheck == true &&
                    generatedObjects[y, x].weldId == generatedObjects[y, x + 1].weldId)
                {
                    rezCheck = MoveBlock(x + 1, y, dir, isCheckOnly);
                }
                if (isCheckOnly == true)
                {
                    generatedObjects[y, x].isMoveDirectionSet = false;
                }

                if (rezCheck == false)
                {
                    return false;
                }
            }
            //Конец сварки
            if (isCheckOnly != true)
            {
                generatedObjects[y, x].isMoveDirectionSet = true;
            }

            //если блок находится на платформе и направление платформы не совпадает с заданым направлением 
            if (worldObjects[y, x].objType == WTFHelper.WORLD_OBJ_TYPE.PLATFORM
                && (int)worldObjDir != (int)dir
                && generatedObjects[y, x].weldId == 0)
            {
                //если движение по платформе на которой находится блок - невозможно, тогда продолжаем пытаться продвинуть в заданом направдении
                if (MoveBlock(x, y, (WTFHelper.OBJ_MOVE_DIRECTION)worldObjDir, isCheckOnly) == true)
                {
                    return false;
                }
            }

            if (dir != WTFHelper.OBJ_MOVE_DIRECTION.NONE && CordInWorldAreaRange(x, y, dir) == true)
            {
                //если впереди нет блока и нет стены
                if (nextGeneratedObj == null && nextWorldObj.isBlocked != true)
                {
                    if (isCheckOnly != true)
                    {
                        generatedObjects[y, x].objMoveDir = dir;
                        //блокируем след.пустую ячейку(на которую движемся)
                        nextWorldObj.isBlocked = true;
                    }
                    return true;
                }
                //логика для движения сварочных блоков
                else if (nextGeneratedObj != null && nextGeneratedObj.weldId != 0)
                {

                    if (nextGeneratedObj.weldId == generatedObjects[y, x].weldId)
                    {
                        if (isCheckOnly != true)
                        {
                            generatedObjects[y, x].objMoveDir = dir;
                        }
                        return true;
                    }
                    else if (nextGeneratedObj.objMoveDir == dir)
                    {
                        if (isCheckOnly != true)
                        {
                            generatedObjects[y, x].objMoveDir = dir;
                        }
                        return true;
                    }
                    else if (nextGeneratedObj.objMoveDir == WTFHelper.OBJ_MOVE_DIRECTION.NONE)
                    {
                        bool chekedRez = MoveBlock(nextGeneratedObj.xPos, nextGeneratedObj.yPos, dir, true);
                        if (chekedRez == true)
                        {
                            if (isCheckOnly != true)
                            {
                                generatedObjects[y, x].objMoveDir = dir;
                                MoveBlock(nextGeneratedObj.xPos, nextGeneratedObj.yPos, dir, false);
                            }
                            return true;
                        }
                    }
                    return false;
                }

                //если блок заблокирован блоком
                else if (nextGeneratedObj != null && nextGeneratedObj.objMoveDir == WTFHelper.OBJ_MOVE_DIRECTION.NONE)
                {
                    //блокируем текущий блок дабы не влезть в рекурсию
                    generatedObjects[y, x].isBlocked = true;

                    bool rez = MoveBlock(nextGeneratedObj.xPos, nextGeneratedObj.yPos, dir, isCheckOnly);

                    generatedObjects[y, x].isBlocked = false;
                    if (rez == true)
                    {
                        if (isCheckOnly != true)
                        {
                            generatedObjects[y, x].objMoveDir = dir;
                        }
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return false;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            //TODO HERE
            if (isPause == false)
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

                    //обновляем состояние массива (меняем индексы объектов так как у них могло поменятся местоположение)
                    for (int y = 0; y < worldSizeY; y++)
                    {
                        for (int x = 0; x < worldSizeX; x++)
                        {
                            if (generatedObjects[y, x]!=null)
                                _tmpGeneratedObjects[generatedObjects[y, x].yPos, generatedObjects[y, x].xPos] = generatedObjects[y, x];
                        }
                    }
                    for (int y = 0; y < worldSizeY; y++)
                    {
                        for (int x = 0; x < worldSizeX; x++)
                        {
                            generatedObjects[y, x] = null;
                            if (_tmpGeneratedObjects[y, x] != null)
                            {
                                generatedObjects[y, x] = _tmpGeneratedObjects[y, x];
                                _tmpGeneratedObjects[y, x] = null;
                            }
                        } 
                    }


                    for (int y = 0; y < worldSizeY; y++)
                    {
                        for (int x = 0; x < worldSizeX; x++)
                        {
                            //создаем новые блоки на старте
                            if (worldObjects[y, x]?.objType == WTFHelper.WORLD_OBJ_TYPE.START && generatedObjects[y, x] == null && currentStepCountToNewBlock == 0)
                            {
                                generatedObjects[y, x] = new GeneratedObj(WTFHelper.GENERATED_OBJ_TYPE.BOX, x, y);
                                generatedObjects[y, x].ChangeOffsetDraw(offsetDraw);
                            }
                            //удаляем блоки с финиша                          
                            else if (worldObjects[y, x]?.objType == WTFHelper.WORLD_OBJ_TYPE.FINISH && generatedObjects[y, x] != null)
                            {
                                score = score + 1;
                                generatedObjects[y, x] = null;
                            }
                        }
                    }


                    //сварка
                    for (int y = 0; y < worldSizeY; y++)
                    {
                        for (int x = 0; x < worldSizeX; x++)
                        {
                            //свариваем блоки
                            if (worldObjects[y, x].objType == WTFHelper.WORLD_OBJ_TYPE.WELDING)
                            {
                                //если ниже сварка с таким же направлением по оси Х
                                if (worldObjects[y, x].objDrawDir == WTFHelper.OBJ_DRAW_DIRECTION.LEFT || worldObjects[y, x].objDrawDir == WTFHelper.OBJ_DRAW_DIRECTION.RIGHT)
                                {              
                                    if ((y + 1) <= worldSizeY && worldObjects[y + 1, x].objType == WTFHelper.WORLD_OBJ_TYPE.WELDING &&
                                         worldObjects[y + 1, x].objDrawDir == worldObjects[y, x].objDrawDir)
                                    {
                                        int xWeldDir = worldObjects[y, x].objDrawDir.X();
                                        //если рядом 2 не свареных между собой блока
                                        if (generatedObjects[y, x + xWeldDir] != null
                                         && generatedObjects[y + 1, x + xWeldDir] != null
                                         && (generatedObjects[y, x + xWeldDir].weldId != generatedObjects[y + 1, x + xWeldDir].weldId
                                         || generatedObjects[y, x + xWeldDir].weldId == 0 && generatedObjects[y + 1, x + xWeldDir].weldId == 0))
                                        {
                                            //берем больший weld id
                                            int weldId = generatedObjects[y, x + xWeldDir].weldId > generatedObjects[y + 1, x + xWeldDir].weldId ?
                                                generatedObjects[y, x + xWeldDir].weldId : generatedObjects[y + 1, x + xWeldDir].weldId;

                                            if (weldId == 0)
                                            {
                                                weldId = curGameWeldId;
                                                curGameWeldId++;
                                            }

                                            if (generatedObjects[y, x + xWeldDir].weldId == 0)
                                                generatedObjects[y, x + xWeldDir].weldId = weldId;
                                            else if (generatedObjects[y, x + xWeldDir].weldId != weldId)
                                            {
                                                SetWeldId(x + xWeldDir, y, weldId);
                                            }

                                            if (generatedObjects[y + 1, x + xWeldDir].weldId == 0)
                                                generatedObjects[y + 1, x + xWeldDir].weldId = weldId;
                                            else
                                            if (generatedObjects[y + 1, x + xWeldDir].weldId != weldId)
                                            {
                                                SetWeldId(x + xWeldDir, y + 1, weldId);
                                            }
                                        }
                                    }
                                }
                                //если справа сварка с таким же направлением по оси Y
                                else if (worldObjects[y, x].objDrawDir == WTFHelper.OBJ_DRAW_DIRECTION.TOP || worldObjects[y, x].objDrawDir == WTFHelper.OBJ_DRAW_DIRECTION.BOTTOM)
                                {      
                                    if ((x + 1) <= worldSizeX && worldObjects[y, x+1].objType == WTFHelper.WORLD_OBJ_TYPE.WELDING &&
                                         worldObjects[y, x+1].objDrawDir == worldObjects[y, x].objDrawDir)
                                    {
                                        int yWeldDir = worldObjects[y, x].objDrawDir.Y();
                                        //если рядом 2 не свареных между собой блока
                                        if (generatedObjects[y + yWeldDir, x] != null
                                         && generatedObjects[y + yWeldDir, x + 1] != null
                                         && (generatedObjects[y + yWeldDir, x].weldId != generatedObjects[y + yWeldDir, x + 1].weldId
                                         || generatedObjects[y + yWeldDir, x].weldId == 0 && generatedObjects[y + yWeldDir, x + 1].weldId == 0))
                                        {

                                            int weldId = generatedObjects[y + yWeldDir, x].weldId > generatedObjects[y + yWeldDir, x + 1].weldId ?
                                                generatedObjects[y + yWeldDir, x].weldId : generatedObjects[y + yWeldDir, x + 1].weldId;
                                            if (weldId == 0)
                                            {
                                                weldId = curGameWeldId;
                                                curGameWeldId++;
                                            }

                                            if (generatedObjects[y + yWeldDir, x].weldId == 0)
                                                generatedObjects[y + yWeldDir, x].weldId = weldId;
                                            else if (generatedObjects[y + yWeldDir, x].weldId != weldId)
                                            {
                                                SetWeldId(x, y + yWeldDir, weldId);
                                            }
                                            if (generatedObjects[y + yWeldDir, x + 1].weldId == 0)
                                                generatedObjects[y + yWeldDir, x + 1].weldId = weldId;
                                            else if (generatedObjects[y + yWeldDir, x + 1].weldId != weldId)
                                            {
                                                SetWeldId(x + 1, y + yWeldDir, weldId);
                                            }
                                        }
                                    }
                                }     
                            }
                        }
                    }

                    //поворот
                    for (int y = 0; y < worldSizeY; y++)
                    {
                        for (int x = 0; x < worldSizeX; x++)
                        {
                            //поворот блока/группы блоков
                            if (generatedObjects[y, x] != null
                                    && (worldObjects[y, x].objType == WTFHelper.WORLD_OBJ_TYPE.ROTATE || worldObjects[y, x].objType == WTFHelper.WORLD_OBJ_TYPE.ROTATEBACK))
                            {
                                if (generatedObjects[y, x].isRotated == false && generatedObjects[y, x].isBlocked == false)
                                {
                                    RotateGeneratedObjects(x, y, (worldObjects[y, x].objType == WTFHelper.WORLD_OBJ_TYPE.ROTATE ?
                                        BaseObject.ROTATE_DIRRECTION.RIGHT : BaseObject.ROTATE_DIRRECTION.LEFT));
                                }
                            }
                        }
                    }
                    //указываем направление движения блокам
                    for (int y = 0; y < worldSizeY; y++)
                    {
                        for (int x = 0; x < worldSizeX; x++)
                        {
                            if (generatedObjects[y, x]?.isMoveDirectionSet == false && generatedObjects[y, x]?.isRotated==false)
                            {
                                //если блок "сварной"
                                if (generatedObjects[y, x].weldId != 0)
                                {
                                    if (MoveBlock(x, y, WTFHelper.OBJ_MOVE_DIRECTION.NONE, true) == true)
                                        MoveBlock(x, y, WTFHelper.OBJ_MOVE_DIRECTION.NONE);
                                }
                                else
                                {
                                    MoveBlock(x, y, WTFHelper.OBJ_MOVE_DIRECTION.NONE);
                                }
                            }
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

        public void SetWeldId(int x,int y, int newID )
        {
            //нужно заменить дирсет 
            if (generatedObjects[y,x].weldId != 0 && generatedObjects[y,x].isProcessed == false)
            {
                generatedObjects[y,x].isProcessed = true;
                if (y - 1 > 0 && generatedObjects[y - 1,x] != null && generatedObjects[y - 1,x].weldId != 0 && generatedObjects[y,x].weldId == generatedObjects[y - 1,x].weldId)
                {
                    SetWeldId(x, y - 1, newID);
                }
                if (y + 1 <= worldSizeY && generatedObjects[y + 1,x] != null && generatedObjects[y + 1,x].weldId != 0 && generatedObjects[y,x].weldId == generatedObjects[y + 1,x].weldId)
                {
                    SetWeldId(x, y + 1, newID);
                }
                if (x - 1 > 0 && generatedObjects[y,x - 1] != null && generatedObjects[y,x - 1].weldId != 0 && generatedObjects[y,x].weldId == generatedObjects[y,x - 1].weldId)
                {
                    SetWeldId(x - 1, y, newID);
                }
                if (x + 1 <= worldSizeX && generatedObjects[y,x + 1] != null && generatedObjects[y,x + 1].weldId != 0 && generatedObjects[y,x].weldId == generatedObjects[y,x + 1].weldId)
                {
                    SetWeldId(x + 1, y, newID);
                }
                generatedObjects[y,x].weldId = newID;
                generatedObjects[y,x].isProcessed = false;
            }
        }
        public bool CheckRotate(int cX, int cY, int x, int y, BaseObject.ROTATE_DIRRECTION direction, bool needBlockRotateArea)
        {
            int fX = 0;
            int fY = 0;
            int dir = (int)direction;
            float dopusk = 0.05f;
            //HalfCeilSize(коэф.точност, 0.5 - максимальная)

            float hc = 0.5f;
            //радиус от точки до точки
            double radius = Math.Sqrt(Math.Pow(Math.Abs(cX - x), 2) + Math.Pow(Math.Abs(cY - y), 2));
            fY = cY - (cX - x) * dir;
            fX = cX + (cY - y) * dir;

            double ccX = cX + 0.5;
            double ccY = cY + 0.5;
            // PI/2=90градусов, radius*N - плотность точек проверки
            for (double i = 0; i <= MathHelper.PiOver2 && radius>0; i += MathHelper.PiOver2/radius/10 /* MathHelper.PiOver2/radius*6.0*/)
            {
                // формула поворота точки(x, y) отностительно другой(x0, y0)
                //X = x0 + (x - x0) * cos(a) - (y - y0) * sin(a);
                //Y = y0 + (y - y0) * cos(a) + (x - x0) * sin(a);

                double ltX = ccX + (x - ccX + dopusk) * Math.Cos(i * dir) - (y - ccY + dopusk) * Math.Sin(i * dir);
                double ltY = ccY + (y - ccY + dopusk) * Math.Cos(i * dir) + (x - ccX + dopusk) * Math.Sin(i * dir);
                double lbX = ccX + (x - ccX + dopusk) * Math.Cos(i * dir) - (y + 1 - ccY - dopusk) * Math.Sin(i * dir);
                double lbY = ccY + (y + 1 - ccY - dopusk) * Math.Cos(i * dir) + (x - ccX + dopusk) * Math.Sin(i * dir);
                double rtX = ccX + (x + 1 - ccX - dopusk) * Math.Cos(i * dir) - (y - ccY + dopusk) * Math.Sin(i * dir);
                double rtY = ccY + (y - ccY + dopusk) * Math.Cos(i * dir) + (x + 1 - ccX - dopusk) * Math.Sin(i * dir);
                double rbX = ccX + (x + 1 - ccX - dopusk) * Math.Cos(i * dir) - (y + 1 - ccY - dopusk) * Math.Sin(i * dir);
                double rbY = ccY + (y + 1 - ccY - dopusk) * Math.Cos(i * dir) + (x + 1 - ccX - dopusk) * Math.Sin(i * dir);

                /*FOR DEBUG
                print(round(ltX, 2)..'/'..round(ltY, 2)..'\t'..round(rtX, 2)..'/'..round(rtY, 2))
                print(round(lbX, 2)..'/'..round(lbY, 2)..'\t'..round(rbX, 2)..'/'..round(rbY, 2))
                print('Floor')
                print(math.Floor(ltX)..'/'..math.Floor(ltY)..'\t'..math.Floor(rtX)..'/'..math.Floor(rtY))
                print(math.Floor(lbX)..'/'..math.Floor(lbY)..'\t'..math.Floor(rbX)..'/'..math.Floor(rbY)..'\n')
                display.newRect(worldDGroup, math.Floor(ltX) * cellSize, math.Floor(ltY) * cellSize, cellSize / 5 - 2, cellSize / 5 - 2)
                display.newRect(worldDGroup, math.Floor(rtX) * cellSize, math.Floor(rtY) * cellSize, cellSize / 5 - 2, cellSize / 5 - 2)
                display.newRect(worldDGroup, math.Floor(lbX) * cellSize, math.Floor(lbY) * cellSize, cellSize / 5 - 2, cellSize / 5 - 2)
                display.newRect(worldDGroup, math.Floor(rbX) * cellSize, math.Floor(rbY) * cellSize, cellSize / 5 - 2, cellSize / 5 - 2)
                display.newRect(worldDGroup, (ltX - 0.5) * cellSize, (ltY - 0.5) * cellSize, cellSize / 5 - 2, cellSize / 5 - 2)
                display.newRect(worldDGroup, (rtX - 0.5) * cellSize, (rtY - 0.5) * cellSize, cellSize / 5 - 2, cellSize / 5 - 2)
                display.newRect(worldDGroup, (lbX - 0.5) * cellSize, (lbY - 0.5) * cellSize, cellSize / 5 - 2, cellSize / 5 - 2)
                display.newRect(worldDGroup, (rbX - 0.5) * cellSize, (rbY - 0.5) * cellSize, cellSize / 5 - 2, cellSize / 5 - 2)
                */

                // DEBUGRECTS.Clear();
                
                DEBUGRECTS.Add(new RectangleF(Math.Floor(ltX) * cellSize+cellSize/2, Math.Floor(ltY) * cellSize + cellSize / 2, 1 / Main.screenScale, 1 / Main.screenScale));
                DEBUGRECTS.Add(new RectangleF(Math.Floor(rtX) * cellSize + cellSize / 2, Math.Floor(rtY) * cellSize + cellSize / 2, 1 / Main.screenScale, 1 / Main.screenScale));
                DEBUGRECTS.Add(new RectangleF(Math.Floor(lbX) * cellSize + cellSize / 2, Math.Floor(lbY) * cellSize + cellSize / 2, 1 / Main.screenScale, 1 / Main.screenScale));
                DEBUGRECTS.Add(new RectangleF(Math.Floor(rbX) * cellSize + cellSize / 2, Math.Floor(rbY) * cellSize + cellSize / 2, 1 / Main.screenScale, 1 / Main.screenScale));
                
                /*
                DEBUGRECTS.Add(new RectangleF((ltX) * cellSize, ((ltY) * cellSize), 1 / Main.screenScale, 1 / Main.screenScale));
                DEBUGRECTS.Add(new RectangleF((rtX) * cellSize, ((rtY) * cellSize), 1 / Main.screenScale, 1 / Main.screenScale));
                DEBUGRECTS.Add(new RectangleF((lbX ) * cellSize, ((lbY ) * cellSize), 1 / Main.screenScale, 1 / Main.screenScale));
                DEBUGRECTS.Add(new RectangleF((rbX) * cellSize, ((rbY) * cellSize), 1/Main.screenScale, 1/Main.screenScale));
                */

                ltY = Math.Floor(ltY);
                ltX = Math.Floor(ltX);
                lbY = Math.Floor(lbY);
                lbX = Math.Floor(lbX);
                rtY = Math.Floor(rtY);
                rtX = Math.Floor(rtX);
                rbY = Math.Floor(rbY);
                rbX = Math.Floor(rbX);

                if (needBlockRotateArea == true)
                {
                    worldObjects[(int)ltY, (int)ltX].isBlocked = true;
                    worldObjects[(int)lbY, (int)lbX].isBlocked = true;
                    worldObjects[(int)rtY, (int)rtX].isBlocked = true;
                    worldObjects[(int)rbY, (int)rbX].isBlocked = true;
                }
                else
                {
                    if (ltX >= 0 && ltX < worldSizeX  && ltY >= 0 && ltY < worldSizeY  &&
                        lbX >= 0 && lbX < worldSizeX  && lbY >= 0 && lbY < worldSizeY  &&
                        rtX >= 0 && rtX < worldSizeX  && rtY >= 0 && rtY < worldSizeY  &&
                        rbX >= 0 && rbX < worldSizeX  && rbY >= 0 && rbY < worldSizeY  &&
                        (generatedObjects[(int)ltY, (int)ltX] == null || generatedObjects[(int)ltY, (int)ltX].weldId == generatedObjects[(int)y, (int)x].weldId) && worldObjects[(int)ltY, (int)ltX].isBlocked == false &&
                        (generatedObjects[(int)lbY, (int)lbX] == null || generatedObjects[(int)lbY, (int)lbX].weldId == generatedObjects[(int)y, (int)x].weldId) && worldObjects[(int)lbY, (int)lbX].isBlocked == false &&
                        (generatedObjects[(int)rtY, (int)rtX] == null || generatedObjects[(int)rtY, (int)rtX].weldId == generatedObjects[(int)y, (int)x].weldId) && worldObjects[(int)rtY, (int)rtX].isBlocked == false &&
                        (generatedObjects[(int)rbY, (int)rbX] == null || generatedObjects[(int)rbY, (int)rbX].weldId == generatedObjects[(int)y, (int)x].weldId) && worldObjects[(int)rbY, (int)rbX].isBlocked == false)
                    {
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        public override void Touch(Point touch, ButtonState touchState, bool isPressedMove)
        {
            base.Touch(touch, touchState, isPressedMove);

            if (worldObjectsRect.Contains(touch))
            {
                double doubleTouchedWorldX = (touch.X - offsetDraw.X) / MainG.cellSize;
                double doubleTouchedWorldY = (touch.Y - offsetDraw.Y) / MainG.cellSize;
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
                    //отрисовка лучей сварки
                    if (worldObjects[y, x]?.objType == WTFHelper.WORLD_OBJ_TYPE.WELDING)
                    {
                        //если внизу/справа объект "сварка" с таким же направлением отрисовки
                        if ((CordInWorldAreaRange(x, y, WTFHelper.OBJ_MOVE_DIRECTION.BOTTOM) 
                                && (worldObjects[y, x].objDrawDir== WTFHelper.OBJ_DRAW_DIRECTION.LEFT || worldObjects[y, x].objDrawDir== WTFHelper.OBJ_DRAW_DIRECTION.RIGHT)
                                && worldObjects[y + 1, x ].objType == WTFHelper.WORLD_OBJ_TYPE.WELDING
                                && worldObjects[y + 1, x ].objDrawDir == worldObjects[y, x].objDrawDir)
                           || (CordInWorldAreaRange(x, y, WTFHelper.OBJ_MOVE_DIRECTION.RIGHT)
                                && (worldObjects[y, x].objDrawDir == WTFHelper.OBJ_DRAW_DIRECTION.TOP || worldObjects[y, x].objDrawDir == WTFHelper.OBJ_DRAW_DIRECTION.BOTTOM)
                                && worldObjects[y , x + 1].objType == WTFHelper.WORLD_OBJ_TYPE.WELDING
                                && worldObjects[y , x + 1].objDrawDir == worldObjects[y, x].objDrawDir))
                        {
                            Rectangle drawRect = new Rectangle(
                                worldObjects[y, x].drawRect.Center.X - (worldObjects[y, x].objDrawDir.X() < 0 ? cellSize : 0),
                                worldObjects[y, x].drawRect.Center.Y - (worldObjects[y, x].objDrawDir.Y() < 0 ? cellSize : 0),
                                cellSize, cellSize
                                );
                            spriteBatch.Draw(WTFHelper.effectsTextures[WTFHelper.EFFECT_TYPE.WELD.ToString()],
                              drawRect,
                              null,
                              WTFHelper.objColor,
                              (MathHelper.PiOver2 * ((int)worldObjects[y, x].objDrawDir - 1)),
                              new Vector2(WTFHelper.textureSize / 2),
                              SpriteEffects.None,
                              WTFHelper.DRAW_LAYER.EFFECT_OBJ.F());
                        }
                    }
                }
            }

            if (isDebug == true)
            {
                for (int i = 0; i < DEBUGRECTS.Count; i++)
                {
                    spriteBatch.Draw(DrawHelper.emptyTexture,destinationRectangle: DEBUGRECTS[i].GetRectangle(), color: Color.FromNonPremultiplied(255, 255, 255, WTFHelper.alpha), layerDepth: WTFHelper.DRAW_LAYER.DEBUG.F());
                }
                isPaused = true;

                string debugStr = $"CurStepTime: {Math.Round(stepCurrentTime)}\n" +
                                  $"CurStepTimeToNewBlock: {Math.Round(currentStepCountToNewBlock)}";
 
                spriteBatch.DrawString(DrawHelper.spriteFont, debugStr, new Vector2(1100, 0), Color.FromNonPremultiplied(60, 60, 60, 255),0,Vector2.Zero,Vector2.One,SpriteEffects.None, WTFHelper.DRAW_LAYER.DEBUG.F());
            }
        }
        public override void ButtonStateChanged(Button sender)
        {
            base.ButtonStateChanged(sender);
            //TODO HERE

            if (sender.state == Button.State.Released)
            {
                //выбираем блок, или обработка кнопок
                WTFHelper.WORLD_OBJ_TYPE clickedWordObjMenuItem;
                if (Enum.TryParse<WTFHelper.WORLD_OBJ_TYPE>(sender.Name, out clickedWordObjMenuItem))
                {
                    if (selectedWorldObj != clickedWordObjMenuItem)
                        selectedWorldObj = clickedWordObjMenuItem;
                    else
                        selectedWorldObj = WTFHelper.WORLD_OBJ_TYPE.EMPTY;
                }
                else
                {
                    switch (sender.Name)
                    {
                        case "START_BTN":
                            if (isStoped)
                            {
                                sceneButtons.Find(m => m.Name == "START_BTN").Text = "STOP";
                                this.isPause = false;
                                this.isStoped = false;
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
                                this.isStoped = true;
                                this.isPause = true;
                                sceneButtons.Find(m => m.Name == "START_BTN").Text = "START";
                            }
                            break;
                        case "PAUSE_BTN":
                            this.isPause = true;
                            break;
                        case "BACK_BTN":
                            Main.GoToScene(WTFHelper.SCENES.LEVEL_SELECTOR);
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
                                sceneButtons.GetButton("S-").classicButtonDefaultColor = WTFHelper.buttonDefaultColor;
                            }
                            else
                            {
                                sceneButtons.GetButton("S+").classicButtonDefaultColor = WTFHelper.buttonDisabledColor;
                            }
                            break;
                        case "S-":
                            if (stepTimeInMs <= 2000)
                            {
                                //stepCurrentTime += 475 * stepCurrentTime / stepTimeInMs;
                                //stepTimeInMs += 475;
                                stepCurrentTime *= 2;
                                stepTimeInMs *= 2;
                                sceneButtons.GetButton("S+").classicButtonDefaultColor = WTFHelper.buttonDefaultColor;
                            }
                            else
                            {
                                sceneButtons.GetButton("S-").classicButtonDefaultColor = WTFHelper.buttonDisabledColor;
                            }
                            break;
                    }
                }
            }
        }
    }
}
