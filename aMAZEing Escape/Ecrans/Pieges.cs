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
    class Pieges : Ecran
    {
        

        private string[] menu;
        private int curseur;
        private SpriteFont texte;
        private Texture2D pics;
        private Texture2D lasers;
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
     
        public Pieges(GraphicsDeviceManager graphics, ContentManager Content) : base(graphics, Content, "Pièges")
        {
        }

        public override bool Init()
        {
           
            curseur = 0;
            clavier = new KeyboardState();
            clavier_prec = Keyboard.GetState();

            spriteBatch = new SpriteBatch(graphics.GraphicsDevice);
            texte = Content.Load<SpriteFont>("Polices/menu");
            pics = Content.Load<Texture2D>("Images/pics");
            lasers = Content.Load<Texture2D>("Images/laser_menu");

            // Sons et musiques
            Audio = new AudioEngine("Content/Musiques/ambiance.xgs");
            // Chargement des banques
            BanqueSon = new SoundBank(Audio, "Content/Musiques/Sound Bank.xsb");
            BanqueWave = new WaveBank(Audio, "Content/Musiques/Wave Bank.xwb");
            son_menu_curseur = "menu_curseur";
            son_menu_validation = "menu_validation";
            son_menu_retour = "menu_retour";
            Audio.Update();

            if (!Config.active_pieges && Config.nb_pieges_index != 3)
                Config.active_pieges = true;

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
                Gestion_Ecran.Goto_Ecran("Nouvelle Partie");
            }
            this.Shutdown();
            if (clavier_prec != clavier && clavier.IsKeyDown(Keys.Enter))
            {
                
                switch (curseur)
                {
                    case 0:
                        BanqueSon.GetCue(son_menu_validation).Play();
                        Config.nb_pieges_index = (Config.nb_pieges_index + 1) % Config.nb_pieges_texte.Length;
                        if (Config.nb_pieges_index == 3)
                        {
                            Config.active_pieges = false;
                            Config.laser = false;
                            Config.trappe = false;
                        }
                        else
                            Config.active_pieges = true;

                        Afficher_Ecran();
                        break;
                    case 1:
                        BanqueSon.GetCue(son_menu_validation).Play();
                        Config.laser = !Config.laser;
                        Afficher_Ecran();
                        break;

                    case 2:
                        BanqueSon.GetCue(son_menu_validation).Play();
                        Config.trappe = !Config.trappe;
                        Afficher_Ecran();
                        break;
                    case 3:
                        BanqueSon.GetCue(son_menu_retour).Play();
                        Gestion_Ecran.Goto_Ecran("Nouvelle Partie");
                        this.Shutdown();
                        break;
                }
            }
            
            // Déplacement du curseur
            if (clavier_prec != clavier && clavier.IsKeyDown(Keys.Down))
            {
                BanqueSon.GetCue(son_menu_curseur).Play();
                if (!Config.active_pieges && curseur == 0)
                        curseur = 3;
                else
                    curseur = (curseur + 1) % menu.Length; 
            }

            else if (clavier_prec != clavier && clavier.IsKeyDown(Keys.Up))
            {
                BanqueSon.GetCue(son_menu_curseur).Play();
                if (!Config.active_pieges && curseur == 3)
                    curseur = 0;
                else
                {
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

            Rectangle recpic = new Rectangle(((graphics.GraphicsDevice.Viewport.Width) / 2) - 300, (graphics.GraphicsDevice.Viewport.Height / 2) - 50, 220, 200);

            if (Config.trappe)
                spriteBatch.Draw(pics, recpic, Color.White);

            if (Config.laser)
                spriteBatch.Draw(lasers, new Rectangle(((graphics.GraphicsDevice.Viewport.Width) / 2) + 100, (graphics.GraphicsDevice.Viewport.Height/2) - 20, lasers.Width / 2, lasers.Height / 2), Color.White);

            spriteBatch.DrawString(texte, nom, new Vector2((graphics.GraphicsDevice.Viewport.Width / 2) - (texte.MeasureString(nom).X / 2), 50), Color.Gray, 0, Vector2.Zero, 1, SpriteEffects.None, 0);

            for (int i = 0; i < menu.Length; i++)
            {
                if (curseur == i)
                    spriteBatch.DrawString(texte, menu[i], new Vector2((graphics.GraphicsDevice.Viewport.Width / 2) - (texte.MeasureString(menu[i]).X / 2) * scale, graphics.GraphicsDevice.Viewport.Height / 2 - texte.MeasureString(menu[i]).Y * scale + texte.MeasureString(menu[i]).Y / 2 * i - 100f), Color.Green, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
                else
                {
                    if(!Config.active_pieges && (i == 1 || i == 2))
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
                "Nombre de piéges: " + Config.nb_pieges_texte[Config.nb_pieges_index]+"\n",
                "Laser: " + (Config.laser? "Activé" : "Desactivé"), 
                "Trappe: " + (Config.trappe? "Activé" : "Desactivé"), 
                "Retour"
            };
        }
    }
}