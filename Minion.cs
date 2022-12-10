using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Diagnostics;//to access Debug
using SharpDX.Direct3D9;
using System.CodeDom;

namespace final_project_rw
{
    internal class Minion : DrawableGameComponent
    {
        protected cState minionState;  // state

        protected Texture2D minionTexture;
        protected Rectangle drawRect;
        protected SpriteBatch spriteBatch;

        protected Vector2 minionPosition;
        protected Vector2 minionDirection;
        protected int minionHealth;
        protected bool hitTrue = false;
        protected bool minionHit = false;

        protected Game1 game;


        public Minion(Game1 theGame, int xAxis, int yAxis, int health, cState state)
            : base(theGame)
        {
            minionPosition = new Vector2(xAxis, yAxis);

            minionDirection = new Vector2();

            minionHealth = health;

            minionState = state;

            game = theGame;
        }


        public Vector2 getPosition()
        {
            return minionPosition;
        }

        public void setPosition(int xAxis, int yAxis)
        {
            minionPosition = new Vector2(xAxis, yAxis);
        }

        public void setState(cState ia)
        {
            minionState = ia;
        }

        public cState getState()
        {
            return minionState;
        }

        public int getMinionHealth()
        {
            return minionHealth;
        }

        public void setMinionHealth(int newMinionHealth)
        {
            minionHealth = newMinionHealth;
        }

        public void setMinionHit(bool gotHit)
        {
            minionHit = gotHit;
        }

        public bool getMinionHit()
        {
            return minionHit;
        }

        public void resetMinion(int xAxis, int yAxis, int health, cState state)
        {
            setPosition(xAxis, yAxis);
            setMinionHealth(health);
            setState(state);
        }

        public void movement(int wizardXAxis, int wizardYAxis)
        {
            minionDirection = new Vector2(wizardXAxis - getPosition().X, wizardYAxis - getPosition().Y);
        }


        public bool spellHit(Vector2 spell, int spellDamage)
        {
            bool hit = false;
            double radius1 = Math.Sqrt((this.minionPosition.X * this.minionPosition.Y) / Math.PI);
            double radius2 = Math.Sqrt((spell.X * spell.Y) / Math.PI);

            float distance = (spell.X - this.minionPosition.X) * (spell.X - this.minionPosition.X) - (spell.Y - this.minionPosition.Y) * (spell.Y - this.minionPosition.Y);

            if (getState() == cState.Alive && ((spell.X - this.minionPosition.X) * (spell.X - this.minionPosition.X) + (spell.Y - this.minionPosition.Y) * (spell.Y - this.minionPosition.Y)) < Math.Sqrt(radius1 + radius2) * 200) 
            {
                minionHealth -= spellDamage;
                hitTrue = true;
                hit = true;
            }
            return hit;
        }

        public bool hitsWizard(Vector2 wizardPosition)
        {
            bool hitWizard = false;
            double radius1 = Math.Sqrt((this.minionPosition.X * this.minionPosition.Y) / Math.PI);
            double radius2 = Math.Sqrt((wizardPosition.X * wizardPosition.Y) / Math.PI);

            float distance = (wizardPosition.X - this.minionPosition.X) * (wizardPosition.X - this.minionPosition.X) - (wizardPosition.Y - this.minionPosition.Y) * (wizardPosition.Y - this.minionPosition.Y);

            if (getState() == cState.Alive && ((wizardPosition.X - this.minionPosition.X) * (wizardPosition.X - this.minionPosition.X) + (wizardPosition.Y - this.minionPosition.Y) * (wizardPosition.Y - this.minionPosition.Y)) < Math.Sqrt(radius1 + radius2) * 300)
            {
                hitTrue = true;
                hitWizard = true;
            }
            return hitWizard;

        }


        public override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            minionTexture = game.getMinionTexture();
            spriteBatch = game.getSpriteBatch();

            drawRect = new Rectangle((int)minionPosition.X, (int)minionPosition.Y, minionTexture.Width / 20, minionTexture.Height / 20); // change to adj. sprite size

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            float eTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            minionPosition.X += minionDirection.X * eTime / 30;
            minionPosition.Y += minionDirection.Y * eTime / 30;


            // if minion's health hits 0, they will be considered Dead
            if (getState() == cState.Alive && minionHealth == 0 || hitTrue == true)
            {
                this.setState(cState.Dead);
                hitTrue = false;
            }
            if (minionState == cState.Dead) return;


            drawRect.X = (int)minionPosition.X;
            drawRect.Y = (int)minionPosition.Y;
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();

            try
            {
                switch (minionState)
                {
                    case cState.Alive:
                        spriteBatch.Draw(minionTexture, drawRect, Color.White);
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
