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
    class Nouvelle_Partie : Ecran
    {
        private string[] menu;
        private int compteur_touche;
        private int curseur;
        private Texture2D drstrangelove;
        private Texture2D monstre;
        private Texture2D cerveau;
        private SpriteFont texte;
        private SpriteBatch spriteBatch;
        private KeyboardState clavier;
        private KeyboardState clavier_prec;

        private string mort_subite;

        float pulsation_diminution;

        // Sons et musiques
        private AudioEngine Audio;
        private SoundBank BanqueSon;
        private WaveBank BanqueWave;
        private string son_menu_curseur;
        private string son_menu_validation;
        private string son_menu_retour;

        public Nouvelle_Partie(GraphicsDeviceManager graphics, ContentManager Content)
            : base(graphics, Content, "Nouvelle Partie")
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

            Afficher_Ecran();
            compteur_touche = 0;
            curseur = 0;
            clavier = new KeyboardState();
            clavier_prec = Keyboard.GetState();

            spriteBatch = new SpriteBatch(graphics.GraphicsDevice);
            texte = Content.Load<SpriteFont>("Polices/menu");
            drstrangelove = Content.Load<Texture2D>("Images/Dr Strangelove");
            monstre = Content.Load<Texture2D>("Images/monstre");
            cerveau = Content.Load<Texture2D>("Images/cerveau");

            // Sons et musiques
            Audio = new AudioEngine("Content/Musiques/ambiance.xgs");
            // Chargement des banques
            BanqueSon = new SoundBank(Audio, "Content/Musiques/Sound Bank.xsb");
            BanqueWave = new WaveBank(Audio, "Content/Musiques/Wave Bank.xwb");
            son_menu_curseur = "menu_curseur";
            son_menu_validation = "menu_validation";
            son_menu_retour = "menu_retour";
            Audio.Update();

            return base.Init();
        }

        public override void Shutdown()
        {
            base.Shutdown();
        }

        public override void Update(GameTime gameTime)
        {
            if (!Config.active_mort_subite)
                mort_subite = "Aucune";
            else if (Config.sec_mort_subite == 0 && Config.min_mort_subite == 0)
                mort_subite = "Immédiate";
            else
                mort_subite = (Config.min_mort_subite) + " min " + (Config.sec_mort_subite) + " s";

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
                    if (clavier_prec != clavier && clavier.IsKeyDown(Keys.Enter))
                    {  
                        BanqueSon.GetCue(son_menu_validation).Play();
                        Config.pause = false;
                        Config.musique_menu.Stop(AudioStopOptions.Immediate);
                        Config.musique_menu.Dispose();
                        Gestion_Ecran.Goto_Ecran("Jeu");
                    }
                    
                    this.Shutdown();
                    break;
                case 1:
                    if (clavier_prec != clavier && (clavier.IsKeyDown(Keys.Enter) || clavier.IsKeyDown(Keys.Right)))
                    {
                        BanqueSon.GetCue(son_menu_validation).Play();
                        Config.modedejeu = !Config.modedejeu;
                    }
            
                    if (clavier_prec != clavier && clavier.IsKeyDown(Keys.Left))
                    {
                        BanqueSon.GetCue(son_menu_validation).Play();
                        Config.modedejeu = !Config.modedejeu;
                    }                    
                    Afficher_Ecran();
                    break;

                case 2:
                    if (clavier_prec != clavier && (clavier.IsKeyDown(Keys.Enter) || clavier.IsKeyDown(Keys.Right)))
                    {
                        BanqueSon.GetCue(son_menu_validation).Play();
                        Config.index_difficulte = (Config.index_difficulte + 1) % Config.difficulte.Length;
                    }
                    if (clavier_prec != clavier && clavier.IsKeyDown(Keys.Left))
                    {
                        BanqueSon.GetCue(son_menu_validation).Play();
                        Config.index_difficulte = (Config.index_difficulte - 1);
                        if (Config.index_difficulte==-1)
                        {
                            Config.index_difficulte = 2;
                        }
                    }
                    Afficher_Ecran();
                    break;

                case 3:
                    if (clavier_prec != clavier && (clavier.IsKeyDown(Keys.Enter)))
                    {
                        BanqueSon.GetCue(son_menu_validation).Play();
                        Gestion_Ecran.Goto_Ecran("Bonus");
                        this.Shutdown();
                    }
                    break;

                case 4:
                    if (clavier_prec != clavier && (clavier.IsKeyDown(Keys.Enter)))
                    {
                        BanqueSon.GetCue(son_menu_validation).Play();
                        Gestion_Ecran.Goto_Ecran("Pièges");
                        this.Shutdown();
                    }
                    break;

                case 5:
                    if (clavier_prec != clavier && (clavier.IsKeyDown(Keys.Enter) || clavier.IsKeyDown(Keys.Right)))
                    {
                        BanqueSon.GetCue(son_menu_validation).Play();
                        Config.laby_algo = !Config.laby_algo;
                    }
                        if (clavier_prec != clavier && clavier.IsKeyDown(Keys.Left))
                        {
                            BanqueSon.GetCue(son_menu_validation).Play();
                            Config.laby_algo = !Config.laby_algo;
                        }

                    Afficher_Ecran();
                    
                    break;

                case 6:
                    if ((clavier_prec != clavier || compteur_touche > 30) && (clavier.IsKeyDown(Keys.Enter) || clavier.IsKeyDown(Keys.Right)))
                    Config.hauteur_labyrinthe++;
                    if (Config.hauteur_labyrinthe > 100)
                        Config.hauteur_labyrinthe = 10;
                    if ((clavier_prec != clavier || compteur_touche > 30) && clavier.IsKeyDown(Keys.Left))
                         Config.hauteur_labyrinthe--;
                     if (Config.hauteur_labyrinthe < 10)
                         Config.hauteur_labyrinthe = 100;
                     Afficher_Ecran();
                    break;

                case 7:
                    if ((clavier_prec != clavier || compteur_touche > 30) && (clavier.IsKeyDown(Keys.Enter) || clavier.IsKeyDown(Keys.Right)))
                        Config.largeur_labyrinthe++;
                    if (Config.largeur_labyrinthe > 100)
                        Config.largeur_labyrinthe = 10;
                    if ((clavier_prec != clavier || compteur_touche > 30) && clavier.IsKeyDown(Keys.Left))
                        Config.largeur_labyrinthe--;
                    if (Config.largeur_labyrinthe < 10)
                        Config.largeur_labyrinthe = 100;
                    Afficher_Ecran();
                    break;

                case 8:
                    if ((clavier_prec != clavier || compteur_touche > 30) && (clavier.IsKeyDown(Keys.Enter) || clavier.IsKeyDown(Keys.Right)))
                    {
                        Config.sec_mort_subite++;
                        if (Config.sec_mort_subite >= 60)
                        {
                            Config.min_mort_subite++;
                            Config.sec_mort_subite = 0;
                        }
                        if (Config.active_mort_subite && Config.min_mort_subite >= 10)
                        {
                            Config.active_mort_subite = false;
                        }
                        else if (!Config.active_mort_subite)
                        {
                            Config.active_mort_subite = true;
                            Config.min_mort_subite = 0;
                            Config.sec_mort_subite = 0;
                        }
                    }
                    if ((clavier_prec != clavier || compteur_touche > 30) && clavier.IsKeyDown(Keys.Left))
                    {
                        Config.sec_mort_subite--;
                        if (Config.sec_mort_subite <= 0 && Config.min_mort_subite > 0)
                        {
                            Config.sec_mort_subite = 59;
                            Config.min_mort_subite--;
                        }
                        if (Config.active_mort_subite && Config.min_mort_subite == 0 && Config.sec_mort_subite < 0)
                        {
                            Config.active_mort_subite = false;
                        }
                        else if (!Config.active_mort_subite)
                        {
                            Config.active_mort_subite = true;
                            Config.min_mort_subite = 9;
                            Config.sec_mort_subite = 59;
                        }
                        
                    }
                    Afficher_Ecran();
                    break;

                case 9:
                    if (clavier_prec != clavier && clavier.IsKeyDown(Keys.Enter))
                    {
                        BanqueSon.GetCue(son_menu_retour).Play();
                        Gestion_Ecran.Goto_Ecran("Menu Principal");
                        this.Shutdown();
                    }
                    break;
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

            Rectangle rectangledr = new Rectangle((graphics.GraphicsDevice.Viewport.Width / 2)+150, (graphics.GraphicsDevice.Viewport.Height / 2)-150, 150, 400);
            Rectangle rectanglemonstre = new Rectangle((graphics.GraphicsDevice.Viewport.Width / 2) - 450, (graphics.GraphicsDevice.Viewport.Height / 2) - 220, 400, 500);
            Rectangle rectanglecerveau = new Rectangle((graphics.GraphicsDevice.Viewport.Width / 2)-70 , (graphics.GraphicsDevice.Viewport.Height / 2)-200, 150, 100);
            Rectangle rectangle2cerveau = new Rectangle((graphics.GraphicsDevice.Viewport.Width / 2) +50, (graphics.GraphicsDevice.Viewport.Height / 2) - 200, 150, 100);
            Rectangle rectangle3cerveau = new Rectangle((graphics.GraphicsDevice.Viewport.Width / 2) - 190, (graphics.GraphicsDevice.Viewport.Height / 2) - 200, 150, 100);

            if (Config.modedejeu==true)
            {
                spriteBatch.Draw(drstrangelove, rectangledr, Color.White);
            }
            else
            {
                spriteBatch.Draw(monstre, rectanglemonstre, Color.White);
            }
            if (Config.index_difficulte==0)
            {
                spriteBatch.Draw(cerveau, rectanglecerveau, Color.White);
            }
            if (Config.index_difficulte==1)
            {
                spriteBatch.Draw(cerveau, rectanglecerveau, Color.White);
                spriteBatch.Draw(cerveau, rectangle2cerveau, Color.White);
            }
            if (Config.index_difficulte == 2)
            {
                spriteBatch.Draw(cerveau, rectanglecerveau, Color.White);
                spriteBatch.Draw(cerveau, rectangle2cerveau, Color.White);
                spriteBatch.Draw(cerveau, rectangle3cerveau, Color.White);
            }

            spriteBatch.DrawString(texte, nom, new Vector2((graphics.GraphicsDevice.Viewport.Width / 2) - (texte.MeasureString(nom).X / 2), 50), Color.Gray, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            for (int i = 0; i < menu.Length; i++)
            {
                if (curseur == i)
                    spriteBatch.DrawString(texte, menu[i], new Vector2((graphics.GraphicsDevice.Viewport.Width / 2) - (texte.MeasureString(menu[i]).X / 2) * scale, graphics.GraphicsDevice.Viewport.Height / 2 - texte.MeasureString(menu[i]).Y * scale + texte.MeasureString(menu[i]).Y / 2 * i), Color.Green, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
                else
                    spriteBatch.DrawString(texte, menu[i], new Vector2((graphics.GraphicsDevice.Viewport.Width / 2) - (texte.MeasureString(menu[i]).X / 2) * 0.4f, graphics.GraphicsDevice.Viewport.Height / 2 - texte.MeasureString(menu[i]).Y * 0.4f + texte.MeasureString(menu[i]).Y / 2 * i), Color.Gray, 0, Vector2.Zero, 0.4f, SpriteEffects.None, 0);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
        public void Afficher_Ecran()
        {
            menu = new string[] {
                "Jouer", 
                "Mode de jeu: " + (Config.modedejeu? "Chassé" : "Chasseur"), 
                "Difficulté: " + (Config.difficulte[Config.index_difficulte]),
                "Bonus",
                "Pièges",
                "Labyrinthe: " + (Config.laby_algo? "Parfait" : "Imparfait"),
                "Longueur: " + (Config.hauteur_labyrinthe),
                "Largeur: " + (Config.largeur_labyrinthe),
                "Mort subite: " + mort_subite,
                "Retour"
            };
        }
    }
}