using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Diagnostics;
using SharpDX.DirectWrite;
using SharpDX.Direct2D1;
using SpriteBatch = Microsoft.Xna.Framework.Graphics.SpriteBatch;
using SharpDX.Direct3D9;

namespace final_project_rw
{
    public enum GameState { Menu, Playing };
    public enum cState { Dead, Pending, Alive };
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        public SpriteBatch spriteBatch;
        SpriteFont Font1;

        private GameState gameState;

        Color bgColor;
        Color bgColorEndGame;
        Color bgColorMenu;
        float screenWidth, screenHeight;
        Random rand;

        protected Texture2D bigMonsterTexture;
        protected Texture2D minionTexture;
        protected Texture2D pillarOrbTexture;
        protected Texture2D wizardTexture;
        protected Texture2D spellTexture;
        protected Texture2D superSpellTexture;

        protected Rectangle drawRect;

        private SoundEffect monsterDies;
        private SoundEffect castSpell;

        private bool gameOver = false; // turns true if a condition has been met that would end the game
        private bool wonGame = false; // turns true if condition was met that would mean the player won the game (monster dies)
        private String wonOrLost; // Sets up a string that will announce if the player has won or not to be drawn on the end screen
        bool gamePlaying = false;

        int amountOfPillars = 3; // number of pillars
        bool onePillarDestroyed = false;
        Pillar[] pillar; // pillar array

        Wizard wizard; // wizard object

        int amountOfSpells = 5; // max number of spells cast / on screen at one time
        int currentSpells = 0; // current number of spells on screen
        int spellDamage = 1; // amount of damage a regular spell will do
        int deadSpell; // keeps track of which "slots" in spell array have a "dead" spell
        bool readyToCast = true; // if true then another spell can be cast
        String readyOrNot;
        RegularSpell[] spell; // regular spell object

        Minion[] minion; // minion object
        int amountOfMinions = 8; // max number of minions on screen / spawned at one time
        int currentMinions = 0;
        int minionHealth = 1; // amount of health a minion has

        BigMonster monster; // monster object
        Vector2 closestPillarToMonster; // holds the position of the nearest pillar to the monster
        bool monsterIsDead = false;

        SuperSpell superSpell; // superSpell object
        bool readyToCastSuperSpell = false; // determines if super spell is ready to cast
        bool superSpellActive = false; // determines if super spell is active
        int amountToChargeSS = 4; // amount of minions kills needed to charge / activate super spell
        int minionKillCounter = 0; // keeps track of how many minions have been killed to charge super spell


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            graphics.IsFullScreen = false;
            graphics.PreferredBackBufferHeight = 900;
            graphics.PreferredBackBufferWidth = 1600;

            // create new random number generator
            rand = new Random((int)DateTime.Now.Ticks);
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            spriteBatch = new SpriteBatch(GraphicsDevice);
            screenWidth = (float)(Window.ClientBounds.Width);
            screenHeight = (float)(Window.ClientBounds.Height);

            bgColor = new Color(69, 31, 12); // set to your background color; red, green, blue;
            bgColorEndGame = new Color(0, 0, 0);
            bgColorMenu = new Color(42, 6, 64);

            bigMonsterTexture = Content.Load<Texture2D>("Images/big_monster");
            minionTexture = Content.Load<Texture2D>("Images/minion");
            pillarOrbTexture = Content.Load<Texture2D>("Images/pillar_orb");
            wizardTexture = Content.Load<Texture2D>("Images/wizard");
            spellTexture = Content.Load<Texture2D>("Images/spell");
            superSpellTexture = Content.Load<Texture2D>("Images/super_spell");


                // *****************************
                // PILLAR

                int currentPillars = 0;

                int xAxisPillar = 0;
                int yAxisPillar = 0;

                int health = 8;

                pillar = new Pillar[amountOfPillars];

                while (currentPillars < amountOfPillars)
                {
                    xAxisPillar = rand.Next((int)screenWidth - 300);
                    yAxisPillar = rand.Next((int)screenHeight - 450);

                    pillar[currentPillars] = new Pillar(this, xAxisPillar, yAxisPillar, health, cState.Pending);
                    Components.Add(pillar[currentPillars++]);
                }



                // *****************************
                // WIZARD

                // XAxis = 800, YAxis = 700, Health = 10
                wizard = new Wizard(this, 800, 700, 5, cState.Pending);
                Components.Add(wizard);



                // *****************************
                // REGULAR SPELL

                spell = new RegularSpell[amountOfSpells];

                for (int i = 0; i < amountOfSpells; i++)
                {
                    spell[i] = new RegularSpell(this, (int)wizard.getPosition().X, (int)wizard.getPosition().Y, spellDamage, cState.Dead);
                    Components.Add(spell[i]);
                }



                // *****************************
                // MINION

                minion = new Minion[amountOfMinions];

                for (int i = 0; i < amountOfMinions; i++)
                {
                    int xAxisMinion = 0;
                    int yAxisMinion = 0;

                    xAxisMinion = rand.Next((int)screenWidth - 300);
                    yAxisMinion = rand.Next((int)screenHeight - 650);

                    minion[currentMinions] = new Minion(this, xAxisMinion, yAxisMinion, minionHealth, cState.Pending);
                    Components.Add(minion[currentMinions]);

                    currentMinions++;
                }



                // *****************************
                // MONSTER

                // XAxis = 750, YAxis = 400, Health = 3, Damage = 1
                monster = new BigMonster(this, 600, 300, 3, 1, cState.Pending);
                Components.Add(monster);



                // *****************************
                // SUPER SPELL

                superSpell = new SuperSpell(this, (int)wizard.getPosition().X, (int)wizard.getPosition().Y, cState.Dead);
                Components.Add(superSpell);



                // load text font
                Font1 = Content.Load<SpriteFont>("newFont");

                //Debug code for illustration purposes.
                Debug.WriteLine("Window Width " + Window.ClientBounds.Width);
                Debug.WriteLine("Window Height " + Window.ClientBounds.Height);
                Debug.WriteLine("IsFixedTimeStep " + IsFixedTimeStep);
                Debug.WriteLine("TargetElapsedTime " + TargetElapsedTime);

                base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            monsterDies = Content.Load<SoundEffect>("Audio/big_monster_dies");
            castSpell = Content.Load<SoundEffect>("Audio/cast_spell");

            drawRect = new Rectangle(660, 280, wizardTexture.Width / 12, wizardTexture.Height / 12); // change to adj. sprite size

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (gameState == GameState.Menu)
            {
                GetExitInput(gameTime);

                if (gamePlaying == true)
                {
                    for (int i = 0; i < amountOfPillars; i++)
                    {
                        pillar[i].setState(cState.Alive);
                    }
                    for (int i = 0; i < amountOfMinions; i++)
                    {
                        minion[i].setState(cState.Alive);
                    }
                    wizard.setState(cState.Alive);
                    monster.setState(cState.Alive);
                }
            }

            if (gameState == GameState.Playing)
            {
                GetExitInput(gameTime);  // get user input
                int wizardButton = GetWizardInput(gameTime);
                int superSpellButton = GetSuperSpellInput(gameTime);

                if (minionKillCounter == amountToChargeSS)
                {
                    readyToCastSuperSpell = true;
                }


                if (superSpellActive == false)
                {
                    // *****************************
                    // WIZARD

                    wizard.movement(wizardButton);

                    if (wizard.getState() == cState.Dead)
                    {
                        gameOver = true;
                    }



                    // *****************************
                    // REGULAR SPELL


                    // checks if spacebar(3) was pressed AND if readyToCast is true
                    if (wizardButton == 3 && readyToCast == true)
                    {
                        for (int i = 0; i < amountOfSpells; i++)
                        {
                            if (spell[i].getState() == cState.Dead)
                            {
                                deadSpell = i;
                            }
                        }
                        // creates spells; makes cState = Alive
                        if (currentSpells < amountOfSpells)
                        {
                            spell[deadSpell].activateSpell((int)wizard.getPosition().X, (int)wizard.getPosition().Y, cState.Alive);

                            spell[deadSpell].movement(wizardButton);
                            readyToCast = false;
                            spell[deadSpell].setSpellActive(true);

                            castSpell.Play();

                            currentSpells++;
                        }
                    }

                    // if spacebar(3) is let go, and readyToCast is set to true; allows 1 spell to be cast per button press
                    if (wizardButton == 0)
                    {
                        readyToCast = true;
                    }

                    // if a spell "dies" AND spellUsed is true, subtract 1 from currentSpells and set spellUsed to false
                    for (int i = 0; i < amountOfSpells; i++)
                    {
                        if (spell[i].getState() == cState.Dead && spell[i].getSpellActive() == true)
                        {
                            currentSpells--;
                            spell[i].setSpellActive(false);
                        }
                    }

                }



                // *****************************
                // MINION

                // resets a minion to be alive in a new location; spawns new minion
                for (int i = 0; i < amountOfMinions; i++)
                {
                    if (minion[i].getState() == cState.Dead)
                    {
                        int xAxis = 0;
                        int yAxis = 0;

                        xAxis = rand.Next((int)screenWidth - 300);
                        yAxis = rand.Next((int)screenHeight - 650);

                        minion[i].resetMinion(xAxis, yAxis, minionHealth, cState.Alive);
                    }
                }

                // moves minions towards wizard as wizard moves
                for (int j = 0; j < amountOfMinions; j++)
                {
                    if (minion[j].getState() == cState.Alive)
                    {
                        minion[j].movement((int)wizard.getPosition().X, (int)wizard.getPosition().Y);
                    }
                }

                // compares a minion and spell to see if they collide, then returns true or false to hitsAMinion
                for (int i = 0; i < amountOfSpells; i++)
                {
                    for (int j = 0; j < amountOfMinions; j++)
                    {
                        if (minion[j].getState() == cState.Alive && spell[i].getState() == cState.Alive)
                        {
                            bool hit = minion[j].spellHit(spell[i].getPosition(), spell[i].getSpellDamage());
                            spell[i].hitsAMinion(hit);
                            minion[j].setMinionHit(hit);

                            if (hit == true)
                            {
                                minionKillCounter++;
                            }
                        }
                    }
                }

                // determines if a minion collides with the wizard, if true wizard will lose 1 health
                for (int i = 0; i < amountOfMinions; i++)
                {
                    if (minion[i].getState() == cState.Alive && wizard.getState() == cState.Alive)
                    {
                        bool hitsWizard = minion[i].hitsWizard(wizard.getPosition());
                        if (hitsWizard == true)
                        {
                            wizard.setWizardHealth(wizard.getWizardHealth() - 1);
                            hitsWizard = false;
                        }
                    }
                }


                // if a minion dies, reset whether it was hit, adds 1 to minion kill counter
                for (int i = 0; i < amountOfMinions; i++)
                {
                    if (minion[i].getState() == cState.Dead && minion[i].getMinionHit() == true)
                    {
                        minionKillCounter++;
                        minion[i].setMinionHit(false);
                    }
                }



                // *****************************
                // MONSTER

                // finds which pillar is closest to the monster
                for (int i = 0; i < amountOfPillars; i++)
                {
                    for (int j = 0; j < amountOfPillars; j++)
                    {

                        if (i != j && pillar[j].getState() == cState.Alive && pillar[i].getState() == cState.Alive)
                        {
                            closestPillarToMonster = monster.compareClosestPillar(pillar[i].getPosition(), pillar[j].getPosition());
                        }
                        else
                        {
                            for (int k = 0; k < amountOfPillars; k++)
                            {
                                if (i != j && pillar[j].getState() == cState.Dead && pillar[i].getState() == cState.Dead && pillar[k].getState() == cState.Alive)
                                {
                                    closestPillarToMonster = pillar[k].getPosition();
                                }
                            }
                        }
                    }
                }

                // moves the monster towards the nearest pillar
                if (monster.getState() == cState.Alive)
                {
                    monster.movement((int)closestPillarToMonster.X, (int)closestPillarToMonster.Y);
                }

                // finds if monster collides with pillar; if monster reaches the pillar / attacks it
                bool hitPillar = false;

                for (int i = 0; i < amountOfPillars; i++)
                {
                    if (monster.getState() == cState.Alive && pillar[i].getState() == cState.Alive)
                    {
                        hitPillar = pillar[i].collidesWithMonster(monster.getPosition(), monster.getMonsterDamage());
                    }
                    if (hitPillar == true && pillar[i].getState() == cState.Dead)
                    {
                        onePillarDestroyed = true;
                        hitPillar = false;
                    }
                }

                if (onePillarDestroyed == true)
                {
                    monster.reset();
                    onePillarDestroyed = false;
                }

                int allDestroyed = 0;
                if (pillar[allDestroyed].getState() == cState.Dead && pillar[allDestroyed + 1].getState() == cState.Dead && pillar[allDestroyed + 2].getState() == cState.Dead)
                {
                    gameOver = true;
                }

                if (monster.getState() == cState.Dead && monster.getMonsterHealth() == 0 && monsterIsDead == false)
                {
                    monsterDies.Play();
                    monsterIsDead = true;
                    gameOver = true;
                    wonGame = true;
                }



                // *****************************
                // SUPER SPELL

                if (wizardButton == 4 && readyToCastSuperSpell == true)
                {
                    // creates super spell; makes cState = Alive
                    if (superSpell.getState() == cState.Dead)
                    {
                        superSpell.activateSuperSpell((int)wizard.getPosition().X, (int)wizard.getPosition().Y - 50, cState.Alive);

                        readyToCastSuperSpell = false;
                        superSpellActive = true;
                    }
                }
                if (superSpellActive == true)
                {
                    superSpell.movement(superSpellButton);

                    bool collidesWithMonster = superSpell.collidesWithMonster(monster.getPosition());
                    if (collidesWithMonster == true)
                    {
                        superSpell.setState(cState.Dead);
                        superSpellActive = false;
                        monster.setMonsterHealth(monster.getMonsterHealth() - 1);
                        readyToCastSuperSpell = false;
                        minionKillCounter = 0;
                    }

                    for (int i = 0; i < amountOfMinions; i++)
                    {
                        bool collidesWithMinion = superSpell.collidesWithMinion(minion[i].getPosition());
                        if (collidesWithMinion == true)
                        {
                            superSpell.setState(cState.Dead);
                            superSpellActive = false;
                            minion[i].setMinionHealth(minion[i].getMinionHealth() - 1);
                            readyToCastSuperSpell = false;
                            minionKillCounter = 0;
                        }
                    }
                }

                if (readyToCastSuperSpell == true)
                {
                    readyOrNot = "Ready";
                }
                else
                {
                    readyOrNot = "Not Ready";
                }



                // *****************************
                // GAME OVER

                if (gameOver == true)
                {
                    wizard.setState(cState.Dead);
                    monster.setState(cState.Dead);

                    for (int i = 0; i < amountOfMinions; i++)
                    {
                        minion[i].setState(cState.Dead);
                    }

                    for (int i = 0; i < amountOfPillars; i++)
                    {
                        pillar[i].setState(cState.Dead);
                    }

                    for (int i = 0; i < amountOfSpells; i++)
                    {
                        spell[i].setState(cState.Dead);
                    }

                    if (wonGame == true)
                    {
                        wonOrLost = "You Won!";
                    }
                    else
                    {
                        wonOrLost = "You Lost";
                    }

                }
            }

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected void GetExitInput(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.Escape))
            { // if escape key pressed, then exit
                this.Exit();
            }
            if (keyboardState.IsKeyDown(Keys.Enter))
            { // if enter key pressed, then begin game
                gameState = GameState.Playing;
                gamePlaying= true;
            }
        }

        protected int GetWizardInput(GameTime gameTime)
        {
            int wizardButton = 0;
            KeyboardState keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.None))
            {// if no key is pressed, then nothing happens (returns 0)
                wizardButton = 0;
            }
            if (keyboardState.IsKeyDown(Keys.A))
            { // if A key pressed, then move left (returns 1)
                wizardButton = 1;
            }
            if (keyboardState.IsKeyDown(Keys.D))
            {// if D key pressed, then move right (returns 2)
                wizardButton = 2;
            }
            if (keyboardState.IsKeyDown(Keys.Space))
            {// if Space key pressed, then cast a spell (returns 3)
                wizardButton = 3;
            }
            if (keyboardState.IsKeyDown(Keys.E))
            {// if E key is pressed, then cast a Super Spell (returns 4)
                wizardButton = 4;
            }
            return wizardButton;
        }

        protected int GetSuperSpellInput(GameTime gameTime)
        {
            int superSpellButton = 0;
            KeyboardState keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.W))
            { // if W key pressed, then move up (returns 5)
                superSpellButton = 5;
            }
            if (keyboardState.IsKeyDown(Keys.A))
            { // if A key pressed, then move left (returns 6)
                superSpellButton = 6;
            }
            if (keyboardState.IsKeyDown(Keys.S))
            { // if S key pressed, then move down (returns 7)
                superSpellButton = 7;
            }
            if (keyboardState.IsKeyDown(Keys.D))
            {// if D key pressed, then move right (returns 8)
                superSpellButton = 8;
            }
            return superSpellButton;
        }

        protected override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();

            if (gameState == GameState.Menu)
            {
                GraphicsDevice.Clear(bgColorMenu);

                spriteBatch.DrawString(Font1, "Wizard Dungeon", new Vector2(700, 200), Color.White);
                spriteBatch.Draw(wizardTexture, drawRect, Color.White);
                spriteBatch.DrawString(Font1, "Press 'Enter' to Start Game", new Vector2(640, 600), Color.White);
                spriteBatch.DrawString(Font1, "Or Press 'Esc' to Exit Game", new Vector2(640, 650), Color.White);
            }

            if (gameState == GameState.Playing)
            {
                GraphicsDevice.Clear(bgColor);

                if (gameOver == true)
                {
                    GraphicsDevice.Clear(bgColorEndGame);
                }


                if (gameOver == true)
                {
                    spriteBatch.DrawString(Font1, "Game Over", new Vector2(740, 400), Color.White);
                    spriteBatch.DrawString(Font1, wonOrLost, new Vector2(750, 440), Color.White);
                    spriteBatch.DrawString(Font1, "Press 'Esc' to Exit", new Vector2(700, 480), Color.White);
                }
                else
                {
                    spriteBatch.DrawString(Font1, "Move Wizard Left: A", new Vector2(10, 0), Color.White);
                    spriteBatch.DrawString(Font1, "Move Wizard Right: D", new Vector2(10, 20), Color.White);
                    spriteBatch.DrawString(Font1, "Move Super Spell Up: W", new Vector2(10, 50), Color.White);
                    spriteBatch.DrawString(Font1, "Move Super Spell Down: S", new Vector2(10, 70), Color.White);
                    spriteBatch.DrawString(Font1, "Move Super Spell Left: A", new Vector2(10, 90), Color.White);
                    spriteBatch.DrawString(Font1, "Move Super Spell Right: D", new Vector2(10, 110), Color.White);
                    spriteBatch.DrawString(Font1, "Cast Regular Spell: Space", new Vector2(10, 140), Color.White);
                    spriteBatch.DrawString(Font1, "Cast Super Spell: E", new Vector2(10, 160), Color.White);

                    spriteBatch.DrawString(Font1, "Spells Available to Cast: " + "  " + (amountOfSpells - currentSpells), new Vector2(10, screenHeight - 150), Color.White);
                    spriteBatch.DrawString(Font1, "Monster Health Points: " + "  " + monster.getMonsterHealth(), new Vector2(10, screenHeight - 120), Color.White);
                    spriteBatch.DrawString(Font1, "Wizard Health Points: " + "  " + wizard.getWizardHealth(), new Vector2(10, screenHeight - 90), Color.White);
                    spriteBatch.DrawString(Font1, "Minion Kills to Charge Super Spell: " + "  " + (amountToChargeSS - minionKillCounter), new Vector2(10, screenHeight - 60), Color.White);
                    spriteBatch.DrawString(Font1, "Super Spell is: " + "  " + readyOrNot, new Vector2(10, screenHeight - 30), Color.White);

                }

            }


            spriteBatch.End();
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }

        public float getScreenWidth()
        {
            return screenWidth;
        }

        public float getScreenHeight()
        {
            return screenHeight;
        }

        public SpriteBatch getSpriteBatch()
        {
            return spriteBatch;
        }

        public Texture2D getBigMonsterTexture()
        {
            return bigMonsterTexture;
        }

        public Texture2D getMinionTexture()
        {
            return minionTexture;
        }

        public Texture2D getPillarOrbTexture()
        {
            return pillarOrbTexture;
        }

        public Texture2D getWizardTexture()
        {
            return wizardTexture;
        }

        public Texture2D getSpellTexture()
        {
            return spellTexture;
        }

        public Texture2D getSuperSpellTexture()
        {
            return superSpellTexture;
        }
    }
}