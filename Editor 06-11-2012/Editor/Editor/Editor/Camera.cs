using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Editor.Elements;

namespace Editor
{
    public class Camera : DrawableGameComponent
    {
        private Scene scene;

        public Element Target;
        public Vector2 Position;
        public Matrix View;
        public Matrix ViewNonScaled;
        public Matrix Projection;
        public static float DefaultScale = 1.0f;
        private float scale = DefaultScale;
        private float resolutionScale = 1;
        public float Scale
        {
            get { return scale * resolutionScale; }
        }
        public float ScaleTarget = DefaultScale;
        public float ScaleDuration = 5;
        public Vector2 ListenerPosition;

        public void ResetScale()
        {
            scale = ScaleTarget = DefaultScale;
        }
        public bool Grabbing = false;
        public Vector2 GrabbingPosition = Vector2.Zero;

        float goingRight = 0, goingUp = 0, goingLeft = 0, goingDown = 0;
        Vector2 previousVelocity = Vector2.Zero;
        float increment = 0.2f;
        float maxshift = 6;
        static float delay = 0.1f;
        float counter = delay;
        float threshold = 2;

        public Camera(Game game, Scene scene)
            : base(game)
        {
            this.scene = scene;
            Initialize();
            Position = Vector2.Zero;
        }

        MouseState previousMouseState;
        public override void Update(GameTime gameTime)
        {
            MouseState currentMouseState = Mouse.GetState();

            resolutionScale = (GraphicsDevice.Viewport.Width) / (1920.0f);
            if (scale != ScaleTarget)
            {
                Vector2 screenPosition = ScreenToWorld(GraphicsDevice.Viewport.Width / 2, (int)(GraphicsDevice.Viewport.Height / 1.5f));

                if (scale < ScaleTarget)
                    scale = (float)Math.Min(ScaleTarget, scale + gameTime.ElapsedGameTime.TotalSeconds / ScaleDuration);
                else
                    scale = (float)Math.Max(ScaleTarget, scale - gameTime.ElapsedGameTime.TotalSeconds / ScaleDuration);

                Position += screenPosition - ScreenToWorld(GraphicsDevice.Viewport.Width / 2, (int)(GraphicsDevice.Viewport.Height / 1.5f));
            }

#if !FINAL_RELEASE
            if (currentMouseState.ScrollWheelValue - previousMouseState.ScrollWheelValue > 0)
            {
                Vector2 screenPosition = ScreenToWorld(GraphicsDevice.Viewport.Width / 2, (int)(GraphicsDevice.Viewport.Height / 1.5f));
                scale *= 1.1f;
                ScaleTarget = scale;
                if (Target == null)
                    Position += screenPosition - ScreenToWorld(GraphicsDevice.Viewport.Width / 2, (int)(GraphicsDevice.Viewport.Height / 1.5f));
            }
            else if (currentMouseState.ScrollWheelValue - previousMouseState.ScrollWheelValue < 0)
            {
                Vector2 screenPosition = ScreenToWorld(GraphicsDevice.Viewport.Width / 2, (int)(GraphicsDevice.Viewport.Height / 1.5f));
                scale *= 0.9f;
                ScaleTarget = scale;
                if (Target == null)
                    Position += screenPosition - ScreenToWorld(GraphicsDevice.Viewport.Width / 2, (int)(GraphicsDevice.Viewport.Height / 1.5f));
            }
#endif

            if (Target != null)
            {
                Vector2 centeredPosition = Target.Position - new Vector2(Conversion.ToWorld(GraphicsDevice.Viewport.Width / 2 / Scale), Conversion.ToWorld(GraphicsDevice.Viewport.Height / 1.5f / Scale));

                // To make the camera go ahead of the character, so that the player can see better where he/she is heading to
                if (Target is Character)
                {
                    counter -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (counter < 0)
                    {
                        Vector2 velocity = ((Character)Target).torso.LinearVelocity;

                        if (previousVelocity.X > threshold && velocity.X > threshold)
                            goingRight += increment;
                        else
                            goingRight -= increment / 2;
                        if (previousVelocity.X < -threshold && velocity.X < -threshold)
                            goingLeft += increment;
                        else
                            goingLeft -= increment / 2;
                        if (previousVelocity.Y > threshold && velocity.Y > threshold)
                            goingDown += increment * 2;
                        else
                            goingDown -= increment / 2;
                        /*if (previousVelocity.Y < -threshold && velocity.Y < -threshold)
                            goingUp += increment;
                        else
                            goingUp -= increment / 2;*/

                        goingRight = Math.Min(maxshift, Math.Max(0, goingRight));
                        goingLeft = Math.Min(maxshift, Math.Max(0, goingLeft));
                        //goingUp = Math.Min(maxshift, Math.Max(0, goingUp));
                        goingDown = Math.Min(maxshift, Math.Max(0, goingDown));

                        previousVelocity = velocity;
                        counter += delay;
                    }

                    centeredPosition += Vector2.UnitX * goingRight;
                    centeredPosition -= Vector2.UnitX * goingLeft;
                    centeredPosition += Vector2.UnitY * goingDown;
                    centeredPosition -= Vector2.UnitY * goingUp;
                }

                float distanceX = (float)Math.Abs(centeredPosition.X - Position.X);
                float distanceY = (float)Math.Abs(centeredPosition.Y - Position.Y);
                float speedX = Math.Min(81, distanceX * distanceX);
                float speedY = Math.Min(81, distanceY * distanceY);

                if (distanceX >= 0.8f)
                    Position += Vector2.UnitX * (Vector2.Normalize(centeredPosition - Position) * speedX * (float)gameTime.ElapsedGameTime.TotalSeconds);

                if (distanceY >= 0.8f)
                    Position += Vector2.UnitY * (Vector2.Normalize(centeredPosition - Position) * speedY * (float)gameTime.ElapsedGameTime.TotalSeconds);

                //Position = centeredPosition;
            }
#if !FINAL_RELEASE
            else
            {
                if (Keyboard.GetState().IsKeyDown(Keys.NumPad4))
                    Position = Position + Conversion.ToWorld(-Vector2.UnitX * 15);
                if (Keyboard.GetState().IsKeyDown(Keys.NumPad8))
                    Position = Position + Conversion.ToWorld(-Vector2.UnitY * 15);
                if (Keyboard.GetState().IsKeyDown(Keys.NumPad6))
                    Position = Position + Conversion.ToWorld(Vector2.UnitX * 15);
                if (Keyboard.GetState().IsKeyDown(Keys.NumPad2))
                    Position = Position + Conversion.ToWorld(Vector2.UnitY * 15);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.NumPad5))
                Target = null;

            // To grab the camera.
            if (!Grabbing && IsMouseInWindow(currentMouseState) && currentMouseState.MiddleButton == ButtonState.Pressed && previousMouseState.MiddleButton == ButtonState.Released)
            {
                Target = null;
                Grabbing = true;
                GrabbingPosition = new Vector2(currentMouseState.X, currentMouseState.Y);
            }
            else if (currentMouseState.MiddleButton == ButtonState.Released)
                Grabbing = false;
            if (Grabbing && (currentMouseState.X != previousMouseState.X || currentMouseState.Y != previousMouseState.Y))
            {
                Vector2 currentPosition = ScreenToWorld(currentMouseState);
                Vector2 previousPosition = ScreenToWorld(previousMouseState);
                Position.X -= currentPosition.X - previousPosition.X;
                Position.Y -= currentPosition.Y - previousPosition.Y;
            }
#endif

            ViewNonScaled = Matrix.CreateLookAt(new Vector3(Conversion.ToDisplay(Position.X), Conversion.ToDisplay(Position.Y), 1), new Vector3(Conversion.ToDisplay(Position.X), Conversion.ToDisplay(Position.Y), 0), new Vector3(0, 1, 0));
            View = Matrix.CreateScale(Conversion.DisplayUnitsToWorldUnitsRatio) * ViewNonScaled;
            Projection = Matrix.CreateOrthographicOffCenter(0, GraphicsDevice.Viewport.Width / Scale, GraphicsDevice.Viewport.Height / Scale, 0, 0, 1);

            if (Target != null)
                ListenerPosition = Target.Position;
            else
                ListenerPosition = Position + new Vector2(Conversion.ToWorld(GraphicsDevice.Viewport.Width / 2 / Scale), Conversion.ToWorld(GraphicsDevice.Viewport.Height / 1.5f / Scale));
            
            //scene.SoundManager.ISoundEngine.SetListenerPosition(ListenerPosition.X, ListenerPosition.Y, 0f, 0, 0, 1);

            previousMouseState = currentMouseState;
        }

        public Vector2 WorldToScreen(Vector2 worldPosition)
        {
            Vector3 position = Game.GraphicsDevice.Viewport.Project(new Vector3(worldPosition.X, worldPosition.Y, 0), Projection, View, Matrix.Identity);
            return new Vector2(position.X, position.Y);
        }

        public Vector2 ScreenToWorld(Vector2 screenPosition)
        {
            return Conversion.ToWorld(screenPosition) / Scale + Position;
        }

        public Vector2 ScreenToWorld(int x, int y)
        {
            return ScreenToWorld(new Vector2(x, y));
        }

        public Vector2 ScreenToWorld(MouseState mouseState)
        {
            return ScreenToWorld(mouseState.X, mouseState.Y);
        }

        public bool IsMouseInWindow(MouseState mouseState)
        {
            return Game.IsActive &&
                mouseState.X >= 0 && mouseState.X < GraphicsDevice.Viewport.Width &&
                mouseState.Y >= 0 && mouseState.Y < GraphicsDevice.Viewport.Height &&
                System.Windows.Forms.Form.ActiveForm != null &&
                System.Windows.Forms.Form.ActiveForm.Text.Equals(Game.Window.Title);
        }
    }
}

   
