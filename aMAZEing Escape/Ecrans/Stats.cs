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
    class Stats : Ecran
    {

        private Texture2D dr;
        private Texture2D monstre;

        private string mort_subite;
        private string[] menu;
        private int curseur;
        private SpriteFont texte;
        private SpriteBatch spriteBatch;
        private KeyboardState clavier;
        private KeyboardState clavier_prec;

        // Sons et musiques
        private AudioEngine Audio;
        private SoundBank BanqueSon;
        private WaveBank BanqueWave;
        private string son_stats;
        private string son_menu_curseur;
        private string son_menu_validation;

        private float compteur;

        public bool afficher_tems_ecoule;
        public bool afficher_mode_de_jeu;
        public bool afficher_taille;
        public bool afficher_mort_subite;
        public bool afficher_difficulte;
        public bool afficher_cause_fin;
        
        float pulsation_diminution;

        public Stats(GraphicsDeviceManager graphics, ContentManager Content)
            : base(graphics, Content, "Statistiques")
        {
        }

        public override bool Init()
        {
            if (!Config.active_mort_subite)
                mort_subite = "Aucune";
            else if (Config.sec_mort_subite == 0 && Config.min_mort_subite == 0)
                mort_subite = "Immédiate";
            else
                mort_subite = (Config.min_mort_subite) + " min " + (Config.sec_mort_subite) + " s";

            compteur = 5;

            curseur = 0;
            clavier = new KeyboardState();
            clavier_prec = Keyboard.GetState();

            spriteBatch = new SpriteBatch(graphics.GraphicsDevice);
            texte = Content.Load<SpriteFont>("Polices/menu");

            dr = Content.Load<Texture2D>("Images/Dr Strangelove");
            monstre = Content.Load<Texture2D>("Images/monstre");
            // Sons et musiques
            Audio = new AudioEngine("Content/Musiques/ambiance.xgs");
            // Chargement des banques
            BanqueSon = new SoundBank(Audio, "Content/Musiques/Sound Bank.xsb");
            BanqueWave = new WaveBank(Audio, "Content/Musiques/Wave Bank.xwb");
            son_stats = "stats";
            son_menu_validation = "menu_validation";
            son_menu_curseur = "menu_curseur";
            

            afficher_tems_ecoule = false;
            afficher_mode_de_jeu = false;
            afficher_taille = false;
            afficher_mort_subite = false;
            afficher_difficulte = false;
            afficher_cause_fin = false;

            Afficher_Ecran();
            return base.Init();
        }

        public override void Shutdown()
        {
            base.Shutdown();
        }

        public override void Update(GameTime gameTime)
        {

            clavier = Keyboard.GetState();

            if (clavier_prec != clavier && clavier.IsKeyDown(Keys.Enter))
            {
                BanqueSon.GetCue(son_menu_validation).Play();
                switch (curseur)
                {
                    case 0:
                        Gestion_Ecran.Goto_Ecran("Jeu");
                        this.Shutdown();
                        break;
                    case 1:
                        Gestion_Ecran.Goto_Ecran("Menu Principal");
                        this.Shutdown();
                        break;
                }
            }

            if (compteur <= 5 && !afficher_tems_ecoule)
            {
                afficher_tems_ecoule = true;
                BanqueSon.PlayCue(son_stats);
            }
            if (compteur <= 4 && !afficher_taille)
            {
                afficher_taille = true;
                BanqueSon.PlayCue(son_stats);
            }
            if (compteur <= 3 && !afficher_difficulte)
            {
                afficher_difficulte = true;
                BanqueSon.PlayCue(son_stats);
            }
            if (compteur <= 2 && !afficher_mort_subite)
            {
                afficher_mort_subite = true;
                BanqueSon.PlayCue(son_stats);
            }
            if (compteur <= 1 && !afficher_mode_de_jeu)
            {
                afficher_mode_de_jeu = true;
                BanqueSon.PlayCue(son_stats);
            }
            if (compteur <= 0 && !afficher_cause_fin)
            {
                afficher_cause_fin = true;
                BanqueSon.PlayCue(son_stats);
            }

            compteur -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Déplacement du curseur
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
            spriteBatch.DrawString(texte, nom, new Vector2((graphics.GraphicsDevice.Viewport.Width / 2) - (texte.MeasureString(nom).X / 2), 50), Color.Gray, 0, Vector2.Zero, 1, SpriteEffects.None, 0);

            if(afficher_tems_ecoule)
                spriteBatch.DrawString(texte, "Temps écoulé: " + (Config.chrono), new Vector2((graphics.GraphicsDevice.Viewport.Width / 2) - 100, (graphics.GraphicsDevice.Viewport.Height/2) - 150), Color.Gray , 0, Vector2.Zero, 0.6f, SpriteEffects.None, 0);
            if (afficher_taille)
                spriteBatch.DrawString(texte, "Taille: " + (Config.largeur_labyrinthe) + "x" + (Config.hauteur_labyrinthe), new Vector2((graphics.GraphicsDevice.Viewport.Width / 2) - 100, (graphics.GraphicsDevice.Viewport.Height)/2 - 110), Color.Gray, 0, Vector2.Zero, 0.6f, SpriteEffects.None, 0);
            if (afficher_difficulte)
                spriteBatch.DrawString(texte, "Difficulté: " + (Config.difficulte[Config.index_difficulte]), new Vector2((graphics.GraphicsDevice.Viewport.Width / 2) - 100, (graphics.GraphicsDevice.Viewport.Height)/2 - 70), Color.Gray, 0, Vector2.Zero, 0.6f, SpriteEffects.None, 0);
            if (afficher_mort_subite)
                spriteBatch.DrawString(texte, "Mort subite: " + mort_subite , new Vector2((graphics.GraphicsDevice.Viewport.Width / 2) - 100, (graphics.GraphicsDevice.Viewport.Height)/2 - 30), Color.Gray, 0, Vector2.Zero, 0.6f, SpriteEffects.None, 0);
            if(afficher_mode_de_jeu)
                spriteBatch.DrawString(texte, "Type: " + (Config.laby_algo ? "Parfait" : "Imparfait"), new Vector2((graphics.GraphicsDevice.Viewport.Width / 2) - 100, (graphics.GraphicsDevice.Viewport.Height/2) + 10 ), Color.Gray, 0, Vector2.Zero, 0.6f, SpriteEffects.None, 0);
            if (afficher_cause_fin)
                spriteBatch.DrawString(texte, "Cause de fin de jeu: \n" + Config.cause_fin_jeu, new Vector2((graphics.GraphicsDevice.Viewport.Width / 2) - 100, (graphics.GraphicsDevice.Viewport.Height / 2) + 50), Color.Gray, 0, Vector2.Zero, 0.6f, SpriteEffects.None, 0);
                
            for (int i = 0; i < menu.Length; i++)
            {
                if (curseur == i)
                    spriteBatch.DrawString(texte, menu[i], new Vector2((graphics.GraphicsDevice.Viewport.Width / 2) - (texte.MeasureString(menu[i]).X / 2) * scale, graphics.GraphicsDevice.Viewport.Height / 2 - texte.MeasureString(menu[i]).Y * scale + texte.MeasureString(menu[i]).Y / 2 * i + 200f), Color.Green, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
                else
                    spriteBatch.DrawString(texte, menu[i], new Vector2((graphics.GraphicsDevice.Viewport.Width / 2) - (texte.MeasureString(menu[i]).X / 2) * 0.4f, graphics.GraphicsDevice.Viewport.Height / 2 - texte.MeasureString(menu[i]).Y * 0.4f + texte.MeasureString(menu[i]).Y / 2 * i + 200f), Color.Gray, 0, Vector2.Zero, 0.4f, SpriteEffects.None, 0);
            }
            
            Rectangle recdr = new Rectangle((graphics.GraphicsDevice.Viewport.Width / 2) + 150, (graphics.GraphicsDevice.Viewport.Height / 2) - 150, 150, 400);
            Rectangle recmonstre = new Rectangle((graphics.GraphicsDevice.Viewport.Width / 2) - 450, (graphics.GraphicsDevice.Viewport.Height / 2) - 220, 400, 500);
            if (Config.modedejeu == true)
                spriteBatch.Draw(dr, recdr, Color.White);
            else
                spriteBatch.Draw(monstre, recmonstre, Color.White);

            spriteBatch.End();

             
            base.Draw(gameTime);
        }

        public void Afficher_Ecran()
        {
            menu = new string[] 
            { 
                "Rejouer",
                "Quitter" 
            };
        }
    }
}