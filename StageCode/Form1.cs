using StageCode.LIB;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using OrthoDesigner.Other;
using System.Xml.Linq;
using System.Diagnostics;
using System.ComponentModel.Design;
using System.Threading.Channels;
using System.Windows.Forms.VisualStyles;
using Microsoft.VisualBasic;
using System.Windows.Forms;
using StageCode.Other;
using System;
using OrthoDesigner;

namespace StageCode
{
    public partial class Form1 : Form
    {
        public static int Langue = 1; // 1 = English, 2 = Chinese, 3 = German, 4 = French, 5 = Lithuanian
        private Form1 frm;

        FormVide forme;

        private string selectedControl = "";

        private string SelectedPictureBox = "";

        private PictureBox? resizingFrame = null;
        private Point mouseOffset;
        private bool isResizing = false;
        private bool isMoving = false;

        // Variables globales
        private bool isMovable = false;

        //A corriger
        //TabName a faire

        public Form1()
        {
            InitializeComponent();

            this.forme = new FormVide();

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


            //this.WindowState = FormWindowState.Maximized;
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

            forme.panel1.Controls.Clear();

        }


        private void Couper(object sender, EventArgs e)
        {
            // Trouver la PictureBox sélectionnée
            PictureBox? frame = forme.panel1.Controls.OfType<PictureBox>()
                                              .FirstOrDefault(p => p.Name == SelectedPictureBox);

            if (frame != null)
            {
                // Liste des informations des contrôles contenus dans la PictureBox
                List<SerializableControl> controlsData = new List<SerializableControl>();

                foreach (Control ctrl in frame.Controls)
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
                frame.Controls.Clear();
                forme.panel1.Controls.Remove(frame);
            }
        }






        private void Copier(object sender, EventArgs e)
        {
            // Trouver la PictureBox sélectionnée
            PictureBox? frame = forme.panel1.Controls.OfType<PictureBox>()
                                              .FirstOrDefault(p => p.Name == SelectedPictureBox);

            if (frame != null)
            {
                // Liste des informations des contrôles contenus dans la PictureBox
                List<SerializableControl> controlsData = new List<SerializableControl>();

                foreach (Control ctrl in frame.Controls)
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
                    PictureBox newFrame = new PictureBox
                    {
                        BorderStyle = BorderStyle.None,
                        BackColor = Color.White,
                        Size = new Size(200, 200), // Taille par défaut, peut être ajustée
                        Location = mousePosition, // Position à l'endroit du clic
                        AllowDrop = true // Pour gérer éventuellement le drag & drop
                    };

                    // Ajouter les événements à newFrame (et non à un autre contrôle)
                    newFrame.Paint += Frame_Paint;
                    newFrame.Click += NewControl_Click;

                    // Restaurer les contrôles dans la nouvelle PictureBox
                    foreach (var controlData in controlsData)
                    {
                        Type? controlType = Type.GetType(controlData.TypeName);
                        if (controlType != null)
                        {
                            Control newControl = (Control)Activator.CreateInstance(controlType)!;
                            newControl.Name = controlData.Name;
                            newControl.Location = new Point(controlData.X, controlData.Y);
                            newControl.Size = new Size(controlData.Width, controlData.Height);
                            if (newControl is TextBox textBox) textBox.Text = controlData.Text;
                            else newControl.Text = controlData.Text;

                            newFrame.Controls.Add(newControl);
                        }
                    }

                    // Ajouter la nouvelle PictureBox à pnlViewHost
                    forme.panel1.Controls.Add(newFrame);
                    newFrame.Invalidate(); // Redessiner
                    SelectedPictureBox = "";
                }
            }
        }

        private void Supprimer(object sender, EventArgs e)
        {
            PictureBox? frame = forme.panel1.Controls.OfType<PictureBox>()
                                      .FirstOrDefault(p => p.Name == SelectedPictureBox);

            if (frame != null)
            {
                forme.panel1.Controls.Remove(frame);
                frame.Dispose(); // Libérer la mémoire utilisée par la PictureBox
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

                Form1_ClientSizeChanged(new object(), new EventArgs());
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

            // Affichage du message avec le titre et la langue correspondante
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

                            // Définir la taille du contrôle AM60
                            am60Control.Size = new Size(sizeWidth, sizeHeight);

                            // Créer une PictureBox pour contenir le contrôle
                            PictureBox frame = new PictureBox
                            {
                                Size = new Size(am60Control.Size.Width + 10, am60Control.Size.Height + 10), // Augmenter la taille de 10 pixels
                                Location = new Point(locationX, locationY) // Appliquer la position
                            };


                            frame.MouseLeave += Frame_MouseLeave;

                            // Ajouter le contrôle AM60 à la PictureBox
                            frame.Controls.Add(am60Control);

                            am60Control.Click += NewControl_Click;
                            am60Control.MouseEnter += NewControl_MouseEnter;

                            // Ajouter la PictureBox au conteneur principal
                            forme.panel1.Controls.Add(frame);
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
                            PictureBox frame = new PictureBox
                            {
                                Size = new Size(cont1Control.Size.Width + 10, cont1Control.Size.Height + 10), // Augmenter la taille de 10 pixels
                                Location = new Point(locationX, locationY) // Appliquer la position
                            };

                            // Ajouter le contrôle Cont1 à la PictureBox
                            frame.Controls.Add(cont1Control);

                            cont1Control.MouseEnter += NewControl_MouseEnter;
                            frame.MouseLeave += Frame_MouseLeave;

                            cont1Control.Click += NewControl_Click;


                            // Ajouter la PictureBox au conteneur principal
                            forme.panel1.Controls.Add(frame);
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
                            PictureBox frame = new PictureBox
                            {
                                Size = new Size(integControl.Size.Width + 10, integControl.Size.Height + 10), // Augmenter la taille de 10 pixels
                                Location = new Point(locationX, locationY) // Appliquer la position
                            };

                            // Ajouter le contrôle INTEG à la PictureBox
                            frame.Controls.Add(integControl);

                            // Ajouter des gestionnaires d'événements à l'objet INTEG si nécessaire
                            integControl.MouseEnter += NewControl_MouseEnter;
                            frame.MouseLeave += Frame_MouseLeave;

                            integControl.Click += NewControl_Click;

                            // Ajouter la PictureBox au conteneur principal
                            forme.panel1.Controls.Add(frame);
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
                            PictureBox frame = new PictureBox
                            {
                                Size = new Size(orthoADControl.Size.Width + 10, orthoADControl.Size.Height + 10), // Augmenter la taille de 10 pixels
                                Location = new Point(locationX, locationY) // Appliquer la position
                            };

                            // Ajouter le contrôle OrthoAD à la PictureBox
                            frame.Controls.Add(orthoADControl);

                            // Ajouter des gestionnaires d'événements à l'objet OrthoAD si nécessaire
                            orthoADControl.MouseEnter += NewControl_MouseEnter;
                            frame.MouseLeave += Frame_MouseLeave;

                            orthoADControl.Click += NewControl_Click;

                            // Ajouter la PictureBox au conteneur principal
                            forme.panel1.Controls.Add(frame);
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
                            PictureBox frame = new PictureBox
                            {
                                Size = new Size(orthoAlaControl.Size.Width + 10, orthoAlaControl.Size.Height + 10), // Augmenter la taille de 10 pixels
                                Location = new Point(locationX, locationY) // Appliquer la position
                            };

                            // Ajouter le contrôle OrthoAla à la PictureBox
                            frame.Controls.Add(orthoAlaControl);

                            // Ajouter des gestionnaires d'événements à l'objet OrthoAla si nécessaire
                            orthoAlaControl.MouseEnter += NewControl_MouseEnter;
                            frame.MouseLeave += Frame_MouseLeave;
                            orthoAlaControl.Click += NewControl_Click;

                            // Ajouter la PictureBox au conteneur principal
                            forme.panel1.Controls.Add(frame);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Gestion des erreurs
                MessageBox.Show($"Erreur : {ex.Message}");
            }
        }

        private void NewControl_MouseEnter(object? sender, EventArgs e)
        {
            // Activer ou désactiver le mode déplacement
            isMovable = !isMovable;

            // Récupérer le contrôle qui a déclenché l'événement
            Control? ctrl = sender as Control;

            // Vérifier si le contrôle a un parent de type PictureBox
            PictureBox? frame = ctrl?.Parent as PictureBox;

            // Modifier le curseur selon l'état du déplacement
            frame.Cursor = isMovable ? Cursors.SizeAll : Cursors.Default;

            if (IsMdiChild)
            {
                // Ajouter les événements aux enfants pour permettre le déplacement
                foreach (Control child in frame.Controls)
                {
                    child.MouseDown -= Frame_MouseDown;
                    child.MouseMove -= Frame_MouseMove;
                    child.MouseUp -= Frame_MouseUp;

                    child.MouseDown += Frame_MouseDown;
                    child.MouseMove += Frame_MouseMove;
                    child.MouseUp += Frame_MouseUp;
                }
            }
            else
            {
                // Ajouter les événements aux enfants pour permettre le déplacement
                foreach (Control child in frame.Controls)
                {

                    child.MouseDown -= Frame_MouseDown;
                    child.MouseMove -= Frame_MouseMove;
                    child.MouseUp -= Frame_MouseUp;

                    child.MouseDown += Frame_MouseDown;
                    child.MouseMove += Frame_MouseMove;
                    child.MouseUp += Frame_MouseUp;

                }
            }
        }


        private void RecupererContenuTXT(string xmlContent)
        {
            //try
            //{
            //    XElement xml = XElement.Parse(xmlContent);

            //    // Parcourir tous les éléments <Component> du XML
            //    foreach (XElement component in xml.Elements("Component"))
            //    {
            //        string? type = component.Attribute("type")?.Value;

            //        if (type == "AM60")
            //        {
            //            AM60 am60Control = new AM60();
            //            am60Control = am60Control.ReadFileXML(component.ToString());
            //            Controls.Add(am60Control);
            //        }
            //        else if (type == "CONT1")
            //        {
            //            CONT1 reticuleControl = new CONT1();
            //            reticuleControl = reticuleControl.ReadFileXML(component.ToString());
            //            Controls.Add(reticuleControl);
            //        }
            //        else if (type == "INTEG")
            //        {
            //            ProgressBar pbarControl = new ProgressBar();
            //            pbarControl = pbarControl.ReadFileXML(component.ToString());
            //            Controls.Add(pbarControl);
            //        }
            //        else if (type == "RESULT")
            //        {
            //            Result resultControl = new Result();
            //            resultControl = resultControl.ReadFileXML(component.ToString());
            //            Controls.Add(resultControl);
            //        }
            //        else if (type == "VARNAME")
            //        {
            //            VarName varNameControl = new VarName();
            //            varNameControl = varNameControl.ReadFileXML(component.ToString());
            //            Controls.Add(varNameControl);
            //        }
            //        else if (type == "REL")
            //        {
            //            Relais relaisControl = new Relais();
            //            relaisControl = relaisControl.ReadFileXML(component.ToString());
            //            Controls.Add(relaisControl);
            //        }
            //        else if (type == "RESULT")
            //        {
            //            Result resultControl = new Result();
            //            resultControl = resultControl.ReadFileXML(component.ToString());
            //            Controls.Add(resultControl);
            //        }
            //        else if (type == "LABEL")
            //        {
            //            LabelControl labelControl = new LabelControl();
            //            labelControl = labelControl.ReadFileXML(component.ToString());
            //            Controls.Add(labelControl);
            //        }
            //        else if (type == "DI")
            //        {
            //            DiControl diControl = new DiControl();
            //            diControl = diControl.ReadFileXML(component.ToString());
            //            Controls.Add(diControl);
            //        }
            //        else if (type == "COMBO")
            //        {
            //            ComboBoxControl comboBoxControl = new ComboBoxControl();
            //            comboBoxControl = comboBoxControl.ReadFileXML(component.ToString());
            //            Controls.Add(comboBoxControl);
            //        }
            //        else if (type == "EDIT")
            //        {
            //            EditControl editControl = new EditControl();
            //            editControl = editControl.ReadFileXML(component.ToString());
            //            Controls.Add(editControl);
            //        }
            //        else if (type == "IMAGE")
            //        {
            //            ImageControl imageControl = new ImageControl();
            //            imageControl = imageControl.ReadFileXML(component.ToString());
            //            Controls.Add(imageControl);
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show($"Une erreur est survenue lors du traitement du fichier XML : {ex.Message}", "Erreur");
            //}
        }


        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
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

                // Ouvrir un stream pour écrire dans le fichier choisi
                using (StreamWriter writer = new StreamWriter("Save.syno"))
                {
                    // Écrire le texte XML accumulé dans le fichier
                    writer.Write(xmlContent.ToString());
                }

                // Message de confirmation
                MessageBox.Show("Fichier sauvegardé avec succès!", "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                // Créer un StringBuilder pour accumuler le texte de tous les contrôles


                // Appeler la méthode SaveAs pour accumuler tous les contrôles dans xmlContent
                StringBuilder accumulatedText = SaveAsTXT(); // Récupère le texte accumulé des contrôles

                using (StreamWriter writer = new StreamWriter("A.syno"))
                {
                    // Écrire le texte XML accumulé dans le fichier
                    writer.Write(accumulatedText.ToString());
                }

                // Message de confirmation
                MessageBox.Show("Fichier sauvegardé avec succès!", "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            string message = "";
            string title = "";

            // Message de confirmation pour sauvegarder les changements
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
                // Si l'utilisateur choisit "Annuler", empêcher la fermeture du formulaire
                e.Cancel = true;
            }
        }


        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Déclaration des variables
            string message = "";
            string title = "";


            // Définir les messages et les textes des boutons en fonction de la langue
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

            // Affichage d'un MessageBox personnalisé avec des boutons textuels en fonction de la langue
            DialogResult r = MessageBox.Show($"{message}", title, MessageBoxButtons.YesNo);  //ShowCustomMessageBox(message, title, yesText, noText);

            if (r == DialogResult.Yes)
            {
                // Si l'utilisateur choisit "Oui", alors enregistrer le fichier XML
                ExportFormToXml();
            }
            else if (r == DialogResult.No)
            {
                ExportFormToTXT();
                // Si l'utilisateur choisit "Non", ne rien faire
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
            this.MainMenu.Width = this.ClientSize.Width;
            this.MainMenu.Height = (int)(this.ClientSize.Height * 0.08);

            float fontSize = (this.ClientSize.Width * 0.02f + this.ClientSize.Height * 0.02f) / 2;
            foreach (ToolStripMenuItem item in MainMenu.Items)
            {
                item.Font = new Font(item.Font.FontFamily, fontSize);
            }

            // Adapter pnlViewHost pour occuper tout l'espace en dessous du menu
            pnlViewHost.Location = new Point(0, MainMenu.Bottom);
            pnlViewHost.Width = (int)(this.ClientSize.Width * 0.8);
            pnlViewHost.Height = this.ClientSize.Height - pnlViewHost.Top;

            // Ajuster lstToolbox (à gauche du pnlViewHost)
            lstToolbox.Width = (int)(pnlViewHost.Width * 0.3);
            lstToolbox.Height = pnlViewHost.Height / 2; // moitié de pnlViewHost
            lstToolbox.Location = new Point(pnlViewHost.Right, pnlViewHost.Top);

            // Ajuster propertyGrid1 (sous lstToolbox, même largeur)
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
                selectedControl = lstToolbox.SelectedItem.ToString();

                this.Cursor = Cursors.Cross;
            }
        }

        #endregion

        #region PnlView

        private void pnlViewHost_Click(object sender, MouseEventArgs e)
        {
            // Si le curseur est dans son état par défaut, on désactive les bordures pointillées sur toutes les PictureBox
            if (this.Cursor == DefaultCursor)
            {
                // Parcours toutes les PictureBox dans le pnlViewHost et supprime les bordures pointillées
                foreach (Control control in forme.panel1.Controls)
                {
                    if (control is PictureBox frame)
                    {
                        // Supprimer le handler d'événement Paint pour ne plus dessiner la bordure
                        frame.Paint -= Frame_Paint;

                        SelectedPictureBox = "";

                        frame.Invalidate();
                    }
                }
                return; // Sortir de la méthode si le curseur est par défaut
            }

            // Si un contrôle est sélectionné, on procède à l'ajout
            Control? newControl = null;

            // Vérifier si un contrôle est sélectionné avant de créer un nouveau contrôle
            switch (selectedControl)
            {
                case "AM60":
                    newControl = new AM60();
                    break;

                case "Cont1":
                    newControl = new CONT1();
                    break;

                case "INTEG":
                    newControl = new INTEG();
                    break;

                case "OrthoAD":
                    newControl = new OrthoAD();
                    break;

                case "OrthoAla":
                    newControl = new OrthoAla();
                    break;

                case "OrthoCMDLib":
                    newControl = new OrthoCMDLib();
                    break;

                case "OrthoCombo":
                    newControl = new OrthoCombo();
                    break;

                case "OrthoDI":
                    newControl = new OrthoDI();
                    break;

                case "OrthoEdit":
                    newControl = new OrthoEdit();
                    break;

                case "Ortholmage":
                    newControl = new OrthoImage();
                    break;

                case "OrthoLabel":
                    newControl = new OrthoLabel();
                    break;

                case "OrthoPbar":
                    newControl = new OrthoPbar();
                    break;

                case "OrthoRel":
                    newControl = new OrthoRel();
                    break;

                case "OrthoResult":
                    newControl = new OrthoResult();
                    break;

                case "OrthoVarname":
                    newControl = new OrthoVarname();
                    break;

                case "Reticule":
                    newControl = new Reticule();
                    break;

                default:
                    return;
            }

            if (newControl != null)
            {
                // Création de la PictureBox qui servira de cadre
                PictureBox frame = new PictureBox
                {
                    Size = new Size(newControl.Width + 10, newControl.Height + 10), // Ajuste la taille
                    Location = new Point(e.X - 5, e.Y - 5) // Position ajustée par rapport au clic
                };

                // Gérer le dessin personnalisé pour la bordure en pointillés
                frame.Paint += Frame_Paint;

                newControl.MouseEnter += NewControl_MouseEnter;

                // Ajouter le contrôle à l'intérieur de la PictureBox
                newControl.Location = new Point(5, 5);
                frame.Controls.Add(newControl);

                frame.MouseLeave += Frame_MouseLeave;

                // frame.Bufer
                // Ajouter la PictureBox au conteneur principal
                forme.panel1.Controls.Add(frame);

                // Réinitialiser le curseur
                this.Cursor = DefaultCursor;

                newControl.Click += NewControl_Click;
            }
        }

        private void Frame_MouseLeave(object? sender, EventArgs e)
        {
            PictureBox? p = sender as PictureBox;
            foreach (Control ctrl in p.Controls)
            {
                p.Size = ctrl.Size;

                p.Width += 10;
                p.Height += 10;
            }

            p.Paint -= Frame_Paint;
            p.Invalidate();

            this.Cursor = DefaultCursor;

            this.isMoving = false;
            this.isMovable = false;

        }

        private void NewControl_Click(object? sender, EventArgs e)
        {
            Control? controle = sender as Control;

            // Vérifier si le contrôle est un contrôle enfant d'une PictureBox
            PictureBox? frame = controle?.Parent as PictureBox;

            if (frame != null)
            {
                // Ajouter des gestionnaires d'événements pour déplacer ou redimensionner
                frame.MouseDown += Frame_MouseDown2;
                frame.MouseMove += Frame_MouseMove2;
                frame.MouseUp += Frame_MouseUp2;

                // Ajouter la gestion du dessin des bordures pointillées
                frame.Paint += Frame_Paint;
                frame.Invalidate(); // Cela va déclencher l'événement Paint pour redessiner

                // Si un contrôle est sélectionné, afficher ses propriétés dans le PropertyGrid
                if (controle is AM60 am60Control)
                {
                    // Mettre à jour le PropertyGrid avec l'objet AM60 sélectionné
                    propertyGrid1.SelectedObject = am60Control;
                }
                else if (controle is CONT1 cont1Control)
                {
                    propertyGrid1.SelectedObject = cont1Control;
                }
                else if (controle is INTEG integControl)
                {
                    propertyGrid1.SelectedObject = integControl;
                }
                else if (controle is OrthoAD orthoADControl)
                {
                    propertyGrid1.SelectedObject = orthoADControl;
                }
                else if (controle is OrthoAla orthoAlaControl)
                {
                    propertyGrid1.SelectedObject = orthoAlaControl;
                }
                else if (controle is OrthoCMDLib orthoCMDLibControl)
                {
                    propertyGrid1.SelectedObject = orthoCMDLibControl;
                }
                else if (controle is OrthoCombo orthoComboControl)
                {
                    propertyGrid1.SelectedObject = orthoComboControl;
                }
                else if (controle is OrthoDI orthoDIControl)
                {
                    propertyGrid1.SelectedObject = orthoDIControl;
                }
                else if (controle is OrthoEdit orthoEditControl)
                {
                    propertyGrid1.SelectedObject = orthoEditControl;
                }
                else if (controle is OrthoImage orthoImageControl)
                {
                    propertyGrid1.SelectedObject = orthoImageControl;
                }
                else if (controle is OrthoLabel orthoLabelControl)
                {
                    propertyGrid1.SelectedObject = orthoLabelControl;
                }
                else if (controle is OrthoPbar orthoPbarControl)
                {
                    propertyGrid1.SelectedObject = orthoPbarControl;
                }
                else if (controle is OrthoRel orthoRelControl)
                {
                    propertyGrid1.SelectedObject = orthoRelControl;
                }
                else if (controle is OrthoResult orthoResultControl)
                {
                    propertyGrid1.SelectedObject = orthoResultControl;
                }
                else if (controle is OrthoVarname orthoVarnameControl)
                {
                    propertyGrid1.SelectedObject = orthoVarnameControl;
                }
                else if (controle is Reticule reticuleControl)
                {
                    propertyGrid1.SelectedObject = reticuleControl;
                }

                // Ajouter des vérifications pour les autres types de contrôles

                // Réinitialiser le curseur
                this.Cursor = Cursors.Default;
            }
        }

        #endregion

        #region Mouse

        private void Frame_MouseDown(object sender, MouseEventArgs e)
        {
            // Vérifier que le déplacement est activé
            if (!isMovable) return;

            // Récupérer la PictureBox, même si on clique sur un élément à l’intérieur
            PictureBox? frame = (sender as PictureBox) ?? (sender as Control)?.Parent as PictureBox;

            if (frame != null)
            {
                isMoving = true;
                mouseOffset = e.Location;
            }
        }

        private void Frame_MouseMove(object sender, MouseEventArgs e)
        {
            if (!isMoving) return;

            PictureBox? frame = (sender as PictureBox) ?? (sender as Control)?.Parent as PictureBox;

            if (frame != null)
            {
                frame.Left += e.X - mouseOffset.X;
                frame.Top += e.Y - mouseOffset.Y;
            }
        }

        private void Frame_MouseUp(object sender, MouseEventArgs e)
        {
            isMoving = false;
        }



        private void Frame_MouseDown2(object sender, MouseEventArgs e)
        {
            PictureBox? frame = sender as PictureBox;

            if (frame != null)
            {
                // Détecter si on clique sur une bordure pour le redimensionnement
                if (IsNearBorder(e.Location, frame))
                {
                    resizingFrame = frame;
                    mouseOffset = e.Location;
                    isResizing = true;
                    frame.Cursor = Cursors.SizeNWSE; // Curseur de redimensionnement
                }
            }
        }

        private void Frame_MouseMove2(object sender, MouseEventArgs e)
        {
            PictureBox? frame = sender as PictureBox;

            if (isResizing && resizingFrame != null)
            {
                // Ajuster la taille de la PictureBox en fonction du mouvement de la souris
                int deltaX = e.X - mouseOffset.X;
                int deltaY = e.Y - mouseOffset.Y;

                resizingFrame.Width = Math.Max(10, resizingFrame.Width + deltaX);
                resizingFrame.Height = Math.Max(10, resizingFrame.Height + deltaY);

                // Redimensionner l'objet à l'intérieur de la PictureBox pour qu'il prenne toute la taille
                if (resizingFrame.Controls.Count > 0)
                {
                    Control child = resizingFrame.Controls[0];
                    child.Width = resizingFrame.Width;
                    child.Height = resizingFrame.Height;
                }

                // Mise à jour de la position de la souris
                mouseOffset = e.Location;

                // Redessiner la bordure de la PictureBox
                resizingFrame.Invalidate();
            }
            else
            {
                // Modifier le curseur en fonction de la position de la souris
                if (frame != null && IsNearBorder(e.Location, frame))
                {
                    frame.Cursor = Cursors.SizeNWSE; // Curseur de redimensionnement
                }
                else
                {
                    frame.Cursor = Cursors.Default; // Curseur par défaut
                }
            }
        }

        private void Frame_MouseUp2(object sender, MouseEventArgs e)
        {
            // Réinitialiser les indicateurs de redimensionnement
            resizingFrame = null;
            isResizing = false;

            PictureBox? frame = sender as PictureBox;
            if (frame != null)
            {
                frame.Cursor = Cursors.Default; // Remettre le curseur par défaut
            }
        }



        #endregion

        #region Border

        private bool IsNearBorder(Point mousePosition, PictureBox frame)
        {
            // Vérifier si la souris est proche des bords de la PictureBox
            int borderDistance = 10; // Distance de la bordure pour le redimensionnement
            return mousePosition.X >= frame.Width - borderDistance ||
                   mousePosition.X <= borderDistance ||
                   mousePosition.Y >= frame.Height - borderDistance ||
                   mousePosition.Y <= borderDistance;
        }

        // Méthode pour dessiner les bordures pointillées sur les PictureBox
        private void Frame_Paint(object sender, PaintEventArgs e)
        {
            PictureBox? frame = sender as PictureBox;

            int pictureBoxCount = forme.panel1.Controls.OfType<PictureBox>().Count();

            // Mettre à jour le nom du PictureBox avec un numéro unique
            frame.Name = "PictureBox" + (pictureBoxCount + 1);  // Exemple : PictureBox1, PictureBox2, etc.

            SelectedPictureBox = frame.Name;

            if (frame != null)
            {
                using (Pen pen = new Pen(Color.Black))
                {
                    pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot; // Bordure en pointillés
                    e.Graphics.DrawRectangle(pen, 0, 0, frame.Width - 1, frame.Height - 1);

                }
            }
        }

        #endregion

        // Fonction pour afficher un MessageBox personnalisé
        private DialogResult ShowCustomMessageBox(string message, string title, string yesText, string noText)
        {
            using (Form customDialog = new Form())
            {
                customDialog.Text = title;
                customDialog.FormBorderStyle = FormBorderStyle.FixedDialog;
                customDialog.StartPosition = FormStartPosition.CenterScreen;
                customDialog.MinimizeBox = false;
                customDialog.MaximizeBox = false;
                customDialog.ClientSize = new Size(300, 150);

                // Ajouter un label pour afficher le message
                Label messageLabel = new Label()
                {
                    Text = message,
                    Location = new Point(50, 30),
                    AutoSize = true
                };
                customDialog.Controls.Add(messageLabel);

                // Bouton "Oui"
                Button btnYes = new Button()
                {
                    Text = yesText,
                    DialogResult = DialogResult.Yes,
                    Location = new Point(30, 80)
                };
                customDialog.Controls.Add(btnYes);

                // Bouton "Non"
                Button btnNo = new Button()
                {
                    Text = noText,
                    DialogResult = DialogResult.No,
                    Location = new Point(110, 80)
                };
                customDialog.Controls.Add(btnNo);

                customDialog.AcceptButton = btnYes; // Le bouton par défaut pour "Oui"
                customDialog.CancelButton = btnNo;  // Le bouton "Non" comme bouton d'annulation

                // Affichage du formulaire personnalisé
                DialogResult dialogResult = customDialog.ShowDialog();
                return dialogResult;
            }
        }

        private void controlCommentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(SelectedPictureBox))
            {
                MessageBox.Show("Aucun element sélectionné.");
                return;
            }

            Control? control = null;

            foreach (PictureBox ctrl in this.forme.panel1.Controls)
            {
                if (ctrl.Name == SelectedPictureBox)
                {
                    foreach (Control childControl in ctrl.Controls)
                    {
                        Type controlType = childControl.GetType();

                        if (childControl.Name == selectedControl)
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

                        object? newControl = Activator.CreateInstance(control.GetType());

                        if (newControl is Control newCtrl)
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

            FormVide forme = new FormVide();

            forme.Location = new Point(0, 0);
            forme.panel1 = this.forme.panel1;

            forme.panel1.MouseClick += pnlViewHost_Click;

            AfficherFormDansPanel(forme, pnlViewHost);
        }
    }
}
