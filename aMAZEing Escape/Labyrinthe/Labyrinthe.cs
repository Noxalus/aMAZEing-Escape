using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace aMAZEing_Escape
{
    class Labyrinthe
    {
        //Carte
        public int[,] Carte;
        // Taille
        public Point Size;
        public Vector3 CellSize;
        
        // Joueur
        public Vector3 joueur_position_initiale;
        public Point joueur_position_initiale_case;
        // Chasseur
        public Vector3 chasseur_position_initiale;
        // Scientifique
        public Vector3 scientifique_position_initiale;
        public Point scientifique_position_initiale_case;
        // Sortie
        public Point sortie_position;

        // Bonus
        public int nb_bonus;
        public List<Bonus> liste_bonus;

        // Pièges
        public int nb_pieges;
        public List<Pieges> liste_pieges;

        // Aléatoire
        Random random;

        // Matrices
        public bool[,] carte_espace_bonus;
        public bool[,] carte_espace_pieges;
        public bool[,] carte_sortie;

        // Pour la génération du labyrinthe
        Stack<Point> pos_prec;
        Point pos_actuelle;
        bool[] casespossibles;
        bool[] murs_possibles;

        public void BuildMaze(Point size, Vector3 cellSize, ContentManager Content, Labyrinthe laby, GraphicsDevice graphics, List<int> liste_bonus_actifs)
        {

            // On créé la carte réelle
            Carte = new int[size.X, size.Y];

            carte_espace_bonus = new bool[Config.largeur_labyrinthe, Config.hauteur_labyrinthe];
            carte_espace_pieges = new bool[Config.largeur_labyrinthe, Config.hauteur_labyrinthe];

            // On la remplie de mur
            for (int y = 0; y < size.Y; y++)
                for (int x = 0; x < size.X; x++)
                    Carte[x, y] = 1;

            // Taille
            Size = size;
            CellSize = cellSize;

            // Aléatoire
            random = new Random();

            //Bonus
            // Création de la liste des bonus
            liste_bonus = new List<Bonus>();

            // Pièges
            liste_pieges = new List<Pieges>();

            // On place la sortie
            sortie_position = new Point(random.Next(size.X), random.Next(size.Y));
            // On la rabat sur le bord le plus proche      
            if (sortie_position.X > size.X / 2)
            {
                if (sortie_position.Y > sortie_position.X)
                    sortie_position = new Point(sortie_position.X, size.Y - 1);
                else
                    sortie_position = new Point(size.X - 1, sortie_position.Y);
            }
            else
            {
                if (sortie_position.Y > sortie_position.X)
                    sortie_position = new Point(0, sortie_position.Y);
                else
                    sortie_position = new Point(sortie_position.X, 0);
            }
            // On place la sortie sur la carte
            Carte[sortie_position.X, sortie_position.Y] = 3;

            // On initialise le tableau des positions précédente pour la génération du labyrinthe
            pos_prec = new Stack<Point>();
            pos_prec.Push(sortie_position);
            pos_actuelle = sortie_position;

            // On initialise le tableau des cases possibles
            casespossibles = new bool[4];
            murs_possibles = new bool[4];

            // Puis on applique un algorithme pour générer le labyrinthe
            if (Config.laby_algo)
            {
                // Labyrinthe parfait
                GenLabyParfait();
            }
            else
            {
                // Labyrinthe imparfait
                GenLabyImparfait(sortie_position, pos_prec, 2);
            }

            // On place le joueur
            Point position_aleatoire = new Point(random.Next(size.X), random.Next(size.Y));

            int[,] matrice_distance_sortie_joueur = new int[size.X, size.Y];
            matrice_distance_sortie_joueur = IA.GenLabyIA(sortie_position, matrice_distance_sortie_joueur, laby);
            while (Carte[position_aleatoire.X, position_aleatoire.Y] == 1 || (matrice_distance_sortie_joueur[position_aleatoire.X, position_aleatoire.Y] <= (Size.X + Size.Y) / 2))
                position_aleatoire = new Point(random.Next(size.X), random.Next(size.Y));
            // On place le joueur dans le jeu (au milieu de la case qui a été choisie aléatoirement)
            joueur_position_initiale = new Vector3((position_aleatoire.X * cellSize.X) + cellSize.X / 2, cellSize.Y / 2, (position_aleatoire.Y * cellSize.Z) + cellSize.Z / 2);
            // On calcule sa position en case
            joueur_position_initiale_case = new Point((int)(joueur_position_initiale.X / laby.CellSize.X), (int)(joueur_position_initiale.Z / laby.CellSize.Z));
            // On place le joueur sur la carte
            Carte[position_aleatoire.X, position_aleatoire.Y] = 2;

            if (Config.modedejeu)
            {
                // On place le chasseur
                position_aleatoire = new Point(random.Next(size.X), random.Next(size.Y));
                // Calcul de la distance joueur - chasseur
                int[,] matrice_distance_joueur_chasseur = new int[size.X, size.Y];
                matrice_distance_joueur_chasseur = IA.GenLabyIA(joueur_position_initiale_case, matrice_distance_joueur_chasseur, laby);

                while (Carte[position_aleatoire.X, position_aleatoire.Y] != 0 || matrice_distance_joueur_chasseur[position_aleatoire.X, position_aleatoire.Y] <= (laby.Size.X + laby.Size.Y)/2)
                {
                    position_aleatoire.X = random.Next(size.X);
                    position_aleatoire.Y = random.Next(size.Y);
                }
                // On place le chasseur dans le jeu (au milieu de la case qui a été choisie aléatoirement)
                chasseur_position_initiale = new Vector3((position_aleatoire.X * cellSize.X) + cellSize.X / 2, 0, (position_aleatoire.Y * cellSize.Z) + cellSize.Z / 2);
                // On place le chasseur sur la carte
                Carte[position_aleatoire.X, position_aleatoire.Y] = 4;
            }
            else
            {
                // On place le scientifique
                position_aleatoire = new Point(random.Next(size.X), random.Next(size.Y));
                // Calcul de la distance joueur - scientifique
                int[,] matrice_distance_joueur_scientifique = new int[size.X, size.Y];
                matrice_distance_joueur_scientifique = IA.GenLabyIA(joueur_position_initiale_case, matrice_distance_joueur_scientifique, laby);
                // Calcul de la distance scientifique - sortie
                int[,] matrice_distance_sortie_scientifique = new int[size.X, size.Y];
                matrice_distance_sortie_scientifique = IA.GenLabyIA(sortie_position, matrice_distance_sortie_scientifique, laby);

                while (Carte[position_aleatoire.X, position_aleatoire.Y] == 1 || matrice_distance_joueur_scientifique[position_aleatoire.X, position_aleatoire.Y] <= (laby.Size.X + laby.Size.Y) / 3 || matrice_distance_sortie_scientifique[position_aleatoire.X, position_aleatoire.Y] <= (laby.Size.X + laby.Size.Y) / 2)
                {
                    position_aleatoire.X = random.Next(size.X);
                    position_aleatoire.Y = random.Next(size.Y);
                }
                // On place le joueur dans le jeu (au milieu de la case qui a été choisie aléatoirement)
                scientifique_position_initiale = new Vector3((position_aleatoire.X * cellSize.X) + cellSize.X / 2, 0, (position_aleatoire.Y * cellSize.Z) + cellSize.Z / 2);
                // On calcule sa position en case
                scientifique_position_initiale_case = new Point((int)(scientifique_position_initiale.X / laby.CellSize.X), (int)(scientifique_position_initiale.Z / laby.CellSize.Z));
                // On place le joueur sur la carte
                Carte[position_aleatoire.X, position_aleatoire.Y] = 4;
            }

            /*** On place les bonus ***/
            if (Config.active_bonus)
            {
                // On remplit toutes les cases non chemin avec des true pour ne pas poser de bonus dessus
                for (int y = 0; y < size.Y; y++)
                    for (int x = 0; x < size.X; x++)
                        if (Carte[x, y] != 0)
                            carte_espace_bonus[x, y] = true;
                nb_bonus = 0;
                bool plein_bonus = false;
                while (!plein_bonus)
                {
                    Point position_bonus = new Point(random.Next(size.X), random.Next(size.Y));
                    while (carte_espace_bonus[position_bonus.X, position_bonus.Y])
                    {
                        position_bonus = new Point(random.Next(size.X), random.Next(size.Y));
                    }
                    liste_bonus.Add(new Bonus(laby, Content, new Vector3((position_bonus.X * cellSize.X) + cellSize.X / 2, cellSize.Y / 8, (position_bonus.Y * cellSize.Z) + cellSize.Z / 2), liste_bonus_actifs));
                    Carte[position_bonus.X, position_bonus.Y] = 5;

                    // On indique les positions où ne peuvent pas apparaitre d'autre bonus
                    for (int x = 0; x < Config.largeur_labyrinthe; x++)
                        for (int y = 0; y < Config.hauteur_labyrinthe; y++)
                        {
                            double distance_case_bonus = Math.Sqrt(Math.Pow(liste_bonus[nb_bonus].position_case_bonus.X - x, 2) + Math.Pow(liste_bonus[nb_bonus].position_case_bonus.Y - y, 2));
                            if (distance_case_bonus <= ray(laby.Size) / 3)
                                carte_espace_bonus[x, y] = true;
                        }

                    int nb_true = 0;
                    for (int x = 0; x < Config.largeur_labyrinthe; x++)
                        for (int y = 0; y < Config.hauteur_labyrinthe; y++)
                            if (carte_espace_bonus[x, y])
                                nb_true++;

                    if (nb_true == Config.hauteur_labyrinthe * Config.largeur_labyrinthe)
                        plein_bonus = true;

                    nb_bonus++;
                }

                // On supprime des bonus en fonction de la configuration du jeu
                int bonus_aleatoire = random.Next(0, liste_bonus.Count);
                if (Config.nb_bonus_index == 0)
                {
                    for (int i = 0; i < nb_bonus / 1.2; i++)
                    {
                        Carte[liste_bonus[bonus_aleatoire].position_case_bonus.X, liste_bonus[bonus_aleatoire].position_case_bonus.Y] = 0;
                        liste_bonus.Remove(liste_bonus[bonus_aleatoire]);
                        bonus_aleatoire = random.Next(0, liste_bonus.Count);
                        nb_bonus--;
                    }
                }
                else if (Config.nb_bonus_index == 1)
                {
                    for (int i = 0; i < nb_bonus / 1.5; i++)
                    {
                        Carte[liste_bonus[bonus_aleatoire].position_case_bonus.X, liste_bonus[bonus_aleatoire].position_case_bonus.Y] = 0;
                        liste_bonus.Remove(liste_bonus[bonus_aleatoire]);
                        bonus_aleatoire = random.Next(0, liste_bonus.Count);
                        nb_bonus--;
                    }
                }
                if (Config.nb_bonus_index < 2)
                {
                    // Actualise la carte des apparitions possibles pour les bonus
                    for (int x = 0; x < Config.largeur_labyrinthe; x++)
                    {
                        for (int y = 0; y < Config.hauteur_labyrinthe; y++)
                        {
                            if (Carte[x, y] == 0)
                                carte_espace_bonus[x, y] = false;
                        }
                    }

                    // On indique les positions où ne peuvent pas apparaitre d'autres bonus
                    for (int x = 0; x < Config.largeur_labyrinthe; x++)
                    {
                        for (int y = 0; y < Config.hauteur_labyrinthe; y++)
                        {
                            foreach (Bonus bonus in liste_bonus)
                            {
                                double distance_case_bonus = Math.Sqrt(Math.Pow(bonus.position_case_bonus.X - x, 2) + Math.Pow(bonus.position_case_bonus.Y - y, 2));
                                if (distance_case_bonus <= ray(laby.Size) / 5)
                                {
                                    carte_espace_bonus[x, y] = true;
                                }
                            }
                        }
                    }
                }
            }
            /*** On place les pièges ***/
            if (Config.active_pieges)
            {
                for (int y = 0; y < size.Y; y++)
                    for (int x = 0; x < size.X; x++)
                        if (Carte[x, y] != 0)
                            carte_espace_pieges[x, y] = true;

                nb_pieges = 0;
                bool plein_piege = false;
                while (!plein_piege)
                {
                    Point position_pieges = new Point(random.Next(size.X), random.Next(size.Y));
                    while (Carte[position_pieges.X, position_pieges.Y] != 0 || carte_espace_pieges[position_pieges.X, position_pieges.Y])
                    {
                        position_pieges = new Point(random.Next(size.X), random.Next(size.Y));
                    }

                    liste_pieges.Add(new Pieges(laby, Content, new Vector3((position_pieges.X * cellSize.X) + cellSize.X / 2, 0, (position_pieges.Y * cellSize.Z) + cellSize.Z / 2)));

                    Carte[position_pieges.X, position_pieges.Y] = 6;
                    // On indique les positions où ne peuvent pas apparaître d'autre pieges
                    for (int x = 0; x < Config.largeur_labyrinthe; x++)
                    {
                        for (int y = 0; y < Config.hauteur_labyrinthe; y++)
                        {
                            foreach (Pieges pieges in liste_pieges)
                            {
                                double distance_case_pieges = Math.Sqrt(Math.Pow(pieges.position_case.X - x, 2) + Math.Pow(pieges.position_case.Y - y, 2));
                                if (distance_case_pieges <= ray(laby.Size) / 5)
                                    carte_espace_pieges[x, y] = true;
                            }
                        }
                    }
                    int nb_true = 0;
                    for (int x = 0; x < Config.largeur_labyrinthe; x++)
                    {
                        for (int y = 0; y < Config.hauteur_labyrinthe; y++)
                        {
                            if (carte_espace_pieges[x, y])
                                nb_true++;
                        }

                    }
                    if (nb_true == Config.hauteur_labyrinthe * Config.largeur_labyrinthe)
                        plein_piege = true;
                    nb_pieges++;
                }

                // Si il n'y a que les lasers d'activé
                if (!Config.trappe)
                {
                    for (int i = 0; i < liste_pieges.Count; i++)
                    {
                        if (!(
                        ((liste_pieges[i].position_case.Y == Config.hauteur_labyrinthe - 1 || laby.Carte[liste_pieges[i].position_case.X, liste_pieges[i].position_case.Y + 1] == 1) && (liste_pieges[i].position_case.Y == 0 || laby.Carte[liste_pieges[i].position_case.X, liste_pieges[i].position_case.Y - 1] == 1)) ||
                        ((liste_pieges[i].position_case.X == Config.largeur_labyrinthe - 1 || laby.Carte[liste_pieges[i].position_case.X + 1, liste_pieges[i].position_case.Y] == 1) && (liste_pieges[i].position_case.X == 0 || laby.Carte[liste_pieges[i].position_case.X - 1, liste_pieges[i].position_case.Y] == 1))))
                        {
                            Carte[liste_pieges[i].position_case.X, liste_pieges[i].position_case.Y] = 0;
                            liste_pieges.Remove(liste_pieges[i]);
                            nb_pieges--;
                            i--;
                        }
                    }
                }

                // On supprime des pièges en fonction de la configuration du jeu
                int piege_aleatoire = random.Next(0, liste_pieges.Count);
                if (Config.nb_pieges_index == 0)
                {
                    for (int i = 0; i < nb_pieges / 1.2; i++)
                    {
                        Carte[liste_pieges[piege_aleatoire].position_case.X, liste_pieges[piege_aleatoire].position_case.Y] = 0;
                        liste_pieges.Remove(liste_pieges[piege_aleatoire]);
                        piege_aleatoire = random.Next(0, liste_pieges.Count);
                        nb_pieges--;
                    }
                }
                else if (Config.nb_pieges_index == 1)
                {
                    for (int i = 0; i < nb_pieges / 1.5; i++)
                    {
                        Carte[liste_pieges[piege_aleatoire].position_case.X, liste_pieges[piege_aleatoire].position_case.Y] = 0;
                        liste_pieges.Remove(liste_pieges[piege_aleatoire]);
                        piege_aleatoire = random.Next(0, liste_pieges.Count);
                        nb_pieges--;
                    }
                }
                if (Config.nb_pieges_index < 2)
                {
                    // Actualise la carte des apparitions possibles pour les bonus
                    for (int x = 0; x < Config.largeur_labyrinthe; x++)
                    {
                        for (int y = 0; y < Config.hauteur_labyrinthe; y++)
                        {
                            if (Carte[x, y] == 0)
                                carte_espace_pieges[x, y] = false;
                        }
                    }

                    // On indique les positions où ne peuvent pas apparaitre d'autres bonus
                    for (int x = 0; x < Config.largeur_labyrinthe; x++)
                    {
                        for (int y = 0; y < Config.hauteur_labyrinthe; y++)
                        {
                            foreach (Pieges piege in liste_pieges)
                            {
                                double distance_case_piege = Math.Sqrt(Math.Pow(piege.position_case.X - x, 2) + Math.Pow(piege.position_case.Y - y, 2));
                                if (distance_case_piege <= ray(laby.Size) / 3)
                                {
                                    carte_espace_pieges[x, y] = true;
                                }
                            }
                        }
                    }
                }
            }
        }

        public int ray(Point taille)
        {
            return ((int)3 * ((taille.X + taille.Y) / 2) / 5);
        }

        public void Update(bool[,] carte_sortie)
        {
            this.carte_sortie = carte_sortie;
        }

        #region Algorithmes de génération du labyrinthe
        /** Génére un layrnithe parfait **/
        public void GenLabyParfait()
        {
            do
            {
                // On réinitialise le tableau
                for (int i = 0; i < 4; i++)
                {
                    casespossibles[i] = false;
                }
                // On recherche les cases possibles
                casespossibles = RechercheCasesPossibles();

                int j = 0;
                for (int i = 0; i < 4; i++)
                {
                    if (casespossibles[i] == false)
                        j++;
                }
                // Si aucune case n'est possible
                if (j == 4)
                {
                    // On retourne à la case précédente
                    pos_actuelle = pos_prec.Pop();
                }
                // Si au moins un case est possible 
                // => on en choisi une au hasard
                else
                {
                    int mur_alea = random.Next(4);
                    while (casespossibles[mur_alea] == false)
                    {
                        mur_alea = random.Next(4);
                    }

                    // Haut
                    if (mur_alea == 0)
                    {
                        // On pose un chemin
                        Carte[pos_actuelle.X, pos_actuelle.Y - 1] = 0;
                        // On stocke notre position actuelle
                        pos_prec.Push(pos_actuelle);
                        // On avance sur cette case
                        pos_actuelle.Y--;
                    }
                    // Droite
                    else if (mur_alea == 1)
                    {
                        // On pose un chemin
                        Carte[pos_actuelle.X + 1, pos_actuelle.Y] = 0;
                        // On stocke notre position actuelle
                        pos_prec.Push(pos_actuelle);
                        // On avance sur cette case
                        pos_actuelle.X++;
                    }
                    // Bas
                    else if (mur_alea == 2)
                    {
                        // On pose un chemin
                        Carte[pos_actuelle.X, pos_actuelle.Y + 1] = 0;
                        // On stocke notre position actuelle
                        pos_prec.Push(pos_actuelle);
                        // On avance sur cette case
                        pos_actuelle.Y++;
                    }
                    // Gauche
                    else if (mur_alea == 3)
                    {
                        // On pose un chemin
                        Carte[pos_actuelle.X - 1, pos_actuelle.Y] = 0;
                        // On stocke notre position actuelle
                        pos_prec.Push(pos_actuelle);
                        // On avance sur cette case
                        pos_actuelle.X--;
                    }
                }
            } while (pos_actuelle != sortie_position);
        }

        /** Recherche les cases qui peuvent être "creusées" autour de la case actuelle **/
        public bool[] RechercheCasesPossibles()
        {

            // On vérifie d'abord qu'on est pas sur un bord
            // Bord gauche
            if (pos_actuelle.X == 0)
            {
                // Coin en haut à gauche
                if (pos_actuelle.Y == 0)
                {
                    // Traitement des cases B et D
                    // Droite
                    // On regarde uniquement si ce n'était pas notre position précédente
                    if (pos_prec.Peek().X != pos_actuelle.X + 1)
                    {

                        murs_possibles[0] = false; // Haut
                        murs_possibles[1] = true; // Droite
                        murs_possibles[2] = true; // Bas
                        murs_possibles[3] = false; // Gauche

                        if (EntoureMurs(new Point(pos_actuelle.X + 1, pos_actuelle.Y), pos_actuelle, murs_possibles) && Carte[pos_actuelle.X + 1, pos_actuelle.Y] == 1)
                            casespossibles[1] = true;
                    }
                    // Bas
                    // On regarde uniquement si ce n'était pas notre position précédente
                    if (pos_prec.Peek().Y != pos_actuelle.Y + 1)
                    {
                        murs_possibles[0] = false; // Haut
                        murs_possibles[1] = true; // Droite
                        murs_possibles[2] = true; // Bas
                        murs_possibles[3] = false; // Gauche

                        if (EntoureMurs(new Point(pos_actuelle.X, pos_actuelle.Y + 1), pos_actuelle, murs_possibles) && Carte[pos_actuelle.X, pos_actuelle.Y + 1] == 1)
                            casespossibles[2] = true;
                    }
                }
                // Coin en bas à gauche
                else if (pos_actuelle.Y == Size.Y - 1)
                {
                    // Traitement des cases H et D
                    // Haut
                    // On regarde uniquement si ce n'était pas notre position précédente
                    if (pos_prec.Peek().Y != pos_actuelle.Y - 1)
                    {
                        murs_possibles[0] = true; // Haut
                        murs_possibles[1] = true; // Droite
                        murs_possibles[2] = false; // Bas
                        murs_possibles[3] = false; // Gauche

                        if (EntoureMurs(new Point(pos_actuelle.X, pos_actuelle.Y - 1), pos_actuelle, murs_possibles) && Carte[pos_actuelle.X, pos_actuelle.Y - 1] == 1)
                            casespossibles[0] = true;
                    }
                    // Droite
                    // On regarde uniquement si ce n'était pas notre position précédente
                    if (pos_prec.Peek().X != pos_actuelle.X + 1)
                    {
                        murs_possibles[0] = true; // Haut
                        murs_possibles[1] = true; // Droite
                        murs_possibles[2] = false; // Bas
                        murs_possibles[3] = false; // Gauche

                        if (EntoureMurs(new Point(pos_actuelle.X + 1, pos_actuelle.Y), pos_actuelle, murs_possibles) && Carte[pos_actuelle.X + 1, pos_actuelle.Y] == 1)
                            casespossibles[1] = true;
                    }
                }
                // Bord milieu gauche
                else
                {
                    // Traitement des cases H, B et D
                    // Haut
                    // On regarde uniquement si ce n'était pas notre position précédente
                    if (pos_prec.Peek().Y != pos_actuelle.Y - 1)
                    {
                        // On vérifie que c'est pas un coin
                        if (pos_actuelle.Y - 1 == 0)
                        {
                            murs_possibles[0] = false; // Haut
                            murs_possibles[1] = true; // Droite
                            murs_possibles[2] = true; // Bas
                            murs_possibles[3] = false; // Gauche

                            if (EntoureMurs(new Point(pos_actuelle.X, pos_actuelle.Y - 1), pos_actuelle, murs_possibles) && Carte[pos_actuelle.X, pos_actuelle.Y - 1] == 1)
                                casespossibles[0] = true;
                        }
                        else
                        {
                            murs_possibles[0] = true; // Haut
                            murs_possibles[1] = true; // Droite
                            murs_possibles[2] = true; // Bas
                            murs_possibles[3] = false; // Gauche

                            if (EntoureMurs(new Point(pos_actuelle.X, pos_actuelle.Y - 1), pos_actuelle, murs_possibles) && Carte[pos_actuelle.X, pos_actuelle.Y - 1] == 1)
                                casespossibles[0] = true;
                        }
                    }
                    // Droite
                    // On regarde uniquement si ce n'était pas notre position précédente
                    if (pos_prec.Peek().X != pos_actuelle.X + 1)
                    {
                        murs_possibles[0] = true; // Haut
                        murs_possibles[1] = true; // Droite
                        murs_possibles[2] = true; // Bas
                        murs_possibles[3] = false; // Gauche

                        if (EntoureMurs(new Point(pos_actuelle.X + 1, pos_actuelle.Y), pos_actuelle, murs_possibles) && Carte[pos_actuelle.X + 1, pos_actuelle.Y] == 1)
                            casespossibles[1] = true;
                    }
                    // Bas
                    // On regarde uniquement si ce n'était pas notre position précédente
                    if (pos_prec.Peek().Y != pos_actuelle.Y + 1)
                    {
                        // On vérifie que c'est pas un coin
                        if (pos_actuelle.Y + 1 == Size.Y - 1)
                        {
                            murs_possibles[0] = true; // Haut
                            murs_possibles[1] = true; // Droite
                            murs_possibles[2] = false; // Bas
                            murs_possibles[3] = false; // Gauche

                            if (EntoureMurs(new Point(pos_actuelle.X, pos_actuelle.Y + 1), pos_actuelle, murs_possibles) && Carte[pos_actuelle.X, pos_actuelle.Y + 1] == 1)
                                casespossibles[2] = true;
                        }
                        else
                        {
                            murs_possibles[0] = true; // Haut
                            murs_possibles[1] = true; // Droite
                            murs_possibles[2] = true; // Bas
                            murs_possibles[3] = false; // Gauche

                            if (EntoureMurs(new Point(pos_actuelle.X, pos_actuelle.Y + 1), pos_actuelle, murs_possibles) && Carte[pos_actuelle.X, pos_actuelle.Y + 1] == 1)
                                casespossibles[2] = true;
                        }
                    }
                }
            }
            // Bord droit
            else if (pos_actuelle.X == Size.X - 1)
            {
                // Coin en haut à droite
                if (pos_actuelle.Y == 0)
                {
                    // Traitement des cases B et G
                    // Bas
                    // On regarde uniquement si ce n'était pas notre position précédente
                    if (pos_prec.Peek().Y != pos_actuelle.Y + 1)
                    {
                        murs_possibles[0] = false; // Haut
                        murs_possibles[1] = false; // Droite
                        murs_possibles[2] = true; // Bas
                        murs_possibles[3] = true; // Gauche

                        if (EntoureMurs(new Point(pos_actuelle.X, pos_actuelle.Y + 1), pos_actuelle, murs_possibles) && Carte[pos_actuelle.X, pos_actuelle.Y + 1] == 1)
                            casespossibles[2] = true;
                    }
                    // Gauche
                    // On regarde uniquement si ce n'était pas notre position précédente
                    if (pos_prec.Peek().X != pos_actuelle.X - 1)
                    {
                        murs_possibles[0] = false; // Haut
                        murs_possibles[1] = false; // Droite
                        murs_possibles[2] = true; // Bas
                        murs_possibles[3] = true; // Gauche

                        if (EntoureMurs(new Point(pos_actuelle.X - 1, pos_actuelle.Y), pos_actuelle, murs_possibles) && Carte[pos_actuelle.X - 1, pos_actuelle.Y] == 1)
                            casespossibles[3] = true;
                    }
                }
                // Coin en bas à droite
                else if (pos_actuelle.Y == Size.Y - 1)
                {
                    // Traitement des cases H et G
                    // Haut
                    if (pos_prec.Peek().X != pos_actuelle.Y - 1)
                    {
                        murs_possibles[0] = true; // Haut
                        murs_possibles[1] = false; // Droite
                        murs_possibles[2] = false; // Bas
                        murs_possibles[3] = true; // Gauche

                        if (EntoureMurs(new Point(pos_actuelle.X, pos_actuelle.Y - 1), pos_actuelle, murs_possibles) && Carte[pos_actuelle.X, pos_actuelle.Y - 1] == 1)
                            casespossibles[0] = true;
                    }
                    // Gauche
                    if (pos_prec.Peek().X != pos_actuelle.X - 1)
                    {
                        murs_possibles[0] = true; // Haut
                        murs_possibles[1] = false; // Droite
                        murs_possibles[2] = false; // Bas
                        murs_possibles[3] = true; // Gauche

                        if (EntoureMurs(new Point(pos_actuelle.X - 1, pos_actuelle.Y), pos_actuelle, murs_possibles) && Carte[pos_actuelle.X - 1, pos_actuelle.Y] == 1)
                            casespossibles[3] = true;
                    }
                }
                // Bord milieu droit
                else
                {
                    // Traitement des cases H, B et G
                    // Haut
                    // On regarde uniquement si ce n'était pas notre position précédente
                    if (pos_prec.Peek().Y != pos_actuelle.Y - 1)
                    {
                        // On vérifie que c'est pas un coin
                        if (pos_actuelle.Y - 1 == 0)
                        {
                            murs_possibles[0] = false; // Haut
                            murs_possibles[1] = false; // Droite
                            murs_possibles[2] = true; // Bas
                            murs_possibles[3] = true; // Gauche

                            if (EntoureMurs(new Point(pos_actuelle.X, pos_actuelle.Y - 1), pos_actuelle, murs_possibles) && Carte[pos_actuelle.X, pos_actuelle.Y - 1] == 1)
                                casespossibles[0] = true;
                        }
                        else
                        {
                            murs_possibles[0] = true; // Haut
                            murs_possibles[1] = false; // Droite
                            murs_possibles[2] = true; // Bas
                            murs_possibles[3] = true; // Gauche

                            if (EntoureMurs(new Point(pos_actuelle.X, pos_actuelle.Y - 1), pos_actuelle, murs_possibles) && Carte[pos_actuelle.X, pos_actuelle.Y - 1] == 1)
                                casespossibles[0] = true;
                        }
                    }
                    // Bas
                    // On regarde uniquement si ce n'était pas notre position précédente
                    if (pos_prec.Peek().Y != pos_actuelle.Y + 1)
                    {
                        // On vérifie que c'est pas un coin
                        if (pos_actuelle.Y + 1 == Size.Y - 1)
                        {
                            murs_possibles[0] = true; // Haut
                            murs_possibles[1] = false; // Droite
                            murs_possibles[2] = false; // Bas
                            murs_possibles[3] = true; // Gauche

                            if (EntoureMurs(new Point(pos_actuelle.X, pos_actuelle.Y + 1), pos_actuelle, murs_possibles) && Carte[pos_actuelle.X, pos_actuelle.Y + 1] == 1)
                                casespossibles[2] = true;
                        }
                        else
                        {
                            murs_possibles[0] = true; // Haut
                            murs_possibles[1] = false; // Droite
                            murs_possibles[2] = true; // Bas
                            murs_possibles[3] = true; // Gauche

                            if (EntoureMurs(new Point(pos_actuelle.X, pos_actuelle.Y + 1), pos_actuelle, murs_possibles) && Carte[pos_actuelle.X, pos_actuelle.Y + 1] == 1)
                                casespossibles[2] = true;
                        }
                    }
                    // Gauche
                    // On regarde uniquement si ce n'était pas notre position précédente
                    if (pos_prec.Peek().X != pos_actuelle.X - 1)
                    {
                        murs_possibles[0] = true; // Haut
                        murs_possibles[1] = false; // Droite
                        murs_possibles[2] = true; // Bas
                        murs_possibles[3] = true; // Gauche

                        if (EntoureMurs(new Point(pos_actuelle.X - 1, pos_actuelle.Y), pos_actuelle, murs_possibles) && Carte[pos_actuelle.X - 1, pos_actuelle.Y] == 1)
                            casespossibles[3] = true;
                    }
                }
            }
            // Les coins ont été traités
            // Bord haut
            else if (pos_actuelle.Y == 0)
            {
                // Traitement des cases D, G et B
                // Droite
                // On regarde uniquement si ce n'était pas notre position précédente
                if (pos_prec.Peek().X != pos_actuelle.X + 1)
                {
                    // On vérifie que c'est pas un coin
                    if (pos_actuelle.X + 1 == Size.X - 1)
                    {
                        murs_possibles[0] = false; // Haut
                        murs_possibles[1] = false; // Droite
                        murs_possibles[2] = true; // Bas
                        murs_possibles[3] = true; // Gauche

                        if (EntoureMurs(new Point(pos_actuelle.X + 1, pos_actuelle.Y), pos_actuelle, murs_possibles) && Carte[pos_actuelle.X + 1, pos_actuelle.Y] == 1)
                            casespossibles[1] = true;
                    }
                    else
                    {
                        murs_possibles[0] = false; // Haut
                        murs_possibles[1] = true; // Droite
                        murs_possibles[2] = true; // Bas
                        murs_possibles[3] = true; // Gauche

                        if (EntoureMurs(new Point(pos_actuelle.X + 1, pos_actuelle.Y), pos_actuelle, murs_possibles) && Carte[pos_actuelle.X + 1, pos_actuelle.Y] == 1)
                            casespossibles[1] = true;
                    }
                }
                // Bas
                // On regarde uniquement si ce n'était pas notre position précédente
                if (pos_prec.Peek().Y != pos_actuelle.Y + 1)
                {
                    murs_possibles[0] = false; // Haut
                    murs_possibles[1] = true; // Droite
                    murs_possibles[2] = true; // Bas
                    murs_possibles[3] = true; // Gauche

                    if (EntoureMurs(new Point(pos_actuelle.X, pos_actuelle.Y + 1), pos_actuelle, murs_possibles) && Carte[pos_actuelle.X, pos_actuelle.Y + 1] == 1)
                        casespossibles[2] = true;
                }
                // Gauche
                // On regarde uniquement si ce n'était pas notre position précédente
                if (pos_prec.Peek().X != pos_actuelle.X - 1)
                {
                    // On vérifie que c'est pas un coin
                    if (pos_actuelle.X - 1 == 0)
                    {
                        murs_possibles[0] = false; // Haut
                        murs_possibles[1] = true; // Droite
                        murs_possibles[2] = true; // Bas
                        murs_possibles[3] = false; // Gauche

                        if (EntoureMurs(new Point(pos_actuelle.X - 1, pos_actuelle.Y), pos_actuelle, murs_possibles) && Carte[pos_actuelle.X - 1, pos_actuelle.Y] == 1)
                            casespossibles[3] = true;
                    }
                    else
                    {
                        murs_possibles[0] = false; // Haut
                        murs_possibles[1] = true; // Droite
                        murs_possibles[2] = true; // Bas
                        murs_possibles[3] = true; // Gauche

                        if (EntoureMurs(new Point(pos_actuelle.X - 1, pos_actuelle.Y), pos_actuelle, murs_possibles) && Carte[pos_actuelle.X - 1, pos_actuelle.Y] == 1)
                            casespossibles[3] = true;
                    }
                }
            }
            // Bord bas
            else if (pos_actuelle.Y == Size.Y - 1)
            {
                // Traitement des cases D, G et H
                // Haut
                // On regarde uniquement si ce n'était pas notre position précédente
                if (pos_prec.Peek().Y != pos_actuelle.Y - 1)
                {
                    murs_possibles[0] = true; // Haut
                    murs_possibles[1] = true; // Droite
                    murs_possibles[2] = false; // Bas
                    murs_possibles[3] = true; // Gauche

                    if (EntoureMurs(new Point(pos_actuelle.X, pos_actuelle.Y - 1), pos_actuelle, murs_possibles) && Carte[pos_actuelle.X, pos_actuelle.Y - 1] == 1)
                        casespossibles[0] = true;
                }
                // Droite
                // On regarde uniquement si ce n'était pas notre position précédente
                if (pos_prec.Peek().X != pos_actuelle.X + 1)
                {
                    // On vérifie que c'est pas un coin
                    if (pos_actuelle.X + 1 == Size.X - 1)
                    {
                        murs_possibles[0] = true; // Haut
                        murs_possibles[1] = false; // Droite
                        murs_possibles[2] = false; // Bas
                        murs_possibles[3] = true; // Gauche

                        if (EntoureMurs(new Point(pos_actuelle.X + 1, pos_actuelle.Y), pos_actuelle, murs_possibles) && Carte[pos_actuelle.X + 1, pos_actuelle.Y] == 1)
                            casespossibles[1] = true;
                    }
                    else
                    {
                        murs_possibles[0] = true; // Haut
                        murs_possibles[1] = true; // Droite
                        murs_possibles[2] = false; // Bas
                        murs_possibles[3] = true; // Gauche

                        if (EntoureMurs(new Point(pos_actuelle.X + 1, pos_actuelle.Y), pos_actuelle, murs_possibles) && Carte[pos_actuelle.X + 1, pos_actuelle.Y] == 1)
                            casespossibles[1] = true;
                    }
                }
                // Gauche
                // On regarde uniquement si ce n'était pas notre position précédente
                if (pos_prec.Peek().X != pos_actuelle.X - 1)
                {
                    // On vérifie que c'est pas un coin
                    if (pos_actuelle.X - 1 == 0)
                    {
                        murs_possibles[0] = true; // Haut
                        murs_possibles[1] = true; // Droite
                        murs_possibles[2] = false; // Bas
                        murs_possibles[3] = false; // Gauche

                        if (EntoureMurs(new Point(pos_actuelle.X - 1, pos_actuelle.Y), pos_actuelle, murs_possibles) && Carte[pos_actuelle.X - 1, pos_actuelle.Y] == 1)
                            casespossibles[3] = true;
                    }
                    else
                    {
                        murs_possibles[0] = true; // Haut
                        murs_possibles[1] = true; // Droite
                        murs_possibles[2] = false; // Bas
                        murs_possibles[3] = true; // Gauche

                        if (EntoureMurs(new Point(pos_actuelle.X - 1, pos_actuelle.Y), pos_actuelle, murs_possibles) && Carte[pos_actuelle.X - 1, pos_actuelle.Y] == 1)
                            casespossibles[3] = true;
                    }
                }
            }

            // Traitement de base sur les 4 cases voisines
            else
            {
                // Traitement sur les cases H, B, G et D
                // Haut
                // On regarde uniquement si ce n'était pas notre position précédente
                if (pos_prec.Peek().Y != pos_actuelle.Y - 1)
                {
                    // On vérifie qu'on ne se trouve pas tout en haut
                    if (pos_actuelle.Y - 1 == 0)
                    {
                        murs_possibles[0] = false; // Haut
                        murs_possibles[1] = true; // Droite
                        murs_possibles[2] = true; // Bas
                        murs_possibles[3] = true; // Gauche

                        if (EntoureMurs(new Point(pos_actuelle.X, pos_actuelle.Y - 1), pos_actuelle, murs_possibles) && Carte[pos_actuelle.X, pos_actuelle.Y - 1] == 1)
                            casespossibles[0] = true;
                    }
                    else
                    {
                        murs_possibles[0] = true; // Haut
                        murs_possibles[1] = true; // Droite
                        murs_possibles[2] = true; // Bas
                        murs_possibles[3] = true; // Gauche

                        if (EntoureMurs(new Point(pos_actuelle.X, pos_actuelle.Y - 1), pos_actuelle, murs_possibles) && Carte[pos_actuelle.X, pos_actuelle.Y - 1] == 1)
                            casespossibles[0] = true;
                    }
                }
                // Droite
                // On regarde uniquement si ce n'était pas notre position précédente
                if (pos_prec.Peek().X != pos_actuelle.X + 1)
                {
                    // On vérifie qu'on ne se trouve pas tout à droite
                    if (pos_actuelle.X + 1 == Size.X - 1)
                    {
                        murs_possibles[0] = true; // Haut
                        murs_possibles[1] = false; // Droite
                        murs_possibles[2] = true; // Bas
                        murs_possibles[3] = true; // Gauche

                        if (EntoureMurs(new Point(pos_actuelle.X + 1, pos_actuelle.Y), pos_actuelle, murs_possibles) && Carte[pos_actuelle.X + 1, pos_actuelle.Y] == 1)
                            casespossibles[1] = true;
                    }
                    else
                    {
                        murs_possibles[0] = true; // Haut
                        murs_possibles[1] = true; // Droite
                        murs_possibles[2] = true; // Bas
                        murs_possibles[3] = true; // Gauche

                        if (EntoureMurs(new Point(pos_actuelle.X + 1, pos_actuelle.Y), pos_actuelle, murs_possibles) && Carte[pos_actuelle.X + 1, pos_actuelle.Y] == 1)
                            casespossibles[1] = true;
                    }
                }
                // Bas
                // On regarde uniquement si ce n'était pas notre position précédente
                if (pos_prec.Peek().Y != pos_actuelle.Y + 1)
                {
                    // On vérifie qu'on ne se trouve pas tout en bas
                    if (pos_actuelle.Y + 1 == Size.Y - 1)
                    {
                        murs_possibles[0] = true; // Haut
                        murs_possibles[1] = true; // Droite
                        murs_possibles[2] = false; // Bas
                        murs_possibles[3] = true; // Gauche

                        if (EntoureMurs(new Point(pos_actuelle.X, pos_actuelle.Y + 1), pos_actuelle, murs_possibles) && Carte[pos_actuelle.X, pos_actuelle.Y + 1] == 1)
                            casespossibles[2] = true;
                    }
                    else
                    {
                        murs_possibles[0] = true; // Haut
                        murs_possibles[1] = true; // Droite
                        murs_possibles[2] = true; // Bas
                        murs_possibles[3] = true; // Gauche

                        if (EntoureMurs(new Point(pos_actuelle.X, pos_actuelle.Y + 1), pos_actuelle, murs_possibles) && Carte[pos_actuelle.X, pos_actuelle.Y + 1] == 1)
                            casespossibles[2] = true;
                    }
                }
                // Gauche
                // On regarde uniquement si ce n'était pas notre position précédente
                if (pos_prec.Peek().X != pos_actuelle.X - 1)
                {
                    // On vérifie qu'on ne se trouve pas tout à gauche
                    if (pos_actuelle.X - 1 == 0)
                    {
                        murs_possibles[0] = true; // Haut
                        murs_possibles[1] = true; // Droite
                        murs_possibles[2] = true; // Bas
                        murs_possibles[3] = false; // Gauche

                        if (EntoureMurs(new Point(pos_actuelle.X - 1, pos_actuelle.Y), pos_actuelle, murs_possibles) && Carte[pos_actuelle.X - 1, pos_actuelle.Y] == 1)
                            casespossibles[3] = true;
                    }
                    else
                    {
                        murs_possibles[0] = true; // Haut
                        murs_possibles[1] = true; // Droite
                        murs_possibles[2] = true; // Bas
                        murs_possibles[3] = true; // Gauche

                        if (EntoureMurs(new Point(pos_actuelle.X - 1, pos_actuelle.Y), pos_actuelle, murs_possibles) && Carte[pos_actuelle.X - 1, pos_actuelle.Y] == 1)
                            casespossibles[3] = true;
                    }
                }

            }

            return casespossibles;
        }

        /** Vérifie qu'une case est bien entourée de murs ! **/
        public bool EntoureMurs(Point pos_mur, Point pos_mur_exclu, bool[] murs_possibles)
        {

            return ((!murs_possibles[1] || Carte[pos_mur.X + 1, pos_mur.Y] == 1 || pos_mur.X + 1 == pos_mur_exclu.X) &&
                (!murs_possibles[3] || Carte[pos_mur.X - 1, pos_mur.Y] == 1 || pos_mur.X - 1 == pos_mur_exclu.X) &&
                (!murs_possibles[2] || Carte[pos_mur.X, pos_mur.Y + 1] == 1 || pos_mur.Y + 1 == pos_mur_exclu.Y) &&
                (!murs_possibles[0] || Carte[pos_mur.X, pos_mur.Y - 1] == 1 || pos_mur.Y - 1 == pos_mur_exclu.Y));
        }

        /** Génére un labyrinthe imparfait **/
        public void GenLabyImparfait(Point pos_actuelle, Stack<Point> pos_prec, int quantite_murs)
        {
            GenLabyParfait();

            // On retire quelques murs en plus
            int nb_murs = ((Size.X + Size.Y) / 2) * quantite_murs;
            Point position_aleatoire = new Point(0, 0);

            for (int i = 1; i <= nb_murs; i++)
            {
                position_aleatoire = new Point(random.Next(Size.X), random.Next(Size.Y));
                while (Carte[position_aleatoire.X, position_aleatoire.Y] != 1)
                {
                    position_aleatoire = new Point(random.Next(Size.X), random.Next(Size.Y));
                }
                Carte[position_aleatoire.X, position_aleatoire.Y] = 0;
            }
        }
        #endregion
    }

    #region Les Vertices
    /**********************************************************************/
    /**************************** Les Vertices ****************************/
    /**********************************************************************/

    class Material : IDisposable
    {
        public GraphicsDevice GraphicsDevice;
        public VertexBuffer VertexBuffer;
        public IndexBuffer IndexBuffer;
        public BasicEffect Effect;
        public VertexDeclaration VertexDeclaration;
        public Texture2D Texture2D;
        public bool resolution;
        public PrimitiveType PrimitiveType;
        public int VertexStride;
        public int VertexCount;
        public int IndexCount;
        public int PrimitiveCount;
        public bool IsValid { get { return PrimitiveCount > 0; } }

        public void Dispose()
        {
            if (VertexBuffer != null) VertexBuffer.Dispose();
            if (IndexBuffer != null) IndexBuffer.Dispose();
            if (Effect != null) Effect.Dispose();
            if (VertexDeclaration != null) VertexDeclaration.Dispose();
        }
    }

    static class MaterialRenderer
    {
        public static void Draw(GameTime gameTime, List<Material> materials, Matrix worldMatrix, Matrix viewMatrix, Matrix projectionMatrix)
        {
            if (materials != null)
            {
                foreach (Material material in materials)
                {
                    if (material != null && material.IsValid)
                    {
                        Draw(gameTime, material, worldMatrix, viewMatrix, projectionMatrix);
                    }
                }
            }
        }

        static void Draw(GameTime gameTime, Material material, Matrix worldMatrix, Matrix viewMatrix, Matrix projectionMatrix)
        {
            material.GraphicsDevice.VertexDeclaration = material.VertexDeclaration;
            material.GraphicsDevice.Vertices[0].SetSource(material.VertexBuffer, 0, material.VertexStride);
            material.GraphicsDevice.Indices = material.IndexBuffer;

            material.GraphicsDevice.RenderState.DepthBufferEnable = true;
            material.GraphicsDevice.RenderState.AlphaTestEnable = false;
            material.GraphicsDevice.RenderState.AlphaBlendEnable = false;

            material.GraphicsDevice.RenderState.CullMode = CullMode.None;

            BasicEffect effect = material.Effect;

            effect.View = viewMatrix;
            effect.Projection = projectionMatrix;
            effect.World = worldMatrix;
            effect.TextureEnabled = ((effect.Texture = material.Texture2D) != null);
            effect.VertexColorEnabled = false;

            effect.Begin();
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Begin();
                material.GraphicsDevice.DrawIndexedPrimitives(material.PrimitiveType, 0, 0, material.VertexCount, 0, material.PrimitiveCount);
                pass.End();
            }
            effect.End();
        }
    }

    class MeshMaker
    {
        class MaterialHolder
        {
            public Material Material;
            public List<VertexPositionNormalTexture> Vertices;
            public List<int> Indices;

            public MaterialHolder()
            {
                Material = new Material();
                Vertices = new List<VertexPositionNormalTexture>();
                Indices = new List<int>();
            }
        }

        List<MaterialHolder> materials = new List<MaterialHolder>();

        public List<Material> BuildMaterials(GraphicsDevice graphicsDevice)
        {
            List<Material> result = new List<Material>();

            foreach (MaterialHolder holder in materials)
            {
                if (holder.Vertices.Count > 0)
                {
                    BuildMaterial(holder, graphicsDevice);
                    result.Add(holder.Material);
                }
            }

            return result;
        }

        void BuildMaterial(MaterialHolder holder, GraphicsDevice graphicsDevice)
        {
            Material m = holder.Material;
            m.GraphicsDevice = graphicsDevice;
            m.VertexDeclaration = new VertexDeclaration(graphicsDevice, VertexPositionNormalTexture.VertexElements);
            m.Effect = new BasicEffect(graphicsDevice, null);
            if(m.resolution)
                m.Effect.DiffuseColor = new Vector3(255, 255, 0);
            else
                m.Effect.DiffuseColor = new Vector3(255, 255, 255);

            m.Effect.AmbientLightColor = new Vector3(0, 0, 0);

            m.PrimitiveType = PrimitiveType.TriangleList;

            m.VertexStride = m.VertexDeclaration.GetVertexStrideSize(0);
            m.VertexCount = holder.Vertices.Count;
            m.IndexCount = holder.Indices.Count;
            m.PrimitiveCount = m.IndexCount / 3;

            m.VertexBuffer = new VertexBuffer(graphicsDevice, typeof(VertexPositionNormalTexture), m.VertexCount, BufferUsage.None);
            m.VertexBuffer.SetData(holder.Vertices.ToArray());

            m.IndexBuffer = new IndexBuffer(graphicsDevice, typeof(int), m.IndexCount, BufferUsage.None);
            m.IndexBuffer.SetData(holder.Indices.ToArray());
        }

        public void AddQuad(
            VertexPositionNormalTexture v1,
            VertexPositionNormalTexture v2,
            VertexPositionNormalTexture v3,
            VertexPositionNormalTexture v4,
            Texture2D texture, bool resolution)
        {
            MaterialHolder holder = FindOrCreateMaterial(texture, resolution);

            int indexOffset = holder.Vertices.Count;

            holder.Vertices.Add(v1);
            holder.Vertices.Add(v2);
            holder.Vertices.Add(v3);
            holder.Vertices.Add(v4);

            holder.Indices.Add(indexOffset + 0);
            holder.Indices.Add(indexOffset + 1);
            holder.Indices.Add(indexOffset + 2);
            holder.Indices.Add(indexOffset + 0);
            holder.Indices.Add(indexOffset + 2);
            holder.Indices.Add(indexOffset + 3);
        }

        MaterialHolder FindOrCreateMaterial(Texture2D texture, bool resolution)
        {
            foreach (MaterialHolder holder in materials)
            {
                if (holder.Material.Texture2D == texture && holder.Material.resolution == resolution)
                    return holder;
            }

            MaterialHolder result = new MaterialHolder();
            result.Material.Texture2D = texture;
            result.Material.resolution = resolution;
            materials.Add(result);
            return result;
        }
    }
    class MazeMaterialBuilder
    {
        Labyrinthe laby;
        int[,] carte;

        public List<Material> BuildMazeMaterial(Labyrinthe laby, GraphicsDevice graphicsDevice, Texture2D[] textures, bool fil_ariane, bool[,] carte_resolution)
        {
            this.laby = laby;
            carte = laby.Carte;

            MeshMaker maker = new MeshMaker();

            for (int y = 0; y < laby.Size.Y; y++)
                for (int x = 0; x < laby.Size.X; x++)
                    BuildMazeMaterial(maker, x, y, textures, fil_ariane, carte_resolution);

            return maker.BuildMaterials(graphicsDevice);
        }

        void BuildMazeMaterial(MeshMaker maker, int x, int y, Texture2D[] textures, bool fil_ariane, bool[,] carte_resolution)
        {
            // S'il s'agit d'un mur
            if (carte[x, y] == 1)
            {
                if (x != 0)
                {
                    if (carte[x - 1, y] != 1 && carte[x - 1, y] != 3)
                        AddWestWall(maker, x, 0, y, textures[1]);
                    else if (carte[x - 1, y] == 3)
                        AddWestWall(maker, x, 0, y, textures[2]);
                }
                if (x != laby.Size.X - 1)
                {
                    if (carte[x + 1, y] != 1 && carte[x + 1, y] != 3)
                        AddEastWall(maker, x, 0, y, textures[1]);
                    else if (carte[x + 1, y] == 3)
                        AddEastWall(maker, x, 0, y, textures[2]);
                }
                if (y != 0)
                {
                    if (carte[x, y - 1] != 1 && carte[x, y - 1] != 3)
                        AddSouthWall(maker, x, 0, y, textures[1]);
                    else if (carte[x, y - 1] == 3)
                        AddSouthWall(maker, x, 0, y, textures[2]);
                }
                if (y != laby.Size.Y - 1)
                {
                    if (carte[x, y + 1] != 1 && carte[x, y + 1] != 3)
                        AddNorthWall(maker, x, 0, y, textures[1]);
                    else if (carte[x, y + 1] == 3)
                        AddNorthWall(maker, x, 0, y, textures[2]);
                }
            }
            // Sinon
            else
            {

                // Si la case est différent de la sortie
                if (carte[x, y] != 3)
                {
                    bool trappe = false;
                    AddRoof(maker, x, 0, y, textures[0]);
                    if (laby.Carte[x, y] == 6)
                    {
                        foreach (Pieges piege in laby.liste_pieges)
                        {
                            if (piege.position_case.X == x && piege.position_case.Y == y && piege.type == 0)
                            {
                                AddFloor(maker, x, -1, y, textures[6], false);
                                AddWestWall(maker, x, -1, y, textures[5]);
                                AddEastWall(maker, x, -1, y, textures[5]);
                                AddSouthWall(maker, x, -1, y, textures[5]);
                                AddNorthWall(maker, x, -1, y, textures[5]);

                                trappe = true;
                            }
                        }
                    }
                    if (!trappe)
                    {
                        // Si resolution
                        if(fil_ariane && carte_resolution[x, y])
                            AddFloor(maker, x, 0, y, textures[4], true);
                        else
                            AddFloor(maker, x, 0, y, textures[4], false);
                    }
                }
                // Si c'est la sortie
                else
                {
                    // Bord gauche
                    if (x == 0)
                    {
                        AddWestWall(maker, x, 0, y, textures[3]);
                        AddWestWall(maker, x, 1, y, textures[3]);
                    }
                    else
                        AddWestWall(maker, x, 1, y, textures[2]);

                    if (x == laby.Size.X - 1)
                    {
                        AddEastWall(maker, x, 0, y, textures[3]);
                        AddEastWall(maker, x, 1, y, textures[3]);
                    }
                    else
                        AddEastWall(maker, x, 1, y, textures[2]);

                    if (y == 0)
                    {
                        AddSouthWall(maker, x, 0, y, textures[3]);
                        AddSouthWall(maker, x, 1, y, textures[3]);
                    }
                    else
                        AddSouthWall(maker, x, 1, y, textures[2]);

                    if (y == laby.Size.Y - 1)
                    {
                        AddNorthWall(maker, x, 0, y, textures[3]);
                        AddNorthWall(maker, x, 1, y, textures[3]);
                    }
                    else
                        AddNorthWall(maker, x, 1, y, textures[2]);
                    
                    AddFloor(maker, x, 0, y, textures[7], false);
                    AddRoof(maker, x, 1, y, textures[7]);

                }
                    
                if (x == 0 && carte[x, y] != 3)
                    AddWestWall(maker, x, 0, y, textures[1]);
                if (x == laby.Size.X - 1 && carte[x, y] != 3)
                    AddEastWall(maker, x, 0, y, textures[1]);
                if (y == 0 && carte[x, y] != 3)
                    AddSouthWall(maker, x, 0, y, textures[1]);
                if (y == laby.Size.Y - 1 && carte[x, y] != 3)
                    AddNorthWall(maker, x, 0, y, textures[1]);

            }
        }

        void AddRoof(MeshMaker maker, int x, int z, int y, Texture2D texture)
        {
            VertexPositionNormalTexture v1, v2, v3, v4;

            v1.Position = new Vector3(x, z + 1, y) * laby.CellSize;
            v2.Position = new Vector3(x, z + 1, y + 1) * laby.CellSize;
            v3.Position = new Vector3(x + 1, z + 1, y + 1) * laby.CellSize;
            v4.Position = new Vector3(x + 1, z + 1, y) * laby.CellSize;

            v1.Normal = v2.Normal = v3.Normal = v4.Normal = Vector3.Up;

            v1.TextureCoordinate = new Vector2(0, 1);
            v2.TextureCoordinate = new Vector2(0, 0);
            v3.TextureCoordinate = new Vector2(1, 0);
            v4.TextureCoordinate = new Vector2(1, 1);

            maker.AddQuad(v1, v2, v3, v4, texture, false);
        }

        void AddFloor(MeshMaker maker, int x, int z, int y, Texture2D texture, bool resolution)
        {
            VertexPositionNormalTexture v1, v2, v3, v4;

            v1.Position = new Vector3(x, z, y) * laby.CellSize;
            v2.Position = new Vector3(x, z, y + 1) * laby.CellSize;
            v3.Position = new Vector3(x + 1, z, y + 1) * laby.CellSize;
            v4.Position = new Vector3(x + 1, z, y) * laby.CellSize;

            v1.Normal = v2.Normal = v3.Normal = v4.Normal = Vector3.Up;

            v1.TextureCoordinate = new Vector2(0, 1);
            v2.TextureCoordinate = new Vector2(0, 0);
            v3.TextureCoordinate = new Vector2(1, 0);
            v4.TextureCoordinate = new Vector2(1, 1);

            maker.AddQuad(v1, v2, v3, v4, texture, resolution);
        }

        void AddWestWall(MeshMaker maker, int x, int z, int y, Texture2D texture)
        {
            VertexPositionNormalTexture v1, v2, v3, v4;

            v1.Position = new Vector3(x, z, y) * laby.CellSize;
            v2.Position = new Vector3(x, z + 1, y) * laby.CellSize;
            v3.Position = new Vector3(x, z + 1, y + 1) * laby.CellSize;
            v4.Position = new Vector3(x, z, y + 1) * laby.CellSize;

            v1.Normal = v2.Normal = v3.Normal = v4.Normal = Vector3.Left;

            v1.TextureCoordinate = new Vector2(0, 1);
            v2.TextureCoordinate = new Vector2(0, 0);
            v3.TextureCoordinate = new Vector2(1, 0);
            v4.TextureCoordinate = new Vector2(1, 1);

            maker.AddQuad(v1, v2, v3, v4, texture, false);
        }

        void AddEastWall(MeshMaker maker, int x, int z, int y, Texture2D texture)
        {
            VertexPositionNormalTexture v1, v2, v3, v4;

            v1.Position = new Vector3(x + 1, z, y + 1) * laby.CellSize;
            v2.Position = new Vector3(x + 1, z + 1, y + 1) * laby.CellSize;
            v3.Position = new Vector3(x + 1, z + 1, y) * laby.CellSize;
            v4.Position = new Vector3(x + 1, z, y) * laby.CellSize;

            v1.Normal = v2.Normal = v3.Normal = v4.Normal = Vector3.Right;

            v1.TextureCoordinate = new Vector2(0, 1);
            v2.TextureCoordinate = new Vector2(0, 0);
            v3.TextureCoordinate = new Vector2(1, 0);
            v4.TextureCoordinate = new Vector2(1, 1);

            maker.AddQuad(v1, v2, v3, v4, texture, false);
        }

        void AddNorthWall(MeshMaker maker, int x, int z, int y, Texture2D texture)
        {
            VertexPositionNormalTexture v1, v2, v3, v4;

            v1.Position = new Vector3(x, z, y + 1) * laby.CellSize;
            v2.Position = new Vector3(x, z + 1, y + 1) * laby.CellSize;
            v3.Position = new Vector3(x + 1, z + 1, y + 1) * laby.CellSize;
            v4.Position = new Vector3(x + 1, z, y + 1) * laby.CellSize;

            v1.Normal = v2.Normal = v3.Normal = v4.Normal = Vector3.Backward;

            v1.TextureCoordinate = new Vector2(0, 1);
            v2.TextureCoordinate = new Vector2(0, 0);
            v3.TextureCoordinate = new Vector2(1, 0);
            v4.TextureCoordinate = new Vector2(1, 1);

            maker.AddQuad(v1, v2, v3, v4, texture, false);
        }

        void AddSouthWall(MeshMaker maker, int x, int z, int y, Texture2D texture)
        {
            VertexPositionNormalTexture v1, v2, v3, v4;

            v1.Position = new Vector3(x + 1, z, y) * laby.CellSize;
            v2.Position = new Vector3(x + 1, z + 1, y) * laby.CellSize;
            v3.Position = new Vector3(x, z + 1, y) * laby.CellSize;
            v4.Position = new Vector3(x, z, y) * laby.CellSize;

            v1.Normal = v2.Normal = v3.Normal = v4.Normal = Vector3.Backward;

            v1.TextureCoordinate = new Vector2(0, 1);
            v2.TextureCoordinate = new Vector2(0, 0);
            v3.TextureCoordinate = new Vector2(1, 0);
            v4.TextureCoordinate = new Vector2(1, 1);

            maker.AddQuad(v1, v2, v3, v4, texture, false);
        }
    }
    #endregion
}