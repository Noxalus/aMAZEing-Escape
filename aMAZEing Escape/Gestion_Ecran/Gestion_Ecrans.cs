using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace aMAZEing_Escape
{
    /// <summary>
    /// Screen Manager
    /// Keeps a list of available screens
    /// so you can switch between them, 
    /// ie. jumping from the start screen to the game screen 
    /// </summary>
    public static class Gestion_Ecran
    {
        // Protected Members
        static private List<Ecran> ecrans = new List<Ecran>();
        static private bool started = false;
        static private Ecran precedent = null;
        // Public Members
        static public Ecran EcranActif = null;

        /// <summary>
        /// Add new Screen
        /// </summary>
        /// <param name="screen">New screen, name must be unique</param>
        static public void Ajouter_Ecran(Ecran ecran)
        {
            foreach (Ecran ecr in ecrans)
            {
                if (ecr.nom == ecran.nom)
                {
                    return;
                }
            }
            ecrans.Add(ecran);
        }

        static public int Get_Nombre_Ecran()
        {
            return ecrans.Count;
        }

        static public Ecran Get_Ecran(int idx)
        {
            return ecrans[idx];
        }

        /// <summary>
        /// Go to screen
        /// </summary>
        /// <param name="name">Screen name</param>
        static public void Goto_Ecran(string nom)
        {
            foreach (Ecran ecran in ecrans)
            {
                if (ecran.nom == nom)
                {
                    // Shutsdown Previous Screen           
                    precedent = EcranActif;
                    if (EcranActif != null)
                        EcranActif.Shutdown();
                    // Inits New Screen
                    EcranActif = ecran;
                    if (started) 
                        EcranActif.Init();
                    return;
                }
            }
        }

        /// <summary>
        /// Init Screen manager
        /// Only at this point is screen manager going to init the selected screen
        /// </summary>
        static public void Init()
        {
            started = true;
            if (EcranActif != null)
                EcranActif.Init();
        }
        /// <summary>
        /// Falls back to previous selected screen if any
        /// </summary>
        static public void Retourner()
        {
            if (precedent != null)
            {
                Goto_Ecran(precedent.nom);
                return;
            }
        }


        /// <summary>
        /// Updates Active Screen
        /// </summary>
        /// <param name="elapsed">GameTime</param>
        static public void Update(GameTime gameTime)
        {
            if (started == false) 
                return;
            if (EcranActif != null)
                EcranActif.Update(gameTime);
        }

        static public void Draw(GameTime gameTime)
        {
            if (started == false) return;
            if (EcranActif != null)
            {
                EcranActif.Draw(gameTime);
            }
        }
    }
}
