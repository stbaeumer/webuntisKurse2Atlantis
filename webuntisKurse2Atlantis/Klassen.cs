using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;

namespace webuntisKurse2Atlantis
{
    internal class Klassen : List<Klasse>
    {
        public Klassen(string connectionStringAtlantis, string aktSj)
        {
            using (OdbcConnection connection = new OdbcConnection(connectionStringAtlantis))
            {
                DataSet dataSet = new DataSet();
                OdbcDataAdapter schuelerAdapter = new OdbcDataAdapter(@"SELECT DBA.klasse.kl_id,
DBA.klasse.klasse,
DBA.klasse.schul_jahr,
DBA.klasse.jahrgang,
DBA.klasse.s_klasse_art AS Klassenart,
DBA.klasse.s_uorg,
DBA.klasse.s_gliederungsplan_kl,
DBA.klasse.s_bildungsgang
FROM DBA.klasse
WHERE schul_jahr = '" + aktSj + "' ORDER BY DBA.klasse.klasse ASC", connection);

                connection.Open();
                schuelerAdapter.Fill(dataSet, "DBA.klasse");

                foreach (DataRow theRow in dataSet.Tables["DBA.klasse"].Rows)
                {
                    try
                    {
                        Klasse klasse = new Klasse();
                        klasse.IdAtlantis = theRow["kl_id"] == null ? -99 : Convert.ToInt32(theRow["kl_id"]);
                        klasse.NameAtlantis = theRow["klasse"] == null ? "" : theRow["klasse"].ToString();
                        klasse.Gliederung = theRow["s_uorg"] == null ? "" : theRow["s_uorg"].ToString();
                        klasse.OrgForm = theRow["Klassenart"] == null ? "" : theRow["Klassenart"].ToString();
                        string jahrgang = theRow["jahrgang"].ToString();
                        klasse.Jahrgang = theRow["jahrgang"] == null || klasse.Gliederung == "" ? "" : "0" + theRow["jahrgang"].ToString().Replace(klasse.Gliederung, "");
                        klasse.Anlage = theRow["s_uorg"] == null ? "" : theRow["s_uorg"].ToString().Substring(0, Math.Min(1, klasse.Jahrgang.Length));
                        klasse.Gliederungsplan = theRow["s_gliederungsplan_kl"] == null ? "" : theRow["s_gliederungsplan_kl"].ToString();
                        this.Add(klasse);
                    }
                    catch (Exception ex)
                    {

                    }
                }
                connection.Close();
            }
        }
    }
}