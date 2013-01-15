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
    class Menu_Principal: Ecran
    {
        private string[] menu;
        private int curseur;
        private Texture2D banniere;
        private SpriteFont texte;
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

        private float pulsation_diminution;
        private int compteur_decalage;

        public Menu_Principal(GraphicsDeviceManager graphics, ContentManager Content)
            : base(graphics, Content, "Menu Principal")
        {
        }

        public override bool Init()
        {
            menu = new string[] { "Nouvelle Partie", "Reprendre", "Options", "Crédits", "Quitter" };
            curseur = 0;
            clavier = new KeyboardState();
            clavier_prec = Keyboard.GetState();

            spriteBatch = new SpriteBatch(graphics.GraphicsDevice);
            texte = Content.Load<SpriteFont>("Polices/menu");
            banniere = Content.Load<Texture2D>("Images/banniere");

            // Sons et musiques
            Audio = new AudioEngine("Content/Musiques/ambiance.xgs");
            // Chargement des banques
            BanqueSon = new SoundBank(Audio, "Content/Musiques/Sound Bank.xsb");
            BanqueWave = new WaveBank(Audio, "Content/Musiques/Wave Bank.xwb");
            Config.musique_menu = BanqueSon.GetCue("menu");
            son_menu_curseur = "menu_curseur";
            son_menu_validation = "menu_validation";
            son_menu_retour = "menu_retour";
            Config.musique_menu.Play();
            Audio.Update();

            compteur_decalage = 0;

            return base.Init();
        }

        public override void Shutdown()
        {
            base.Shutdown();
        }

        public override void Update(GameTime gameTime)
        {
            Audio.Update();
            clavier = Keyboard.GetState();

            if (clavier_prec != clavier && clavier.IsKeyDown(Keys.Escape))
            {
                if (!Config.pause)
                {
                    BanqueSon.GetCue(son_menu_retour).Play();
                    Gestion_Ecran.Goto_Ecran("Quitter");
                    this.Shutdown();
                }
                else
                {
                    BanqueSon.GetCue(son_menu_retour).Play();
                    Config.musique_menu.Stop(AudioStopOptions.Immediate);
                    Config.musique_menu.Dispose();
                    Gestion_Ecran.Goto_Ecran("Jeu");
                    this.Shutdown();
                }
            }
            // Permet de jouer si le joueur appui sur la touche Entrer
            if (clavier_prec != clavier && clavier.IsKeyDown(Keys.Enter))
            {
                switch (curseur)
                {
                    case 0:
                        BanqueSon.GetCue(son_menu_validation).Play();
                        Gestion_Ecran.Goto_Ecran("Nouvelle Partie");
                        this.Shutdown();
                        break;
                    case 1:
                        BanqueSon.GetCue(son_menu_validation).Play();
                        Config.musique_menu.Stop(AudioStopOptions.Immediate);
                        Config.musique_menu.Dispose();
                        Gestion_Ecran.Goto_Ecran("Jeu");
                        this.Shutdown();
                        break;
                    case 2:
                        BanqueSon.GetCue(son_menu_validation).Play();
                        Gestion_Ecran.Goto_Ecran("Options");
                        this.Shutdown();
                        break;
                    case 3:
                        BanqueSon.GetCue(son_menu_validation).Play();
                        Gestion_Ecran.Goto_Ecran("Credits");
                        this.Shutdown();
                        break;
                    case 4:
                        BanqueSon.GetCue(son_menu_retour).Play();
                        Gestion_Ecran.Goto_Ecran("Quitter");
                        this.Shutdown();
                        break;
                }

            }

            if (clavier_prec != clavier && clavier.IsKeyDown(Keys.Down))
            {
                BanqueSon.GetCue(son_menu_curseur).Play();
                if(!Config.pause && curseur == 0)
                    curseur = (curseur + 2) % menu.Length;
                else
                    curseur = (curseur + 1) % menu.Length;
            }
            else if (clavier_prec != clavier && clavier.IsKeyDown(Keys.Up))
            {
                BanqueSon.GetCue(son_menu_curseur).Play();
                if (curseur <= 0)
                    curseur = menu.Length - 1;
                else
                {
                    if (curseur == 2 && !Config.pause)
                        curseur -= 2;
                    else
                        curseur--;
                }
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
            float scale = 0.7f + pulsation * 0.05f * pulsation_diminution;

            graphics.GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();
            Rectangle rec_banniere = new Rectangle((graphics.GraphicsDevice.Viewport.Width/ 2) - banniere.Width / 2, graphics.GraphicsDevice.Viewport.Height / 10, banniere.Width, banniere.Height);
            
            //spriteBatch.DrawString(texte, nom, new Vector2((graphics.GraphicsDevice.Viewport.Width / 2) - (texte.MeasureString(nom).X / 2), 50), Color.Gray, 0, Vector2.Zero, 1, SpriteEffects.None, 0);

            spriteBatch.Draw(banniere, rec_banniere, Color.White);

            compteur_decalage = 50;
            for (int i = 0; i < menu.Length; i++)
            {
                if (Config.pause)
                {
                    if (curseur == i)
                        spriteBatch.DrawString(texte, menu[i], new Vector2((graphics.GraphicsDevice.Viewport.Width / 2) - (texte.MeasureString(menu[i]).X / 2) * scale, graphics.GraphicsDevice.Viewport.Height / 2 - texte.MeasureString(menu[i]).Y * scale + 2 * texte.MeasureString(menu[i]).Y / 2 * i + 50), Color.Green, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
                    else
                        spriteBatch.DrawString(texte, menu[i], new Vector2((graphics.GraphicsDevice.Viewport.Width / 2) - (texte.MeasureString(menu[i]).X / 2) * 0.4f, graphics.GraphicsDevice.Viewport.Height / 2 - texte.MeasureString(menu[i]).Y * 0.4f + 2 * texte.MeasureString(menu[i]).Y / 2 * i + 50), Color.Gray, 0, Vector2.Zero, 0.4f, SpriteEffects.None, 0);
                }
                else
                {
                    if (i != 1)
                    {
                        if (curseur == i)
                            spriteBatch.DrawString(texte, menu[i], new Vector2((graphics.GraphicsDevice.Viewport.Width / 2) - (texte.MeasureString(menu[i]).X / 2) * scale, graphics.GraphicsDevice.Viewport.Height / 2 - texte.MeasureString(menu[i]).Y * scale + 2 * texte.MeasureString(menu[i]).Y / 2 * i + compteur_decalage), Color.Green, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
                        else
                            spriteBatch.DrawString(texte, menu[i], new Vector2((graphics.GraphicsDevice.Viewport.Width / 2) - (texte.MeasureString(menu[i]).X / 2) * 0.4f, graphics.GraphicsDevice.Viewport.Height / 2 - texte.MeasureString(menu[i]).Y * 0.4f + 2 * texte.MeasureString(menu[i]).Y / 2 * i + compteur_decalage), Color.Gray, 0, Vector2.Zero, 0.4f, SpriteEffects.None, 0);
                        compteur_decalage = 0;
                    }
                }
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}