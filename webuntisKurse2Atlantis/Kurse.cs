// Pubished und the terms of GPL3 by Stefan Bäumer 2021using System;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Linq;

namespace webuntisKurse2Atlantis
{
    internal class Kurse : List<Kurs>
    {
        public Kurse(Studentgroups webuntisStudentgroups, Schuelers schueler, Fachs fachs, string aktSJ)
        {
            Console.Write("Webuntis-Kurse ".PadRight(30, '.'));
            
            Kurse unsortiert = new Kurse();

            var beschreibung = true;

            foreach (var w in webuntisStudentgroups)
            {
                Kurs kurs;

                try
                {
                    kurs = new Kurs();
                    kurs.NameUntis = w.StudentgroupName.Substring(0,Math.Min(20,w.StudentgroupName.Length));
                    kurs.Jahrgang = (from s in schueler where s.IdAtlantis == w.StudentId select s.Jahrgang).FirstOrDefault();
                    kurs.IdFach = (from f in fachs where f.UntisNamen.Contains(w.Subject) select f.IdAtlantis).FirstOrDefault();
                    kurs.Fach = (from f in fachs where f.UntisNamen.Contains(w.Subject) select f.Kürzel).FirstOrDefault();
                    kurs.Kurstitel = (from f in fachs where f.UntisNamen.Contains(w.Subject) select f.Kürzel).FirstOrDefault();
                    kurs.NameUntis = w.StudentgroupName.Substring(0, Math.Min(20, w.StudentgroupName.Length));
                    kurs.Klassen = kurs.NameUntis.Split('_')[1];
                    kurs.Halbjahr = DateTime.Now.Month > 1 && DateTime.Now.Month <= 7 ? 2 : 1;
                    kurs.Schuljahr = aktSJ;                    
                    kurs.Druckname = kurs.Fach;
                    kurs.Art = "";

                    if (!(from t in unsortiert where t.NameUntis == kurs.NameUntis select t).Any())
                    {
                        if (kurs.IdFach == 0)
                        {
                            if (beschreibung)
                            {
                                UpdateKurs("Bei folgenden Untis-Kursen können in Atlantis keine korrespondierenden Fächer zugeordnet werden.");
                                UpdateKurs("Das kann gelöst werden, indem die Fächer entweder in Atlantis neu angelegt oder umbenannt werden oder es wird unter");
                                UpdateKurs("*Zeugnisse/Fach* bei dem gewünschten Fach in Atlantis ein (weiteres) Untis Kürzel hinzugefügt:");
                                UpdateKurs("");
                                beschreibung = false;
                            }
                            UpdateKurs("   Beim Untis-Kurs " + w.StudentgroupName + " kann das Untis-Fach " + w.Subject + " in Atlantis keinem Fach zugeordnet werden.");
                        }
                        else
                        {
                            kurs.Art = kurs.Jahrgang.StartsWith("D") && kurs.IdFach != 0 && kurs.Fach.Contains(" ") ? (kurs.Fach.Replace("  ", " ")).Split(' ')[1].Substring(0, 1) : "";
                        }
                        
                        unsortiert.Add(kurs);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }                                
            }
            this.AddRange(unsortiert.OrderBy(x => x.Jahrgang).ThenBy(x => x.Klassen));
            Console.WriteLine((" " + this.Count.ToString()).PadLeft(30, '.'));
        }

        public Kurse(string connectionStringAtlantis, string aktSj)
        {
            Console.Write("Kurse aus Atlantis ".PadRight(30, '.'));

            try
            {
                using (OdbcConnection connection = new OdbcConnection(connectionStringAtlantis))
                {
                    DataSet dataSet = new DataSet();
                    OdbcDataAdapter schuelerAdapter = new OdbcDataAdapter(@"SELECT DBA.kurs.ku_id,
DBA.kurs.programm_nr,
DBA.kurs.gruppen_nr_name,
DBA.kurs.kurs_titel,
DBA.kurs.id_fach,
DBA.kurs.s_periode,
DBA.kurs.s_gliederungsplan_ku,
DBA.kurs.s_jahrgang,
DBA.kurs.alias_name_1 AS Untisname,
DBA.kurs.alias_name_2,
DBA.kurs.notenblatt_relevant_jn
FROM DBA.kurs
WHERE programm_nr = '" + aktSj + "' ORDER BY DBA.kurs.ku_id DESC", connection);

                    connection.Open();
                    schuelerAdapter.Fill(dataSet, "DBA.klasse");

                    foreach (DataRow theRow in dataSet.Tables["DBA.klasse"].Rows)
                    {
                        try
                        {
                            Kurs kurs = new Kurs();
                            kurs.Ku_Id = theRow["ku_id"] == null ? -99 : Convert.ToInt32(theRow["ku_id"]);
                            kurs.NameAtlantis = theRow["gruppen_nr_name"] == null ? "" : theRow["gruppen_nr_name"].ToString();
                            kurs.Kurstitel = theRow["kurs_titel"] == null ? "" : theRow["kurs_titel"].ToString();
                            kurs.IdFach = theRow["id_fach"] == null ? -99 : Convert.ToInt32(theRow["id_fach"]);
                            kurs.Jahrgang = theRow["s_jahrgang"] == null ? "" : theRow["s_jahrgang"].ToString();
                            kurs.Art = theRow["s_gliederungsplan_ku"] == null ? "" : theRow["s_gliederungsplan_ku"].ToString();
                            kurs.Halbjahr = theRow["s_periode"] == null ? -99 : Convert.ToInt32(theRow["s_periode"]);
                            kurs.NameUntis = theRow["Untisname"] == null ? "" : theRow["Untisname"].ToString();
                            kurs.Druckname = theRow["alias_name_2"] == null ? "" : theRow["alias_name_2"].ToString();
                            kurs.Schuljahr = theRow["programm_nr"] == null ? "" : theRow["programm_nr"].ToString();

                            // Bei Namensgleichheit wird nur der zuletzt angelegte Kurs eingefügt.

                            if (!(from t in this where t.NameUntis == kurs.NameUntis select t).Any())
                            {
                                this.Add(kurs);
                            }
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            
            Console.WriteLine((" " + this.Count.ToString()).PadLeft(30, '.'));
        }

        public Kurse()
        {
        }

        internal void Add(Kurse atlantisKurse)
        {
            try
            {
                UpdateKurs("", "");
                UpdateKurs("Anzulegende Kurse:", "");
                UpdateKurs("Untis-Kursname |Art|Hj.|Jahrg.", "");

                foreach (var w in this)
                {
                    if (!(from a in atlantisKurse where a.NameUntis == w.NameUntis where a.Schuljahr == w.Schuljahr where a.Halbjahr == w.Halbjahr select a).Any())
                    {
                        UpdateKurs((w.NameUntis.Substring(0, Math.Min(w.NameUntis.Length, 13))).PadRight(14) + " | " + (w.Art).PadRight(1) + " | " + w.Halbjahr + " | " + w.Jahrgang, @"INSERT INTO kurs(kurs_titel, s_jahrgang, alias_name_2, alias_name_1, s_periode, s_gliederungsplan_ku, id_fach, programm_nr, sc_id) VALUES('" + w.NameUntis + "','" + w.Jahrgang + "','" + w.NameUntis + "','" + w.NameUntis + "','" + w.Halbjahr + "','" + w.Art + "'," + w.IdFach + ",'" + w.Schuljahr + "',38);");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        internal void Delete(Kurse webuntisKurse)
        {
            try
            {
                UpdateKurs("", "");
                UpdateKurs("Zu löschende Kurse:", "");
                UpdateKurs("", "");

                foreach (var a in this)
                {
                    if (!(from w in webuntisKurse where w.NameUntis == a.NameUntis where w.Schuljahr == a.Schuljahr where w.Halbjahr == a.Halbjahr select w).Any())
                    {
                        // Kurse aus anderen Halbjahren werden nicht angefasst

                        if (a.Halbjahr == (DateTime.Now.Month > 1 && DateTime.Now.Month <= 7 ? 2 : 1))
                        {
                            var dd = (from w in webuntisKurse where w.NameUntis.StartsWith(a.NameUntis) where w.Schuljahr == a.Schuljahr where w.Halbjahr == a.Halbjahr select w).FirstOrDefault();
                            UpdateKurs(a.NameUntis + " | " + a.Halbjahr + ".Hj", @"DELETE FROM kurs WHERE ku_id = " + a.Ku_Id + ";");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }            
        }

        private void UpdateKurs(string message, string updateQuery)
        {
            try
            {
                Global.Output.Add("/* " + message.Substring(0, Math.Min(32, message.Length)).PadRight(32) + " */ " + updateQuery);                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private void UpdateKurs(string message)
        {
            try
            {
                Global.Output.Add("/* " + message.Substring(0, Math.Min(200, message.Length)).PadRight(200) + " */ ");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}