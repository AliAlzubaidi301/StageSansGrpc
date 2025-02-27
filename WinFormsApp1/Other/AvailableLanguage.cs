namespace StageCode.Other
{
    public class AvailableLanguage : IEquatable<AvailableLanguage>
    {
        private int _langID;

        private string _langName;

        public int LanguageID => _langID;

        public string LanguageName => _langName;

        public AvailableLanguage(int ID, string Name)
        {
            _langID = ID;
            _langName = Name;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            AvailableLanguage availableLanguage = (AvailableLanguage)obj;
            if ((object)availableLanguage == null)
            {
                return false;
            }

            return Equals1(availableLanguage);
        }

        public override int GetHashCode()
        {
            return _langID;
        }

        public bool Equals1(AvailableLanguage other)
        {
            if ((object)other == null)
            {
                return false;
            }

            return LanguageID == other.LanguageID;
        }

        bool IEquatable<AvailableLanguage>.Equals(AvailableLanguage other)
        {
            return this.Equals1(other);
        }

        public static object operator ==(AvailableLanguage Lang1, AvailableLanguage Lang2)
        {
            if ((object)Lang1 == null || (object)Lang2 == null)
            {
                return false;
            }

            return Lang1.Equals1(Lang2);
        }

        public static object operator !=(AvailableLanguage Lang1, AvailableLanguage Lang2)
        {
            return !Lang1.Equals(Lang2);
        }

        public override string ToString()
        {
            return _langName;
        }
    }
}