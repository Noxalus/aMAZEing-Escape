using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace aMAZEing_Escape
{
    class Bonus
    {
        Model bonus;
        public Vector3 position_bonus;
        public Point position_case_bonus;
        public int type;
        static Random rand = new Random();

        public Bonus(Labyrinthe laby, ContentManager Content, Vector3 position, List<int> liste_bonus_actifs)
        {
            position_bonus = position;
            position_case_bonus = new Point((int)(position_bonus.X / laby.CellSize.X), (int)(position_bonus.Z / laby.CellSize.Z));

            // On choisit aléatoirement le type du bonus            
            /*
             * 0 => Lenteur
             * 1 => Inversion
             * 2 => Gel
             * 3 => Touches changées
             * 4 => Caméra inversée
             * 5 => Téléportation
             * 6 => Sprint
             * 7 => Fil d'Ariane
             * 8 => Carte
             * 9 => Caméra changée
             * 10 => Obscurité
             * 11 => Boussole sortie
            */
            if (liste_bonus_actifs.Count > 0)
            {
                type = rand.Next(liste_bonus_actifs.Count);
                type = liste_bonus_actifs[type];
            }
            else
                type = rand.Next(12);
            // On charge le model
            bonus = Content.Load<Model>(@"Models\bonus");
        }

        public void Draw(Matrix Projection, Vector3 position, Vector3 cible)
        {
            //On déssine le bonus
            Matrix[] transforms = new Matrix[bonus.Bones.Count];
            bonus.CopyAbsoluteBoneTransformsTo(transforms);

            foreach (ModelMesh mesh in bonus.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.Projection = Projection;
                    effect.View = Matrix.CreateLookAt(position, cible, Vector3.Up);
                    effect.World = transforms[mesh.ParentBone.Index] * Matrix.CreateTranslation(position_bonus);
                }
                mesh.Draw();
            }
        }
    }
}