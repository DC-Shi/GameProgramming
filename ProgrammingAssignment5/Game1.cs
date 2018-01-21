using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using TeddyMineExplosion;

namespace ProgrammingAssignment5
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        // Mine sprite
        static Texture2D explosionSprite;
        static Texture2D teddybearSprite;
        static Texture2D mineSprite;
        //Add land mine placement functionality
        static List<Mine> mines = new List<Mine>();
        static List<TeddyBear> bears = new List<TeddyBear>();
        static List<Explosion> explodes = new List<Explosion>();
        // Click effect
        bool canLandMine = false;
        // Bear generate cooldown
        float cooldown = 1000;

        // resolution
        public const int WindowWidth = 800;
        public const int WindowHeight = 600;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // set resolution
            graphics.PreferredBackBufferWidth = WindowWidth;
            graphics.PreferredBackBufferHeight = WindowHeight;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            mineSprite = Content.Load<Texture2D>(@"graphics\mine");
            explosionSprite = Content.Load<Texture2D>(@"graphics\explosion");
            teddybearSprite = Content.Load<Texture2D>(@"graphics\teddybear");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            // get current mouse state and update burger
            MouseState mouse = Mouse.GetState();

            if (mouse.LeftButton == ButtonState.Pressed) canLandMine = true;
            // Previous pressed and then released is one click.
            if (mouse.LeftButton == ButtonState.Released && canLandMine)
            {
                Mine m = new Mine(mineSprite, mouse.X, mouse.Y);
                m.Active = true;
                mines.Add(m);
                canLandMine = false;
            }

            // Remove all inactive bears & mines.
            bears.RemoveAll(bear => !bear.Active);
            mines.RemoveAll(mine => !mine.Active);

            // Keep at most 3 bears.
            SpawnBearAtMost3(gameTime);

            foreach (var bear in bears) bear.Update(gameTime);

            foreach (var bear in bears)
            {
                foreach (var m in mines)
                {
                    // bear and mine are collided
                    if(m.CollisionRectangle.Intersects(bear.CollisionRectangle))
                    {
                        Explosion e = new Explosion(explosionSprite, m.CollisionRectangle.Center.X, m.CollisionRectangle.Center.Y);
                        explodes.Add(e);
                        m.Active = false;
                        bear.Active = false;
                    }
                }
            }

            foreach (Explosion e in explodes) e.Update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            // draw game objects
            foreach (Mine m in mines)
            {
                m.Draw(spriteBatch);
            }
            foreach (TeddyBear bear in bears)
            {
                bear.Draw(spriteBatch);
            }
            foreach(Explosion e in explodes)
            {
                e.Draw(spriteBatch);
            }

            // draw score and health

            spriteBatch.End();

            base.Draw(gameTime);
        }
        
        
        /// <summary>
        /// Spawns a new teddy bear at a random location
        /// </summary>
        /// <param name="gt">game time</param>
        private void SpawnBearAtMost3(GameTime gt)
        {
            if (cooldown < 0 && bears.Count < 3)
            {
                // generate random velocity, do not generate static bears.
                float x = 0, y = 0;
                while (x * x + y * y == 0)
                {
                    x = GetRandomFloat(-0.5f, 0.5f);
                    y = GetRandomFloat(-0.5f, 0.5f);
                }
                Vector2 velocity = new Vector2(x, y);

                // create new bear
                TeddyBear newBear = new TeddyBear(teddybearSprite, velocity, WindowWidth, WindowHeight);

                // add new bear to list
                bears.Add(newBear);
                cooldown = GetRandomFloat(1000, 3000);
            }
            /// Minus cooldown timer if currently in cooldown process.
            if (cooldown >= 0)
                cooldown -= gt.ElapsedGameTime.Milliseconds;
        }

        /// <summary>
        /// Random generator
        /// </summary>
        Random r = new Random();

        /// <summary>
        /// Get random float number
        /// </summary>
        /// <param name="min">minimum (inclusive)</param>
        /// <param name="max">maximum (exclusive)</param>
        /// <returns>random number</returns>
        private float GetRandomFloat(float min, float max)
        {
            return min + (max - min) * (float)r.NextDouble();
        }
    }
}
