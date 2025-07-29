using System;
using System.Collections.Generic;
using System.Text;
using WtfApp.GUI;
using WtfApp.Classes;
using WtfApp.Engine.Scene;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System.Diagnostics;

namespace WtfApp.App2
{
    class MainG: Scene
    {
        public static int worldSizeX = 45;
        public static int worldSizeY = 30;
        public static int worldObjSize = 9;
        public static double stepTimeInMs = 2000;
        public static double stepCountToNewBlock = 2;
        public double stepCurrentTime = 0.0;
        public double currentStepCountToNewBlock = 0;
        public double stepProgress = 0.0;
        public double curUpdateProgress = 0.0;
        public static bool isPaused = false;
        public bool isStoped = false; //change to TRUE
        public static int score = 0;

        public World world;

        //debug
        double avgSWTime = 0;

        public MainG(Rectangle sceneRect):  base(WTFHelper.SCENES.APP2,sceneRect)
        {
            world = new World(new Point(worldSizeX, worldSizeY));
            world.CreateWorld();
           // isDebug = true;
          /*Objects.Air air = new Objects.Air();
            air.CreateNormalAir();
            air.IncreaseGasConcetrationWithOtherPercentDecrease(Objects.GasType.Oxygen, 0.001f);*/
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
          //  spriteBatch.DrawGrid(new Rectangle(1, 1, worldObjSize*worldSizeX, worldObjSize*worldSizeY), new Point(worldSizeX, worldSizeY), Color.FromNonPremultiplied(255, 255, 255, WTFHelper.alpha), 1);
            world.DrawWorld(spriteBatch,new Point( worldObjSize,worldObjSize));
            //world.DrawAir(spriteBatch, new Point(worldObjSize, worldObjSize));
            
            base.Draw(spriteBatch);
        }

        public override void Update(GameTime gameTime)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            world.AirAirInteractionOptimize();
           // world.AirAirInteraction();
            sw.Stop();
            avgSWTime += sw.Elapsed.TotalMilliseconds/60;
            if (gameTime.TotalGameTime.TotalMilliseconds % 1000 < 1)
            {
                debugStr = $"AirUp time: {Math.Round(sw.Elapsed.TotalMilliseconds, 2).ToString("0.00")}";
                debugStr += $"\nAvgSW time: {(avgSWTime).ToString("0.00")}";
                debugStr += $"\nAvgAir: {world.GetAirSum()}";
                avgSWTime = 0;  
            }
            base.Update(gameTime);
        }
    }
}
