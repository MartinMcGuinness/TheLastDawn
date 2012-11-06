using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics.Dynamics.Joints;
using Microsoft.Xna.Framework.Input;
using FarseerPhysics.Dynamics.Contacts;

namespace Editor.Elements  
{
    public class Character : Element, IControllable, IActivable
    {
        public int contactsNumber = 0;
        public Body body;
        public Body torso;
        public Texture2D texture;
        public RevoluteJoint revoluteJoint;
        public SpriteEffects Effect;

        public Body touchedBody;
        public Body lastContact;
        

        public Ladder Ladder;
        public Ladder LastLadder;

        protected CharacterState state;
        public CharacterState State
        {
            get
            {
                return state;
            }
            set
            {
                if (state != null)
                    state.Exit(value);
                state = value;
#if !FINAL_RELEASE
                Console.WriteLine(state.GetType().Name);
#endif
                state.Enter();
            }
        }

        private bool isActive = true;
        public bool Active
        {
            get
            {
                return isActive;
            }

            set
            {
                isActive = value;
            }
        }

        float height;
        public override float Height
        {
            get { return height; }
            set { }
        }

        float width;
        public override float Width
        {
            get { return width; }
            set { }
        }

        public override Vector2 Position
        {
            get
            {
                return torso.Position + Vector2.UnitY * body.FixtureList[0].Shape.Radius / 2;
            }
            set
            {
                Vector2 previous = torso.Position;
                torso.Position = value - Vector2.UnitY * body.FixtureList[0].Shape.Radius / 2;
                body.Position += torso.Position - previous;
            }
        }

        public override float Rotation
        {
            get
            {
                return 0;
            }
            set
            {
            }
        }

        public Character(Game game, Scene scene, Vector2 position)
            : base(game, scene)
        {
            texture = Game.Content.Load<Texture2D>("zombie");
            ZBuffer = 0f;

            width = Conversion.ToWorld(44);
            height = Conversion.ToWorld(165);

            body = BodyFactory.CreateCircle(scene.World, (width / 2f), 40);
            body.Position = position - Vector2.UnitY * body.FixtureList[0].Shape.Radius / 2 + Vector2.UnitY * (height - width + width / 2.0f) / 2.0f;
            body.BodyType = BodyType.Dynamic;
            body.Friction = float.MaxValue;
            body.UserData = this;
            body.CollisionCategories = ElementCategory.CHARACTER;
            body.SleepingAllowed = false;

            body.OnCollision += new OnCollisionEventHandler(body_OnCollision);
            body.OnSeparation += new OnSeparationEventHandler(body_OnSeparation);

            torso = BodyFactory.CreateRectangle(scene.World, (width), (height - width + width / 2.0f), 40);
            torso.Position = position - Vector2.UnitY * body.FixtureList[0].Shape.Radius / 2;
            torso.BodyType = BodyType.Dynamic;
            torso.FixedRotation = true;
            torso.Friction = 0.0f;
            torso.UserData = this;
            torso.CollisionCategories = ElementCategory.CHARACTER;
            torso.CollidesWith = Category.All & ~ElementCategory.ENERGY;
            torso.SleepingAllowed = false;

            //torso.OnCollision += new OnCollisionEventHandler(torso_OnCollision);
            torso.OnSeparation += new OnSeparationEventHandler(torso_OnSeparation);

            revoluteJoint = new RevoluteJoint(torso, body, new Vector2(0, (height - width + width / 2.0f) / 2.0f), Vector2.Zero);
            revoluteJoint.CollideConnected = false;
            scene.World.AddJoint(revoluteJoint);

            State = new RunningCharacterState(scene, this);
            
        }

        float countToFall = 0;

        protected void body_OnSeparation(Fixture fixtureA, Fixture fixtureB)
        {
            if (!fixtureB.IsSensor)
            {
                contactsNumber = Math.Max(0, contactsNumber - 1);
                if (fixtureB.Body == lastContact)
                    lastContact = null;
            }
        }

        protected bool body_OnCollision(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
            if (!fixtureB.IsSensor)
            {
                contactsNumber++;
                countToFall = 0;
                lastContact = fixtureB.Body;
            }
            return true;
        }

        protected void torso_OnSeparation(Fixture fixtureA, Fixture fixtureB)
        {
            if (fixtureB.Body == touchedBody)
                touchedBody = null;
        }


        Vector2 velocity = Vector2.Zero;
        Vector2 prevVelocity = Vector2.Zero;
        public override void Update(GameTime gameTime)
        {
            if (contactsNumber == 0)
            {
                countToFall += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (countToFall > 0.13f && !(state is FallingCharacterState) && !(state is DyingCharacterState) && !(state is ClimbingCharacterState) && GraphicsDevice != null) //TODO: same thing in onseparation with GraphicsDevice, since it's creating a new object after being disposed.
                    State = new FallingCharacterState(scene, this);
            }

            prevVelocity = velocity;
            velocity = body.LinearVelocity;

            if (Math.Abs(prevVelocity.Y - velocity.Y) > 20&& !(state is ClimbingCharacterState) && !(state is DyingCharacterState))
                State = new DyingCharacterState(scene, this);

            updateLadder();
            State.Update(gameTime);
        }

        public bool IsTouchingElement(Element o)
        {
            return Math.Abs(Position.X - o.Position.X) <= 0.5 * (Width + o.Width + 0.1f) && //+0.1f to provide a margin
                Math.Abs(Position.Y - o.Position.Y) <= 0.5 * (Height + o.Height + 0.1f);
        }

        /// <summary>
        /// Updates which ladder si the current one.
        /// </summary>
        protected void updateLadder()
        {
            if (Ladder != null && !IsLadderInRange(Ladder))
            {
                Ladder = null;
                LastLadder = null;
            }

            if (Ladder == null)
            {
                foreach (Element i in scene.Elements)
                {
                    if (i as Ladder != null && IsLadderInRange((Ladder)i))
                    {
                        Ladder = (Ladder)i;
                        break;
                    }
                }
            }
        }
        /// <summary>
        /// Returns if the ladder close enough to climb or not
        /// </summary>
        public bool IsLadderInRange(Ladder ladder)
        {
            Vector2 headPosition = torso.Position;

            if (Math.Abs(headPosition.X - ladder.Position.X) < Conversion.ToWorld(40))
            {
                if (ladder.Position.Y - ladder.Height / 2 <= headPosition.Y - Height / 2 + 0.5f && headPosition.Y - 0.5f <= ladder.Position.Y + ladder.Height / 2)
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Checks if the characters position against the ladders position in order to see if the character
        /// is able to move up on the ladder.
        /// </summary>
        public bool IsLadderInRangeToGoUp(Ladder ladder)
        {
            Vector2 headPosition = torso.Position;
            if (Math.Abs(headPosition.X - ladder.Position.X) < Conversion.ToWorld(40))
                if (ladder.Position.Y - ladder.Height / 2 <= headPosition.Y - Height / 2)
                    return true;
            return false;
        }

        /// <summary>
        /// Checks if the characters position against the ladders position in order to see if the character
        /// is able to move down on the ladder.
        /// </summary>
        public bool IsLadderInRangeToGoDown(Ladder ladder)
        {
            Vector2 headPosition = torso.Position;
            if (Math.Abs(headPosition.X - ladder.Position.X) < Conversion.ToWorld(40))
                if (headPosition.Y <= ladder.Position.Y + ladder.Height / 2)
                    return true;
            return false;
        }
        /// <summary>
        /// Disposes of the Farseer Physics properties of the character 
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            scene.World.RemoveJoint(revoluteJoint);
            torso.Dispose();
            body.Dispose();
            base.Dispose(disposing);
        }

        /// <summary>
        /// Adds a sprite to a batch of sprites for rendering using a specific texture, position, scource rectangle etc.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            scene.SpriteBatch.Draw(texture, scene.Camera.Scale * (Conversion.ToDisplay(Position - scene.Camera.Position) - Vector2.UnitY * 10), // this -10 is to make the character a bit up, not to be below the floor
                new Rectangle(State.textureXmin, State.textureYmin, State.characterWidth, State.characterHeight),
                Color.White, torso.Rotation, new Vector2(State.characterWidth / 2.0f, State.characterHeight / 2.0f), scene.Camera.Scale, Effect, 0);
        }

        public virtual void AActionStart()
        {
            State.AActionStart();
        }

        public virtual void AAction()
        {
            State.AAction();
        }

        public virtual void AActionStop()
        {
            State.AActionStop();
        }

        public virtual void BActionStart()
        {
            foreach (Element i in scene.Elements)
            {
                Activator activator = i as Activator;
                if (activator != null && !activator.EnergyElement && IsTouchingElement(i))
                {
                    activator.Activate();
                }
            }

            State.BActionStart();
        }

        public virtual void BAction()
        {
            State.BAction();
        }

        public virtual void BActionStop()
        {
            State.BActionStop();
        }

        public virtual void XActionStart()
        {
            State.XActionStart();
        }

        public virtual void XAction()
        {
            State.XAction();
        }

        public virtual void XActionStop()
        {
            State.XActionStop();
        }

        public virtual void YActionStart()
        {
            State.YActionStart();
        }

        public virtual void YAction()
        {
            State.YAction();
        }

        public virtual void YActionStop()
        {
            State.YActionStop();
        }

        public virtual void RightActionStart()
        {
            State.RightActionStart();
        }

        public virtual void RightAction()
        {
            State.RightAction();
        }

        public virtual void RightActionStop()
        {
            State.RightActionStop();
        }

        public virtual void LeftActionStart()
        {
            State.LeftActionStart();
        }

        public virtual void LeftAction()
        {
            State.LeftAction();
        }

        public virtual void LeftActionStop()
        {
            State.LeftActionStop();
        }

        public virtual void UpActionStart()
        {
            State.UpActionStart();
        }

        public virtual void UpAction()
        {
            State.UpAction();
        }

        public virtual void UpActionStop()
        {
            State.UpActionStop();
        }

        public virtual void DownActionStart()
        {
            State.DownActionStart();
        }

        public virtual void DownAction()
        {
            State.DownAction();
        }

        public virtual void DownActionStop()
        {
            State.DownActionStop();
        }
    }
}

