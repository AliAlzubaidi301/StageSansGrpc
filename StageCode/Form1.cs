using StageCode.LIB;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using OrthoDesigner.Other;
using System.Xml.Linq;
using OrthoDesigner;

namespace StageCode
{
    public partial class Form1 : Form
    {
        public static int Langue = 1; // 1 = English, 2 = Chinese, 3 = German, 4 = French, 5 = Lithuanian
        private Form1 frm;

        public static FormVide forme;
        private bool peutViderListe = false;

        private string ControlSelectionner = "";

        private string PictureBoxSelectonner = "";

        private PictureBox? ChangerPicture = null;
        private Point SourisDecalage;
        private bool Changement = false;
        private bool Bouger = false;

        private bool EnMoouvement = false;

        private bool Aligner = false;

        List<PictureBox> listPic = new List<PictureBox>();

        //A corriger
        //les commentaire et position des controls + la bd

        public Form1()
        {
            InitializeComponent();

            forme = new FormVide();

            pnlViewHost.BorderStyle = BorderStyle.FixedSingle;

            AfficherFormDansPanel(forme, pnlViewHost);

            this.DoubleBuffered = true;

            this.ClientSizeChanged += Form1_ClientSizeChanged;
            forme.panel1.MouseClick += pnlViewHost_Click;
        }
        private void AfficherFormDansPanel(Form form, Panel panel)
        {
            form.TopLevel = false;  // Le formulaire n'est pas un formulaire principal
                                    // panel.Controls.Clear();  // Supprime les contrôles existants dans le panel
            panel.Controls.Add(form);  // Ajoute le formulaire dans le panel

            form.Show();  // Affiche le formulaire
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            string tmp = Application.ProductVersion[0].ToString();
            tmp += Application.ProductVersion[1].ToString();
            tmp += Application.ProductVersion[2].ToString();
            tmp += Application.ProductVersion[3].ToString();

            this.btnVersion.Text = "V " + tmp;

            AjouterRaccourcisMenuFile();
            AjouterMenuEdition();
            AjouterbtnQuit();
            AjouterMenuVew();
            AppliquerLangue();

            Initialize();
        }

        #region Ajouter Menu File

        private void AjouterRaccourcisMenuFile()
        {
            newToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.N;
            openToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.O;
            saveToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.S;
        }

        private void AjouterbtnQuit()
        {
            btnFile.DropDownItems.Add(new ToolStripSeparator());

            ToolStripMenuItem quitMenuItem = new ToolStripMenuItem("Quit")
            {
                ShortcutKeys = Keys.Control | Keys.Q
            };
            quitMenuItem.Click += (sender, e) => Application.Exit();
            btnFile.DropDownItems.Add(quitMenuItem);
        }

        #endregion

        #region Ajouter Menu Edition

        private void AjouterMenuEdition()
        {
            ToolStripMenuItem cutMenuItem = CreerMenuItem("Cut", Keys.Control | Keys.X, Couper);
            ToolStripMenuItem copyMenuItem = CreerMenuItem("Copy", Keys.Control | Keys.C, Copier);
            ToolStripMenuItem pasteMenuItem = CreerMenuItem("Paste", Keys.Control | Keys.V, Coller);
            ToolStripMenuItem deleteMenuItem = CreerMenuItem("Delete", Keys.Delete, Supprimer);
            ToolStripMenuItem resizeMenuItem = CreerMenuItem("Resize synoptique", null, RedimensionnerSynoptique);
            ToolStripMenuItem protectedMenuItem = CreerMenuItem("Protect", null, Proteger);

            ToolStripSeparator Separateur = new ToolStripSeparator();
            ToolStripSeparator Separateur2 = new ToolStripSeparator();

            btnEdition.DropDownItems.AddRange(new ToolStripItem[]
            {
                cutMenuItem,
                copyMenuItem,
                pasteMenuItem,
                Separateur2,
                deleteMenuItem,
                Separateur,
                resizeMenuItem,
                protectedMenuItem
            });
        }

        #endregion

        #region Ajouter Menu Voir

        private void AjouterMenuVew()
        {
            ToolStripMenuItem resolutionMenuItem = CreerMenuItem("Resolution", null, null);

            ToolStripMenuItem resolution1 = CreerMenuItem("Resolution 1 (640x480)", null, (sender, e) => ChangerResolution(640, 480));
            ToolStripMenuItem resolution2 = CreerMenuItem("Resolution 2 (800x600)", null, (sender, e) => ChangerResolution(800, 600));
            ToolStripMenuItem resolution3 = CreerMenuItem("Resolution 3 (1027x768)", null, (sender, e) => ChangerResolution(1024, 768));
            ToolStripMenuItem resolution4 = CreerMenuItem("Resolution 4 (1240x1025)", null, (sender, e) => ChangerResolution(1240, 1024));

            resolutionMenuItem.DropDownItems.AddRange(new ToolStripMenuItem[] {
                resolution1,
                resolution2,
                resolution3,
                resolution4
            });

            ToolStripMenuItem languageMenuItem = new ToolStripMenuItem("Language");
            ToolStripMenuItem englishMenuItem = CreerMenuItem("English", null, (sender, e) => ChangerLangue(1));
            ToolStripMenuItem chineseMenuItem = CreerMenuItem("Chinese", null, (sender, e) => ChangerLangue(2));
            ToolStripMenuItem germanMenuItem = CreerMenuItem("German", null, (sender, e) => ChangerLangue(3));
            ToolStripMenuItem frenchMenuItem = CreerMenuItem("French", null, (sender, e) => ChangerLangue(4));
            ToolStripMenuItem lithuanianMenuItem = CreerMenuItem("Lithuanian", null, (sender, e) => ChangerLangue(5));

            languageMenuItem.DropDownItems.AddRange(new ToolStripMenuItem[]
            {
                englishMenuItem,
                chineseMenuItem,
                germanMenuItem,
                frenchMenuItem,
                lithuanianMenuItem
            });

            ToolStripMenuItem visibilityCheckerMenuItem = CreerMenuItem("Visibility Checker", null, VerifierVisibilite);

            btnView.DropDownItems.AddRange(new ToolStripMenuItem[]
            {
                resolutionMenuItem,
                languageMenuItem,
                visibilityCheckerMenuItem
            });
        }

        #endregion

        #region Méthodes Utilitaires

        private ToolStripMenuItem CreerMenuItem(string text, Keys? shortcutKeys, EventHandler? clickEvent)
        {
            ToolStripMenuItem menuItem = new ToolStripMenuItem(text);
            if (shortcutKeys.HasValue)
            {
                menuItem.ShortcutKeys = shortcutKeys.Value;
            }
            menuItem.Click += clickEvent;

            return menuItem;
        }

        #endregion

        #region Actions des menus
        public void ExportFormToXml()
        {
            // Créer un StringBuilder pour accumuler le texte de tous les contrôles
            StringBuilder xmlContent = new StringBuilder();

            // Début du fichier XML
            xmlContent.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            xmlContent.AppendLine("<Syno name=\"settings\">");

            xmlContent.AppendLine("  <Controls>");

            // Appeler la méthode SaveAs pour accumuler tous les contrôles dans xmlContent
            StringBuilder accumulatedText = SaveAsXML(); // Récupère le texte accumulé des contrôles

            // Ajouter le texte accumulé (contenu des contrôles) dans le XML
            xmlContent.AppendLine(accumulatedText.ToString());  // Ajouter le contenu des contrôles au XML
            xmlContent.AppendLine("  </Controls>");

            // Clôturer l'élément principal <Syno>
            xmlContent.AppendLine("</Syno>");

            // Ouvrir un SaveFileDialog pour choisir l'emplacement et le nom du fichier .syn
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "Fichier SYN (*.syn)|*.syn"; // Filtrer les fichiers pour .syn
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // Ouvrir un stream pour écrire dans le fichier choisi
                    using (StreamWriter writer = new StreamWriter(saveFileDialog.FileName))
                    {
                        // Écrire le texte XML accumulé dans le fichier
                        writer.Write(xmlContent.ToString());
                    }

                    // Message de confirmation
                    MessageBox.Show("Fichier sauvegardé avec succès!", "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
        private StringBuilder SaveAsXML()
        {
            // Créer un StringBuilder pour accumuler le texte de tous les contrôles
            StringBuilder accumulatedText = new StringBuilder();

            // Parcours tous les contrôles dans le panneau ou le conteneur (ici pnlViewHost)
            foreach (Control controle in forme.panel1.Controls)
            {
                // Vérifier si le contrôle est une PictureBox
                if (controle is PictureBox pictureBox)
                {
                    // Parcourir les contrôles enfants de la PictureBox
                    foreach (Control childControl in pictureBox.Controls)
                    {
                        childControl.Location = pictureBox.Location;

                        // Vérifier le type de contrôle enfant et appeler la méthode WriteFile correspondante
                        if (childControl is AM60 am60Control)
                        {
                            accumulatedText.AppendLine(" " + " " + am60Control.WriteFileXML());
                        }
                        else if (childControl is CONT1 cont1Control)
                        {
                            accumulatedText.AppendLine(cont1Control.WriteFileXML());
                        }
                        else if (childControl is INTEG integControl)
                        {
                            accumulatedText.AppendLine(integControl.WriteFileXML());
                        }
                        else if (childControl is OrthoAD orthoADControl)
                        {
                            accumulatedText.AppendLine(orthoADControl.WriteFileXML());
                        }
                        else if (childControl is OrthoAla orthoAlaControl)
                        {
                            accumulatedText.AppendLine(orthoAlaControl.WriteFileXML());
                        }
                        else if (childControl is OrthoCMDLib orthoCMDLibControl)
                        {
                            accumulatedText.AppendLine(orthoCMDLibControl.WriteFileXML());
                        }
                        else if (childControl is OrthoCombo orthoComboControl)
                        {
                            accumulatedText.AppendLine(orthoComboControl.WriteFileXML());
                        }
                        else if (childControl is OrthoDI orthoDIControl)
                        {
                            accumulatedText.AppendLine(orthoDIControl.WriteFileXML());
                        }
                        else if (childControl is OrthoEdit orthoEditControl)
                        {
                            accumulatedText.AppendLine(orthoEditControl.WriteFileXML());
                        }
                        else if (childControl is OrthoImage orthoImageControl)
                        {
                            accumulatedText.AppendLine(orthoImageControl.WriteFileXML());
                        }
                        else if (childControl is OrthoLabel orthoLabelControl)
                        {
                            accumulatedText.AppendLine(orthoLabelControl.WriteFileXML());
                        }
                        else if (childControl is OrthoPbar orthoPbarControl)
                        {
                            accumulatedText.AppendLine(orthoPbarControl.WriteFileXML());
                        }
                        else if (childControl is OrthoRel orthoRelControl)
                        {
                            accumulatedText.AppendLine(orthoRelControl.WriteFileXML());
                        }
                        else if (childControl is OrthoResult orthoResultControl)
                        {
                            accumulatedText.AppendLine(orthoResultControl.WriteFileXML());
                        }
                        else if (childControl is OrthoVarname orthoVarnameControl)
                        {
                            accumulatedText.AppendLine(orthoVarnameControl.WriteFileXML());
                        }
                        else if (childControl is Reticule reticuleControl)
                        {
                            accumulatedText.AppendLine(reticuleControl.WriteFileXML());
                        }
                    }
                }
            }

            return accumulatedText;
        }


        public void ExportFormToTXT()
        {
            // Créer un StringBuilder pour accumuler le texte de tous les contrôles


            // Appeler la méthode SaveAs pour accumuler tous les contrôles dans xmlContent
            StringBuilder accumulatedText = SaveAsTXT(); // Récupère le texte accumulé des contrôles



            // Ouvrir un SaveFileDialog pour choisir l'emplacement et le nom du fichier .syn
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "Fichier SYN (*.syn)|*.syn"; // Filtrer les fichiers pour .syn
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // Ouvrir un stream pour écrire dans le fichier choisi
                    using (StreamWriter writer = new StreamWriter(saveFileDialog.FileName))
                    {
                        // Écrire le texte XML accumulé dans le fichier
                        writer.Write(accumulatedText.ToString());
                    }

                    // Message de confirmation
                    MessageBox.Show("Fichier sauvegardé avec succès!", "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
        private StringBuilder SaveAsTXT()
        {
            // Créer un StringBuilder pour accumuler le texte de tous les contrôles
            StringBuilder accumulatedText = new StringBuilder();

            // Parcours tous les contrôles dans le panneau ou le conteneur (ici pnlViewHost)
            foreach (Control controle in forme.panel1.Controls)
            {
                // Vérifier si le contrôle est une PictureBox
                if (controle is PictureBox pictureBox)
                {
                    // Parcourir les contrôles enfants de la PictureBox
                    foreach (Control childControl in pictureBox.Controls)
                    {
                        childControl.Location = pictureBox.Location;

                        // Vérifier le type de contrôle enfant et appeler la méthode WriteFile correspondante
                        if (childControl is AM60 am60Control)
                        {
                            accumulatedText.AppendLine(am60Control.WriteFile());
                        }
                        else if (childControl is CONT1 cont1Control)
                        {
                            accumulatedText.AppendLine(cont1Control.WriteFile());
                        }
                        else if (childControl is INTEG integControl)
                        {
                            accumulatedText.AppendLine(integControl.WriteFile());
                        }
                        else if (childControl is OrthoAD orthoADControl)
                        {
                            accumulatedText.AppendLine(orthoADControl.WriteFile());
                        }
                        else if (childControl is OrthoAla orthoAlaControl)
                        {
                            accumulatedText.AppendLine(orthoAlaControl.WriteFile());
                        }
                        else if (childControl is OrthoCMDLib orthoCMDLibControl)
                        {
                            accumulatedText.AppendLine(orthoCMDLibControl.WriteFile());
                        }
                        else if (childControl is OrthoCombo orthoComboControl)
                        {
                            accumulatedText.AppendLine(orthoComboControl.WriteFile());
                        }
                        else if (childControl is OrthoDI orthoDIControl)
                        {
                            accumulatedText.AppendLine(orthoDIControl.WriteFile());
                        }
                        else if (childControl is OrthoEdit orthoEditControl)
                        {
                            accumulatedText.AppendLine(orthoEditControl.WriteFile());
                        }
                        else if (childControl is OrthoImage orthoImageControl)
                        {
                            accumulatedText.AppendLine(orthoImageControl.WriteFile());
                        }
                        else if (childControl is OrthoLabel orthoLabelControl)
                        {
                            accumulatedText.AppendLine(orthoLabelControl.WriteFile());
                        }
                        else if (childControl is OrthoPbar orthoPbarControl)
                        {
                            accumulatedText.AppendLine(orthoPbarControl.WriteFile());
                        }
                        else if (childControl is OrthoRel orthoRelControl)
                        {
                            accumulatedText.AppendLine(orthoRelControl.WriteFile());
                        }
                        else if (childControl is OrthoResult orthoResultControl)
                        {
                            accumulatedText.AppendLine(orthoResultControl.WriteFile());
                        }
                        else if (childControl is OrthoVarname orthoVarnameControl)
                        {
                            accumulatedText.AppendLine(orthoVarnameControl.WriteFile());
                        }
                        else if (childControl is Reticule reticuleControl)
                        {
                            accumulatedText.AppendLine(reticuleControl.WriteFile());
                        }
                    }
                }
            }

            return accumulatedText;
        }



        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string message = "";
            string title = "";

            switch (Langue)
            {
                case 1: // English
                    message = "Save the changes?";
                    title = "Save";
                    break;
                case 2: // Chinese
                    message = "保存更改？";
                    title = "保存";
                    break;
                case 3: // German
                    message = "Änderungen speichern?";
                    title = "Speichern";
                    break;
                case 4: // French
                    message = "Enregistrer les modifications ?";
                    title = "Enregistrer";
                    break;
                case 5: // Lithuanian
                    message = "Išsaugoti pakeitimus?";
                    title = "Išsaugoti";
                    break;
            }

            DialogResult r = MessageBox.Show(message, title, MessageBoxButtons.YesNoCancel);

            if (r == DialogResult.Yes)
            {
                // Demande si l'utilisateur souhaite sauvegarder en XML ou non
                string saveMessage = "";
                string saveTitle = "";

                switch (Langue)
                {
                    case 1: // English
                        saveMessage = "Do you want to save as XML?";
                        saveTitle = "Save As XML";
                        break;
                    case 2: // Chinese
                        saveMessage = "您是否要以XML格式保存？";
                        saveTitle = "保存为XML";
                        break;
                    case 3: // German
                        saveMessage = "Möchten Sie als XML speichern?";
                        saveTitle = "Als XML speichern";
                        break;
                    case 4: // French
                        saveMessage = "Voulez-vous enregistrer en XML ?";
                        saveTitle = "Enregistrer en XML";
                        break;
                    case 5: // Lithuanian
                        saveMessage = "Ar norite išsaugoti kaip XML?";
                        saveTitle = "Išsaugoti kaip XML";
                        break;
                }

                // Affichage de la boîte de dialogue pour le choix de sauvegarde en XML
                DialogResult saveResult = MessageBox.Show(saveMessage, saveTitle, MessageBoxButtons.YesNo);

                if (saveResult == DialogResult.Yes)
                {
                    ExportFormToXml();  // Sauvegarder en XML
                }
                else
                {
                    ExportFormToTXT();
                }
            }
            else if (r == DialogResult.Cancel)
            {
                return;
            }

            pnlViewHost.Controls.Clear();

            forme = new FormVide();

            pnlViewHost.BorderStyle = BorderStyle.FixedSingle;

            AfficherFormDansPanel(forme, pnlViewHost);
        }


        private void Couper(object sender, EventArgs e)
        {
            // Trouver la PictureBox sélectionnée
            PictureBox? pic = forme.panel1.Controls.OfType<PictureBox>()
                                              .FirstOrDefault(p => p.Name == PictureBoxSelectonner);

            if (pic != null)
            {
                // Liste des informations des contrôles contenus dans la PictureBox
                List<SerializableControl> controlsData = new List<SerializableControl>();

                foreach (Control ctrl in pic.Controls)
                {
                    controlsData.Add(new SerializableControl
                    {
                        TypeName = ctrl.GetType().AssemblyQualifiedName ?? "",
                        Name = ctrl.Name,
                        X = ctrl.Location.X,
                        Y = ctrl.Location.Y,
                        Width = ctrl.Width,
                        Height = ctrl.Height,
                        Text = (ctrl is TextBox textBox) ? textBox.Text : ctrl.Text
                    });
                }

                // Sérialiser les données
                DataObject data = new DataObject();
                using (MemoryStream ms = new MemoryStream())
                {
#pragma warning disable SYSLIB0011 // Le type ou le membre est obsolète
                    BinaryFormatter bf = new BinaryFormatter();
#pragma warning restore SYSLIB0011 // Le type ou le membre est obsolète
                    bf.Serialize(ms, controlsData);
                    data.SetData("ControlsData", ms.ToArray());
                }

                // Mettre les données dans le presse-papiers
                Clipboard.SetDataObject(data, true);

                // Supprimer uniquement les contrôles internes
                pic.Controls.Clear();
                forme.panel1.Controls.Remove(pic);
            }
        }






        private void Copier(object sender, EventArgs e)
        {
            // Trouver la PictureBox sélectionnée
            PictureBox? pic = forme.panel1.Controls.OfType<PictureBox>()
                                              .FirstOrDefault(p => p.Name == PictureBoxSelectonner);

            if (pic != null)
            {
                // Liste des informations des contrôles contenus dans la PictureBox
                List<SerializableControl> controlsData = new List<SerializableControl>();

                foreach (Control ctrl in pic.Controls)
                {
                    controlsData.Add(new SerializableControl
                    {
                        TypeName = ctrl.GetType().AssemblyQualifiedName ?? "",
                        Name = ctrl.Name,
                        X = ctrl.Location.X,
                        Y = ctrl.Location.Y,
                        Width = ctrl.Width,
                        Height = ctrl.Height,
                        Text = (ctrl is TextBox textBox) ? textBox.Text : ctrl.Text
                    });
                }

                // Sérialiser les données
                DataObject data = new DataObject();
                using (MemoryStream ms = new MemoryStream())
                {
#pragma warning disable SYSLIB0011 // Le type ou le membre est obsolète
                    BinaryFormatter bf = new BinaryFormatter();
#pragma warning restore SYSLIB0011 // Le type ou le membre est obsolète
                    bf.Serialize(ms, controlsData);
                    data.SetData("ControlsData", ms.ToArray());
                }

                // Mettre les données dans le presse-papiers
                Clipboard.SetDataObject(data, true);

                MessageBox.Show("Copie effectuée !");
            }
            else
            {
                MessageBox.Show("Aucune PictureBox sélectionnée.");
            }
        }


        private void Coller(object sender, EventArgs e)
        {
            if (Clipboard.ContainsData("ControlsData"))
            {
                // Récupérer la position de la souris relative à pnlViewHost
                Point mousePosition = forme.panel1.PointToClient(Cursor.Position);

                // Récupérer les données du presse-papiers
                byte[] rawData = (byte[])Clipboard.GetData("ControlsData")!;
                using (MemoryStream ms = new MemoryStream(rawData))
                {
#pragma warning disable SYSLIB0011 // Suppression de l'avertissement d'obsolescence
                    BinaryFormatter bf = new BinaryFormatter();
#pragma warning restore SYSLIB0011
                    List<SerializableControl> controlsData = (List<SerializableControl>)bf.Deserialize(ms);

                    // Créer une nouvelle PictureBox pour contenir les contrôles collés
                    PictureBox newpic = new PictureBox
                    {
                        BorderStyle = BorderStyle.None,
                        BackColor = Color.White,
                        Size = new Size(200, 200), // Taille par défaut, peut être ajustée
                        Location = mousePosition, // Position à l'endroit du clic
                        AllowDrop = true // Pour gérer éventuellement le drag & drop
                    };

                    newpic.Paint += pic_Paint;
                    newpic.Click += Control_Click;
                    newpic.MouseLeave += pic_MouseLeave;

                    // Restaurer les contrôles dans la nouvelle PictureBox
                    foreach (var controlData in controlsData)
                    {
                        Type? controlType = Type.GetType(controlData.TypeName);
                        if (controlType != null)
                        {
                            Control Control = (Control)Activator.CreateInstance(controlType)!;
                            Control.Name = controlData.Name;
                            Control.Location = new Point(controlData.X, controlData.Y);
                            Control.Size = new Size(controlData.Width, controlData.Height);
                            if (Control is TextBox textBox) textBox.Text = controlData.Text;
                            else Control.Text = controlData.Text;

                            newpic.Controls.Add(Control);

                            Control.MouseEnter += Control_MouseEnter;

                            Control.Click += Control_Click;

                            Control.MouseClick += Control_MouseClick;
                        }
                    }

                    forme.panel1.Controls.Add(newpic);
                    newpic.Invalidate(); // Redessiner
                    PictureBoxSelectonner = "";
                }
            }
        }

        private void Supprimer(object sender, EventArgs e)
        {
            PictureBox? pic = forme.panel1.Controls.OfType<PictureBox>()
                                      .FirstOrDefault(p => p.Name == PictureBoxSelectonner);

            if (pic != null)
            {
                forme.panel1.Controls.Remove(pic);
                pic.Dispose(); // Libérer la mémoire utilisée par la PictureBox
            }
            else
            {
                MessageBox.Show("Aucune PictureBox sélectionnée.");
            }
        }

        private void RedimensionnerSynoptique(object sender, EventArgs e)
        {
            FormResize forme = new FormResize(this);
            forme.ShowDialog();
        }

        private void Proteger(object sender, EventArgs e)
        {
            MessageBox.Show("Protégé");
        }

        private void ChangerResolution(int largeur, int hauteur)
        {
            if (largeur > 0 && hauteur > 0)
            {
                this.ClientSize = new Size(largeur, hauteur);

                // pnlViewHost.Controls.Clear();

                //  Form1_ClientSizeChanged(new object(), new EventArgs());
            }
        }

        private void ChangerLangue(int nouvelleLangue)
        {
            Langue = nouvelleLangue;
            AppliquerLangue();
        }

        private void VerifierVisibilite(object sender, EventArgs e)
        {
            MessageBox.Show("Vérifier la visibilité");
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            switch (Langue)
            {
                case 1: // English
                    MessageBox.Show("Designer synoptic version: " + Application.ProductVersion);
                    break;

                case 2: // Chinese
                    MessageBox.Show("设计师概览版本: " + Application.ProductVersion);
                    break;

                case 3: // German
                    MessageBox.Show("Designer Synoptik Version: " + Application.ProductVersion);
                    break;

                case 4: // French
                    MessageBox.Show("Version du designer synoptique : " + Application.ProductVersion);
                    break;

                case 5: // Lithuanian
                    MessageBox.Show("Sinoptinio dizainerio versija: " + Application.ProductVersion);
                    break;

                default:
                    MessageBox.Show("Designer synoptic version: " + Application.ProductVersion);
                    break;
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string message = "";
            string title = "";

            // Définition du message et du titre en fonction de la langue
            switch (Langue)
            {
                case 1: // English
                    message = "Save the changes before opening a new file?";
                    title = "Save Changes";
                    break;
                case 2: // Chinese
                    message = "在打开新文件之前保存更改？";
                    title = "保存更改";
                    break;
                case 3: // German
                    message = "Änderungen speichern, bevor eine neue Datei geöffnet wird?";
                    title = "Änderungen speichern";
                    break;
                case 4: // French
                    message = "Enregistrer les modifications avant d'ouvrir un nouveau fichier ?";
                    title = "Enregistrer les modifications";
                    break;
                case 5: // Lithuanian
                    message = "Prieš atidarant naują failą, išsaugoti pakeitimus?";
                    title = "Išsaugoti pakeitimus";
                    break;
            }

            DialogResult r = MessageBox.Show(message, title, MessageBoxButtons.YesNo, MessageBoxIcon.Information);

            try
            {
                if (r == DialogResult.Yes)
                {
                    ExportFormToXml();
                }

                forme.panel1.Controls.Clear();

                OpenFileDialog file = new OpenFileDialog();
                file.Filter = "Fichier SYN (*.syn)|*.syn";

                file.ShowDialog();
                string a = LireXML(file.FileName);

                MessageBox.Show(a);
                RecupererContenuXML(a);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private string LireXML(string filePath)
        {
            try
            {
                string xmlContent = File.ReadAllText(filePath);
                return xmlContent;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la lecture du fichier XML : {ex.Message}");
            }
        }

        private void RecupererContenuXML(string xmlContent)
        {
            try
            {
                XElement xml = XElement.Parse(xmlContent);

                // Parcourir tous les éléments <Component> du XML
                foreach (XElement component in xml.Descendants("Component"))
                {
                    string? type = component.Attribute("type")?.Value;

                    // Convertir l'élément en texte pour passer à ReadFileXML
                    string componentText = component.ToString();

                    if (type == "AM60")
                    {
                        // Appeler la fonction statique ReadFileXML pour récupérer l'objet AM60
                        AM60 am60Control = AM60.ReadFileXML(componentText);

                        // Extraire les informations de position et de taille depuis le XML
                        XElement? appearance = component.Element("Apparence");
                        if (appearance != null)
                        {
                            // Assurez-vous de définir la taille et la position
                            int sizeWidth = int.Parse(appearance.Element("SizeWidth")?.Value ?? "100");
                            int sizeHeight = int.Parse(appearance.Element("SizeHeight")?.Value ?? "100");
                            int locationX = int.Parse(appearance.Element("LocationX")?.Value ?? "0");
                            int locationY = int.Parse(appearance.Element("LocationY")?.Value ?? "0");

                            am60Control.Size = new Size(sizeWidth, sizeHeight);

                            //Mes
                            PictureBox pic = new PictureBox
                            {
                                Size = new Size(am60Control.Size.Width + 10, am60Control.Size.Height + 10), // Augmenter la taille de 10 pixels
                                Location = new Point(locationX, locationY) // Appliquer la position
                            };

                            am60Control.Location = new Point(5, 5);

                            pic.MouseLeave += pic_MouseLeave;

                            pic.Controls.Add(am60Control);

                            am60Control.Click += Control_Click;
                            am60Control.MouseEnter += Control_MouseEnter;

                            forme.panel1.Controls.Add(pic);
                        }
                    }
                    else if (type == "CONT1")
                    {
                        // Appeler la fonction statique ReadFileXML pour récupérer l'objet Cont1
                        CONT1 cont1Control = CONT1.ReadFileXML(componentText);

                        // Extraire les informations de position et de taille depuis le XML
                        XElement? appearance = component.Element("Apparence");
                        if (appearance != null)
                        {
                            // Assurez-vous de définir la taille et la position
                            int sizeWidth = int.Parse(appearance.Element("SizeWidth")?.Value ?? "100");
                            int sizeHeight = int.Parse(appearance.Element("SizeHeight")?.Value ?? "100");
                            int locationX = int.Parse(appearance.Element("LocationX")?.Value ?? "0");
                            int locationY = int.Parse(appearance.Element("LocationY")?.Value ?? "0");

                            // Définir la taille du contrôle Cont1
                            cont1Control.Size = new Size(sizeWidth, sizeHeight);

                            // Créer une PictureBox pour contenir le contrôle
                            PictureBox pic = new PictureBox
                            {
                                Size = new Size(cont1Control.Size.Width + 10, cont1Control.Size.Height + 10), // Augmenter la taille de 10 pixels
                                Location = new Point(locationX, locationY) // Appliquer la position
                            };

                            cont1Control.Location = new Point(5, 5);


                            // Ajouter le contrôle Cont1 à la PictureBox
                            pic.Controls.Add(cont1Control);

                            cont1Control.MouseEnter += Control_MouseEnter;
                            pic.MouseLeave += pic_MouseLeave;

                            cont1Control.Click += Control_Click;


                            // Ajouter la PictureBox au conteneur principal
                            forme.panel1.Controls.Add(pic);
                        }
                    }
                    else if (type == "INTEG")
                    {
                        // Appeler la fonction statique ReadFileXML pour récupérer l'objet INTEG
                        INTEG integControl = INTEG.ReadFileXML(componentText);

                        // Extraire les informations de position et de taille depuis le XML
                        XElement? appearance = component.Element("Apparence");
                        if (appearance != null)
                        {
                            // Assurez-vous de définir la taille et la position
                            int sizeWidth = int.Parse(appearance.Element("SizeWidth")?.Value ?? "100");
                            int sizeHeight = int.Parse(appearance.Element("SizeHeight")?.Value ?? "100");
                            int locationX = int.Parse(appearance.Element("LocationX")?.Value ?? "0");
                            int locationY = int.Parse(appearance.Element("LocationY")?.Value ?? "0");

                            // Définir la taille du contrôle INTEG
                            integControl.Size = new Size(sizeWidth, sizeHeight);

                            // Créer une PictureBox pour contenir le contrôle
                            PictureBox pic = new PictureBox
                            {
                                Size = new Size(integControl.Size.Width + 10, integControl.Size.Height + 10), // Augmenter la taille de 10 pixels
                                Location = new Point(locationX, locationY) // Appliquer la position
                            };

                            integControl.Location = new Point(5, 5);

                            // Ajouter le contrôle INTEG à la PictureBox
                            pic.Controls.Add(integControl);

                            // Ajouter des gestionnaires d'événements à l'objet INTEG si nécessaire
                            integControl.MouseEnter += Control_MouseEnter;
                            pic.MouseLeave += pic_MouseLeave;

                            integControl.Click += Control_Click;

                            // Ajouter la PictureBox au conteneur principal
                            forme.panel1.Controls.Add(pic);
                        }
                    }
                    else if (type == "OrthoAD")
                    {
                        // Appeler la fonction statique ReadFileXML pour récupérer l'objet OrthoAD
                        OrthoAD orthoADControl = OrthoAD.ReadFileXML(componentText);

                        // Extraire les informations de position et de taille depuis le XML
                        XElement? appearance = component.Element("Apparence");
                        if (appearance != null)
                        {
                            // Assurez-vous de définir la taille et la position
                            int sizeWidth = int.Parse(appearance.Element("SizeWidth")?.Value ?? "100");
                            int sizeHeight = int.Parse(appearance.Element("SizeHeight")?.Value ?? "100");
                            int locationX = int.Parse(appearance.Element("LocationX")?.Value ?? "0");
                            int locationY = int.Parse(appearance.Element("LocationY")?.Value ?? "0");

                            // Définir la taille du contrôle OrthoAD
                            orthoADControl.Size = new Size(sizeWidth, sizeHeight);

                            // Créer une PictureBox pour contenir le contrôle
                            PictureBox pic = new PictureBox
                            {
                                Size = new Size(orthoADControl.Size.Width + 10, orthoADControl.Size.Height + 10), // Augmenter la taille de 10 pixels
                                Location = new Point(locationX, locationY) // Appliquer la position
                            };

                            orthoADControl.Location = new Point(5, 5);

                            // Ajouter le contrôle OrthoAD à la PictureBox
                            pic.Controls.Add(orthoADControl);

                            // Ajouter des gestionnaires d'événements à l'objet OrthoAD si nécessaire
                            orthoADControl.MouseEnter += Control_MouseEnter;
                            pic.MouseLeave += pic_MouseLeave;

                            orthoADControl.Click += Control_Click;

                            // Ajouter la PictureBox au conteneur principal
                            forme.panel1.Controls.Add(pic);
                        }
                    }
                    else if (type == "OrthoAla")
                    {
                        // Appeler la fonction statique ReadFileXML pour récupérer l'objet OrthoAla
                        OrthoAla orthoAlaControl = OrthoAla.ReadFileXML(componentText);

                        // Extraire les informations de position et de taille depuis le XML
                        XElement? appearance = component.Element("Apparence");
                        if (appearance != null)
                        {
                            // Assurez-vous de définir la taille et la position
                            int sizeWidth = int.Parse(appearance.Element("SizeWidth")?.Value ?? "100");
                            int sizeHeight = int.Parse(appearance.Element("SizeHeight")?.Value ?? "100");
                            int locationX = int.Parse(appearance.Element("LocationX")?.Value ?? "0");
                            int locationY = int.Parse(appearance.Element("LocationY")?.Value ?? "0");

                            // Définir la taille du contrôle OrthoAla
                            orthoAlaControl.Size = new Size(sizeWidth, sizeHeight);

                            // Créer une PictureBox pour contenir le contrôle
                            PictureBox pic = new PictureBox
                            {
                                Size = new Size(orthoAlaControl.Size.Width + 10, orthoAlaControl.Size.Height + 10), // Augmenter la taille de 10 pixels
                                Location = new Point(locationX, locationY) // Appliquer la position
                            };

                            orthoAlaControl.Location = new Point(5, 5);


                            // Ajouter le contrôle OrthoAla à la PictureBox
                            pic.Controls.Add(orthoAlaControl);

                            // Ajouter des gestionnaires d'événements à l'objet OrthoAla si nécessaire
                            orthoAlaControl.MouseEnter += Control_MouseEnter;
                            pic.MouseLeave += pic_MouseLeave;
                            orthoAlaControl.Click += Control_Click;

                            // Ajouter la PictureBox au conteneur principal
                            forme.panel1.Controls.Add(pic);
                        }
                    }
                    else if (type == "OrthoCMDLib")
                    {
                        // Appeler la fonction statique ReadFileXML pour récupérer l'objet OrthoAla
                        OrthoCMDLib orthoAlaControl = OrthoCMDLib.ReadFileXML(componentText);

                        // Extraire les informations de position et de taille depuis le XML
                        XElement? appearance = component.Element("Apparence");
                        if (appearance != null)
                        {
                            // Assurez-vous de définir la taille et la position
                            int sizeWidth = int.Parse(appearance.Element("SizeWidth")?.Value ?? "100");
                            int sizeHeight = int.Parse(appearance.Element("SizeHeight")?.Value ?? "100");
                            int locationX = int.Parse(appearance.Element("LocationX")?.Value ?? "0");
                            int locationY = int.Parse(appearance.Element("LocationY")?.Value ?? "0");

                            // Définir la taille du contrôle OrthoAla
                            orthoAlaControl.Size = new Size(sizeWidth, sizeHeight);

                            // Créer une PictureBox pour contenir le contrôle
                            PictureBox pic = new PictureBox
                            {
                                Size = new Size(orthoAlaControl.Size.Width + 10, orthoAlaControl.Size.Height + 10), // Augmenter la taille de 10 pixels
                                Location = new Point(locationX, locationY) // Appliquer la position
                            };

                            orthoAlaControl.Location = new Point(5, 5);


                            // Ajouter le contrôle OrthoAla à la PictureBox
                            pic.Controls.Add(orthoAlaControl);

                            // Ajouter des gestionnaires d'événements à l'objet OrthoAla si nécessaire
                            orthoAlaControl.MouseEnter += Control_MouseEnter;
                            pic.MouseLeave += pic_MouseLeave;
                            orthoAlaControl.Click += Control_Click;

                            // Ajouter la PictureBox au conteneur principal
                            forme.panel1.Controls.Add(pic);
                        }
                    }
                    else if (type == "OrthoCombo")
                    {
                        // Appeler la fonction statique ReadFileXML pour récupérer l'objet OrthoAla
                        OrthoCombo orthoAlaControl = OrthoCombo.ReadFileXML(componentText);

                        // Extraire les informations de position et de taille depuis le XML
                        XElement? appearance = component.Element("Apparence");
                        if (appearance != null)
                        {
                            // Assurez-vous de définir la taille et la position
                            int sizeWidth = int.Parse(appearance.Element("SizeWidth")?.Value ?? "100");
                            int sizeHeight = int.Parse(appearance.Element("SizeHeight")?.Value ?? "100");
                            int locationX = int.Parse(appearance.Element("LocationX")?.Value ?? "0");
                            int locationY = int.Parse(appearance.Element("LocationY")?.Value ?? "0");

                            // Définir la taille du contrôle OrthoAla
                            orthoAlaControl.Size = new Size(sizeWidth, sizeHeight);

                            // Créer une PictureBox pour contenir le contrôle
                            PictureBox pic = new PictureBox
                            {
                                Size = new Size(orthoAlaControl.Size.Width + 10, orthoAlaControl.Size.Height + 10), // Augmenter la taille de 10 pixels
                                Location = new Point(locationX, locationY) // Appliquer la position
                            };

                            orthoAlaControl.Location = new Point(5, 5);


                            // Ajouter le contrôle OrthoAla à la PictureBox
                            pic.Controls.Add(orthoAlaControl);

                            orthoAlaControl.MouseEnter += Control_MouseEnter;
                            pic.MouseLeave += pic_MouseLeave;
                            orthoAlaControl.Click += Control_Click;

                            forme.panel1.Controls.Add(pic);
                        }
                    }
                    else if (type == "OrthoDI")
                    {
                        OrthoDI orthoAlaControl = OrthoDI.ReadFileXML(componentText);

                        XElement? appearance = component.Element("Apparence");
                        if (appearance != null)
                        {
                            int sizeWidth = int.Parse(appearance.Element("SizeWidth")?.Value ?? "100");
                            int sizeHeight = int.Parse(appearance.Element("SizeHeight")?.Value ?? "100");
                            int locationX = int.Parse(appearance.Element("LocationX")?.Value ?? "0");
                            int locationY = int.Parse(appearance.Element("LocationY")?.Value ?? "0");

                            orthoAlaControl.Size = new Size(sizeWidth, sizeHeight);

                            PictureBox pic = new PictureBox
                            {
                                Size = new Size(orthoAlaControl.Size.Width + 10, orthoAlaControl.Size.Height + 10), // Augmenter la taille de 10 pixels
                                Location = new Point(locationX, locationY) // Appliquer la position
                            };

                            orthoAlaControl.Location = new Point(5, 5);

                            pic.Controls.Add(orthoAlaControl);

                            orthoAlaControl.MouseEnter += Control_MouseEnter;
                            pic.MouseLeave += pic_MouseLeave;
                            orthoAlaControl.Click += Control_Click;

                            forme.panel1.Controls.Add(pic);
                        }
                    }
                    else if (type == "OrthoEdit")
                    {
                        OrthoEdit orthoAlaControl = new OrthoEdit();

                        orthoAlaControl = orthoAlaControl.ReadFileXML(componentText);

                        XElement? appearance = component.Element("Apparence");
                        if (appearance != null)
                        {
                            int sizeWidth = int.Parse(appearance.Element("SizeWidth")?.Value ?? "100");
                            int sizeHeight = int.Parse(appearance.Element("SizeHeight")?.Value ?? "100");
                            int locationX = int.Parse(appearance.Element("LocationX")?.Value ?? "0");
                            int locationY = int.Parse(appearance.Element("LocationY")?.Value ?? "0");

                            orthoAlaControl.Size = new Size(sizeWidth, sizeHeight);

                            PictureBox pic = new PictureBox
                            {
                                Size = new Size(orthoAlaControl.Size.Width + 10, orthoAlaControl.Size.Height + 10), // Augmenter la taille de 10 pixels
                                Location = new Point(locationX, locationY) // Appliquer la position
                            };

                            orthoAlaControl.Location = new Point(5, 5);

                            pic.Controls.Add(orthoAlaControl);

                            orthoAlaControl.MouseEnter += Control_MouseEnter;
                            pic.MouseLeave += pic_MouseLeave;
                            orthoAlaControl.Click += Control_Click;

                            forme.panel1.Controls.Add(pic);
                        }
                    }
                    else if (type == "OrthoImage")
                    {
                        OrthoImage orthoAlaControl = new OrthoImage();

                        orthoAlaControl = orthoAlaControl.ReadFileXML(componentText);

                        XElement? appearance = component.Element("Apparence");
                        if (appearance != null)
                        {
                            int sizeWidth = int.Parse(appearance.Element("SizeWidth")?.Value ?? "100");
                            int sizeHeight = int.Parse(appearance.Element("SizeHeight")?.Value ?? "100");
                            int locationX = int.Parse(appearance.Element("LocationX")?.Value ?? "0");
                            int locationY = int.Parse(appearance.Element("LocationY")?.Value ?? "0");

                            orthoAlaControl.Size = new Size(sizeWidth, sizeHeight);

                            PictureBox pic = new PictureBox
                            {
                                Size = new Size(orthoAlaControl.Size.Width + 10, orthoAlaControl.Size.Height + 10), // Augmenter la taille de 10 pixels
                                Location = new Point(locationX, locationY) // Appliquer la position
                            };

                            orthoAlaControl.Location = new Point(5, 5);

                            pic.Controls.Add(orthoAlaControl);

                            orthoAlaControl.MouseEnter += Control_MouseEnter;
                            pic.MouseLeave += pic_MouseLeave;
                            orthoAlaControl.Click += Control_Click;

                            forme.panel1.Controls.Add(pic);
                        }
                    }
                    else if (type == "OrthoLabel")
                    {
                        OrthoLabel orthoAlaControl = new OrthoLabel();

                        orthoAlaControl = orthoAlaControl.ReadFileXML(componentText);

                        XElement? appearance = component.Element("Apparence");
                        if (appearance != null)
                        {
                            int sizeWidth = int.Parse(appearance.Element("SizeWidth")?.Value ?? "100");
                            int sizeHeight = int.Parse(appearance.Element("SizeHeight")?.Value ?? "100");
                            int locationX = int.Parse(appearance.Element("LocationX")?.Value ?? "0");
                            int locationY = int.Parse(appearance.Element("LocationY")?.Value ?? "0");

                            orthoAlaControl.Size = new Size(sizeWidth, sizeHeight);

                            PictureBox pic = new PictureBox
                            {
                                Size = new Size(orthoAlaControl.Size.Width + 10, orthoAlaControl.Size.Height + 10), // Augmenter la taille de 10 pixels
                                Location = new Point(locationX, locationY) // Appliquer la position
                            };

                            orthoAlaControl.Location = new Point(5, 5);


                            pic.Controls.Add(orthoAlaControl);

                            orthoAlaControl.MouseEnter += Control_MouseEnter;
                            pic.MouseLeave += pic_MouseLeave;
                            orthoAlaControl.Click += Control_Click;

                            forme.panel1.Controls.Add(pic);
                        }
                    }
                    else if (type == "OrthoPbar")
                    {
                        OrthoPbar orthoAlaControl = new OrthoPbar();

                        orthoAlaControl = orthoAlaControl.ReadFileXML(componentText);

                        XElement? appearance = component.Element("Apparence");
                        if (appearance != null)
                        {
                            int sizeWidth = int.Parse(appearance.Element("SizeWidth")?.Value ?? "100");
                            int sizeHeight = int.Parse(appearance.Element("SizeHeight")?.Value ?? "100");
                            int locationX = int.Parse(appearance.Element("LocationX")?.Value ?? "0");
                            int locationY = int.Parse(appearance.Element("LocationY")?.Value ?? "0");

                            orthoAlaControl.Size = new Size(sizeWidth, sizeHeight);

                            PictureBox pic = new PictureBox
                            {
                                Size = new Size(orthoAlaControl.Size.Width + 10, orthoAlaControl.Size.Height + 10), // Augmenter la taille de 10 pixels
                                Location = new Point(locationX, locationY) // Appliquer la position
                            };

                            orthoAlaControl.Location = new Point(5, 5);

                            pic.Controls.Add(orthoAlaControl);

                            orthoAlaControl.MouseEnter += Control_MouseEnter;
                            pic.MouseLeave += pic_MouseLeave;
                            orthoAlaControl.Click += Control_Click;

                            forme.panel1.Controls.Add(pic);
                        }
                    }
                    else if (type == "OrthoRel")
                    {
                        OrthoRel orthoAlaControl = new OrthoRel();

                        orthoAlaControl = orthoAlaControl.ReadFileXML(componentText);

                        XElement? appearance = component.Element("Apparence");
                        if (appearance != null)
                        {
                            int sizeWidth = int.Parse(appearance.Element("SizeWidth")?.Value ?? "100");
                            int sizeHeight = int.Parse(appearance.Element("SizeHeight")?.Value ?? "100");
                            int locationX = int.Parse(appearance.Element("LocationX")?.Value ?? "0");
                            int locationY = int.Parse(appearance.Element("LocationY")?.Value ?? "0");

                            orthoAlaControl.Size = new Size(sizeWidth, sizeHeight);

                            PictureBox pic = new PictureBox
                            {
                                Size = new Size(orthoAlaControl.Size.Width + 10, orthoAlaControl.Size.Height + 10), // Augmenter la taille de 10 pixels
                                Location = new Point(locationX, locationY) // Appliquer la position
                            };

                            orthoAlaControl.Location = new Point(5, 5);

                            pic.Controls.Add(orthoAlaControl);

                            orthoAlaControl.MouseEnter += Control_MouseEnter;
                            pic.MouseLeave += pic_MouseLeave;
                            orthoAlaControl.Click += Control_Click;

                            forme.panel1.Controls.Add(pic);
                        }
                    }
                    else if (type == "OrthoResult")
                    {
                        OrthoResult orthoAlaControl = new OrthoResult();

                        orthoAlaControl = orthoAlaControl.ReadFileXML(componentText);

                        XElement? appearance = component.Element("Apparence");
                        if (appearance != null)
                        {
                            int sizeWidth = int.Parse(appearance.Element("SizeWidth")?.Value ?? "100");
                            int sizeHeight = int.Parse(appearance.Element("SizeHeight")?.Value ?? "100");
                            int locationX = int.Parse(appearance.Element("LocationX")?.Value ?? "0");
                            int locationY = int.Parse(appearance.Element("LocationY")?.Value ?? "0");

                            orthoAlaControl.Size = new Size(sizeWidth, sizeHeight);

                            PictureBox pic = new PictureBox
                            {
                                Size = new Size(orthoAlaControl.Size.Width + 10, orthoAlaControl.Size.Height + 10), // Augmenter la taille de 10 pixels
                                Location = new Point(locationX, locationY) // Appliquer la position
                            };

                            orthoAlaControl.Location = new Point(5, 5);


                            pic.Controls.Add(orthoAlaControl);

                            orthoAlaControl.MouseEnter += Control_MouseEnter;
                            pic.MouseLeave += pic_MouseLeave;
                            orthoAlaControl.Click += Control_Click;

                            forme.panel1.Controls.Add(pic);
                        }
                    }
                    else if (type == "OrthoVarname")
                    {
                        OrthoVarname orthoAlaControl = new OrthoVarname();

                        orthoAlaControl = orthoAlaControl.ReadFileXML(componentText);

                        XElement? appearance = component.Element("Apparence");
                        if (appearance != null)
                        {
                            int sizeWidth = int.Parse(appearance.Element("SizeWidth")?.Value ?? "100");
                            int sizeHeight = int.Parse(appearance.Element("SizeHeight")?.Value ?? "100");
                            int locationX = int.Parse(appearance.Element("LocationX")?.Value ?? "0");
                            int locationY = int.Parse(appearance.Element("LocationY")?.Value ?? "0");

                            orthoAlaControl.Size = new Size(sizeWidth, sizeHeight);

                            PictureBox pic = new PictureBox
                            {
                                Size = new Size(orthoAlaControl.Size.Width + 10, orthoAlaControl.Size.Height + 10), // Augmenter la taille de 10 pixels
                                Location = new Point(locationX, locationY)
                            };

                            orthoAlaControl.Location = new Point(5, 5);

                            pic.Controls.Add(orthoAlaControl);

                            orthoAlaControl.MouseEnter += Control_MouseEnter;
                            pic.MouseLeave += pic_MouseLeave;
                            orthoAlaControl.Click += Control_Click;

                            forme.panel1.Controls.Add(pic);
                        }
                    }
                    else if (type == "Reticule")
                    {
                        Reticule orthoAlaControl = new Reticule();

                        orthoAlaControl = orthoAlaControl.ReadFileXML(componentText);

                        XElement? appearance = component.Element("Apparence");
                        if (appearance != null)
                        {
                            int sizeWidth = int.Parse(appearance.Element("SizeWidth")?.Value ?? "100");
                            int sizeHeight = int.Parse(appearance.Element("SizeHeight")?.Value ?? "100");
                            int locationX = int.Parse(appearance.Element("LocationX")?.Value ?? "0");
                            int locationY = int.Parse(appearance.Element("LocationY")?.Value ?? "0");

                            orthoAlaControl.Size = new Size(sizeWidth, sizeHeight);

                            PictureBox pic = new PictureBox
                            {
                                Size = new Size(orthoAlaControl.Size.Width + 10, orthoAlaControl.Size.Height + 10), // Augmenter la taille de 10 pixels
                                Location = new Point(locationX, locationY)
                            };

                            orthoAlaControl.Location = new Point(5, 5);

                            pic.Controls.Add(orthoAlaControl);

                            orthoAlaControl.MouseEnter += Control_MouseEnter;
                            pic.MouseLeave += pic_MouseLeave;
                            orthoAlaControl.Click += Control_Click;

                            forme.panel1.Controls.Add(pic);
                        }
                    }




                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur : {ex.Message}");
            }
        }

        private void Control_MouseEnter(object? sender, EventArgs e)
        {
            EnMoouvement = !EnMoouvement;

            Control? ctrl = sender as Control;

            PictureBox? pic = ctrl?.Parent as PictureBox;

            pic.Cursor = EnMoouvement ? Cursors.SizeAll : Cursors.Default;

            if (IsMdiChild)
            {
                foreach (Control child in pic.Controls)
                {
                    child.MouseDown -= pic_MouseDown;
                    child.MouseMove -= pic_MouseMove;
                    child.MouseUp -= pic_MouseUp;

                    child.MouseDown += pic_MouseDown;
                    child.MouseMove += pic_MouseMove;
                    child.MouseUp += pic_MouseUp;
                }
            }
            else
            {
                foreach (Control child in pic.Controls)
                {

                    child.MouseDown -= pic_MouseDown;
                    child.MouseMove -= pic_MouseMove;
                    child.MouseUp -= pic_MouseUp;

                    child.MouseDown += pic_MouseDown;
                    child.MouseMove += pic_MouseMove;
                    child.MouseUp += pic_MouseUp;

                }
            }
        }


        //private void RecupererContenuTXT(string xmlContent)
        //{
        //    try
        //    {
        //        XElement xml = XElement.Parse(xmlContent);

        //        // Parcourir tous les éléments <Component> du XML
        //        foreach (XElement component in xml.Elements("Component"))
        //        {
        //            string? type = component.Attribute("type")?.Value;

        //            if (type == "AM60")
        //            {
        //                AM60 am60Control = new AM60();
        //                am60Control = am60Control.ReadFile(component.ToString());
        //                Controls.Add(am60Control);
        //            }
        //            else if (type == "CONT1")
        //            {
        //                CONT1 reticuleControl = new CONT1();
        //                reticuleControl = reticuleControl.ReadFileXML(component.ToString());
        //                Controls.Add(reticuleControl);
        //            }
        //            else if (type == "INTEG")
        //            {
        //                ProgressBar pbarControl = new ProgressBar();
        //                pbarControl = pbarControl.ReadFileXML(component.ToString());
        //                Controls.Add(pbarControl);
        //            }
        //            else if (type == "RESULT")
        //            {
        //                Result resultControl = new Result();
        //                resultControl = resultControl.ReadFileXML(component.ToString());
        //                Controls.Add(resultControl);
        //            }
        //            else if (type == "VARNAME")
        //            {
        //                VarName varNameControl = new VarName();
        //                varNameControl = varNameControl.ReadFileXML(component.ToString());
        //                Controls.Add(varNameControl);
        //            }
        //            else if (type == "REL")
        //            {
        //                Relais relaisControl = new Relais();
        //                relaisControl = relaisControl.ReadFileXML(component.ToString());
        //                Controls.Add(relaisControl);
        //            }
        //            else if (type == "RESULT")
        //            {
        //                Result resultControl = new Result();
        //                resultControl = resultControl.ReadFileXML(component.ToString());
        //                Controls.Add(resultControl);
        //            }
        //            else if (type == "LABEL")
        //            {
        //                LabelControl labelControl = new LabelControl();
        //                labelControl = labelControl.ReadFileXML(component.ToString());
        //                Controls.Add(labelControl);
        //            }
        //            else if (type == "DI")
        //            {
        //                DiControl diControl = new DiControl();
        //                diControl = diControl.ReadFileXML(component.ToString());
        //                Controls.Add(diControl);
        //            }
        //            else if (type == "COMBO")
        //            {
        //                ComboBoxControl comboBoxControl = new ComboBoxControl();
        //                comboBoxControl = comboBoxControl.ReadFileXML(component.ToString());
        //                Controls.Add(comboBoxControl);
        //            }
        //            else if (type == "EDIT")
        //            {
        //                EditControl editControl = new EditControl();
        //                editControl = editControl.ReadFileXML(component.ToString());
        //                Controls.Add(editControl);
        //            }
        //            else if (type == "IMAGE")
        //            {
        //                ImageControl imageControl = new ImageControl();
        //                imageControl = imageControl.ReadFileXML(component.ToString());
        //                Controls.Add(imageControl);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show($"Une erreur est survenue lors du traitement du fichier XML : {ex.Message}", "Erreur");
        //    }
        //}


        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string saveMessage = "";
            string saveTitle = "";

            switch (Langue)
            {
                case 1: // English
                    saveMessage = "Do you want to save as XML?";
                    saveTitle = "Save As XML";
                    break;
                case 2: // Chinese
                    saveMessage = "您是否要以XML格式保存？";
                    saveTitle = "保存为XML";
                    break;
                case 3: // German
                    saveMessage = "Möchten Sie als XML speichern?";
                    saveTitle = "Als XML speichern";
                    break;
                case 4: // French
                    saveMessage = "Voulez-vous enregistrer en XML ?";
                    saveTitle = "Enregistrer en XML";
                    break;
                case 5: // Lithuanian
                    saveMessage = "Ar norite išsaugoti kaip XML?";
                    saveTitle = "Išsaugoti kaip XML";
                    break;
            }

            DialogResult saveResult = MessageBox.Show(saveMessage, saveTitle, MessageBoxButtons.YesNo);

            if (saveResult == DialogResult.Yes)
            {
                StringBuilder xmlContent = new StringBuilder();

                xmlContent.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                xmlContent.AppendLine("<Syno name=\"settings\">");

                xmlContent.AppendLine("  <Controls>");

                StringBuilder accumulatedText = SaveAsXML();

                xmlContent.AppendLine(accumulatedText.ToString());
                xmlContent.AppendLine("  </Controls>");

                xmlContent.AppendLine("</Syno>");

                using (StreamWriter writer = new StreamWriter("Save.syno"))
                {
                    writer.Write(xmlContent.ToString());
                }

                MessageBox.Show("Fichier sauvegardé avec succès!", "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {


                StringBuilder accumulatedText = SaveAsTXT();

                using (StreamWriter writer = new StreamWriter("A.syno"))
                {
                    writer.Write(accumulatedText.ToString());
                }

                MessageBox.Show("Fichier sauvegardé avec succès!", "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            string message = "";
            string title = "";

            switch (Langue)
            {
                case 1: // English
                    message = "Save the changes?";
                    title = "Save";
                    break;
                case 2: // Chinese
                    message = "保存更改？";
                    title = "保存";
                    break;
                case 3: // German
                    message = "Änderungen speichern?";
                    title = "Speichern";
                    break;
                case 4: // French
                    message = "Enregistrer les modifications ?";
                    title = "Enregistrer";
                    break;
                case 5: // Lithuanian
                    message = "Išsaugoti pakeitimus?";
                    title = "Išsaugoti";
                    break;
            }

            DialogResult r = MessageBox.Show(message, title, MessageBoxButtons.YesNoCancel);

            if (r == DialogResult.Yes)
            {
                string saveMessage = "";
                string saveTitle = "";

                switch (Langue)
                {
                    case 1: // English
                        saveMessage = "Do you want to save as XML?";
                        saveTitle = "Save As XML";
                        break;
                    case 2: // Chinese
                        saveMessage = "您是否要以XML格式保存？";
                        saveTitle = "保存为XML";
                        break;
                    case 3: // German
                        saveMessage = "Möchten Sie als XML speichern?";
                        saveTitle = "Als XML speichern";
                        break;
                    case 4: // French
                        saveMessage = "Voulez-vous enregistrer en XML ?";
                        saveTitle = "Enregistrer en XML";
                        break;
                    case 5: // Lithuanian
                        saveMessage = "Ar norite išsaugoti kaip XML?";
                        saveTitle = "Išsaugoti kaip XML";
                        break;
                }

                DialogResult saveResult = MessageBox.Show(saveMessage, saveTitle, MessageBoxButtons.YesNo);

                if (saveResult == DialogResult.Yes)
                {
                    ExportFormToXml();
                }
                else
                {
                    ExportFormToTXT();
                }
            }
            else if (r == DialogResult.Cancel)
            {
                e.Cancel = true;
            }
        }


        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string message = "";
            string title = "";


            switch (Langue)
            {
                case 1: // English
                    message = "Would you like to save changes in XML format?";
                    title = "Save as XML";

                    break;
                case 2: // Chinese
                    message = "您是否希望以XML格式保存更改？";
                    title = "保存为XML";

                    break;
                case 3: // German
                    message = "Möchten Sie die Änderungen im XML-Format speichern?";
                    title = "Als XML speichern";

                    break;
                case 4: // French
                    message = "Souhaitez-vous enregistrer les modifications au format XML ?";
                    title = "Enregistrer sous XML";

                    break;
                case 5: // Lithuanian
                    message = "Ar norite išsaugoti pakeitimus XML formatu?";
                    title = "Įrašyti kaip XML";

                    break;
            }

            DialogResult r = MessageBox.Show($"{message}", title, MessageBoxButtons.YesNo);  //ShowCustomMessageBox(message, title, yesText, noText);

            if (r == DialogResult.Yes)
            {
                ExportFormToXml();
            }
            else if (r == DialogResult.No)
            {
                ExportFormToTXT();
                return;
            }
        }

        #endregion

        #region Appliquer la langue

        private void AppliquerLangue()
        {
            switch (Langue)
            {
                case 1: // English
                    btnFile.Text = "File";
                    btnEdition.Text = "Edit";
                    btnView.Text = "View";
                    newToolStripMenuItem.Text = "New";
                    openToolStripMenuItem.Text = "Open";
                    saveToolStripMenuItem.Text = "Save";
                    btnInfos.Text = "Info";

                    // Menu Infos
                    controlCommentToolStripMenuItem.Text = "Control Comment";
                    aboutToolStripMenuItem.Text = "About";

                    // Menu Edit
                    btnEdition.DropDownItems[0].Text = "Cut";
                    btnEdition.DropDownItems[1].Text = "Copy";
                    btnEdition.DropDownItems[2].Text = "Paste";
                    btnEdition.DropDownItems[4].Text = "Delete";
                    btnEdition.DropDownItems[6].Text = "Resize synoptique";
                    btnEdition.DropDownItems[7].Text = "Protect";

                    // Menu View
                    btnView.DropDownItems[0].Text = "Resolution";
                    btnView.DropDownItems[1].Text = "Language";
                    btnView.DropDownItems[2].Text = "Visibility Checker";
                    break;

                case 2: // Chinese
                    btnFile.Text = "文件";
                    btnEdition.Text = "编辑";
                    btnView.Text = "视图";
                    newToolStripMenuItem.Text = "新建";
                    openToolStripMenuItem.Text = "打开";
                    saveToolStripMenuItem.Text = "保存";
                    btnInfos.Text = "信息";

                    // Menu Infos
                    controlCommentToolStripMenuItem.Text = "控制评论";
                    aboutToolStripMenuItem.Text = "关于";

                    // Menu Edit
                    btnEdition.DropDownItems[0].Text = "剪切";
                    btnEdition.DropDownItems[1].Text = "复制";
                    btnEdition.DropDownItems[2].Text = "粘贴";
                    btnEdition.DropDownItems[4].Text = "删除";
                    btnEdition.DropDownItems[6].Text = "调整大小";
                    btnEdition.DropDownItems[7].Text = "保护";

                    // Menu View
                    btnView.DropDownItems[0].Text = "分辨率";
                    btnView.DropDownItems[1].Text = "语言";
                    btnView.DropDownItems[2].Text = "可见性检查";
                    break;

                case 3: // German
                    btnFile.Text = "Datei";
                    btnEdition.Text = "Bearbeiten";
                    btnView.Text = "Ansicht";
                    newToolStripMenuItem.Text = "Neu";
                    openToolStripMenuItem.Text = "Öffnen";
                    saveToolStripMenuItem.Text = "Speichern";
                    btnInfos.Text = "Info";

                    // Menu Infos
                    controlCommentToolStripMenuItem.Text = "Kontrollkommentar";
                    aboutToolStripMenuItem.Text = "Über";

                    // Menu Edit
                    btnEdition.DropDownItems[0].Text = "Ausschneiden";
                    btnEdition.DropDownItems[1].Text = "Kopieren";
                    btnEdition.DropDownItems[2].Text = "Einfügen";
                    btnEdition.DropDownItems[4].Text = "Löschen";
                    btnEdition.DropDownItems[6].Text = "Größe ändern";
                    btnEdition.DropDownItems[7].Text = "Schützen";

                    // Menu View
                    btnView.DropDownItems[0].Text = "Auflösung";
                    btnView.DropDownItems[1].Text = "Sprache";
                    btnView.DropDownItems[2].Text = "Sichtbarkeitsprüfung";
                    break;

                case 4: // French
                    btnFile.Text = "Fichier";
                    btnEdition.Text = "Édition";
                    btnView.Text = "Voir";
                    newToolStripMenuItem.Text = "Nouveau";
                    openToolStripMenuItem.Text = "Ouvrir";
                    saveToolStripMenuItem.Text = "Enregistrer";
                    btnInfos.Text = "Infos";

                    // Menu Infos
                    controlCommentToolStripMenuItem.Text = "Contrôle Commentaire";
                    aboutToolStripMenuItem.Text = "À propos";

                    // Menu Edit
                    btnEdition.DropDownItems[0].Text = "Couper";
                    btnEdition.DropDownItems[1].Text = "Copier";
                    btnEdition.DropDownItems[2].Text = "Coller";
                    btnEdition.DropDownItems[4].Text = "Supprimer";
                    btnEdition.DropDownItems[6].Text = "Redimensionner synoptique";
                    btnEdition.DropDownItems[7].Text = "Protéger";

                    // Menu View
                    btnView.DropDownItems[0].Text = "Résolution";
                    btnView.DropDownItems[1].Text = "Langue";
                    btnView.DropDownItems[2].Text = "Vérifier visibilité";
                    break;

                case 5: // Lithuanian
                    btnFile.Text = "Byla";
                    btnEdition.Text = "Redaguoti";
                    btnView.Text = "Peržiūra";
                    newToolStripMenuItem.Text = "Naujas";
                    openToolStripMenuItem.Text = "Atidaryti";
                    saveToolStripMenuItem.Text = "Išsaugoti";
                    btnInfos.Text = "Informacija";

                    // Menu Infos
                    controlCommentToolStripMenuItem.Text = "Kontrolės komentaras";
                    aboutToolStripMenuItem.Text = "Apie";

                    // Menu Edit
                    btnEdition.DropDownItems[0].Text = "Pjauti";
                    btnEdition.DropDownItems[1].Text = "Kopijuoti";
                    btnEdition.DropDownItems[2].Text = "Įklijuoti";
                    btnEdition.DropDownItems[4].Text = "Ištrinti";
                    btnEdition.DropDownItems[6].Text = "Pakeisti dydį";
                    btnEdition.DropDownItems[7].Text = "Apsaugoti";

                    // Menu View
                    btnView.DropDownItems[0].Text = "Rezoliucija";
                    btnView.DropDownItems[1].Text = "Kalba";
                    btnView.DropDownItems[2].Text = "Matomumo patikrinimas";
                    break;
            }
        }
        #endregion

        #region Responsive

        private void Form1_ClientSizeChanged(object sender, EventArgs e)
        {
            if(this.WindowState == FormWindowState.Minimized)
            {
                return;
            }

            this.MainMenu.Width = this.ClientSize.Width;
            this.MainMenu.Height = (int)(this.ClientSize.Height * 0.08);

            float fontSize = (this.ClientSize.Width * 0.02f + this.ClientSize.Height * 0.02f) / 2;
            foreach (ToolStripMenuItem item in MainMenu.Items)
            {
                item.Font = new Font(item.Font.FontFamily, fontSize);
            }

            pnlViewHost.Location = new Point(0, MainMenu.Bottom);
            pnlViewHost.Width = (int)(this.ClientSize.Width * 0.8);
            pnlViewHost.Height = this.ClientSize.Height - pnlViewHost.Top;

            lstToolbox.Width = (int)(pnlViewHost.Width * 0.3);
            lstToolbox.Height = pnlViewHost.Height / 2;
            lstToolbox.Location = new Point(pnlViewHost.Right, pnlViewHost.Top);

            propertyGrid1.Width = lstToolbox.Width;
            propertyGrid1.Height = pnlViewHost.Height - lstToolbox.Height;
            propertyGrid1.Location = new Point(lstToolbox.Left, lstToolbox.Bottom);
        }

        #endregion

        #region lstToolbox

        private void Initialize()
        {
            lstToolbox.Items.Clear();
            lstToolbox.Items.AddRange(new string[]
            {
                "AM60",
                "Cont1",
                "INTEG",
                "OrthoAD",
                "OrthoAla",
                "OrthoCMDLib",
                "OrthoCombo",
                "OrthoDI",
                "OrthoEdit",
                "Ortholmage",
                "OrthoLabel",
                "OrthoPbar",
                "OrthoRel",
                "OrthoResult",
                "OrthoVarname",
                "Reticule"
            });

        }

        private void lstToolbox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstToolbox.SelectedItem != null)
            {
                ControlSelectionner = lstToolbox.SelectedItem.ToString();

                this.Cursor = Cursors.Cross;
            }
        }

        #endregion

        #region PnlView
        private void pnlViewHost_Click(object sender, MouseEventArgs e)
        {
            if (Aligner)
            {
                return;
            }
            if (this.Cursor == DefaultCursor)
            {
                foreach (Control control in forme.panel1.Controls)
                {
                    if (control is PictureBox pic)
                    {
                        pic.Paint -= pic_Paint;

                        PictureBoxSelectonner = "";

                        pic.Invalidate();
                    }
                }
                return;
            }

            Control? Ctrl = null;

            switch (ControlSelectionner)
            {
                case "AM60":
                    Ctrl = new AM60();
                    break;

                case "Cont1":
                    Ctrl = new CONT1();
                    break;

                case "INTEG":
                    Ctrl = new INTEG();
                    break;

                case "OrthoAD":
                    Ctrl = new OrthoAD();
                    break;

                case "OrthoAla":
                    Ctrl = new OrthoAla();
                    break;

                case "OrthoCMDLib":
                    Ctrl = new OrthoCMDLib();
                    break;

                case "OrthoCombo":
                    Ctrl = new OrthoCombo();
                    break;

                case "OrthoDI":
                    Ctrl = new OrthoDI();
                    break;

                case "OrthoEdit":
                    Ctrl = new OrthoEdit();
                    break;

                case "Ortholmage":
                    Ctrl = new OrthoImage();
                    break;

                case "OrthoLabel":
                    Ctrl = new OrthoLabel();
                    break;

                case "OrthoPbar":
                    Ctrl = new OrthoPbar();
                    break;

                case "OrthoRel":
                    Ctrl = new OrthoRel();
                    break;

                case "OrthoResult":
                    Ctrl = new OrthoResult();
                    break;

                case "OrthoVarname":
                    Ctrl = new OrthoVarname();
                    break;

                case "Reticule":
                    Ctrl = new Reticule();
                    break;

                default:
                    return;
            }

            if (Ctrl != null)
            {
                PictureBox pic = new PictureBox
                {
                    Size = new Size(Ctrl.Width + 10, Ctrl.Height + 10),
                    Location = new Point(e.X - 5, e.Y - 5)
                };

                pic.Paint += pic_Paint;

                Ctrl.MouseEnter += Control_MouseEnter;

                Ctrl.Location = new Point(5, 5);
                pic.Controls.Add(Ctrl);

                pic.MouseLeave += pic_MouseLeave;

                forme.panel1.Controls.Add(pic);

                this.Cursor = DefaultCursor;

                Ctrl.Click += Control_Click;

                Ctrl.MouseClick += Control_MouseClick;
            }
        }
        private void Control_MouseClick(object? sender, MouseEventArgs e)
        {
            Control? Con = sender as Control;
            PictureBox? parentPictureBox = Con.Parent as PictureBox;

            if (e.Button == MouseButtons.Left && (Control.ModifierKeys & Keys.Control) == Keys.Control)
            {
                if (peutViderListe)
                {
                    listPic.Clear();
                }

                Aligner = true;

                if (Con != null)
                {
                    if (parentPictureBox != null)
                    {
                        peutViderListe = false;

                        this.listPic.Add(parentPictureBox);
                    }
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                if (Aligner)
                {
                    ContextMenuStrip contextMenu = new ContextMenuStrip();

                    contextMenu.Items.Add("Horizontalement", null, Horizontale);

                    contextMenu.Items.Add("Verticalement", null, Verticale);

                    if (parentPictureBox != null)
                        contextMenu.Show(parentPictureBox, e.Location);
                }
            }
            else
            {
                listPic.Clear();
            }
        }

        private void Verticale(object? sender, EventArgs e)
        {
            peutViderListe = true;
            Aligner = false;

            int xPosition = listPic[0].Location.X;
            int spacing = listPic[0].Location.Y;

            foreach (PictureBox c in listPic)
            {
                c.Location = new Point(xPosition, spacing);
                spacing += c.Height + 10;

                c.Paint -= pic_Paint;
            }

            listPic.Clear();
        }

        private void Horizontale(object? sender, EventArgs e)
        {
            int yPosition = listPic[0].Location.Y;
            int Espace = listPic[0].Location.X;

            peutViderListe = true;
            Aligner = false;

            foreach (PictureBox c in listPic)
            {
                c.Location = new Point(Espace, yPosition);
                Espace += c.Width + 10;

                c.Paint -= pic_Paint;
            }

            listPic.Clear();
        }

        private void pic_MouseLeave(object? sender, EventArgs e)
        {
            PictureBox? p = sender as PictureBox;
            foreach (Control ctrl in p.Controls)
            {
                p.Size = ctrl.Size;

                p.Width += 10;
                p.Height += 10;
            }

            p.Paint -= pic_Paint;
            p.Invalidate();

            this.Cursor = DefaultCursor;

            this.Bouger = false;
            this.EnMoouvement = false;

        }

        private void Control_Click(object? sender, EventArgs e)
        {
            Control? controle = sender as Control;

            PictureBox? pic = controle?.Parent as PictureBox;

            if (pic != null)
            {
                pic.MouseDown += pic_MouseDown2;
                pic.MouseMove += pic_MouseMove2;
                pic.MouseUp += pic_MouseUp2;

                pic.Paint += pic_Paint;
                pic.Invalidate();

                //if (controle is AM60 am60Control)
                //{

                //    propertyGrid1.SelectedObject = am60Control;

                //}
                //else if (controle is CONT1 cont1Control)
                //{
                //    propertyGrid1.SelectedObject = cont1Control;
                //}
                //else if (controle is INTEG integControl)
                //{
                //    propertyGrid1.SelectedObject = integControl;
                //}
                //else if (controle is OrthoAD orthoADControl)
                //{
                //    propertyGrid1.SelectedObject = orthoADControl;
                //}
                //else if (controle is OrthoAla orthoAlaControl)
                //{
                //    propertyGrid1.SelectedObject = orthoAlaControl;
                //}
                //else if (controle is OrthoCMDLib orthoCMDLibControl)
                //{
                //    propertyGrid1.SelectedObject = orthoCMDLibControl;
                //}
                //else if (controle is OrthoCombo orthoComboControl)
                //{
                //    propertyGrid1.SelectedObject = orthoComboControl;
                //}
                //else if (controle is OrthoDI orthoDIControl)
                //{
                //    propertyGrid1.SelectedObject = orthoDIControl;
                //}
                //else if (controle is OrthoEdit orthoEditControl)
                //{
                //    propertyGrid1.SelectedObject = orthoEditControl;
                //}
                //else if (controle is OrthoImage orthoImageControl)
                //{
                //    propertyGrid1.SelectedObject = orthoImageControl;
                //}
                //else if (controle is OrthoLabel orthoLabelControl)
                //{
                //    propertyGrid1.SelectedObject = orthoLabelControl;
                //}
                //else if (controle is OrthoPbar orthoPbarControl)
                //{
                //    propertyGrid1.SelectedObject = orthoPbarControl;
                //}
                //else if (controle is OrthoRel orthoRelControl)
                //{
                //    propertyGrid1.SelectedObject = orthoRelControl;
                //}
                //else if (controle is OrthoResult orthoResultControl)
                //{
                //    propertyGrid1.SelectedObject = orthoResultControl;
                //}
                //else if (controle is OrthoVarname orthoVarnameControl)
                //{
                //    propertyGrid1.SelectedObject = orthoVarnameControl;
                //}
                //else if (controle is Reticule reticuleControl)
                //{
                //    propertyGrid1.SelectedObject = reticuleControl;
                //}
                //else
                //{
                //    propertyGrid1.SelectedObject = forme;
                //}

                propertyGrid1.SelectedObject = controle;
            }
        }

        #endregion

        #region Mouse

        private void pic_MouseDown(object sender, MouseEventArgs e)
        {
            if (!EnMoouvement) return;

            PictureBox? picturebox = (sender as PictureBox) ?? (sender as Control)?.Parent as PictureBox;

            if (picturebox != null)
            {
                Bouger = true;
                SourisDecalage = e.Location;
            }
        }

        private void pic_MouseMove(object sender, MouseEventArgs e)
        {
            if (!Bouger) return;

            PictureBox? pictureBox = (sender as PictureBox) ?? (sender as Control)?.Parent as PictureBox;

            if (pictureBox != null)
            {
                pictureBox.Left += e.X - SourisDecalage.X;
                pictureBox.Top += e.Y - SourisDecalage.Y;
            }
        }

        private void pic_MouseUp(object sender, MouseEventArgs e)
        {
            Bouger = false;
        }



        private void pic_MouseDown2(object sender, MouseEventArgs e)
        {
            PictureBox? pictureBox = sender as PictureBox;

            if (pictureBox != null)
            {
                if (IsNearBorder(e.Location, pictureBox))
                {
                    ChangerPicture = pictureBox;
                    SourisDecalage = e.Location;
                    Changement = true;
                    pictureBox.Cursor = Cursors.SizeNWSE; 
                }
            }
        }

        private void pic_MouseMove2(object sender, MouseEventArgs e)
        {
            PictureBox? pic = sender as PictureBox;

            if (Changement && ChangerPicture != null)
            {
                int deltaX = e.X - SourisDecalage.X;
                int deltaY = e.Y - SourisDecalage.Y;

                ChangerPicture.Width = Math.Max(10, ChangerPicture.Width + deltaX);
                ChangerPicture.Height = Math.Max(10, ChangerPicture.Height + deltaY);

                if (ChangerPicture.Controls.Count > 0)
                {
                    Control child = ChangerPicture.Controls[0];
                    child.Width = ChangerPicture.Width;
                    child.Height = ChangerPicture.Height;
                }

                SourisDecalage = e.Location;

                ChangerPicture.Invalidate();
            }
            else
            {
                if (pic != null && IsNearBorder(e.Location, pic))
                {
                    pic.Cursor = Cursors.SizeNWSE; 
                }
                else
                {
                    pic.Cursor = Cursors.Default;
                }
            }
        }

        private void pic_MouseUp2(object sender, MouseEventArgs e)
        {
            ChangerPicture = null;
            Changement = false;

            PictureBox? pic = sender as PictureBox;
            if (pic != null)
            {
                pic.Cursor = Cursors.Default;
            }
        }



        #endregion

        #region Border

        private bool IsNearBorder(Point mousePosition, PictureBox pic)
        {
            int borderDistance = 10; 
            return mousePosition.X >= pic.Width - borderDistance ||
                   mousePosition.X <= borderDistance ||
                   mousePosition.Y >= pic.Height - borderDistance ||
                   mousePosition.Y <= borderDistance;
        }

        private void pic_Paint(object sender, PaintEventArgs e)
        {
            PictureBox? pic = sender as PictureBox;

            int pictureBoxCount = forme.panel1.Controls.OfType<PictureBox>().Count();

            pic.Name = "PictureBox" + (pictureBoxCount + 1);  

            PictureBoxSelectonner = pic.Name;

            if (pic != null)
            {
                using (Pen pen = new Pen(Color.Black))
                {
                    pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot; // Bordure en pointillés
                    e.Graphics.DrawRectangle(pen, 0, 0, pic.Width - 1, pic.Height - 1);

                }
            }
        }

        #endregion
        private void controlCommentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(PictureBoxSelectonner))
            {
                MessageBox.Show("Aucun element sélectionné.");
                return;
            }

            Control? control = null;

            foreach (PictureBox ctrl in forme.panel1.Controls)
            {
                if (ctrl.Name == PictureBoxSelectonner)
                {
                    foreach (Control childControl in ctrl.Controls)
                    {
                        Type controlType = childControl.GetType();

                        if (childControl.Name == ControlSelectionner)
                        {
                            control = childControl;
                            break;
                        }
                    }

                    if (control != null)
                    {
                        var commentWindow = new ControlComment();
                        commentWindow.ShowDialog();

                        string commentaire = ControlComment.commentaire;

                        object? Control = Activator.CreateInstance(control.GetType());

                        if (Control is Control newCtrl)
                        {
                            newCtrl.GetType().GetProperty("Comment")?.SetValue(newCtrl, commentaire);
                        }

                        return;
                    }
                    else
                    {
                        MessageBox.Show("Aucun contrôle trouvé dans le PictureBox sélectionné.");
                        return;
                    }
                }
            }

            MessageBox.Show("Aucune PictureBox correspondante n'a été trouvée.");
        }

        private void newFormeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pnlViewHost.BorderStyle = BorderStyle.FixedSingle;

            FormVide tmp = new FormVide();

            tmp.Location = new Point(0, 0);

            tmp.panel1.MouseClick += pnlViewHost_Click;

            AfficherFormDansPanel(tmp, pnlViewHost);
        }

        private void propertyGrid1_PropertyValueChanged(object sender, PropertyValueChangedEventArgs e)
        {
            Control? objet = (Control)sender;
            PictureBox? pic = objet.Parent as PictureBox;

            if (objet != null && pic != null)
            {
                pic.Size = new Size(objet.Size.Width + 10, objet.Size.Height + 10);
            }
        }
    }
}
