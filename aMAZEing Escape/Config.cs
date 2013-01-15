using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
namespace aMAZEing_Escape
{
    public static class Config
    {
        // Mode de jeu: true = chassé ; false = chasseur
        public static bool modedejeu = true;

        // Bonus
        public static bool active_bonus = true;
        public static bool active_pieges = true;

        // Labyrinthe
        public static int hauteur_labyrinthe = 30;
        public static int largeur_labyrinthe = 30;
        // true = parfait ; false = imparfait
        public static bool laby_algo = true; 
        public static string[] difficulte = { "Facile", "Moyen", "Difficile" };
        public static int index_difficulte = 1;
        //mort subite
        public static bool active_mort_subite = false;
        public static int min_mort_subite = 0;
        public static int sec_mort_subite = 0;
        //Chronomètre
        public static string chrono = "";
        public static int min = 0;
        public static int sec = 0;

        static public int[,] resolutions = new int[,] { { 800, 600 }, { 1024, 768 }, { 1280, 1024 }, {1680, 1050}, { 1920, 1080 } };

        // Police
        public static SpriteFont Menufont;
        public static SpriteFont Perso;

        //Bonus
        public static Dictionary<int, string> liste_bonus_texte = 
            new Dictionary<int, string> {
            {0, "Lenteur" }, 
            {1, "Inversion" }, 
            {2, "Gel"}, 
            {3, "Touches inversees"}, 
            {4, "Camera inversee"}, 
            {5, "Teleportation"}, 
            {6, "Sprint"}, 
            {7, "Fil d'Ariane"}, 
            {8, "Carte"}, 
            {9, "Camera changee"}, 
            {10, "Obscurite"}, 
            {11, "Boussole sortie"} };

        public static string[] nb_bonus_texte = { "Peu", "Normal", "Beaucoup", "Aucun" };
        public static int nb_bonus_index = 1;
        public static bool lenteur = true;
        public static bool inversion = true;
        public static bool gel = true;
        public static bool touches_changees = true;
        public static bool camera_inversee = true;
        public static bool teleportation = true;
        public static bool sprint = true;
        public static bool fil_ariane = true;
        public static bool carte = true;
        public static bool camera_changee = true;
        public static bool obscurite = true;
        public static bool boussole_sortie = true;

        public static float transparence_carte = 1;

        // Volume
        public static float volume_musiques = 1.0f;
        public static float volume_sons = 1.0f;

        //Piége
        public static string[] nb_pieges_texte = {"Peu", "Normal", "Beaucoup", "Aucun"};
        public static int nb_pieges_index = 1;
        public static bool laser = true;
        public static bool trappe = true;

        // La cause de mort
        public static string cause_fin_jeu;

        // Quitter le jeu
        public static bool quitter_jeu = false;

        // Pause
        public static bool pause = false;

        // Musique
        public static Cue musique_menu;
    }
}