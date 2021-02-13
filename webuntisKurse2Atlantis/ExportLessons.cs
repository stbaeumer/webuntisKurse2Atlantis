using System;
using System.Collections.Generic;
using System.IO;

namespace webuntisKurse2Atlantis
{
    public class ExportLessons : List<ExportLesson>
    {
        private string exportLessons;

        public ExportLessons(string exportLessons)
        {
            using (StreamReader reader = new StreamReader(exportLessons))
            {
                Console.Write("Unterrichte aus Webuntis ".PadRight(30, '.'));

                while (true)
                {
                    string line = reader.ReadLine();
                    try
                    {
                        ExportLesson exportLesson = new ExportLesson();
                        var x = line.Split('\t');
                        exportLesson.LessonId = Convert.ToInt32(x[0]);
                        exportLesson.LessonNumber = Convert.ToInt32(x[1]) / 100;
                        exportLesson.Subject = x[2];
                        exportLesson.Teacher = x[3];
                        exportLesson.Klassen = x[4];
                        exportLesson.Studentgroup = x[5];
                        exportLesson.Periods = x[6];
                        exportLesson.Startdate = x[7];
                        exportLesson.EndDate = x[8];
                        exportLesson.Room = x[9];
                        exportLesson.Foreignkey = x[9];
                        this.Add(exportLesson);
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