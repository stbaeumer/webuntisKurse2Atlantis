using System;
using System.Collections.Generic;
using System.IO;

namespace webuntisKurse2Atlantis
{
    public class Studentgroups : List<Studentgroup>
    {
        public Studentgroups(string studentgroupStudents, List<DateTime> aktuellesHalbjahr)
        {
            using (StreamReader reader = new StreamReader(studentgroupStudents))
            {
                Console.Write("Schülergruppen aus Webuntis ".PadRight(30, '.'));

                while (true)
                {
                    string line = reader.ReadLine();
                    try
                    {
                        if (line != null)
                        {
                            Studentgroup studentgroup = new Studentgroup();
                            var x = line.Split('\t');
                            studentgroup.StudentId = Convert.ToInt32(x[0]);
                            studentgroup.Name = x[1];
                            studentgroup.Forename = x[2];
                            studentgroup.StudentgroupName = x[3];
                            studentgroup.Subject = x[4];
                            studentgroup.StartDate = x[5] == "" ? aktuellesHalbjahr[0] :  DateTime.ParseExact(x[5], "dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture);
                            studentgroup.EndDate = x[6] == "" ? aktuellesHalbjahr[1] : DateTime.ParseExact(x[6], "dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture);
                            //studentgroup.Kurzname = studentgroup.generateKurzname();
                            this.Add(studentgroup);
                        }                        
                    }
                    catch (Exception)
                    {
                    }

                    if (line == null)
                    {
                        break;
                    }
                }
                Console.WriteLine((" " + this.Count.ToString()).PadLeft(30, '.'));
            }
        }
    }
}