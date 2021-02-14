// Pubished und the terms of GPL3 by Stefan Bäumer 2021
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;

namespace webuntisKurse2Atlantis
{
    internal class Schuelers : List<Schueler>
    {
        public Schuelers(string connectionStringAtlantis, string aktSj)
        {
            Console.Write("Schüler aus Atlantis ".PadRight(30, '.'));
            try
            {
                using (OdbcConnection connection = new OdbcConnection(connectionStringAtlantis))
                {
                    DataSet dataSet = new DataSet();
                    OdbcDataAdapter schuelerAdapter = new OdbcDataAdapter(@"SELECT 
DBA.schue_sj.pu_id,
DBA.schue_sj.pj_id,
DBA.schue_sj.s_jahrgang,
DBA.adresse.s_typ_adr as Typ,
DBA.klasse.klasse as Klasse,
DBA.schueler.name_1,
DBA.schueler.name_2,
DBA.adresse.name_2 AS EVorname,
DBA.adresse.name_1 AS ENachname,
DBA.schueler.dat_geburt as Geburtsdatum,
DBA.schueler.s_geschl as Geschlecht,
DBA.adresse.strasse AS Strasse,
DBA.adresse.plz AS Plz,
DBA.adresse.ort AS Ort,
DBA.adresse.sorge_berechtigt_jn,
DBA.adresse.s_anrede,
DBA.schueler.s_erzb_1_art,
DBA.schueler.s_erzb_2_art,
DBA.schueler.id_hauptadresse,
DBA.adresse.hauptadresse_jn,
DBA.adresse.anrede_text,
DBA.schueler.anrede_text,
DBA.adresse.name_3,
DBA.adresse.plz_postfach as PlzPostfach,
DBA.adresse.postfach as Postfach,
DBA.adresse.s_titel_ad,
DBA.adresse.s_sorgerecht,
DBA.adresse.brief_adresse,
DBA.schue_sj.kl_id, 
DBA.adresse.s_famstand_adr
FROM((DBA.schue_sj JOIN DBA.klasse ON DBA.schue_sj.kl_id = DBA.klasse.kl_id) JOIN DBA.schueler ON DBA.schue_sj.pu_id = DBA.schueler.pu_id) JOIN DBA.adresse ON DBA.schueler.pu_id = DBA.adresse.pu_id
WHERE vorgang_schuljahr = '" + aktSj + @"';", connection);

                    connection.Open();
                    schuelerAdapter.Fill(dataSet, "DBA.klasse");

                    foreach (DataRow theRow in dataSet.Tables["DBA.klasse"].Rows)
                    {
                        try
                        {
                            Schueler schueler = new Schueler();
                            schueler.IdAtlantis = theRow["pu_id"] == null ? -99 : Convert.ToInt32(theRow["pu_id"]);
                            schueler.IdAtlantisSchuljahr = theRow["pj_id"] == null ? -99 : Convert.ToInt32(theRow["pj_id"]);
                            schueler.Jahrgang = theRow["s_jahrgang"] == null ? "" : theRow["s_jahrgang"].ToString();
                            schueler.Nachname = theRow["name_1"] == null ? "" : theRow["name_1"].ToString();
                            schueler.Vorname = theRow["name_2"] == null ? "" : theRow["name_2"].ToString();
                            this.Add(schueler);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
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
    }
}
