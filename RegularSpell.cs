using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Diagnostics;//to access Debug
using SharpDX.Direct3D9;
using System.CodeDom;
using System.Windows.Forms;

namespace final_project_rw
{
    internal class RegularSpell : DrawableGameComponent
    {
        protected cState spellState;  // state

        protected Texture2D spellTexture;
        protected Rectangle drawRect;
        protected SpriteBatch spriteBatch;

        protected Vector2 spellPosition;
        protected bool keepGoing = false;
        protected int spellDamage;
        protected bool hitTrue = false;
        protected bool spellWasUsed = false;

        protected Game1 game;


        public RegularSpell(Game1 theGame, int xAxis, int yAxis, int damage, cState state)
            : base(theGame)
        {
            spellPosition = new Vector2(xAxis, yAxis);

            spellDamage = damage;

            spellState = state;

            game = theGame;
        }


        public Vector2 getPosition()
        {
            return spellPosition;
        }

        public void setPosition(int xAxis, int yAxis)
        {
            spellPosition = new Vector2(xAxis, yAxis);
        }

        public void setState(cState ia)
        {
            spellState = ia;
        }

        public cState getState()
        {
            return spellState;
        }

        public int getSpellDamage()
        {
            return spellDamage;
        }

        public void setSpellDamage(int damage)
        {
            spellDamage = damage;
        }

        public void hitsAMinion(bool hit)
        {
            if (hit == true)
            {
                hitTrue = true;
            }
        }

        public void setSpellActive(bool active)
        {
            spellWasUsed = active;
        }

        public bool getSpellActive()
        {
            return spellWasUsed;
        }

        public void activateSpell(int xAxis, int yAxis, cState state)
        {
            setPosition(xAxis, yAxis);
            setState(state);
        }

        public int movement(int wizardButton)
        {
            int result = 0;
            if (wizardButton == 3)
            {// moves spell straight up
                spellPosition.Y -= 1;
                keepGoing = true;
                result = 3;
            }
            return result;
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spellTexture = game.getSpellTexture();
            spriteBatch = game.getSpriteBatch();

            drawRect = new Rectangle((int)spellPosition.X, (int)spellPosition.Y, spellTexture.Width / 22, spellTexture.Height / 22); // change to adj. sprite size

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            // change position; movement
            spellPosition.X = getPosition().X;
            spellPosition.Y = getPosition().Y;

            bool keepGoingFurther = false;
            if (keepGoing == true)
            {
                keepGoingFurther = true;
            }

            if (keepGoingFurther == true)
            {
                spellPosition.Y -= 1;
            }

            // if spell hits top of window OR hits a minion then it will be considered Dead
            if (getState() == cState.Alive && spellPosition.Y < 0 || hitTrue == true)
            {
                this.setState(cState.Dead);
                keepGoing = false;
                hitTrue = false;
            }
            if (spellState == cState.Dead) return;


            drawRect.X = (int)spellPosition.X;
            drawRect.Y = (int)spellPosition.Y;
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();

            try
            {
                switch (spellState)
                {
                    case cState.Alive:
                        spriteBatch.Draw(spellTexture, drawRect, Color.White);
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
