using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;

namespace aMAZEing_Escape
{
    class Video_Intro : Ecran
    {

        SpriteBatch spriteBatch;
        Video myVideoFile;
        VideoPlayer videoPlayer;
        KeyboardState clavier;

        public Video_Intro(GraphicsDeviceManager graphics, ContentManager Content)
            : base(graphics, Content, "Vidéo d'introduction")
        {

            // Create the VideoPlayer
            videoPlayer = new VideoPlayer();
        }

        public override bool Init()
        {
            spriteBatch = new SpriteBatch(graphics.GraphicsDevice);
            myVideoFile = Content.Load<Video>(@"Vidéos/introduction");
            clavier = Keyboard.GetState();
            return base.Init();
        }

        public override void Shutdown()
        {
            base.Shutdown();
        }

        public override void Update(GameTime gameTime)
        {
            clavier = Keyboard.GetState();
            videoPlayer.Play(myVideoFile);

            if (gameTime.TotalGameTime.TotalSeconds > myVideoFile.Duration.Seconds + 0.5f || clavier.GetPressedKeys().Length > 0)
            {
                videoPlayer.Stop();
                Gestion_Ecran.Goto_Ecran("Menu Principal");
                this.Shutdown();
            }
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();
            if (videoPlayer.State == MediaState.Playing)
            {
                spriteBatch.Draw(videoPlayer.GetTexture(), new Rectangle((graphics.GraphicsDevice.Viewport.Width - myVideoFile.Width) / 2, (graphics.GraphicsDevice.Viewport.Height - myVideoFile.Height) / 2, myVideoFile.Width, myVideoFile.Height), Color.White);
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}