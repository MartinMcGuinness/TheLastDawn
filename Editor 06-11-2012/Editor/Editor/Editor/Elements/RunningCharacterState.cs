using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Editor.Elements
{
    public class RunningCharacterState : CharacterState
    {
        Random rand = new Random();
        public RunningCharacterState(Scene scene, Character character)
            : base(scene, character)
        {

            texture = scene.Game.Content.Load<Texture2D>("running");
            characterWidth = texture.Width / 10;// 8;
            characterHeight = texture.Height / 2;///5;
            character.texture = texture;
            textureXmin = 0;
            textureYmin = 0;
        }

        //public override void Update(GameTime gameTime)
        //{
        //    changeRunningTextures(gameTime);
        //    if ((currentFrame == 3 || currentFrame == 10))
        //    {
        //        scene.SoundManager.ISoundEngine.Play2D(scene.SoundManager.steps[rand.Next(scene.SoundManager.steps.Count)], false, false, false);
        //    }
        //}

        int currentFrame = 0;

        float seconds = 0;
        private Vector2 changeRunningTextures(GameTime gameTime)
        {
            float delay = 0.03f / (lastRunningSpeed / runningSpeed);

            seconds += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (seconds > delay)
            {
                currentFrame++;
                seconds -= delay;
                textureXmin += texture.Width / 10;

                if (textureXmin == (texture.Width / 10) * 4 && textureYmin == texture.Height / 2)
                {
                    currentFrame = 0;
                    textureXmin = 0;
                    textureYmin = 0;
                }
                else if (textureXmin == texture.Width)
                {
                    textureXmin = 0;
                    textureYmin += texture.Height / 2;
                }
            }
            return new Vector2(textureXmin, textureYmin);
        }

        public override void Exit(CharacterState nextState)
        {
        }


        public override void UpAction()
        {
            if (character.Ladder != null)
            {
                character.State = new ClimbingCharacterState(scene, character);
                character.State.UpAction();
            }
        }

        public override void DownAction()
        {
            if (character.Ladder != null)
            {
                character.State = new ClimbingCharacterState(scene, character);
                character.State.DownAction();
            }
        }

        public override void AActionStart()
        {
            character.State = new JumpingCharacterState(scene, character);
        }

        public override void UpActionStart()
        {
            if (character.Ladder == null)
                character.State = new JumpingCharacterState(scene, character);
        }

        //public override void BActionStart()
        //{
        //    if (character.touchedBody != null)
        //    {
        //        character.State = new GrabbingCharacterState(scene, character, character.touchedBody);
        //        character.State.BActionStart();
        //    }
        //}

        float lastRunningSpeed = 4.3f;
        float runningSpeed = 4.3f;
        public override void RightAction()
        {
            lastRunningSpeed = (character.body.LinearVelocity.Y < -1) ? runningSpeed / (-1 * character.body.LinearVelocity.Y) : runningSpeed;
            character.body.FixedRotation = false;
            if (character.lastContact != null)
            {
                float velocityDifference = character.body.LinearVelocity.Y - character.lastContact.LinearVelocity.Y;
                lastRunningSpeed = (velocityDifference < -1) ? runningSpeed / (-1 * velocityDifference) : runningSpeed;

                //GlidePlatform glidePlatform = character.lastContact.UserData as GlidePlatform;
                //if (glidePlatform != null && glidePlatform.Active)
                //{
                //    character.torso.LinearVelocity = new Vector2(((GlidePlatform)character.lastContact.UserData).Velocity + lastRunningSpeed, character.torso.LinearVelocity.Y);
                //}
                //else
                //{
                    character.torso.LinearVelocity = new Vector2(character.lastContact.LinearVelocity.X + lastRunningSpeed, character.torso.LinearVelocity.Y);
                //}
            }
            else
                character.torso.LinearVelocity = new Vector2(lastRunningSpeed, character.torso.LinearVelocity.Y);
            character.Effect = SpriteEffects.None;
        }

        public override void LeftAction()
        {
            lastRunningSpeed = (character.body.LinearVelocity.Y < -1) ? runningSpeed / (-1 * character.body.LinearVelocity.Y) : runningSpeed;
            character.body.FixedRotation = false;
            if (character.lastContact != null)
            {
                float velocityDifference = character.body.LinearVelocity.Y - character.lastContact.LinearVelocity.Y;
                lastRunningSpeed = (velocityDifference < -1) ? runningSpeed / (-1 * velocityDifference) : runningSpeed;

                //GlidePlatform glidePlatform = character.lastContact.UserData as GlidePlatform;
                //if (glidePlatform != null && glidePlatform.Active)
                //{
                //    character.torso.LinearVelocity = new Vector2(((GlidePlatform)character.lastContact.UserData).Velocity - lastRunningSpeed, character.torso.LinearVelocity.Y);
                //}
                //else
                //{
                    character.torso.LinearVelocity = new Vector2(character.lastContact.LinearVelocity.X - lastRunningSpeed, character.torso.LinearVelocity.Y);
                //}
            }
            else
                character.torso.LinearVelocity = new Vector2(-lastRunningSpeed, character.torso.LinearVelocity.Y);
            character.Effect = SpriteEffects.FlipHorizontally;
        }

        public override void RightActionStop()
        {
            character.body.FixedRotation = true;
            character.torso.LinearVelocity = Vector2.UnitY * character.torso.LinearVelocity;
            character.body.AngularVelocity = 0;
            //character.State = new IdleCharacterState(scene, character);
        }

        public override void LeftActionStop()
        {
            character.body.FixedRotation = true;
            character.torso.LinearVelocity = Vector2.UnitY * character.torso.LinearVelocity;
            character.body.AngularVelocity = 0;
            //character.State = new IdleCharacterState(scene, character);
        }
    }
}
