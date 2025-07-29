using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace WtfApp.App3
{
    public class SaveLoadLevel
    {
        
        //for serialize
        public struct SER_WorldObj
        {
            public int x;
            public int y;
            public WTFHelper.OBJ_DRAW_DIRECTION dir;
            public WTFHelper.WORLD_OBJ_TYPE type;
          //  public  variableParam;

            public SER_WorldObj(int x,int y, WTFHelper.OBJ_DRAW_DIRECTION direction, WTFHelper.WORLD_OBJ_TYPE type)
            {
                this.x = x;
                this.y = y;
                this.dir = direction;
                this.type = type;
               
            }
        }

#if WINDOWS && DEBUG
            private static readonly string levelDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalizedResources)+"../../../../../App/Content/Levels/APP3";
#elif WINDOWS
            private static readonly string levelDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalizedResources);
#elif ANDROID
        private static readonly string levelDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
#endif
        public int levelId;
        public int levelSizeX;
        public int levelSizeY;
        private List<SER_WorldObj> _worldObjects;
        public SER_WorldObj[] worldObjects;
        public WTFHelper.WORLD_OBJ_TYPE[] availableUserWorldObj;

        public WorldObj[,] GetNormalizeWorldArray()
        {
            WorldObj[,] rezult = new WorldObj[levelSizeY, levelSizeX];

            for(int i=0;i<worldObjects.Length;i++)
            { 
                rezult[worldObjects[i].y, worldObjects[i].x] = new WorldObj(worldObjects[i].type, worldObjects[i].x, worldObjects[i].y, worldObjects[i].dir);
            }

            return rezult;
        }
        public void SaveLevel(int levelId, int worldSizeX, int worldSizeY, WorldObj[,] worldObjects, List<WTFHelper.WORLD_OBJ_TYPE> availableUserWorldObj)
        {
            this.levelId = levelId;
            this.levelSizeX = worldSizeX;
            this.levelSizeY = worldSizeY;
            this.availableUserWorldObj = availableUserWorldObj.ToArray();
            this._worldObjects = new List<SER_WorldObj>();

            for (int y = 0; y < worldSizeY; y++)
            {
                for (int x = 0; x < worldSizeX; x++)
                {
                    if (worldObjects[y, x] != null)
                    {
                        this._worldObjects.Add(new SER_WorldObj(worldObjects[y, x].xPos, worldObjects[y, x].yPos, worldObjects[y, x].objDrawDir, worldObjects[y, x].objType));
                    }
                }
            }
            this.worldObjects = new SER_WorldObj[_worldObjects.Count];
            _worldObjects.CopyTo(this.worldObjects);

            SerializeObject(this, levelId.ToString());
        }
        public static SaveLoadLevel LoadLevel(int levelId)
        {
            string fileName = levelId.ToString();
            string file = string.IsNullOrEmpty(levelDir) ? fileName : Path.DirectorySeparatorChar.ToString() + fileName;
            if (!File.Exists(levelDir+file))
                throw new Exception("FILE LVL NOT EXISTS:\n" + levelDir+file);

            return DeSerializeObject<SaveLoadLevel>(levelId.ToString());
            /*  this.levelId = levelId;
              this.levelSizeX = worldSizeX;
              this.levelSizeY = worldSizeY;
              this.worldObjects = new WorldObj[worldSizeX * worldSizeY];

              for (int y = 0, index = 0; y < worldSizeY; y++)
              {
                  for (int x = 0; x < worldSizeX; x++)
                  {
                      this.worldObjects[index] = worldObjects[y, x];
                      index++;
                  }
              }
              SerializeObject(this, levelId.ToString());*/
        }

        /// <summary>
        /// Serializes an object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serializableObject"></param>
        /// <param name="fileName"></param>
        public void SerializeObject<T>(T serializableObject, string fileName)
        {
            if (serializableObject == null) { return; }
            try
            {
                string file = string.IsNullOrEmpty(levelDir) ? fileName : Path.DirectorySeparatorChar.ToString() + fileName;
                XmlDocument xmlDocument = new XmlDocument();
                XmlSerializer serializer = new XmlSerializer(serializableObject.GetType());
                using (MemoryStream stream = new MemoryStream())
                {
                    serializer.Serialize(stream, serializableObject);
                    stream.Position = 0;
                    xmlDocument.Load(stream);
                    xmlDocument.Save(levelDir+file);
                }
            }
            catch (Exception ex)
            {
                //Log exception here
            }
        }
        /// <summary>
        /// Deserializes an xml file into an object list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static T DeSerializeObject<T>(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) { return default(T); }
            T objectOut = default(T);
            try
            {
                string file = string.IsNullOrEmpty(levelDir) ? fileName : Path.DirectorySeparatorChar.ToString() + fileName;
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(levelDir+file);
                string xmlString = xmlDocument.OuterXml;

                using (StringReader read = new StringReader(xmlString))
                {
                    Type outType = typeof(T);

                    XmlSerializer serializer = new XmlSerializer(outType);
                    using (XmlReader reader = new XmlTextReader(read))
                    {
                        objectOut = (T)serializer.Deserialize(reader);
                    }
                }
            }
            catch (Exception ex)
            {
                //Log exception here
            }

            return objectOut;
        }
        public static int GetMaxSavedLvl()
        {
            string file = string.IsNullOrEmpty(levelDir) ? "" : Path.DirectorySeparatorChar.ToString();
            for (int i = 1; true; i++)
            {
                if (!File.Exists(levelDir+file + i.ToString()))
                {
                    return i;
                }
            }
        }
        public static bool IsLevelExists(int levelId)
        {
            string file = string.IsNullOrEmpty(levelDir) ? "" : Path.DirectorySeparatorChar.ToString();
            if (File.Exists(levelDir + file + levelId.ToString()))
                return true;
            else
                return false;
        }
    }
}
