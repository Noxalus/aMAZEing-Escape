using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.GamerServices;

namespace aMAZEing_Escape
{
    class Succes : Ecran
    {

        private Texture2D image_succes;
        private Texture2D metal_tex;
        private SpriteFont texte;
      
        private SpriteBatch spriteBatch;
        private KeyboardState clavier;
        private KeyboardState clavier_prec;

        // Sons et musiques
        private AudioEngine Audio;
        private SoundBank BanqueSon;
        private WaveBank BanqueWave;
        private Cue musique_victoire;
        
        private string son_menu_validation;
        
        float pulsation_diminution;

        public Succes(GraphicsDeviceManager graphics, ContentManager Content)
            : base(graphics, Content, "Succes")
        {
        }

        public override bool Init()
        {
            clavier = new KeyboardState();
            clavier_prec = Keyboard.GetState();

            spriteBatch = new SpriteBatch(graphics.GraphicsDevice);
            texte = Content.Load<SpriteFont>("Polices/menu");
            image_succes = Content.Load<Texture2D>("Images/BackGround_GAGNER");
            metal_tex = Content.Load<Texture2D>("Images/blanc");

            // Sons et musiques
            Audio = new AudioEngine("Content/Musiques/ambiance.xgs");
            // Chargement des banques
            BanqueSon = new SoundBank(Audio, "Content/Musiques/Sound Bank.xsb");
            BanqueWave = new WaveBank(Audio, "Content/Musiques/Wave Bank.xwb");
            musique_victoire = BanqueSon.GetCue("musique_victoire");
            son_menu_validation = "menu_validation";

            musique_victoire.Play();

            Audio.Update();

            return base.Init();
        }

        public override void Shutdown()
        {
            musique_victoire.Dispose();
            base.Shutdown();
        }

        public override void Update(GameTime gameTime)
        {

            clavier = Keyboard.GetState();

            if (clavier_prec != clavier && clavier.GetPressedKeys().Length > 0)
            {
                BanqueSon.GetCue(son_menu_validation).Play();
                Gestion_Ecran.Goto_Ecran("Statistiques");
                this.Shutdown();
            }

            float pulsation_vitesse = (float)gameTime.ElapsedGameTime.TotalSeconds * 4;
            pulsation_diminution = Math.Min(pulsation_diminution + pulsation_vitesse, 1);

            clavier_prec = clavier;

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            // Pulsation
            double time = gameTime.TotalGameTime.TotalSeconds;
            float pulsation = (float)Math.Sin(time * 6) + 1;
            float scale = 0.5f + pulsation * 0.05f * pulsation_diminution;

            graphics.GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();
            Rectangle fullscreen = new Rectangle(0, 0, graphics.GraphicsDevice.Viewport.Width, graphics.GraphicsDevice.Viewport.Height);
            spriteBatch.Draw(image_succes, fullscreen, Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}