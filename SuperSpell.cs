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
    internal class SuperSpell : DrawableGameComponent
    {
        protected cState superSpellState;  // state

        protected Texture2D superSpellTexture;
        protected Rectangle drawRect;
        protected Rectangle sourceRect;
        protected SpriteBatch spriteBatch;

        protected Vector2 superSpellPosition;
        protected Vector2 spritePosition;

        protected Game1 game;

        protected double spriteLength = 5.0;
        protected bool getStartTime = true;
        protected double startTime;
        protected double cTime;
        protected int totalSprites = 25;
        protected int xSprites = 5;
        protected int ySprites = 5;
        protected int xSheetSize = 3300;
        protected int ySheetSize = 3300;

        protected int spriteWidth;
        protected int spriteHeight;
        protected double percentOfTotalAnimation;
        protected int animationFrame;
        protected int spriteXPosition;
        protected int spriteYPosition;
        protected Vector2 origin = new Vector2(0, 0);


        public SuperSpell(Game1 theGame, int xAxis, int yAxis, cState state)
            : base(theGame)
        {
            superSpellPosition = new Vector2(xAxis, yAxis);

            superSpellState = state;

            game = theGame;
        }


        public Vector2 getPosition()
        {
            return superSpellPosition;
        }

        public void setPosition(int xAxis, int yAxis)
        {
            superSpellPosition = new Vector2(xAxis, yAxis);
        }

        public void setState(cState ia)
        {
            superSpellState = ia;
        }

        public cState getState()
        {
            return superSpellState;
        }

        public void activateSuperSpell(int xAxis, int yAxis, cState state)
        {
            setPosition(xAxis, yAxis);
            setState(state);
        }

        public bool collidesWithMonster(Vector2 monsterPosition)
        {
            bool collided = false;
            double radius1 = Math.Sqrt((this.superSpellPosition.X * this.superSpellPosition.Y) / Math.PI);
            double radius2 = Math.Sqrt((monsterPosition.X * monsterPosition.Y) / Math.PI);

            float distance = (monsterPosition.X - this.superSpellPosition.X) * (monsterPosition.X - this.superSpellPosition.X) - (monsterPosition.Y - this.superSpellPosition.Y) * (monsterPosition.Y - this.superSpellPosition.Y);

            if (getState() == cState.Alive && ((monsterPosition.X - this.superSpellPosition.X) * (monsterPosition.X - this.superSpellPosition.X) + (monsterPosition.Y - this.superSpellPosition.Y) * (monsterPosition.Y - this.superSpellPosition.Y)) < Math.Sqrt(radius1 + radius2) * 500)
            {
                collided = true;
            }
            return collided;
        }

        public int movement(int superSpellButton)
        {
            int result = 0;
            if (superSpellButton == 5)
            {// moves super spell up
                superSpellPosition.Y -= 3;
                result = 5;
            }
            if (superSpellButton == 6)
            {// moves super spell left
                superSpellPosition.X -= 3;
                result = 6;
            }
            if (superSpellButton == 7)
            {// moves super spell down
                superSpellPosition.Y += 3;
                result = 7;
            }
            if (superSpellButton == 8)
            {// moves super spell right
                superSpellPosition.X += 3;
                result = 8;
            }
            return result;
        }

        public void setStartTime(GameTime gameTime)
        {
            if (getStartTime == true)
            {
                startTime = gameTime.TotalGameTime.Seconds;
                getStartTime = false;
            }
        }

        public void calculateSpriteSheet()
        {
            spriteWidth = xSheetSize / xSprites;
            spriteHeight = ySheetSize / ySprites;

            percentOfTotalAnimation = ((cTime - startTime) / spriteLength) * 0.1;
            animationFrame = (int)(totalSprites * percentOfTotalAnimation);

            spriteXPosition = (animationFrame % xSprites) * spriteWidth;
            spriteYPosition = (animationFrame / xSprites) * spriteHeight;

            spritePosition = new Vector2(spriteXPosition, spriteYPosition);
        }




        public override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            superSpellTexture = game.getSuperSpellTexture();
            spriteBatch = game.getSpriteBatch();

            drawRect = new Rectangle((int)superSpellPosition.X, (int)superSpellPosition.Y, superSpellTexture.Width / 25, superSpellTexture.Height / 25); // change to adj. sprite size

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            // if spell hits top of window OR hits a minion then it will be considered Dead
            if (getState() == cState.Alive && superSpellPosition.Y < 0 || superSpellPosition.X < 0)
            {
                this.setState(cState.Dead);
            }
            if (superSpellState == cState.Dead) return;


            if (getState() == cState.Alive)
            {
                setStartTime(gameTime);

                if(getState() == cState.Dead)
                {
                    getStartTime = true;
                }
            }
            cTime = gameTime.TotalGameTime.Seconds;

            if (getState() == cState.Alive)
            {
                calculateSpriteSheet();
            }

            sourceRect = new Rectangle((int)spritePosition.X, (int)spritePosition.Y, spriteWidth, spriteHeight);


            // change position; movement
            superSpellPosition.X = getPosition().X;
            superSpellPosition.Y = getPosition().Y;


            drawRect.X = (int)superSpellPosition.X;
            drawRect.Y = (int)superSpellPosition.Y;
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();

            try
            {
                switch (superSpellState)
                {
                    case cState.Alive:
                        spriteBatch.Draw(superSpellTexture, drawRect, sourceRect, Color.White, 0, origin, SpriteEffects.None, 0);
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
