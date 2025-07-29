using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WtfApp.App2.Objects;

namespace WtfApp.App2
{
    enum Biom
    {
        Desert,
        Normal,
    }

    class World
    {
        private WorldObj[,] world;
        public readonly Point worldSize;

        public World(Point worldSize)
        {
            this.worldSize = worldSize;
        }

        public void CreateWorld()
        {
           
            float surfaceWaterChanse = 0.3f;
            float stoneChanse = 0.1f;
            float sandChanse = 0.05f;

            world = new WorldObj[worldSize.Y, worldSize.X];
            for (int y = 0; y < worldSize.Y; y++)
            {
                for (int x = 0; x < worldSize.X; x++)
                {
                    world[y, x] = new WorldObj();
                    world[y, x].CreateObj();

                    switch(WTFHelper.rand.NextDouble())
                    {
                        case double n when (n < sandChanse): world[y, x].ground = new Ground(Ground.GroundType.Sand); break;
                        case double n when (n < stoneChanse): world[y, x].ground = new Ground(Ground.GroundType.Stone); break;
                        default: world[y, x].ground = new Ground(Ground.GroundType.Earth); break;
                    }
                }
            }


        }
        public void DrawWorld(SpriteBatch spriteBatch, Point cellSize)
        {
            for (int y = 0; y < worldSize.Y; y++)
            {
                for (int x = 0; x < worldSize.X; x++)
                {
                    switch(world[y,x].ground.groundType)
                    {
                        case Ground.GroundType.Sand:
                            spriteBatch.Draw(DrawHelper.GetTexture(),
                                new Rectangle(cellSize.X * x, cellSize.Y * y, cellSize.X, cellSize.Y),
                                Color.FromNonPremultiplied(255, 255, 0, WTFHelper.alpha));
                            break;
                        case Ground.GroundType.Stone:
                            spriteBatch.Draw(DrawHelper.GetTexture(),
                                new Rectangle(cellSize.X * x, cellSize.Y * y, cellSize.X, cellSize.Y),
                                Color.FromNonPremultiplied(200, 200, 200, WTFHelper.alpha));
                            break;
                        case Ground.GroundType.Earth:
                            spriteBatch.Draw(DrawHelper.GetTexture(),
                                new Rectangle(cellSize.X * x, cellSize.Y * y, cellSize.X, cellSize.Y),
                                Color.FromNonPremultiplied(100, 100, 50, WTFHelper.alpha));
                            break;
                    }
                }
            }
        }

        public void DrawAir(SpriteBatch spriteBatch,Point cellSize)
        {
            for (int y = 0; y < worldSize.Y; y++)
            {
                for (int x = 0; x < worldSize.X; x++)
                {
                    spriteBatch.Draw(DrawHelper.GetTexture(),
                        new Rectangle(cellSize.X*x, cellSize.Y*y,cellSize.X,cellSize.Y),
                        Color.FromNonPremultiplied((int)(world[y, x].air.gases[(byte)GasType.CarbonDioxide] * 1000), (int)(world[y,x].air.gases[(byte)GasType.Oxygen]*1000),0,WTFHelper.alpha));
                }
            }
        }

        public void AirAirInteraction()
        {
            for (int y =0;y<worldSize.Y;y++)
            {
                for(int x =0; x<worldSize.X;x++)
                {
                    if (y == worldSize.Y - 1 || x == worldSize.X - 1)
                    {
                        if (y == worldSize.Y - 1 && x == worldSize.X - 1)
                        {
                            //тогда нихуя не делаем
                        }
                        else if (y == worldSize.Y - 1)
                        {
                            FAir.AirInterchange(world[y, x].air, world[y, x + 1].air);
                        }
                        else if (x == worldSize.X - 1)
                        {
                            FAir.AirInterchange(world[y, x].air, world[y + 1, x].air);
                        }
                    }
                    else
                        FAir.AirInterchange(world[y, x].air, world[y + 1, x].air, world[y, x + 1].air);
                }
            }
        }
        public void AirAirInteractionOptimize()
        {
            for (int y = 0; y < worldSize.Y - 1; y++)
            {
                FAir.AirInterchange(world[y, worldSize.X - 1].air, world[y + 1, worldSize.X - 1].air);
            }
            for (int x = 0; x < worldSize.X - 1; x++)
            {
                FAir.AirInterchange(world[worldSize.Y - 1, x].air, world[worldSize.Y - 1, x + 1].air);
            }

            for (int y = 0; y < worldSize.Y-1; y++)
            {
                for (int x = 0; x < worldSize.X-1; x++)
                {
                    FAir.AirInterchange(world[y, x].air, world[y + 1, x].air, world[y, x + 1].air);
                }
            }
        }
        public float GetAirSum()
        {
            float res = 0;
            for (int y = 0; y < worldSize.Y ; y++)
            {
                for (int x = 0; x < worldSize.X ; x++)
                {
                    for (int i = 0; i < world[y, x].air.gases.Length; i++)
                    {
                        res += world[y, x].air.gases[i];
                    }
                }
            }
            res = res / (worldSize.X * worldSize.Y);
            return res;
        }
    }
}
