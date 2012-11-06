using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework.Graphics;
//using Editor.ParticleSystem;
using Microsoft.Xna.Framework.Input;
using FarseerPhysics;
using Editor.Elements;
using FarseerPhysics.DebugViews;
using IrrKlang;
using Editor;
using Editor.Menus;
using Microsoft.Kinect;


namespace Editor
{
    public class Scene : DrawableGameComponent
    {
        public Random Random = new Random();
        public SpriteBatch SpriteBatch;
        public InputManager InputManager;
        KinectSensor Kinect;

#if !FINAL_RELEASE
        public SelectionManager SelectionManager;
#endif
        public Transitioner Transitioner;
        public SceneLoader SceneLoader;
        public Camera Camera;
        public World World;
        public DebugViewXNA PhysicsDebug;
        public SortedList<Element> GarbageElements;
        public SortedList<Element> RespawnElements;
        public SortedList<Element> Elements;


        RenderTarget2D sceneTarget;
        RenderTarget2D renderTarget1;
        RenderTarget2D renderTarget2;


        Texture2D blank;
        //SpriteFont debugfont;

        public Scene(Game game)
            : base(game)
        {

            World = new World(new Vector2(0, 18));
            PhysicsDebug = new DebugViewXNA(World);
            InputManager = new InputManager(Game);
            Transitioner = new Transitioner(Game, this);
#if !FINAL_RELEASE
            SelectionManager = new SelectionManager(Game, this);
#endif
            SceneLoader = new SceneLoader(Game);
            Camera = new Camera(Game, this);
            GarbageElements = new SortedList<Element>();
            RespawnElements = new SortedList<Element>();
            Elements = new SortedList<Element>();

            PhysicsDebug.Enabled = false;
            SelectionManager.ShowEmblems = false;
            Kinect.ColorStream.Enable();
            Kinect.DepthStream.Enable();
            Kinect.SkeletonStream.Enable();
            Kinect.Start();
            
            //SelectionManager.ShowForm = false;
        }

        public override void Initialize()
        {
            base.Initialize();

#if !FINAL_RELEASE
            try
            {
                string text = System.IO.File.ReadAllText(@"Content\levels\lastlevel");
                CleanAndLoad(text);
            }
            catch (Exception)
            {
                CleanAndLoad("level1");
            }
#endif
        }

        protected override void LoadContent()
        {
            SpriteBatch = new SpriteBatch(GraphicsDevice);

            PhysicsDebug.LoadContent(GraphicsDevice, Game.Content);
            PhysicsDebug.AppendFlags(DebugViewFlags.Shape);
            PhysicsDebug.AppendFlags(DebugViewFlags.PolygonPoints);
            PhysicsDebug.AppendFlags(DebugViewFlags.CenterOfMass);

            GraphicsDevice.SamplerStates[1] = SamplerState.LinearClamp;

            sceneTarget = new RenderTarget2D(GraphicsDevice, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, false, SurfaceFormat.Color, DepthFormat.Depth24);
            renderTarget1 = new RenderTarget2D(GraphicsDevice, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, false, SurfaceFormat.Color, DepthFormat.None);
            renderTarget2 = new RenderTarget2D(GraphicsDevice, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, false, SurfaceFormat.Color, DepthFormat.None);

            blank = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            blank.SetData(new[] { Color.White });
            //debugfont = Game.Content.Load<SpriteFont>("fonts\\debugfont");

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            if (sceneTarget.Width != GraphicsDevice.Viewport.Width || sceneTarget.Height != GraphicsDevice.Viewport.Height)
            {
                sceneTarget = new RenderTarget2D(GraphicsDevice, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, false, SurfaceFormat.Color, DepthFormat.Depth24);
                renderTarget1 = new RenderTarget2D(GraphicsDevice, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, false, SurfaceFormat.Color, DepthFormat.None);
                renderTarget2 = new RenderTarget2D(GraphicsDevice, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, false, SurfaceFormat.Color, DepthFormat.None);
            }
#if !FINAL_RELEASE
            SelectionManager.Update(gameTime);
#endif
            //Menu TODO
                foreach (Element i in GarbageElements)
                {
                    Elements.Remove(i);
                    i.Dispose();
                }
                GarbageElements.Clear();

            
                World.Step((float)gameTime.ElapsedGameTime.TotalSeconds);
                Camera.Update(gameTime);
                InputManager.Update(gameTime);

                foreach (Element i in Elements)
                    i.Update(gameTime);

                foreach (Element i in RespawnElements)
                    Elements.Add(i);
                RespawnElements.Clear();
            

#if !FINAL_RELEASE
            SceneLoader.Update(gameTime, this);
#endif

            if (cleanAndLoad && !Transitioner.InTransition)
            {
                Camera.ResetScale();
                Clean();
                Load(levelName, characterPosition, characterActive);
                cleanAndLoad = false;
                levelName = "";
                characterPosition = null;
                Transitioner.Delay = 1;
                Transitioner.AlphaTarget = 0;
            }

            Transitioner.Update(gameTime);
        }

        private bool cleanAndLoad = false;
        private String levelName = "";
        private Vector2? characterPosition = null;
        private bool? characterActive = null;

        public void CleanAndLoad(String levelName, Vector2? characterPosition = null, bool? characterActive = null)
        {
            cleanAndLoad = true;
            this.levelName = levelName;
            this.characterPosition = characterPosition;
            this.characterActive = characterActive;
            Transitioner.AlphaTarget = 1;
        }

        public void Load(String levelName, Vector2? characterPosition = null, bool? characterActive = null)
        {
            Camera.Target = null;
            SceneLoader.SceneFromXml(@"Content\levels\" + levelName + ".xml", this);
            if (Camera.Target != null)
            {
                if (characterPosition != null)
                {
                    Camera.Target.Position = (Vector2)characterPosition;
                    ((Character)Camera.Target).Active = (bool)characterActive;
                }
                Camera.Position = Camera.Target.Position;
            }

            this.levelName = "";
            this.characterPosition = null;
            this.characterActive = null;
        }

        public void Clean()
        {

            foreach (Element i in Elements)
                i.Dispose();
            foreach (Element i in RespawnElements)
                i.Dispose();
            Elements.Clear();
            RespawnElements.Clear();
            GarbageElements.Clear();
            Camera.Target = null;
            InputManager.Target = null;
#if !FINAL_RELEASE
            SelectionManager.Selection = null;
#endif
            World.Clear();
            //Soundmanager TODO
        }

        int frames = 0;
        double seconds = 0;
        double fps = 0;

        public override void Draw(GameTime gameTime)
        {
            frames++;
            seconds += gameTime.ElapsedGameTime.TotalSeconds;
            if (seconds > 1)
            {
                fps = frames / seconds;
                seconds = 0;
                frames = 0;
            }

            
                drawScene(gameTime);
            

            Transitioner.Draw(gameTime);

#if !FINAL_RELEASE
            SelectionManager.Draw(gameTime);
            PhysicsDebug.RenderDebugData(ref Camera.Projection, ref Camera.View);
            if (PhysicsDebug.Enabled)
            {
                SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                SpriteBatch.End();

                SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                SpriteBatch.End();
            }
#endif

            
        }

        public void drawScene(GameTime gameTime)
        {
            //blah
            GraphicsDevice.Clear(Color.White);
            SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            
            foreach (Element i in Elements)
                i.Draw(gameTime);
            SpriteBatch.End();

            SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            
            SpriteBatch.End();
        }

        ///// <summary>
        ///// Evaluates a single point on the gaussian falloff curve.
        ///// Used for setting up the blur filter weightings.
        ///// </summary>
        //float ComputeGaussian(float n)
        //{
        //    float theta = 5;

        //    return (float)((1.0 / Math.Sqrt(2 * Math.PI * theta)) *
        //                   Math.Exp(-(n * n) / (2 * theta * theta)));
        //}
    }
}
