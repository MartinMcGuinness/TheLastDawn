using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using FarseerPhysics.Factories;

namespace Editor.Elements
{
    public class Ladder : Element
    {
        Body body;
        Texture2D texture;

        private int stepsNumber;
        public int StepsNumber
        {
            get
            {
                return stepsNumber;
            }
            set
            {
                stepsNumber = value;
                createBody();
            }
        }

        public override float Height
        {
            get
            {
                return Conversion.ToWorld(texture.Height) * stepsNumber;
            }
            set
            {
            }
        }

        public override float Width
        {
            get
            {
                return Conversion.ToWorld(texture.Width);
            }
            set
            {
            }
        }

        Vector2 position;
        public override Vector2 Position
        {
            get
            {
                return body.Position;
            }
            set
            {
                body.Position = value;
                position = value;
            }
        }

        public override float Rotation
        {
            get
            {
                return body.Rotation;
            }
            set
            {
                body.Rotation = value;
            }
        }

        public Ladder(Game game, Scene scene, int stepsNumber, Vector2 position)
            : base(game, scene)
        {
            ZBuffer = -5f;
            this.stepsNumber = stepsNumber;
            texture = Game.Content.Load<Texture2D>("ladder");
            this.position = position;
            createBody();

            body.UserData = this;
        }

        private void createBody()
        {
            if (body != null)
                body.Dispose();
            body = BodyFactory.CreateRectangle(scene.World, Width, Height, 20f);
            body.Position = position;
            body.BodyType = BodyType.Static;
            body.CollisionCategories = Category.None;
            body.CollidesWith = Category.None;
        }

        public override void Draw(GameTime gameTime)
        {
            float currentElementPosition = texture.Height / 2.0f * (stepsNumber - 1);
            for (int i = 0; i < stepsNumber; i++)
            {
                scene.SpriteBatch.Draw(texture, scene.Camera.Scale * Conversion.ToDisplay(body.Position - scene.Camera.Position) + new Vector2(0, scene.Camera.Scale * currentElementPosition),
                    null, Color.White, body.Rotation, new Vector2(texture.Width / 2.0f, texture.Height / 2.0f), scene.Camera.Scale, SpriteEffects.None, 0);
                currentElementPosition -= texture.Height;
            }
        }

        protected override void Dispose(bool disposing)
        {
            body.Dispose();
            base.Dispose(disposing);
        }
    }
}
