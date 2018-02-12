using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GameProject
{
    /// <summary>
    /// A burger
    /// </summary>
    public class Burger
    {
        #region Fields

        // graphic and drawing info
        Texture2D sprite;
        Rectangle drawRectangle;

        // burger stats
        int health = 100;
        public int Health
        {
            get
            {
                return health;
            }
            set
            {
                health = value > 0 ? value : 0;
            }
        }

        // shooting support
        bool canShoot = true;
        int elapsedCooldownMilliseconds = 0;

        // sound effect
        SoundEffect shootSound;

        #endregion

        #region Constructors

        /// <summary>
        ///  Constructs a burger
        /// </summary>
        /// <param name="contentManager">the content manager for loading content</param>
        /// <param name="spriteName">the sprite name</param>
        /// <param name="x">the x location of the center of the burger</param>
        /// <param name="y">the y location of the center of the burger</param>
        /// <param name="shootSound">the sound the burger plays when shooting</param>
        public Burger(ContentManager contentManager, string spriteName, int x, int y,
            SoundEffect shootSound)
        {
            LoadContent(contentManager, spriteName, x, y);
            this.shootSound = shootSound;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the collision rectangle for the burger
        /// </summary>
        public Rectangle CollisionRectangle
        {
            get { return drawRectangle; }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Updates the burger's location based on mouse. Also fires 
        /// french fries as appropriate
        /// </summary>
        /// <param name="gameTime">game time</param>
        /// <param name="mouse">the current state of the mouse</param>
        public void Update(GameTime gameTime, MouseState mouse)
        {
            // burger should only respond to input if it still has health
            if (health > 0)
            {
                // move burger using mouse
                int x = mouse.X, y = mouse.Y;

                // clamp burger in window
                x = Math.Max(x, 0);
                y = Math.Max(y, 0);
                x = Math.Min(x, GameConstants.WindowWidth - drawRectangle.Width);
                y = Math.Min(y, GameConstants.WindowHeight - drawRectangle.Height);
                drawRectangle.X = x;
                drawRectangle.Y = y;

                // update shooting allowed
                // timer concept (for animations) introduced in Chapter 7
                if (mouse.LeftButton == ButtonState.Pressed && canShoot)
                {
                    Projectile prj = new Projectile(ProjectileType.FrenchFries, Game1.GetProjectileSprite(ProjectileType.FrenchFries),
                        drawRectangle.Center.X, drawRectangle.Center.Y + GameConstants.FrenchFriesProjectileOffset,
                        -GameConstants.FrenchFriesProjectileSpeed);
                    Game1.AddProjectile(prj);
                    canShoot = false;
                }
                // shoot if appropriate
                if (!canShoot)
                {
                    elapsedCooldownMilliseconds += gameTime.ElapsedGameTime.Milliseconds;
                    if (elapsedCooldownMilliseconds > GameConstants.BurgerTotalCooldownMilliseconds || mouse.LeftButton == ButtonState.Released)
                    {
                        elapsedCooldownMilliseconds = 0;
                        canShoot = true;
                    }
                }
            }
        }

        /// <summary>
        /// Draws the burger
        /// </summary>
        /// <param name="spriteBatch">the sprite batch to use</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(sprite, drawRectangle, Color.White);
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Loads the content for the burger
        /// </summary>
        /// <param name="contentManager">the content manager to use</param>
        /// <param name="spriteName">the name of the sprite for the burger</param>
        /// <param name="x">the x location of the center of the burger</param>
        /// <param name="y">the y location of the center of the burger</param>
        private void LoadContent(ContentManager contentManager, string spriteName,
            int x, int y)
        {
            // load content and set remainder of draw rectangle
            sprite = contentManager.Load<Texture2D>(spriteName);
            drawRectangle = new Rectangle(x - sprite.Width / 2,
                y - sprite.Height / 2, sprite.Width,
                sprite.Height);
        }

        #endregion
    }
}
