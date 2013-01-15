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
    class Options : Ecran
    {
        private string[] menu;

        private int curseur;
        private int compteur_touche;

        // Sons et musiques
        private AudioEngine Audio;
        private SoundBank BanqueSon;
        private WaveBank BanqueWave;
        private string son_menu_curseur;
        private string son_menu_retour;
        private string son_menu_validation;
        private string son_menu_bonus;
        private Cue musique_ambiance;
 
        private SpriteFont texte;
        private SpriteBatch spriteBatch;

        private Texture2D rectangle;
        private Texture2D barre;

        private KeyboardState clavier;
        private KeyboardState clavier_prec;

        float pulsation_diminution;
     
        public Options(GraphicsDeviceManager graphics, ContentManager Content)
            : base(graphics, Content, "Options")
        {
        }

        public override bool Init()
        {

            Afficher_Ecran();

            compteur_touche = 0;
            curseur = 0;
            clavier = new KeyboardState();
            clavier_prec = Keyboard.GetState();

            spriteBatch = new SpriteBatch(graphics.GraphicsDevice);
            texte = Content.Load<SpriteFont>("Polices/menu");

            rectangle = Content.Load<Texture2D>("Images/rectangle_volume");
            barre = Content.Load<Texture2D>("Images/barre_volume");

            // Sons et musiques
            Audio = new AudioEngine("Content/Musiques/ambiance.xgs");
            // Chargement des banques
            BanqueSon = new SoundBank(Audio, "Content/Musiques/Sound Bank.xsb");
            BanqueWave = new WaveBank(Audio, "Content/Musiques/Wave Bank.xwb");
            son_menu_curseur = "menu_curseur";
            son_menu_retour = "menu_retour";
            son_menu_validation = "menu_validation";
            son_menu_bonus = "menu_refuser";
            musique_ambiance = BanqueSon.GetCue("ambiance_labyrinthe");

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
                this.Shutdown();
            }

            if (clavier_prec == clavier && clavier.GetPressedKeys().Length > 0 && clavier.GetPressedKeys()[0] != Keys.None)
                compteur_touche += 1;
            else
                compteur_touche = 0;

            switch (curseur)
            {
                case 0:
                    if (clavier_prec != clavier && (clavier.IsKeyDown(Keys.Right) || clavier.IsKeyDown(Keys.Enter)))
                    {
                        BanqueSon.GetCue(son_menu_validation).Play();
                        aMAZEing_Escape.Properties.Settings.Default.index_resolution = (aMAZEing_Escape.Properties.Settings.Default.index_resolution + 1) % ((Config.resolutions.Length / 2));
                        graphics.PreferredBackBufferWidth = Config.resolutions[aMAZEing_Escape.Properties.Settings.Default.index_resolution, 0];
                        graphics.PreferredBackBufferHeight = Config.resolutions[aMAZEing_Escape.Properties.Settings.Default.index_resolution, 1];
                        graphics.ApplyChanges();
                        Afficher_Ecran();
                    }
                    else if (clavier_prec != clavier && clavier.IsKeyDown(Keys.Left))
                    {
                        BanqueSon.GetCue(son_menu_validation).Play();
                        if (aMAZEing_Escape.Properties.Settings.Default.index_resolution <= 0)
                            aMAZEing_Escape.Properties.Settings.Default.index_resolution = (Config.resolutions.Length / 2) - 1;
                        else
                            aMAZEing_Escape.Properties.Settings.Default.index_resolution--;

                        
                        graphics.PreferredBackBufferWidth = Config.resolutions[aMAZEing_Escape.Properties.Settings.Default.index_resolution, 0];
                        graphics.PreferredBackBufferHeight = Config.resolutions[aMAZEing_Escape.Properties.Settings.Default.index_resolution, 1];
                        graphics.ApplyChanges();
                        Afficher_Ecran();
                    }
                   
                    break;
                case 1:
                    if (clavier_prec != clavier && clavier.IsKeyDown(Keys.Enter))
                    {
                        BanqueSon.GetCue(son_menu_validation).Play();
                        Gestion_Ecran.Goto_Ecran("Configuration des touches");
                        this.Shutdown();
                    }
                    break;
                case 2:
                    if (Config.musique_menu.IsPaused)
                        Config.musique_menu.Resume();
                    if (musique_ambiance.IsPlaying)
                    {
                        musique_ambiance.Stop(AudioStopOptions.Immediate);
                        musique_ambiance.Dispose();
                    }
                    if ((clavier_prec != clavier || compteur_touche > 30) && clavier.IsKeyDown(Keys.Left))
                    {
                        BanqueSon.GetCue(son_menu_validation).Play();
                        if (aMAZEing_Escape.Properties.Settings.Default.sensibilite_souris <= 1)
                            aMAZEing_Escape.Properties.Settings.Default.sensibilite_souris = 20;
                        else
                            aMAZEing_Escape.Properties.Settings.Default.sensibilite_souris--;
                    }
                    else if ((clavier_prec != clavier || compteur_touche > 30) && (clavier.IsKeyDown(Keys.Right)||clavier.IsKeyDown(Keys.Enter)))
                    {
                        BanqueSon.GetCue(son_menu_validation).Play();
                        if (aMAZEing_Escape.Properties.Settings.Default.sensibilite_souris >= 20)
                            aMAZEing_Escape.Properties.Settings.Default.sensibilite_souris = 1;
                        else
                            aMAZEing_Escape.Properties.Settings.Default.sensibilite_souris++;
                    }
                    Afficher_Ecran();
                    break;
                case 3:
                    if (Config.musique_menu.IsPlaying)
                        Config.musique_menu.Pause();

                    if (!musique_ambiance.IsPlaying)
                    {
                        musique_ambiance = BanqueSon.GetCue("ambiance_labyrinthe");
                        musique_ambiance.Play();
                    }
                    if ((clavier_prec != clavier || compteur_touche > 30) && clavier.IsKeyDown(Keys.Right))
                    {
                        aMAZEing_Escape.Properties.Settings.Default.volume_musiques += 0.01f;
                        if (aMAZEing_Escape.Properties.Settings.Default.volume_musiques > 1)
                            aMAZEing_Escape.Properties.Settings.Default.volume_musiques = 0;
                        Audio.GetCategory("Musiques").SetVolume(aMAZEing_Escape.Properties.Settings.Default.volume_musiques);
                    }
                    if ((clavier_prec != clavier || compteur_touche > 30) && clavier.IsKeyDown(Keys.Left))
                    {
                        aMAZEing_Escape.Properties.Settings.Default.volume_musiques -= 0.01f;
                        if (aMAZEing_Escape.Properties.Settings.Default.volume_musiques < 0)
                            aMAZEing_Escape.Properties.Settings.Default.volume_musiques = 1;
                        Audio.GetCategory("Musiques").SetVolume(aMAZEing_Escape.Properties.Settings.Default.volume_musiques);
                    }
                    break;

                case 4:
                    if (Config.musique_menu.IsPaused)
                        Config.musique_menu.Resume();

                    if (musique_ambiance.IsPlaying)
                    {
                        musique_ambiance.Stop(AudioStopOptions.Immediate);
                        musique_ambiance.Dispose();
                    }
                    if ((clavier_prec != clavier || compteur_touche > 30) && clavier.IsKeyDown(Keys.Right))
                    {
                        aMAZEing_Escape.Properties.Settings.Default.volume_sons += 0.01f;
                        if (aMAZEing_Escape.Properties.Settings.Default.volume_sons > 1)
                        {
                            aMAZEing_Escape.Properties.Settings.Default.volume_sons = 0;
                        }
                    }
                    if ((clavier_prec != clavier || compteur_touche > 30) && clavier.IsKeyDown(Keys.Left))
                    {
                        aMAZEing_Escape.Properties.Settings.Default.volume_sons -= 0.01f;
                        if (aMAZEing_Escape.Properties.Settings.Default.volume_sons < 0)
                        {
                            aMAZEing_Escape.Properties.Settings.Default.volume_sons = 1;

                        }
                    }

                    if ((clavier.IsKeyDown(Keys.Right) || clavier.IsKeyDown(Keys.Left) && ((compteur_touche > 30 && ((int)(aMAZEing_Escape.Properties.Settings.Default.volume_sons * 100)) % 5 == 0) || (clavier_prec != clavier))))
                    {
                        Audio.GetCategory("Sons").SetVolume(aMAZEing_Escape.Properties.Settings.Default.volume_sons);
                        BanqueSon.GetCue(son_menu_bonus).Play();
                    }

                    break;
                case 5:
                    if (((clavier_prec != clavier) || (compteur_touche > 30)) && (clavier.IsKeyDown(Keys.Right) || clavier.IsKeyDown(Keys.Enter)))
                    {
                        if (aMAZEing_Escape.Properties.Settings.Default.densite_brouillard >= 1)
                            aMAZEing_Escape.Properties.Settings.Default.densite_brouillard = 0;
                        else
                            aMAZEing_Escape.Properties.Settings.Default.densite_brouillard += 0.01f;
                    }
                    else if ((clavier_prec != clavier || (compteur_touche > 30)) && clavier.IsKeyDown(Keys.Left))
                    {
                        if (aMAZEing_Escape.Properties.Settings.Default.densite_brouillard <= 0)
                            aMAZEing_Escape.Properties.Settings.Default.densite_brouillard = 1;
                        else
                            aMAZEing_Escape.Properties.Settings.Default.densite_brouillard -= 0.01f;
                    }
                    Afficher_Ecran();
                    break;
                case 6:
                    if (clavier_prec != clavier && clavier.IsKeyDown(Keys.Enter))
                    {
                        BanqueSon.GetCue(son_menu_validation).Play();
                        aMAZEing_Escape.Properties.Settings.Default.plein_ecran = !aMAZEing_Escape.Properties.Settings.Default.plein_ecran;
                        graphics.IsFullScreen = aMAZEing_Escape.Properties.Settings.Default.plein_ecran;
                        Afficher_Ecran();
                        graphics.ApplyChanges();     
                    }
                    
                    break;
                case 7:
                    if (clavier_prec != clavier && clavier.IsKeyDown(Keys.Enter))
                    {
                        BanqueSon.GetCue(son_menu_retour).Play();
                        Gestion_Ecran.Goto_Ecran("Menu Principal");
                        this.Shutdown();
                    }
                    break;
                default:
                    break;
            }


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

            for (int i = 0; i < menu.Length; i++)
            {
                if (curseur == i)
                    spriteBatch.DrawString(texte, menu[i], new Vector2((graphics.GraphicsDevice.Viewport.Width / 2) - (texte.MeasureString(menu[i]).X / 2) * scale, graphics.GraphicsDevice.Viewport.Height / 2 - texte.MeasureString(menu[i]).Y * scale + texte.MeasureString(menu[i]).Y * i - 100), Color.Green, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
                else
                    spriteBatch.DrawString(texte, menu[i], new Vector2((graphics.GraphicsDevice.Viewport.Width / 2) - (texte.MeasureString(menu[i]).X / 2) * 0.4f, graphics.GraphicsDevice.Viewport.Height / 2 - texte.MeasureString(menu[i]).Y * 0.4f + texte.MeasureString(menu[i]).Y * i - 100), Color.Gray, 0, Vector2.Zero, 0.4f, SpriteEffects.None, 0);
            }

            Rectangle rec_rectangle_musique = new Rectangle((graphics.GraphicsDevice.Viewport.Width / 2) + 115, (graphics.GraphicsDevice.Viewport.Height / 2) + 13, 150, 30);
            Rectangle rec_rectangle_effets = new Rectangle((graphics.GraphicsDevice.Viewport.Width / 2) + 115, (graphics.GraphicsDevice.Viewport.Height / 2) + 61, 150, 30);
            Rectangle rec_barre_musique = new Rectangle((graphics.GraphicsDevice.Viewport.Width / 2) + 120, (graphics.GraphicsDevice.Viewport.Height / 2) + 18, (int)(aMAZEing_Escape.Properties.Settings.Default.volume_musiques * 100 * 1.4f), 15);
            Rectangle rec_barre_effet = new Rectangle((graphics.GraphicsDevice.Viewport.Width / 2) + 120, (graphics.GraphicsDevice.Viewport.Height / 2) + 66, (int)(aMAZEing_Escape.Properties.Settings.Default.volume_sons * 100 * 1.4f), 15);

            spriteBatch.Draw(rectangle, rec_rectangle_musique, Color.White);
            spriteBatch.Draw(rectangle, rec_rectangle_effets, Color.White);
            spriteBatch.Draw(barre, rec_barre_musique, Color.White);
            spriteBatch.Draw(barre, rec_barre_effet, Color.White);

            spriteBatch.DrawString(texte, ((int)(aMAZEing_Escape.Properties.Settings.Default.volume_musiques * 100)).ToString() + "%", new Vector2((graphics.GraphicsDevice.Viewport.Width / 2) + 100 + 150 / 2, (graphics.GraphicsDevice.Viewport.Height / 2) + 17), Color.White, 0, Vector2.Zero, 0.4f, SpriteEffects.None, 0);
            spriteBatch.DrawString(texte, ((int)(aMAZEing_Escape.Properties.Settings.Default.volume_sons * 100)).ToString() + "%", new Vector2((graphics.GraphicsDevice.Viewport.Width / 2) + 100 + 150 / 2, (graphics.GraphicsDevice.Viewport.Height / 2) + 65.5f), Color.White, 0, Vector2.Zero, 0.4f, SpriteEffects.None, 0);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        public void Afficher_Ecran()
        {
            menu = new string[] { "Résolution: " + Config.resolutions[aMAZEing_Escape.Properties.Settings.Default.index_resolution, 0] + "x" + Config.resolutions[aMAZEing_Escape.Properties.Settings.Default.index_resolution, 1], 
                "Configuration des touches", 
                "Sensibilité souris : " + aMAZEing_Escape.Properties.Settings.Default.sensibilite_souris, 
                "Volume des musiques : ",
                "Volume des effets : ",
                "Densité du brouillard : " + (int)(aMAZEing_Escape.Properties.Settings.Default.densite_brouillard * 100) + "%", 
                aMAZEing_Escape.Properties.Settings.Default.plein_ecran ? "Mode fenêtré" : "Plein écran", 
                "Retour" };
        }
    }
}