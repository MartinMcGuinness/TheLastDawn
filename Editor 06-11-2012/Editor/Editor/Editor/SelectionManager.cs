using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Editor.Elements;
using Microsoft.Xna.Framework.Graphics;

namespace Editor
{
    public class SelectionManager : DrawableGameComponent
    {
        SpriteFont font;
        Texture2D elementEmblem;
        Texture2D selectionEmblem;
        Texture2D backgroundEmblem;
        Texture2D foregroundEmblem;
        public static bool ShowEmblems = true;

        private Editor1.FormProperties form;
        private Scene scene;
        private Element selection = null;
        public Element Selection
        {
            get
            {
                return selection;
            }
            set
            {
                selection = value;
                form.Selection = selection;
            }
        }

        public SelectionManager(Game game, Scene scene)
            : base(game)
        {
            this.scene = scene;

            //font = Game.Content.Load<SpriteFont>("fonts\\debugfont");
            //elementEmblem = Game.Content.Load<Texture2D>("icons\\elementemblem");
            //selectionEmblem = Game.Content.Load<Texture2D>("icons\\selectionemblem");
            //backgroundEmblem = Game.Content.Load<Texture2D>("icons\\backgroundemblem");
            //foregroundEmblem = Game.Content.Load<Texture2D>("icons\\foregroundemblem");

            System.Windows.Forms.Application.EnableVisualStyles();
            this.form = new Editor1.FormProperties(Game, scene);
            this.form.Show();

            Initialize();
        }

        MouseState previous;
        public override void Update(GameTime gameTime)
        {
            if (Selection != null && IsFocusOnWindow())
            {
                float speed = Conversion.ToWorld(1);
                if (Keyboard.GetState().IsKeyDown(Keys.LeftShift))
                    speed = Conversion.ToWorld(20);

                if (Keyboard.GetState().IsKeyDown(Keys.OemBackslash))
                    speed = Conversion.ToWorld(0.02f);

                if (Keyboard.GetState().IsKeyDown(Keys.W))
                    Selection.Position = Selection.Position - Vector2.UnitY * speed;
                if (Keyboard.GetState().IsKeyDown(Keys.S))
                    Selection.Position = Selection.Position + Vector2.UnitY * speed;
                if (Keyboard.GetState().IsKeyDown(Keys.A))
                    Selection.Position = Selection.Position - Vector2.UnitX * speed;
                if (Keyboard.GetState().IsKeyDown(Keys.D))
                    Selection.Position = Selection.Position + Vector2.UnitX * speed;

                if (Keyboard.GetState().IsKeyDown(Keys.Add) && Keyboard.GetState().IsKeyDown(Keys.LeftControl))
                    Selection.Height = Selection.Height + speed;
                else if (Keyboard.GetState().IsKeyDown(Keys.Add))
                    Selection.Width = Selection.Width + speed;
                if (Keyboard.GetState().IsKeyDown(Keys.Subtract) && Keyboard.GetState().IsKeyDown(Keys.LeftControl))
                    Selection.Height = Math.Max(0.001f, Selection.Height - speed);
                else if (Keyboard.GetState().IsKeyDown(Keys.Subtract))
                    Selection.Width = Math.Max(0.001f, Selection.Width - speed);
                if (Keyboard.GetState().IsKeyDown(Keys.Delete) || Keyboard.GetState().IsKeyDown(Keys.Back))
                {
                    selection.Dispose();
                    Selection = null;
                }

                if (Keyboard.GetState().IsKeyDown(Keys.W) || Keyboard.GetState().IsKeyDown(Keys.S) || Keyboard.GetState().IsKeyDown(Keys.A) || Keyboard.GetState().IsKeyDown(Keys.D) ||
                    Keyboard.GetState().IsKeyDown(Keys.Add) || Keyboard.GetState().IsKeyDown(Keys.Subtract))
                {
                    form.Selection = selection;
                }
            }

            if (Mouse.GetState().LeftButton == ButtonState.Pressed && previous.LeftButton == ButtonState.Released && IsMouseInWindow(Mouse.GetState()))
            {
                Console.WriteLine(scene.Camera.ScreenToWorld(Mouse.GetState()));
                Element newSelection = null;
                List<Element> elements = new List<Element>();
                elements.AddRange(scene.Elements);
                foreach (Element i in elements)
                {
                    if (Vector2.Distance(scene.Camera.WorldToScreen(i.Position), new Vector2(Mouse.GetState().X, Mouse.GetState().Y)) < 8)
                    {
                        newSelection = i;
                        Console.WriteLine("Selected one at " + i.Position + ", Width " + i.Width + ", Height " + i.Height);
                        break;
                    }
                }
                if (newSelection != Selection)
                    Selection = newSelection;
                else
                    Selection = null;
            }

            if (Mouse.GetState().RightButton == ButtonState.Pressed && previous.RightButton == ButtonState.Released)
            {
                Element element = null;
                switch (form.NewElementType)
                {
                    case "Platform":
                        element = new Platform(Game, scene, scene.Camera.ScreenToWorld(previous), Vector2.One);
                        break;
                    case "Bridge":
                        element = new Bridge(Game, scene, scene.Camera.ScreenToWorld(previous), Vector2.One);
                        break;
                    case "Ladder":
                        element = new Ladder(Game, scene, 1, scene.Camera.ScreenToWorld(previous));
                        break;
                    case "Character":
                        element = new Character(Game, scene, scene.Camera.ScreenToWorld(previous));
                        scene.Camera.Target = element;
                        //scene.InputManager.Target = (Character)element;
                        break;
                }
                if (element != null)
                    scene.Elements.Add(element);

            }

            previous = Mouse.GetState();
        }

        public override void Draw(GameTime gameTime)
        {
            if (ShowEmblems)
            {
                scene.SpriteBatch.Begin();

                foreach (Element i in scene.Elements)
                    scene.SpriteBatch.Draw(elementEmblem, scene.Camera.Scale * Conversion.ToDisplay(i.Position - scene.Camera.Position), null, Color.White, 0, new Vector2(backgroundEmblem.Width / 2, backgroundEmblem.Height / 2), 1.0f, SpriteEffects.None, 0);

                if (selection != null)
                {
                    String selectionName = selection.GetType().Name;
                    Vector2 selectionPosition = scene.Camera.WorldToScreen(selection.Position) - font.MeasureString(selectionName) / 2 + new Vector2(0, -20);
                    scene.SpriteBatch.DrawString(font, selectionName, selectionPosition + Vector2.UnitX, Color.Black);
                    scene.SpriteBatch.DrawString(font, selectionName, selectionPosition - Vector2.UnitX, Color.Black);
                    scene.SpriteBatch.DrawString(font, selectionName, selectionPosition + Vector2.UnitY, Color.Black);
                    scene.SpriteBatch.DrawString(font, selectionName, selectionPosition - Vector2.UnitY, Color.Black);
                    scene.SpriteBatch.DrawString(font, selectionName, selectionPosition, Color.White);

                    scene.SpriteBatch.Draw(selectionEmblem, scene.Camera.WorldToScreen(selection.Position), null, Color.White, 0, new Vector2(backgroundEmblem.Width / 2, backgroundEmblem.Height / 2), 1.0f, SpriteEffects.None, 0);
                }
                scene.SpriteBatch.End();
            }
        }

        public bool IsMouseInWindow(MouseState mouseState)
        {
            return Game.IsActive &&
                mouseState.X >= 0 && mouseState.X < GraphicsDevice.Viewport.Width &&
                mouseState.Y >= 0 && mouseState.Y < GraphicsDevice.Viewport.Height &&
                System.Windows.Forms.Form.ActiveForm != null &&
                System.Windows.Forms.Form.ActiveForm.Text.Equals(Game.Window.Title);
        }

        public bool IsFocusOnWindow()
        {
            return Game.IsActive &&
                System.Windows.Forms.Form.ActiveForm != null &&
                System.Windows.Forms.Form.ActiveForm.Text.Equals(Game.Window.Title);
        }
    }
}
