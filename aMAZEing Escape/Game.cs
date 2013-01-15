using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using aMAZEing_Escape.Ecrans;
using aMAZEing_Escape.Properties;

namespace aMAZEing_Escape
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game : Microsoft.Xna.Framework.Game
    {
        
        GraphicsDeviceManager graphics;

        public Game()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferHeight = Config.resolutions[aMAZEing_Escape.Properties.Settings.Default.index_resolution, 1];
            graphics.PreferredBackBufferWidth = Config.resolutions[aMAZEing_Escape.Properties.Settings.Default.index_resolution, 0];
            graphics.IsFullScreen = aMAZEing_Escape.Properties.Settings.Default.plein_ecran;
            graphics.ApplyChanges();
        }

        protected override void Initialize()
        {
            

            // Chargement de tous les écrans du jeu
            Gestion_Ecran.Ajouter_Ecran(new Menu_Principal(graphics, Content));
            Gestion_Ecran.Ajouter_Ecran(new Nouvelle_Partie(graphics, Content));
            Gestion_Ecran.Ajouter_Ecran(new Options(graphics, Content));
            Gestion_Ecran.Ajouter_Ecran(new Touches(graphics, Content));
            Gestion_Ecran.Ajouter_Ecran(new Jeu(graphics, Content));
            Gestion_Ecran.Ajouter_Ecran(new Quitter(graphics, Content));
            Gestion_Ecran.Ajouter_Ecran(new aMAZEing_Escape.Ecrans.Bonus(graphics, Content));
            Gestion_Ecran.Ajouter_Ecran(new aMAZEing_Escape.Ecrans.Pieges(graphics, Content));
            Gestion_Ecran.Ajouter_Ecran(new Fail(graphics, Content));
            Gestion_Ecran.Ajouter_Ecran(new Succes(graphics, Content));
            Gestion_Ecran.Ajouter_Ecran(new Stats(graphics, Content));
            Gestion_Ecran.Ajouter_Ecran(new Stats(graphics, Content));
            Gestion_Ecran.Ajouter_Ecran(new Video_Intro(graphics, Content));
            Gestion_Ecran.Ajouter_Ecran(new Credits(graphics, Content));
            
            Gestion_Ecran.Goto_Ecran("Vidéo d'introduction");

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            Gestion_Ecran.Init();
        }

        protected override void UnloadContent()
        {
            Settings.Default.Save();
        }

        protected override void Update(GameTime gameTime)
        {

            if (Config.quitter_jeu)
            {
                this.Exit();
            }
            // On update l'écran actuelle
            Gestion_Ecran.Update(gameTime);
           

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            // Tell ScreenManager to draw
            Gestion_Ecran.Draw(gameTime);

            base.Draw(gameTime);
        }
    }
}