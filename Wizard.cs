using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Diagnostics;//to access Debug
using SharpDX.Direct3D9;
using SharpDX.DirectWrite;

namespace final_project_rw
{
    internal class Wizard : DrawableGameComponent
    {
        protected cState wizardState;  // state

        protected Texture2D wizardTexture;
        protected Rectangle drawRect;
        protected SpriteBatch spriteBatch;

        protected Vector2 wizardPosition;
        protected int wizardHealth;

        protected Game1 game;


        public Wizard(Game1 theGame, int xAxis, int yAxis, int health, cState state)
            : base(theGame)
        {
            wizardPosition = new Vector2(xAxis, yAxis);

            wizardHealth = health;

            wizardState = state;

            game = theGame;
        }


        public Vector2 getPosition()
        {
            return wizardPosition;
        }

        public void setState(cState ia)
        {
            wizardState = ia;
        }

        public cState getState()
        {
            return wizardState;
        }

        public void setWizardHealth(int newHealth)
        {
            wizardHealth = newHealth;
        }

        public int getWizardHealth()
        {
            return wizardHealth;
        }

        public int movement(int wizardButton)
        {
            int result = 0;
            if (wizardButton == 1)
            {// moves wizard left
                wizardPosition.X -= 3;
                result = 1;
            }
            if (wizardButton == 2)
            {// moves wizard right
                wizardPosition.X += 3;
                result = 2;
            }
            return result;
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            wizardTexture = game.getWizardTexture();
            spriteBatch = game.getSpriteBatch();

            drawRect = new Rectangle((int)wizardPosition.X, (int)wizardPosition.Y, wizardTexture.Width / 16, wizardTexture.Height / 16); // change to adj. sprite size

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            // change position; movement
            wizardPosition.X = getPosition().X;
            wizardPosition.Y = getPosition().Y;

            // if wizard health runs to zero sets wizard state to Dead
            if (wizardHealth == 0)
            {
                this.setState(cState.Dead);
            }
            if (wizardState == cState.Dead) return;


            drawRect.X = (int)wizardPosition.X;
            drawRect.Y = (int)wizardPosition.Y;
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();

            try
            {
                switch (wizardState)
                {
                    case cState.Alive:
                        spriteBatch.Draw(wizardTexture, drawRect, Color.White);
                        break;

                    case cState.Dead:
                        return; // don't draw anything.
                }
            }
            finally
            {
                spriteBatch.End();
            }
        }
    }
}
