﻿using StageCode.LIB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Xml.Linq;

namespace StageCode
{
    public partial class Form1 : Form
    {
        public static int Langue = 1; // 1 = English, 2 = Chinese, 3 = German, 4 = French, 5 = Lithuanian
        private Form1 frm;

        private Control selectedFrame = null; // Stocke la PictureBox sélectionnée
        private bool isResizings = false;
        private Point lastMousePosition;

        private string selectedControl = "";

        //A corriger
        //ORthoAD et CButton TabName a faire

        public Form1()
        {
            InitializeComponent();

            this.ClientSizeChanged += Form1_ClientSizeChanged;
            pnlViewHost.MouseClick += pnlViewHost_Click;
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

        public void ExportFormToXml(Form form, string filePath)
        {
            StringBuilder xmlContent = new StringBuilder();

            xmlContent.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            xmlContent.AppendLine("<Syno name=\"settings\">");

            foreach (Control control in form.Controls)
            {
                ExportControlToXml(control, xmlContent);
            }

            xmlContent.AppendLine("</Syno>");

            File.WriteAllText(filePath, xmlContent.ToString());
        }

        private void ExportControlToXml(Control control, StringBuilder xmlContent)
        {
            if (control is AM60 am60Control)
            {
                AM60 am60 = am60Control as AM60;

                xmlContent.Append(am60.WriteFileXML());
            }
        }

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
                ExportFormToXml(this, "A.txt");
            }
            else if (r == DialogResult.No)
            {

            }
        }
        private void Couper(object sender, EventArgs e)
        {
            if (ActiveControl is TextBox textBox && !string.IsNullOrEmpty(textBox.SelectedText))
            {
                Clipboard.SetText(textBox.SelectedText);
                textBox.SelectedText = string.Empty;
            }
            else
            {
                MessageBox.Show("Aucun texte sélectionné à couper.");
            }
        }
        private void Copier(object sender, EventArgs e)
        {
            if (ActiveControl is TextBox textBox && !string.IsNullOrEmpty(textBox.SelectedText))
            {
                Clipboard.SetText(textBox.SelectedText);
            }
            else
            {
                MessageBox.Show("Aucun texte sélectionné à copier.");
            }
        }

        private void Coller(object sender, EventArgs e)
        {
            if (ActiveControl is TextBox textBox && Clipboard.ContainsText())
            {
                textBox.Paste();
            }
            else
            {
                MessageBox.Show("Aucun texte à coller.");
            }
        }
        private void Supprimer(object sender, EventArgs e)
        {
            if (ActiveControl is TextBox textBox && !string.IsNullOrEmpty(textBox.SelectedText))
            {
                textBox.SelectedText = string.Empty;
            }
            else
            {
                MessageBox.Show("Aucun texte sélectionné à supprimer.");
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
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Ouvrir un fichier"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // Lire le contenu du fichier XML
                    string fileContent = LireXML(openFileDialog.FileName);

                    // Appeler la fonction pour traiter le fichier XML en fonction du type d'objet
                    RecupererContenuXML(fileContent);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Une erreur est survenue lors de l'ouverture du fichier : {ex.Message}", "Erreur");
                }
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

                foreach (XElement component in xml.Elements("Component"))
                {
                    string type = component.Attribute("type")?.Value;

                    if (type == "AM60")
                    {
                        AM60 am60Control = new AM60();
                        am60Control = am60Control.ReadFileXML(component.ToString());

                        Controls.Add(am60Control);
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Une erreur est survenue lors du traitement du fichier XML : {ex.Message}", "Erreur");
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
            this.MainMenu.Width = (int)(this.ClientSize.Width * 0.95);
            this.MainMenu.Height = (int)(this.ClientSize.Height * 0.1);

            float fontSize = (this.ClientSize.Width * 0.02f + this.ClientSize.Height * 0.02f) / 2;

            foreach (ToolStripMenuItem item in MainMenu.Items)
            {
                item.Font = new Font(item.Font.FontFamily, fontSize);
            }
        }

        #endregion

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
                "OrthoDl",
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

        private void pnlViewHost_Click(object sender, MouseEventArgs e)
        {
            // Si le curseur est dans son état par défaut, on désactive les bordures pointillées sur toutes les PictureBox
            if (this.Cursor == DefaultCursor)
            {
                // Parcours toutes les PictureBox dans le pnlViewHost et supprime les bordures pointillées
                foreach (Control control in pnlViewHost.Controls)
                {
                    if (control is PictureBox frame)
                    {
                        // Supprimer le handler d'événement Paint pour ne plus dessiner la bordure
                        frame.Paint -= Frame_Paint;

                        frame.Invalidate();
                    }
                }
                return; // Sortir de la méthode si le curseur est par défaut
            }

            // Si un contrôle est sélectionné, on procède à l'ajout
            Control newControl = null;

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

                // Ajouter le contrôle à l'intérieur de la PictureBox
                newControl.Location = new Point(5, 5);
                frame.Controls.Add(newControl);

                // Ajouter la PictureBox au conteneur principal
                pnlViewHost.Controls.Add(frame);

                // Réinitialiser le curseur
                this.Cursor = DefaultCursor;

                newControl.Click += NewControl_Click;
            }
        }
        private PictureBox resizingFrame = null;
        private Point mouseOffset;
        private bool isResizing = false;
        private bool isMoving = false;

        private void NewControl_Click(object? sender, EventArgs e)
        {
            Control controle = sender as Control;

            // Vérifier si le contrôle est un contrôle enfant d'une PictureBox
            PictureBox frame = controle?.Parent as PictureBox;

            if (frame != null)
            {
                // Ajouter des gestionnaires d'événements pour déplacer ou redimensionner
                frame.MouseDown += Frame_MouseDown;
                frame.MouseMove += Frame_MouseMove;
                frame.MouseUp += Frame_MouseUp;

                // Ajouter la gestion du dessin des bordures pointillées
                frame.Paint += Frame_Paint;
                frame.Invalidate(); // Cela va déclencher l'événement Paint pour redessiner
            }
        }

        private void Frame_MouseDown(object sender, MouseEventArgs e)
        {
            PictureBox frame = sender as PictureBox;

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
                else
                {
                    // Si on clique à l'intérieur de la PictureBox, commencer à déplacer la PictureBox elle-même
                    isMoving = true;
                    mouseOffset = e.Location;
                    frame.Cursor = Cursors.SizeAll; // Curseur pour déplacer
                }
            }
        }

        private void Frame_MouseMove(object sender, MouseEventArgs e)
        {
            PictureBox frame = sender as PictureBox;

            if (isResizing && resizingFrame != null)
            {
                // Si on redimensionne, ajuster la taille de la PictureBox
                int deltaX = e.X - mouseOffset.X;
                int deltaY = e.Y - mouseOffset.Y;

                // Ajuster la taille de la PictureBox en fonction du mouvement de la souris
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
            else if (isMoving && frame != null)
            {
                // Si on déplace la PictureBox, ajuster la position de la PictureBox elle-même
                int deltaX = e.X - mouseOffset.X;
                int deltaY = e.Y - mouseOffset.Y;

                // Déplacer la PictureBox en fonction du mouvement de la souris
                frame.Left += deltaX;
                frame.Top += deltaY;

                // Mise à jour de la position de la souris pour les futurs calculs
                mouseOffset = e.Location;
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

        private void Frame_MouseUp(object sender, MouseEventArgs e)
        {
            // Réinitialiser les indicateurs de redimensionnement et déplacement
            resizingFrame = null;
            isResizing = false;
            isMoving = false;

            PictureBox frame = sender as PictureBox;
            if (frame != null)
            {
                frame.Cursor = Cursors.Default; // Remettre le curseur par défaut
            }
        }

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
            PictureBox frame = sender as PictureBox;
            if (frame != null)
            {
                using (Pen pen = new Pen(Color.Black))
                {
                    pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot; // Bordure en pointillés
                    e.Graphics.DrawRectangle(pen, 0, 0, frame.Width - 1, frame.Height - 1);
                }
            }
        }
    }
}
