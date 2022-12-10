using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Diagnostics;//to access Debug
using SharpDX.Direct3D9;
using SharpDX.DirectWrite;
using Microsoft.Xna.Framework.Audio;
using System.Reflection.Metadata;

namespace final_project_rw
{
    internal class BigMonster : DrawableGameComponent
    {
        protected cState monsterState;  // state

        protected Texture2D bigMonsterTexture;
        protected Rectangle drawRect;
        protected SpriteBatch spriteBatch;

        protected Vector2 monsterPosition;
        protected Vector2 monsterDirection;
        protected int monsterHealth;
        protected int monsterDamage;
        protected Vector2 closestPillar;

        protected Game1 game;



        public BigMonster(Game1 theGame, int xAxis, int yAxis, int health, int damage, cState state)
    : base(theGame)
        {
            monsterPosition = new Vector2(xAxis, yAxis);

            monsterDirection = new Vector2();

            monsterHealth = health;

            monsterDamage = damage;

            monsterState = state;

            game = theGame;
        }



        public Vector2 getPosition()
        {
            return monsterPosition;
        }

        public void setState(cState ia)
        {
            monsterState = ia;
        }

        public cState getState()
        {
            return monsterState;
        }

        public int getMonsterHealth()
        {
            return monsterHealth;
        }

        public void setMonsterHealth(int newHealth)
        {
            monsterHealth = newHealth;
        }

        public int getMonsterDamage()
        {
            return monsterDamage;
        }

        public Vector2 compareClosestPillar(Vector2 pillarOne, Vector2 pillarTwo)
        {
            float distance1 = (pillarOne.X - this.getPosition().X) * (pillarOne.X - this.getPosition().X) - (pillarOne.Y - this.getPosition().Y) * (pillarOne.Y - this.getPosition().Y);
            float distance2 = (pillarTwo.X - this.getPosition().X) * (pillarTwo.X - this.getPosition().X) - (pillarTwo.Y - this.getPosition().Y) * (pillarTwo.Y - this.getPosition().Y);

            if (distance1 < distance2)
            {
                closestPillar = pillarOne;
            }
            else
            {
                closestPillar = pillarTwo;
            }

            return closestPillar;
        }

        public void movement(int pillarXAxis, int pillarYAxis)
        {
            monsterDirection = new Vector2(pillarXAxis - getPosition().X, pillarYAxis - getPosition().Y) * 2;
        }

        public void reset()
        {
            closestPillar = new Vector2(0, 0);
            monsterDirection = new Vector2(0, 0);
        }


        public override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            bigMonsterTexture = game.getBigMonsterTexture();
            spriteBatch = game.getSpriteBatch();

            drawRect = new Rectangle((int)monsterPosition.X, (int)monsterPosition.Y, bigMonsterTexture.Width / 12, bigMonsterTexture.Height / 12); // change to adj. sprite size

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            float eTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            monsterPosition.X += monsterDirection.X * eTime / 30;
            monsterPosition.Y += monsterDirection.Y * eTime / 30;


            // if minion's health hits 0, they will be considered Dead
            if (monsterHealth == 0)
            {
                this.setState(cState.Dead);
            }
            if (monsterState == cState.Dead) return;


            drawRect.X = (int)monsterPosition.X;
            drawRect.Y = (int)monsterPosition.Y;
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();

            try
            {
                switch (monsterState)
                {
                    case cState.Alive:
                        spriteBatch.Draw(bigMonsterTexture, drawRect, Color.White);
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
