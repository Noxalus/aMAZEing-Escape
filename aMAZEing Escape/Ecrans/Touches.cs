using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;

namespace aMAZEing_Escape
{
    class Touches : Ecran
    {
        private bool[] touches;

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
        private string son_menu_refuser;
        private string son_menu_retour;
        private string son_menu_curseur;
        private string son_menu_validation;

        private bool touche_prise;
        private float compteur_touche_prise;
        private string texte_touche_prise;
        private float opacite_touche_prise;

        float pulsation_diminution;
     
        public Touches(GraphicsDeviceManager graphics, ContentManager Content) : base(graphics, Content, "Configuration des touches")
        {
        }

        public override bool Init()
        {
            touches = new bool[6];
            curseur = 0;
            clavier = new KeyboardState();
            clavier_prec = Keyboard.GetState();

            spriteBatch = new SpriteBatch(graphics.GraphicsDevice);
            texte = Content.Load<SpriteFont>("Polices/menu");

            touche_prise = false;
            texte_touche_prise = "Cette touche est déjà prise !";
            opacite_touche_prise = 255;

            // Sons et musiques
            Audio = new AudioEngine("Content/Musiques/ambiance.xgs");
            // Chargement des banques
            BanqueSon = new SoundBank(Audio, "Content/Musiques/Sound Bank.xsb");
            BanqueWave = new WaveBank(Audio, "Content/Musiques/Wave Bank.xwb");
            son_menu_refuser = "menu_refuser";
            son_menu_retour = "menu_retour";
            son_menu_curseur = "menu_curseur";
            son_menu_validation = "menu_validation"; 

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

            if (clavier_prec != clavier && clavier.IsKeyDown(Keys.Escape))
            {
                BanqueSon.GetCue(son_menu_retour).Play();
                Gestion_Ecran.Goto_Ecran("Options");
            }
            this.Shutdown();

            switch (curseur)
            {
                case 0:
                    if (clavier_prec != clavier && !clavier.IsKeyDown(Keys.Enter) && !clavier.IsKeyDown(Keys.Escape) && touches[0] && clavier.GetPressedKeys().Length > 0)
                    {
                        if (Touche_Libre(clavier.GetPressedKeys()[0]))
                        {
                            BanqueSon.GetCue(son_menu_validation).Play();
                            aMAZEing_Escape.Properties.Settings.Default.avancer = clavier.GetPressedKeys()[0];
                            clavier_prec = Keyboard.GetState();
                            touches[0] = !touches[0];
                        }
                        else
                        {
                            BanqueSon.PlayCue(son_menu_refuser);
                            if(!touche_prise)
                                touche_prise = true;
                        }
                    }
                    else if (clavier_prec != clavier && clavier.IsKeyDown(Keys.Enter) && !clavier.IsKeyDown(Keys.Escape))
                        touches[0] = !touches[0];
                    Afficher_Ecran(); 
                    break;

                case 1:
                    if (clavier_prec != clavier && !clavier.IsKeyDown(Keys.Enter) && !clavier.IsKeyDown(Keys.Escape) && touches[1] && clavier.GetPressedKeys().Length > 0)
                    {
                        if (Touche_Libre(clavier.GetPressedKeys()[0]))
                        {
                            BanqueSon.GetCue(son_menu_validation).Play();
                            aMAZEing_Escape.Properties.Settings.Default.reculer = clavier.GetPressedKeys()[0];
                            clavier_prec = Keyboard.GetState();
                            touches[1] = !touches[1];
                        }
                        else
                        {
                            BanqueSon.PlayCue(son_menu_refuser);
                            if (!touche_prise)
                                touche_prise = true;
                        }
                    }
                    else if (clavier_prec != clavier && (clavier.IsKeyDown(Keys.Enter) || clavier.IsKeyDown(Keys.Escape)))
                        touches[1] = !touches[1];
                    Afficher_Ecran(); 
                    break;

                case 2:
                    if (clavier_prec != clavier && !clavier.IsKeyDown(Keys.Enter) && !clavier.IsKeyDown(Keys.Escape) && touches[2] && clavier.GetPressedKeys().Length > 0)
                    {
                        if (Touche_Libre(clavier.GetPressedKeys()[0]))
                        {
                            BanqueSon.GetCue(son_menu_validation).Play();
                            aMAZEing_Escape.Properties.Settings.Default.gauche = clavier.GetPressedKeys()[0];
                            clavier_prec = Keyboard.GetState();
                            touches[2] = !touches[2];
                        }
                        else
                        {
                            BanqueSon.PlayCue(son_menu_refuser);
                            if (!touche_prise)
                                touche_prise = true;
                        }
                    }
                    else if (clavier_prec != clavier && (clavier.IsKeyDown(Keys.Enter) || clavier.IsKeyDown(Keys.Escape)))
                        touches[2] = !touches[2];
                    Afficher_Ecran(); 
                    break;
                case 3:
                    if (clavier_prec != clavier && !clavier.IsKeyDown(Keys.Enter) && !clavier.IsKeyDown(Keys.Escape) && touches[3] && clavier.GetPressedKeys().Length > 0)
                    {
                        if (Touche_Libre(clavier.GetPressedKeys()[0]))
                        {
                            BanqueSon.GetCue(son_menu_validation).Play();
                            aMAZEing_Escape.Properties.Settings.Default.droite = clavier.GetPressedKeys()[0];
                            clavier_prec = Keyboard.GetState();
                            touches[3] = !touches[3];
                        }
                        else
                        {
                            BanqueSon.PlayCue(son_menu_refuser);
                            if (!touche_prise)
                                touche_prise = true;
                        }
                    }
                    else if (clavier_prec != clavier && (clavier.IsKeyDown(Keys.Enter) || clavier.IsKeyDown(Keys.Escape)))
                        touches[3] = !touches[3];
                    Afficher_Ecran();
                    break;

                case 4:
                    if (clavier_prec != clavier && !clavier.IsKeyDown(Keys.Enter) && !clavier.IsKeyDown(Keys.Escape) && touches[4] && clavier.GetPressedKeys().Length > 0)
                    {
                        if (Touche_Libre(clavier.GetPressedKeys()[0]))
                        {
                            BanqueSon.GetCue(son_menu_validation).Play();
                            aMAZEing_Escape.Properties.Settings.Default.sauter = clavier.GetPressedKeys()[0];
                            clavier_prec = Keyboard.GetState();
                            touches[4] = !touches[4];
                        }
                        else
                        {
                            BanqueSon.PlayCue(son_menu_refuser);
                            if (!touche_prise)
                                touche_prise = true;
                        }
                    }
                    else if (clavier_prec != clavier && (clavier.IsKeyDown(Keys.Enter) || clavier.IsKeyDown(Keys.Escape)))
                        touches[4] = !touches[4];
                    Afficher_Ecran();
                    break;

                case 5:
                    if (clavier_prec != clavier && !clavier.IsKeyDown(Keys.Enter) && !clavier.IsKeyDown(Keys.Escape) && touches[5] && clavier.GetPressedKeys().Length > 0)
                    {
                        if (Touche_Libre(clavier.GetPressedKeys()[0]))
                        {
                            BanqueSon.GetCue(son_menu_validation).Play();
                            aMAZEing_Escape.Properties.Settings.Default.sebaisser= clavier.GetPressedKeys()[0];
                            clavier_prec = Keyboard.GetState();
                            touches[5] = !touches[5];
                        }
                        else
                        {
                            BanqueSon.PlayCue(son_menu_refuser);
                            if (!touche_prise)
                                touche_prise = true;
                        }
                    }
                    else if (clavier_prec != clavier && (clavier.IsKeyDown(Keys.Enter) || clavier.IsKeyDown(Keys.Escape)))
                        touches[5] = !touches[5];
                    Afficher_Ecran();
                    break;
                case 6:
                    if (clavier_prec != clavier && clavier.IsKeyDown(Keys.Enter))
                    {
                        BanqueSon.GetCue(son_menu_retour).Play();
                        Gestion_Ecran.Goto_Ecran("Options");
                        this.Shutdown();
                    }
                    break;
                default:
                    break;
            }

            if (touche_prise)
            {
                touche_prise = false;
                compteur_touche_prise = 2.0f;
                opacite_touche_prise = 255;
            }

            if (compteur_touche_prise > 0)
            {
                compteur_touche_prise -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                opacite_touche_prise -= 5;
            }

            // Déplacement du curseur
            if (Changement_Touche(touches) && clavier_prec != clavier)
            {
                if (clavier.IsKeyDown(Keys.Down))
                {
                    BanqueSon.GetCue(son_menu_curseur).Play();
                    curseur = (curseur + 1) % menu.Length;
                }
                else if (clavier.IsKeyDown(Keys.Up))
                {
                    BanqueSon.GetCue(son_menu_curseur).Play();
                    if (curseur <= 0)
                        curseur = menu.Length - 1;
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
            float scale = 0.5f + pulsation * 0.05f * pulsation_diminution;

            graphics.GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();
            spriteBatch.DrawString(texte, nom, new Vector2((graphics.GraphicsDevice.Viewport.Width / 2) - (texte.MeasureString(nom).X / 2), 50), Color.Gray, 0, Vector2.Zero, 1, SpriteEffects.None, 0);

            if(compteur_touche_prise > 0)
                spriteBatch.DrawString(texte, texte_touche_prise, new Vector2((graphics.GraphicsDevice.Viewport.Width / 2) - (texte.MeasureString(texte_touche_prise).X / 2) * 0.6f, 200), new Color(Color.Gray, opacite_touche_prise), 0, Vector2.Zero, 0.6f, SpriteEffects.None, 0);


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
            string avancer;
            if(!touches[0])
                avancer = aMAZEing_Escape.Properties.Settings.Default.avancer.ToString();
            else
                avancer = "_";

            string reculer;
            if (!touches[1])
                reculer = aMAZEing_Escape.Properties.Settings.Default.reculer.ToString();
            else
                reculer = "_";

            string gauche;
            if (!touches[2])
                gauche = aMAZEing_Escape.Properties.Settings.Default.gauche.ToString();
            else
                gauche = "_";

            string droite;
            if (!touches[3])
                droite = aMAZEing_Escape.Properties.Settings.Default.droite.ToString();
            else
                droite = "_";

            string sauter;
            if (!touches[4])
                sauter = aMAZEing_Escape.Properties.Settings.Default.sauter.ToString();
            else
                sauter = "_";

            string sebaisser;
            if (!touches[5])
                sebaisser = aMAZEing_Escape.Properties.Settings.Default.sebaisser.ToString();
            else
                sebaisser = "_";
            

            menu = new string[] 
            { 
                "Avancer: " + avancer, 
                "Reculer: " + reculer, 
                "Pas latéral gauche: " + gauche, 
                "Pas latéral droit: " + droite, 
                "Sauter: " + sauter, 
                "Se baisser: " + sebaisser,
                "Retour" 
            };
        }


        public static bool Touche_Libre(Keys touche)
        {
            if (touche == aMAZEing_Escape.Properties.Settings.Default.avancer)
                return false;
            else if (touche == aMAZEing_Escape.Properties.Settings.Default.reculer)
                return false;
            else if (touche == aMAZEing_Escape.Properties.Settings.Default.droite)
                return false;
            else if (touche == aMAZEing_Escape.Properties.Settings.Default.gauche)
                return false;
            else if (touche == aMAZEing_Escape.Properties.Settings.Default.sauter)
                return false;
            else if (touche == aMAZEing_Escape.Properties.Settings.Default.sebaisser)
                return false;
            return true;
        }

        public static bool Changement_Touche(bool[] touches)
        {
            for (int i = 0; i < touches.Length; i++)
            {
                if (touches[i])
                    return false;
            }
            return true;
        }
    }
}