using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Editor.Elements;

namespace Editor
{
    public class InputManager : DrawableGameComponent
    {
        public IControllable Target;
        Keys keyboardA = Keys.Space;
        Keys keyboardY = Keys.LeftControl;
        Keys keyboardB = Keys.LeftAlt;
        Keys keyboardX = Keys.RightControl;

        public InputManager(Game game)
            : base(game)
        {

        }

        public override void Update(GameTime gameTime)
        {
            if (Target != null)
                processKeyboard();
        }

        float hThreshold = 0.10f;
        float vThreshold = 0.9f;
        protected KeyboardState previousKeyboardState;
        protected GamePadState previosGamepadState;
        protected void processKeyboard()
        {
            KeyboardState currentKeyboardState = Keyboard.GetState();
            GamePadState currentGamepadState = GamePad.GetState(PlayerIndex.One);

#if FINAL_RELEASE
            if (Game.IsActive)
#else
            if (Game.IsActive && System.Windows.Forms.Form.ActiveForm != null && System.Windows.Forms.Form.ActiveForm.Text.Equals(Game.Window.Title))
#endif
            {
                if(currentKeyboardState.IsKeyDown(keyboardA) && previousKeyboardState.IsKeyUp(keyboardA))
                {
                    Target.AActionStart();
#if DEBUG_INPUT
                    Console.WriteLine("AActionStart");
#endif
                }
                if (currentKeyboardState.IsKeyDown(keyboardB) && previousKeyboardState.IsKeyUp(keyboardB))
                {
                    Target.BActionStart();
#if DEBUG_INPUT
                    Console.WriteLine("BActionStart");
#endif
                }
                if ((currentKeyboardState.IsKeyDown(keyboardX) && previousKeyboardState.IsKeyUp(keyboardX)))
                {
                    Target.XActionStart();
#if DEBUG_INPUT
                    Console.WriteLine("XActionStart");
#endif
                }
                if (currentKeyboardState.IsKeyDown(keyboardY) && previousKeyboardState.IsKeyUp(keyboardY)) 
                {
                    Target.YActionStart();
#if DEBUG_INPUT
                    Console.WriteLine("YActionStart");
#endif
                }
                if (currentKeyboardState.IsKeyDown(Keys.Left) && previousKeyboardState.IsKeyUp(Keys.Left))
                {
                    Target.LeftActionStart();
#if DEBUG_INPUT
                    Console.WriteLine("LeftActionStart");
#endif
                }
                if (currentKeyboardState.IsKeyDown(Keys.Right) && previousKeyboardState.IsKeyUp(Keys.Right))
                {
                    Target.RightActionStart();
#if DEBUG_INPUT
                    Console.WriteLine("RightActionStart");
#endif
                }
                if(currentKeyboardState.IsKeyDown(Keys.Up) && previousKeyboardState.IsKeyUp(Keys.Up))
                {
                    Target.UpActionStart();
#if DEBUG_INPUT
                    Console.WriteLine("UpActionStart");
#endif
                }
                if(currentKeyboardState.IsKeyDown(Keys.Down) && previousKeyboardState.IsKeyUp(Keys.Down))
                {
                    Target.DownActionStart();
#if DEBUG_INPUT
                    Console.WriteLine("DownActionStart");
#endif
                }
                if (currentKeyboardState.IsKeyUp(keyboardA) && previousKeyboardState.IsKeyDown(keyboardA))
                {
                    Target.AActionStop();
#if DEBUG_INPUT
                    Console.WriteLine("AActionStop");
#endif
                }
                if (currentKeyboardState.IsKeyUp(keyboardB) && previousKeyboardState.IsKeyDown(keyboardB))
                {
                    Target.BActionStop();
#if DEBUG_INPUT
                    Console.WriteLine("BActionStop");
#endif
                }
                if (currentKeyboardState.IsKeyUp(keyboardX) && previousKeyboardState.IsKeyDown(keyboardX))
                {
                    Target.XActionStop();
#if DEBUG_INPUT
                    Console.WriteLine("XActionStop");
#endif
                }
                if (currentKeyboardState.IsKeyUp(keyboardY) && previousKeyboardState.IsKeyDown(keyboardY))
                {
                    Target.YActionStop();
#if DEBUG_INPUT
                    Console.WriteLine("YActionStop");
#endif
                }
                if (currentKeyboardState.IsKeyUp(Keys.Left) && previousKeyboardState.IsKeyDown(Keys.Left))
                {
                    Target.LeftActionStop();
#if DEBUG_INPUT
                    Console.WriteLine("LeftActionStop");
#endif
                }
                if (currentKeyboardState.IsKeyUp(Keys.Right) && previousKeyboardState.IsKeyDown(Keys.Right))
                {
                    Target.RightActionStop();
#if DEBUG_INPUT
                    Console.WriteLine("RightActionStop");
#endif
                }
                if (currentKeyboardState.IsKeyUp(Keys.Up) && previousKeyboardState.IsKeyDown(Keys.Up))
                {
                    Target.UpActionStop();
#if DEBUG_INPUT
                    Console.WriteLine("UpActionStop");
#endif
                }
                if (currentKeyboardState.IsKeyUp(Keys.Down) && previousKeyboardState.IsKeyDown(Keys.Down))
                {
                    Target.DownActionStop();
#if DEBUG_INPUT
                    Console.WriteLine("DownActionStop");
#endif
                }
                if (currentKeyboardState.IsKeyDown(keyboardA))
                {
                    Target.AAction();
#if DEBUG_INPUT
                    Console.WriteLine("AAction");
#endif
                }
                if (currentKeyboardState.IsKeyDown(keyboardB))
                {
                    Target.BAction();
#if DEBUG_INPUT
                    Console.WriteLine("BAction");
#endif
                }
                if (currentKeyboardState.IsKeyDown(keyboardX))
                {
                    Target.XAction();
#if DEBUG_INPUT
                    Console.WriteLine("XAction");
#endif
                }
                if (currentKeyboardState.IsKeyDown(keyboardY))
                {
                    Target.YAction();
#if DEBUG_INPUT
                    Console.WriteLine("YAction");
#endif
                }
                if (currentKeyboardState.IsKeyDown(Keys.Left))
                {
                    Target.LeftAction();
#if DEBUG_INPUT
                    Console.WriteLine("LeftAction");
#endif
                }
                if (currentKeyboardState.IsKeyDown(Keys.Right))
                {
                    Target.RightAction();
#if DEBUG_INPUT
                    Console.WriteLine("RightAction");
#endif
                }
                if (currentKeyboardState.IsKeyDown(Keys.Up))
                {
                    Target.UpAction();
#if DEBUG_INPUT
                    Console.WriteLine("UpAction");
#endif
                }
                if (currentKeyboardState.IsKeyDown(Keys.Down))
                {
                    Target.DownAction();
#if DEBUG_INPUT
                    Console.WriteLine("DownAction");
#endif
                }
            }

            previousKeyboardState = currentKeyboardState;
            previosGamepadState = currentGamepadState;
        }
    }
}
