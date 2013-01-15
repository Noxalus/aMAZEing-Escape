using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace aMAZEing_Escape
{
    class Joueur
    {
        public int[,] matrice_cout;
        // Audio
        private SoundBank SoundBank;
        private string son_saut;
        // Initalisation du joueur
        public Vector3 position;
        public Point position_case_actuelle;
        public Point position_case_precedente;
        public bool changement_de_case;
        Point case_transition;
        public Vector3 reference;
        public Vector3 cible;
        public float vitesse;
        public float vitesse_initiale;
        public bool noclip; // Passage à travers les murs
        public bool fil_ariane;
        public bool gel;
        public bool inversertouches;
        public Keys[] Touches;
        public Keys[] TouchesFausses;
        public bool immobile;
        public bool vivant;
        // Le saut
        private double valeur_saut;
        private float compteur_saut;
        private float compteur_saut_initial;
        private float vitesse_saut;
        public bool saut;
        // Se baisser
        public bool baisse;
        // Piège
        public bool trappe;
        public bool laser;
        // Gestion des périphériques
        KeyboardState clavier;
        KeyboardState clavier_prec;
        public bool bonus_actif = false;

        // Le constructeur
        public Joueur(Viewport vp, Labyrinthe laby, SoundBank SoundBank)
        {
            /** Infos sur le joueur **/
            position = laby.joueur_position_initiale;
            position_case_actuelle = new Point((int)(position.X / laby.CellSize.X), (int)(position.Z / laby.CellSize.Z));
            position_case_precedente = position_case_actuelle;
            case_transition = position_case_actuelle;
            changement_de_case = false;
            reference = new Vector3(0.0f, 0.0f, 1.0f);
            cible = position + reference;
            vitesse_initiale = 4.0f;
            vitesse = vitesse_initiale;
            noclip = false;
            gel = false;
            inversertouches = false;

            immobile = true;

            // Le saut
            valeur_saut = 0;
            vitesse_saut = 3.5f;
            compteur_saut_initial = (3 / vitesse_saut) - 0.021f;
            compteur_saut = compteur_saut_initial;
            saut = false;
            // Se baisser
            baisse = false;
            this.SoundBank = SoundBank;
            if (Config.modedejeu)
                son_saut = "saut";
            else
                son_saut = "saut_chasseur";

            vivant = true;
            // Clavier
            clavier_prec = Keyboard.GetState();

            // Les touches
            Touches = new Keys[] 
            { 
               aMAZEing_Escape.Properties.Settings.Default.avancer, 
               aMAZEing_Escape.Properties.Settings.Default.droite,
               aMAZEing_Escape.Properties.Settings.Default.reculer, 
               aMAZEing_Escape.Properties.Settings.Default.gauche
            };

            TouchesFausses = new Keys[] { Keys.Z, Keys.D, Keys.S, Keys.Q };

            // Initialisation de la matrice de coût
            matrice_cout = new int[Config.largeur_labyrinthe, Config.hauteur_labyrinthe];
            matrice_cout = IA.GenLabyIA(position_case_actuelle, matrice_cout, laby);
        }


        public void Update(Viewport vp, GameTime gameTime, ContentManager cm, float yaw, float pitch, float temps, Labyrinthe laby, bool triche)
        {
            clavier = Keyboard.GetState();
            Vector3 deplacement = Vector3.Zero;

            // On actualise la position du joueur
            position_case_actuelle = new Point((int)(position.X / laby.CellSize.X), (int)(position.Z / laby.CellSize.Z));

            // Est-ce que le joueur se trouve sur un piège ?
            if (laby.Carte[position_case_actuelle.X, position_case_actuelle.Y] == 6)
            {
                // Quel piège ?
                foreach (Pieges piege in laby.liste_pieges)
                {
                    if (piege.position_case == position_case_actuelle)
                    {
                        if (piege.type == 0 && !piege.afficher_trappe)
                            trappe = true;
                        else
                            laser = true;
                    }
                }
            }
            else
            {
                trappe = false;
                laser = false;
            }

            // Le joueur change de case
            if (case_transition != position_case_actuelle)
            {
                if (immobile)
                    immobile = false;
                changement_de_case = true;
                matrice_cout = IA.GenLabyIA(position_case_actuelle, matrice_cout, laby);
                position_case_precedente = case_transition;
            }
            else
                changement_de_case = false;

            if (!gel && vivant)
            {
                if (inversertouches)
                {
                    if (clavier.IsKeyDown(TouchesFausses[0]))
                        deplacement.Z += vitesse * temps;
                    if (clavier.IsKeyDown(TouchesFausses[1]))
                        deplacement.Z -= vitesse * temps;
                    if (clavier.IsKeyDown(TouchesFausses[2]))
                        deplacement.X += vitesse * temps;
                    if (clavier.IsKeyDown(TouchesFausses[3]))
                        deplacement.X -= vitesse * temps;
                }
                else
                {
                    if (clavier.IsKeyDown(Touches[0]))
                        deplacement.Z += vitesse * temps;
                    if (clavier.IsKeyDown(Touches[2]))
                        deplacement.Z -= vitesse * temps;
                    if (clavier.IsKeyDown(Touches[3]))
                        deplacement.X += vitesse * temps;
                    if (clavier.IsKeyDown(Touches[1]))
                        deplacement.X -= vitesse * temps;
                }
            }
            

            // Activation du mode noclip
            if (triche && clavier.IsKeyDown(Keys.N))
                noclip = true;
            else if(triche && clavier_prec.IsKeyDown(Keys.N) && !clavier.IsKeyDown(Keys.N))
                noclip = false;

            // Empêche le joueur des limites du labyrinthe
            if (position.Y > 0)
            {
                position.X = MathHelper.Clamp(position.X, 0 + 0.2f, (laby.Size.X * laby.CellSize.X) - 0.2f);
                position.Z = MathHelper.Clamp(position.Z, 0 + 0.2f, (laby.Size.Y * laby.CellSize.Z) - 0.2f);
            }

            #region Collisions
            if (noclip == false)
            {
                if (trappe && position.Y < laby.joueur_position_initiale.Y)
                {

                    position.Z = MathHelper.Clamp(position.Z, (position_case_actuelle.Y) * laby.CellSize.Z + 0.2f, (position_case_actuelle.Y + 1) * laby.CellSize.Z - 0.2f);
                    position.X = MathHelper.Clamp(position.X, (position_case_actuelle.X) * laby.CellSize.X + 0.2f, (position_case_actuelle.X + 1) * laby.CellSize.Z - 0.2f);
                }

                else
                {
                    //Mur en X

                    // Si on est pas au bord
                    if (position_case_actuelle.X + 1 < laby.Size.X && position_case_actuelle.X - 1 >= 0)
                    {
                        // Mur à gauche
                        if (laby.Carte[position_case_actuelle.X - 1, position_case_actuelle.Y] == 1)
                        {
                            // Mur à gauche et à droite
                            if (laby.Carte[position_case_actuelle.X + 1, position_case_actuelle.Y] == 1)
                                position.X = MathHelper.Clamp(position.X, position_case_actuelle.X * laby.CellSize.X + 0.2f, (position_case_actuelle.X + 1) * laby.CellSize.X - 0.2f);
                            // Mur à gauche et pas à droite
                            else
                                position.X = MathHelper.Clamp(position.X, position_case_actuelle.X * laby.CellSize.X + 0.2f, (position_case_actuelle.X + 2) * laby.CellSize.X - 0.2f);
                        }
                        // Pas de mur à gauche
                        else
                        {
                            // Pas de mur à droite et mur à gauche
                            if (laby.Carte[position_case_actuelle.X + 1, position_case_actuelle.Y] == 1)
                                position.X = MathHelper.Clamp(position.X, (position_case_actuelle.X - 1) * laby.CellSize.X + 0.2f, (position_case_actuelle.X + 1) * laby.CellSize.X - 0.2f);
                            // Sinon, il n'y a pas de mur => on ne fait rien !
                        }
                    }

                    // Si on est au bord à gauche et qu'il y a un mur à droite
                    else if (position_case_actuelle.X - 1 < 0 && laby.Carte[position_case_actuelle.X + 1, position_case_actuelle.Y] == 1)
                        position.X = MathHelper.Clamp(position.X, 0 + 0.2f, (position_case_actuelle.X + 1) * laby.CellSize.X - 0.2f);

                    // Si on est au bord à droite et qu'il y a un mur à gauche
                    else if (position_case_actuelle.X + 1 >= laby.Size.X && laby.Carte[position_case_actuelle.X - 1, position_case_actuelle.Y] == 1)
                        position.X = MathHelper.Clamp(position.X, position_case_actuelle.X * laby.CellSize.X + 0.2f, laby.Size.X * laby.CellSize.X - 0.2f);

                    // Murs en Y

                    // Si on est pas au bord
                    if (position_case_actuelle.Y + 1 < laby.Size.Y && position_case_actuelle.Y - 1 >= 0)
                    {
                        // Mur en haut
                        if (laby.Carte[position_case_actuelle.X, position_case_actuelle.Y - 1] == 1)
                        {
                            // Mur en haut et en bas
                            if (laby.Carte[position_case_actuelle.X, position_case_actuelle.Y + 1] == 1)
                                position.Z = MathHelper.Clamp(position.Z, position_case_actuelle.Y * laby.CellSize.Z + 0.2f, (position_case_actuelle.Y + 1) * laby.CellSize.Z - 0.2f);
                            // Mur en haut et pas de mur en bas
                            else
                                position.Z = MathHelper.Clamp(position.Z, position_case_actuelle.Y * laby.CellSize.Z + 0.2f, (position_case_actuelle.Y + 2) * laby.CellSize.Z - 0.2f);
                        }
                        // Pas de mur en haut
                        else
                        {
                            // Pas de mur en haut et pas de mur en bas
                            if (laby.Carte[position_case_actuelle.X, position_case_actuelle.Y + 1] == 1)
                                position.Z = MathHelper.Clamp(position.Z, (position_case_actuelle.Y - 1) * laby.CellSize.Z + 0.2f, (position_case_actuelle.Y + 1) * laby.CellSize.Z - 0.2f);
                        }
                    }

                    // Si on est au bord en haut et qu'il y a un mur en bas
                    else if (position_case_actuelle.Y - 1 < 0 && laby.Carte[position_case_actuelle.X, position_case_actuelle.Y + 1] == 1)
                        position.Z = MathHelper.Clamp(position.Z, 0 + 0.2f, (position_case_actuelle.Y + 1) * laby.CellSize.Z - 0.2f);

                    // Si on est au bord en bas et qu'il y a un mur en haut
                    else if (position_case_actuelle.Y + 1 >= laby.Size.Y && laby.Carte[position_case_actuelle.X, position_case_actuelle.Y - 1] == 1)
                        position.Z = MathHelper.Clamp(position.Z, position_case_actuelle.Y * laby.CellSize.Z + 0.2f, laby.Size.Y * laby.CellSize.Z - 0.2f);

                    // **** Collisions coins **** \\
                    /*
                     * Il y a plusieurs problemes dus a la conversion de la carte 2D en 3D : 
                     * D'abord on fait le test si dans les coins il y a les murs et si autour non.
                     * Ensuite on compare la position du joueur avec celle du coin du mur en faisant attention
                     * avec les coordonnées, nous amenant parfois a comparer avec un autre mur, car la coordonnée du mur 
                     * est située en bas a droite sur une carte... Compliqué tout ça !!
                     * 
                     */


                    double distPointHautGauche = Math.Sqrt(Math.Pow(position.X - (position_case_actuelle.X * laby.CellSize.X), 2) + Math.Pow(position.Z - (position_case_actuelle.Y * laby.CellSize.Z), 2));
                    double distPointHautDroite = Math.Sqrt(Math.Pow(position.X - ((position_case_actuelle.X + 1) * laby.CellSize.X), 2) + Math.Pow(position.Z - (position_case_actuelle.Y * laby.CellSize.Z), 2));
                    double distPointBasGauche = Math.Sqrt(Math.Pow(position.X - ((position_case_actuelle.X) * laby.CellSize.X), 2) + Math.Pow(position.Z - ((position_case_actuelle.Y + 1) * laby.CellSize.Z), 2));
                    double distPointBasDroite = Math.Sqrt(Math.Pow(position.X - ((position_case_actuelle.X + 1) * laby.CellSize.X), 2) + Math.Pow(position.Z - ((position_case_actuelle.Y + 1) * laby.CellSize.Z), 2));

                    // Coté gauche de carte
                    if (position_case_actuelle.X == 0)
                    {
                        // Coin haut-gauche
                        if (position_case_actuelle.Y == 0)
                        {
                            // Mur bas-droite
                            if (laby.Carte[position_case_actuelle.X + 1, position_case_actuelle.Y] != 1 &&
                                laby.Carte[position_case_actuelle.X, position_case_actuelle.Y + 1] != 1 &&
                                laby.Carte[position_case_actuelle.X + 1, position_case_actuelle.Y + 1] == 1 &&
                                    distPointBasDroite < 0.1f)
                            {
                                position.X -= 0.1f;
                                position.Z -= 0.1f;
                            }
                        }

                        // Coté gauche hors coins haut et bas
                        else if (position_case_actuelle.Y + 1 < laby.Size.Y)
                        {
                            // Pas de mur a droite
                            if (laby.Carte[position_case_actuelle.X + 1, position_case_actuelle.Y] != 1)
                            {
                                // Mur haut-droite
                                if (laby.Carte[position_case_actuelle.X, position_case_actuelle.Y - 1] != 1 &&
                                    laby.Carte[position_case_actuelle.X + 1, position_case_actuelle.Y - 1] == 1 &&
                                        distPointHautDroite < 0.1f)
                                {
                                    position.X -= 0.1f;
                                    position.Z += 0.1f;
                                }


                                // Mur bas-droite
                                if (laby.Carte[position_case_actuelle.X, position_case_actuelle.Y + 1] != 1 &&
                                    laby.Carte[position_case_actuelle.X + 1, position_case_actuelle.Y + 1] == 1 &&
                                        distPointBasDroite < 0.1f)
                                {
                                    position.X -= 0.1f;
                                    position.Z -= 0.1f;
                                }

                            }
                        }
                        // Coin bas-gauche
                        else
                        {
                            // Mur haut-droite
                            if (laby.Carte[position_case_actuelle.X, position_case_actuelle.Y - 1] != 1 &&
                                laby.Carte[position_case_actuelle.X + 1, position_case_actuelle.Y] != 1 &&
                                laby.Carte[position_case_actuelle.X + 1, position_case_actuelle.Y - 1] == 1 &&
                                    distPointHautDroite < 0.1f)
                            {
                                position.X -= 0.1f;
                                position.Z += 0.1f;
                            }
                        }
                    }

                    // Partout dans la carte sauf coins haut-gauche et bas-gauche
                    else if (position_case_actuelle.X > 0 && position_case_actuelle.X + 1 < laby.Size.X)
                    {
                        // bord haut
                        if (position_case_actuelle.Y == 0)
                        {
                            // Pas de mur en bas
                            if (laby.Carte[position_case_actuelle.X, position_case_actuelle.Y + 1] != 1)
                            {
                                // Mur bas-gauche
                                if (laby.Carte[position_case_actuelle.X - 1, position_case_actuelle.Y] != 1 &&
                                    laby.Carte[position_case_actuelle.X - 1, position_case_actuelle.Y + 1] == 1 &&
                                        distPointBasGauche < 0.1f)
                                {
                                    position.X += 0.1f;
                                    position.Z -= 0.1f;
                                }

                                // Mur bas-droite
                                if (laby.Carte[position_case_actuelle.X + 1, position_case_actuelle.Y] != 1 &&
                                    laby.Carte[position_case_actuelle.X + 1, position_case_actuelle.Y + 1] == 1 &&
                                        distPointBasDroite < 0.1f)
                                {
                                    position.X -= 0.1f;
                                    position.Z -= 0.1f;
                                }
                            }
                        }

                        // Milieu de carte
                        else if (position_case_actuelle.Y + 1 < laby.Size.Y)
                        {
                            // Pas de mur en haut
                            if (laby.Carte[position_case_actuelle.X, position_case_actuelle.Y - 1] != 1)
                            {
                                // Mur haut-gauche
                                if (laby.Carte[position_case_actuelle.X - 1, position_case_actuelle.Y] != 1 &&
                                    laby.Carte[position_case_actuelle.X - 1, position_case_actuelle.Y - 1] == 1 &&
                                    distPointHautGauche < 0.1f)
                                {
                                    position.X += 0.1f;
                                    position.Z += 0.1f;
                                }

                                // Mur haut-droite
                                if (laby.Carte[position_case_actuelle.X + 1, position_case_actuelle.Y] != 1 &&
                                    laby.Carte[position_case_actuelle.X + 1, position_case_actuelle.Y - 1] == 1 &&
                                    distPointHautDroite < 0.1f)
                                {
                                    position.X -= 0.1f;
                                    position.Z += 0.1f;
                                }
                            }

                            // Pas de mur en bas
                            if (laby.Carte[position_case_actuelle.X, position_case_actuelle.Y + 1] != 1)
                            {
                                // Mur bas-gauche
                                if (laby.Carte[position_case_actuelle.X - 1, position_case_actuelle.Y] != 1 &&
                                    laby.Carte[position_case_actuelle.X - 1, position_case_actuelle.Y + 1] == 1 &&
                                    distPointBasGauche < 0.1f)
                                {
                                    position.X += 0.1f;
                                    position.Z -= 0.1f;
                                }

                                // Mur bas-droite
                                if (laby.Carte[position_case_actuelle.X + 1, position_case_actuelle.Y] != 1 &&
                                    laby.Carte[position_case_actuelle.X + 1, position_case_actuelle.Y + 1] == 1 &&
                                    distPointBasDroite < 0.1f)
                                {
                                    position.X -= 0.1f;
                                    position.Z -= 0.1f;
                                }
                            }
                        }

                        // coté bas
                        else
                        {
                            // Pas de mur en haut
                            if (laby.Carte[position_case_actuelle.X, position_case_actuelle.Y - 1] != 1)
                            {
                                // Mur haut-gauche
                                if (laby.Carte[position_case_actuelle.X - 1, position_case_actuelle.Y] != 1 &&
                                    laby.Carte[position_case_actuelle.X - 1, position_case_actuelle.Y - 1] == 1 &&
                                    distPointHautGauche < 0.1f)
                                {
                                    position.X += 0.1f;
                                    position.Z += 0.1f;
                                }

                                // Mur haut-droite
                                if (laby.Carte[position_case_actuelle.X + 1, position_case_actuelle.Y] != 1 &&
                                    laby.Carte[position_case_actuelle.X + 1, position_case_actuelle.Y - 1] == 1 &&
                                        distPointHautDroite < 0.1f)
                                {
                                    position.X -= 0.1f;
                                    position.Z += 0.1f;
                                }
                            }
                        }
                    }

                    // Bord droit
                    else if (position_case_actuelle.X + 1 == laby.Size.X)
                    {
                        // Coin haut-droite
                        if (position_case_actuelle.Y == 0)
                        {
                            // Mur bas-gauche
                            if (laby.Carte[position_case_actuelle.X - 1, position_case_actuelle.Y] != 1 &&
                                laby.Carte[position_case_actuelle.X, position_case_actuelle.Y + 1] != 1 &&
                                laby.Carte[position_case_actuelle.X - 1, position_case_actuelle.Y + 1] == 1 &&
                                    distPointBasGauche < 0.1f)
                            {
                                position.X += 0.1f;
                                position.Z -= 0.1f;
                            }
                        }

                        // Coté droit sauf coins
                        else if (position_case_actuelle.Y > 0 && position_case_actuelle.Y + 1 < laby.Size.Y)
                        {
                            // Pas de mur a gauche
                            if (laby.Carte[position_case_actuelle.X, position_case_actuelle.Y] != 1)
                            {
                                // Mur haut-gauche
                                if (laby.Carte[position_case_actuelle.X, position_case_actuelle.Y - 1] != 1 &&
                                    laby.Carte[position_case_actuelle.X - 1, position_case_actuelle.Y - 1] == 1 &&
                                        distPointHautGauche < 0.1f)
                                {
                                    position.X += 0.1f;
                                    position.Z += 0.1f;
                                }

                                // Mur bas-gauche
                                if (laby.Carte[position_case_actuelle.X, position_case_actuelle.Y + 1] != 1 &&
                                    laby.Carte[position_case_actuelle.X - 1, position_case_actuelle.Y + 1] == 1 &&
                                        distPointBasGauche < 0.1f)
                                {
                                    position.X += 0.1f;
                                    position.Z -= 0.1f;
                                }
                            }
                        }

                        // Coin bas-droite 
                        else
                        {
                            // Mur haut-gauche
                            if (laby.Carte[position_case_actuelle.X - 1, position_case_actuelle.Y] != 1 &&
                                laby.Carte[position_case_actuelle.X, position_case_actuelle.Y] != 1 &&
                                laby.Carte[position_case_actuelle.X - 1, position_case_actuelle.Y - 1] == 1 &&
                                    distPointHautGauche < 0.1f)
                            {
                                position.X += 0.1f;
                                position.Z += 0.1f;
                            }
                        }
                    }
                }
            }
            #endregion

            // Tombe dans une trappe
            if (trappe && !saut || position.Y < 0)
                position.Y -= vitesse_saut/30;
            else if (!baisse)
                position.Y = (float)(laby.joueur_position_initiale.Y + (Math.Sin(MathHelper.ToRadians((float)valeur_saut) % Math.PI)));

            /** Saut **/

            // Son
            if (!saut && !baisse && clavier_prec != clavier && clavier.IsKeyDown(aMAZEing_Escape.Properties.Settings.Default.sauter) && !trappe)
                SoundBank.PlayCue(son_saut);

            if (saut || (clavier_prec != clavier && clavier.IsKeyDown(aMAZEing_Escape.Properties.Settings.Default.sauter) && !trappe))
            {
                valeur_saut += vitesse_saut;
                compteur_saut -= temps;

                if (!saut)
                    saut = true;

                if (compteur_saut < 0 || baisse)
                {
                    saut = false;
                    compteur_saut = compteur_saut_initial;
                    valeur_saut = 0;
                }
            }

            // Se baisser
            float gravite = 9.8f;
            // 2 cas ou l'on modifie la hauteur du joueur
            if (clavier.IsKeyDown(aMAZEing_Escape.Properties.Settings.Default.sebaisser)) 
            {
                baisse = true;
                vitesse = vitesse_initiale / 2;
                // Soit il est en position normale et appuie sur la touche
                if (!(saut) && position.Y > 0.5) 
                    position.Y -= gravite * temps;
            }
            // Soit il n'appuie pas et il etait accroupi.
            else if (position.Y < laby.joueur_position_initiale.Y && !(saut) && !trappe)
            {
                vitesse = vitesse_initiale;
                position.Y += gravite * temps;
                baisse = false;
            }

            Matrix ViewRotationMatrix = Matrix.CreateRotationX(yaw) * Matrix.CreateRotationY(pitch);
            Matrix MoveRotationMatrix = Matrix.CreateRotationY(pitch);

            Vector3 transformedReference = Vector3.Transform(reference, ViewRotationMatrix);
            position += Vector3.Transform(deplacement, MoveRotationMatrix);
            cible = transformedReference + position;

            case_transition = position_case_actuelle;
            clavier_prec = clavier;
        }
    }
}