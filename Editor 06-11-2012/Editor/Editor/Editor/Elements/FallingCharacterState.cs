using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;

namespace Editor.Elements
{
    public class FallingCharacterState : CharacterState
    {
        int rows = 1;
        int columns = 3;
        bool maxSpeedRight = false;
        bool maxSpeedLeft = false;

        public FallingCharacterState(Scene scene, Character character)
            : base(scene, character)
        {
            texture = scene.Game.Content.Load<Texture2D>("falling");
            characterWidth = texture.Width / columns;
            characterHeight = texture.Height / rows;
            character.texture = texture;
            textureXmin = 0;
            textureYmin = 0;
        }
        /// <summary>
        /// Update for Falling State, checks the max speed both left and right
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            changeRunningTextures(gameTime);

            if (character.body.LinearVelocity.X > 4f)
            {
                maxSpeedRight = true;
            }
            else if (character.body.LinearVelocity.X < -4f)
                maxSpeedLeft = true;
            else
            {
                maxSpeedLeft = false;
                maxSpeedRight = false;
            }
        }

        float seconds = 0;
        /// <summary>
        /// Updates the characters texture for animation 
        /// </summary>
        private Vector2 changeRunningTextures(GameTime gameTime)
        {
            seconds += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (seconds > 0.1f)
            {
                seconds -= 0.1f;
                textureXmin += texture.Width / columns;

                if (textureXmin == texture.Width)
                    textureXmin = 0;
            }
            return new Vector2(textureXmin, textureYmin);
        }


        /// <summary>
        /// Checks if the character is jumping and infront of a ladder at the same time.
        /// </summary>
        public override void UpAction()
        {
            if (character.Ladder != null)
            {
                character.State = new ClimbingCharacterState(scene, character);
                character.State.UpAction();
            }
        }

        /// <summary>
        /// Checks if the character is falling and infront of a ladder at the same time.
        /// </summary>
        public override void DownAction()
        {
            if (character.Ladder != null)
            {
                character.State = new ClimbingCharacterState(scene, character);
                character.State.DownAction();
            }
        }
        /// <summary>
        /// Allows the character to increase velocity while moving right until masSpeedRight is achieved 
        /// </summary>
        public override void RightAction()
        {
            if (!maxSpeedRight)
            {
                character.torso.ApplyLinearImpulse(new Vector2(5f, 0));
                character.Effect = SpriteEffects.None;
            }
        }
        /// <summary>
        /// Allows the character to increase velocity while moving left until masSpeedRight is achieved 
        /// </summary>
        public override void LeftAction()
        {
            if (!maxSpeedLeft)
            {
                character.torso.ApplyLinearImpulse(new Vector2(-5f, 0));
                character.Effect = SpriteEffects.FlipHorizontally;
            }
        }
    }
}