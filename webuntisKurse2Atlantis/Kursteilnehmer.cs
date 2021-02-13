using System;

namespace webuntisKurse2Atlantis
{
    internal class Kursteilnehmer
    {        
        /// <summary>
        /// Kurs-ID aus Atlantis
        /// </summary>
        public int Ku_Id { get; internal set; }
        public string Abiturfach { get; internal set; }
        /// <summary>
        /// Id des Schülers in diesem Schuljahr
        /// </summary>
        public int Pj_Id { get; internal set; }
        public string KursNameUntis { get; internal set; }
        /// <summary>
        /// Atlantis-ID des Schülers
        /// </summary>
        public int Pu_Id { get; internal set; }
        public string Nachname { get; internal set; }
        public string Vorname { get; internal set; }
        public int KuPj_Id { get; internal set; }
        public DateTime Startdate { get; internal set; }
        public DateTime Enddate { get; internal set; }
    }
}