using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SkeletonBaseLibrary;

namespace aMAZEing_Escape
{
    class Scientifique
    {
        // Initalisation du scientifique
        public Vector3 position;
        public Point position_case_actuelle;
        public Point position_case_precedente;
        public bool changement_de_case;
        private Point case_transition;
        private bool immobile;
        public bool estpasmort = true;
        // Vitesse
        public float vitesse;
        private float vitesse_initiale;
        public float temporisation;
        Labyrinthe laby;
        //int pos_prec;
        public int[,] frequence;
        private int BordGaucheFrequence;
        private int BordHautFrequence;
        private int EspaceTexte;
        public int direction;

        public float distance_deplacement;
        private int min_id;
        private int min_id_prec;
        private Vector3 pos_temp;
        public float vitesse_rotation;

        // Animation
        public string animation = "walk_normal";

        private IA ia;

        public Vector2 increment;

        #region Pour le model
        // Variables spécifiques au model
        public Vector3 rotation;
        private Model model;
        public CSkeletonBase skeleton_base;
        private float scale;
        private Matrix localWorld;

        // Méthode relatives au model
        public Matrix LocalWorld
        {
            get
            {
                return Matrix.CreateRotationY(rotation.Y)
                        * Matrix.CreateRotationX(rotation.X)
                        * Matrix.CreateRotationZ(rotation.Z)
                        * Matrix.CreateTranslation(position)
                        * Matrix.CreateScale(scale); ;
            }
            set { localWorld = value; }
        }

        public float Scale
        {
            get { return scale; }
            set { scale = value; }
        }

        public Model Model
        {
            get { return model; }
            set { model = value; }
        }

        public Vector3 Rotation
        {
            get { return rotation; }
            set { rotation = value; }
        }

        public void RotationX(float rotX) { rotation.X = rotX; }
        public void RotationY(float rotY) { rotation.Y = rotY; }
        public void RotationZ(float rotZ) { rotation.Z = rotZ; }

        public Vector3 Position
        {
            get { return position; }
            set { position = value; }
        }

        #endregion

        // Le constructeur
        public Scientifique(Viewport vp, Labyrinthe labyrinthe, ContentManager Content, IA ia)
        {
            /** Pour le model **/
            model = Content.Load<Model>("Models/Scientifique/hazmat");
            skeleton_base = new CSkeletonBase(model);
            skeleton_base.setAnimation("walk_normal");
            skeleton_base.TimeScale = 2;
            scale = 0.02f;
            RotationX(0);
            RotationY(0);
            RotationZ(0);
            localWorld = Matrix.Identity;

            /** Infos sur le scientifique **/
            increment = new Vector2(0);
            laby = labyrinthe;
            position = laby.scientifique_position_initiale / scale;
            position_case_actuelle = new Point((int)(position.X / (laby.CellSize.X / scale)), (int)(position.Z / (laby.CellSize.Z / scale)));
            position_case_precedente = position_case_actuelle;
            case_transition = position_case_actuelle;
            changement_de_case = false;
            immobile = true;

            pos_temp = position;

            temporisation = 0;
            //pos_prec = -1;
            frequence = new int[laby.Size.X, laby.Size.Y];
            for (int x = 0; x < laby.Size.X; x++)
            {
                for (int y = 0; y < laby.Size.Y; y++)
                {
                    frequence[x, y] = 0;
                }
            }

            EspaceTexte = 22;
            BordGaucheFrequence = vp.Width / 2 - (EspaceTexte * laby.Size.X) / 2;
            BordHautFrequence = vp.Height / 2 - (EspaceTexte * laby.Size.Y) / 2;

            // Pour la rotation du chasseur
            direction = 2; // Au début, il regarde en bas

            vitesse_initiale = 0.01f;
            vitesse = vitesse_initiale;
            distance_deplacement = 0.42f;
            vitesse_rotation = 0.05f;

            // IA
            this.ia = ia;
            min_id = MinID(ia.carte_ia);
            min_id_prec = min_id;

        }

        public void Update(float temps, bool triche)
        {
            KeyboardState clavier = Keyboard.GetState();

            // Actualisation de la position du scientifique
            position_case_actuelle = new Point((int)(position.X / (laby.CellSize.X / scale)), (int)(position.Z / (laby.CellSize.Z / scale)));

            if (case_transition != position_case_actuelle)
            {
                if (immobile)
                    immobile = false;
                changement_de_case = true;
                position_case_precedente = case_transition;
            }
            else
                changement_de_case = false;


            //Faire tourner le modèle avec les touches directionelles
            if(clavier.IsKeyDown(Keys.Left))
                increment.X = (increment.X + 0.05f) % MathHelper.ToRadians(360);
            if(clavier.IsKeyDown(Keys.Right))
                increment.X = (increment.X - 0.05f) % MathHelper.ToRadians(360);

            if (clavier.IsKeyDown(Keys.Up))
                increment.Y += 0.05f % MathHelper.ToRadians(360);
            if (clavier.IsKeyDown(Keys.Down))
                increment.Y -= 0.05f % MathHelper.ToRadians(360);

            temporisation -= temps;

            RotationY(increment.X);
            RotationZ(increment.Y);
            
            // Déplacement du chasseur => IA
            if (temporisation < 0)
            {

                if (min_id == 0 && ((position.Z * Scale) / laby.CellSize.Z) <= ((pos_temp.Z * Scale) / laby.CellSize.Z) - 1 ||
                    min_id == 1 && ((position.X * Scale) / laby.CellSize.X) >= ((pos_temp.X * Scale) / laby.CellSize.X) + 1 ||
                    min_id == 2 && ((position.Z * Scale) / laby.CellSize.Z) >= ((pos_temp.Z * Scale) / laby.CellSize.Z) + 1 ||
                     min_id == 3 && ((position.X * Scale) / laby.CellSize.X) <= ((pos_temp.X * Scale) / laby.CellSize.X) - 1)
                {
                    pos_temp = position;
                    min_id = MinID(ia.carte_ia);
                }

                #region Les rotation
                // Les rotations
                
                if (min_id != direction)
                {
                    // Le chasseur doit aller en haut
                    if (min_id == 0)
                    {
                        // Le chasseur regarde à droite
                        if (direction == 1)
                        {
                            // On le fait tourner de 90°
                            if (increment.X < MathHelper.ToRadians(180))
                                increment.X = (increment.X + vitesse_rotation) % MathHelper.ToRadians(360);
                            else
                                direction = 0;
                        }
                        // Le chasseur regarde en bas
                        else if (direction == 2)
                        {
                            // On le fait tourner de 180°
                            if (increment.X < MathHelper.ToRadians(180))
                                increment.X = (increment.X + vitesse_rotation) % MathHelper.ToRadians(360);
                            else
                                direction = 0;
                        }
                        // Le chasseur regarde à gauche
                        else if (direction == 3)
                        {
                            // On le fait tourner de - 90°
                            if (increment.X > MathHelper.ToRadians(-180))
                                increment.X = (increment.X - vitesse_rotation) % MathHelper.ToRadians(360);
                            else
                            {
                                direction = 0;
                                increment.X = MathHelper.ToRadians(180);
                            }
                        }
                    }
                    
                    // Le chasseur doit aller à droite
                    else if (min_id == 1)
                    {
                        // Le chasseur regarde en haut
                        if (direction == 0)
                        {
                            // On le fait tourner de -90°
                            if (increment.X > MathHelper.ToRadians(90))
                                increment.X = (increment.X - vitesse_rotation) % MathHelper.ToRadians(360);
                            else
                                direction = 1;
                        }
                        // Le chasseur regarde en bas
                        else if (direction == 2)
                        {
                            // On le fait tourner de 90°
                            if (increment.X < MathHelper.ToRadians(90))
                                increment.X = (increment.X + vitesse_rotation) % MathHelper.ToRadians(360);
                            else
                                direction = 1;
                        }
                        // Le chasseur regarde à gauche
                        else if (direction == 3)
                        {
                            // On le fait tourner de 180°
                            if (increment.X < MathHelper.ToRadians(90))
                                increment.X = (increment.X + vitesse_rotation) % MathHelper.ToRadians(360);
                            else
                                direction = 1;
                        }
                    }
                    
                    
                    // Le chasseur doit aller en bas
                    else if (min_id == 2)
                    {
                        // Le chasseur regarde en haut
                        if (direction == 0)
                        {
                            // On le fait tourner de 180
                            if (increment.X < MathHelper.ToRadians(180))
                                increment.X = (increment.X + vitesse_rotation) % MathHelper.ToRadians(360);
                            else
                            {
                                direction = 2;
                                increment.X = MathHelper.ToRadians(0);
                            }
                        }
                        // Le chasseur regarde à droite
                        else if (direction == 1)
                        {
                            // On le fait tourner de -90°
                            if (increment.X > MathHelper.ToRadians(0))
                                increment.X = (increment.X - vitesse_rotation) % MathHelper.ToRadians(360);
                            else
                                direction = 2;
                        }
                        // Le chasseur regarde à gauche
                        else if (direction == 3)
                        {
                            // On le fait tourner de 90°
                            if (increment.X < MathHelper.ToRadians(0))
                                increment.X = (increment.X + vitesse_rotation) % MathHelper.ToRadians(360);
                            else
                                direction = 2;
                        }
                    }
                    // Le chasseur doit aller à gauche
                    else if (min_id == 3)
                    {
                        // Le chasseur regarde en haut
                        if (direction == 0)
                        {
                            // On le fait tourner de -90°
                            if (increment.X < MathHelper.ToRadians(270))
                                increment.X = (increment.X + vitesse_rotation) % MathHelper.ToRadians(360);
                            else
                            {
                                direction = 3;
                                increment.X = MathHelper.ToRadians(-90);
                            }
                        }
                        // Le chasseur regarde à droite
                        else if (direction == 1)
                        {
                            // On le fait tourner de 180°
                            if (increment.X > MathHelper.ToRadians(-90))
                                increment.X = (increment.X - vitesse_rotation) % MathHelper.ToRadians(360);
                            else
                                direction = 3;
                        }
                        // Le chasseur regarde en bas
                        else if (direction == 2)
                        {
                            // On le fait tourner de -90°
                            if (increment.X > MathHelper.ToRadians(-90))
                                increment.X = (increment.X - vitesse_rotation) % MathHelper.ToRadians(360);
                            else
                                direction = 3;
                        }
                    }

                    RotationY(increment.X);
                    RotationZ(increment.Y);
                }

                #endregion

                #region Déplacement du scientifique
                // On déplace l'IA d'une case
                // Haut
                if (min_id == 0)
                    position.Z -= distance_deplacement * laby.CellSize.Z;
                // Droite
                else if (min_id == 1)
                    position.X += distance_deplacement * laby.CellSize.X;
                // Bas
                else if (min_id == 2)
                    position.Z += distance_deplacement * laby.CellSize.Z;
                // Gauche
                else if (min_id == 3)
                    position.X -= distance_deplacement * laby.CellSize.X;
                #endregion

                temporisation = vitesse;
            }

            if (triche)
            {

                // Déplacement de la carte des fréquence
                if (clavier.IsKeyDown(Keys.Left))
                    BordGaucheFrequence -= 5;
                if (clavier.IsKeyDown(Keys.Right))
                    BordGaucheFrequence += 5;
                if (clavier.IsKeyDown(Keys.Up))
                    BordHautFrequence -= 5;
                if (clavier.IsKeyDown(Keys.Down))
                    BordHautFrequence += 5;

                if (clavier.IsKeyDown(Keys.NumPad8))
                    vitesse += 0.0001f;
                else if (clavier.IsKeyDown(Keys.NumPad2))
                    vitesse -= 0.0001f;
            }

            case_transition = position_case_actuelle;

        }

        #region Draw
        public void DrawString(KeyboardState clavier, SpriteBatch spriteBatch, SpriteFont Kootenay, int BordGauche, int EspaceTexte, int BordHaut)
        {
            // Carte fréquence chasseur
            if (clavier.IsKeyDown(Keys.F))
            {
                spriteBatch.DrawString(Kootenay, "Carte des fréquences", new Vector2(BordGaucheFrequence + (laby.Size.X * EspaceTexte) / 2 - (4 * EspaceTexte), BordHautFrequence - EspaceTexte), Color.Red);

                for (int x = 0; x < laby.Size.X; x++)
                {
                    for (int y = 0; y < laby.Size.Y; y++)
                    {
                        if (frequence[x, y] > 0)
                            spriteBatch.DrawString(Kootenay, frequence[x, y].ToString(), new Vector2(BordGaucheFrequence + EspaceTexte * x, BordHautFrequence + EspaceTexte * y), Color.Red);
                        else
                            spriteBatch.DrawString(Kootenay, frequence[x, y].ToString(), new Vector2(BordGaucheFrequence + EspaceTexte * x, BordHautFrequence + EspaceTexte * y), Color.White);
                    }
                }
            }
        }
        #endregion

        #region Frequence minimum
        public int FrequenceMin()
        {
            int min = -1;
            int min_id = -1;
            // Haut
            if ((int)((position.Z * Scale) / laby.CellSize.Z) - 1 >= 0 &&
                laby.Carte[(int)((position.X * Scale) / laby.CellSize.X), (int)((position.Z * Scale) / laby.CellSize.Z) - 1] != 1 &&
                (min > frequence[(int)((position.X * Scale) / laby.CellSize.X), (int)((position.Z * Scale) / laby.CellSize.Z) - 1] || min == -1))
            {
                min = frequence[(int)((position.X * Scale) / laby.CellSize.X), (int)((position.Z * Scale) / laby.CellSize.Z) - 1];
                min_id = 0;
            }
            // Droite
            if ((int)((position.X * Scale) / laby.CellSize.X) + 1 < laby.Size.X &&
                laby.Carte[(int)((position.X * Scale) / laby.CellSize.X) + 1, (int)((position.Z * Scale) / laby.CellSize.Z)] != 1 &&
                (min > frequence[(int)((position.X * Scale) / laby.CellSize.X) + 1, (int)((position.Z * Scale) / laby.CellSize.Z)] || min == -1))
            {
                min = frequence[(int)((position.X * Scale) / laby.CellSize.X) + 1, (int)((position.Z * Scale) / laby.CellSize.Z)];
                min_id = 1;
            }
            // Bas
            if ((int)((position.Z * Scale) / laby.CellSize.Z) + 1 < laby.Size.Y &&
                laby.Carte[(int)((position.X * Scale) / laby.CellSize.X), (int)((position.Z * Scale) / laby.CellSize.Z) + 1] != 1 &&
                (min > frequence[(int)((position.X * Scale) / laby.CellSize.X), (int)((position.Z * Scale) / laby.CellSize.Z) + 1] || min == -1))
            {
                min = frequence[(int)((position.X * Scale) / laby.CellSize.X), (int)((position.Z * Scale) / laby.CellSize.Z) + 1];
                min_id = 2;
            }
            // Gauche
            if ((int)((position.X * Scale) / laby.CellSize.X) - 1 >= 0 &&
                laby.Carte[(int)((position.X * Scale) / laby.CellSize.X) - 1, (int)((position.Z * Scale) / laby.CellSize.Z)] != 1 &&
                (min > frequence[(int)((position.X * Scale) / laby.CellSize.X) - 1, (int)((position.Z * Scale) / laby.CellSize.Z)] || min == -1))
            {
                min = frequence[(int)((position.X * Scale) / laby.CellSize.X) - 1, (int)((position.Z * Scale) / laby.CellSize.Z)];
                min_id = 3;
            }

            return min_id;
        }
#endregion

        #region ID Minimum
        public int MinID(int[,] carte_ia)
        {
            int min = laby.Size.X * laby.Size.Y;
            int min_id = -1;
            float valeur = 1f;

            // Bas
            if ((((position.Z * Scale) / laby.CellSize.Z) + valeur) < laby.Size.Y &&
                laby.Carte[(int)((position.X * Scale) / laby.CellSize.X), (int)(((position.Z * Scale) / laby.CellSize.Z) + valeur)] != 1 &&
                (min > carte_ia[(int)((position.X * Scale) / laby.CellSize.X), (int)(((position.Z * Scale) / laby.CellSize.Z) + valeur)]))
            {
                min = carte_ia[(int)((position.X * Scale) / laby.CellSize.X), (int)(((position.Z * Scale) / laby.CellSize.Z) + valeur)];
                min_id = 2;
            }

            // Gauche
            if ((((position.X * Scale) / laby.CellSize.X) - valeur) >= 0 &&
                laby.Carte[(int)(((position.X * Scale) / laby.CellSize.X) - valeur), (int)((position.Z * Scale) / laby.CellSize.Z)] != 1 &&
                (min > carte_ia[(int)(((position.X * Scale) / laby.CellSize.X) - valeur), (int)((position.Z * Scale) / laby.CellSize.Z)]))
            {
                min = carte_ia[(int)(((position.X * Scale) / laby.CellSize.X) - valeur), (int)((position.Z * Scale) / laby.CellSize.Z)];
                min_id = 3;
            }

            // Haut
            if (((position.Z * Scale) / laby.CellSize.Z - valeur) >= 0 &&
                laby.Carte[(int)((position.X * Scale) / laby.CellSize.X), (int)(((position.Z * Scale) / laby.CellSize.Z) - valeur)] != 1 &&
                (min >= carte_ia[(int)((position.X * Scale) / laby.CellSize.X), (int)(((position.Z * Scale) / laby.CellSize.Z) - valeur)]))
            {
                min = carte_ia[(int)((position.X * Scale) / laby.CellSize.X), (int)(((position.Z * Scale) / laby.CellSize.Z) - valeur)];
                min_id = 0;
            }
            // Droite
            if ((((position.X * Scale) / laby.CellSize.X) + valeur) < laby.Size.X &&
                laby.Carte[(int)(((position.X * Scale) / laby.CellSize.X) + valeur), (int)((position.Z * Scale) / laby.CellSize.Z)] != 1 &&
                (min >= carte_ia[(int)(((position.X * Scale) / laby.CellSize.X) + valeur), (int)((position.Z * Scale) / laby.CellSize.Z)]))
            {
                min = carte_ia[(int)(((position.X * Scale) / laby.CellSize.X) + valeur), (int)((position.Z * Scale) / laby.CellSize.Z)];
                min_id = 1;
            }
            return min_id;
        }
        #endregion
    }
}