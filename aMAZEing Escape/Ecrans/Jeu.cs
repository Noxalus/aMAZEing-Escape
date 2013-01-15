#region Using Statements
using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;
#endregion

namespace aMAZEing_Escape
{
    class Jeu : Ecran
    {
        #region Fields
        SpriteFont Kootenay;
        SpriteFont Arial;
        SpriteFont perso;

        SpriteBatch spriteBatch;

        public Effect effet;

        // Aléatoire
        static Random rand;

        // Triche
        bool triche;
        string pass;
        float compteur_message_triche;

        // Gestion de l'audio (XACT)
        private AudioEngine Audio;
        private SoundBank SoundBank;
        private WaveBank WaveBank;

        // Sons et musiques
        private Cue musique_ambiance;
        private Cue musique_poursuite;
        private Cue son_ambiance_aleatoire;
        private string son_mort;
        private Cue son_mort_subite;
        private bool son_mort_subite_unique;
        private float compteur_son_ambiance_aleatoire;

        // Création de l'objet joueur
        Joueur joueur;
        bool son_mort_unique;

        // L'IA
        IA ia;

        // Création de l'objet chasseur
        Chasseur chasseur;
        bool poursuite;
        double distance_joueur_chasseur;
        float compteur_animation_fin_chasseur;
        bool chasseur_touche;

        // Création du scientifique
        Scientifique scientifique;
        float compteur_animation_fin_scientifique;
        bool scientifique_touche;
        double distance_joueur_scientifique;

        // Chronomètre
        float chrono;
        float chrono_meilleur_score;
        Texture2D chronometre_texture;

        //Mort Subite
        int mortsubite;
        double facteur;
        bool disapply_mortsubite;

        // Boussole
        Texture2D boussole_support;
        Texture2D boussole_fleche;
        Texture2D boussole_fleche_sortie;

        //Radar
        Texture2D radar_support;
        Texture2D radar_cible;

        // Initialisation de la caméra
        Matrix Projection;
        float nearPlane;
        float farPlane;
        float fieldOfView;
        float aspectRatio;
        float yaw;
        float pitch;
        Matrix Vue;

        // Le labyrinthe
        Labyrinthe laby;
        List<Material> materials;
        List<Material> materials_save;
        Texture2D[] textures;
        MazeMaterialBuilder materialBuilder;

        // Carte de résolution
        bool[,] carte_joueur_sortie;
        bool[,] carte_joueur_scientifique;

        // Gestion du clavier est de la souris
        KeyboardState clavier;
        KeyboardState clavier_prec;
        MouseState souris;
        MouseState souris_prec;

        // Infos par texte
        int InfoTexteGauche;
        int InfoTexteHaut;

        // La carte 2D
        Texture2D carte2D_chemin;
        Texture2D carte2D_mur;
        Texture2D carte2D_joueur;
        Texture2D carte2D_sortie;
        Texture2D carte2D_chasseur;
        Texture2D carte2D_bonus;
        Texture2D carte2D_piege;
        int BordGauche;
        int BordHaut;
        int EspaceTexte;
        int BordGaucheTexture;
        int BordHautTexture;
        int EspaceTexture;
        int TextureRedimension;
        Vector2 carte2D_taille_texture;
        Color carte2D_couleur_texture;
        public float echelle;

        // Bonus
        private List<int> liste_bonus_actifs;
        private float compteur_message_bonus;
        private double distance_bonus_joueur;
        private float compteur_total;
        private bool inverser_position;
        private bool teleportation_aleatoire;
        private float geler_compteur;
        private float changervitesse_compteur;
        private float inversertouche_compteur;
        private bool inversercamera;
        private float inversercamera_compteur;
        private bool changercamera;
        private float changercamera_compteur;
        private float fil_ariane_compteur;
        private float affichercarte2D_compteur;
        private bool affichercarte2D;
        private float cecite_compteur;
        private bool cecite;
        private byte cecite_transparence;
        private byte cecite_transparence_tmp;
        private bool boussole_sortie;
        private float boussole_sortie_compteur;
        Texture2D noir;

        // La temporisation
        private Texture2D tempo_load;
        private Texture2D tempo_fixe;
        private int tempo_load_largeur;
        private int tempo_load_hauteur;
        private int tempo_fixe_largeur;
        private int tempo_fixe_hauteur;
        private bool barre_redimension;

        // Transitions
        private Texture2D transition_fin_blanc;
        private bool transition_debut_finie;
        private float compteur_transition_debut;
        private string[] transition_debut_messages;

        #region Constructeur
        public Jeu(GraphicsDeviceManager graphics, ContentManager Content)
            : base(graphics, Content, "Jeu")
        {
        }
        #endregion

        #endregion

        #region Initialization

        public override bool Init()
        {
            if (!Config.pause)
            {
                /**************************************/
                /*** Constantes pour le labyrinthe ****/
                /**************************************/
                int hauteur = Config.hauteur_labyrinthe;
                int largeur = Config.largeur_labyrinthe;

                // On génére un chiffre aléatoire
                Random rand = new Random();

                spriteBatch = new SpriteBatch(graphics.GraphicsDevice);

                // On charge la police du texte
                Kootenay = Content.Load<SpriteFont>(@"Polices\Kootenay");
                Arial = Content.Load<SpriteFont>(@"Polices\Arial");

                perso = Content.Load<SpriteFont>(@"Polices\Perso");

                // Carte 2D
                carte2D_joueur = Content.Load<Texture2D>(@"Images\carte2D\joueur");
                carte2D_mur = Content.Load<Texture2D>(@"Images\carte2D\mur");
                carte2D_chemin = Content.Load<Texture2D>(@"Images\carte2D\chemin");
                carte2D_sortie = Content.Load<Texture2D>(@"Images\carte2D\sortie");
                if (Config.modedejeu)
                {
                    carte2D_chasseur = Content.Load<Texture2D>(@"Images\carte2D\chasseur");
                    carte2D_joueur = Content.Load<Texture2D>(@"Images\carte2D\joueur");
                }
                else
                {
                    carte2D_chasseur = Content.Load<Texture2D>(@"Images\carte2D\joueur");
                    carte2D_joueur = Content.Load<Texture2D>(@"Images\carte2D\chasseur");
                }

                carte2D_bonus = Content.Load<Texture2D>(@"Images\carte2D\bonus");
                carte2D_piege = Content.Load<Texture2D>(@"Images\carte2D\piege");

                // Boussole et Chronometre 
                if (Config.modedejeu)
                {
                    chronometre_texture = Content.Load<Texture2D>(@"Images\chronometre");
                    boussole_support = Content.Load<Texture2D>(@"Images\boussole_support");
                    boussole_fleche = Content.Load<Texture2D>(@"Images\boussole_fleche");
                    boussole_fleche_sortie = Content.Load<Texture2D>(@"Images\boussole_fleche-sortie");
                }
                else
                {
                    radar_cible = Content.Load<Texture2D>(@"Images\radar_cible");
                    radar_support = Content.Load<Texture2D>(@"Images\radar_support");
                }

                // Temporisation
                tempo_load = Content.Load<Texture2D>(@"Images\tempo_load");
                tempo_fixe = Content.Load<Texture2D>(@"Images\tempo_fixe");

                effet = Content.Load<Effect>(@"Effets\Effet2");

                // Audio
                Audio = new AudioEngine(@"Content\Musiques\ambiance.xgs");

                // Charge la banque de musiques
                SoundBank = new SoundBank(Audio, @"Content\Musiques\Sound Bank.xsb");
                WaveBank = new WaveBank(Audio, @"Content\Musiques\Wave Bank.xwb");

                // Joue la musique d'ambiance
                musique_ambiance = SoundBank.GetCue("ambiance_labyrinthe");

                // Sons
                if (Config.modedejeu)
                    son_mort = "mort";
                else
                    son_mort = "mort_chasseur";

                musique_ambiance.Play();

                // Fixe le volume du jeu
                Audio.GetCategory("Musiques").SetVolume(aMAZEing_Escape.Properties.Settings.Default.volume_musiques);
                Audio.GetCategory("Sons").SetVolume(aMAZEing_Escape.Properties.Settings.Default.volume_sons);

                // Sons d'ambiance aléatoire
                son_ambiance_aleatoire = SoundBank.GetCue("son_ambiance_aleatoire");
                compteur_son_ambiance_aleatoire = rand.Next(10, 30);

                // Son mort subite
                if (Config.modedejeu)
                    son_mort_subite = SoundBank.GetCue("son_mort_subite_chasseur");
                else
                    son_mort_subite = SoundBank.GetCue("son_mort_subite_chasse");

                son_mort_subite_unique = false;

                if (Config.modedejeu)
                    musique_poursuite = SoundBank.GetCue("poursuite");

                // On charge les textures
                textures = new Texture2D[8];
                textures[0] = Content.Load<Texture2D>(@"Textures\texture_plafond");
                textures[1] = Content.Load<Texture2D>(@"Textures\texture_mur");
                textures[2] = Content.Load<Texture2D>(@"Textures\texture_mur_sortie");
                textures[3] = Content.Load<Texture2D>(@"Textures\texture_fond_sortie");
                textures[4] = Content.Load<Texture2D>(@"Textures\texture_sol");
                textures[5] = Content.Load<Texture2D>(@"Textures\texture_mur_trappe");
                textures[6] = Content.Load<Texture2D>(@"Textures\texture_sol_trappe");
                textures[7] = Content.Load<Texture2D>(@"Textures\texture_sol_plafond_sortie");

                noir = Content.Load<Texture2D>(@"Images\black");

                transition_fin_blanc = Content.Load<Texture2D>(@"Images\blanc");

                compteur_transition_debut = 2f;
                transition_debut_finie = false;
                transition_debut_messages = new string[2];
                transition_debut_messages[0] = "Pret ?";
                transition_debut_messages[1] = "Go !";

                /***************************************************/
                /***************** Initialisation ! ****************/
                /***************************************************/

                // Triche
                triche = false;
                pass = "";
                compteur_message_triche = 0;

                // Liste des bonus actifs
                if (!Config.modedejeu)
                    Config.boussole_sortie = false;
                liste_bonus_actifs = new List<int>();
                liste_bonus_actifs = Liste_Bonus_Actifs();

                if (liste_bonus_actifs.Count == 0)
                    Config.active_bonus = false;

                // Pièges désactivés ?
                if (!Config.trappe && !Config.laser)
                    Config.active_pieges = false;

                // Génération du labyrinthe
                laby = new Labyrinthe();

                laby.BuildMaze(new Point(largeur, hauteur), new Vector3(2.5f), Content, laby, graphics.GraphicsDevice, liste_bonus_actifs);
                // On charge le labyrinthe en vertices
                materialBuilder = new MazeMaterialBuilder();

                // On génére un chiffre aléatoire
                rand = new Random();

                // Joueur
                joueur = new Joueur(graphics.GraphicsDevice.Viewport, laby, SoundBank);
                son_mort_unique = false;

                // Chasseur
                if (Config.modedejeu)
                {
                    // IA
                    ia = new IA(laby, new Point((int)(joueur.position.X / laby.CellSize.X), (int)(joueur.position.Z / laby.CellSize.Z)), graphics.GraphicsDevice.Viewport);

                    chasseur = new Chasseur(graphics.GraphicsDevice.Viewport, laby, Content, ia);
                    poursuite = false;
                    compteur_animation_fin_chasseur = 1f;
                    chasseur_touche = false;
                }
                // Scientifique
                else
                {
                    // IA
                    ia = new IA(laby, laby.sortie_position, graphics.GraphicsDevice.Viewport);

                    scientifique = new Scientifique(graphics.GraphicsDevice.Viewport, laby, Content, ia);
                    compteur_animation_fin_scientifique = 1f;
                    scientifique_touche = false;
                }

                // Carte de sortie
                if (Config.modedejeu)
                {
                    carte_joueur_sortie = new bool[Config.largeur_labyrinthe, Config.hauteur_labyrinthe];
                    GenCarteJoueurSortie();
                    materials = materialBuilder.BuildMazeMaterial(laby, graphics.GraphicsDevice, textures, joueur.fil_ariane, carte_joueur_sortie);
                    materials_save = materials;
                }
                else
                {
                    carte_joueur_scientifique = new bool[Config.largeur_labyrinthe, Config.hauteur_labyrinthe];
                    GenCarteJoueurScientifique();
                    materials = materialBuilder.BuildMazeMaterial(laby, graphics.GraphicsDevice, textures, joueur.fil_ariane, carte_joueur_scientifique);
                    materials_save = materials;
                }

                // Infos par texte
                InfoTexteGauche = 0;
                InfoTexteHaut = 40;

                // Constantes pour l'affichage de la carte 2D
                EspaceTexte = 22;
                EspaceTexture = carte2D_chemin.Width;

                TextureRedimension = 1;
                carte2D_taille_texture = new Vector2(carte2D_chemin.Width * TextureRedimension, carte2D_chemin.Height * TextureRedimension);
                BordGaucheTexture = (int)(graphics.GraphicsDevice.Viewport.Width / 2 - (carte2D_taille_texture.X * (laby.Size.X) * echelle) / 2);
                BordHautTexture = (int)(graphics.GraphicsDevice.Viewport.Height / 2 - (carte2D_taille_texture.Y * laby.Size.Y * echelle) / 2);
                carte2D_couleur_texture = Color.White;

                echelle = 0;
                while (carte2D_taille_texture.Y * echelle * laby.Size.Y < graphics.GraphicsDevice.Viewport.Height - (graphics.GraphicsDevice.Viewport.Height / 4) &&
                        carte2D_taille_texture.X * echelle * laby.Size.X < graphics.GraphicsDevice.Viewport.Height)
                {
                    echelle += 0.01f;
                }

                BordGaucheTexture = (int)(graphics.GraphicsDevice.Viewport.Width / 2 - (carte2D_taille_texture.X * (laby.Size.X) * echelle) / 2);
                BordHautTexture = (int)(graphics.GraphicsDevice.Viewport.Height / 2 - (carte2D_taille_texture.Y * laby.Size.Y * echelle) / 2);

                // Pour le déplacement de la caméra
                nearPlane = 0.01f;
                farPlane = 10000.0f;
                fieldOfView = 60.0f;
                aspectRatio = (float)graphics.GraphicsDevice.Viewport.Width / (float)graphics.GraphicsDevice.Viewport.Height;
                Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(fieldOfView), aspectRatio, nearPlane, farPlane);

                // Souris
                yaw = 0;
                pitch = 0;

                clavier_prec = Keyboard.GetState();
                Mouse.SetPosition(graphics.GraphicsDevice.Viewport.Width / 2, graphics.GraphicsDevice.Viewport.Height / 2);
                souris_prec = Mouse.GetState();

                // Chrono
                chrono = 1;
                chrono_meilleur_score = 0;

                // Bonus
                geler_compteur = 0;
                changervitesse_compteur = 0;
                inverser_position = false;
                teleportation_aleatoire = false;
                inversertouche_compteur = 0;
                inversercamera_compteur = 0;
                changercamera_compteur = 0;
                inversercamera = false;
                changercamera = false;
                affichercarte2D_compteur = 0;
                affichercarte2D = false;
                cecite = false;
                cecite_compteur = 0;
                boussole_sortie = false;
                boussole_sortie_compteur = 0;
                compteur_message_bonus = 0;

                compteur_total = 0;

                // Temporisation
                tempo_load_largeur = ((tempo_load.Width / 2) * graphics.GraphicsDevice.Viewport.Width) / 800;
                tempo_load_hauteur = ((tempo_load.Height / 2) * graphics.GraphicsDevice.Viewport.Height) / 600;

                tempo_fixe_largeur = graphics.GraphicsDevice.Viewport.Width;
                tempo_fixe_hauteur = ((tempo_fixe.Height / 1) * graphics.GraphicsDevice.Viewport.Height) / 600;

                ia.CaseIntersection(); //Définis les cases du labyrinthe qui sont des intersections.
                ia.CaseImpasse(); //Définis les cases du labyrinthe qui sont des impasses.
                if(Config.active_mort_subite)
                    mortsubite = 60 * Config.min_mort_subite + Config.sec_mort_subite;
                facteur = 1.52196;
                
            }
            else
            {
                Config.pause = false;
                if (Config.modedejeu)
                {
                    if(musique_poursuite.IsPaused)
                        musique_poursuite.Resume();
                }
                musique_ambiance.Resume();
            }
            return base.Init();
        }
        #endregion

        #region Shutdown
        public override void Shutdown()
        {
            if (!Config.pause)
            {
                foreach (Material m in materials)
                    m.Dispose();
                if (Config.modedejeu)
                    musique_poursuite.Dispose();
                musique_ambiance.Dispose();
            }
            graphics.GraphicsDevice.RenderState.FogEnable = false;
            base.Shutdown();
        }
        #endregion

        #region Update
        public override void Update(GameTime gameTime)
        {
            float temps = (float)gameTime.ElapsedGameTime.TotalSeconds;

            Config.chrono = chronometre(chrono);

            clavier = Keyboard.GetState();
            souris = Mouse.GetState();
            Random rand = new Random();

            // Transition au début du jeu
            if (!transition_debut_finie && compteur_transition_debut > 0)
            {
                compteur_transition_debut -= temps;
                if (compteur_transition_debut < 0)
                    transition_debut_finie = true;
            }


            // Pause
            if (clavier.IsKeyDown(Keys.Escape) && !clavier_prec.IsKeyDown(Keys.Escape))
            {
                Config.pause = true;
                if (Config.modedejeu)
                {
                    if (musique_poursuite.IsPlaying)
                        musique_poursuite.Pause();
                }
                musique_ambiance.Pause();

                graphics.GraphicsDevice.RenderState.FogEnable = false;
                Gestion_Ecran.Goto_Ecran("Menu Principal");
                this.Shutdown();
            }

            // Triche
            Triche();

            if (triche && clavier.IsKeyDown(Keys.NumPad0))
            {
                // On charge le labyrinthe en vertices
                MazeMaterialBuilder materialBuilder = new MazeMaterialBuilder();
            }

            if (Config.modedejeu)
            {
                // Calcul de la distance joueur - chasseur
                distance_joueur_chasseur = Math.Sqrt(Math.Pow(chasseur.position.X * chasseur.Scale - joueur.position.X, 2) + Math.Pow(chasseur.position.Z * chasseur.Scale - joueur.position.Z, 2));
            }
            else
            {
                // Calcul de la distance joueur - scientifique
                distance_joueur_scientifique = Math.Sqrt(Math.Pow(scientifique.position.X * scientifique.Scale - joueur.position.X, 2) + Math.Pow(scientifique.position.Z * scientifique.Scale - joueur.position.Z, 2));
            }

            #region Fin du jeu

            // Gagné (trouvé la sortie)
            if ((Config.modedejeu && laby.Carte[joueur.position_case_actuelle.X, joueur.position_case_actuelle.Y] == 3))
            {
                Config.cause_fin_jeu = "Vous avez trouvé la sortie !";
                Gestion_Ecran.Goto_Ecran("Succes");
                this.Shutdown();
            }
            // Le joueur touche le scientifique
            else if (!Config.modedejeu && (distance_joueur_scientifique < 1 || scientifique_touche))
            {
                scientifique_touche = true;
                compteur_animation_fin_scientifique -= temps;
                if (compteur_animation_fin_scientifique < 0)
                {
                    Config.cause_fin_jeu = "Vous avez trouvé le scientifique !";
                    Gestion_Ecran.Goto_Ecran("Succes");
                    this.Shutdown();
                }
            }

            // Perdu (attrapé par le chasseur)
            if ((Config.modedejeu && distance_joueur_chasseur < 1.5 && laby.Carte[joueur.position_case_actuelle.X, joueur.position_case_actuelle.Y] != 1) || chasseur_touche)
            {
                joueur.vivant = false;
                chasseur_touche = true;
                compteur_animation_fin_chasseur -= temps;

                if (compteur_animation_fin_chasseur < 0.25 && !son_mort_unique)
                {
                    SoundBank.PlayCue(son_mort);
                    son_mort_unique = true;
                }
                if (compteur_animation_fin_chasseur < 0)
                {
                    chasseur_touche = false;
                    // Screen
                    Config.cause_fin_jeu = "Vous vous êtes fait attraper";
                    Gestion_Ecran.Goto_Ecran("Fail");
                    this.Shutdown();
                }
            }
            // Perdu (le scientifique a trouvé la sortie)
            else if (!Config.modedejeu && laby.sortie_position == scientifique.position_case_actuelle)
            {
                // Screen
                Config.cause_fin_jeu = "Le scientifique a réussi à s'échapper !";
                Gestion_Ecran.Goto_Ecran("Fail");
                this.Shutdown();
            }

            // Perdu (piques)
            if (joueur.position.Y < laby.joueur_position_initiale.Y - laby.CellSize.Y)
            {
                SoundBank.PlayCue(son_mort);
                Config.cause_fin_jeu = "Vous vous êtes fait empaler !";
                Gestion_Ecran.Goto_Ecran("Fail");
                this.Shutdown();
            }

            // Perdu (lasers)
            if (joueur.laser)
            {
                foreach (Pieges piege in laby.liste_pieges)
                {
                    if (piege.position_case == joueur.position_case_actuelle)
                    {
                        if (piege.direction_laser == "NS")
                        {
                            if (piege.position.X + 0.5 >= joueur.position.X && joueur.position.X >= piege.position.X - 0.5)
                            {
                                if (piege.hauteur_laser == laby.CellSize.Y / 2 && !joueur.baisse)
                                {
                                    SoundBank.PlayCue(son_mort);
                                    Config.cause_fin_jeu = "Vous vous êtes fait\ndécouper par un laser !";
                                    Gestion_Ecran.Goto_Ecran("Fail");
                                    this.Shutdown();
                                }
                                else if (piege.hauteur_laser == 0.5 && !joueur.saut)
                                {
                                    SoundBank.PlayCue(son_mort);
                                    Config.cause_fin_jeu = "Vous vous êtes fait\ndécouper par un laser !";
                                    Gestion_Ecran.Goto_Ecran("Fail");
                                    this.Shutdown();
                                }
                            }
                        }
                        else
                        {
                            if (piege.position.Z + 0.5 + laby.CellSize.Z / 2 >= joueur.position.Z && joueur.position.Z >= piege.position.Z - 0.5 + laby.CellSize.Z / 2)
                            {
                                if (piege.hauteur_laser == laby.CellSize.Y / 2 && !joueur.baisse)
                                {
                                    SoundBank.PlayCue(son_mort);
                                    Config.cause_fin_jeu = "Vous vous êtes fait\ndécouper par un laser !";
                                    Gestion_Ecran.Goto_Ecran("Fail");
                                    this.Shutdown();
                                }
                                else if (piege.hauteur_laser == 0.5 && !joueur.saut)
                                {
                                    SoundBank.PlayCue(son_mort);
                                    Config.cause_fin_jeu = "Vous vous êtes fait\ndécouper par un laser !";
                                    Gestion_Ecran.Goto_Ecran("Fail");
                                    this.Shutdown();
                                }
                            }
                        }
                    }
                }
            }
            #endregion

            // Zoom carte 2D
            if (clavier.IsKeyDown(Keys.PageUp))
                echelle += 0.01f;
            if (clavier.IsKeyDown(Keys.PageDown))
                echelle -= 0.01f;

            // Supprime tout le labyrinthe !
            if (triche && clavier.IsKeyDown(Keys.Delete))
            {
                if (materials != null)
                {
                    foreach (Material m in materials)
                    {
                        m.Dispose();
                    }
                    materials = null;
                }
            }

            if (triche && clavier.IsKeyDown(Keys.T) && !clavier_prec.IsKeyDown(Keys.T))
                TeleportationAleatoire();

            // Déplacer les informations sur le jeu
            if (triche && (clavier.IsKeyDown(Keys.A)))
            {
                // Déplacement de la carte 2D
                if (clavier.IsKeyDown(Keys.Left))
                    InfoTexteGauche -= 5;
                if (clavier.IsKeyDown(Keys.Right))
                    InfoTexteGauche += 5;
                if (clavier.IsKeyDown(Keys.Up))
                    InfoTexteHaut -= 5;
                if (clavier.IsKeyDown(Keys.Down))
                    InfoTexteHaut += 5;
            }

            // Déplacer la carte de l'IA
            if (triche && (clavier.IsKeyDown(Keys.Tab) || clavier.IsKeyDown(Keys.I)))
            {
                // Déplacement de la carte 2D
                if (clavier.IsKeyDown(Keys.Left))
                    BordGauche -= 5;
                if (clavier.IsKeyDown(Keys.Right))
                    BordGauche += 5;
                if (clavier.IsKeyDown(Keys.Up))
                    BordHaut -= 5;
                if (clavier.IsKeyDown(Keys.Down))
                    BordHaut += 5;
            }

            // Déplacer la carte 2D
            if (triche && clavier.IsKeyDown(Keys.E))
            {
                // Déplacement de la carte 2D
                if (clavier.IsKeyDown(Keys.Left))
                    BordGaucheTexture -= 5;
                if (clavier.IsKeyDown(Keys.Right))
                    BordGaucheTexture += 5;
                if (clavier.IsKeyDown(Keys.Up))
                    BordHautTexture -= 5;
                if (clavier.IsKeyDown(Keys.Down))
                    BordHautTexture += 5;
            }

            // Génére un nouveau labyrinthe
            if (triche && clavier.IsKeyDown(Keys.G))
                GenLaby();

            if (Config.modedejeu)
            {
                // Actualisation de l'IA
                if (chasseur.changement_de_case)
                    if (distance_joueur_chasseur < 4.6 || !Config.laby_algo)
                        ia.Update(joueur.position_case_actuelle, triche);
                    else
                        ia.UpdateChasseur(chasseur, triche, joueur);
                // Chasseur
                if (distance_joueur_chasseur > 1.5)
                {
                    disapply_mortsubite = false;
                    if (chasseur.animation != "walk7")
                    {
                        chasseur.skeleton_base.setAnimation("walk7");
                        chasseur.animation = "walk7";
                    }

                    if ((triche && !clavier.IsKeyDown(Keys.P)) || !triche)
                        chasseur.Update((float)gameTime.ElapsedGameTime.TotalSeconds, triche);
                }
                else
                {
                    vitesseIA(1.52196);
                    disapply_mortsubite = true;
                    if (chasseur.animation != "attack3")
                    {
                        chasseur.skeleton_base.setAnimation("attack3");
                        chasseur.animation = "attack3";
                    }
                }
            }
            else
            {

                /** Scientifique **/
                // Si le scientifique est bloqué
                if (!ia.ScienNonBloque(laby.sortie_position, scientifique.position_case_actuelle, joueur))
                {
                    ia.UpdateBis(scientifique, joueur);
                    if (scientifique.position_case_actuelle == ia.case_eloigne && distance_joueur_scientifique >= 1 && scientifique.estpasmort)
                    {
                        disapply_mortsubite = true;
                        vitesseIA(1.52196);
                        if (scientifique.animation != "freaked_out")
                        {
                            scientifique.skeleton_base.setAnimation("freaked_out");
                            scientifique.animation = "freaked_out";
                        }

                    }
                    else if (scientifique.position_case_actuelle == ia.case_eloigne && distance_joueur_scientifique < 1)
                    {
                        disapply_mortsubite = true;
                        vitesseIA(1.52196);
                        if (scientifique.animation != "invasion_ledgefall")
                        {
                            scientifique.skeleton_base.setAnimation("invasion_ledgefall");
                            scientifique.estpasmort = false;
                            scientifique.animation = "invasion_ledgefall";
                        }
                    }
                    else
                    {

                        if (distance_joueur_scientifique < 1)
                        {
                            disapply_mortsubite = true;
                            vitesseIA(1.52196);
                            if (scientifique.animation != "invasion_ledgefall")
                            {
                                scientifique.skeleton_base.setAnimation("invasion_ledgefall");
                                scientifique.estpasmort = false;
                                scientifique.animation = "invasion_ledgefall";
                            }
                        }
                        else if ((triche && !clavier.IsKeyDown(Keys.P) || !triche) && scientifique.estpasmort)
                        {
                            scientifique.Update((float)gameTime.ElapsedGameTime.TotalSeconds, triche);
                        }
                        if (scientifique.position_case_actuelle != joueur.position_case_actuelle && scientifique.animation != "walk_normal" && scientifique.estpasmort)
                        {
                            disapply_mortsubite = false;
                            scientifique.skeleton_base.setAnimation("walk_normal");
                            scientifique.animation = "walk_normal";
                        }
                    }
                }
                //Scientifique non bloqué recherche la sortie
                else
                {
                    if (scientifique.changement_de_case)
                        ia.UpdateScientifique2(scientifique, laby.sortie_position);
                    if (distance_joueur_scientifique < 1)
                    {
                        if (scientifique.animation != "invasion_ledgefall")
                        {
                            scientifique.skeleton_base.setAnimation("invasion_ledgefall");
                            scientifique.estpasmort = false;
                            scientifique.animation = "invasion_ledgefall";
                        }
                    }
                    else if (((triche && !clavier.IsKeyDown(Keys.P)) || !triche) || scientifique.estpasmort)
                    {
                        scientifique.Update((float)gameTime.ElapsedGameTime.TotalSeconds, triche);
                    }
                    if (scientifique.position_case_actuelle != joueur.position_case_actuelle && scientifique.animation != "walk_normal" && scientifique.estpasmort)
                    {
                        scientifique.skeleton_base.setAnimation("walk_normal");
                        scientifique.animation = "walk_normal";
                    }
                }

            }


            // Joueur
            joueur.Update(graphics.GraphicsDevice.Viewport, gameTime, Content, yaw, pitch, temps, laby, triche);

            // Place le joueur en temps réel sur la carte 2D
            for (int y = 0; y < laby.Size.Y; y++)
            {
                for (int x = 0; x < laby.Size.X; x++)
                {
                    if (laby.Carte[x, y] == 2)
                        laby.Carte[x, y] = 0;
                }
            }

            if (laby.Carte[(int)(joueur.position.X / laby.CellSize.X), (int)(joueur.position.Z / laby.CellSize.Z)] == 0)
                laby.Carte[(int)(joueur.position.X / laby.CellSize.X), (int)(joueur.position.Z / laby.CellSize.Z)] = 2;

            // Place le chasseur en temps réel sur la carte 2D
            for (int y = 0; y < laby.Size.Y; y++)
            {
                for (int x = 0; x < laby.Size.X; x++)
                {
                    if (laby.Carte[x, y] == 4)
                        laby.Carte[x, y] = 0;
                }
            }

            if (Config.modedejeu)
            {
                if (laby.Carte[(int)(chasseur.position.X * chasseur.Scale / laby.CellSize.X), (int)(chasseur.position.Z * chasseur.Scale / laby.CellSize.Z)] == 0)
                    laby.Carte[(int)(chasseur.position.X * chasseur.Scale / laby.CellSize.X), (int)(chasseur.position.Z * chasseur.Scale / laby.CellSize.Z)] = 4;
            }
            else
            {
                if (laby.Carte[(int)(scientifique.position.X * scientifique.Scale / laby.CellSize.X), (int)(scientifique.position.Z * scientifique.Scale / laby.CellSize.Z)] == 0)
                    laby.Carte[(int)(scientifique.position.X * scientifique.Scale / laby.CellSize.X), (int)(scientifique.position.Z * scientifique.Scale / laby.CellSize.Z)] = 4;

            }

            #region Bonus
            /*** Bonus ***/
            // Si le joueur est sur un bonus
            if (laby.Carte[joueur.position_case_actuelle.X, joueur.position_case_actuelle.Y] == 5 && !joueur.bonus_actif)
            {
                // On localise ce bonus pour connaître ses cordonnées dans l'espace
                for (int i = 0; i < laby.liste_bonus.Count; i++)
                {
                    if (laby.liste_bonus[i].position_case_bonus == joueur.position_case_actuelle)
                    {
                        // On regarde si le joueur touche le bonus
                        distance_bonus_joueur = Math.Sqrt(Math.Pow(laby.liste_bonus[i].position_bonus.X - joueur.position.X, 2) + Math.Pow(laby.liste_bonus[i].position_bonus.Z - joueur.position.Z, 2) + Math.Pow(laby.liste_bonus[i].position_bonus.Y - (joueur.position.Y * 0.25), 2));
                        if (distance_bonus_joueur <= 0.5)
                        {
                            SoundBank.PlayCue("bonus");
                            SupprimerBonus(laby.liste_bonus[i]);
                            joueur.bonus_actif = true;
                        }
                    }
                }
            }

            // On ajoute un bonus si le joueur appui sur B
            if (clavier.IsKeyDown(Keys.B))
                NouveauBonus();

            // On supprimer tous les bonus si on appui sur Suppr
            if (clavier.IsKeyDown(Keys.Delete))
                SupprimerTousBonus();

            // Variables pour les bonus
            if (joueur.gel && geler_compteur < chrono)
                joueur.gel = false;

            if (joueur.vitesse != joueur.vitesse_initiale && changervitesse_compteur < chrono)
                joueur.vitesse = joueur.vitesse_initiale;

            if (joueur.inversertouches && inversertouche_compteur < chrono)
                joueur.inversertouches = false;

            if (inversercamera && inversercamera_compteur < chrono)
                inversercamera = false;

            if (changercamera && changercamera_compteur < chrono)
                changercamera = false;

            if (joueur.fil_ariane && fil_ariane_compteur < chrono)
            {
                joueur.fil_ariane = false;
                materials = materials_save;
            }
            if (affichercarte2D && affichercarte2D_compteur < chrono)
                affichercarte2D = false;

            if (cecite && cecite_compteur < chrono)
                cecite = false;
            else if (cecite_transparence_tmp < cecite_transparence)
                cecite_transparence_tmp++;

            if (boussole_sortie && boussole_sortie_compteur < chrono)
                boussole_sortie = false;

            if (compteur_message_bonus < 0)
                inverser_position = false;
            if (compteur_message_bonus < 0)
                teleportation_aleatoire = false;

            if (!joueur.gel && !joueur.inversertouches && !joueur.fil_ariane && (joueur.vitesse == joueur.vitesse_initiale) && !inversercamera
                && !changercamera && !affichercarte2D && !cecite && !boussole_sortie && !inverser_position && !teleportation_aleatoire)
            {
                joueur.bonus_actif = false;
            }
            #endregion

            // Chronomètre
            if (laby.Carte[(int)(joueur.position.X / laby.CellSize.X), (int)(joueur.position.Z / laby.CellSize.Z)] != 3)
                chrono += (float)gameTime.ElapsedGameTime.TotalSeconds;
            else if (laby.Carte[(int)(joueur.position.X / laby.CellSize.X), (int)(joueur.position.Z / laby.CellSize.Z)] == 3)
                chrono_meilleur_score = chrono;


            // Audio
            Audio.Update();

            compteur_son_ambiance_aleatoire -= temps;

            // Son ambiance aléatoire
            if (compteur_son_ambiance_aleatoire < 0)
            {
                if (son_ambiance_aleatoire.IsStopped)
                {
                    son_ambiance_aleatoire.Dispose();
                    son_ambiance_aleatoire = SoundBank.GetCue("son_ambiance_aleatoire");
                }
                if (son_ambiance_aleatoire.IsPrepared)
                    son_ambiance_aleatoire.Play();

                compteur_son_ambiance_aleatoire = rand.Next(10, 30);
            }

            // Son mort subite
            if (Config.active_mort_subite && ((chrono > Config.min_mort_subite * 60 + Config.sec_mort_subite) && !son_mort_subite_unique))
            {
                son_mort_subite.Play();
                son_mort_subite_unique = true;
            }

            // Volume du jeu
            if (triche)
            {
                Audio.GetCategory("Musiques").SetVolume(aMAZEing_Escape.Properties.Settings.Default.volume_musiques);
                Audio.GetCategory("Sons").SetVolume(aMAZEing_Escape.Properties.Settings.Default.volume_sons);

                // Réglage du volume des musiques
                if (clavier.IsKeyDown(Keys.PageDown) && aMAZEing_Escape.Properties.Settings.Default.volume_musiques > 0.1f)
                    aMAZEing_Escape.Properties.Settings.Default.volume_musiques -= 0.01f;
                else if (clavier.IsKeyDown(Keys.PageUp) && aMAZEing_Escape.Properties.Settings.Default.volume_musiques < 0.9f)
                    aMAZEing_Escape.Properties.Settings.Default.volume_musiques += 0.01f;

                // Réglage du volume des sons
                if (clavier.IsKeyDown(Keys.End) && aMAZEing_Escape.Properties.Settings.Default.volume_sons > 0.1f)
                    aMAZEing_Escape.Properties.Settings.Default.volume_sons -= 0.01f;
                else if (clavier.IsKeyDown(Keys.Home) && aMAZEing_Escape.Properties.Settings.Default.volume_sons < 0.9f)
                    aMAZEing_Escape.Properties.Settings.Default.volume_sons += 0.01f;
            }

            if (Config.modedejeu)
            {
                if (musique_ambiance.IsPaused)
                    musique_ambiance.Resume();
                if (musique_poursuite.IsPaused)
                    musique_poursuite.Resume();

                if (joueur.matrice_cout[(int)(chasseur.position.X * chasseur.Scale / laby.CellSize.X), (int)(chasseur.position.Z * chasseur.Scale / laby.CellSize.Z)] < ray(laby.Size) && poursuite == false && !musique_poursuite.IsPlaying)
                {
                    musique_poursuite.Play();
                    poursuite = true;
                }
                else if (joueur.matrice_cout[(int)(chasseur.position.X * chasseur.Scale / laby.CellSize.X), (int)(chasseur.position.Z * chasseur.Scale / laby.CellSize.Z)] > ray(laby.Size) && musique_poursuite.IsPlaying)
                {
                    // Stopper la musique
                    musique_poursuite.Stop(AudioStopOptions.AsAuthored);
                    musique_poursuite.Dispose();
                    musique_poursuite = SoundBank.GetCue("poursuite");
                    poursuite = false;
                }
            }

            // Changement de la caméra en fonction de la souris
            Vector2 position_souris = new Vector2(souris.X - souris_prec.X, souris.Y - souris_prec.Y);

            // pitch = rotation autour de l'axe des X | yaw = rotation autour de l'axe des Y

            if (inversercamera)
            {
                pitch += (position_souris.X * (aMAZEing_Escape.Properties.Settings.Default.sensibilite_souris) * temps) / 50;
                yaw -= (position_souris.Y * (aMAZEing_Escape.Properties.Settings.Default.sensibilite_souris) * temps) / 50;
            }
            else
            {
                if (changercamera)
                {
                    yaw -= (position_souris.X * (aMAZEing_Escape.Properties.Settings.Default.sensibilite_souris) * temps) / 50;
                    pitch += (position_souris.Y * (aMAZEing_Escape.Properties.Settings.Default.sensibilite_souris) * temps) / 50;
                }
                else
                {
                    pitch -= (position_souris.X * (aMAZEing_Escape.Properties.Settings.Default.sensibilite_souris) * temps) / 50;
                    yaw += (position_souris.Y * (aMAZEing_Escape.Properties.Settings.Default.sensibilite_souris) * temps) / 50;
                }
            }

            yaw = MathHelper.Clamp(yaw, MathHelper.ToRadians(-89.9f), MathHelper.ToRadians(89.9f));

            Mouse.SetPosition(graphics.GraphicsDevice.Viewport.Width / 2, graphics.GraphicsDevice.Viewport.Height / 2);

            if (Config.modedejeu)
            {
                // Animations du chasseur
                chasseur.skeleton_base.update(gameTime);
            }

            else
            {
                // Animation du scientifique
                scientifique.skeleton_base.update(gameTime);
            }

            Vue = Matrix.CreateLookAt(joueur.position, joueur.cible, Vector3.Up);
            clavier_prec = Keyboard.GetState();

            // On rejoue les sons stoppé
            if (musique_ambiance.IsPaused)
                musique_ambiance.Resume();
            if (Config.modedejeu && musique_poursuite.IsPaused)
                musique_poursuite.Resume();


            if (Config.active_mort_subite && (chrono >= mortsubite && !disapply_mortsubite))
                if (Config.index_difficulte == 2) // dificile
                    facteur = facteur + 0.01;
                else if (Config.index_difficulte == 1) // moyen
                    facteur = facteur + 0.005;
                else // facile
                    facteur = facteur + 0.001;

            vitesseIA(facteur);

            base.Update(gameTime);
        }
        #endregion

        #region Draw
        public override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.White);

            // Affichage des murs
            MaterialRenderer.Draw(gameTime, materials, Matrix.Identity, Vue, Projection);

            // Affichage des bonus
            foreach (Bonus bonus in laby.liste_bonus)
                bonus.Draw(Projection, joueur.position, joueur.cible);

            if (Config.modedejeu)
                foreach (Pieges piege in laby.liste_pieges)
                     piege.Draw(graphics, Projection, joueur.position, chasseur.position, chasseur.Scale, joueur.cible, SoundBank, joueur.fil_ariane, carte_joueur_sortie);
            else
                foreach (Pieges piege in laby.liste_pieges)
                    piege.Draw(graphics, Projection, joueur.position, scientifique.position, scientifique.Scale, joueur.cible, SoundBank, joueur.fil_ariane, carte_joueur_scientifique);


            int count = 0;
            if (Config.modedejeu)
            {
                foreach (ModelMesh mesh in chasseur.Model.Meshes)
                {
                    foreach (Effect effect in mesh.Effects)
                    {
                        effect.Parameters["World"].SetValue(chasseur.LocalWorld);
                        effect.Parameters["View"].SetValue(Vue);
                        effect.Parameters["Projection"].SetValue(Projection);
                        effect.Parameters["cameraPosition"].SetValue(chasseur.position);
                        effect.Parameters["scaleBias"].SetValue(new Vector2(0.03f, -0.018f));

                        effect.Parameters["ambientLightColor"].SetValue(Color.Gray.ToVector4());
                        effect.Parameters["diffuseLightColor"].SetValue(Color.Gray.ToVector4());
                        effect.Parameters["specularLightColor"].SetValue(Color.White.ToVector4());

                        effect.Parameters["Bones"].SetValue(chasseur.skeleton_base.getSkinTransforms(count));


                        effect.CurrentTechnique = effect.Techniques["NormalMapping"];

                    }
                    mesh.Draw();
                    ++count;
                }
            }
            else
            {
                foreach (ModelMesh mesh in scientifique.Model.Meshes)
                {
                    foreach (Effect effect in mesh.Effects)
                    {
                        effect.Parameters["World"].SetValue(scientifique.LocalWorld);
                        effect.Parameters["View"].SetValue(Vue);
                        effect.Parameters["Projection"].SetValue(Projection);
                        effect.Parameters["cameraPosition"].SetValue(scientifique.position);
                        effect.Parameters["scaleBias"].SetValue(new Vector2(0.03f, -0.018f));

                        effect.Parameters["ambientLightColor"].SetValue(Color.Gray.ToVector4());
                        effect.Parameters["diffuseLightColor"].SetValue(Color.Gray.ToVector4());
                        effect.Parameters["specularLightColor"].SetValue(Color.White.ToVector4());

                        effect.Parameters["Bones"].SetValue(scientifique.skeleton_base.getSkinTransforms(count));


                        effect.CurrentTechnique = effect.Techniques["NormalMapping"];

                    }
                    mesh.Draw();
                    ++count;
                }
            }

            // Brouillard
            graphics.GraphicsDevice.RenderState.FogColor = Color.Black;        
            graphics.GraphicsDevice.RenderState.FogStart = 0.0f;
            graphics.GraphicsDevice.RenderState.FogEnd = 1.0f;
            graphics.GraphicsDevice.RenderState.FogTableMode = FogMode.Exponent;
            graphics.GraphicsDevice.RenderState.FogVertexMode = FogMode.Exponent;
            graphics.GraphicsDevice.RenderState.FogDensity = aMAZEing_Escape.Properties.Settings.Default.densite_brouillard;

            // Changement de la densité du brouillard
            if (triche)
            {
                if (clavier.IsKeyDown(Keys.F11) && aMAZEing_Escape.Properties.Settings.Default.densite_brouillard > 0.05f)
                    aMAZEing_Escape.Properties.Settings.Default.densite_brouillard -= 0.01f;
                else if (clavier.IsKeyDown(Keys.F12) && aMAZEing_Escape.Properties.Settings.Default.densite_brouillard < 0.95f)
                    aMAZEing_Escape.Properties.Settings.Default.densite_brouillard += 0.01f;
            }

            if (triche && clavier.IsKeyDown(Keys.R))
                graphics.GraphicsDevice.RenderState.FogEnable = false;
            else
                graphics.GraphicsDevice.RenderState.FogEnable = true;

            spriteBatch.Begin();


            if (!transition_debut_finie)
            {
                spriteBatch.Draw(noir, new Rectangle(0, 0, graphics.GraphicsDevice.Viewport.Width, graphics.GraphicsDevice.Viewport.Height), new Color(Color.White, compteur_transition_debut * 51));

                    spriteBatch.DrawString(perso, transition_debut_messages[0], new Vector2((graphics.GraphicsDevice.Viewport.Width / 2) - perso.MeasureString(transition_debut_messages[0]).X / 2 * (float)(1.3 * graphics.GraphicsDevice.Viewport.Width / 800) * 1.5f, (graphics.GraphicsDevice.Viewport.Height / 2) - perso.MeasureString(transition_debut_messages[0]).Y / 2 * (float)(1.3 * graphics.GraphicsDevice.Viewport.Width / 800) * 1.5f - 100), Color.White, 0f, Vector2.Zero, (float)(1.3 * graphics.GraphicsDevice.Viewport.Width / 800) * 1.5f, SpriteEffects.None, 0);
                if(compteur_transition_debut < 0.5f)
                    spriteBatch.DrawString(perso, transition_debut_messages[1], new Vector2((graphics.GraphicsDevice.Viewport.Width / 2) - perso.MeasureString(transition_debut_messages[1]).X / 2 * (float)(1.3 * graphics.GraphicsDevice.Viewport.Width / 800) * 1.5f, (graphics.GraphicsDevice.Viewport.Height / 2) - perso.MeasureString(transition_debut_messages[1]).Y / 2 * (float)(1.3 * graphics.GraphicsDevice.Viewport.Width / 800) * 1.5f + 50), Color.White, 0f, Vector2.Zero, (float)(1.3 * graphics.GraphicsDevice.Viewport.Width / 800) * 1.5f, SpriteEffects.None, 0);
            }

            if (cecite)
                spriteBatch.Draw(noir, new Rectangle(0, 0, graphics.GraphicsDevice.Viewport.Width, graphics.GraphicsDevice.Viewport.Height), new Color(255, 255, 255, cecite_transparence_tmp));


            #region Affichage de la carte 2D en chiffre
            // On affiche la carte sous forme de chiffre
            if (triche && clavier.IsKeyDown(Keys.Tab))
            {
                spriteBatch.DrawString(Kootenay, "Carte", new Vector2(BordGauche + (laby.Size.X * EspaceTexte) / 2 - EspaceTexte, BordHaut - EspaceTexte), Color.Red);

                for (int x = 0; x < laby.Size.X; x++)
                {
                    for (int y = 0; y < laby.Size.Y; y++)
                    {
                        if (triche && clavier.IsKeyDown(Keys.F1))
                        {
                            // Cases trop proches de la sortie
                            double distance_case_sortie = Math.Sqrt(Math.Pow(laby.sortie_position.X - x, 2) + Math.Pow(laby.sortie_position.Y - y, 2));
                            if (distance_case_sortie <= (laby.Size.X + laby.Size.Y) / 4)
                            {
                                // Chemin
                                if (laby.Carte[x, y] == 0)
                                    spriteBatch.DrawString(Kootenay, "0", new Vector2(BordGauche + EspaceTexte * x, BordHaut + EspaceTexte * y), Color.Green);
                                // Mur
                                else if (laby.Carte[x, y] == 1)
                                    spriteBatch.DrawString(Kootenay, "1", new Vector2(BordGauche + EspaceTexte * x, BordHaut + EspaceTexte * y), Color.Green);
                                // Joueur
                                else if (laby.Carte[x, y] == 2)
                                    spriteBatch.DrawString(Kootenay, "J", new Vector2(BordGauche + EspaceTexte * x, BordHaut + EspaceTexte * y), Color.Green);
                                // Sortie
                                else if (laby.Carte[x, y] == 3)
                                    spriteBatch.DrawString(Kootenay, "S", new Vector2(BordGauche + EspaceTexte * x, BordHaut + EspaceTexte * y), Color.Green);
                                // Chasseur
                                else if (laby.Carte[x, y] == 4)
                                    spriteBatch.DrawString(Kootenay, "C", new Vector2(BordGauche + EspaceTexte * x, BordHaut + EspaceTexte * y), Color.Green);
                                // Bonus
                                else if (laby.Carte[x, y] == 5)
                                    spriteBatch.DrawString(Kootenay, "B", new Vector2(BordGauche + EspaceTexte * x, BordHaut + EspaceTexte * y), Color.Green);
                            }
                            else
                            {
                                // Chemin
                                if (laby.Carte[x, y] == 0)
                                    spriteBatch.DrawString(Kootenay, "0", new Vector2(BordGauche + EspaceTexte * x, BordHaut + EspaceTexte * y), Color.White);
                                // Mur
                                else if (laby.Carte[x, y] == 1)
                                    spriteBatch.DrawString(Kootenay, "1", new Vector2(BordGauche + EspaceTexte * x, BordHaut + EspaceTexte * y), Color.Black);
                                // Joueur
                                else if (laby.Carte[x, y] == 2)
                                    spriteBatch.DrawString(Kootenay, "J", new Vector2(BordGauche + EspaceTexte * x, BordHaut + EspaceTexte * y), Color.Blue);
                                // Sortie
                                else if (laby.Carte[x, y] == 3)
                                    spriteBatch.DrawString(Kootenay, "S", new Vector2(BordGauche + EspaceTexte * x, BordHaut + EspaceTexte * y), Color.Green);
                                // Chasseur
                                else if (laby.Carte[x, y] == 4)
                                    spriteBatch.DrawString(Kootenay, "C", new Vector2(BordGauche + EspaceTexte * x, BordHaut + EspaceTexte * y), Color.Red);
                                // Bonus
                                else if (laby.Carte[x, y] == 5)
                                    spriteBatch.DrawString(Kootenay, "B", new Vector2(BordGauche + EspaceTexte * x, BordHaut + EspaceTexte * y), Color.Yellow);
                            }
                        }
                        // Cases trop proches du chasseur
                        else if (triche && clavier.IsKeyDown(Keys.F2))
                        {
                            double distance_case_chasseur = Math.Sqrt(Math.Pow((int)(chasseur.position.X / laby.CellSize.X) - x, 2) + Math.Pow((int)(chasseur.position.Z / laby.CellSize.Z) - y, 2));
                            // Cases proches du chasseur                      
                            if (distance_case_chasseur <= ((laby.Size.X + laby.Size.Y) / 2) / ((laby.CellSize.X + laby.CellSize.Y) / 2))
                            {
                                // Chemin
                                if (laby.Carte[x, y] == 0)
                                    spriteBatch.DrawString(Kootenay, "0", new Vector2(BordGauche + EspaceTexte * x, BordHaut + EspaceTexte * y), Color.Red);
                                // Mur
                                else if (laby.Carte[x, y] == 1)
                                    spriteBatch.DrawString(Kootenay, "1", new Vector2(BordGauche + EspaceTexte * x, BordHaut + EspaceTexte * y), Color.Red);
                                // Joueur
                                else if (laby.Carte[x, y] == 2)
                                    spriteBatch.DrawString(Kootenay, "J", new Vector2(BordGauche + EspaceTexte * x, BordHaut + EspaceTexte * y), Color.Red);
                                // Sortie
                                else if (laby.Carte[x, y] == 3)
                                    spriteBatch.DrawString(Kootenay, "S", new Vector2(BordGauche + EspaceTexte * x, BordHaut + EspaceTexte * y), Color.Red);
                                // Chasseur
                                else if (laby.Carte[x, y] == 4)
                                    spriteBatch.DrawString(Kootenay, "C", new Vector2(BordGauche + EspaceTexte * x, BordHaut + EspaceTexte * y), Color.Red);
                                // Bonus
                                else if (laby.Carte[x, y] == 5)
                                    spriteBatch.DrawString(Kootenay, "B", new Vector2(BordGauche + EspaceTexte * x, BordHaut + EspaceTexte * y), Color.Red);
                            }
                            else
                            {
                                // Chemin
                                if (laby.Carte[x, y] == 0)
                                    spriteBatch.DrawString(Kootenay, "0", new Vector2(BordGauche + EspaceTexte * x, BordHaut + EspaceTexte * y), Color.White);
                                // Mur
                                else if (laby.Carte[x, y] == 1)
                                    spriteBatch.DrawString(Kootenay, "1", new Vector2(BordGauche + EspaceTexte * x, BordHaut + EspaceTexte * y), Color.Black);
                                // Joueur
                                else if (laby.Carte[x, y] == 2)
                                    spriteBatch.DrawString(Kootenay, "J", new Vector2(BordGauche + EspaceTexte * x, BordHaut + EspaceTexte * y), Color.Blue);
                                // Sortie
                                else if (laby.Carte[x, y] == 3)
                                    spriteBatch.DrawString(Kootenay, "S", new Vector2(BordGauche + EspaceTexte * x, BordHaut + EspaceTexte * y), Color.Green);
                                // Chasseur
                                else if (laby.Carte[x, y] == 4)
                                    spriteBatch.DrawString(Kootenay, "C", new Vector2(BordGauche + EspaceTexte * x, BordHaut + EspaceTexte * y), Color.Red);
                                // Bonus
                                else if (laby.Carte[x, y] == 5)
                                    spriteBatch.DrawString(Kootenay, "B", new Vector2(BordGauche + EspaceTexte * x, BordHaut + EspaceTexte * y), Color.Yellow);
                            }
                        }
                        else
                        {
                            // Chemin
                            if (laby.Carte[x, y] == 0)
                                spriteBatch.DrawString(Kootenay, "0", new Vector2(BordGauche + EspaceTexte * x, BordHaut + EspaceTexte * y), Color.White);
                            // Mur
                            else if (laby.Carte[x, y] == 1)
                                spriteBatch.DrawString(Kootenay, "1", new Vector2(BordGauche + EspaceTexte * x, BordHaut + EspaceTexte * y), Color.Black);
                            // Joueur
                            else if (laby.Carte[x, y] == 2)
                                spriteBatch.DrawString(Kootenay, "J", new Vector2(BordGauche + EspaceTexte * x, BordHaut + EspaceTexte * y), Color.Blue);
                            // Sortie
                            else if (laby.Carte[x, y] == 3)
                                spriteBatch.DrawString(Kootenay, "S", new Vector2(BordGauche + EspaceTexte * x, BordHaut + EspaceTexte * y), Color.Green);
                            // Chasseur
                            else if (laby.Carte[x, y] == 4)
                                spriteBatch.DrawString(Kootenay, "C", new Vector2(BordGauche + EspaceTexte * x, BordHaut + EspaceTexte * y), Color.Red);
                            // Bonus
                            else if (laby.Carte[x, y] == 5)
                                spriteBatch.DrawString(Kootenay, "B", new Vector2(BordGauche + EspaceTexte * x, BordHaut + EspaceTexte * y), Color.Yellow);
                        }
                    }
                }
            }
            #endregion


            if (triche && clavier.IsKeyDown(Keys.M))
            {
                for (int x = 0; x < laby.Size.X; x++)
                {
                    for (int y = 0; y < laby.Size.Y; y++)
                    {
                        spriteBatch.DrawString(Kootenay, joueur.matrice_cout[x, y].ToString(), new Vector2(BordGauche + 40 * x, BordHaut + 40 * y), Color.White);
                    }
                }
            }

            #region Affichage de la carte 2D texturée
            if ((triche && clavier.IsKeyDown(Keys.E)) || affichercarte2D)
            {
                bool affichage_bonus = false;
                bool affichage_pieges = false;
                //spriteBatch.DrawString(Kootenay, "Carte", new Vector2(BordGaucheTexture + (laby.Size.X * EspaceTexture) / 2 - EspaceTexte, BordHautTexture - EspaceTexture), Color.Red);
                for (int x = 0; x < laby.Size.X; x++)
                {
                    for (int y = 0; y < laby.Size.Y; y++)
                    {
                        // Cases trop proches de la sortie
                        if (triche && clavier.IsKeyDown(Keys.F1))
                        {
                            double distance_case_sortie = Math.Sqrt(Math.Pow(laby.sortie_position.X - x, 2) + Math.Pow(laby.sortie_position.Y - y, 2));
                            if (distance_case_sortie <= (laby.Size.X + laby.Size.Y) / 4)
                                carte2D_couleur_texture = Color.Green;
                            else
                                carte2D_couleur_texture = Color.White;
                        }
                        // Cases trop proches des bonus
                        else if (triche && clavier.IsKeyDown(Keys.F2))
                        {
                            affichage_bonus = true;
                        }
                        // Cases proches du joueur   
                        else if (triche && clavier.IsKeyDown(Keys.F3))
                        {
                            if (joueur.matrice_cout[x, y] < ray(laby.Size))
                                carte2D_couleur_texture = Color.Red;
                            else
                                carte2D_couleur_texture = Color.White;
                        }
                        // Cases proches du chasseur   
                        else if (triche && clavier.IsKeyDown(Keys.F4) && Config.modedejeu)
                        {
                            double distance_case_chasseur = Math.Sqrt(Math.Pow(chasseur.position_case_actuelle.X - x, 2) + Math.Pow(chasseur.position_case_actuelle.Y - y, 2));
                            if (distance_case_chasseur <= 5)
                                carte2D_couleur_texture = Color.Red;
                            else
                                carte2D_couleur_texture = Color.White;
                        }
                        // Cases proches du scientifique   
                        else if (triche && clavier.IsKeyDown(Keys.F4) && !Config.modedejeu)
                        {
                            // Cases proches du scientifique
                        }
                        // Cases menant à la sortie
                        else if (triche && clavier.IsKeyDown(Keys.F5))
                        {
                            if (Config.modedejeu)
                            {
                                if(joueur.changement_de_case)
                                    GenCarteJoueurSortie();
                                if (carte_joueur_sortie[x, y])
                                    carte2D_couleur_texture = Color.Green;
                                else
                                    carte2D_couleur_texture = Color.White;
                            }
                            else
                            {
                                if(scientifique.changement_de_case || joueur.changement_de_case)
                                    GenCarteJoueurScientifique();
                                if (carte_joueur_scientifique[x, y])
                                    carte2D_couleur_texture = Color.Green;
                                else
                                    carte2D_couleur_texture = Color.White;
                            }
                        }
                        // Cases trop proches des pieges
                        else if (triche && clavier.IsKeyDown(Keys.F6))
                        {
                            affichage_pieges = true;
                        }
                        // Cases trop proches du joueur
                        else if (triche && clavier.IsKeyDown(Keys.F7))
                        {
                            double distance_joueur_chasseur = (int)Math.Sqrt(Math.Pow(laby.joueur_position_initiale_case.X - x, 2) + Math.Pow(laby.joueur_position_initiale_case.Y - y, 2));
                            if (distance_joueur_chasseur <= (laby.Size.X + laby.Size.Y) / 6)
                                carte2D_couleur_texture = Color.Red;
                            else
                                carte2D_couleur_texture = Color.White;
                        }
                        else
                            carte2D_couleur_texture = Color.White;


                        if (affichage_bonus)
                        {
                            if (laby.carte_espace_bonus[x, y])
                                carte2D_couleur_texture = Color.Yellow;
                            else
                                carte2D_couleur_texture = Color.White;
                        }

                        if (affichage_pieges)
                        {
                            if (laby.carte_espace_pieges[x, y])
                                carte2D_couleur_texture = Color.Red;
                            else
                                carte2D_couleur_texture = Color.White;
                        }

                        // Chemin
                        if (laby.Carte[x, y] == 0)
                            spriteBatch.Draw(carte2D_chemin, new Vector2(BordGaucheTexture + (EspaceTexture * echelle) * x, BordHautTexture + (EspaceTexture * echelle) * y), null, new Color(carte2D_couleur_texture, Config.transparence_carte), 0, new Vector2(0), new Vector2(echelle), SpriteEffects.None, 0);
                        // Mur
                        else if (laby.Carte[x, y] == 1)
                            spriteBatch.Draw(carte2D_mur, new Vector2(BordGaucheTexture + (EspaceTexture * echelle) * x, BordHautTexture + (EspaceTexture * echelle) * y), null, new Color(carte2D_couleur_texture, Config.transparence_carte), 0, new Vector2(0), new Vector2(echelle), SpriteEffects.None, 0);
                        // Joueur
                        else if (laby.Carte[x, y] == 2)
                            spriteBatch.Draw(carte2D_joueur, new Vector2(BordGaucheTexture + (EspaceTexture * echelle) * x, BordHautTexture + (EspaceTexture * echelle) * y), null, new Color(carte2D_couleur_texture, Config.transparence_carte), 0, new Vector2(0), new Vector2(echelle), SpriteEffects.None, 0);
                        // Sortie
                        else if (laby.Carte[x, y] == 3)
                            spriteBatch.Draw(carte2D_sortie, new Vector2(BordGaucheTexture + (EspaceTexture * echelle) * x, BordHautTexture + (EspaceTexture * echelle) * y), null, new Color(carte2D_couleur_texture, Config.transparence_carte), 0, new Vector2(0), new Vector2(echelle), SpriteEffects.None, 0);
                        // Chasseur
                        else if (laby.Carte[x, y] == 4)
                            spriteBatch.Draw(carte2D_chasseur, new Vector2(BordGaucheTexture + (EspaceTexture * echelle) * x, BordHautTexture + (EspaceTexture * echelle) * y), null, new Color(carte2D_couleur_texture, Config.transparence_carte), 0, new Vector2(0), new Vector2(echelle), SpriteEffects.None, 0);
                        // Bonus
                        else if (laby.Carte[x, y] == 5)
                            spriteBatch.Draw(carte2D_bonus, new Vector2(BordGaucheTexture + (EspaceTexture * echelle) * x, BordHautTexture + (EspaceTexture * echelle) * y), null, new Color(carte2D_couleur_texture, Config.transparence_carte), 0, new Vector2(0), new Vector2(echelle), SpriteEffects.None, 0);
                        else if (laby.Carte[x, y] == 6)
                            spriteBatch.Draw(carte2D_piege, new Vector2(BordGaucheTexture + (EspaceTexture * echelle) * x, BordHautTexture + (EspaceTexture * echelle) * y), null, new Color(carte2D_couleur_texture, Config.transparence_carte), 0, new Vector2(0), new Vector2(echelle), SpriteEffects.None, 0);
                    }
                }
            }
            #endregion

            //spriteBatch.DrawString(perso, pass, new Vector2(graphics.GraphicsDevice.Viewport.Width / 2, graphics.GraphicsDevice.Viewport.Height / 2), Color.White);

            // Affichage de quelques infos sur le jeu
            if (clavier.IsKeyDown(Keys.A) && triche)
            {
                if (Config.modedejeu)
                {
                    spriteBatch.DrawString(perso, chasseur.increment.X.ToString(), new Vector2(InfoTexteGauche + 50, InfoTexteHaut + 10), Color.White);
                    spriteBatch.DrawString(perso, chasseur.direction.ToString(), new Vector2(InfoTexteGauche + 50, InfoTexteHaut + 30), Color.White);
                    spriteBatch.DrawString(perso, "Chasseur: " + chasseur.position.ToString(), new Vector2(InfoTexteGauche + 50, InfoTexteHaut + 230), Color.White);
                    spriteBatch.DrawString(perso, "Temporisation: " + chasseur.temporisation, new Vector2(InfoTexteGauche + 50, InfoTexteHaut + 250), Color.White);
                    spriteBatch.DrawString(perso, "Vitesse (chasseur): " + chasseur.vitesse.ToString(), new Vector2(InfoTexteGauche + 50, InfoTexteHaut + 270), Color.White);
                    spriteBatch.DrawString(perso, "Distance du chasseur : " + distance_joueur_chasseur.ToString(), new Vector2(InfoTexteGauche + 50, InfoTexteHaut + 290), Color.White);

                }
                else
                {
                    spriteBatch.DrawString(perso, scientifique.increment.X.ToString(), new Vector2(InfoTexteGauche + 50, InfoTexteHaut + 10), Color.White);
                    spriteBatch.DrawString(perso, scientifique.direction.ToString(), new Vector2(InfoTexteGauche + 50, InfoTexteHaut + 30), Color.White);
                    spriteBatch.DrawString(perso, "Scientifique: " + scientifique.position.ToString(), new Vector2(InfoTexteGauche + 50, InfoTexteHaut + 230), Color.White);
                    spriteBatch.DrawString(perso, "Temporisation: " + scientifique.temporisation, new Vector2(InfoTexteGauche + 50, InfoTexteHaut + 250), Color.White);
                    spriteBatch.DrawString(perso, "Vitesse (scientifique): " + scientifique.vitesse.ToString(), new Vector2(InfoTexteGauche + 50, InfoTexteHaut + 270), Color.White);
                }
                spriteBatch.DrawString(perso, joueur.position.ToString(), new Vector2(InfoTexteGauche + 50, InfoTexteHaut + 50), Color.White);
                spriteBatch.DrawString(perso, "Yaw: " + yaw, new Vector2(InfoTexteGauche + 50, InfoTexteHaut + 70), Color.White);
                spriteBatch.DrawString(perso, "Pitch: " + pitch, new Vector2(InfoTexteGauche + 50, InfoTexteHaut + 90), Color.White);
                spriteBatch.DrawString(perso, "Vitesse: " + joueur.vitesse, new Vector2(InfoTexteGauche + 50, InfoTexteHaut + 110), Color.White);

                spriteBatch.DrawString(perso, "Nombre de bonus: " + laby.nb_bonus.ToString(), new Vector2(InfoTexteGauche + 50, InfoTexteHaut + 130), Color.White);
                spriteBatch.DrawString(perso, "Chrono: " + Config.chrono, new Vector2(InfoTexteGauche + 50, InfoTexteHaut + 200), Color.White);

                if (chrono_meilleur_score > 0)
                    spriteBatch.DrawString(perso, "Meilleur score: " + chrono_finale(chrono_meilleur_score), new Vector2(InfoTexteGauche + 50, InfoTexteHaut + 190), Color.White);
            }

            // Affichage des types de chaque bonus
            if (triche && clavier.IsKeyDown(Keys.OemQuotes))
            {
                int gauche = 50;
                int j = 0;
                for (int i = 0; i < laby.liste_bonus.Count; i++)
                {
                    j++;
                    if (75 + 20 * j >= graphics.GraphicsDevice.Viewport.Height)
                    {
                        gauche += 20;
                        j = 0;
                    }
                    spriteBatch.DrawString(perso, Config.liste_bonus_texte[laby.liste_bonus[i].type] + " => (" + i + ", " + j + ")", new Vector2(gauche, 75 + 20 * j), Color.White);
                }
            }

            // Boussole et chronometre
            if (Config.modedejeu)
            {
                int compaSupolarg = (boussole_support.Width / 2) * graphics.GraphicsDevice.Viewport.Width / 800;
                int compaSupohaut = (boussole_support.Height / 2) * graphics.GraphicsDevice.Viewport.Height / 600;

                int compaFlelar = (boussole_fleche.Width / 2) * graphics.GraphicsDevice.Viewport.Width / 800;
                int compaFlehau = (boussole_fleche.Height / 2) * graphics.GraphicsDevice.Viewport.Height / 600;

                // Affichage de la temporisation
                spriteBatch.Draw(boussole_support, new Rectangle(graphics.GraphicsDevice.Viewport.Width - compaSupolarg, 0, compaSupolarg, compaSupohaut), Color.White);

                if (boussole_sortie)
                    spriteBatch.Draw(boussole_fleche_sortie, new Rectangle(graphics.GraphicsDevice.Viewport.Width - compaSupolarg * 5 / 18, compaSupohaut * 5 / 18, compaFlelar, compaFlehau), null, Color.White, -bonus_boussole(), new Vector2(compaFlelar, 0), SpriteEffects.None, 0);
                else
                    spriteBatch.Draw(boussole_fleche, new Rectangle(graphics.GraphicsDevice.Viewport.Width - compaSupolarg * 5 / 18, compaSupohaut * 5 / 18, compaFlelar, compaFlehau), null, Color.White, -degree(), new Vector2(compaFlelar, 0), SpriteEffects.None, 0);

                // Chronomètre
                spriteBatch.Draw(chronometre_texture, new Rectangle(graphics.GraphicsDevice.Viewport.Width / 2 - (Convert.ToInt32(graphics.GraphicsDevice.Viewport.Width / 2.2f) / 2), 0, Convert.ToInt32(graphics.GraphicsDevice.Viewport.Width / 2.2f), graphics.GraphicsDevice.Viewport.Height / 8), Color.White);
                spriteBatch.DrawString(perso, chronometre(chrono), new Vector2(graphics.GraphicsDevice.Viewport.Width / 2 - Convert.ToInt32(graphics.GraphicsDevice.Viewport.Width / 2.2f) / 4.3f, (graphics.GraphicsDevice.Viewport.Height / 8) / 3), Color.White, 0, new Vector2(0, 0), (float)(1.3 * graphics.GraphicsDevice.Viewport.Width / 800), SpriteEffects.None, 0);
            }

            else
            {
                //variables pour que le radar s'affiche proportionnellement à la taille de l'écran.

                int compaCibllar = (radar_cible.Width / 2) * graphics.GraphicsDevice.Viewport.Width / 800;
                int compaCiblhau = (radar_cible.Height / 2) * graphics.GraphicsDevice.Viewport.Height / 600;

                int compa_alienlar = (radar_support.Width / 2) * graphics.GraphicsDevice.Viewport.Width / 800;
                int compa_alienhau = (radar_support.Height / 2) * graphics.GraphicsDevice.Viewport.Height / 600;


                spriteBatch.Draw(radar_support, new Rectangle(graphics.GraphicsDevice.Viewport.Width - compa_alienlar, 0, compa_alienlar, compa_alienhau), Color.White);
                spriteBatch.Draw(radar_cible, new Rectangle(graphics.GraphicsDevice.Viewport.Width - compa_alienlar * 5 / 10, compa_alienhau * 5 / 10, compaCibllar, compaCiblhau), null, Color.White, -degree_target(), new Vector2(compaCibllar, 0), SpriteEffects.None, 0);
            }

            // Bonus
            if (compteur_total - chrono + 1 <= 0)
            {
                barre_redimension = true;
            }
            // Affichage du message indiquant quel bonus a été pris !
            if (compteur_message_bonus > 0)
            {
                if (joueur.gel)
                    Afficher_Texte("Gel");
                else if (inverser_position)
                    Afficher_Texte("Inversion des positions");
                else if (inversercamera)
                    Afficher_Texte("Camera inversee");
                else if (changercamera)
                    Afficher_Texte("Camera changee");
                else if (cecite)
                    Afficher_Texte("Obscurite");
                else if (boussole_sortie)
                    Afficher_Texte("Boussole sortie");
                else if (affichercarte2D)
                    Afficher_Texte("Carte");
                else if (joueur.fil_ariane)
                    Afficher_Texte("Fil d'Ariane");
                else if (joueur.inversertouches)
                    Afficher_Texte("Touches changees");
                else if (joueur.vitesse > joueur.vitesse_initiale)
                    Afficher_Texte("Sprint");
                else if (joueur.vitesse < joueur.vitesse_initiale)
                    Afficher_Texte("Lenteur");
                else if (teleportation_aleatoire)
                    Afficher_Texte("Teleportation");

                compteur_message_bonus -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            // Affichage d'un message lors de l'activation/la désactivation des triches
            if (compteur_message_triche > 0)
            {
                if (triche)
                    Afficher_Texte("Triche activee !");
                else if (!triche)
                    Afficher_Texte("Triche desactivee !");

                compteur_message_triche -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            }



            CompteurBonus((int)(compteur_total - chrono + 1), spriteBatch);

            ia.Draw(clavier, spriteBatch, Kootenay, BordGauche, BordHaut, triche);

            if (Config.modedejeu)
                chasseur.DrawString(clavier, spriteBatch, Kootenay, BordGauche, EspaceTexte, BordHaut);

            spriteBatch.End();

            clavier_prec = clavier;

            base.Draw(gameTime);
        }

        #endregion

        #region Méthodes pour les Bonus
        /** Les bonus **/

        // Echange de place adversaire <=> joueur
        private void InverserPosition()
        {
            Vector3 temp = joueur.position;
            if (Config.modedejeu)
            {
                joueur.position = new Vector3(chasseur.position.X * chasseur.Scale, laby.CellSize.Y / 2, chasseur.position.Z * chasseur.Scale);
                chasseur.position = new Vector3(temp.X / chasseur.Scale, 0, temp.Z / chasseur.Scale);
                laby.Carte[(int)((chasseur.position.X * chasseur.Scale) / laby.CellSize.X), (int)((chasseur.position.Z * chasseur.Scale) / laby.CellSize.Z)] = 4;
                ia.UpdateChasseur(chasseur, triche, joueur);
            }
            else
            {
                joueur.position = new Vector3(scientifique.position.X * scientifique.Scale, laby.CellSize.Y / 2, scientifique.position.Z * scientifique.Scale);
                scientifique.position = new Vector3(temp.X / scientifique.Scale, 0, temp.Z / scientifique.Scale);
                laby.Carte[(int)((scientifique.position.X * scientifique.Scale) / laby.CellSize.X), (int)((scientifique.position.Z * scientifique.Scale) / laby.CellSize.Z)] = 4;
                ia.UpdateScientifique2(scientifique, laby.sortie_position);
            }
        }

        private void TeleportationAleatoire()
        {
            rand = new Random();
            Point position_aleatoire = new Point(rand.Next(laby.Size.X), rand.Next(laby.Size.Y));
            while (laby.Carte[position_aleatoire.X, position_aleatoire.Y] != 0)
            {
                position_aleatoire.X = rand.Next(laby.Size.X);
                position_aleatoire.Y = rand.Next(laby.Size.Y);
            }
            joueur.position.X = position_aleatoire.X * laby.CellSize.X + laby.CellSize.X / 2;
            joueur.position.Z = position_aleatoire.Y * laby.CellSize.Z + laby.CellSize.Z / 2;
        }

        private void SupprimerBonus(Bonus bonus)
        {
            rand = new Random();
            // Effet du bonus
            switch (bonus.type)
            {
                // Lenteur
                case 0:
                    joueur.vitesse = joueur.vitesse - ((float)rand.Next(1, 4) / 4 * joueur.vitesse);
                    int duree1 = rand.Next(5, 15);
                    changervitesse_compteur = chrono + duree1;
                    compteur_total = chrono + duree1;
                    break;
                // Inversion
                case 1:
                    inverser_position = true;
                    InverserPosition();
                    break;
                // Gel
                case 2:
                    joueur.gel = true;
                    int duree2 = rand.Next(5, 15);
                    geler_compteur = chrono + duree2;
                    compteur_total = chrono + duree2;
                    break;
                // Touches changées
                case 3:
                    joueur.inversertouches = true;
                    bool[] touches = { false, false, false, false };
                    int j = 0;
                    int nb_alea = rand.Next(0, 4);
                    while (j < 4)
                    {
                        if (!touches[nb_alea])
                        {
                            joueur.TouchesFausses[j] = joueur.Touches[nb_alea];
                            touches[nb_alea] = true;
                            j++;
                        }
                        nb_alea = rand.Next(4);
                    }
                    int duree3 = rand.Next(15, 60);
                    inversertouche_compteur = chrono + duree3;
                    compteur_total = chrono + duree3;
                    break;
                // Caméra inversée
                case 4:
                    inversercamera = true;
                    int duree4 = rand.Next(15, 60);
                    inversercamera_compteur = chrono + duree4;
                    compteur_total = chrono + duree4;
                    break;
                // Téléportation aléatoire
                case 5:
                    teleportation_aleatoire = true;
                    TeleportationAleatoire();
                    break;
                // Sprint
                case 6:
                    joueur.vitesse = joueur.vitesse + ((float)rand.Next(1, 4) / 4 * joueur.vitesse);
                    int duree5 = rand.Next(5, 15);
                    changervitesse_compteur = chrono + duree5;
                    compteur_total = chrono + duree5;
                    break;
                // Fil d'Ariane
                case 7:
                    joueur.fil_ariane = true;
                    
                    if (Config.modedejeu)
                    {
                        GenCarteJoueurSortie();
                        materials = materialBuilder.BuildMazeMaterial(laby, graphics.GraphicsDevice, textures, joueur.fil_ariane, carte_joueur_sortie);
                    }
                    else
                    {
                        GenCarteJoueurScientifique();
                        materials = materialBuilder.BuildMazeMaterial(laby, graphics.GraphicsDevice, textures, joueur.fil_ariane, carte_joueur_scientifique);
                    }
                    int duree6 = rand.Next(5, 30);
                    fil_ariane_compteur = chrono + duree6;
                    compteur_total = chrono + duree6;
                    break;
                // Carte 2D
                case 8:
                    affichercarte2D = true;
                    int duree7 = rand.Next(5, 15);
                    affichercarte2D_compteur = chrono + duree7;
                    compteur_total = chrono + duree7;
                    break;
                // Caméra changée 
                case 9:
                    changercamera = true;
                    int duree8 = rand.Next(15, 60);
                    changercamera_compteur = chrono + duree8;
                    compteur_total = chrono + duree8;
                    break;
                // Cecité
                case 10:
                    cecite = true;
                    cecite_transparence = (byte)rand.Next(100, 200);
                    cecite_transparence_tmp = 0;
                    int duree9 = rand.Next(15, 60);
                    cecite_compteur = chrono + duree9;
                    compteur_total = chrono + duree9;
                    break;
                // Boussole dirigée vers la boussole
                case 11:
                    boussole_sortie = true;
                    int duree10 = rand.Next(15, 60);
                    boussole_sortie_compteur = chrono + duree10;
                    compteur_total = chrono + duree10;
                    break;
                default:
                    break;
            }

            compteur_message_bonus = 2;

            laby.liste_bonus.Remove(bonus);
            laby.nb_bonus--;

            // Actualise la carte des apparitions possibles pour les bonus
            for (int x = 0; x < Config.largeur_labyrinthe; x++)
            {
                for (int y = 0; y < Config.hauteur_labyrinthe; y++)
                {
                    if (laby.Carte[x, y] == 0)
                        laby.carte_espace_bonus[x, y] = false;
                }
            }

            // On indique les positions où ne peuvent pas apparaitre d'autres bonus
            for (int x = 0; x < Config.largeur_labyrinthe; x++)
            {
                for (int y = 0; y < Config.hauteur_labyrinthe; y++)
                {
                    foreach (Bonus b in laby.liste_bonus)
                    {
                        double distance_case_bonus = Math.Sqrt(Math.Pow(b.position_case_bonus.X - x, 2) + Math.Pow(b.position_case_bonus.Y - y, 2));
                        if (distance_case_bonus <= ray(laby.Size) / 3)
                        {
                            laby.carte_espace_bonus[x, y] = true;
                        }
                    }
                }
            }

            // On remplace le bonus par un chemin
            laby.Carte[joueur.position_case_actuelle.X, joueur.position_case_actuelle.Y] = 0;

        }

        private void NouveauBonus()
        {
            rand = new Random();
            Point pos_bonus = new Point(rand.Next(laby.Size.X), rand.Next(laby.Size.Y));
            if (laby.Carte[pos_bonus.X, pos_bonus.Y] == 0)
            {
                if (!laby.carte_espace_bonus[pos_bonus.X, pos_bonus.Y])
                {
                    laby.liste_bonus.Add(new Bonus(laby, Content, new Vector3(pos_bonus.X * laby.CellSize.X + laby.CellSize.X / 2, laby.CellSize.Y / 8, pos_bonus.Y * laby.CellSize.X + laby.CellSize.Z / 2), liste_bonus_actifs));
                    laby.Carte[pos_bonus.X, pos_bonus.Y] = 5;
                    laby.nb_bonus++;
                    // On indique les positions où ne peuvent pas apparaître d'autre bonus
                    for (int x = 0; x < Config.largeur_labyrinthe; x++)
                    {
                        for (int y = 0; y < Config.hauteur_labyrinthe; y++)
                        {
                            foreach (Bonus bonus in laby.liste_bonus)
                            {
                                double distance_case_bonus = Math.Sqrt(Math.Pow(bonus.position_case_bonus.X - x, 2) + Math.Pow(bonus.position_case_bonus.Y - y, 2));
                                if (distance_case_bonus <= ray(laby.Size) / 3)
                                {
                                    laby.carte_espace_bonus[x, y] = true;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void SupprimerTousBonus()
        {
            laby.liste_bonus.Clear();

            for (int y = 0; y < laby.Size.Y; y++)
            {
                for (int x = 0; x < laby.Size.X; x++)
                {
                    if (laby.Carte[x, y] == 5)
                        laby.Carte[x, y] = 0;
                }
            }
        }

        public float bonus_boussole()
        {
            float espaceZ = (joueur.position.Z - laby.sortie_position.Y * laby.CellSize.Y);
            float espaceX = (joueur.position.X - laby.sortie_position.X * laby.CellSize.X);
            float angle;
            if (espaceZ < 0 && espaceX > 0)
            {
                angle = (float)Math.PI - (float)Math.Atan(Convert.ToDouble(Math.Abs(espaceX / espaceZ)));
            }
            else if (espaceZ < 0 && espaceX < 0)
            {
                angle = (float)Math.PI + (float)Math.Atan(Convert.ToDouble(Math.Abs(espaceX / espaceZ)));
            }
            else
            {
                angle = (float)Math.Atan(Convert.ToDouble(espaceX / espaceZ));
            }

            return ((angle - pitch) % 180);
        }

        private void CompteurBonus(int durée, SpriteBatch spriteBatch)
        {
            if (joueur.gel || joueur.inversertouches || joueur.vitesse != joueur.vitesse_initiale || inversercamera || joueur.fil_ariane || affichercarte2D || changercamera || cecite || boussole_sortie)
            {
                int i = 0;
                spriteBatch.Draw(tempo_fixe, new Rectangle(0, graphics.GraphicsDevice.Viewport.Height - tempo_fixe_hauteur, tempo_fixe_largeur, tempo_fixe_hauteur), Color.White);

                while (tempo_load_largeur * durée > tempo_fixe_largeur / 2.4f)
                    tempo_load_largeur--;
                if (barre_redimension)
                {
                    while (tempo_load_largeur * durée < tempo_fixe_largeur / 2.4f)
                    {
                        tempo_load_largeur++;
                    }
                }
                barre_redimension = false;
                while (i < durée)
                {
                    spriteBatch.Draw(tempo_load, new Rectangle(graphics.GraphicsDevice.Viewport.Width / 2 - tempo_load_largeur * i, graphics.GraphicsDevice.Viewport.Height - Convert.ToInt32(tempo_load_hauteur * 1.3f), tempo_load_largeur, tempo_load_hauteur), Color.White);
                    spriteBatch.Draw(tempo_load, new Rectangle(graphics.GraphicsDevice.Viewport.Width / 2 + tempo_load_largeur * i, graphics.GraphicsDevice.Viewport.Height - Convert.ToInt32(tempo_load_hauteur * 1.3f), tempo_load_largeur, tempo_load_hauteur), Color.White);
                    i++;
                }

            }
        }

        #endregion

        #region Fonctions annexes

        #region Chronomètre
        public string chronometre(float sec)
        {
            Config.sec = Convert.ToInt32(sec);
            if (sec < 60)
            {
                if (sec < 9.5)
                    return ("00 : 0" + Config.sec);
                else
                    return ("00 : " + Config.sec);
            }
            else
            {
                int min = (int)sec / 60;
                int secondes = Config.sec % 60;
                if (min < 10)
                {
                    if (secondes < 9.5)
                        return ("0" + min + " : 0" + secondes);
                    else
                        return ("0" + min + " : " + secondes);
                }
                else
                {
                    if (secondes < 9.5)
                        return (min + " : 0" + secondes);
                    else
                        return (min + " : " + secondes);
                }

            }
        }

        public string chrono_finale(float sec)
        {
            if (sec < 60)
            {
                if (sec < 9.5)
                    return ("00 : 0" + sec);
                else
                    return ("00 : " + sec);
            }
            else
            {
                Config.min = (int)sec / 60;
                float secondes = sec - (int)((sec / 60)) * 60;
                if (Config.min < 10)
                {
                    if (secondes < 9.5)
                        return ("0" + Config.min + " : 0" + secondes);
                    else
                        return ("0" + Config.min + " : " + secondes);

                }
                else
                {
                    if (secondes < 9.5)
                        return (Config.min + " : 0" + secondes);
                    else
                        return (Config.min + " : " + secondes);
                }
            }
        }
        #endregion

        //fonction très compliqué calculant la distance a laquelle la musique poursuite est jouée.
        public int ray(Point taille)
        {
            return ((int)3 * ((taille.X + taille.Y) / 2) / 5);
        }

        //Calcul du nombre de degréé pour la boussole, en partant d'une direction Est
        public float degree()
        {
            return (pitch % 360);
        }

        //fonction calculant le degree entre la direction ou le joueur regarde et la cible.
        public float degree_target()
        {
            float espaceZ = (joueur.position.Z - scientifique.position.Z * scientifique.Scale);
            float espaceX = (joueur.position.X - scientifique.position.X * scientifique.Scale);
            float angle;
            if (espaceZ < 0 && espaceX > 0)
            {
                angle = (float)Math.PI - (float)Math.Atan(Convert.ToDouble(Math.Abs(espaceX / espaceZ)));
            }
            else if (espaceZ < 0 && espaceX < 0)
            {
                angle = (float)Math.PI + (float)Math.Atan(Convert.ToDouble(Math.Abs(espaceX / espaceZ)));
            }
            else
            {
                angle = (float)Math.Atan(Convert.ToDouble(espaceX / espaceZ));
            }

            return ((angle - pitch) % 180);
        }

        //Génére un du joueur jusqu'à la sortie
        public void GenCarteJoueurSortie()
        {
            // On remet la carte à zéro
            for (int i = 0; i < Config.largeur_labyrinthe; i++)
                for (int j = 0; j < Config.hauteur_labyrinthe; j++)
                    carte_joueur_sortie[i, j] = false;

            int[,] matrice_de_cout_sortie = new int[laby.Size.X, laby.Size.Y];
            matrice_de_cout_sortie = IA.GenLabyIA(laby.sortie_position, matrice_de_cout_sortie, laby);

            Point position_resolution = joueur.position_case_actuelle;

            while (position_resolution != laby.sortie_position)
            {
                carte_joueur_sortie[position_resolution.X, position_resolution.Y] = true;
                position_resolution = Min_ID(laby.Carte, matrice_de_cout_sortie, position_resolution);
            }
        }

         // Génére un chemin du joueur jusqu'au scientifique
        public void GenCarteJoueurScientifique()
        {
            // On remet la carte à zéro
            // On remet la carte à zéro
            for (int i = 0; i < Config.largeur_labyrinthe; i++)
                for (int j = 0; j < Config.hauteur_labyrinthe; j++)
                    carte_joueur_scientifique[i, j] = false;

            int[,] matrice_de_cout_scientifique = new int[laby.Size.X, laby.Size.Y];
            matrice_de_cout_scientifique = IA.GenLabyIA(scientifique.position_case_actuelle, matrice_de_cout_scientifique, laby);

            Point position_resolution = joueur.position_case_actuelle;

            while (position_resolution != scientifique.position_case_actuelle)
            {
                carte_joueur_scientifique[position_resolution.X, position_resolution.Y] = true;
                position_resolution = Min_ID(laby.Carte, matrice_de_cout_scientifique, position_resolution);
            }
        }

        public static List<Point> Cases_Voisines(int[,] carte, Point position)
        {
            List<Point> cases_voisines = new List<Point>();

            // Haut
            if (position.Y > 0 && carte[position.X, position.Y - 1] != 1)
                cases_voisines.Add(new Point(position.X, position.Y - 1));
            // Bas
            if (position.Y < carte.GetLength(1) - 1 && carte[position.X, position.Y + 1] != 1)
                cases_voisines.Add(new Point(position.X, position.Y + 1));
            // Gauche
            if (position.X > 0 && carte[position.X - 1, position.Y] != 1)
                cases_voisines.Add(new Point(position.X - 1, position.Y));
            // Droite
            if (position.X < carte.GetLength(0) - 1 && carte[position.X + 1, position.Y] != 1)
                cases_voisines.Add(new Point(position.X + 1, position.Y));

            return cases_voisines;
        }

        public static Point Min_ID(int[,] carte, int[,] matrice_de_cout, Point position)
        {
            Point case_min_id = new Point();
            int min_id = matrice_de_cout[position.X, position.Y];

            List<Point> cases_voisines = new List<Point>();
            cases_voisines = Cases_Voisines(carte, position);

            foreach (Point c in cases_voisines)
            {
                if (matrice_de_cout[c.X, c.Y] < min_id)
                {
                    case_min_id = c;
                    min_id = matrice_de_cout[c.X, c.Y];
                }
            }

            return case_min_id;
        }

        public void vitesseIA(double x)
        {
            if (Config.modedejeu)
            {
                chasseur.skeleton_base.TimeScale = MathHelper.Clamp((float)(Math.Log(x) / Math.Log(1.52196)), 0, 95);
                chasseur.distance_deplacement = MathHelper.Clamp((float)Math.Log(x), 0, 95);
                chasseur.vitesse_rotation = (float)((0.05 / 1.52196) * x);
            }
            else
            {
                scientifique.distance_deplacement = MathHelper.Clamp((float)Math.Log(x), 0, 95);
                scientifique.skeleton_base.TimeScale = MathHelper.Clamp((float)(Math.Log(x) / Math.Log(1.52196)), 0, 95);
                scientifique.vitesse_rotation = (float)((0.05 / 1.52196) * x);
            }
        }

        public void Afficher_Texte(string texte)
        {
            spriteBatch.DrawString(perso, texte, new Vector2((graphics.GraphicsDevice.Viewport.Width / 2) - perso.MeasureString(texte).X / 2 * (float)(1.3 * graphics.GraphicsDevice.Viewport.Width / 800), (graphics.GraphicsDevice.Viewport.Height / 2) - perso.MeasureString(texte).Y / 2 * (float)(1.3 * graphics.GraphicsDevice.Viewport.Width / 800)), Color.White, 0, new Vector2(0, 0), (float)(1.3 * graphics.GraphicsDevice.Viewport.Width / 800), SpriteEffects.None, 0);
        }

        private void Triche()
        {
            // Triche
            if (pass == "" && clavier.IsKeyDown(Keys.D))
                pass += "d";
            else if (pass == "d" && clavier.IsKeyDown(Keys.E))
                pass += "e";
            else if (pass == "de" && clavier.IsKeyDown(Keys.I))
                pass += "i";
            else if (pass == "dei" && clavier.IsKeyDown(Keys.T))
                pass += "t";
            else if (pass == "deit" && clavier.IsKeyDown(Keys.Y))
                pass += "y";
            else if (pass == "deity" && clavier.IsKeyDown(Keys.Space))
                pass += " ";
            else if (pass == "deity " && clavier.IsKeyDown(Keys.C))
                pass += "c";
            else if (pass == "deity c" && clavier.IsKeyDown(Keys.R))
                pass += "r";
            else if (pass == "deity cr" && clavier.IsKeyDown(Keys.E))
                pass += "e";
            else if (pass == "deity cre" && clavier.IsKeyDown(Keys.W))
                pass += "w";
            else if (pass == "deity crew" && clavier.IsKeyDown(Keys.Enter))
            {
                if (triche)
                {
                    SoundBank.PlayCue("bonus");
                    triche = false;
                    compteur_message_triche = 2;
                }
                else
                {
                    SoundBank.PlayCue("bonus");
                    triche = true;
                    compteur_message_triche = 2;
                }
                pass = "";
            }
        }

        public void GenLaby()
        {
            if (materials != null)
            {
                foreach(Material m in materials)
                    m.Dispose();
                materials = null;
            }
            laby.BuildMaze(new Point(laby.Size.X, laby.Size.Y), new Vector3(laby.CellSize.X, laby.CellSize.Y, laby.CellSize.Z), Content, laby, graphics.GraphicsDevice, liste_bonus_actifs);
            // On charge le labyrinthe en vertices
            MazeMaterialBuilder materialBuilder = new MazeMaterialBuilder();

            if(Config.modedejeu)
                materials = materialBuilder.BuildMazeMaterial(laby, graphics.GraphicsDevice, textures, joueur.fil_ariane, carte_joueur_sortie);
            else
                materials = materialBuilder.BuildMazeMaterial(laby, graphics.GraphicsDevice, textures, joueur.fil_ariane, carte_joueur_scientifique);
            materials_save = materials;
            // On refixe les positions du joueur et du chasseur
            joueur.position = laby.joueur_position_initiale;

            if (Config.modedejeu)
                chasseur.position = new Vector3(laby.chasseur_position_initiale.X / chasseur.Scale, laby.chasseur_position_initiale.Y, laby.chasseur_position_initiale.Z / chasseur.Scale);
            else
                scientifique.position = new Vector3(laby.chasseur_position_initiale.X / scientifique.Scale, laby.chasseur_position_initiale.Y, laby.chasseur_position_initiale.Z / scientifique.Scale);
        }

        public static List<int> Liste_Bonus_Actifs()
        {
            List<int> liste_bonus_actifs = new List<int>();

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

            if (Config.lenteur)
                liste_bonus_actifs.Add(0);
            if(Config.inversion)
                liste_bonus_actifs.Add(1);
            if(Config.gel)
                liste_bonus_actifs.Add(2);
            if (Config.touches_changees)
                liste_bonus_actifs.Add(3);
            if (Config.camera_inversee)
                liste_bonus_actifs.Add(4);
            if (Config.teleportation)
                liste_bonus_actifs.Add(5);
            if (Config.sprint)
                liste_bonus_actifs.Add(6);
            if (Config.fil_ariane)
                liste_bonus_actifs.Add(7);
            if (Config.carte)
                liste_bonus_actifs.Add(8);
            if (Config.camera_changee)
                liste_bonus_actifs.Add(9);
            if (Config.obscurite)
                liste_bonus_actifs.Add(10);
            if (Config.boussole_sortie)
                liste_bonus_actifs.Add(11);

            return liste_bonus_actifs;
        }
        #endregion
    }
}
