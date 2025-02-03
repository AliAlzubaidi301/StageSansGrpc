using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StageCode.LIB
{
    public class CONT1 : UserControl
    {
        private int _LevelVisible = 0;      // Niveau d'accès minimum pour rendre l'objet visible
        private int _LevelEnabled = 0;      // Niveau d'accès minimum pour rendre l'objet accessible
        private string _comment = "";       // Commentaire sur le contrôle
        private string _visibility = "";    // (Champ de visibilité)

        #region Control Data
        private string _Detecteur;
        #endregion

        public event EventHandler VisibilityChanging;
        public event EventHandler VisibilityChanged;

    }
}
