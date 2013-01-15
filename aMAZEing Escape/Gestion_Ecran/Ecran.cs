using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace aMAZEing_Escape
{
    public class Ecran
    {
        protected GraphicsDeviceManager graphics = null;
        protected ContentManager Content = null;

        public string nom
        {
            get;
            set;
        }

        /// <summary>
        /// Screen Constructor
        /// </summary>
        /// <param name="name">Must be unique since when you use ScreenManager is per name</param>
        public Ecran(GraphicsDeviceManager graphics, ContentManager Content, string nom)
        {
            this.nom = nom;
            this.graphics = graphics;
            this.Content = Content;
        }

        /// <summary>
        /// Virtual Function that's called when entering a Screen
        /// override it and add your own initialization code
        /// </summary>
        /// <returns></returns>
        public virtual bool Init()
        {
            return true;
        }

        /// <summary>
        /// Virtual Function that's called when exiting a Screen
        /// override it and add your own shutdown code
        /// </summary>
        /// <returns></returns>
        public virtual void Shutdown()
        {
        }

        /// <summary>
        /// Override it to have access to elapsed time
        /// </summary>
        /// <param name="elapsed">GameTime</param>
        public virtual void Update(GameTime gameTime)
        {

        }

        public virtual void Draw(GameTime gameTime)
        {

        }
    }
}