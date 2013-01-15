using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;

namespace aMAZEing_Escape.Ecrans
{
    class Quitter : Ecran
    {
        private string[] menu;
        private int curseur;
        private Rectangle backgroundRectangle;
        private SpriteFont texte;
        private SpriteFont titre;
        private Texture2D monstre;
        private Texture2D bulle;
        private SpriteBatch spriteBatch;
        private KeyboardState clavier;
        private KeyboardState clavier_prec;

        // Sons et musiques
        private AudioEngine Audio;
        private SoundBank BanqueSon;
        private WaveBank BanqueWave;
     
        private string son_menu_curseur;
        private string son_menu_validation;
        private string son_menu_retour;

        float pulsation_diminution;

        public Quitter(GraphicsDeviceManager graphics, ContentManager Content)
            : base(graphics, Content, "Quitter")
        {
        }

        public override bool Init()
        {
            Afficher_Ecran();
            curseur = 1;
            clavier = new KeyboardState();
            clavier_prec = Keyboard.GetState();

            spriteBatch = new SpriteBatch(graphics.GraphicsDevice);
            backgroundRectangle = new Rectangle (200,200,500,200);
            texte = Content.Load<SpriteFont>("Polices/menu");
            titre = Content.Load<SpriteFont>("Polices/menutitre");
            monstre = Content.Load<Texture2D>("Images/monstre");
            bulle = Content.Load<Texture2D>("Images/bulle");

            // Sons et musiques
            Audio = new AudioEngine("Content/Musiques/ambiance.xgs");
            // Chargement des banques
            BanqueSon = new SoundBank(Audio, "Content/Musiques/Sound Bank.xsb");
            BanqueWave = new WaveBank(Audio, "Content/Musiques/Wave Bank.xwb");
            
            son_menu_validation = "menu_validation";
            son_menu_curseur = "menu_curseur";
            son_menu_retour = "menu_retour";

            return base.Init();
        }

        public override void Shutdown()
        {
            base.Shutdown();
        }

        public override void Update(GameTime gameTime)
        {
            clavier = Keyboard.GetState();
            if (clavier_prec != clavier && clavier.IsKeyDown(Keys.Escape))
            {
                BanqueSon.GetCue(son_menu_retour).Play();
                Gestion_Ecran.Goto_Ecran("Menu Principal");
            }
            this.Shutdown();
            if (clavier_prec != clavier && clavier.IsKeyDown(Keys.Enter))
            {
                
            switch (curseur)
            {
                case 0:
                    BanqueSon.GetCue(son_menu_validation).Play();
                    Config.quitter_jeu = true;
                    break;
 
                case 1:
                    BanqueSon.GetCue(son_menu_retour).Play();
                    Gestion_Ecran.Goto_Ecran("Menu Principal");
                    break;

                
            }
            this.Shutdown();
            }



            //Déplacement du curseur
            if (clavier_prec != clavier && clavier.IsKeyDown(Keys.Down))
            {
                BanqueSon.GetCue(son_menu_curseur).Play();
                curseur = (curseur + 1) % menu.Length; 
            }

            else if (clavier_prec != clavier && clavier.IsKeyDown(Keys.Up))
            {
                BanqueSon.GetCue(son_menu_curseur).Play();
                if (curseur <= 0)
                    curseur = menu.Length - 1;
                else
                    curseur--;
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
            spriteBatch.DrawString(titre,"Etes-vous sûr de vouloir quitter ?",new Vector2 ((graphics.GraphicsDevice.Viewport.Width / 2) - (titre.MeasureString("Etes-vous sûr de vouloir quitter?").X / 2), graphics.GraphicsDevice.Viewport.Height/2 - 100),Color.Gray, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            //spriteBatch.DrawString(texte, nom, new Vector2((graphics.GraphicsDevice.Viewport.Width / 2) - (texte.MeasureString(nom).X / 2), 50), Color.Gray, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            for (int i = 0; i < menu.Length; i++)
            {
                if (curseur == i)
                    spriteBatch.DrawString(texte, menu[i], new Vector2((graphics.GraphicsDevice.Viewport.Width / 2) - (texte.MeasureString(menu[i]).X / 2) * scale, graphics.GraphicsDevice.Viewport.Height / 2 - texte.MeasureString(menu[i]).Y * scale + texte.MeasureString(menu[i]).Y / 2 * i), Color.Green, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
                else
                    spriteBatch.DrawString(texte, menu[i], new Vector2((graphics.GraphicsDevice.Viewport.Width / 2) - (texte.MeasureString(menu[i]).X / 2) * 0.4f, (graphics.GraphicsDevice.Viewport.Height / 2) - (texte.MeasureString(menu[i]).Y * 0.4f) + texte.MeasureString(menu[i]).Y / 2 * i), Color.Gray, 0, Vector2.Zero, 0.4f, SpriteEffects.None, 0);
            }
            Rectangle rectanglemonstre = new Rectangle((graphics.GraphicsDevice.Viewport.Width / 2) - 430, (graphics.GraphicsDevice.Viewport.Height / 2) - 60, 300, 300);
            Rectangle rectanglebulle = new Rectangle((graphics.GraphicsDevice.Viewport.Width)/2-250, (graphics.GraphicsDevice.Viewport.Height)/2-110, 500, 100);
            spriteBatch.Draw(monstre, rectanglemonstre, Color.White);
            spriteBatch.Draw(bulle, rectanglebulle, Color.Green);
            spriteBatch.End();

            base.Draw(gameTime);
        }
        public void Afficher_Ecran()
        {
            menu = new string[] {
                "Oui", 
                "Non"
            };
        }
    }
}

