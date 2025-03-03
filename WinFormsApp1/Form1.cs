using CodeExceptionManager.Controller.DatabaseEngine.Implementation;
using CodeExceptionManager.Model.Objects;
using Google.Protobuf.Collections;
using Grpc.Core;
using IGeneralConfigurationManager;
using IIOManager;
using Microsoft.Data.Sqlite;
using OrthoDesigner;
using OrthoDesigner.LIB;
using OrthoDesigner.LIB___Copier;
using OrthoDesigner.Other;
using Orthodyne.CoreCommunicationLayer.Controllers;
using Orthodyne.CoreCommunicationLayer.Models.GeneralConfiguration;
using Orthodyne.CoreCommunicationLayer.Models.IO;
using Orthodyne.CoreCommunicationLayer.Services;
using StageCode.LIB;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml.Linq;

namespace StageCode
{
    public partial class Forme1 : Form
    {
        #region Attribut

        // forme pour indiquer l'IP et le port
        FormeIPEtPORT formePortAndIP = new FormeIPEtPORT();

        // variable pour la langue
        public static int Langue = 1; // 1 = English, 2 = Chinese, 3 = German, 4 = French, 5 = Lithuanian

        // variable qui fera réfèrence du forme actuelle pour le forme formeResize pour le redimensionnement
        private Forme1 frm;

        // le forme vide qui contiendra les PictureBox et les controles
        public static FormVide forme;
        private bool peutViderListe = false;

        private string ControlSélectionner = ""; //OK
        private string PictureBoxSelectonner = ""; // OK

        private PictureBox? ChangerPicture = null;
        private Point SourisDecalage;
        private bool Changement = false;
        private bool Bouger = false;

        private bool EnMoouvement = false;
        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();

        private bool Aligner = false;

        private List<PictureBox> listPic = new List<PictureBox>();
        private ContextMenuStrip contextMenu = new ContextMenuStrip();

        // le logo connexion et deconnexion
        private ToolStripMenuItem imageTmpLogo;

        private List<ControlPictureBoxWrapper> listeControle = new List<ControlPictureBoxWrapper>();

        private string ChemainOuvert = "";

        //private GrpcClient _grpcClient;

        //OrthoAla CMDLIB COMBO DI BUG EDIT 

        #endregion

        #region Attribut GRPC

        private string PORT_NUMBER = "50099";
        private string DEFAULT_CORE_IP = "10.1.6.11";
        private static string applicationGuid = "TROP GALERE GRPC";
        private static Channel grpcChannel;
        private static IGeneralConfigurationManager.Methods.MethodsClient clientInterface;
        public static Dictionary<long, IoController> ioControllers = new Dictionary<long, IoController>();
        public static List<IoStream> listeStrem = new List<IoStream>();
        public static List<FlagItem> listeFlag = new List<FlagItem>();

        private object remoteMethods;

        #endregion

        #region Constructeur/Load

        public Forme1()
        {
            InitializeComponent();

            forme = new FormVide(pnlViewHost);
            pnlViewHost.BorderStyle = BorderStyle.FixedSingle;

            // Affichage de la forme dans le panel
            AfficherFormDansPanel(forme, pnlViewHost);

            this.DoubleBuffered = true;
            this.ClientSizeChanged += Form1_ClientSizeChanged;
            forme.panel1.MouseClick += pnlViewHost_Click;

            // formePortAndIP.ShowDialog();
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            for (int i = 0; i < menuStrip1.Items.Count; i++)
            {
                var item = menuStrip1.Items[i];

                item.Margin = new Padding(3, 0, 3, 0);
                item.Padding = new Padding(5);
                item.BackgroundImageLayout = ImageLayout.Zoom;

                item.Image = item.BackgroundImage;
                item.BackgroundImage = null;

                item.MouseHover += Item_MouseHover;
                item.MouseLeave += MenuItem_MouseLeave;

                item.Size = new Size((int)(item.Width * 0.7), (int)(item.Height * 0.7));
            }

            // formePortAndIP.ShowDialog();

            ToolStripSeparator separator = new ToolStripSeparator();
            menuStrip1.Items.Insert(6, separator);

            this.Text = "OrthoDesigner V" + 1;

            AjouterRaccourcisMenuFile();
            AjouterMenuEdition();
            AjouterbtnQuit();
            AjouterMenuVew();
            AppliquerLangue();

            Initialize();

            this.imageTmpLogo = new ToolStripMenuItem();

            this.imageTmpLogo.Image = toolStripMenuItem1.Image;
            toolStripMenuItem1.Image = toolStripMenuItem2.Image;
            toolStripMenuItem2.Image = imageTmpLogo.Image;

            this.WindowState = FormWindowState.Maximized;
        }
        
        private void AfficherFormDansPanel(FormVide frm, Panel panel)
        {
            frm.TopLevel = false;  // Le formulaire n'est pas un formulaire principal
                                   // panel.Controls.Clear();  // Supprime les contrôles existants dans le panel
            panel.Controls.Add(frm);  // Ajoute le formulaire dans le panel

            frm.Show();  // Affiche le formulaire

            forme = frm;
        }

        #endregion

        #region Exception
        public void LogException(Exception ex)
        {
            new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
        }
        #endregion

        #region Ajouter Menu File

        private void AjouterRaccourcisMenuFile()
        {
            newToolStripMenuItem1.ShortcutKeys = Keys.Control | Keys.N;
            newToolStripMenuItem1.Click += newToolStripMenuItem1_Click;

            openToolStripMenuItem1.ShortcutKeys = Keys.Control | Keys.O;
            openToolStripMenuItem1.Click += openToolStripMenuItem1_Click;

            saveToolStripMenuItem1.ShortcutKeys = Keys.Control | Keys.S;
            saveToolStripMenuItem1.Click += saveToolStripMenuItem_Click;
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

        private Task<Control?> RechercheControlAsyncDansPic(PictureBox pic)
        {
            return Task.Run(() =>
            {
                foreach (Control control in pic.Controls)
                {
                    return control;
                }

                return null;
            });
        }

        private Task<PictureBox?> RechercherPictureBoxAsync()
        {
            return Task.Run(() =>
            {
                foreach (Control control in forme.panel1.Controls)
                {
                    if (control is PictureBox pictureBox && pictureBox.Tag?.ToString() == PictureBoxSelectonner)
                    {
                        return pictureBox;
                    }
                }
                return null;
            });
        }

        private void FormVideClosing(object? sender, FormClosingEventArgs e)
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

        private void FormePrincipaleClose(object sender, FormClosingEventArgs e)
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

        #endregion

        #region Actions des menus

        #region Copy/Paste/Cut

        private void Supprimer(object sender, EventArgs e)
        {
            PictureBox? pic = forme.panel1.Controls.OfType<PictureBox>()
                                      .FirstOrDefault(p => p.Tag == PictureBoxSelectonner);

            if (pic != null)
            {
                forme.panel1.Controls.Remove(pic);
                pic.Dispose();

                foreach(var i in listeControle)
                {
                    if(i.PictureBox == pic)
                    {
                        listeControle.Remove(i);
                        break;
                    }
                }
            }
            else
            {
                MessageBox.Show("Aucune PictureBox sélectionnée.");
            }
        }
        private void Couper(object sender, EventArgs e)
        {
            PictureBox? pic = forme.panel1.Controls.OfType<PictureBox>()
                                              .FirstOrDefault(p => p.Tag == PictureBoxSelectonner);

            if (pic != null)
            {
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
                        Text = (ctrl is System.Windows.Forms.TextBox textBox) ? textBox.Text : ctrl.Text
                    });
                }

                DataObject data = new DataObject();
                using (MemoryStream ms = new MemoryStream())
                {
#pragma warning disable SYSLIB0011 // Le type ou le membre est obsolète
                    BinaryFormatter bf = new BinaryFormatter();
#pragma warning restore SYSLIB0011 // Le type ou le membre est obsolète
                    bf.Serialize(ms, controlsData);
                    data.SetData("ControlsData", ms.ToArray());
                }

                Clipboard.SetDataObject(data, true);

                pic.Controls.Clear();
                forme.panel1.Controls.Remove(pic);
            }
        }
        private void Copier(object sender, EventArgs e)
        {
            PictureBox? pic = forme.panel1.Controls.OfType<PictureBox>()
                                              .FirstOrDefault(p => p.Tag == PictureBoxSelectonner);

            if (pic != null)
            {
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
                        Text = (ctrl is System.Windows.Forms.TextBox textBox) ? textBox.Text : ctrl.Text
                    });
                }

                DataObject data = new DataObject();
                using (MemoryStream ms = new MemoryStream())
                {
#pragma warning disable SYSLIB0011 
                    BinaryFormatter bf = new BinaryFormatter();
#pragma warning restore SYSLIB0011 
                    bf.Serialize(ms, controlsData);
                    data.SetData("ControlsData", ms.ToArray());
                }

                Clipboard.SetDataObject(data, true);

                //  MessageBox.Show("Copie effectuée !");
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
                Point mousePosition = forme.panel1.PointToClient(Cursor.Position);

                byte[] rawData = (byte[])Clipboard.GetData("ControlsData")!;
                using (MemoryStream ms = new MemoryStream(rawData))
                {
#pragma warning disable SYSLIB0011 
                    BinaryFormatter bf = new BinaryFormatter();
#pragma warning restore SYSLIB0011
                    List<SerializableControl> controlsData = (List<SerializableControl>)bf.Deserialize(ms);

                    PictureBox newpic = new PictureBox
                    {
                        BorderStyle = BorderStyle.FixedSingle,
                        BackColor = Color.White,
                        Size = new Size(200, 200),
                        Location = mousePosition,
                        AllowDrop = true
                    };

                    newpic.Paint += pic_Paint;
                    newpic.Click += Control_Click;
                    newpic.MouseLeave += pic_MouseLeave;
                    newpic.MouseEnter += pic_MouseEnter;

                    foreach (var controlData in controlsData)
                    {
                        System.Type? controlType = System.Type.GetType(controlData.TypeName);
                        if (controlType != null)
                        {
                            Control Control = (Control)Activator.CreateInstance(controlType)!;
                            Control.Name = controlData.Name;
                            Control.Location = new Point(controlData.X, controlData.Y);
                            Control.Size = new Size(controlData.Width, controlData.Height);
                            if (Control is System.Windows.Forms.TextBox textBox) textBox.Text = controlData.Text;
                            else Control.Text = controlData.Text;

                            newpic.Controls.Add(Control);

                            Control.MouseEnter += Control_MouseEnter;

                            Control.Click += Control_Click;

                            Control.MouseClick += Control_MouseClick;

                            newpic.Width = Control.Size.Width + 10;
                            newpic.Height = Control.Size.Height + 10;
                        }
                    }

                    forme.panel1.Controls.Add(newpic);
                    newpic.Invalidate();
                    PictureBoxSelectonner = "";
                }
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
                    newToolStripMenuItem1.Text = "New";
                    openToolStripMenuItem1.Text = "Open";
                    saveToolStripMenuItem1.Text = "Save";
                    saveAsToolStripMenuItem1.Text = "Save As"; // Save As ajouté
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
                    newToolStripMenuItem1.Text = "新建";
                    openToolStripMenuItem1.Text = "打开";
                    saveToolStripMenuItem1.Text = "保存";
                    saveAsToolStripMenuItem1.Text = "另存为"; // "Save As" en chinois
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
                    newToolStripMenuItem1.Text = "Neu";
                    openToolStripMenuItem1.Text = "Öffnen";
                    saveToolStripMenuItem1.Text = "Speichern";
                    saveAsToolStripMenuItem1.Text = "Speichern unter"; // "Save As" en allemand
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
                    newToolStripMenuItem1.Text = "Nouveau";
                    openToolStripMenuItem1.Text = "Ouvrir";
                    saveToolStripMenuItem1.Text = "Enregistrer";
                    saveAsToolStripMenuItem1.Text = "Enregistrer sous"; // "Save As" en français
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
                    newToolStripMenuItem1.Text = "Naujas";
                    openToolStripMenuItem1.Text = "Atidaryti";
                    saveToolStripMenuItem1.Text = "Išsaugoti";
                    saveAsToolStripMenuItem1.Text = "Išsaugoti kaip"; // "Save As" en lituanien
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

        #region Other
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
                if (ctrl.Tag == PictureBoxSelectonner)
                {
                    foreach (Control childControl in ctrl.Controls)
                    {
                        System.Type controlType = childControl.GetType();

                        if (childControl.Name == ControlSélectionner)
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

            FormVide tmp = new FormVide(pnlViewHost);

            tmp.Location = new Point(0, 0);

            tmp.panel1.MouseClick += pnlViewHost_Click;

            tmp.FormClosing += FormVideClosing;

            AfficherFormDansPanel(tmp, pnlViewHost);
        }
        private void newToolStripMenuItem1_Click(object sender, EventArgs e)
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

                // Affichage de la boîte de dialogue pour le choix de sauvegarde en XML
                DialogResult saveResult = MessageBox.Show(saveMessage, saveTitle, MessageBoxButtons.YesNo);

                //ModuleGeneralConfigurationRevocationService a = new ModuleGeneralConfigurationRevocationService("","");
                //a.LoadConfigurationElements();

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

            forme = new FormVide(pnlViewHost);

            pnlViewHost.BorderStyle = BorderStyle.FixedSingle;

            AfficherFormDansPanel(forme, pnlViewHost);
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

        #endregion

        #region OpenFile
        private void openToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            string message = "";
            string title = "";

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
                    //ExportFormToXml();

                    string xmlMessage = string.Empty;
                    string xmlTitle = string.Empty;

                    switch (Langue)
                    {
                        case 1: // English
                            xmlMessage = "Do you want to save as XML?";
                            xmlTitle = "Save as XML";
                            break;
                        case 2: // Chinese
                            xmlMessage = "是否以XML格式保存？";
                            xmlTitle = "保存为XML";
                            break;
                        case 3: // German
                            xmlMessage = "Möchten Sie als XML speichern?";
                            xmlTitle = "Als XML speichern";
                            break;
                        case 4: // French
                            xmlMessage = "Souhaitez-vous enregistrer en XML ?";
                            xmlTitle = "Enregistrer en XML";
                            break;
                        case 5: // Lithuanian
                            xmlMessage = "Ar norite išsaugoti kaip XML?";
                            xmlTitle = "Išsaugoti kaip XML";
                            break;
                    }

                    DialogResult saveXmlResponse = MessageBox.Show(xmlMessage, xmlTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (saveXmlResponse == DialogResult.Yes)
                    {
                        ExportFormToXml();
                    }
                    else
                    {
                        ExportFormToTXT();
                    }
                }

                else
                {
                    forme.panel1.Controls.Clear();

                    OpenFileDialog file = new OpenFileDialog();
                    file.Filter = "Fichier SYN (*.syn)|*.syn|Fichier XML (*.xml)|*.xml"; // Vous pouvez ajuster si vous avez d'autres filtres de fichiers

                    file.ShowDialog();

                    if (string.IsNullOrEmpty(file.FileName))
                    {
                        throw new InvalidOperationException("Aucun fichier sélèctionner");
                    }

                    bool isXmlFile = false;

                    try
                    {

                        using (StreamReader reader = new StreamReader(file.FileName))
                        {
                            for (int i = 0; i < 5; i++)
                            {
                                string? firstLine = reader.ReadLine();
                                if (firstLine != null && firstLine.TrimStart().StartsWith("<?xml"))
                                {
                                    isXmlFile = true;
                                }
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        LogException(ex);
                        return;
                    }

                    if (isXmlFile)
                    {
                        string? xmlContent = LireXML(file.FileName);

                        if (xmlContent == null)
                        {
                            MessageBox.Show("Le fichier XML n'a pas pu être chargé.", "Erreur de chargement", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        RecupererContenuXML(xmlContent);
                    }
                    else
                    {
                        charger_fichierTXT(file.FileName);
                    }



                    ChemainOuvert = file.FileName;

                    string fileName = file.FileName;

                    string fileNameWithoutPath = System.IO.Path.GetFileName(fileName);

                    forme.label.Text = fileNameWithoutPath;

                }

            }
            catch (Exception ex)
            {
                LogException(ex);
            }
        }

        #region ReadXML
        private string? LireXML(string filePath)
        {
            try
            {
                string xmlContent = File.ReadAllText(filePath);
                return xmlContent;
            }
            catch (Exception ex)
            {
                LogException(ex);
            }

            return null;
        }

        public void RecupererContenuXML(string xmlContent)
        {
            try
            {
                XElement xml = XElement.Parse(xmlContent);

                foreach (XElement component in xml.Descendants("Component"))
                {
                    string? type = component.Attribute("type")?.Value;

                    string componentText = component.ToString();

                    if (type == "Chart")
                    {
                        Chart ChartControl = Chart.ReadFileXML(componentText);

                        XElement? appearance = component.Element("Apparence");
                        if (appearance != null)
                        {
                            // Assurez-vous de définir la taille et la position
                            int sizeWidth = int.Parse(appearance.Element("SizeWidth")?.Value ?? "100");
                            int sizeHeight = int.Parse(appearance.Element("SizeHeight")?.Value ?? "100");
                            int locationX = int.Parse(appearance.Element("LocationX")?.Value ?? "0");
                            int locationY = int.Parse(appearance.Element("LocationY")?.Value ?? "0");

                            ChartControl.Size = new Size(sizeWidth, sizeHeight);

                            //Mes
                            PictureBox pic = new PictureBox
                            {
                                Size = new Size(ChartControl.Size.Width + 10, ChartControl.Size.Height + 10), // Augmenter la taille de 10 pixels
                                Location = new Point(locationX, locationY) // Appliquer la position

                            };

                            pic.BorderStyle = BorderStyle.FixedSingle;

                            ChartControl.Location = new Point(5, 5);

                            pic.MouseLeave += pic_MouseLeave;

                            pic.Controls.Add(ChartControl);

                            ChartControl.Click += Control_Click;
                            ChartControl.MouseEnter += Control_MouseEnter;

                            forme.panel1.Controls.Add(pic);
                        }
                    }
                    else if (type == "CONT1")
                    {
                        CONT1 cont1Control = CONT1.ReadFileXML(componentText);

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
                            pic.BorderStyle = BorderStyle.FixedSingle;

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
                            pic.BorderStyle = BorderStyle.FixedSingle;

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
                            pic.BorderStyle = BorderStyle.FixedSingle;

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
                            int sizeWidth = int.Parse(appearance.Element("SizeWidth")?.Value ?? "100");
                            int sizeHeight = int.Parse(appearance.Element("SizeHeight")?.Value ?? "100");
                            int locationX = int.Parse(appearance.Element("LocationX")?.Value ?? "0");
                            int locationY = int.Parse(appearance.Element("LocationY")?.Value ?? "0");

                            // Définir la taille et la position du contrôle OrthoAla
                            orthoAlaControl.Size = new Size(sizeWidth, sizeHeight);
                            orthoAlaControl.Location = new Point(5, 5); // Position interne dans la PictureBox

                            // Créer une PictureBox pour contenir le contrôle
                            PictureBox pic = new PictureBox
                            {
                                Size = new Size(sizeWidth + 10, sizeHeight + 10), // Augmenter la taille de 10 pixels
                                Location = new Point(locationX, locationY), // Appliquer la position récupérée
                                BorderStyle = BorderStyle.FixedSingle
                            };

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
                            pic.BorderStyle = BorderStyle.FixedSingle;

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
                            pic.BorderStyle = BorderStyle.FixedSingle;

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
                            pic.BorderStyle = BorderStyle.FixedSingle;

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
                            pic.BorderStyle = BorderStyle.FixedSingle;

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
                            pic.BorderStyle = BorderStyle.FixedSingle;

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
                            pic.BorderStyle = BorderStyle.FixedSingle;

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
                            pic.BorderStyle = BorderStyle.FixedSingle;

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
                            pic.BorderStyle = BorderStyle.FixedSingle;


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
                            pic.BorderStyle = BorderStyle.FixedSingle;

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
                            pic.BorderStyle = BorderStyle.FixedSingle;

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
                            pic.BorderStyle = BorderStyle.FixedSingle;

                            orthoAlaControl.Location = new Point(5, 5);

                            pic.Controls.Add(orthoAlaControl);

                            orthoAlaControl.MouseEnter += Control_MouseEnter;
                            pic.MouseLeave += pic_MouseLeave;
                            orthoAlaControl.Click += Control_Click;

                            forme.panel1.Controls.Add(pic);
                        }
                    }
                    else if (type == "STMLINES")
                    {
                        OrthoSTMLINES TabeNameControl = OrthoSTMLINES.ReadFileXML(componentText);

                        XElement? appearance = component.Element("Apparence");
                        if (appearance != null)
                        {
                            int sizeWidth = int.Parse(appearance.Element("SizeWidth")?.Value ?? "100");
                            int sizeHeight = int.Parse(appearance.Element("SizeHeight")?.Value ?? "100");
                            int locationX = int.Parse(appearance.Element("LocationX")?.Value ?? "0");
                            int locationY = int.Parse(appearance.Element("LocationY")?.Value ?? "0");

                            TabeNameControl.Size = new Size(sizeWidth, sizeHeight);

                            PictureBox pic = new PictureBox
                            {
                                Size = new Size(TabeNameControl.Size.Width + 10, TabeNameControl.Size.Height + 10),
                                Location = new Point(locationX, locationY)

                            };

                            pic.BorderStyle = BorderStyle.FixedSingle;

                            TabeNameControl.Location = new Point(5, 5);

                            pic.MouseLeave += pic_MouseLeave;

                            pic.Controls.Add(TabeNameControl);

                            TabeNameControl.Click += Control_Click;
                            TabeNameControl.MouseEnter += Control_MouseEnter;

                            forme.panel1.Controls.Add(pic);
                        }
                    }
                    else if (type == "OrthoTabname")
                    {
                        OrthoSTMLINES TabeNameControl = OrthoSTMLINES.ReadFileXML(componentText);

                        XElement? appearance = component.Element("Apparence");
                        if (appearance != null)
                        {
                            int sizeWidth = int.Parse(appearance.Element("SizeWidth")?.Value ?? "100");
                            int sizeHeight = int.Parse(appearance.Element("SizeHeight")?.Value ?? "100");
                            int locationX = int.Parse(appearance.Element("LocationX")?.Value ?? "0");
                            int locationY = int.Parse(appearance.Element("LocationY")?.Value ?? "0");

                            TabeNameControl.Size = new Size(sizeWidth, sizeHeight);

                            PictureBox pic = new PictureBox
                            {
                                Size = new Size(TabeNameControl.Size.Width + 10, TabeNameControl.Size.Height + 10),
                                Location = new Point(locationX, locationY)

                            };

                            pic.BorderStyle = BorderStyle.FixedSingle;

                            TabeNameControl.Location = new Point(5, 5);

                            pic.MouseLeave += pic_MouseLeave;

                            pic.Controls.Add(TabeNameControl);

                            TabeNameControl.Click += Control_Click;
                            TabeNameControl.MouseEnter += Control_MouseEnter;

                            forme.panel1.Controls.Add(pic);
                        }
                    }
                    else if (type == "Checkbox")
                    {
                        OrthoCheckbox orthoAlaControl = new OrthoCheckbox();

                        orthoAlaControl = OrthoCheckbox.ReadFileXML(componentText);

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
                            pic.BorderStyle = BorderStyle.FixedSingle;

                            orthoAlaControl.Location = new Point(5, 5);


                            pic.Controls.Add(orthoAlaControl);

                            orthoAlaControl.MouseEnter += Control_MouseEnter;
                            pic.MouseLeave += pic_MouseLeave;
                            orthoAlaControl.Click += Control_Click;

                            forme.panel1.Controls.Add(pic);
                        }
                    }
                    else if (type == "Selector")
                    {
                        OrthoSelector orthoAlaControl = new OrthoSelector();

                        orthoAlaControl = OrthoSelector.ReadFileXML(componentText);

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
                            pic.BorderStyle = BorderStyle.FixedSingle;

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
                LogException(ex);
            }
        }

        #endregion

        #region Charger Fichier TXT
        private void charger_fichierTXT(string FilePath)
        {
            try
            {
                //var forme2 = new FormVide();
                //pnlViewHost.Controls.Remove(forme);
                //AfficherFormDansPanel(forme2, pnlViewHost);

                forme.panel1.Controls.Clear();

                forme.WindowState = FormWindowState.Maximized;

                int compteur = 0;

                forme.Text = FilePath;

                using (StreamReader sr = new StreamReader(FilePath, Encoding.UTF8, true))
                {
                    string? ligne;

                    // Lire chaque ligne jusqu'à ce qu'on atteigne la fin ou un nombre limite de lignes vides (ici, 5)
                    while ((ligne = sr.ReadLine()) != null && compteur < 5)
                    {
                        // Ignorer les commentaires (lignes qui commencent par "'") et les lignes vides
                        if (!ligne.StartsWith("'") && !string.IsNullOrWhiteSpace(ligne))
                        {
                            string[] splitPvirgule = ligne.Split(";");

                            // Créer l'objet à partir du texte
                            Control? nouvelObjet = CreerObjetDepuisTexte(splitPvirgule, FilePath);

                            if (nouvelObjet != null)
                            {
                                compteur = 0;  // Réinitialiser le compteur pour les lignes valides

                                // Vérification du Tag de nouvelObjet avant de l'ajouter au PictureBox
                                if (nouvelObjet.Tag != null && !string.IsNullOrEmpty(nouvelObjet.Tag.ToString()))
                                {
                                    string tmp2 = nouvelObjet.Tag.ToString();
                                    PictureBoxSelectonner = tmp2;
                                }
                                else
                                {
                                    int pictureBoxCount = forme.panel1.Controls.OfType<PictureBox>().Count();
                                    nouvelObjet.Tag = "PictureBox" + (pictureBoxCount + 1);
                                }

                                // Créer un PictureBox pour afficher l'objet
                                PictureBox pb = new PictureBox
                                {
                                    BorderStyle = BorderStyle.FixedSingle,
                                    BackColor = Color.White,
                                    Size = new Size(nouvelObjet.Size.Width + 15, nouvelObjet.Size.Height + 15),
                                    Location = nouvelObjet.Location,
                                    AllowDrop = true
                                };

                                // Appliquer les événements au PictureBox
                                pb.Paint += pic_Paint;
                                pb.MouseEnter += pic_MouseEnter;
                                pb.MouseLeave += pic_MouseLeave;
                                pb.Click += Control_Click;

                                // Déplacer l'objet à l'intérieur du PictureBox
                                nouvelObjet.Location = new Point(5, 5);

                                // Ajouter les événements pour l'objet
                                nouvelObjet.MouseEnter += Control_MouseEnter;
                                nouvelObjet.Click += Control_Click;
                                nouvelObjet.MouseClick += Control_MouseClick;

                                // Vérification pour les objets spécifiques comme OrthoSTMLINES
                                if (nouvelObjet is OrthoSTMLINES orthoTabname)
                                {
                                    if (orthoTabname.btn != null)
                                    {
                                        orthoTabname.btn.Width = pb.Width - 10;
                                        orthoTabname.btn.Height = pb.Height - 10;
                                    }
                                }

                                // Ajouter l'objet au PictureBox
                                pb.Controls.Add(nouvelObjet);

                                // Ajouter le PictureBox au panel
                                forme.panel1.Controls.Add(pb);
                            }        
                        }
                        else if (string.IsNullOrWhiteSpace(ligne)) // Incrémenter le compteur pour les lignes vides
                        {
                            compteur++;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogException(ex);
            }
        }

        private Control? CreerObjetDepuisTexte(string[] splitPvirgule, string FilePath)
        {
            Control? objet = splitPvirgule[0] switch
            {
                "Chart" => new Chart(),
                "CONT1" => new CONT1(),
                "INTEG" => new INTEG(),
                "AD" => new OrthoAD(),
                "ALA" => new OrthoAla(),
                "CMD" => new OrthoCMDLib(),
                "COMBO" => new OrthoCombo(),
                "DI" => new OrthoDI(),
                "EDIT" => new OrthoEdit(),
                "IMAGE" => new OrthoImage(),
                "LABEL" => new OrthoLabel(),
                "PBAR" => new OrthoPbar(),
                "REL" => new OrthoRel(),
                "RESULT" => new OrthoResult(),
                "VARNAME" => new OrthoVarname(),
                "RETICULE" => new Reticule(),
                "TABNAME" => new OrthoSTMLINES(),
                "STMLINES" => new OrthoStmLineGroupe(),
                "Checkbox" => new OrthoCheckbox(),
                "Selector" => new OrthoSelector(),
                _ => null
            };

            if (objet == null)
            {
                objet = splitPvirgule[1] switch
                {
                    "Chart" => new Chart(),
                    "CONT1" => new CONT1(),
                    "INTEG" => new INTEG(),
                    "AD" => new OrthoAD(),
                    "ALA" => new OrthoAla(),
                    "CMD" => new OrthoCMDLib(),
                    "COMBO" => new OrthoCombo(),
                    "DI" => new OrthoDI(),
                    "EDIT" => new OrthoEdit(),
                    "IMAGE" => new OrthoImage(),
                    "LABEL" => new OrthoLabel(),
                    "PBAR" => new OrthoPbar(),
                    "REL" => new OrthoRel(),
                    "RESULT" => new OrthoResult(),
                    "VARNAME" => new OrthoVarname(),
                    "RETICULE" => new Reticule(),
                    "TABNAME" => new OrthoSTMLINES(),
                    "STMLINES" => new OrthoStmLineGroupe(),
                    "Checkbox" => new OrthoCheckbox(),
                    "Selector" => new OrthoSelector(),
                    _ => null
                };
            }
            if (objet != null)
            {
                dynamic objDynamic = objet;
                objDynamic.ReadFile(splitPvirgule, "", FilePath, false);
            }

            return objet;
        }

        #endregion

        #endregion

        #region Save

        #region Save TXT

        public void ExportFormToTXT()
        {

            StringBuilder accumulatedText = SaveAsTXT(); // Récupère le texte accumulé des contrôles

            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "Fichier SYN (*.syn)|*.syn"; // Filtrer les fichiers pour .syn
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    using (StreamWriter writer = new StreamWriter(saveFileDialog.FileName))
                    {
                        writer.Write(accumulatedText.ToString());
                    }

                    MessageBox.Show("Fichier sauvegardé avec succès!", "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    string fileName = saveFileDialog.FileName;

                    string fileNameWithoutPath = System.IO.Path.GetFileName(fileName);

                    forme.label.Text = fileNameWithoutPath;
                }
            }
        }
        private StringBuilder SaveAsTXT()
        {
            StringBuilder accumulatedText = new StringBuilder();

            foreach (Control controle in forme.panel1.Controls)
            {
                if (controle is PictureBox pictureBox)
                {
                    foreach (Control childControl in pictureBox.Controls)
                    {
                        childControl.Location = pictureBox.Location;

                        if (childControl is Chart CharteControle)
                        {
                            accumulatedText.AppendLine(CharteControle.WriteFile());
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
                        else if (childControl is OrthoStmLineGroupe Stmlines)
                        {
                            accumulatedText.AppendLine(Stmlines.WriteFile());
                        }
                        else if (childControl is OrthoCheckbox checkbox)
                        {
                            accumulatedText.AppendLine(checkbox.WriteFile());
                        }
                        else if (childControl is OrthoCheckbox select)
                        {
                            accumulatedText.AppendLine(select.WriteFile());
                        }

                        childControl.Location = new Point(5, 5);

                    }
                }
            }

            return accumulatedText;
        }

        #endregion

        #region Save XML
        public void ExportFormToXml()
        {
            StringBuilder xmlContent = new StringBuilder();

            xmlContent.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            xmlContent.AppendLine("<Syno name=\"settings\">");

            xmlContent.AppendLine("  <Controls>");

            StringBuilder accumulatedText = SaveAsXML();

            xmlContent.AppendLine(accumulatedText.ToString());
            xmlContent.AppendLine("  </Controls>");

            xmlContent.AppendLine("</Syno>");

            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "Fichier SYN (*.syn)|*.syn";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    using (StreamWriter writer = new StreamWriter(saveFileDialog.FileName))
                    {
                        writer.Write(xmlContent.ToString());
                    }

                    MessageBox.Show("Fichier sauvegardé avec succès!", "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);


                    string fileName = saveFileDialog.FileName;

                    string fileNameWithoutPath = System.IO.Path.GetFileName(fileName);

                    forme.label.Text = fileNameWithoutPath;
                }
            }
        }
        private StringBuilder SaveAsXML()
        {
            StringBuilder accumulatedText = new StringBuilder();

            foreach (Control controle in forme.panel1.Controls)
            {
                if (controle is PictureBox pictureBox)
                {
                    foreach (Control childControl in pictureBox.Controls)
                    {
                        childControl.Location = pictureBox.Location;

                        if (childControl is Chart CharteControleControl)
                        {
                            accumulatedText.AppendLine(" " + " " + CharteControleControl.WriteFileXML());
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
                        if (childControl is OrthoStmLineGroupe StmlInes)
                        {
                            accumulatedText.AppendLine(StmlInes.WriteFileXML());
                        }

                        childControl.Location = new Point(5, 5);
                    }
                }
            }

            return accumulatedText;
        }

        #endregion
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

            string tmp = ChemainOuvert;

            if (tmp == "")
            {
                tmp = forme.Text + ".syn";
            }

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

                using (StreamWriter writer = new StreamWriter(tmp))
                {
                    writer.Write(xmlContent.ToString());
                }

                MessageBox.Show("Fichier sauvegardé avec succès!", "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                StringBuilder accumulatedText = SaveAsTXT();

                using (StreamWriter writer = new StreamWriter(tmp))
                {
                    writer.Write(accumulatedText.ToString());
                }

                MessageBox.Show("Fichier sauvegardé avec succès!", "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

            DialogResult r = MessageBox.Show($"{message}", title, MessageBoxButtons.YesNo);

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

        #endregion

        #region Responsive

        private void Form1_ClientSizeChanged(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                return;
            }

            this.MainMenu.Width = this.ClientSize.Width;
            this.MainMenu.Height = (int)(this.ClientSize.Height * 0.08);

            float fontSize = (this.ClientSize.Width * 0.01f + this.ClientSize.Height * 0.01f) / 2;
            foreach (ToolStripMenuItem item in MainMenu.Items)
            {
                item.Font = new Font(item.Font.FontFamily, fontSize);
            }

            int sidebarWidth = Math.Max(180, (int)(this.ClientSize.Width * 0.29));
            int sidebarX = this.ClientSize.Width - sidebarWidth;

            pnlViewHost.Location = new Point(0, menuStrip1.Bottom);
            pnlViewHost.Width = sidebarX - 1;
            pnlViewHost.Height = this.ClientSize.Height - pnlViewHost.Top;

            lstToolbox.Width = sidebarWidth - 1;
            lstToolbox.Height = pnlViewHost.Height / 2 - 1;
            lstToolbox.Location = new Point(sidebarX, pnlViewHost.Top + 1);

            propertyGrid1.Width = sidebarWidth - 1;
            propertyGrid1.Height = pnlViewHost.Height - lstToolbox.Height - 1;
            propertyGrid1.Location = new Point(sidebarX, lstToolbox.Bottom + 1);

            lstToolbox.Refresh();
            propertyGrid1.Refresh();
            propertyGrid1.PerformLayout();
            propertyGrid1.Invalidate();

            foreach (FormVide tmp in pnlViewHost.Controls)
            {
                if (tmp.WindowState == FormWindowState.Minimized)
                {
                    tmp.WindowState = FormWindowState.Maximized;
                    tmp.Location = new Point(tmp.Location.X, pnlViewHost.Height);
                    tmp.WindowState = FormWindowState.Minimized;
                }
            }
        }

        #endregion

        #region lstToolbox
        private void Initialize()
        {
            string[] toolboxItems = new string[]
            {
                "Chart", "Cont1", "INTEG", "OrthoAD", "OrthoAla", "OrthoCMDLib", "OrthoCombo",
                "OrthoDI", "OrthoEdit", "Ortholmage", "OrthoLabel", "OrthoPbar", "OrthoRel",
                "OrthoResult", "OrthoVarname", "Reticule", "TABNAME", "STMLINES","Checkbox","Selector"
            };

            string[] activationKeywords = new string[] { "AD", "REL", "ALA", "DI", "AO" };

            lstToolbox.Items.Clear();
            lstToolbox.Items.AddRange(toolboxItems);
        }

        #endregion

        #region PnlView
        private void pnlViewHost_Click(object sender, MouseEventArgs e)
        {
            if (Aligner)
            {
                foreach (PictureBox p in listPic)
                {
                    p.Paint -= pic_Paint;
                }

                listPic.Clear();

                Aligner = false;
                return;
            }

            if (this.Cursor == DefaultCursor)
            {
                var tmp2 = new FormPanelWrapper(forme);

                propertyGrid1.SelectedObject = tmp2;
                propertyGrid1.ExpandAllGridItems();

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

            switch (ControlSélectionner)
            {
                case "Chart":
                    Ctrl = new Chart();
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
                case "TABNAME":
                    Ctrl = new OrthoSTMLINES();
                    break;
                case "STMLINES":
                    Ctrl = new OrthoStmLineGroupe();
                    break;
                case "Checkbox":
                    Ctrl = new OrthoCheckbox();
                    break;
                case "Selector":
                    Ctrl = new OrthoSelector();
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


                foreach (Control a in Ctrl.Controls)
                {
                    a.Enabled = false;
                    a.SendToBack();
                }
                foreach (PictureBox pic2 in forme.panel1.Controls)
                {
                    pic2.MouseDown -= pic_MouseDown2;
                    pic2.MouseMove -= pic_MouseMove2;
                    pic2.MouseUp -= pic_MouseUp2;

                    pic2.Paint -= pic_Paint;
                    pic2.Invalidate();
                }

                pic.BorderStyle = BorderStyle.FixedSingle;

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

        private void Control_Click(object? sender, EventArgs e)
        {
            foreach (PictureBox pic2 in forme.panel1.Controls)
            {
                if (pic2 != null)
                {
                    pic2.MouseDown -= pic_MouseDown2;
                    pic2.MouseMove -= pic_MouseMove2;
                    pic2.MouseUp -= pic_MouseUp2;

                    pic2.Paint -= pic_Paint;
                    pic2.Invalidate();
                }
            }

            Control? controle = sender as Control;
            PictureBox? pic = controle?.Parent as PictureBox;

            if (pic != null)
            {
                pic.MouseDown += pic_MouseDown2;
                pic.MouseMove += pic_MouseMove2;
                pic.MouseUp += pic_MouseUp2;

                pic.Paint += pic_Paint;

                if (pic.Tag != null && !string.IsNullOrEmpty(pic.Tag.ToString()))
                {
                    PictureBoxSelectonner = pic.Tag.ToString();
                }
                else
                {
                    int pictureBoxCount = forme.panel1.Controls.OfType<PictureBox>().Count();
                    pic.Tag = "PictureBox" + (pictureBoxCount + 1);
                }

                pic.Invalidate();

                // Vérifier si le contrôle est déjà dans la liste
                var tmp = listeControle.FirstOrDefault(c => c.Control == controle);

                if (tmp == null && controle != null)
                {
                    // Ajouter seulement si le contrôle n'existe pas encore dans la liste
                    tmp = new ControlPictureBoxWrapper(pic, controle);
                    
                    listeControle.Add(tmp);

                    propertyGrid1.SelectedObject = tmp;
                    propertyGrid1.ExpandAllGridItems();

                    ControlPictureBoxWrapper? check = propertyGrid1.SelectedObject as ControlPictureBoxWrapper;

                    if (check != null && check.Control != null)
                    {
                        Control ctrl = check.Control;

                        // Récupération des valeurs des propriétés si elles existent
                        string texteFlag = ctrl.GetType().GetProperty("Flage")?.GetValue(ctrl)?.ToString() ?? "";
                        string texteStream = ctrl.GetType().GetProperty("IoStream")?.GetValue(ctrl)?.ToString() ?? "";
                        string texteOrthoCore = ctrl.GetType().GetProperty("SimpleName")?.GetValue(ctrl)?.ToString() ?? "";

                        foreach (var i2 in check.OrthoCoreItem)
                        {
                            if (texteOrthoCore.Contains(i2.Name))
                            {
                                i2.Selected = true;
                            }
                        }
                        foreach (var i2 in check.Flags)
                        {
                            if (texteFlag.Contains(i2.Name))
                            {
                                i2.Selected = true;
                            }
                        }
                        foreach (var i2 in check.Stream)
                        {
                            if (texteStream.Contains(i2.Name))
                            {
                                i2.Selected = true;
                            }
                        }
                    }


                    //foreach (var t in tmp.Flags)
                    //{
                    //    foreach (var propertyName in propertiesToCheck)
                    //    {
                    //        if (property != null && property.PropertyType == typeof(string))
                    //        {
                    //            string? propertyValue = property.GetValue(controle) as string;
                    //            if (!string.IsNullOrEmpty(propertyValue) && propertyValue.Contains(t.Name))
                    //            {
                                    
                    //            }
                    //        }
                    //    }
                    //}

                }

                if (tmp != null)
                {
                    propertyGrid1.SelectedObject = tmp;
                    propertyGrid1.ExpandAllGridItems();


                    
                }
            }
        }

        #endregion

        //s
        #region AlignerItem
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

                        parentPictureBox.Paint += pic_Paint;

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
                foreach (PictureBox p in listPic)
                {
                    p.Paint -= pic_Paint;
                }

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

        #endregion

        #region Mouse (Resize et Move Control & PictureBox)
        #region Move
        private void pic_MouseDown(object sender, MouseEventArgs e)
        {
            if (!EnMoouvement) return;

            PictureBox? picturebox = (sender as PictureBox) ?? (sender as Control)?.Parent as PictureBox;

            if (picturebox != null)
            {
                Bouger = true;
                SourisDecalage = e.Location;
                picturebox.BringToFront();
            }
        }

        private void pic_MouseMove(object Sender, MouseEventArgs e)
        {
            if (!Bouger) return;

            PictureBox? pictureBox = (Sender as PictureBox) ?? (Sender as Control)?.Parent as PictureBox;

            if (pictureBox == null) return;

            int deltaX = e.X - SourisDecalage.X;
            int deltaY = e.Y - SourisDecalage.Y;

            var pictureBoxes = forme.panel1.Controls.OfType<PictureBox>().ToList();

            if (listPic.Count > 1)
            {
                foreach (var pic in listPic)
                {
                    pic.Left += deltaX;
                    pic.Top += deltaY;
                }
            }
            else
            {
                pictureBox.Left += deltaX;
                pictureBox.Top += deltaY;
            }

            Task.Run(() =>
            {
                using (Graphics g = forme.panel1.CreateGraphics())
                {
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    g.Clear(forme.panel1.BackColor);

                    List<(Point start, Point end)> drawnLines = new List<(Point, Point)>();
                    int tolerance = 0;

                    foreach (Control ctrl in forme.panel1.Controls)
                    {
                        if (ctrl is PictureBox pic && pic != pictureBox)
                        {
                            Point top1 = new Point(pictureBox.Left + pictureBox.Width / 2, pictureBox.Top);
                            Point bottom1 = new Point(pictureBox.Left + pictureBox.Width / 2, pictureBox.Bottom);
                            Point left1 = new Point(pictureBox.Left, pictureBox.Top + pictureBox.Height / 2);
                            Point right1 = new Point(pictureBox.Right, pictureBox.Top + pictureBox.Height / 2);

                            Point top2 = new Point(pic.Left + pic.Width / 2, pic.Top);
                            Point bottom2 = new Point(pic.Left + pic.Width / 2, pic.Bottom);
                            Point left2 = new Point(pic.Left, pic.Top + pic.Height / 2);
                            Point right2 = new Point(pic.Right, pic.Top + pic.Height / 2);

                            using (Pen pen = new Pen(Color.Blue, 2))
                            {
                                if (Math.Abs(pictureBox.Top - pic.Top) <= tolerance)
                                    g.DrawLine(pen, top1, top2);

                                if (Math.Abs(pictureBox.Bottom - pic.Bottom) <= tolerance)
                                    g.DrawLine(pen, bottom1, bottom2);

                                if (Math.Abs(pictureBox.Left - pic.Left) <= tolerance)
                                    g.DrawLine(pen, left1, left2);

                                if (Math.Abs(pictureBox.Right - pic.Right) <= tolerance)
                                    g.DrawLine(pen, right1, right2);

                                using (Pen pen2 = new Pen(Color.Blue, 2))
                                {
                                    if (Math.Abs(pictureBox.Top - pic.Bottom) <= tolerance)
                                        g.DrawLine(pen2, top1, bottom2);

                                    if (Math.Abs(pictureBox.Bottom - pic.Top) <= tolerance)
                                        g.DrawLine(pen2, bottom1, top2);

                                    if (Math.Abs(pictureBox.Left - pic.Right) <= tolerance)
                                        g.DrawLine(pen2, left1, right2);

                                    if (Math.Abs(pictureBox.Right - pic.Left) <= tolerance)
                                        g.DrawLine(pen2, right1, left2);
                                }
                            }
                        }
                    }
                }
            });
        }

        private void pic_MouseUp(object Sender, MouseEventArgs e)
        {
            Bouger = false;
        }
        #endregion


        #region Resize Picturebox
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

                    pictureBox.BringToFront();
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
                    child.Width = ChangerPicture.Width - 10;
                    child.Height = ChangerPicture.Height - 10;
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

        #endregion

        #region Border

        private bool IsNearBorder(Point mousePosition, PictureBox pic)
        {
            int borderDistance = 15;
            return mousePosition.X >= pic.Width - borderDistance ||
                   mousePosition.X <= borderDistance ||
                   mousePosition.Y >= pic.Height - borderDistance ||
                   mousePosition.Y <= borderDistance;
        }

        private void pic_Paint(object sender, PaintEventArgs e)
        {
            PictureBox? pic = sender as PictureBox;

            int pictureBoxCount = forme.panel1.Controls.OfType<PictureBox>().Count();

            pic.Tag = "PictureBox" + (pictureBoxCount + 1);

            PictureBoxSelectonner = pic.Tag.ToString();

            if (pic != null)
            {
                using (Pen pen = new Pen(Color.SkyBlue, 4))
                {
                    //  pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid; 
                    e.Graphics.DrawRectangle(pen, pic.ClientRectangle);
                }
            }
        }

        #endregion

        #region Logo Click

        private void ConnexionLogo(object sender, EventArgs e)
        {
            if (toolStripMenuItem2.Image != null)
            {
                bool AncienConnecter = FormeIPEtPORT.connecter;

                if (FormeIPEtPORT.connecter)
                {
                    this.imageTmpLogo.Image = toolStripMenuItem1.Image;
                    toolStripMenuItem1.Image = toolStripMenuItem2.Image;
                    toolStripMenuItem2.Image = imageTmpLogo.Image;

                    if (!AncienConnecter)
                    {
                        toolStripMenuItem1.Tag = "Deconnexion";
                    }
                    else
                    {
                        toolStripMenuItem1.Tag = "Conexion";
                        FormeIPEtPORT.ip = "";
                        this.btnIP.Text = FormeIPEtPORT.ip.ToString();

                        // Vérifie si SelectedObject est bien du type ControlPictureBoxWrapper
                        if (propertyGrid1.SelectedObject is ControlPictureBoxWrapper tmp)
                        {
                            // Trouve l'élément dans la liste en fonction du contrôle sélectionné
                            var controlWrapper = this.listeControle.Find(item => item.Control == tmp.Control);

                            if (controlWrapper != null)
                            {
                                // Crée une nouvelle instance de ControlPictureBoxWrapper avec le PictureBox et le Control
                                var tmp2 = new ControlPictureBoxWrapper(tmp.PictureBox, tmp.Control);

                                // Retire l'élément existant de la liste
                                listeControle.Remove(controlWrapper);

                                // Ajoute la nouvelle instance à la liste
                                listeControle.Add(tmp2);

                                // Met à jour l'objet sélectionné dans le PropertyGrid
                                propertyGrid1.SelectedObject = tmp2;
                            }
                        }
                    }

                    FormeIPEtPORT.connecter = !FormeIPEtPORT.connecter;
                    return;
                }

                formePortAndIP.ShowDialog();

                if (AncienConnecter != FormeIPEtPORT.connecter)
                {
                    this.imageTmpLogo.Image = toolStripMenuItem1.Image;
                    toolStripMenuItem1.Image = toolStripMenuItem2.Image;
                    toolStripMenuItem2.Image = imageTmpLogo.Image;

                    if (!AncienConnecter)
                    {
                        toolStripMenuItem1.Tag = "Deconnexion";
                    }
                    else
                    {
                        toolStripMenuItem1.Tag = "Conexion";
                        FormeIPEtPORT.ip = "";
                    }

                    this.btnIP.Text = FormeIPEtPORT.ip.ToString();
                }
            }
            else
            {
                string message = GetMessage("NoBackgroundImage");
                MessageBox.Show(message);
            }

            if (FormeIPEtPORT.connecter)
            {
                DEFAULT_CORE_IP = FormeIPEtPORT.ip;
                PORT_NUMBER = formePortAndIP.portNumbers;

                try
                {
                    //grpcChannel = new Channel(DEFAULT_CORE_IP + ":" + PORT_NUMBER, ChannelCredentials.Insecure);
                    //clientInterface = new Methods.MethodsClient(grpcChannel);


                    listeControle.Clear();

                    Task.Run(() =>
                    {
                        try
                        {
                            GeneralController generalController = new GeneralController("", this.DEFAULT_CORE_IP);
                            generalController.ModuleAccessController.CheckRunningCore();

                            ChargerContenuOrthoCore();

                            listeStrem = GetAllStreamsDataTableIoStream();

                            listeStrem = GetAllStreamsDataTable();
                            var i = new ModuleGeneralConfigurationControllerOrthoDesigner(new ModuleGeneralConfigurationRevocationService("", this.DEFAULT_CORE_IP), new GeneralController("", this.DEFAULT_CORE_IP));
                            listeFlag = i.LoadFlags();

                            string connectionMessage = GetMessage("SuccessfulConnection");
                            listeControle.Clear();
                            MessageBox.Show(connectionMessage);

                            string streamMessage = GetMessage("NoStreamFound");
                            if (listeStrem.Count <= 0)
                            {
                                MessageBox.Show(streamMessage);
                            }

                            string flagMessage = GetMessage("NoFlagFound");
                            if (listeFlag.Count < 0)
                            {
                                MessageBox.Show(flagMessage);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    });
                }
                catch (Exception ex)
                {
                    LogException(ex);
                }
            }
            else
            {
                DEFAULT_CORE_IP = "";
                PORT_NUMBER = "";

               // grpcChannel?.ShutdownAsync().Wait();

              //  Forme1.ioControllers.Clear();
            }
        }

        // Fonction générale pour obtenir les messages en fonction du type de message et de la langue
        private string GetMessage(string messageType)
        {
            string message = string.Empty;

            switch (Langue)
            {
                case 1: // English
                    message = messageType switch
                    {
                        "NoBackgroundImage" => "No background image set on toolStripMenuItem2.",
                        "SuccessfulConnection" => "Connection successful.",
                        "NoStreamFound" => "No stream found!",
                        "NoFlagFound" => "No flag found!",
                        _ => "Unknown message type"
                    };
                    break;
                case 2: // Chinese
                    message = messageType switch
                    {
                        "NoBackgroundImage" => "未在toolStripMenuItem2上设置背景图片。",
                        "SuccessfulConnection" => "连接成功",
                        "NoStreamFound" => "没有找到流!",
                        "NoFlagFound" => "没有找到旗帜!",
                        _ => "未知的消息类型"
                    };
                    break;
                case 3: // German
                    message = messageType switch
                    {
                        "NoBackgroundImage" => "Kein Hintergrundbild auf toolStripMenuItem2 gesetzt.",
                        "SuccessfulConnection" => "Verbindung erfolgreich",
                        "NoStreamFound" => "Kein Stream gefunden!",
                        "NoFlagFound" => "Keine Flagge gefunden!",
                        _ => "Unbekannter Nachrichtentyp"
                    };
                    break;
                case 4: // French
                    message = messageType switch
                    {
                        "NoBackgroundImage" => "Aucune image de fond définie sur toolStripMenuItem2.",
                        "SuccessfulConnection" => "Connexion réussie",
                        "NoStreamFound" => "Aucun Stream trouvé !",
                        "NoFlagFound" => "Aucun Flag trouvé !",
                        _ => "Type de message inconnu"
                    };
                    break;
                case 5: // Lithuanian
                    message = messageType switch
                    {
                        "NoBackgroundImage" => "Įrankių juostoje nėra nustatyta fono nuotrauka.",
                        "SuccessfulConnection" => "Prisijungimas sėkmingas",
                        "NoStreamFound" => "Nerasta jokių srautų!",
                        "NoFlagFound" => "Nerasta jokių vėliavų!",
                        _ => "Nežinomas pranešimo tipas"
                    };
                    break;
                default:
                    message = "Unknown message type";
                    break;
            }

            return message;
        }

        //private void toolStripMenuItem1_Click(object sender, EventArgs e)
        //{
        //    if (toolStripMenuItem2.Image != null)
        //    {
        //        bool AncienConnecter = FormeIPEtPORT.connecter;

        //        if (FormeIPEtPORT.connecter)
        //        {
        //            this.imageTmpLogo.Image = toolStripMenuItem1.Image;
        //            toolStripMenuItem1.Image = toolStripMenuItem2.Image;
        //            toolStripMenuItem2.Image = imageTmpLogo.Image;

        //            if (!AncienConnecter)
        //            {
        //                toolStripMenuItem1.Tag = "Deconnexion";
        //            }
        //            else
        //            {
        //                toolStripMenuItem1.Tag = "Conexion";
        //                FormeIPEtPORT.ip = "";
        //                this.btnIP.Text = FormeIPEtPORT.ip.ToString();
        //                // Vérifie si SelectedObject est bien du type ControlPictureBoxWrapper
        //                if (propertyGrid1.SelectedObject is ControlPictureBoxWrapper tmp)
        //                {
        //                    // Trouve l'élément dans la liste en fonction du contrôle sélectionné
        //                    var controlWrapper = this.listeControle.Find(item => item.Control == tmp.Control);

        //                    if (controlWrapper != null)
        //                    {
        //                        // Crée une nouvelle instance de ControlPictureBoxWrapper avec le PictureBox et le Control
        //                        var tmp2 = new ControlPictureBoxWrapper(tmp.PictureBox, tmp.Control);

        //                        // Retire l'élément existant de la liste
        //                        listeControle.Remove(controlWrapper);

        //                        // Ajoute la nouvelle instance à la liste
        //                        listeControle.Add(tmp2);

        //                        // Met à jour l'objet sélectionné dans le PropertyGrid
        //                        propertyGrid1.SelectedObject = tmp2;
        //                    }
        //                }
        //            }

        //            FormeIPEtPORT.connecter = !FormeIPEtPORT.connecter;

        //            return;
        //        }

        //        formePortAndIP.ShowDialog();

        //        if (AncienConnecter != FormeIPEtPORT.connecter)
        //        {
        //            this.imageTmpLogo.Image = toolStripMenuItem1.Image;
        //            toolStripMenuItem1.Image = toolStripMenuItem2.Image;
        //            toolStripMenuItem2.Image = imageTmpLogo.Image;

        //            if (!AncienConnecter)
        //            {
        //                toolStripMenuItem1.Tag = "Deconnexion";
        //            }
        //            else
        //            {
        //                toolStripMenuItem1.Tag = "Conexion";
        //                FormeIPEtPORT.ip = "";
        //            }

        //            this.btnIP.Text = FormeIPEtPORT.ip.ToString();
        //        }
        //    }
        //    else
        //    {
        //        MessageBox.Show("Aucune image de fond définie sur toolStripMenuItem2.");
        //    }

        //    if (FormeIPEtPORT.connecter)
        //    {
        //        DEFAULT_CORE_IP = FormeIPEtPORT.ip;
        //        PORT_NUMBER = formePortAndIP.portNumbers;

        //        try
        //        {
        //            grpcChannel = new Channel(DEFAULT_CORE_IP + ":" + PORT_NUMBER, Grpc.Core.ChannelCredentials.Insecure);
        //            clientInterface = new Methods.MethodsClient(grpcChannel);

        //            MessageBox.Show("Connexion reussis");

        //            ChargerContenuOrthoCore();

        //           // listeStrem = GetAllStreamsDataTableIoStream();

        //            listeStrem = GetAllStreamsDataTable();

        //            var i = new ModuleGeneralConfigurationControllerOrthoDesigner(new ModuleGeneralConfigurationRevocationService(""), new GeneralController(""));
        //            ////i.F
        //            ///
        //            var listeFlag = i.LoadFlags();

        //            foreach(var flag in listeFlag)
        //            {
        //                MessageBox.Show(flag.Id.ToString());
        //            }

        //            if (listeStrem.Count <= 0)
        //            {
        //                MessageBox.Show("Aucun Stream trouver !");
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            LogException(ex);
        //        }
        //    }
        //    else
        //    {
        //        DEFAULT_CORE_IP = "";
        //        PORT_NUMBER = "";

        //        grpcChannel?.ShutdownAsync().Wait();

        //        Forme1.ioControllers.Clear();
        //    }

        //}

        private void SavecLogoClick(object sender, EventArgs e)
        {
            saveToolStripMenuItem_Click(sender, e);
        }

        private void NouveauLogo_Click(object sender, EventArgs e)
        {
            newFormeToolStripMenuItem_Click(sender, e);
        }

        private void OpenLogo_Click(object sender, EventArgs e)
        {
            openToolStripMenuItem1_Click(sender, e);
        }
        
        private void SaveALL_Click(object sender, EventArgs e)
        {
            string saveMessage = "";
            string saveTitle = "";

            switch (Langue)
            {
                case 1: saveMessage = "Do you want to save as XML?"; saveTitle = "Save As XML"; break;
                case 2: saveMessage = "您是否要以XML格式保存？"; saveTitle = "保存为XML"; break;
                case 3: saveMessage = "Möchten Sie als XML speichern?"; saveTitle = "Als XML speichern"; break;
                case 4: saveMessage = "Voulez-vous enregistrer en XML ?"; saveTitle = "Enregistrer en XML"; break;
                case 5: saveMessage = "Ar norite išsaugoti kaip XML?"; saveTitle = "Išsaugoti kaip XML"; break;
            }

            DialogResult saveResult = MessageBox.Show(saveMessage, saveTitle, MessageBoxButtons.YesNo);
            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
            {
                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    string selectedPath = folderDialog.SelectedPath;

                    foreach (FormVide vide in this.pnlViewHost.Controls)
                    {
                        forme = vide;
                        string filePath = Path.Combine(selectedPath, forme.Text + ".Syn");

                        if (saveResult == DialogResult.Yes)
                        {
                            StringBuilder xmlContent = new StringBuilder();
                            xmlContent.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                            xmlContent.AppendLine("<Syno name=\"settings\">\n  <Controls>");
                            xmlContent.AppendLine(SaveAsXML().ToString());
                            xmlContent.AppendLine("  </Controls>\n</Syno>");

                            File.WriteAllText(filePath, xmlContent.ToString());
                        }
                        else
                        {
                            File.WriteAllText(filePath, SaveAsTXT().ToString());
                        }
                    }
                    MessageBox.Show("Fichiers sauvegardés avec succès!", "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void Close_Click(object sender, EventArgs e)
        {
            forme.Close();
        }

        private void CloseALL_Click(object sender, EventArgs e)
        {
            pnlViewHost.Controls.Clear();
        }

        private void CouperLogo_Click(object sender, EventArgs e)
        {
            Couper(sender, e);
        }

        private void CopierLogo_Click(object sender, EventArgs e)
        {
            Copier(sender, e);
        }

        private void CollerLogo_Click(object sender, EventArgs e)
        {
            Coller(sender, e);
        }

        #endregion

        #region Logo Mouse Enter/Leave
        private void Item_MouseHover(object? sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem menuItem)
            {
                contextMenu.Items.Clear();
                contextMenu.Items.Add($"{menuItem.Tag}");
                contextMenu.Items[0].Enabled = false;
                contextMenu.Enabled = true;

                // Point menuPosition = new Point(Cursor.Position.X + 10, Cursor.Position.Y + 10);
                contextMenu.Show(Cursor.Position, ToolStripDropDownDirection.BelowLeft);

                //timer.Stop();

                //timer.Interval = 3000;
                //timer.Tick += (s, args) =>
                //{
                //    if (!contextMenu.Bounds.Contains(Cursor.Position))
                //    {
                //        contextMenu.Hide();
                //        timer.Stop();
                //    }
                //};
                //timer.Start();
            }
        }

        private void MenuItem_MouseLeave(object? sender, EventArgs e)
        {
            contextMenu.Hide();
        }

        #endregion

        #region PropertyGrid1
        private void propertyGrid1_PropertyValueChanged(object sender, PropertyValueChangedEventArgs e)
        {
            if (sender is not PropertyGrid pr || pr.SelectedObject is not ControlPictureBoxWrapper selectedControl)
                return;

            selectedControl.AfficherSelection();
            Control? control = selectedControl.Control;

            if (control == null) return;

            foreach (var i in listeControle.Where(i => i.Control?.GetType() == selectedControl.Control.GetType()))
            {
                for (int z = 0; z < i.Stream.Count; z++)
                {
                    i.Stream[z].Selected = selectedControl.Stream[z].Selected;
                }
            }

            if (propertyGrid1.SelectedObject is not ControlPictureBoxWrapper check || check.Control == null)
                return;

            Dictionary<string, string> propertiesToUpdate = new()
            {
                { "Flage", string.Join(",", check.Flags.Where(i => i.Selected).Select(i => i.Name)) },
                { "IoStream", string.Join(",", check.Stream.Where(i => i.Selected).Select(i => i.Name)) },
                { "SimpleName", string.Join(",", check.OrthoCoreItem.Where(i => i.Selected).Select(i => i.Name)) }
            };

            foreach (var (propertyName, value) in propertiesToUpdate)
            {
                var property = check.Control.GetType().GetProperty(propertyName);
                if (property?.CanWrite == true && property.PropertyType == typeof(string))
                {
                    property.SetValue(check.Control, value);
                }
            }

            foreach (var i in check.OrthoCoreItem) i.Selected = propertiesToUpdate["SimpleName"].Contains(i.Name);
            foreach (var i in check.Flags) i.Selected = propertiesToUpdate["Flage"].Contains(i.Name);
            foreach (var i in check.Stream) i.Selected = propertiesToUpdate["IoStream"].Contains(i.Name);

            //// Vérifier si le contrôle est un PictureBox et si sa visibilité est 0

            //var property2 = check.Control.GetType().GetProperty("Visibility");
            //MessageBox.Show(property2.GetType().ToString());

            //if (property2 == null || property2.PropertyType != typeof(int))
            //    return;

            //int visibilityValue = (int)property2.GetValue(check.Control);

            //if (control is PictureBox pictureBox && visibilityValue == 0)
            //{
            //    DrawCrossOnPictureBox(pictureBox);
            //}

            string i2 = (string)control.GetType().GetProperty("Visibility").GetValue(control);
            int tmp = Convert.ToInt32(i2);

            var t = propertyGrid1.SelectedObject as ControlPictureBoxWrapper;

            if (tmp == 0)
            {
                DrawCrossOnControl(t.Control);
            }
            else
            {
                using Graphics g = t.Control.CreateGraphics();

                g.Clear(t.Control.BackColor);
            }
        }

        private void DrawCrossOnControl(Control control)
        {
            using Graphics g = control.CreateGraphics();
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias; // Améliore la qualité du dessin

            using Pen pen = new Pen(Color.Red, 3);

            int x1 = 0, y1 = 0;
            int x2 = control.Width, y2 = control.Height;

            // Dessiner la croix
            g.DrawLine(pen, x1, y1, x2, y2);
            g.DrawLine(pen, x2, y1, x1, y2);

            // Dessiner le cadre rouge autour du contrôle
            g.DrawRectangle(pen, 0, 0, control.Width - 1, control.Height - 1);
        }


        private void lstToolbox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstToolbox.SelectedIndex >= 0 && lstToolbox.SelectedIndex < lstToolbox.Items.Count)
            {
                if (this.Cursor == Cursors.Default)
                {
                    ControlSélectionner = lstToolbox.SelectedItem.ToString();
                    this.Cursor = Cursors.Cross;
                }
            }
        }
        #endregion

        #region PictureBox Enter/Leave

        private void pic_MouseLeave(object? sender, EventArgs e)
        {
            ////PictureBoxSelectonner = "";

            //PictureBox? p = sender as PictureBox;
            //p.Paint -= pic_Paint;
            //foreach (Control ctrl in p.Controls)
            //{
            //    p.Size = ctrl.Size;

            //    p.Width += 10;
            //    p.Height += 10;
            //}

            //p.Invalidate();

            //this.Cursor = DefaultCursor;

            //this.Bouger = false;
            //this.EnMoouvement = false;

            //Control? a = sender as Control;

            //if (a != null)
            //{
            //    foreach (Control tmp in a.Controls)
            //    {
            //        a.Enabled = false;
            //    }
            //}

            return;
        }

        private void pic_MouseEnter(object? sender, EventArgs e)
        {
            //PictureBoxSelectonner = "";

            //PictureBox? pic = sender as PictureBox;

            //if (pic != null)
            //{
            //    pic.Paint += pic_Paint;
            //}
        }

        #endregion

        #region Fonction GRPC

        public void ChargerContenuOrthoCore()
        {
            try
            {
                ioControllers.Clear();

                Task.Run(() =>
                {
                    var module = new ModuleIoRemoteMethodInvocationService("", DEFAULT_CORE_IP);

                    //module.DEFAULT_CORE_IP = DEFAULT_CORE_IP;
                    //module.PORT_NUMBER = PORT_NUMBER;

                    var b = new ModuleIoControllerOrthoDesigner(module, new GeneralController("", this.DEFAULT_CORE_IP));
                    b.RefreshControllers();

                    ioControllers = b.GetIoControllers();

                    if (ioControllers.Count == 0)
                    {
                        MessageBox.Show("Aucun controllers");
                    }
                });
            }
            catch (Exception ex)
            {
                LogException(ex);
            }
        }

        //public static List<string> AfficherContenuListeGRPCParType(string typeFiltre)
        //{
        //    List<string> listes = new List<string>();

        //    try
        //    {
        //        grpcChannel = new Channel(DEFAULT_CORE_IP + ":" + PORT_NUMBER, Grpc.Core.ChannelCredentials.Insecure);
        //        clientInterface = new Methods.MethodsClient(grpcChannel);

        //        var b = new ModuleIoController(new ModuleIoRemoteMethodInvocationService(""), new GeneralController());
        //        b.RefreshControllers();

        //        ioControllers = b.GetIoControllers();

        //        listes = ioControllers
        //            .Where(kv => ExtraireTypeDepuisNom(kv.Value.FullName) == typeFiltre)
        //            .Select(kv => $"Clé : {kv.Key}, Valeur : {kv.Value.FullName}\n")
        //            .ToList();
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show($"Erreur: {ex.Message}");
        //    }

        //    return listes;
        //}

        //Méthode pour extraire le type entre parenthèses dans le nom complet
        private static string ExtraireTypeDepuisNom(string fullName)
        {
            int debut = fullName.IndexOf('(');
            int fin = fullName.IndexOf(')');

            if (debut >= 0 && fin > debut)
            {
                return fullName.Substring(debut + 1, fin - debut - 1);
            }

            return string.Empty;
        }

        public List<IoStream> GetAllStreamsDataTableIoStream()
        {
            List<IoStream> streamsList = new List<IoStream>();

            try
            {
                var module = new ModuleIoControllerOrthoDesigner(new ModuleIoRemoteMethodInvocationService("", DEFAULT_CORE_IP), new GeneralController("", DEFAULT_CORE_IP));
                var liste = module.GetIoStreams();
            }
            catch (Exception ex)
            {
                LogException(ex);
            }

            return streamsList;
        }


        public List<IoStream> GetAllStreamsDataTable()
        {
            List<IoStream> streamsList = new List<IoStream>();

            try
            {
                GetDefinedStreamsOutput definedStreamsOutput = new ModuleIoRemoteMethodInvocationService("", this.DEFAULT_CORE_IP).GetDefinedStreams();

                foreach (StreamElement stream in definedStreamsOutput.Streams)
                {
                    int streamId = Convert.ToInt32(stream.Identifier);
                    RepeatedField<StreamDataTableElement> streamDataTable =
                        new ModuleIoRemoteMethodInvocationService("", this.DEFAULT_CORE_IP).GetStreamDataTable(streamId).DataTable;

                    IoStream tmpstream = new IoStream()
                    {
                        Id = streamId,
                        ComponentType = stream.ComponentType,
                        ShortType = stream.ShortTypeName,
                        IsArchive = stream.IsArchive
                    };

                    foreach (StreamDataTableElement dataTableItem in streamDataTable)
                    {
                        IoStreamDataTableItem ioStreamDataTable = new IoStreamDataTableItem()
                        {
                            Name = dataTableItem.DataName,
                            Unit = dataTableItem.Unit,
                            ErrorMsg = dataTableItem.Errormsg,
                            Priority = (int)dataTableItem.Priority,
                            Type = (int)dataTableItem.Type,
                            Id = Convert.ToInt32(dataTableItem.Id),
                            MinValue = Convert.ToDouble(dataTableItem.Minvalue),
                            MaxValue = Convert.ToDouble(dataTableItem.Maxvalue)
                        };

                        foreach (IIOManager.ElementProperty elementPropertyAsIdentifier in dataTableItem.PropertiesAsIdentifier)
                        {
                            ioStreamDataTable.GetIdentifierProperties().Add(new IoItemPropertyItem()
                            {
                                PropertyName = elementPropertyAsIdentifier.PropertyName,
                                PropertyValueInString = elementPropertyAsIdentifier.ParsedToStringPropertyValue
                            });
                        }

                        foreach (string item in dataTableItem.AuthorizedControllerTypes)
                        {
                            ioStreamDataTable.GetauthorizedControllerTypes().Add(item);
                        }

                        tmpstream.GetIoStreamDataTableItems().Add(ioStreamDataTable);
                    }

                    streamsList.Add(tmpstream);
                }
            }
            catch (Exception ex)
            {
                new LoggedException(
                    Assembly.GetExecutingAssembly().GetName().Name,
                    Assembly.GetExecutingAssembly().GetName().Version.ToString(),
                    this.GetType().Name,
                    MethodBase.GetCurrentMethod().Name,
                    ex.Message,
                    ex.StackTrace
                );
            }

            return streamsList;
        }

        #endregion
    }
}
