// Pubished und the terms of GPL3 by Stefan Bäumer 2021

using System;
using System.Collections.Generic;
using System.IO;

namespace webuntisKurse2Atlantis
{
    public class Studentgroups : List<Studentgroup>
    {
        public Studentgroups(string studentgroupStudents, List<DateTime> aktuellesHalbjahr)
        {
            Console.Write("Schülergruppen aus Webuntis ".PadRight(30, '.'));
            try
            {
                using (StreamReader reader = new StreamReader(studentgroupStudents))
                {
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
                                studentgroup.StartDate = x[5] == "" ? aktuellesHalbjahr[0] : DateTime.ParseExact(x[5], "dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture);
                                studentgroup.EndDate = x[6] == "" ? aktuellesHalbjahr[1] : DateTime.ParseExact(x[6], "dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture);
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
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            Console.WriteLine((" " + this.Count.ToString()).PadLeft(30, '.'));
        }
    }
}