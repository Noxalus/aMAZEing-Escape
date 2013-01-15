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
    class Bonus : Ecran
    {
        private string[] menu;
        private int curseur;
        private Texture2D image_objets;
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

        float pulsation_diminution;

        public Bonus(GraphicsDeviceManager graphics, ContentManager Content)
            : base(graphics, Content, "Bonus")
        {
        }

        public override bool Init()
        {

            curseur = 0;
            clavier = new KeyboardState();
            clavier_prec = Keyboard.GetState();

            spriteBatch = new SpriteBatch(graphics.GraphicsDevice);
            texte = Content.Load<SpriteFont>("Polices/menu");
            image_objets = Content.Load<Texture2D>("Images/bonus");

            if (!Config.active_bonus && Config.nb_bonus_index != 3)
                Config.active_bonus = true;

            if (!Config.modedejeu)
                Config.boussole_sortie = false;

            // Sons et musiques
            Audio = new AudioEngine("Content/Musiques/ambiance.xgs");
            // Chargement des banques
            BanqueSon = new SoundBank(Audio, "Content/Musiques/Sound Bank.xsb");
            BanqueWave = new WaveBank(Audio, "Content/Musiques/Wave Bank.xwb");
            son_menu_curseur = "menu_curseur";
            son_menu_validation = "menu_validation";
            son_menu_retour = "menu_retour";
            Audio.Update();

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
                BanqueSon.PlayCue(son_menu_retour);
                Gestion_Ecran.Goto_Ecran("Nouvelle Partie");
            }

            this.Shutdown();
            if (clavier_prec != clavier && clavier.IsKeyDown(Keys.Enter))
                switch (curseur)
                {
                    case 0:
                        Config.nb_bonus_index = (Config.nb_bonus_index + 1) % Config.nb_bonus_texte.Length;
                        if (Config.nb_bonus_index == 3)
                        {
                            Config.active_bonus = false;
                            Config.lenteur = false;
                            Config.inversion = false;
                            Config.gel = false;
                            Config.touches_changees = false;
                            Config.camera_inversee = false;
                            Config.teleportation = false;
                            Config.sprint = false;
                            Config.fil_ariane = false;
                            Config.carte = false;
                            Config.camera_changee = false;
                            Config.obscurite = false;
                            Config.boussole_sortie = false;
                        }
                        else
                            Config.active_bonus = true;

                        Afficher_Ecran();
                        break;
                    case 1:
                        BanqueSon.GetCue(son_menu_validation).Play();
                        Config.lenteur = !Config.lenteur;
                        Afficher_Ecran();
                        break;
                    case 2:
                        BanqueSon.GetCue(son_menu_validation).Play();
                        Config.inversion = !Config.inversion;
                        Afficher_Ecran();
                        break;

                    case 3:
                        BanqueSon.GetCue(son_menu_validation).Play();
                        Config.gel = !Config.gel;
                        Afficher_Ecran();
                        break;

                    case 4:
                        BanqueSon.GetCue(son_menu_validation).Play();
                        Config.touches_changees = !Config.touches_changees;
                        Afficher_Ecran();
                        break;

                    case 5:
                        BanqueSon.GetCue(son_menu_validation).Play();
                        Config.camera_inversee = !Config.camera_inversee;
                        Afficher_Ecran();
                        break;
                    case 6:
                        BanqueSon.GetCue(son_menu_validation).Play();
                        Config.teleportation = !Config.teleportation;
                        Afficher_Ecran();
                        break;
                    case 7:
                        BanqueSon.GetCue(son_menu_validation).Play();
                        Config.sprint = !Config.sprint;
                        Afficher_Ecran();
                        break;
                    case 8:
                        BanqueSon.GetCue(son_menu_validation).Play();
                        Config.fil_ariane = !Config.fil_ariane;
                        Afficher_Ecran();
                        break;
                    case 9:
                        BanqueSon.GetCue(son_menu_validation).Play();
                        Config.carte = !Config.carte;
                        Afficher_Ecran();
                        break;
                    case 10:
                        BanqueSon.GetCue(son_menu_validation).Play();
                        Config.camera_changee = !Config.camera_changee;
                        Afficher_Ecran();
                        break;
                    case 11:
                        BanqueSon.GetCue(son_menu_validation).Play();
                        Config.obscurite = !Config.obscurite;
                        Afficher_Ecran();
                        break;
                    case 12:
                        BanqueSon.GetCue(son_menu_validation).Play();
                        Config.boussole_sortie = !Config.boussole_sortie;
                        Afficher_Ecran();
                        break;
                    case 13:
                        BanqueSon.PlayCue(son_menu_retour);
                        Gestion_Ecran.Goto_Ecran("Nouvelle Partie");
                        break;
                }

            // Déplacement du curseur
            if (clavier_prec != clavier && clavier.IsKeyDown(Keys.Down))
            {
                BanqueSon.GetCue(son_menu_curseur).Play();
                if (!Config.active_bonus && curseur == 0)
                    curseur = 13;
                else if (!Config.modedejeu && curseur == 11)
                    curseur = 13;
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
                    if (!Config.active_bonus && curseur == 13)
                        curseur = 0;
                    else if (!Config.modedejeu && curseur == 13)
                        curseur = 11;
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
            Rectangle recbonus = new Rectangle((graphics.GraphicsDevice.Viewport.Width / 2) + 150, (graphics.GraphicsDevice.Viewport.Height / 2), 150, 150);
            spriteBatch.Draw(image_objets, recbonus, Color.White);
            spriteBatch.DrawString(texte, nom, new Vector2((graphics.GraphicsDevice.Viewport.Width / 2) - (texte.MeasureString(nom).X / 2), 50), Color.Gray, 0, Vector2.Zero, 1, SpriteEffects.None, 0);

            for (int i = 0; i < menu.Length; i++)
            {
                if (curseur == i)
                    spriteBatch.DrawString(texte, menu[i], new Vector2((graphics.GraphicsDevice.Viewport.Width / 2) - (texte.MeasureString(menu[i]).X / 2) * scale, graphics.GraphicsDevice.Viewport.Height / 2 - texte.MeasureString(menu[i]).Y * scale + texte.MeasureString(menu[i]).Y / 2 * i - 100f), Color.Green, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
                else
                {
                    if ((!Config.active_bonus && (i > 0 && i < 13)) || (!Config.modedejeu && i == 12))
                        spriteBatch.DrawString(texte, menu[i], new Vector2((graphics.GraphicsDevice.Viewport.Width / 2) - (texte.MeasureString(menu[i]).X / 2) * 0.4f, graphics.GraphicsDevice.Viewport.Height / 2 - texte.MeasureString(menu[i]).Y * 0.4f + texte.MeasureString(menu[i]).Y / 2 * i - 100f), new Color(Color.Gray, 150), 0, Vector2.Zero, 0.4f, SpriteEffects.None, 0);
                    else
                        spriteBatch.DrawString(texte, menu[i], new Vector2((graphics.GraphicsDevice.Viewport.Width / 2) - (texte.MeasureString(menu[i]).X / 2) * 0.4f, graphics.GraphicsDevice.Viewport.Height / 2 - texte.MeasureString(menu[i]).Y * 0.4f + texte.MeasureString(menu[i]).Y / 2 * i - 100f), Color.Gray, 0, Vector2.Zero, 0.4f, SpriteEffects.None, 0);

                }
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        public void Afficher_Ecran()
        {
            menu = new string[] 
            { 
                "Nombre de bonus: " + Config.nb_bonus_texte[Config.nb_bonus_index]+"\n",
                "Lenteur: " + (Config.lenteur ? "Activé" : "Desactivé"), 
                "Inversion: " + (Config.inversion ? "Activé" : "Desactivé"), 
                "Gel: " + (Config.gel ? "Activé" : "Desactivé"), 
                "Touches changées: " + (Config.touches_changees ? "Activé" : "Desactivé"),
                "Caméra inversée: " + (Config.camera_inversee? "Activé" : "Desactivé"),
                "Téléportation: " + (Config.teleportation?  "Activé" : "Desactivé"),
                "Sprint: " + (Config.sprint ? "Activé" : "Desactivé"),
                "Fil d'Ariane: " + (Config.fil_ariane ? "Activé" : "Desactivé"),
                "Carte 2D: " + (Config.carte ? "Activé" : "Desactivé)"),
                "Caméra changée: " + (Config.camera_changee ? "Activé" : "Desactivé"),
                "Obscurité: " + (Config.obscurite ? "Activé" : "Desactivé"),
                "Boussole sortie: " + (Config.boussole_sortie? "Activé" : "Desactivé") ,
                "Retour"
            };
        }
    }
}