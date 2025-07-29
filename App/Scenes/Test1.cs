using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using WtfApp.GUI;
using WtfApp;
using WtfApp.Engine.Scene;

namespace WtfApp.Scenes
{
    public enum Direction : int { UP, RIGHT, DOWN, LEFT };

    public static class Help
    {
        public static int X(this Direction direction)
        {
            return direction == Direction.LEFT ? -2 : (direction == Direction.RIGHT ? 2 : 0);
        }
        public static int Y(this Direction direction)
        {
            return direction == Direction.UP ? -2 : (direction == Direction.DOWN ? 2 : 0);
        }
    }

    public class Test1 : Scene
    {

        struct GridCell
        {
            public bool isWay;//for debug
            public int id;//for debug
            public int type;
            public int x, y;
            public int dirX, dirY;
            public Direction dirToPreviousCell;
        }

        Label l;

        int gridDrawSize = 20;
        int gridSizeX;
        int gridSizeY;
        Random random;
        GridCell[,] grid;

        int whileCount = 0;//for debug
        public List<Thread> threads;

        int maxX, maxY, maxId;

        public Test1(Rectangle sceneRectangle) : base(WTFHelper.SCENES.TEST1, sceneRectangle)
        {
            threads = new List<Thread>();
            l = new Label("LABEL1", "TEST1", new Rectangle(0, 0, 200, 100), DrawHelper.spriteFont, Color.Black, AlignXY.RIGHT_BOTTOM);
            AddComponent(new Button("RECREATE", "RECREATE", new Rectangle(sceneRectangle.Right - 300, sceneRectangle.Top + 20, 280, 140)));
            AddComponent(new Button("BACK", "BACK", new Rectangle(App.screenBounds.Right - 320, App.screenBounds.Bottom - 170, 300, 150)));
            random = new Random();
            Create(75, 50);
            SetWay(1, 1, maxX, maxY);
        }

        public void Create(int x, int y)
        {
            whileCount = 0;
            gridSizeX = x;
            gridSizeY = y;
            grid = new GridCell[y, x];

            for (int y1 = 0; y1 < y; y1++)
            {
                for (int x1 = 0; x1 < x; x1++)
                {
                    grid[y1, x1] = new GridCell();
                    int tmpVAL = (y1 % 2 != 0 && x1 % 2 != 0) ? 0 : -1;
                    grid[y1, x1].id = tmpVAL;
                }
            }
            grid[1, 1].id = 1;

            /*grid[3, 3].id = -1;
            grid[3, 5].id = -1;
            grid[5, 3].id = -1;
            grid[5, 5].id = -1;

            grid[13, 7].id = -1;
            grid[15, 7].id = -1;
            grid[13, 9].id = -1;
            grid[15, 9].id = -1;

            grid[7, 13].id = -1;
            grid[7, 15].id = -1;
            grid[7, 17].id = -1;
            grid[9, 13].id = -1;
            grid[9, 15].id = -1;
            grid[9, 17].id = -1;*/

            threads.Add(new Thread(new ParameterizedThreadStart(CreateLab)));
            threads.Last().Start((startX: 1, startY: 1,  2, color: Color.Red));

            Thread.Sleep(50);     
            for (int i = 0; i < threads.Count; i++)
            {
                if (threads[i].IsAlive == true)
                {
                    //Thread.Sleep(5000);
                    i = 0;
                }
            }

            threads.Clear();
            //CreateLab((1, 1, 1, Color.Red, 1));
        }

        public override void GUIStateChanged(GuiObject sender)
        {
            if (sender.state == Button.State.Released)
                switch (sender.Name)
                {
                    case "RECREATE":
                        maxId = 0;
                        maxX = 0;
                        maxY = 0;
                        Create(75, 50);
                        SetWay(1, 1, maxX, maxY);
                        break;
                    case "BACK":
                        App.GoToScene(WTFHelper.SCENES.MAIN_MENU);
                        break;
                }

            base.GUIStateChanged(sender);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            l.Draw(spriteBatch, WTFHelper.DRAW_LAYER.GUI.F());
            spriteBatch.DrawString(DrawHelper.spriteFont, whileCount.ToString(), new Vector2(1500, 300), Color.Green, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 1);
            String threadsAliveCount = "";

            for (int i = 0; i < threads.Count; i++)
            {
                if (threads[i].IsAlive)
                {
                    threadsAliveCount += i.ToString() + '\n';
                }
            }

            spriteBatch.DrawString(DrawHelper.spriteFont, threadsAliveCount, new Vector2(1500, 400), Color.Green, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 1);
            spriteBatch.DrawString(DrawHelper.spriteFont, threads.Count.ToString(), new Vector2(1700, 400), Color.Green, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 1);

            for (int y = 0; y < gridSizeY; y++)
            {
                for (int x = 0; x < gridSizeX; x++)
                {
                    if (grid[y, x].id < 0)
                    {
                        spriteBatch.Draw(DrawHelper.GetTexture(), new Rectangle(x * gridDrawSize, y * gridDrawSize, gridDrawSize, gridDrawSize), Color.FromNonPremultiplied(28, 28, 28, 255));
                    }
                    else //if(grid[y, x].type == 1)
                    {
                        
                        if(grid[y,x].isWay)
                            spriteBatch.Draw(DrawHelper.GetTexture(), new Rectangle(x * gridDrawSize, y * gridDrawSize, gridDrawSize, gridDrawSize), Color.FromNonPremultiplied(32 /*+ grid[y, x].id / 5*/, 140, 32, 255));
                        else
                            spriteBatch.Draw(DrawHelper.GetTexture(), new Rectangle(x * gridDrawSize, y * gridDrawSize, gridDrawSize, gridDrawSize), Color.FromNonPremultiplied(132 /*+ grid[y, x].id / 5*/, 32, 32, 255));
                        //  spriteBatch.DrawString(DrawHelper.spriteFont, grid[y, x].id.ToString(), new Vector2(x * gridDrawSize /*+ gridDrawSize / 2*/, y * gridDrawSize /*+ gridDrawSize / 2*/), Color.FromNonPremultiplied(25 + grid[y, x].id / 5, 28, 28, 255), 0, Vector2.Zero, 1.0f, SpriteEffects.None, 1);
                    }
                }
            }
        }

        public override void Touch(Point touch, ButtonState touchState, bool isPressedMove)
        {
            base.Touch(touch, touchState, isPressedMove);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            int stepTimeInMs = 3000;

            double stepProgress = 1.0 / stepTimeInMs * (gameTime.TotalGameTime.TotalMilliseconds % stepTimeInMs);
        }

        public void CreateLab(object obj/*,int startX, int startY, int wayVal, Color color, int threadID*/)
        {
            var param = ((int startX, int startY, int initId, Color color))obj;

            int id = param.initId;
            int x = param.startX;
            int y = param.startY;

            int[] neighbors = new int[] { 0, 1, 2, 3 };

            int xOffset = 0;
            int yOffset = 0;
            Direction val;
            int rand = 0;
            int i = grid[y, x].type + 2;
            int prevX = 0;
            int prevY = 0;

            while (true)
            {
                whileCount++;
                if (neighbors.Length == 0)
                {
                    prevX = x - grid[y, x].dirToPreviousCell.X();
                    prevY = y - grid[y, x].dirToPreviousCell.Y();

                    if (grid[y, x].id <= param.initId)
                        break;

                    x = prevX;
                    y = prevY;
                    id--;

                    neighbors = new int[] { 0, 1, 2, 3 };
                }

                rand = random.Next(0, neighbors.Length);
                val = (Direction)neighbors[rand];

                xOffset = val.X();  //--*needOffset
                yOffset = val.Y();  //--*needOffset

                if (x + xOffset >= 0 && x + xOffset < grid.GetLength(1) &&
                    y + yOffset >= 0 && y + yOffset < grid.GetLength(0) &&
                    grid[y + yOffset, x + xOffset].id == 0)
                {

                    x = x + xOffset;
                    y = y + yOffset;

                    grid[y, x].id = id;
                    grid[y, x].dirToPreviousCell = val;
                    grid[y, x].dirX = xOffset / 2;
                    grid[y, x].dirY = yOffset / 2;

                    //меняем промежуточную ячейку
                    grid[y - yOffset / 2, x - xOffset / 2].id = id;
                    grid[y - yOffset / 2, x - xOffset / 2].dirX = xOffset / 2;
                    grid[y - yOffset / 2, x - xOffset / 2].dirY = yOffset / 2;

                    if(id>maxId)
                    {
                        maxId = id;
                        maxX = x;
                        maxY = y;
                    }

                    neighbors = new int[] { 0, 1, 2, 3 };
                    id++;
                    if (random.Next(0, 10) == 0)
                    {
                        var t = new Thread(() => CreateLab((startX: x, startY: y, id, param.color)));
                        t.Start();
                        threads.Add(t);
                    }
                }
                else
                {
                    //удаляем елемент массива по индексу
                    neighbors = neighbors.Where(v => v != neighbors[rand]).ToArray();
                }
            }
        }

        public void SetWay(int fromX, int fromY, int toX, int toY)
        {

            if (fromX >= 0 && fromX < grid.GetLength(1) &&
                fromY >= 0 && fromY < grid.GetLength(0))
            {
                //local result = { val, x, y };
                int prevVal = grid[toY, toX].id;
                int curX = toX;
                int curY = toY;
                int tmpX = 0;
                int breakVal = 0;
                int i = 1;

                while (true)
                {
                    if ((curX == fromX && curY == fromY) || grid[curY, curX].id <= grid[fromY, fromX].id)
                    {
                        break;
                    }

                    tmpX = curX - grid[curY, curX].dirX;
                    curY = curY - grid[curY, curX].dirY;
                    curX = tmpX;

                    grid[curY, curX].isWay = true;

                    breakVal = grid[curY, curX].id;
                    i++;
                }

                /*if(grid[curY,curX].id== grid[fromY, fromX].id) 
    {
                    curX = fromX;
                    curY = fromY;

                    bool loop = true;
				    while(loop)
                    { 

					    for( j=1,#result-1 )
                            { 
						    if(result[j].x==curX &&
						     result[j].y==curY) 
    {
							    breakVal=result[j].val
							    --print('breakVal = '..breakVal)

                                loop=false
							    break
						    }
					    }
					    if(grid[curY,curX].val==world.startVal)
    {
					 	    break
				 	    }					
					    tmpX=curX-grid[curY,curX].dirX
                        curY = curY - grid[curY,curX].dirY

                        curX=tmpX
                        result[i] = { val = grid[curY,curX].val, x = curX, y = curY }

                        i=i+1
				    }
			    }
			    local n = 1
			    while n<=#result  do
				    if(result[n].val<breakVal) 
    {
					    table.remove(result, n)
				    else
					    n=n+1
				    }
			    }
			    return result
		    }
		    return nil;*/
            }
        }
    }
}
