using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;

namespace WtfApp
{
    public static class WTFHelper
    {
        public const float textureSize=90;
#if WINDOWS
        public const int alpha = 255;
#elif ANDROID
        public const int alpha = 255;
#endif

        public static Color buttonTextColor = Color.FromNonPremultiplied(10, 10, 10, alpha);
        public static Color buttonDefaultColor = Color.FromNonPremultiplied(80, 80, 80, alpha);
        public static Color buttonDisabledColor = Color.FromNonPremultiplied(50, 50, 50, 255);

        public static Color buttonHoverColor = Color.FromNonPremultiplied(230, 230, 230, 255);
        public static Color buttonPressedColor = Color.FromNonPremultiplied(200, 200, 200, 255);
        public static Color buttonReleassedColor = buttonDefaultColor;

#if WINDOWS
        public static Color objColor = Color.FromNonPremultiplied(255, 255, 255, alpha );
#elif ANDROID
        public static Color objColor = Color.FromNonPremultiplied(255, 255, 255, alpha);
#endif
        public static Random rand = new Random();

        public enum DRAW_LAYER : int { ABSOLUTE_BACK = 1, WORLD_OBJ = 2, EFFECT_OBJ = 3, GENERATED_OBJ = 4, GUI = 5, ABSOLUTE_FRONT=10 };
        public static float F(this DRAW_LAYER layer)
        {
            return (float)layer/100;
        }

        public enum MENU_BUTTON_ALIGN : int { LEFT =1, CENTER=2, RIGHT=3 };
        //порядок важен в обоих энумах
        public enum OBJ_DRAW_DIRECTION : int { TOP = 1, RIGHT = 2, BOTTOM = 3, LEFT = 4 };
        public static int X(this OBJ_DRAW_DIRECTION objDir)
        {
            switch (objDir)
            {
                case WTFHelper.OBJ_DRAW_DIRECTION.RIGHT:
                    return 1;
                case WTFHelper.OBJ_DRAW_DIRECTION.LEFT:
                    return -1;
                default: return 0;
            }
        }
        public static int Y(this OBJ_DRAW_DIRECTION objDir)
        {
            switch (objDir)
            {
                case WTFHelper.OBJ_DRAW_DIRECTION.TOP:
                    return -1;
                case WTFHelper.OBJ_DRAW_DIRECTION.BOTTOM:
                    return 1;
                default: return 0;
            }
        }

        public enum OBJ_MOVE_DIRECTION : int { NONE= 0, TOP = 1, RIGHT = 2, BOTTOM = 3, LEFT = 4 };
        public static int X(this OBJ_MOVE_DIRECTION objDir)
        {
            switch (objDir)
            {
                case WTFHelper.OBJ_MOVE_DIRECTION.RIGHT:
                    return 1;
                case WTFHelper.OBJ_MOVE_DIRECTION.LEFT:
                    return -1;
                default: return 0;
            }
        }
        public static int Y(this OBJ_MOVE_DIRECTION objDir)
        {
            switch (objDir)
            {
                case WTFHelper.OBJ_MOVE_DIRECTION.TOP:
                    return -1;
                case WTFHelper.OBJ_MOVE_DIRECTION.BOTTOM:
                    return 1;
                default: return 0;
            }
        } 

        public enum WORLD_OBJ_TYPE : int { EMPTY, START, FINISH, WALL, PLATFORM, WELDING, ROTATE, ROTATEBACK };
        public static bool isBlocked(this WORLD_OBJ_TYPE objType)
        {
            switch(objType)
            {
                case WORLD_OBJ_TYPE.WALL:
                case WORLD_OBJ_TYPE.WELDING:
                    return true;
                default:
                    return false;
            }
        }

        public enum GENERATED_OBJ_TYPE : int { BOX };

        public enum EFFECT_TYPE : int { WELD };

        public enum CREATURE_TYPE : int { HUMAN };

        public static string[] worldObjNames = Enum.GetNames(typeof(WTFHelper.WORLD_OBJ_TYPE));
        public static string[] generatedObjNames = Enum.GetNames(typeof(WTFHelper.GENERATED_OBJ_TYPE));
        public static string[] effectNames = Enum.GetNames(typeof(WTFHelper.EFFECT_TYPE));
        public enum SCENES : int {MODAL_TEXT, MAIN_MENU, GAME,APP2,APP3, APP3_LEVEL_SELECTOR, APP3_LEVEL_EDITOR, LEVEL_EDITOR, LEVEL_SELECTOR, TEST1 ,TEST2,TEST3,TEST4,TEST5};

        public static Dictionary<string, Texture2D> worldObjTextures;
        public static Dictionary<string, Texture2D> generatedObjTextures;
        public static Dictionary<string, Texture2D> effectsTextures;
        public static Dictionary<string, Texture2D> buttonsTextures;
        public static readonly string[] BLOCKS_NAME = { };

        public static void LoadResource(ContentManager contentManager)
        {
            worldObjTextures = new Dictionary<string, Texture2D>();
            generatedObjTextures = new Dictionary<string, Texture2D>();
            effectsTextures = new Dictionary<string, Texture2D>();
            //buttonsTextures = new Dictionary<string, Texture2D>();

            foreach (var name in Enum.GetNames(typeof(WORLD_OBJ_TYPE)))
            {
                worldObjTextures.Add(name, contentManager.Load<Texture2D>("Resources/Images/WorldObjects/" + name));
            }
            foreach (var name in Enum.GetNames(typeof(GENERATED_OBJ_TYPE)))
            {
                generatedObjTextures.Add(name, contentManager.Load<Texture2D>("Resources/Images/GeneratedObjects/" + name));
            }
            foreach (var name in Enum.GetNames(typeof(EFFECT_TYPE)))
            {
                effectsTextures.Add(name, contentManager.Load<Texture2D>("Resources/Images/Effects/" + name));
            }
            foreach (var name in Enum.GetNames(typeof(CREATURE_TYPE)))
            {
                effectsTextures.Add(name, contentManager.Load<Texture2D>("Resources/Images/Creatures/" + name));
            }

    /*foreach (var name in Enum.GetNames(typeof(EFFECT_TYPE)))
    {
        buttonsTextures.Add(name, contentManager.Load<Texture2D>("Resources/Images/Effects/" + name));
    }*/

}

        //возвращает путь до цели в виде массива поинтов
        public static Point[] WaveAlgoritm(Point worldSize, List<Object> list, Point posFrom, Point posTo)
        {
            int[,] arr = new int[worldSize.X, worldSize.Y];


            for (int i = 0; i < list.Count; i++)
            {
               // arr[list[i].xPos, list[i].yPos] = -1;
            }

            arr[posFrom.X, posFrom.Y] = 1;
            arr[posTo.X, posTo.Y] = 0;

            sbyte rez = 0;
            for (int i = 1; rez == 0; i++)
            {
                rez = -1;
                for (int x = 0; x < worldSize.X; x++)
                {
                    for (int y = 0; y < worldSize.Y; y++)
                    {
                        if (arr[x, y] == i)
                        {

                            //check finish
                            if (((y - 1 == posTo.Y || y + 1 == posTo.Y) && x == posTo.X) ||
                                ((x - 1 == posTo.X || x + 1 == posTo.X) && y == posTo.Y))
                            {
                                arr[posTo.X, posTo.Y] = i + 1;
                                rez = 1;
                                goto FinishLink;
                            }
                            //top
                            if (y > 0 && arr[x, y - 1] == 0)
                            {
                                arr[x, y - 1] = i + 1;
                                rez = 0;
                            }
                            //bot
                            if (y < worldSize.Y - 1 && arr[x, y + 1] == 0)
                            {
                                arr[x, y + 1] = i + 1;
                                rez = 0;
                            }
                            //left
                            if (x > 0 && arr[x - 1, y] == 0)
                            {
                                arr[x - 1, y] = i + 1;
                                rez = 0;
                            }
                            //right
                            if (x < worldSize.X - 1 && arr[x + 1, y] == 0)
                            {
                                arr[x + 1, y] = i + 1;
                                rez = 0;
                            }
                        }
                    }
                }
            }

            if (rez == -1)
                return null;

            FinishLink:

            //заполняем 
            Point[] rezPoints = new Point[arr[posTo.X, posTo.Y]];
            rezPoints[rezPoints.Length - 1] = new Point(posTo.X, posTo.Y);
            for (int i = rezPoints.Length - 1; i > 0; i--)
            {
                if (rezPoints[i].Y > 0 && arr[rezPoints[i].X, rezPoints[i].Y - 1] == i)
                    rezPoints[i - 1] = new Point(rezPoints[i].X, rezPoints[i].Y - 1);
                else if (rezPoints[i].Y < worldSize.Y - 1 && arr[rezPoints[i].X, rezPoints[i].Y + 1] == i)
                    rezPoints[i - 1] = new Point(rezPoints[i].X, rezPoints[i].Y + 1);
                else if (rezPoints[i].X > 0 && arr[rezPoints[i].X - 1, rezPoints[i].Y] == i)
                    rezPoints[i - 1] = new Point(rezPoints[i].X - 1, rezPoints[i].Y);
                else if (rezPoints[i].X < worldSize.X - 1 && arr[rezPoints[i].X + 1, rezPoints[i].Y] == i)
                    rezPoints[i - 1] = new Point(rezPoints[i].X + 1, rezPoints[i].Y);
            }

            /*   string s = "";
               for (int i = 0;i<rezPoints.Length;i++)
               {
                   s += rezPoints[i].X.ToString() + " " + rezPoints[i].Y.ToString() + "\n";
               }
               MessageBox.Show(s);*/


            return rezPoints;
        }

        /*public static Dictionary<String, T> LoadContent<T>(ContentManager contentManager, Type enumType)
        {
            string contentFolder = "";
           
            DirectoryInfo dir = new DirectoryInfo(Path.Combine(contentManager.RootDirectory, contentFolder));
#if WINDOWS
            if (!dir.Exists)
                throw new DirectoryNotFoundException();
#endif

            Dictionary<String, T> result = new Dictionary<String, T>();

            
            FileInfo[] files = dir.GetFiles("*.*");
            foreach (FileInfo file in files)
            {
                string key = Path.GetFileNameWithoutExtension(file.Name).ToUpper();
                result[key] = contentManager.Load<T>(Path.Combine(contentFolder , key));
            }

            return result;
        }*/
    }
}