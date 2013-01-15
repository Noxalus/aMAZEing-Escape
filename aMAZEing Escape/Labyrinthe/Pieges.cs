using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

namespace aMAZEing_Escape
{
    class Pieges
    {
        private Model piques;
        private Model trappe;
        private Model laser;
        public Vector3 position;
        public Point position_case;
        public int type;
        private static Random rand = new Random();
        public bool afficher_trappe;
        private Effect effet_laser;
        private float trappe_declenchement;
        public string direction_laser;
        public float hauteur_laser;
        private Vector3 cellsize;
        public string trappe_ferme;

        private float rotation_trappe;
        private Vector2 translation_trappe;

        public Pieges(Labyrinthe laby, ContentManager Content, Vector3 position)
        {
            this.position = position;
            position_case = new Point((int)(position.X / laby.CellSize.X), (int)(position.Z / laby.CellSize.Z));
            cellsize = laby.CellSize;

            // On choisit aléatoirement le type du bonus            
            if (Config.trappe && Config.laser)
                type = rand.Next(0, 2);
            else if (Config.laser)
                type = 1;
            else
                type = 0;

            if (type == 1)
            {
                // Si il y a 2 murs face à face sur cette case
                if (
                ((position_case.Y == Config.hauteur_labyrinthe - 1 || laby.Carte[position_case.X, position_case.Y + 1] == 1) && (position_case.Y == 0 || laby.Carte[position_case.X, position_case.Y - 1] == 1)) ||
                ((position_case.X == Config.largeur_labyrinthe - 1 || laby.Carte[position_case.X + 1, position_case.Y] == 1) && (position_case.X == 0 || laby.Carte[position_case.X - 1, position_case.Y] == 1)))
                {
                    if ((position_case.X == Config.largeur_labyrinthe - 1 || laby.Carte[position_case.X + 1, position_case.Y] == 1) && (position_case.X == 0 || laby.Carte[position_case.X - 1, position_case.Y] == 1))
                        direction_laser = "EO";
                    else
                        direction_laser = "NS";
                }
                else
                    type = 0;
            }

            // On charge le model
            // Piques
            if (type == 0)
            {
                piques = Content.Load<Model>(@"Models\Pieges\piques");
                trappe = Content.Load<Model>(@"Models\Pieges\trappe");
                position = new Vector3(position.X, position.Y - laby.CellSize.Y, position.Z);
                afficher_trappe = true;
                trappe_ferme = "trappe_ferme";
                rotation_trappe = 90;
                translation_trappe = Vector2.Zero;

                if (Config.index_difficulte == 2)
                    trappe_declenchement = 2f;
                else if (Config.index_difficulte == 1)
                    trappe_declenchement = 3f;
                else
                    trappe_declenchement = 4f;

            }
            // Laser
            else
            {
                effet_laser = Content.Load<Effect>(@"Effets\laser");
                laser = Content.Load<Model>(@"Models\Pieges\laser");
                
                if (rand.Next(2) == 1)
                    hauteur_laser = 0.5f;
                else
                    hauteur_laser = laby.CellSize.Y / 2;

                this.position = new Vector3(this.position.X, this.position.Y + hauteur_laser, this.position.Z - laby.CellSize.Z / 2);
            }
        }

        public void Draw(GraphicsDeviceManager graphics, Matrix Projection, Vector3 position_joueur, Vector3 position_ia, float scale_ia, Vector3 cible, SoundBank BanqueSon, bool resolution, bool[,] carte_resolution)
        {
            //On déssine les pièges
            // Si c'est des piques
            if (type == 0)
            {
                double distance_joueur_trappe = Math.Sqrt(Math.Pow(position_joueur.X - position.X, 2) + Math.Pow(position_joueur.Z - position.Z, 2));
                double distance_ia_trappe = Math.Sqrt(Math.Pow(position_ia.X * scale_ia - position.X, 2) + Math.Pow(position_ia.Z * scale_ia - position.Z, 2));

                if (distance_joueur_trappe >= trappe_declenchement || distance_ia_trappe <= 3)
                {
                    if(!afficher_trappe)
                        BanqueSon.PlayCue("trappe_ferme");
                    afficher_trappe = true;
                }
                else if (afficher_trappe)
                {
                    BanqueSon.PlayCue("trappe");
                    afficher_trappe = false;
                }

                // Les piques
                Matrix[] transforms = new Matrix[piques.Bones.Count];
                piques.CopyAbsoluteBoneTransformsTo(transforms);

                foreach (ModelMesh mesh in piques.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.Projection = Projection;
                        effect.View = Matrix.CreateLookAt(position_joueur, cible, Vector3.Up);
                        effect.World = transforms[mesh.ParentBone.Index] * Matrix.CreateTranslation(new Vector3(position.X, position.Y - 2.5f, position.Z));
                    }
                    mesh.Draw();
                }

                if (afficher_trappe)
                {
                    if (rotation_trappe < 90)
                    {
                        rotation_trappe += 10f;

                        translation_trappe.X -= 0.135f;
                        translation_trappe.Y += 0.14f;
                        
                    }
                }
                else
                {
                    if (rotation_trappe > 0)
                    {
                        rotation_trappe -= 10f;

                        translation_trappe.X += 0.135f;
                        translation_trappe.Y -= 0.14f;

                    }
                }
                /*
                translation_trappe.X = (float)Math.Cos(MathHelper.ToRadians(rotation_trappe));
                translation_trappe.Y = (float)Math.Sin(MathHelper.ToRadians(rotation_trappe)) - 1;
                */
                // Les trappes
                Matrix[] transforms2 = new Matrix[trappe.Bones.Count];
                trappe.CopyAbsoluteBoneTransformsTo(transforms2);

                foreach (ModelMesh mesh in trappe.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        if(resolution && carte_resolution[position_case.X, position_case.Y])
                            effect.DiffuseColor = new Vector3(255, 255, 0);
                        else
                            effect.DiffuseColor = new Vector3(255, 255, 255);

                        effect.Projection = Projection;
                        effect.View = Matrix.CreateLookAt(position_joueur, cible, Vector3.Up);
                        effect.World = Matrix.CreateRotationY(0)
                        * Matrix.CreateRotationX(MathHelper.ToRadians(rotation_trappe))
                        * Matrix.CreateRotationZ(0)
                        * Matrix.CreateTranslation(new Vector3(position.X, position.Y + translation_trappe.Y, position.Z + translation_trappe.X));
                    }
                    mesh.Draw();
                }
            }
            else
            {
                // Les lasers
                Matrix[] transforms = new Matrix[laser.Bones.Count];
                laser.CopyAbsoluteBoneTransformsTo(transforms);


                graphics.GraphicsDevice.RenderState.AlphaBlendEnable = true;
                graphics.GraphicsDevice.RenderState.AlphaTestEnable = false;
                graphics.GraphicsDevice.RenderState.ReferenceAlpha = 75; 

                foreach (ModelMesh mesh in laser.Meshes)
                {
                    foreach (BasicEffect effet_laser in mesh.Effects)
                    {
                        effet_laser.AmbientLightColor = new Vector3(0.2f, 1, 0);
                        effet_laser.DiffuseColor = new Vector3(0.2f, 1, 0);
                        effet_laser.EmissiveColor = new Vector3(0.2f, 1, 0);

                        effet_laser.Projection = Projection;
                        effet_laser.View = Matrix.CreateLookAt(position_joueur, cible, Vector3.Up);
                        if (direction_laser == "EO")
                            effet_laser.World =
                            transforms[mesh.ParentBone.Index]
                            * Matrix.CreateRotationX(MathHelper.ToRadians(90))
                            * Matrix.CreateRotationY(0)
                            * Matrix.CreateRotationZ(0)
                            * Matrix.CreateTranslation(new Vector3((this.position.X) / 125, this.position.Y, this.position.Z + (float)cellsize.Z / 2))
                            * Matrix.CreateScale(new Vector3(125, 1, 1));
                        else
                            effet_laser.World =
                            transforms[mesh.ParentBone.Index]
                            * Matrix.CreateRotationX(0)
                            * Matrix.CreateRotationY(0)
                            * Matrix.CreateRotationZ(0)
                            * Matrix.CreateTranslation(new Vector3(this.position.X, this.position.Y, this.position.Z / 125))
                            * Matrix.CreateScale(new Vector3(1, 1, 125));
                    }
                    mesh.Draw();
                }
            }
        }
    }
}