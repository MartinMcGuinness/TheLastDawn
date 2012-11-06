using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Editor.Elements
{
    public class CharacterState
    {
        protected Scene scene;
        protected Character character;
        protected Texture2D texture;
        public int characterWidth;
        public int characterHeight;
        public int textureXmin;
        public int textureYmin;

        public CharacterState(Scene scene, Character character)
        {
            this.character = character;
            this.scene = scene;
        }

        public virtual void Enter()
        {
        }

        public virtual void Exit(CharacterState nextState)
        {
        }

        public virtual void Update(GameTime gameTime)
        {
        }

        public virtual void Draw(GameTime gameTime)
        {
        }

        public virtual void AActionStart()
        {
        }

        public virtual void AAction()
        {
        }

        public virtual void AActionStop()
        {
        }

        public virtual void BActionStart()
        {
        }

        public virtual void BAction()
        {
        }

        public virtual void BActionStop()
        {
        }

        public virtual void XActionStart()
        {
        }

        public virtual void XAction()
        {
        }

        public virtual void XActionStop()
        {
        }

        public virtual void YActionStart()
        {
        }

        public virtual void YAction()
        {
        }

        public virtual void YActionStop()
        {
        }

        public virtual void RightActionStart()
        {
        }

        public virtual void RightAction()
        {
        }

        public virtual void RightActionStop()
        {
        }

        public virtual void LeftActionStart()
        {
        }

        public virtual void LeftAction()
        {
        }

        public virtual void LeftActionStop()
        {
        }

        public virtual void UpActionStart()
        {
        }

        public virtual void UpAction()
        {
        }

        public virtual void UpActionStop()
        {
        }

        public virtual void DownActionStart()
        {
        }

        public virtual void DownAction()
        {
        }

        public virtual void DownActionStop()
        {
        }
    }
}
