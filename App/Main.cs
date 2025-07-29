using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using WtfApp.Scenes;
using Microsoft.Xna.Framework.Input.Touch;
using WtfApp.Engine.Scene;
using WtfApp.Engine.Scene.Scenes;
using WtfApp.App1;

namespace WtfApp
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class App : Microsoft.Xna.Framework.Game
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

        Color gridClr = new Color(150, 200, 250);
        Color bClr = new Color(30, 150, 255);

#if WINDOWS
        const int sW = 960;
        const int sH = 540;
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

        public static void GoToPrevScene()
        {
            if (prevScene != null)
            {
                curScene = prevScene;
            }
        }

        public static void GoToScene(WTFHelper.SCENES sceneType, Modal.ModalType modalType = (Modal.ModalType)(-1), Rectangle sceneRect = new Rectangle(), object param=null)
        {
            prevScene = curScene;
            curScene = null;
            if (sceneRect.IsEmpty)
                sceneRect = screenBounds;
            
            switch (sceneType)
            {
#if DEBUG
                case WTFHelper.SCENES.TEST1:
                    curScene = new Test1(sceneRect);
                    break;
                case WTFHelper.SCENES.TEST2:
                    curScene = new Test2(sceneRect);
                    break;
                case WTFHelper.SCENES.TEST3:
                    curScene = new Test3(sceneRect);
                    break;
                case WTFHelper.SCENES.MODAL_TEXT:
                    curScene = new Modal(param.ToString(),sceneRect, modalType);
                    break;
                case WTFHelper.SCENES.TEST4:
                    curScene = new Test1(sceneRect);
                    break;
                case WTFHelper.SCENES.TEST5:
                    curScene = new Test1(sceneRect);
                    break;
#endif
                case WTFHelper.SCENES.GAME:
                    curScene = new MainG(sceneRect, (SaveLoadLevel)param);
                    break;
               /* case WTFHelper.SCENES.APP2:
                    curScene = new App2.MainG(sceneRect);
                    break;
                case WTFHelper.SCENES.APP3:
                    curScene = new App3.App3(sceneRect, (App3.SaveLoadLevel)param);
                    break;
                case WTFHelper.SCENES.APP3_LEVEL_EDITOR:
                    curScene = new App3.LevelEditor(sceneRect, (App3.SaveLoadLevel)param);
                    break;
                case WTFHelper.SCENES.APP3_LEVEL_SELECTOR:
                    curScene = new App3.LevelSelector(sceneRect);*/
                    break;
                case WTFHelper.SCENES.MAIN_MENU:
                    curScene = new MainMenu(sceneRect);
                    break;
                case WTFHelper.SCENES.LEVEL_EDITOR:
                    curScene = new LevelEditor(sceneRect, (SaveLoadLevel)param);
                    break;
                case WTFHelper.SCENES.LEVEL_SELECTOR:
                    curScene = new LevelSelector(sceneRect);
                    break;
            }
                curScene.hidePrevious = modalType >= 0 ? false : true;
        }

        public App(PLATFORM platform)
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
            /**/
            Array values = Enum.GetValues(typeof(Environment.SpecialFolder));
            string[] dirs = new string[values.Length];
            for(int i=0;i< values.Length;i++)
            {

                dirs[i] = Environment.GetFolderPath((Environment.SpecialFolder)values.GetValue(i));
            }
            /**/

            App.platform = platform;

            graphics.PreferredBackBufferWidth = sW;
            graphics.PreferredBackBufferHeight = sH;
            //graphics.SynchronizeWithVerticalRetrace = false;
            //this.IsFixedTimeStep = false;
            graphics.ApplyChanges();
            Content.RootDirectory = "Content";

            screenScale = graphics.PreferredBackBufferHeight / 1080.0f;
           // screenScale = graphics.PreferredBackBufferHeight /sH;
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
            TouchPanel.DisplayWidth = screenBounds.Width;
            TouchPanel.DisplayHeight = screenBounds.Height;
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
            DrawHelper.GetTexture(spriteBatch);

            //DrawHelper.spriteFont = Content.Load<SpriteFont>("fontNormalSize");
            DrawHelper.spriteFont = Content.Load<SpriteFont>("font");
            DrawHelper.spriteFontBig = DrawHelper.spriteFont;
            WTFHelper.LoadResource(Content);

            previousKeyboardState = Keyboard.GetState();
            previousMouseState = Mouse.GetState();
            previousTouchCollection = TouchPanel.GetState();

            //GoToScene(WTFHelper.SCENES.APP3_LEVEL_SELECTOR);
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
            /*
             * 
             *
             *
            */
            currentKeyboardState = Keyboard.GetState();
            currentMouseState = Mouse.GetState();
            currentTouchCollection = TouchPanel.GetState();

            //обработка контроля моб. ОС
            if ((App.platform == App.PLATFORM.ANDROID || App.platform == App.PLATFORM.IOS) && currentTouchCollection.Count > 0)
            {
                foreach (var touch in currentTouchCollection)
                {      
                    switch (touch.State)
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
            else if ((App.platform == App.PLATFORM.LINUX || App.platform == App.PLATFORM.WINDOWS || App.platform == App.PLATFORM.MACOSX))
            {
                if (previousMouseState.LeftButton != currentMouseState.LeftButton )
                    curScene.Touch(new Point((int)(currentMouseState.X / screenScale), (int)(currentMouseState.Y / screenScale)), currentMouseState.LeftButton, false);
                else if (currentMouseState.LeftButton == ButtonState.Pressed && !previousMouseState.Position.Equals(currentMouseState.Position))
                    curScene.Touch(new Point((int)(currentMouseState.X / screenScale), (int)(currentMouseState.Y / screenScale)), currentMouseState.LeftButton, true);
            }

            if (currentKeyboardState.IsKeyDown(Keys.P) && !previousKeyboardState.IsKeyDown(Keys.P) && curScene != null)
                curScene.state = curScene.state == Scene.SceneState.Paused ? Scene.SceneState.Running : Scene.SceneState.Paused;
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
                SpriteSortMode.Deferred,
                BlendState.AlphaBlend,
                SamplerState.LinearClamp,
                DepthStencilState.None,
                RasterizerState.CullCounterClockwise,
                null,
                screenXform);

            if (curScene != null)
            {
                if(curScene.hidePrevious==false && prevScene!=null)
                {
                    prevScene.Draw(spriteBatch);
                }
                curScene.Draw(spriteBatch);
            }
            
            //draw window borders
            //DrawHelper.DrawRectagle(spriteBatch, gridClr, 1, 1, screenBounds.Width-2,screenBounds.Height-2, 1,WTFHelper.DRAW_LAYER.ABSOLUTE_FRONT.F());

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
                spriteBatch.DrawString(DrawHelper.spriteFont, debug, new Vector2(1, 1), Color.FromNonPremultiplied(60, 255, 60, 155), 0, Vector2.Zero, Vector2.One,SpriteEffects.None, WTFHelper.DRAW_LAYER.ABSOLUTE_FRONT.F());
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
