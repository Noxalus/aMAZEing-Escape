using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections;
using Microsoft.Xna.Framework.Input;

namespace aMAZEing_Escape
{
    class IA
    {
        public int[,] carte_ia;
        private Point taille;
        private Point pos_ia;
        private bool[,] carte_ia_acces;
        private Labyrinthe laby;
        private Stack<Point> pos_prec;
        private int EspaceTexte;
        private int BordGaucheIA;
        private int BordHautIA;
        public Point case_eloigne;
        private int idmax;

        private bool[,] carte_ia_inter;
        private int[,] nb_chemin;
        public int[,] carte_direct;

        public int[,] carte_ia2;
        public bool[,] carte_impasse;
        bool une_premiere_fois;

        public IA(Labyrinthe labyrinthe, Point position_joueur, Viewport vp)
        {
            laby = labyrinthe;
            taille = new Point(laby.Size.X, laby.Size.Y);
            carte_ia = new int[taille.X, taille.Y];
            pos_prec = new Stack<Point>();

            pos_ia = position_joueur;
            carte_ia[pos_ia.X, pos_ia.Y] = 0;
            pos_prec.Push(pos_ia);
            
            carte_ia = GenLabyIA(pos_ia, carte_ia, laby);

            EspaceTexte = 35;
            BordGaucheIA = vp.Width / 2 - (EspaceTexte * laby.Size.X) / 2;
            BordHautIA = vp.Height / 2 - (EspaceTexte * laby.Size.Y) / 2;

            idmax = 0;
            carte_ia_acces = new bool[Config.largeur_labyrinthe, Config.hauteur_labyrinthe];

            carte_ia_inter = new bool[taille.X, taille.Y];
            carte_direct = new int[taille.X, taille.Y];
            nb_chemin = new int[taille.X, taille.Y];
            carte_ia2 = new int[taille.X, taille.Y];
            carte_impasse = new bool[taille.X, taille.Y];

        }

        public static int[,] GenLabyIA(Point pos_ia, int[,] matrice, Labyrinthe laby)
        {
            for (int x = 0; x < laby.Size.X; x++)
            {
                for (int y = 0; y < laby.Size.Y; y++)
                {
                    matrice[x, y] = laby.Size.X * laby.Size.Y;
                }
            }

            int id = 0;
            Point pos;
            int compteur;
            Queue<Point> file = new Queue<Point>();
            matrice[pos_ia.X, pos_ia.Y] = id;
            file.Enqueue(pos_ia);
            while (file.Count > 0)
            {
                id++;
                compteur = file.Count;
                for (int i = 0; i < compteur; i++)
                {
                    pos = file.Dequeue();
                    // Haut
                    if (pos.Y - 1 >= 0 && laby.Carte[pos.X, pos.Y - 1] != 1 && matrice[pos.X, pos.Y - 1] > id)
                    {
                        matrice[pos.X, pos.Y - 1] = id;
                        file.Enqueue(new Point(pos.X, pos.Y - 1));
                    }
                    // Droite
                    if (pos.X + 1 < laby.Size.X && laby.Carte[pos.X + 1, pos.Y] != 1 && matrice[pos.X + 1, pos.Y] > id)
                    {
                        matrice[pos.X + 1, pos.Y] = id;
                        file.Enqueue(new Point(pos.X + 1, pos.Y));
                    }
                    // Bas
                    if (pos.Y + 1 < laby.Size.Y && laby.Carte[pos.X, pos.Y + 1] != 1 && matrice[pos.X, pos.Y + 1] > id)
                    {
                        matrice[pos.X, pos.Y + 1] = id;
                        file.Enqueue(new Point(pos.X, pos.Y + 1));
                    }
                    // Gauche
                    if (pos.X - 1 >= 0 && laby.Carte[pos.X - 1, pos.Y] != 1 && matrice[pos.X - 1, pos.Y] > id)
                    {
                        matrice[pos.X - 1, pos.Y] = id;
                        file.Enqueue(new Point(pos.X - 1, pos.Y));
                    }
                }
            }

            return matrice;
        }

        public void Update(Point position_joueur, bool triche)
        {
            carte_ia = GenLabyIA(position_joueur, carte_ia, laby);

            if (triche)
            {
                KeyboardState clavier = Keyboard.GetState();

                // Déplacement de la carte des fréquence
                if (clavier.IsKeyDown(Keys.Left))
                    BordGaucheIA -= 5;
                if (clavier.IsKeyDown(Keys.Right))
                    BordGaucheIA += 5;
                if (clavier.IsKeyDown(Keys.Up))
                    BordHautIA -= 5;
                if (clavier.IsKeyDown(Keys.Down))
                    BordHautIA += 5;
            }
        }

        public bool ScienNonBloque(Point pos_ia, Point position_scientifique, Joueur joueur)
        {
            for (int x = 0; x < laby.Size.X; x++)
            {
                for (int y = 0; y < laby.Size.Y; y++)
                {
                    carte_ia[x, y] = laby.Size.X * laby.Size.Y;
                }
            }

            int id = 0;
            Point pos;
            int compteur;
            Queue<Point> file = new Queue<Point>();
            carte_ia[pos_ia.X, pos_ia.Y] = id;
            file.Enqueue(pos_ia);
            while (file.Count > 0)
            {
                id++;
                compteur = file.Count;
                for (int i = 0; i < compteur; i++)
                {
                    pos = file.Dequeue();
                    // Haut
                    if (pos.Y - 1 >= 0 && laby.Carte[pos.X, pos.Y - 1] != 1 && !(pos.X == joueur.position_case_actuelle.X && pos.Y - 1 == joueur.position_case_actuelle.Y) && carte_ia[pos.X, pos.Y - 1] > id)
                    {
                        carte_ia[pos.X, pos.Y - 1] = id;
                        file.Enqueue(new Point(pos.X, pos.Y - 1));
                    }
                    // Droite
                    if (pos.X + 1 < laby.Size.X && laby.Carte[pos.X + 1, pos.Y] != 1 && !(pos.X + 1 == joueur.position_case_actuelle.X && pos.Y == joueur.position_case_actuelle.Y) && carte_ia[pos.X + 1, pos.Y] > id)
                    {
                        carte_ia[pos.X + 1, pos.Y] = id;
                        file.Enqueue(new Point(pos.X + 1, pos.Y));
                    }
                    // Bas
                    if (pos.Y + 1 < laby.Size.Y && laby.Carte[pos.X, pos.Y + 1] != 1 && !(pos.X == joueur.position_case_actuelle.X && pos.Y + 1 == joueur.position_case_actuelle.Y) && carte_ia[pos.X, pos.Y + 1] > id)
                    {
                        carte_ia[pos.X, pos.Y + 1] = id;
                        file.Enqueue(new Point(pos.X, pos.Y + 1));
                    }
                    // Gauche
                    if (pos.X - 1 >= 0 && laby.Carte[pos.X - 1, pos.Y] != 1 && !(pos.X - 1 == joueur.position_case_actuelle.X && pos.Y == joueur.position_case_actuelle.Y) && carte_ia[pos.X - 1, pos.Y] > id)
                    {
                        carte_ia[pos.X - 1, pos.Y] = id;
                        file.Enqueue(new Point(pos.X - 1, pos.Y));
                    }
                }
            }
            return (carte_ia[position_scientifique.X, position_scientifique.Y] != laby.Size.X * laby.Size.Y);
        }

        public void CaseAccssibleScientifique(Point pos_ia, Joueur joueur)
        {
            for (int x = 0; x < laby.Size.X; x++)
            {
                for (int y = 0; y < laby.Size.Y; y++)
                {
                    carte_ia_acces[x, y] = false;
                }
            }
            Point pos;
            int compteur;
            Queue<Point> file = new Queue<Point>();
            carte_ia_acces[pos_ia.X, pos_ia.Y] = true;
            file.Enqueue(pos_ia);
            while (file.Count > 0)
            {
                compteur = file.Count;
                for (int i = 0; i < compteur; i++)
                {
                    pos = file.Dequeue();
                    // Haut
                    if (pos.Y - 1 >= 0 && laby.Carte[pos.X, pos.Y - 1] != 1 && !(pos.X == joueur.position_case_actuelle.X && pos.Y - 1 == joueur.position_case_actuelle.Y) && carte_ia_acces[pos.X, pos.Y - 1] == false)
                    {
                        carte_ia_acces[pos.X, pos.Y - 1] = true;
                        file.Enqueue(new Point(pos.X, pos.Y - 1));
                    }
                    // Droite
                    if (pos.X + 1 < laby.Size.X && laby.Carte[pos.X + 1, pos.Y] != 1 && !(pos.X + 1 == joueur.position_case_actuelle.X && pos.Y == joueur.position_case_actuelle.Y) && carte_ia_acces[pos.X + 1, pos.Y] == false)
                    {
                        carte_ia_acces[pos.X + 1, pos.Y] = true;
                        file.Enqueue(new Point(pos.X + 1, pos.Y));
                    }
                    // Bas
                    if (pos.Y + 1 < laby.Size.Y && laby.Carte[pos.X, pos.Y + 1] != 1 && !(pos.X == joueur.position_case_actuelle.X && pos.Y + 1 == joueur.position_case_actuelle.Y) && carte_ia_acces[pos.X, pos.Y + 1] == false)
                    {
                        carte_ia_acces[pos.X, pos.Y + 1] = true;
                        file.Enqueue(new Point(pos.X, pos.Y + 1));
                    }
                    // Gauche
                    if (pos.X - 1 >= 0 && laby.Carte[pos.X - 1, pos.Y] != 1 && !(pos.X - 1 == joueur.position_case_actuelle.X && pos.Y == joueur.position_case_actuelle.Y) && carte_ia_acces[pos.X - 1, pos.Y] == false)
                    {
                        carte_ia_acces[pos.X - 1, pos.Y] = true;
                        file.Enqueue(new Point(pos.X - 1, pos.Y));
                    }
                }
            }
        }


        public void CaseLaPlusEloigne(Point pos_ia)
        {
            for (int x = 0; x < laby.Size.X; x++)
            {
                for (int y = 0; y < laby.Size.Y; y++)
                {
                    carte_ia[x, y] = laby.Size.X * laby.Size.Y;
                }
            }

            int id = 0;
            Point pos;
            int compteur;
            Queue<Point> file = new Queue<Point>();
            carte_ia[pos_ia.X, pos_ia.Y] = id;
            file.Enqueue(pos_ia);
            while (file.Count > 0)
            {
                id++;
                compteur = file.Count;
                for (int i = 0; i < compteur; i++)
                {
                    pos = file.Dequeue();
                    // Haut; si position regardé differente d'un mur, et accessible par le scientifique.
                    if (pos.Y - 1 >= 0 && laby.Carte[pos.X, pos.Y - 1] != 1 && carte_ia_acces[pos.X, pos.Y - 1] == true && carte_ia[pos.X, pos.Y - 1] > id)
                    {
                        carte_ia[pos.X, pos.Y - 1] = id;
                        if (idmax <= id)
                        {
                            idmax = id;
                            case_eloigne.X = pos.X;
                            case_eloigne.Y = pos.Y - 1;
                        }
                        file.Enqueue(new Point(pos.X, pos.Y - 1));
                    }
                    // Droite
                    if (pos.X + 1 < laby.Size.X && laby.Carte[pos.X + 1, pos.Y] != 1 && carte_ia_acces[pos.X + 1, pos.Y] == true && carte_ia[pos.X + 1, pos.Y] > id)
                    {
                        carte_ia[pos.X + 1, pos.Y] = id;
                        if (idmax <= id)
                        {
                            idmax = id;
                            case_eloigne.X = pos.X + 1;
                            case_eloigne.Y = pos.Y;
                        }
                        file.Enqueue(new Point(pos.X + 1, pos.Y));
                    }
                    // Bas
                    if (pos.Y + 1 < laby.Size.Y && laby.Carte[pos.X, pos.Y + 1] != 1 && carte_ia_acces[pos.X, pos.Y + 1] == true && carte_ia[pos.X, pos.Y + 1] > id)
                    {
                        carte_ia[pos.X, pos.Y + 1] = id;
                        if (idmax <= id)
                        {
                            idmax = id;
                            case_eloigne.X = pos.X;
                            case_eloigne.Y = pos.Y + 1;
                        }
                        file.Enqueue(new Point(pos.X, pos.Y + 1));
                    }
                    // Gauche
                    if (pos.X - 1 >= 0 && laby.Carte[pos.X - 1, pos.Y] != 1 && carte_ia_acces[pos.X - 1, pos.Y] == true && carte_ia[pos.X - 1, pos.Y] > id)
                    {
                        carte_ia[pos.X - 1, pos.Y] = id;
                        if (idmax <= id)
                        {
                            idmax = id;
                            case_eloigne.X = pos.X - 1;
                            case_eloigne.Y = pos.Y;
                        }
                        file.Enqueue(new Point(pos.X - 1, pos.Y));
                    }
                }
            }
        }

        public void CaseIntersection()
        {

            for (int i = 0; i < laby.Size.X; i++)
            {
                for (int j = 0; j < laby.Size.Y; j++)
                {
                    nb_chemin[i, j] = 0;
                    if (laby.Carte[i, j] != 1)
                    {
                        if (j > 0 && laby.Carte[i, j - 1] != 1)
                            nb_chemin[i, j]++;

                        if (i > 0 && laby.Carte[i - 1, j] != 1)
                            nb_chemin[i, j]++;

                        if (j < laby.Size.Y - 1 && laby.Carte[i, j + 1] != 1)
                            nb_chemin[i, j]++;

                        if (i < laby.Size.X - 1 && laby.Carte[i + 1, j] != 1)
                            nb_chemin[i, j]++;
                    }
                    if (nb_chemin[i, j] > 2)
                        carte_ia_inter[i, j] = true;
                    else
                        carte_ia_inter[i, j] = false;
                }
            }
        }

        public void CaseImpasse()
        {
            for (int i = 0; i < taille.X; i++)
            {
                for (int j = 0; j < taille.Y; j++)
                {
                    carte_impasse[i, j] = nb_chemin[i, j] == 1;
                }
            }
        }


        public void Direction(Chasseur chasseur, int[,] carte_ia)
        {
            Point pos_ia = new Point(chasseur.position_case_actuelle.X, chasseur.position_case_actuelle.Y);
            for (int i = 0; i < Config.largeur_labyrinthe; i++)
            {
                for (int j = 0; j < Config.hauteur_labyrinthe; j++)
                {
                    carte_direct[i, j] = laby.Carte[i, j];
                }
            }

            int[,] carte_ia_direction = carte_direct;
            Random rand = new Random();
            int choix = 0;
            Point choix2 = new Point(-1, -1);
            Point choix3 = new Point(-1, -1);
            Point choix4 = new Point(-1, -1);

            if (pos_ia.Y > 0 && laby.Carte[pos_ia.X, pos_ia.Y - 1] != 1 && (new Point(pos_ia.X, pos_ia.Y - 1) != chasseur.position_case_precedente || carte_ia[pos_ia.X, pos_ia.Y - 1] < carte_ia[pos_ia.X, pos_ia.Y]) && (!carte_impasse[pos_ia.X, pos_ia.Y - 1] || carte_ia[pos_ia.X, pos_ia.Y - 1] < carte_ia[pos_ia.X, pos_ia.Y]))
                choix2 = new Point(pos_ia.X, pos_ia.Y - 1);
            if (pos_ia.X > 0 && laby.Carte[pos_ia.X - 1, pos_ia.Y] != 1 && (new Point(pos_ia.X - 1, pos_ia.Y) != chasseur.position_case_precedente || carte_ia[pos_ia.X - 1, pos_ia.Y] < carte_ia[pos_ia.X, pos_ia.Y]) && (!carte_impasse[pos_ia.X - 1, pos_ia.Y] || carte_ia[pos_ia.X - 1, pos_ia.Y] < carte_ia[pos_ia.X, pos_ia.Y]))
                if (choix2 == new Point(-1, -1))
                    choix2 = new Point(pos_ia.X - 1, pos_ia.Y);
                else
                    choix3 = new Point(pos_ia.X - 1, pos_ia.Y);

            if (pos_ia.Y < laby.Size.Y - 1 && laby.Carte[pos_ia.X, pos_ia.Y + 1] != 1 && (new Point(pos_ia.X, pos_ia.Y + 1) != chasseur.position_case_precedente || carte_ia[pos_ia.X, pos_ia.Y + 1] < carte_ia[pos_ia.X, pos_ia.Y]) && (!carte_impasse[pos_ia.X, pos_ia.Y + 1] || carte_ia[pos_ia.X, pos_ia.Y + 1] < carte_ia[pos_ia.X, pos_ia.Y]))
                if (choix2 == new Point(-1, -1))
                    choix2 = new Point(pos_ia.X, pos_ia.Y + 1);
                else if (choix3 == new Point(-1, -1))
                    choix3 = new Point(pos_ia.X, pos_ia.Y + 1);
                else
                    choix4 = new Point(pos_ia.X, pos_ia.Y + 1);

            if (pos_ia.X < laby.Size.X - 1 && laby.Carte[pos_ia.X + 1, pos_ia.Y] != 1 && (new Point(pos_ia.X + 1, pos_ia.Y) != chasseur.position_case_precedente || carte_ia[pos_ia.X + 1, pos_ia.Y] < carte_ia[pos_ia.X, pos_ia.Y]) && (!carte_impasse[pos_ia.X + 1, pos_ia.Y] || carte_ia[pos_ia.X + 1, pos_ia.Y] < carte_ia[pos_ia.X, pos_ia.Y]))
                if (choix2 == new Point(-1, -1))
                    choix2 = new Point(pos_ia.X + 1, pos_ia.Y);
                else if (choix3 == new Point(-1, -1))
                    choix3 = new Point(pos_ia.X + 1, pos_ia.Y);
                else if (choix4 == new Point(-1, -1))
                    choix4 = new Point(pos_ia.X + 1, pos_ia.Y);


            if (choix2 == new Point(-1, -1))
                choix2 = new Point(pos_ia.X, pos_ia.Y);

            if (choix3 == new Point(-1, -1))
                choix3 = choix2;

            if (choix4 == new Point(-1, -1))
                choix4 = choix3;



            if (carte_ia[pos_ia.X, pos_ia.Y] > carte_ia[choix2.X, choix2.Y])
            {
                if (Config.index_difficulte == 2)
                    choix = 2;
                else if (Config.index_difficulte == 1)
                {
                    int rand0m = rand.Next(0, 100);
                    if (rand0m > 33)
                        choix = 2;
                    else if (rand0m >= 16)
                        choix = 3;
                    else
                        choix = 4;
                }
                else
                    if (nb_chemin[pos_ia.X, pos_ia.Y] < 2) //Le chasseur se trouve dans une impasse
                        choix = 2;
                    else
                        choix = rand.Next(2, nb_chemin[pos_ia.X, pos_ia.Y] + 1);

            }
            else if (carte_ia[pos_ia.X, pos_ia.Y] > carte_ia[choix3.X, choix3.Y])
            {
                if (Config.index_difficulte == 2)
                    choix = 3;
                else if (Config.index_difficulte == 1)
                {
                    int rand0m = rand.Next(0, 100);
                    if (rand0m > 33)
                        choix = 3;
                    else if (rand0m >= 16)
                        choix = 2;
                    else
                        choix = 4;
                }
                else
                    if (nb_chemin[pos_ia.X, pos_ia.Y] < 2) //Le chasseur se trouve dans une impasse
                        choix = 3;
                    else
                        choix = rand.Next(2, nb_chemin[pos_ia.X, pos_ia.Y] + 1);
            }
            else if (carte_ia[pos_ia.X, pos_ia.Y] > carte_ia[choix4.X, choix4.Y])
            {
                if (Config.index_difficulte == 2)
                    choix = 4;
                else if (Config.index_difficulte == 1)
                {
                    int rand0m = rand.Next(0, 100);
                    if (rand0m > 33)
                        choix = 4;
                    else if (rand0m >= 16)
                        choix = 2;
                    else
                        choix = 3;
                }
                else
                    if (nb_chemin[pos_ia.X, pos_ia.Y] < 2) //Le chasseur se trouve dans une impasse
                        choix = 4;
                    else
                        choix = rand.Next(2, nb_chemin[pos_ia.X, pos_ia.Y] + 1);
            }
            else
            {
                if (nb_chemin[pos_ia.X, pos_ia.Y] < 2) //Le chasseur se trouve dans une impasse
                    choix = 2;
                else
                    choix = rand.Next(2, nb_chemin[pos_ia.X, pos_ia.Y] + 1);
            }



            if (choix == 2)
            {
                if (new Point(pos_ia.X, pos_ia.Y - 1) != choix2)
                {
                    if (pos_ia.Y > 0)
                        carte_ia_direction[pos_ia.X, pos_ia.Y - 1] = 1;
                }
                if (new Point(pos_ia.X - 1, pos_ia.Y) != choix2)
                {
                    if (pos_ia.X > 0)
                        carte_ia_direction[pos_ia.X - 1, pos_ia.Y] = 1;
                }
                if (new Point(pos_ia.X, pos_ia.Y + 1) != choix2)
                {
                    if (pos_ia.Y < laby.Size.Y - 1)
                        carte_ia_direction[pos_ia.X, pos_ia.Y + 1] = 1;
                }
                if (new Point(pos_ia.X + 1, pos_ia.Y) != choix2)
                {
                    if (pos_ia.X < laby.Size.X - 1)
                        carte_ia_direction[pos_ia.X + 1, pos_ia.Y] = 1;
                }
            }
            else if (choix == 3)
            {
                if (new Point(pos_ia.X, pos_ia.Y - 1) != choix3)
                {
                    if (pos_ia.Y > 0)
                        carte_ia_direction[pos_ia.X, pos_ia.Y - 1] = 1;
                }
                if (new Point(pos_ia.X - 1, pos_ia.Y) != choix3)
                {
                    if (pos_ia.X > 0)
                        carte_ia_direction[pos_ia.X - 1, pos_ia.Y] = 1;
                }
                if (new Point(pos_ia.X, pos_ia.Y + 1) != choix3)
                {
                    if (pos_ia.Y < laby.Size.Y - 1)
                        carte_ia_direction[pos_ia.X, pos_ia.Y + 1] = 1;
                }
                if (new Point(pos_ia.X + 1, pos_ia.Y) != choix3)
                {
                    if (pos_ia.X < laby.Size.X - 1)
                        carte_ia_direction[pos_ia.X + 1, pos_ia.Y] = 1;
                }
            }
            else if (choix == 4)
            {
                if (new Point(pos_ia.X, pos_ia.Y - 1) != choix4)
                {
                    if (pos_ia.Y > 0)
                        carte_ia_direction[pos_ia.X, pos_ia.Y - 1] = 1;
                }
                if (new Point(pos_ia.X - 1, pos_ia.Y) != choix4)
                {
                    if (pos_ia.X > 0)
                        carte_ia_direction[pos_ia.X - 1, pos_ia.Y] = 1;
                }
                if (new Point(pos_ia.X, pos_ia.Y + 1) != choix4)
                {
                    if (pos_ia.Y < laby.Size.Y - 1)
                        carte_ia_direction[pos_ia.X, pos_ia.Y + 1] = 1;
                }
                if (new Point(pos_ia.X + 1, pos_ia.Y) != choix4)
                {
                    if (pos_ia.X < laby.Size.X - 1)
                        carte_ia_direction[pos_ia.X + 1, pos_ia.Y] = 1;
                }
            }
            carte_direct = carte_ia_direction;
        }

        public void DirectionScient(Scientifique scientifique, int[,] carte_ia)
        {
            Point pos_ia = new Point(scientifique.position_case_actuelle.X, scientifique.position_case_actuelle.Y);
            for (int i = 0; i < Config.largeur_labyrinthe; i++)
            {
                for (int j = 0; j < Config.hauteur_labyrinthe; j++)
                {
                    carte_direct[i, j] = laby.Carte[i, j];
                }
            }

            int[,] carte_ia_direction = carte_direct;
            Random rand = new Random();
            int choix = 0;
            Point choix2 = new Point(-1, -1);
            Point choix3 = new Point(-1, -1);
            Point choix4 = new Point(-1, -1);

            if (pos_ia.Y > 0 && laby.Carte[pos_ia.X, pos_ia.Y - 1] != 1 && (new Point(pos_ia.X, pos_ia.Y - 1) != scientifique.position_case_precedente || carte_ia[pos_ia.X, pos_ia.Y - 1] < carte_ia[pos_ia.X, pos_ia.Y]) && !carte_impasse[pos_ia.X, pos_ia.Y - 1])
                choix2 = new Point(pos_ia.X, pos_ia.Y - 1);
            if (pos_ia.X > 0 && laby.Carte[pos_ia.X - 1, pos_ia.Y] != 1 && (new Point(pos_ia.X - 1, pos_ia.Y) != scientifique.position_case_precedente || carte_ia[pos_ia.X - 1, pos_ia.Y] < carte_ia[pos_ia.X, pos_ia.Y]) && !carte_impasse[pos_ia.X - 1, pos_ia.Y])
                if (choix2 == new Point(-1, -1))
                    choix2 = new Point(pos_ia.X - 1, pos_ia.Y);
                else
                    choix3 = new Point(pos_ia.X - 1, pos_ia.Y);

            if (pos_ia.Y < laby.Size.Y - 1 && laby.Carte[pos_ia.X, pos_ia.Y + 1] != 1 && (new Point(pos_ia.X, pos_ia.Y + 1) != scientifique.position_case_precedente || carte_ia[pos_ia.X, pos_ia.Y + 1] < carte_ia[pos_ia.X, pos_ia.Y]) && !carte_impasse[pos_ia.X, pos_ia.Y + 1])
                if (choix2 == new Point(-1, -1))
                    choix2 = new Point(pos_ia.X, pos_ia.Y + 1);
                else if (choix3 == new Point(-1, -1))
                    choix3 = new Point(pos_ia.X, pos_ia.Y + 1);
                else
                    choix4 = new Point(pos_ia.X, pos_ia.Y + 1);

            if (pos_ia.X < laby.Size.X - 1 && laby.Carte[pos_ia.X + 1, pos_ia.Y] != 1 && (new Point(pos_ia.X + 1, pos_ia.Y) != scientifique.position_case_precedente || carte_ia[pos_ia.X + 1, pos_ia.Y] < carte_ia[pos_ia.X, pos_ia.Y]) && !carte_impasse[pos_ia.X + 1, pos_ia.Y])
                if (choix2 == new Point(-1, -1))
                    choix2 = new Point(pos_ia.X + 1, pos_ia.Y);
                else if (choix3 == new Point(-1, -1))
                    choix3 = new Point(pos_ia.X + 1, pos_ia.Y);
                else if (choix4 == new Point(-1, -1))
                    choix4 = new Point(pos_ia.X + 1, pos_ia.Y);


            if (choix2 == new Point(-1, -1))
                choix2 = new Point(pos_ia.X, pos_ia.Y);

            if (choix3 == new Point(-1, -1))
                choix3 = choix2;

            if (choix4 == new Point(-1, -1))
                choix4 = choix3;



            if (carte_ia[pos_ia.X, pos_ia.Y] > carte_ia[choix2.X, choix2.Y])
            {
                if (Config.index_difficulte == 2)
                    choix = 2;
                else if (Config.index_difficulte == 1)
                {
                    int rand0m = rand.Next(0, 100);
                    if (rand0m > 33)
                        choix = 2;
                    else if (rand0m >= 16)
                        choix = 3;
                    else
                        choix = 4;
                }
                else
                    if (nb_chemin[pos_ia.X, pos_ia.Y] < 2) //Le scientifique se trouve dans une impasse
                        choix = 2;
                    else
                        choix = rand.Next(2, nb_chemin[pos_ia.X, pos_ia.Y] + 1);

            }
            else if (carte_ia[pos_ia.X, pos_ia.Y] > carte_ia[choix3.X, choix3.Y])
            {
                if (Config.index_difficulte == 2)
                    choix = 3;
                else if (Config.index_difficulte == 1)
                {
                    int rand0m = rand.Next(0, 100);
                    if (rand0m > 33)
                        choix = 3;
                    else if (rand0m >= 16)
                        choix = 2;
                    else
                        choix = 4;
                }
                else
                    if (nb_chemin[pos_ia.X, pos_ia.Y] < 2) //Le scientifique se trouve dans une impasse
                        choix = 3;
                    else
                        choix = rand.Next(2, nb_chemin[pos_ia.X, pos_ia.Y] + 1);
            }
            else if (carte_ia[pos_ia.X, pos_ia.Y] > carte_ia[choix4.X, choix4.Y])
            {
                if (Config.index_difficulte == 2)
                    choix = 4;
                else if (Config.index_difficulte == 1)
                {
                    int rand0m = rand.Next(0, 100);
                    if (rand0m > 33)
                        choix = 4;
                    else if (rand0m >= 16)
                        choix = 2;
                    else
                        choix = 3;
                }
                else
                    if (nb_chemin[pos_ia.X, pos_ia.Y] < 2) //Le scientifique se trouve dans une impasse
                        choix = 4;
                    else
                        choix = rand.Next(2, nb_chemin[pos_ia.X, pos_ia.Y] + 1);
            }
            else
            {
                if (nb_chemin[pos_ia.X, pos_ia.Y] < 2) //Le scientifique se trouve dans une impasse
                    choix = 2;
                else
                    choix = rand.Next(2, nb_chemin[pos_ia.X, pos_ia.Y] + 1);
            }



            if (choix == 2)
            {
                if (new Point(pos_ia.X, pos_ia.Y - 1) != choix2)
                {
                    if (pos_ia.Y > 0)
                        carte_ia_direction[pos_ia.X, pos_ia.Y - 1] = 1;
                }
                if (new Point(pos_ia.X - 1, pos_ia.Y) != choix2)
                {
                    if (pos_ia.X > 0)
                        carte_ia_direction[pos_ia.X - 1, pos_ia.Y] = 1;
                }
                if (new Point(pos_ia.X, pos_ia.Y + 1) != choix2)
                {
                    if (pos_ia.Y < laby.Size.Y - 1)
                        carte_ia_direction[pos_ia.X, pos_ia.Y + 1] = 1;
                }
                if (new Point(pos_ia.X + 1, pos_ia.Y) != choix2)
                {
                    if (pos_ia.X < laby.Size.X - 1)
                        carte_ia_direction[pos_ia.X + 1, pos_ia.Y] = 1;
                }
            }
            else if (choix == 3)
            {
                if (new Point(pos_ia.X, pos_ia.Y - 1) != choix3)
                {
                    if (pos_ia.Y > 0)
                        carte_ia_direction[pos_ia.X, pos_ia.Y - 1] = 1;
                }
                if (new Point(pos_ia.X - 1, pos_ia.Y) != choix3)
                {
                    if (pos_ia.X > 0)
                        carte_ia_direction[pos_ia.X - 1, pos_ia.Y] = 1;
                }
                if (new Point(pos_ia.X, pos_ia.Y + 1) != choix3)
                {
                    if (pos_ia.Y < laby.Size.Y - 1)
                        carte_ia_direction[pos_ia.X, pos_ia.Y + 1] = 1;
                }
                if (new Point(pos_ia.X + 1, pos_ia.Y) != choix3)
                {
                    if (pos_ia.X < laby.Size.X - 1)
                        carte_ia_direction[pos_ia.X + 1, pos_ia.Y] = 1;
                }
            }
            else if (choix == 4)
            {
                if (new Point(pos_ia.X, pos_ia.Y - 1) != choix4)
                {
                    if (pos_ia.Y > 0)
                        carte_ia_direction[pos_ia.X, pos_ia.Y - 1] = 1;
                }
                if (new Point(pos_ia.X - 1, pos_ia.Y) != choix4)
                {
                    if (pos_ia.X > 0)
                        carte_ia_direction[pos_ia.X - 1, pos_ia.Y] = 1;
                }
                if (new Point(pos_ia.X, pos_ia.Y + 1) != choix4)
                {
                    if (pos_ia.Y < laby.Size.Y - 1)
                        carte_ia_direction[pos_ia.X, pos_ia.Y + 1] = 1;
                }
                if (new Point(pos_ia.X + 1, pos_ia.Y) != choix4)
                {
                    if (pos_ia.X < laby.Size.X - 1)
                        carte_ia_direction[pos_ia.X + 1, pos_ia.Y] = 1;
                }
            }
            carte_direct = carte_ia_direction;
        }


        public bool Impasse(Point pos_ia)
        {
            return (nb_chemin[pos_ia.X, pos_ia.Y] == 1);
        }

        public void CaseObjectif(Point pos_ia)
        {
            for (int x = 0; x < laby.Size.X; x++)
            {
                for (int y = 0; y < laby.Size.Y; y++)
                {
                    carte_ia[x, y] = laby.Size.X * laby.Size.Y;
                }
            }

            int id = 0;
            Point pos;
            int compteur;
            Queue<Point> file = new Queue<Point>();
            carte_ia[pos_ia.X, pos_ia.Y] = id;
            file.Enqueue(pos_ia);
            while (file.Count > 0)
            {
                id++;
                compteur = file.Count;
                for (int i = 0; i < compteur; i++)
                {
                    pos = file.Dequeue();
                    // Haut; si position regardé differente d'un mur, et accessible par le chasseur.
                    if (pos.Y - 1 >= 0 && carte_direct[pos.X, pos.Y - 1] != 1 && carte_ia[pos.X, pos.Y - 1] > id)
                    {
                        carte_ia[pos.X, pos.Y - 1] = id;
                        if (idmax <= id)
                        {
                            idmax = id;
                            case_eloigne.X = pos.X;
                            case_eloigne.Y = pos.Y - 1;
                        }
                        file.Enqueue(new Point(pos.X, pos.Y - 1));
                    }
                    // Droite
                    if (pos.X + 1 < laby.Size.X  && carte_direct[pos.X + 1, pos.Y] != 1 && carte_ia[pos.X + 1, pos.Y] > id)
                    {
                        carte_ia[pos.X + 1, pos.Y] = id;
                        if (idmax <= id)
                        {
                            idmax = id;
                            case_eloigne.X = pos.X + 1;
                            case_eloigne.Y = pos.Y;
                        }
                        file.Enqueue(new Point(pos.X + 1, pos.Y));
                    }
                    // Bas
                    if (pos.Y + 1 < laby.Size.Y  && carte_direct[pos.X, pos.Y + 1] != 1 && carte_ia[pos.X, pos.Y + 1] > id)
                    {
                        carte_ia[pos.X, pos.Y + 1] = id;
                        if (idmax <= id)
                        {
                            idmax = id;
                            case_eloigne.X = pos.X;
                            case_eloigne.Y = pos.Y + 1;
                        }
                        file.Enqueue(new Point(pos.X, pos.Y + 1));
                    }
                    // Gauche
                    if (pos.X - 1 >= 0 && carte_direct[pos.X - 1, pos.Y] != 1 && carte_ia[pos.X - 1, pos.Y] > id)
                    {
                        carte_ia[pos.X - 1, pos.Y] = id;
                        if (idmax <= id)
                        {
                            idmax = id;
                            case_eloigne.X = pos.X - 1;
                            case_eloigne.Y = pos.Y;
                        }
                        file.Enqueue(new Point(pos.X - 1, pos.Y));
                    }
                }
            }
        }




        public void UpdateBis(Scientifique scientifique, Joueur joueur)
        {
            idmax = 0;
            CaseAccssibleScientifique(scientifique.position_case_actuelle, joueur);
            CaseLaPlusEloigne(joueur.position_case_actuelle);
            carte_ia = GenLabyIA(case_eloigne, carte_ia, laby);
        }

        public void UpdateScientifique2(Scientifique scientifique, Point laby_sortie)
        {
            if (!Impasse(scientifique.position_case_actuelle) && !carte_ia_inter[scientifique.position_case_actuelle.X, scientifique.position_case_actuelle.Y])
            {
                if (case_eloigne == Point.Zero && !une_premiere_fois)
                    carte_ia = GenLabyIA(laby_sortie, carte_ia, laby);
                else
                    carte_ia = GenLabyIA(case_eloigne, carte_ia, laby);

            }
            else
            {
                une_premiere_fois = true;
                idmax = 0;
                carte_ia = GenLabyIA(laby_sortie, carte_ia, laby);
                DirectionScient(scientifique, carte_ia);
                CaseObjectif(scientifique.position_case_actuelle);
                carte_ia = GenLabyIA(case_eloigne, carte_ia, laby);
            }
        }


        public void UpdateChasseur(Chasseur chasseur, bool triche, Joueur joueur)
        {
            if (!carte_ia_inter[chasseur.position_case_actuelle.X, chasseur.position_case_actuelle.Y] && !Impasse(chasseur.position_case_actuelle))
            {
                if (case_eloigne == Point.Zero && !une_premiere_fois) 
                    carte_ia = GenLabyIA(joueur.position_case_actuelle, carte_ia, laby);
                else
                    carte_ia = GenLabyIA(case_eloigne, carte_ia, laby);
               
            }
            else
            {
                une_premiere_fois = true;
                idmax = 0;
                carte_ia = GenLabyIA(joueur.position_case_actuelle, carte_ia, laby);
                Direction(chasseur, carte_ia);
                CaseObjectif(chasseur.position_case_actuelle);
                carte_ia = GenLabyIA(case_eloigne, carte_ia, laby);
            }
            if (triche)
            {
                KeyboardState clavier = Keyboard.GetState();

                // Déplacement de la carte des fréquence
                if (clavier.IsKeyDown(Keys.Left))
                    BordGaucheIA -= 5;
                if (clavier.IsKeyDown(Keys.Right))
                    BordGaucheIA += 5;
                if (clavier.IsKeyDown(Keys.Up))
                    BordHautIA -= 5;
                if (clavier.IsKeyDown(Keys.Down))
                    BordHautIA += 5;
            }
        }

        public void Draw(KeyboardState clavier, SpriteBatch spriteBatch, SpriteFont Kootenay, int BordGauche, int BordHaut, bool triche)
        {
            // Affichage de la carte IA
            if (triche && clavier.IsKeyDown(Keys.I))
            {
                spriteBatch.DrawString(Kootenay, "Carte IA", new Vector2(BordGaucheIA + (laby.Size.X * EspaceTexte) / 2 - EspaceTexte - 10, BordHautIA - EspaceTexte), Color.Red);

                for (int x = 0; x < laby.Size.X; x++)
                {
                    for (int y = 0; y < laby.Size.Y; y++)
                    {/*
                        if (carte_ia[x, y] < laby.Size.X * laby.Size.Y)
                            spriteBatch.DrawString(Kootenay, carte_ia[x, y].ToString(), new Vector2(BordGaucheIA + EspaceTexte * x, BordHautIA + EspaceTexte * y), Color.White);
                        else
                            spriteBatch.DrawString(Kootenay, carte_ia[x, y].ToString(), new Vector2(BordGaucheIA + EspaceTexte * x, BordHautIA + EspaceTexte * y), Color.Red);
                    */
                        spriteBatch.DrawString(Kootenay, carte_ia_inter[x, y].ToString(), new Vector2(BordGaucheIA + EspaceTexte * x, BordHautIA + EspaceTexte * y), Color.Red);

                    }
                }

            }
            if (triche && clavier.IsKeyDown(Keys.J))
            {
                spriteBatch.DrawString(Kootenay, "Carte IA", new Vector2(BordGaucheIA + (laby.Size.X * EspaceTexte) / 2 - EspaceTexte - 10, BordHautIA - EspaceTexte), Color.Red);

                for (int x = 0; x < laby.Size.X; x++)
                {
                    for (int y = 0; y < laby.Size.Y; y++)
                    {
                        if (carte_ia[x, y] < laby.Size.X * laby.Size.Y)
                            spriteBatch.DrawString(Kootenay, carte_ia[x, y].ToString(), new Vector2(BordGaucheIA + EspaceTexte * x, BordHautIA + EspaceTexte * y), Color.White);
                        else
                            spriteBatch.DrawString(Kootenay, carte_ia[x, y].ToString(), new Vector2(BordGaucheIA + EspaceTexte * x, BordHautIA + EspaceTexte * y), Color.Red);
                    }
                }
            }
        }
    }
}