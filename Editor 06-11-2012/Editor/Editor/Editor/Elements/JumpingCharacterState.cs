using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using IrrKlang;

namespace Editor.Elements
{
    class JumpingCharacterState : CharacterState
    {
        static Random random = new Random();
        bool isJumping = false;
        bool maxSpeedRight = false;
        bool maxSpeedLeft = false;

        public JumpingCharacterState(Scene scene, Character character)
            : base(scene, character)
        {
            texture = scene.Game.Content.Load<Texture2D>("jumpingStart");
            characterWidth = texture.Width / 5;
            characterHeight = texture.Height;
            character.texture = texture;
            textureXmin = 0;
            textureYmin = 0;
        }

        public override void Update(GameTime gameTime)
        {
            if (character.body.LinearVelocity.Y > 0)
                character.State = new FallingCharacterState(scene, character);
            changeJumpingTextures(gameTime);

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
        private Vector2 changeJumpingTextures(GameTime gameTime)
        {
            if (!isJumping)
            {
                seconds += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (seconds > 0.05f)
                {
                    seconds -= 0.05f;
                    textureXmin += texture.Width / 5;
                }

                if (textureXmin == texture.Width)
                {
                    isJumping = true;
                    textureXmin = 0;
                    textureYmin = 0;
                    texture = scene.Game.Content.Load<Texture2D>("jumpingAir");
                    character.texture = texture;
                }
            }
            else
            {
                if (!(textureXmin == (texture.Width / 5) * 2 && character.torso.LinearVelocity.Y < 0) &&
                    !(textureXmin == (texture.Width / 5) * 4 && character.torso.LinearVelocity.Y > 0))
                    seconds += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (seconds > 0.04f)
                {
                    seconds -= 0.04f;
                    textureXmin += texture.Width / 5;

                    if (textureXmin == texture.Width)
                        textureXmin = 0;
                }
            }
            return new Vector2(textureXmin, textureYmin);
        }

        bool body_OnCollision(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
            //if (character.contactsNumber > 0)
                //character.State = new IdleCharacterState(scene, character);
            //TODO
            return true;
        }

        public override void Enter()
        {
            character.body.OnCollision += body_OnCollision;
            if (character.contactsNumber > 0 || character.Ladder != null)
            {
                character.torso.LinearVelocity = new Vector2(character.torso.LinearVelocity.X, 0);
                character.torso.ApplyLinearImpulse(new Vector2(0, -285f));
                //if (!(character is Energy))
               // {
                    //ISound sound = scene.SoundManager.ISoundEngine.Play3D(scene.SoundManager.Grunt, character.Position.X, character.Position.Y, 0, false, true, false);
                    //sound.Volume = Math.Max(0.1f, (float)random.NextDouble() - 0.5f);
                    //sound.Paused = false;
                //} TODO
            }
            else
                character.torso.Awake = character.body.Awake = true;
        }



        public override void Exit(CharacterState nextState)
        {
            character.body.OnCollision -= body_OnCollision;
        }

        public override void RightAction()
        {
            if (!maxSpeedRight)
            {
                character.torso.ApplyLinearImpulse(new Vector2(10f, 0));
                character.Effect = SpriteEffects.None;
            }
        }

        public override void LeftAction()
        {
            if (!maxSpeedLeft)
            {
                character.torso.ApplyLinearImpulse(new Vector2(-10f, 0));
                character.Effect = SpriteEffects.FlipHorizontally;
            }
        }

        public override void UpAction()
        {
            if (character.Ladder != null && character.Ladder != character.LastLadder)
            {
                character.State = new ClimbingCharacterState(scene, character);
                character.State.UpAction();
            }
        }

        public override void DownAction()
        {
            if (character.Ladder != null && character.Ladder != character.LastLadder)
            {
                character.State = new ClimbingCharacterState(scene, character);
                character.State.DownAction();
            }
        }
    }
}
