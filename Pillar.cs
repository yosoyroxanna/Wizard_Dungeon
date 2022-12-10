using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Diagnostics;//to access Debug
using SharpDX.Direct3D9;

namespace final_project_rw
{
    internal class Pillar : DrawableGameComponent
    {
        protected cState pillarState;  // state

        protected Texture2D pillarOrbTexture;
        protected Rectangle drawRect;
        protected SpriteBatch spriteBatch;

        protected Vector2 pillarPosition;
        protected int pillarHealth;

        protected Game1 game;


        public Pillar(Game1 theGame, int xAxis, int yAxis, int health, cState state)
            : base(theGame)
        {
            pillarPosition = new Vector2(xAxis, yAxis);

            pillarHealth = health;

            pillarState = state;

            game = theGame;
        }


        public Vector2 getPosition()
        {
            return pillarPosition;
        }

        public void setState(cState ia)
        {
            pillarState = ia;
        }

        public cState getState()
        {
            return pillarState;
        }

        public bool collidesWithMonster(Vector2 monsterPosition, int monsterDamage)
        {
            bool attacked = false;
            double radius1 = Math.Sqrt((this.pillarPosition.X * this.pillarPosition.Y) / Math.PI);
            double radius2 = Math.Sqrt((monsterPosition.X * monsterPosition.Y) / Math.PI);

            float distance = (monsterPosition.X - this.pillarPosition.X) * (monsterPosition.X - this.pillarPosition.X) - (monsterPosition.Y - this.pillarPosition.Y) * (monsterPosition.Y - this.pillarPosition.Y);

            if (getState() == cState.Alive && ((monsterPosition.X - this.pillarPosition.X) * (monsterPosition.X - this.pillarPosition.X) + (monsterPosition.Y - this.pillarPosition.Y) * (monsterPosition.Y - this.pillarPosition.Y)) < Math.Sqrt(radius1 + radius2) * 400)
            {
                pillarHealth -= monsterDamage;
                attacked = true;
            }
            return attacked;
        }


        public override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            pillarOrbTexture = game.getPillarOrbTexture();
            spriteBatch = game.getSpriteBatch();

            drawRect = new Rectangle((int)pillarPosition.X, (int)pillarPosition.Y, pillarOrbTexture.Width / 14, pillarOrbTexture.Height / 14); // change to adj. sprite size

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            if (getState() == cState.Alive && pillarHealth == 0)
            {
                this.setState(cState.Dead);
            }
            if (pillarState == cState.Dead) return;


            drawRect.X = (int)pillarPosition.X;
            drawRect.Y = (int)pillarPosition.Y;
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();

            try
            {
                switch (pillarState)
                {
                    case cState.Alive:
                        spriteBatch.Draw(pillarOrbTexture, drawRect, Color.White);
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
