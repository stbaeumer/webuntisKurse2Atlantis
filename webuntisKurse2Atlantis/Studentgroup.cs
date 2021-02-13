using System;
using System.Text.RegularExpressions;

namespace webuntisKurse2Atlantis
{
    public class Studentgroup
    {
        private object x;        
        public int StudentId { get; internal set; }
        public string Name { get; internal set; }
        public string Forename { get; internal set; }
        public string StudentgroupName { get; internal set; }
        public string Subject { get; internal set; }
        public DateTime StartDate { get; internal set; }
        public DateTime EndDate { get; internal set; }
        public string Kurzname { get; internal set; }

        internal string generateKurzname()
        {
            return fBereinigen(Name) + "." + fBereinigen(Forename) + "." + StudentId;
        }

        string fBereinigen(string Textinput)
        {
            string Text = Textinput;

            Text = Text.ToLower();                          // Nur Kleinbuchstaben
            Text = fUmlauteBehandeln(Text);                 // Umlaute ersetzen


            Text = Regex.Replace(Text, "-", "_");           //  kein Minus-Zeichen
            Text = Regex.Replace(Text, ",", "_");           //  kein Komma            
            Text = Regex.Replace(Text, " ", "_");           //  kein Leerzeichen
            // Text = Regex.Replace(Text, @"[^\w]", string.Empty);   // nur Buchstaben

            Text = Regex.Replace(Text, "[^a-z]", string.Empty);   // nur Buchstaben

            Text = Text.Substring(0, Math.Min(6, Text.Length));  // Auf maximal 6 Zeichen begrenzen
            return Text;
        }
        string fUmlauteBehandeln(string Textinput)
        {
            string Text = Textinput;

            // deutsche Sonderzeichen
            Text = Regex.Replace(Text, "[æ|ä]", "ae");
            Text = Regex.Replace(Text, "[Æ|Ä]", "Ae");
            Text = Regex.Replace(Text, "[œ|ö]", "oe");
            Text = Regex.Replace(Text, "[Œ|Ö]", "Oe");
            Text = Regex.Replace(Text, "[ü]", "ue");
            Text = Regex.Replace(Text, "[Ü]", "Ue");
            Text = Regex.Replace(Text, "ß", "ss");

            // Sonderzeichen aus anderen Sprachen
            Text = Regex.Replace(Text, "[ã|à|â|á|å]", "a");
            Text = Regex.Replace(Text, "[Ã|À|Â|Á|Å]", "A");
            Text = Regex.Replace(Text, "[é|è|ê|ë]", "e");
            Text = Regex.Replace(Text, "[É|È|Ê|Ë]", "E");
            Text = Regex.Replace(Text, "[í|ì|î|ï]", "i");
            Text = Regex.Replace(Text, "[Í|Ì|Î|Ï]", "I");
            Text = Regex.Replace(Text, "[õ|ò|ó|ô]", "o");
            Text = Regex.Replace(Text, "[Õ|Ó|Ò|Ô]", "O");
            Text = Regex.Replace(Text, "[ù|ú|û|µ]", "u");
            Text = Regex.Replace(Text, "[Ú|Ù|Û]", "U");
            Text = Regex.Replace(Text, "[ý|ÿ]", "y");
            Text = Regex.Replace(Text, "[Ý]", "Y");
            Text = Regex.Replace(Text, "[ç|č]", "c");
            Text = Regex.Replace(Text, "[Ç|Č]", "C");
            Text = Regex.Replace(Text, "[Ð]", "D");
            Text = Regex.Replace(Text, "[ñ]", "n");
            Text = Regex.Replace(Text, "[Ñ]", "N");
            Text = Regex.Replace(Text, "[š]", "s");
            Text = Regex.Replace(Text, "[Š]", "S");

            return Text;

        }
    }
}