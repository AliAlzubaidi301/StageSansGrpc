using System;
using System.Collections.Generic;

namespace StageCode.Other
{
    public class Langs
    {
        public delegate void LanguageChangedEventHandler(AvailableLanguage NewLanguage);

        private static AvailableLanguage _curLang;
        private static List<AvailableLanguage> _avLanguages = new List<AvailableLanguage>();

        public static AvailableLanguage CurrentLanguage
        {
            get
            {
                return _curLang;
            }
            set
            {
                if ((bool)(_curLang != value))
                {
                    _curLang = value;
                    LanguageChanged?.Invoke(value);
                }
            }
        }

        public static List<AvailableLanguage> AvailableLanguages
        {
            get
            {
                return _avLanguages;
            }
            set
            {
                if (value == null || value.Count == 0)
                {
                    throw new ArgumentException("No Language in list", nameof(value));
                }

                _avLanguages = value;
                bool flag = false;

                foreach (AvailableLanguage avLanguage in _avLanguages)
                {
                    if ((bool)(_curLang == avLanguage)) // Comparaison directe de l'objet
                    {
                        _curLang = avLanguage;
                        flag = true;
                        break;
                    }
                }

                // Si aucune langue courante n'a été trouvée, on définit la première langue
                if (!flag)
                {
                    CurrentLanguage = _avLanguages[0];
                }
            }
        }

        public static event LanguageChangedEventHandler LanguageChanged;

        public static AvailableLanguage FindLang(string LangName)
        {
            // Recherche de la langue par nom avec comparaison insensible à la casse
            return AvailableLanguages.FirstOrDefault(availableLanguage =>
                string.Equals(availableLanguage.LanguageName, LangName, StringComparison.OrdinalIgnoreCase));
        }
    }
}
