using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using WtfApp.Scenes;
using Microsoft.Xna.Framework.Input.Touch;

namespace WtfApp
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Main : Microsoft.Xna.Framework.Game
    {
        public enum PLATFORM
        {
            WINDOWS, LINUX, MACOSX, ANDROID, IOS
        }

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        public static Rectangle screenBounds { get; private set; }
        public static Vector2 screenCenter { get; private set; }
        private readonly Matrix screenXform;
        public static float screenScale { get; private set; }
        
        public static KeyboardState previousKeyboardState{ get; private set; }
        public static MouseState previousMouseState { get; private set; }
        public static TouchCollection previousTouchCollection { get; private set; }

        public static KeyboardState currentKeyboardState { get; private set; }
        public static MouseState currentMouseState { get; private set; }
        public static TouchCollection currentTouchCollection { get; private set; }
        public static PLATFORM platform { get; private set; }

        private static Scene prevScene;
        private static Scene curScene;
        WTFHelper.SCENES curSceneType
        {
            get { return curScene.sceneType; }
        }

        Color gridClr = new Color(40, 40, 40);
        Color bClr = new Color(30, 30, 30);

#if WINDOWS
        const int sW = 480;
        const int sH = 270;
#endif

#if ANDROID
        const int sW = 1920;
        const int sH = 1080;
#endif

        /*
        const int sW = 640;
        const int sH = 360;
        */

        FrameCounter frameCounter = new FrameCounter();

        public bool isFPS = false;

        public static void GoToScene(WTFHelper.SCENES sceneType, object param=null)
        {
            prevScene = curScene;
            curScene = null;
            switch(sceneType)
            {
#if DEBUG
                case WTFHelper.SCENES.TEST: curScene = new Test(screenBounds); break;
#endif
                case WTFHelper.SCENES.GAME: curScene = new MainG(screenBounds,(SaveLoadLevel)param); break;
                case WTFHelper.SCENES.MAIN_MENU: curScene = new MainMenu(screenBounds); break;
                case WTFHelper.SCENES.LEVEL_EDITOR: curScene = new LevelEditor(screenBounds, (SaveLoadLevel)param); break;
                case WTFHelper.SCENES.LEVEL_SELECTOR: curScene = new LevelSelector(screenBounds); break;
            }
        }

        public Main(PLATFORM platform)
        {

            graphics = new GraphicsDeviceManager(this);
#if WINDOWS
            base.Window.Position = new Point(1100, 150);
            base.Window.IsBorderless = true;
            IsMouseVisible = true;
            graphics.IsFullScreen = false;

#elif ANDROID
            graphics.SupportedOrientations = DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight;
            graphics.IsFullScreen = true;
            Game.Activity.RequestedOrientation = Android.Content.PM.ScreenOrientation.Landscape;
    
#endif
            Main.platform = platform;

            graphics.PreferredBackBufferWidth = sW;
            graphics.PreferredBackBufferHeight = sH;
            //graphics.SynchronizeWithVerticalRetrace = false;
            //this.IsFixedTimeStep = false;
            graphics.ApplyChanges();
            Content.RootDirectory = "Content";

            screenScale = graphics.PreferredBackBufferHeight / 1080.0f;
           // screenScale = graphics.PreferredBackBufferHeight / 270.0f;
            screenXform = Matrix.CreateScale(screenScale, screenScale, 1.0f);
            screenBounds = new Rectangle(0, 0,
                (int)Math.Round(graphics.PreferredBackBufferWidth / screenScale),
                (int)Math.Round(graphics.PreferredBackBufferHeight / screenScale));
            screenCenter = screenBounds.Center.ToVector2();


            string[] names = Enum.GetNames(typeof(Environment.SpecialFolder));
            string[] s = new string[names.Length];

            for(int i=0;i< names.Length;i++)
            {
                s[i] = Environment.GetFolderPath((Environment.SpecialFolder)Enum.Parse(typeof(Environment.SpecialFolder), names[i]));
            }
    }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            //System.IO.Path. = '/';// = ("SHOULD");
            TouchPanel.DisplayWidth = screenBounds.Width;
            TouchPanel.DisplayHeight = screenBounds.Height;

            /*SaveLoadLevel saveLoadLevel = SaveLoadLevel.LoadLevel(1);
            worldObjects = saveLoadLevel.GetNormalizeWorldArray();*/
            //saveLoadLevel.SaveLevel(SaveLoadLevel.GetMaxSavedLvl()+1, 10, 10, worldObjects);
            //WTFHelper.SerializeObject(worldObjects, "testSer.xml");
            // TODO: Add your initialization logic here
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {    
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            DrawHelper.emptyTexture = new Texture2D(GraphicsDevice, 1, 1);
            DrawHelper.emptyTexture.SetData(new[] { Color.White });

            DrawHelper.spriteFont = Content.Load<SpriteFont>("font");
            Texture2D txt2 = Content.Load<Texture2D>("Resources/Images/WorldObjects/EMPTY");
            //Texture2D txt = Content.Load<Texture2D>("Resources/Images/WorldObjects/start");
            WTFHelper.LoadResource(Content);

            previousKeyboardState = Keyboard.GetState();
            previousMouseState = Mouse.GetState();
            previousTouchCollection = TouchPanel.GetState();

            //GoToScene(WTFHelper.SCENES.MAIN_MENU);
            GoToScene(WTFHelper.SCENES.LEVEL_SELECTOR);
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            currentKeyboardState = Keyboard.GetState();
            currentMouseState = Mouse.GetState();
            currentTouchCollection = TouchPanel.GetState();

            //обработка контроля моб. ОС
            if ((Main.platform == Main.PLATFORM.ANDROID || Main.platform == Main.PLATFORM.IOS) && currentTouchCollection.Count > 0)
            {
                foreach (var touch in currentTouchCollection)
                {    
                    switch(touch.State)
                    {
                        case TouchLocationState.Pressed:
                            curScene.Touch(new Point((int)(touch.Position.X / screenScale), (int)(touch.Position.Y / screenScale)), ButtonState.Pressed, false);
                            break;
                        case TouchLocationState.Moved:
                            curScene.Touch(new Point((int)(touch.Position.X / screenScale), (int)(touch.Position.Y / screenScale)), ButtonState.Pressed, true);
                            break;
                        case TouchLocationState.Released:
                            curScene.Touch(new Point((int)(touch.Position.X / screenScale), (int)(touch.Position.Y / screenScale)), ButtonState.Released, false);
                            break;
                    }   
                }
            }
            //обработка контроля настольных ОС
            else if ((Main.platform == Main.PLATFORM.LINUX || Main.platform == Main.PLATFORM.WINDOWS || Main.platform == Main.PLATFORM.MACOSX))
            {
                if (previousMouseState.LeftButton != currentMouseState.LeftButton )
                    curScene.Touch(new Point((int)(currentMouseState.X / screenScale), (int)(currentMouseState.Y / screenScale)), currentMouseState.LeftButton, false);
                else if (currentMouseState.LeftButton == ButtonState.Pressed && !previousMouseState.Position.Equals(currentMouseState.Position))
                    curScene.Touch(new Point((int)(currentMouseState.X / screenScale), (int)(currentMouseState.Y / screenScale)), currentMouseState.LeftButton, true);
            }

            if (currentKeyboardState.IsKeyDown(Keys.P) && !previousKeyboardState.IsKeyDown(Keys.P) && curScene != null)
                curScene.isPause = !curScene.isPause;
            else if (currentKeyboardState.IsKeyDown(Keys.F) && !previousKeyboardState.IsKeyDown(Keys.F))
                isFPS = !isFPS;
            else if (currentKeyboardState.IsKeyDown(Keys.D) && !previousKeyboardState.IsKeyDown(Keys.D) && curScene != null)
                curScene.isDebug = !curScene.isDebug;
                

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || currentKeyboardState.IsKeyDown(Keys.Escape))
                Exit();

            if (curScene != null)
                curScene.Update(gameTime);

            previousMouseState = currentMouseState;
            previousTouchCollection = currentTouchCollection;
            previousKeyboardState = currentKeyboardState;
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {  
            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            frameCounter.Update(deltaTime);
            GraphicsDevice.Clear(bClr);
            spriteBatch.Begin(
                SpriteSortMode.FrontToBack,
                BlendState.AlphaBlend,
                SamplerState.LinearClamp,
                DepthStencilState.None,
                RasterizerState.CullCounterClockwise,
                null,
                screenXform);

            if (curScene!=null)
                curScene.Draw(spriteBatch);
            
            //draw window borders
            DrawHelper.DrawRectagle(spriteBatch, gridClr, 0, 0, screenBounds.Width-1,screenBounds.Height-1, 3,WTFHelper.DRAW_LAYER.DEBUG.F());
            //DrawHelper.DrawRectagle(spriteBatch, gridClr, 0, 0, screenBounds.Width - 1, screenBounds.Height - 1, 3);

           /*//draw toch coordinate
            * foreach (var touch in currentTouchCollection)
            {
                spriteBatch.DrawString(DrawHelper.spriteFont, String.Format("id {0} {1}/{2}", touch.Id,Math.Round(touch.Position.X), Math.Round(touch.Position.Y)), touch.Position + new Vector2(0, -200), Color.FromNonPremultiplied(60, 255, 60, 255));
            }*/
            //spriteBatch.DrawString(DrawHelper.spriteFont, currentTouchCollection.Count.ToString(), new Vector2(1, 1), Color.FromNonPremultiplied(60, 60, 60, 255));

            if (isFPS)  
            {
                var debug = $"FPS: {Math.Round(frameCounter.AverageFramesPerSecond)}"+
                            $"\nScreenScale: {screenScale}"+
                            $"\nStepTimeInMs: {MainG.stepTimeInMs}";
                spriteBatch.DrawString(DrawHelper.spriteFont, debug, new Vector2(1, 1), Color.FromNonPremultiplied(60, 255, 60, 155), 0, Vector2.Zero, Vector2.One,SpriteEffects.None, WTFHelper.DRAW_LAYER.DEBUG.F());
                //spriteBatch.DrawString(spriteFont, isPause.ToString(), new Vector2(1, 20), Color.FromNonPremultiplied(60, 60, 60, 255));
            }
            //debug info      
            spriteBatch.End();
            base.Draw(gameTime);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }
    }
}
