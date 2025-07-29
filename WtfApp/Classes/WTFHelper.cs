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
        public const int alpha = 25;
#elif ANDROID
        public const int alpha = 255;
#endif

        public static Color buttonTextColor = Color.FromNonPremultiplied(10, 10, 10, alpha);
        public static Color buttonDefaultColor = Color.FromNonPremultiplied(80, 80, 80, alpha);
        public static Color buttonDisabledColor = Color.FromNonPremultiplied(50, 50, 50, alpha);

        public static Color buttonHoverColor = Color.FromNonPremultiplied(230, 230, 230, alpha);
        public static Color buttonPressedColor = Color.FromNonPremultiplied(200, 200, 200, alpha);
        public static Color buttonReleassedColor = buttonDefaultColor;


#if WINDOWS
        public static Color objColor = Color.FromNonPremultiplied(255, 255, 255, alpha / 2);
#elif ANDROID
        public static Color objColor = Color.FromNonPremultiplied(255, 255, 255, alpha);
#endif


        public enum DRAW_LAYER : int { BACKGROUND = 1, WORLD_OBJ = 2, EFFECT_OBJ = 3, GENERATED_OBJ = 4, GUI = 5, DEBUG=10 };
        public static float F(this DRAW_LAYER layer)
        {
            return (float)layer/10;
        }

        public enum MENU_BUTTON_ALIGN : int { LEFT, CENTER, RIGHT };
        //порядок важен в обоих энумах
        public enum OBJ_DRAW_DIRECTION : int {          TOP = 1, RIGHT = 2, BOTTOM = 3, LEFT = 4 };
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

        public static string[] worldObjNames = Enum.GetNames(typeof(WTFHelper.WORLD_OBJ_TYPE));
        public static string[] generatedObjNames = Enum.GetNames(typeof(WTFHelper.GENERATED_OBJ_TYPE));
        public static string[] effectNames = Enum.GetNames(typeof(WTFHelper.EFFECT_TYPE));
        public enum SCENES : int { TEST,MAIN_MENU, GAME, LEVEL_EDITOR, LEVEL_SELECTOR};

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

            /*foreach (var name in Enum.GetNames(typeof(EFFECT_TYPE)))
            {
                buttonsTextures.Add(name, contentManager.Load<Texture2D>("Resources/Images/Effects/" + name));
            }*/

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