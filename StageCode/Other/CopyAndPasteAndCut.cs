using OrthoDesigner.LIB;
using StageCode.LIB;
using StageCode;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using System.Runtime.Serialization;

namespace OrthoDesigner.Other
{
    [Serializable]

    public class CopyPasteHandler
    {
        public static void CopyToClipboard(Control controle)
        {
            if (controle == null) return;

            // Vérifier si le type du contrôle est sérialisable
            if (!Attribute.IsDefined(controle.GetType(), typeof(SerializableAttribute)))
            {
                MessageBox.Show("Le contrôle n'est pas sérialisable.");
                return;
            }

            // Créer un MemoryStream pour la sérialisation
            using (MemoryStream ms = new MemoryStream())
            {
                // Désactivation du warning sur BinaryFormatter (obsolète)
#pragma warning disable SYSLIB0011 // Le type ou le membre est obsolète
                BinaryFormatter formatter = new BinaryFormatter();
#pragma warning restore SYSLIB0011 // Le type ou le membre est obsolète

                // Sérialisation du contrôle reçu en argument
                try
                {
                    formatter.Serialize(ms, controle);
                }
                catch (SerializationException ex)
                {
                    MessageBox.Show("Erreur de sérialisation : " + ex.Message);
                    return;
                }

                // Crée l'objet contenant les données sérialisées
                var controlData = new
                {
                    PictureBoxSerialized = ms.ToArray(),
                    Type = controle.GetType().AssemblyQualifiedName
                };

                using (MemoryStream dataStream = new MemoryStream())
                {
                    // Sérialisation de controlData dans le MemoryStream
                    formatter.Serialize(dataStream, controlData);

                    // Copier les données dans le presse-papiers avec un format personnalisé
                    Clipboard.SetData("PictureBoxFormat", dataStream.ToArray());
                }
            }
        }
        public Control? PasteFromClipboard()
        {
            //            if (!Clipboard.ContainsData("PictureBoxFormat")) return null;

            //            byte[] rawData = (byte[])Clipboard.GetData("PictureBoxFormat");
            //            using (MemoryStream dataStream = new MemoryStream(rawData))
            //            {
            //#pragma warning disable SYSLIB0011 // Le type ou le membre est obsolète
            //                BinaryFormatter formatter = new BinaryFormatter();
            //#pragma warning restore SYSLIB0011 // Le type ou le membre est obsolète
            //                Control data = (Control)formatter.Deserialize(dataStream);

            //                using (MemoryStream pictureBoxStream = new MemoryStream(data.PictureBoxSerialized))
            //                {
            //                    return (PictureBox)formatter.Deserialize(pictureBoxStream);
            //                }
            //            }

            return null;
        }
    }
}