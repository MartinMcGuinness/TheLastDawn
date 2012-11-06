using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework.Input;

namespace Editor.Elements
{
    public class Bridge : Element
    {
        public Body body;
        Texture2D texture;

        private float height;
        public override float Height
        {
            get
            {
                return height;
            }
            set
            {
                height = value;
                createBody();
            }
        }

        private float width;
        public override float Width
        {
            get
            {
                //return width;
                return Conversion.ToWorld(texture.Width);

            }
            set
            {
                width = value;
                createBody();
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

        public Bridge(Game game, Scene scene, Vector2 position, Vector2 size)
            : base(game, scene)
        {
            ZBuffer = 5f;
            height = size.Y;
            width = size.X;
            this.position = position;
            texture = Game.Content.Load<Texture2D>("bridge");
            createBody();
        }

        public override void Draw(GameTime gameTime)
        {
            float scale = scene.Camera.Scale;
            scene.SpriteBatch.Draw(texture, scale * Conversion.ToDisplay(body.Position - scene.Camera.Position), null, Color.White, body.Rotation, new Vector2(texture.Width / 2.0f, texture.Height / 2.0f), scale * (Vector2.UnitX * Width + Vector2.UnitY * Height), SpriteEffects.None, 0);
        }

        private void createBody()
        {
            float previousRotation = 0;
            if (body != null)
            {
                previousRotation = body.Rotation;
                body.Dispose();
            }
            body = BodyFactory.CreateRectangle(scene.World, Width, Height, 1.0f);
            body.Position = position;
            body.BodyType = BodyType.Static;
            body.Rotation = previousRotation;
            body.CollisionCategories = ElementCategory.FLOOR;
        }

        protected override void Dispose(bool disposing)
        {
            body.Dispose();
            base.Dispose(disposing);
        }
    }
}
