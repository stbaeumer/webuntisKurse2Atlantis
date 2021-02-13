using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Linq;

namespace webuntisKurse2Atlantis
{
    internal class Kursteilnehmers : List<Kursteilnehmer>
    {
        public Kursteilnehmers(Studentgroups webuntisStudentgroups, Schuelers schuelers)
        {
            foreach (var w in webuntisStudentgroups)
            {
                Kursteilnehmer kursteilnehmer = new Kursteilnehmer();
                kursteilnehmer.Pu_Id = Convert.ToInt32(w.StudentId); 
                kursteilnehmer.Pj_Id = (from s in schuelers where s.IdAtlantis == w.StudentId select s.IdAtlantisSchuljahr).FirstOrDefault();
                kursteilnehmer.KursNameUntis = w.StudentgroupName;                
                kursteilnehmer.Nachname = (from s in schuelers where w.StudentId == s.IdAtlantis select s.Nachname).FirstOrDefault();
                kursteilnehmer.Vorname = (from s in schuelers where w.StudentId == s.IdAtlantis select s.Vorname).FirstOrDefault();
                kursteilnehmer.Startdate = w.StartDate;
                kursteilnehmer.Enddate = w.EndDate;
                this.Add(kursteilnehmer);
            }
        }

        public Kursteilnehmers(string connectionStringAtlantis, string aktSj, Schuelers schuelers, List<DateTime> aktuellesHalbjahr)
        {
            using (OdbcConnection connection = new OdbcConnection(connectionStringAtlantis))
            {
                DataSet dataSet = new DataSet();
                OdbcDataAdapter schuelerAdapter = new OdbcDataAdapter(@"SELECT DBA.kurs.ku_id,
DBA.kurs.programm_nr,
DBA.ku_pj.kupj_id,
DBA.ku_pj.pj_id,
DBA.ku_pj.datum,
DBA.ku_pj.datum_2,
DBA.ku_pj.s_abiturfach,
DBA.schueler.name_1,
DBA.schueler.name_2,
DBA.schue_sj.sc_id,
DBA.kurs.alias_name_1
FROM((DBA.kurs JOIN DBA.ku_pj ON DBA.kurs.ku_id = DBA.ku_pj.ku_id) JOIN DBA.schue_sj ON DBA.ku_pj.pj_id = DBA.schue_sj.pj_id) JOIN DBA.schueler ON DBA.schue_sj.pu_id = DBA.schueler.pu_id
WHERE programm_nr = '" + aktSj + "'", connection);

                connection.Open();
                schuelerAdapter.Fill(dataSet, "DBA.klasse");

                foreach (DataRow theRow in dataSet.Tables["DBA.klasse"].Rows)
                {
                    try
                    {
                        var kursteilnehmer = new Kursteilnehmer();
                        kursteilnehmer.KuPj_Id = theRow["kupj_id"] == null ? -99 : Convert.ToInt32(theRow["kupj_id"]);
                        kursteilnehmer.Ku_Id = theRow["ku_id"] == null ? -99 : Convert.ToInt32(theRow["ku_id"]);
                        kursteilnehmer.Pj_Id = theRow["pj_id"] == null ? -99 : Convert.ToInt32(theRow["pj_id"]);
                        kursteilnehmer.Startdate = theRow["datum"].ToString().Length < 3 ? aktuellesHalbjahr[0] : DateTime.ParseExact(theRow["datum"].ToString(), "dd.MM.yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                        kursteilnehmer.Enddate = theRow["datum_2"].ToString().Length < 3 ? aktuellesHalbjahr[1] : DateTime.ParseExact(theRow["datum_2"].ToString(), "dd.MM.yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                        kursteilnehmer.KursNameUntis = theRow["alias_name_1"] == null ? "" : theRow["alias_name_1"].ToString();
                        kursteilnehmer.Nachname = theRow["name_1"] == null ? "" : theRow["name_1"].ToString();
                        kursteilnehmer.Vorname = theRow["name_2"] == null ? "" : theRow["name_2"].ToString();
                        this.Add(kursteilnehmer);
                    }
                    catch (Exception ex)
                    {
                    }
                }
                connection.Close();
            }
        }
               

        internal void Add(Kursteilnehmers atlantisKursteilnehmer, Kurse atlantisKurse)
        {
            UpdateKursteilnehmer("", "");
            UpdateKursteilnehmer("Anzulegende Kursteilnehmer:", "");
            UpdateKursteilnehmer("", "");

            foreach (var w in this)
            {

                int ku_id = (from a in atlantisKurse where a.NameUntis == w.KursNameUntis select a.Ku_Id).FirstOrDefault();

                if (ku_id != 0)
                {
                    if (!(from a in atlantisKursteilnehmer where a.Pj_Id == w.Pj_Id where a.Ku_Id == ku_id select a).Any())
                    {
                        UpdateKursteilnehmer(w.KursNameUntis + "|" + w.Nachname + "," + w.Vorname, @"INSERT INTO ku_pj(ku_id, pj_id,datum,datum_2) VALUES(" + ku_id + "," + w.Pj_Id + ",'" + w.Startdate.ToString("yyyy-MM-dd") + "'," + (w.Enddate <= DateTime.Now ? "'" + w.Enddate.ToString("yyyy-MM-dd") + "'" : "NULL" ) + ");");
                    }
                }
            }
        }

        internal void Update(Kursteilnehmers atlantisKursteilnehmer, Kurse atlantisKurse)
        {
            UpdateKursteilnehmer("", "");
            UpdateKursteilnehmer("Zu aktualisierende Kursteilnehmer:", "");
            UpdateKursteilnehmer("", "");

            foreach (var w in this)
            {

                int ku_id = (from a in atlantisKurse where a.NameUntis == w.KursNameUntis select a.Ku_Id).FirstOrDefault();

                if (ku_id != 0)
                {
                    var at = (from a in atlantisKursteilnehmer where a.Pj_Id == w.Pj_Id where a.Ku_Id == ku_id select a).FirstOrDefault();

                    if (at != null)
                    {
                        if (at.Startdate != w.Startdate)
                        {
                            UpdateKursteilnehmer(w.KursNameUntis.Substring(0,7) + "|" + w.Nachname.Substring(0, 2) + "," + w.Vorname.Substring(0, 2) + "|" + at.Startdate.ToString("dd.MM.yy") + "->" + w.Startdate.ToString("dd.MM.yy"), @"UPDATE ku_pj SET datum = '" + w.Startdate.ToString("yyyy-MM-dd") + "' WHERE ku_id = " + ku_id + " AND pj_id = " + w.Pj_Id + ";");
                        }
                        if (at.Enddate != w.Enddate)
                        {
                            UpdateKursteilnehmer(w.KursNameUntis.Substring(0, 7) + "|" + w.Nachname.Substring(0, 2) + "," + w.Vorname.Substring(0, 2) + "|" + (at.Enddate <= DateTime.Now ? at.Enddate.ToString("dd.MM.yy") : "NULL") + "->" + w.Enddate.ToString("dd.MM.yy"), @"UPDATE ku_pj SET datum_2 = " + (w.Enddate <= DateTime.Now ? "'" + w.Enddate.ToString("yyyy-MM-dd") + "'" : "NULL") + " WHERE ku_id = " + ku_id + " AND pj_id = " + w.Pj_Id + ";");
                        }
                    }
                }
            }
        }

        internal void Delete(Kursteilnehmers webuntiskursteilnehmers)
        {
            UpdateKursteilnehmer("", "");
            UpdateKursteilnehmer("Zu löschende Kursteilnehmer:", "");
            UpdateKursteilnehmer("", "");

            foreach (var a in this)
            {
                if (!(from w in webuntiskursteilnehmers where w.Pj_Id == a.Pj_Id where w.KursNameUntis == a.KursNameUntis select w).Any())
                {
                    UpdateKursteilnehmer(a.KursNameUntis + "|" + a.Nachname + "," + a.Vorname, @"DELETE FROM ku_pj WHERE kupj_id = " + a.KuPj_Id + ";");
                }
            }
        }

        private void UpdateKursteilnehmer(string message, object updateQuery)
        {
            try
            {
                Global.Output.Add("/* " + message.Substring(0, Math.Min(32, message.Length)).PadRight(32) + " */ " + updateQuery);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}