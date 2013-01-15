using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;
using System.IO;

namespace aMAZEing_Escape.Ecrans
{
    class Credits : Ecran
    {
        
        private string[] credits;
        private float credits_animation;
        private bool pause;
        private float rotation;
        private float vitesse;  
       
        private SpriteFont texte;
        private SpriteBatch spriteBatch;
        private KeyboardState clavier;
        private KeyboardState clavier_prec;
        private Texture2D noir;
        private Texture2D[] photos;

        // Sons et musiques
        private AudioEngine Audio;
        private SoundBank BanqueSon;
        private WaveBank BanqueWave;
        private Cue musique;

        private Texture2D fond_credits;
  
        private string son_menu_retour;

        public Credits(GraphicsDeviceManager graphics, ContentManager Content)
            : base(graphics, Content, "Credits")
        {
        }

        public override bool Init()
        {
            credits_animation = 150;
            fond_credits = Content.Load<Texture2D>("Images/fond_credits");
            pause = false;
            rotation = 0f;
            vitesse = 0.85f;

            credits = TableauCredits("Content/Crédits/credits.42");

            clavier = new KeyboardState();
            clavier_prec = Keyboard.GetState();

            spriteBatch = new SpriteBatch(graphics.GraphicsDevice);
            texte = Content.Load<SpriteFont>("Polices/menu");

            noir = Content.Load<Texture2D>("Images/black");
            photos = new Texture2D[4];
            photos[0] = Content.Load<Texture2D>("Photos/jarri_j");
            photos[1] = Content.Load<Texture2D>("Photos/chevri_t");
            photos[2] = Content.Load<Texture2D>("Photos/decast_k");
            photos[3] = Content.Load<Texture2D>("Photos/chigue_w");

            // Sons et musiques
            Audio = new AudioEngine("Content/Musiques/ambiance.xgs");
            // Chargement des banques
            BanqueSon = new SoundBank(Audio, "Content/Musiques/Sound Bank.xsb");
            BanqueWave = new WaveBank(Audio, "Content/Musiques/Wave Bank.xwb");

            musique = BanqueSon.GetCue("credits");

            Config.musique_menu.Stop(AudioStopOptions.Immediate);
            Config.musique_menu.Dispose();

            musique.Play();
            
            son_menu_retour = "menu_retour";
            Audio.Update();

            return base.Init();
        }

        public override void Shutdown()
        {
            musique.Dispose();
            base.Shutdown();
        }

        public override void Update(GameTime gameTime)
        {

            clavier = Keyboard.GetState();
            
            if (clavier_prec != clavier && clavier.IsKeyDown(Keys.Escape))
            {
                BanqueSon.GetCue(son_menu_retour).Play();
                Gestion_Ecran.Goto_Ecran("Menu Principal");
                this.Shutdown();
            }

            if (clavier.IsKeyDown(Keys.Left))
                rotation -= 0.01f;
            else if (clavier.IsKeyDown(Keys.Right))
                rotation += 0.01f;

            if (clavier.IsKeyDown(Keys.Down))
                vitesse -= 0.1f;
            else if (clavier.IsKeyDown(Keys.Up))
                vitesse += 0.1f;

            if (!pause && clavier_prec != clavier && clavier.IsKeyDown(Keys.Space))
            {
                musique.Pause();
                pause = true;
            }
            else if (pause && clavier_prec != clavier && clavier.IsKeyDown(Keys.Space))
            {
                musique.Resume();
                pause = false;
            }
            else if (!pause)
                credits_animation = credits_animation + vitesse;

            if (credits_animation > credits.Length * 25.25f + (aMAZEing_Escape.Properties.Settings.Default.index_resolution) * 100f)
                credits_animation = 150;


            clavier_prec = clavier;

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();

           
            for (int i = 0; i < credits.Length; i++)
            {
                string chaine = credits[i];
                float taille = 0.5f;
                if (chaine.Length > 3 && chaine.Substring(0, 3) == "[b]")
                {
                    //taille = 0.75f;
                    chaine = chaine.Substring(3, chaine.Length - 3);
                }

                spriteBatch.DrawString(texte, chaine,
                    new Vector2((graphics.GraphicsDevice.Viewport.Width / 2) - (texte.MeasureString(credits[i]).X / 2) + (texte.MeasureString(credits[i]).X / 4),
                        (graphics.GraphicsDevice.Viewport.Height - credits_animation + 4 * texte.MeasureString(credits[i]).Y) + 20 * i),
                        Color.Gray, rotation, Vector2.Zero, taille, SpriteEffects.None, 0);
            }

            /*
            spriteBatch.Draw(fond_credits, new Rectangle((graphics.GraphicsDevice.Viewport.Width / 2) - fond_credits.Width / 2, 50, fond_credits.Width, fond_credits.Height),
                            new Rectangle(0, 0, fond_credits.Width, fond_credits.Height),
                             Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0f);*/
            spriteBatch.Draw(noir,new Rectangle (0,0,graphics.GraphicsDevice.Viewport.Width,graphics.GraphicsDevice.Viewport.Height/5), Color.Black);
            spriteBatch.DrawString(texte, nom, new Vector2((graphics.GraphicsDevice.Viewport.Width / 2) - (texte.MeasureString(nom).X / 2), 50), Color.Gray, 0, Vector2.Zero, 1f, SpriteEffects.None, 1f);
            
            float scale = (aMAZEing_Escape.Properties.Settings.Default.index_resolution + 1) / 5f;

            if (clavier.IsKeyDown(Keys.J))
                spriteBatch.Draw(photos[0], new Vector2(graphics.GraphicsDevice.Viewport.Width / 15, 
                                (graphics.GraphicsDevice.Viewport.Height * 0.25f)), 
                                new Rectangle(0, 0, photos[0].Width, photos[0].Height), Color.White, 0f, Vector2.Zero, 
                                scale, SpriteEffects.None, 0f);
            if (clavier.IsKeyDown(Keys.T))
                spriteBatch.Draw(photos[1], new Vector2(graphics.GraphicsDevice.Viewport.Width / 15,
                                (graphics.GraphicsDevice.Viewport.Height * 0.5f)),
                                new Rectangle(0, 0, photos[1].Width, photos[1].Height), Color.White, 0f, Vector2.Zero,
                                scale, SpriteEffects.None, 0f);
            if (clavier.IsKeyDown(Keys.K))
                spriteBatch.Draw(photos[2], new Vector2(graphics.GraphicsDevice.Viewport.Width * 0.75f, 
                                (graphics.GraphicsDevice.Viewport.Height * 0.25f)),
                                new Rectangle(0, 0, photos[2].Width, photos[2].Height), Color.White, 0f, Vector2.Zero,
                                scale, SpriteEffects.None, 0f);
            if (clavier.IsKeyDown(Keys.W))
                spriteBatch.Draw(photos[3], new Vector2(graphics.GraphicsDevice.Viewport.Width * 0.75f, 
                                (graphics.GraphicsDevice.Viewport.Height * 0.5f)),
                                new Rectangle(0, 0, photos[3].Width, photos[3].Height), Color.White, 0f, Vector2.Zero,
                                scale, SpriteEffects.None, 0f);

            spriteBatch.DrawString(texte, credits_animation.ToString(), new Vector2(0, 0), Color.Gray, 0, Vector2.Zero, 1f, SpriteEffects.None, 1f);
            spriteBatch.DrawString(texte, (credits.Length * 25.25f + (aMAZEing_Escape.Properties.Settings.Default.index_resolution) * 100f).ToString(), new Vector2(0, 50), Color.Gray, 0, Vector2.Zero, 1f, SpriteEffects.None, 1f);


            spriteBatch.End();      

            base.Draw(gameTime);
        }

        // Méthodes supplémentaires
        public string[] TableauCredits(string fichier)
        {
            int compteur = 0;

            using (StreamReader readerToCount = new StreamReader(fichier))
            {
                while (readerToCount.ReadLine() != null)
                {
                    compteur++;
                }
            }

            string[] montab = new string[compteur];

            using (StreamReader reader = new StreamReader(fichier))
            {
                // Lecture du fichier et remplissage du tableau
                int i = 0;
                string ligne;
                while ((ligne = reader.ReadLine()) != null)
                {
                    montab[i] = ligne;
                    i++;
                }
            }
            return montab;
        }


        
    }
}