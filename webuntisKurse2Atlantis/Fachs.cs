// Pubished und the terms of GPL3 by Stefan Bäumer 2021
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Linq;

namespace webuntisKurse2Atlantis
{
    internal class Fachs : List<Fach>
    {
        public Fachs(string connectionstring)
        {
            Console.Write("Fächer aus Webuntis ".PadRight(30, '.'));
            
            try
            {
                using (OdbcConnection connection = new OdbcConnection(connectionstring))
                {

                    DataSet dataSet = new DataSet();
                    OdbcDataAdapter schuelerAdapter = new OdbcDataAdapter(@"SELECT DBA.fach.fa_id,
DBA.fach.kuerzel,
DBA.fach_info.bezeichnung_1 AS Untisname
FROM DBA.fach LEFT OUTER JOIN DBA.fach_info ON DBA.fach.fa_id = DBA.fach_info.fa_id
WHERE aktiv_jn = 'j'
ORDER BY DBA.fach.kuerzel ASC ", connection);

                    connection.Open();
                    schuelerAdapter.Fill(dataSet, "DBA.fach");

                    foreach (DataRow theRow in dataSet.Tables["DBA.fach"].Rows)
                    {
                        Fach fach = new Fach();
                        fach.UntisNamen = new List<string>();
                        fach.IdAtlantis = theRow["fa_id"] == null ? -99 : Convert.ToInt32(theRow["fa_id"]);
                        fach.Kürzel = theRow["kuerzel"].ToString();

                        // Wenn es das Fach noch nicht gibt, wird es hinzugefügt.

                        if (!(from f in this where f.Kürzel == fach.Kürzel select f).Any())
                        {
                            if (theRow["Untisname"].ToString() != null && theRow["Untisname"].ToString() != "")
                            {
                                fach.UntisNamen.Add(theRow["Untisname"].ToString());
                            }

                            fach.UntisNamen.Add(theRow["kuerzel"].ToString());
                            this.Add(fach);
                        }
                        else
                        {
                            var x = (from f in this where f.Kürzel == fach.Kürzel select f).FirstOrDefault();

                            if (theRow["Untisname"].ToString() != null && theRow["Untisname"].ToString() != "")
                            {
                                if (!x.UntisNamen.Contains(theRow["Untisname"].ToString()))
                                {
                                    x.UntisNamen.Add(theRow["Untisname"].ToString());
                                }
                            }
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