// Pubished und the terms of GPL3 by Stefan Bäumer 2021
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace webuntisKurse2Atlantis
{
    class Program
    {
        public static string User = System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToUpper().Split('\\')[1];
        public static string Zeitstempel = DateTime.Now.ToString("yyMMdd-HHmmss");
        public static List<string> AktSj = new List<string>
                {
                    (DateTime.Now.Month >= 8 ? DateTime.Now.Year : DateTime.Now.Year - 1).ToString(),
                    (DateTime.Now.Month >= 8 ? DateTime.Now.Year + 1 - 2000 : DateTime.Now.Year - 2000).ToString()
                };
        public static List<DateTime> aktuellesHalbjahr = new List<DateTime>
                {
                    DateTime.Now.Month >= 8 || DateTime.Now.Month < 2 ? new DateTime(Convert.ToInt32(AktSj[0]), 8, 1) : new DateTime((Convert.ToInt32(AktSj[0]) + 1), 2, 1),
                    DateTime.Now.Month >= 8 || DateTime.Now.Month < 2 ? new DateTime((Convert.ToInt32(AktSj[0]) + 1), 1, 31) : new DateTime((Convert.ToInt32(AktSj[0]) + 1), 7, 31)
                };

        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine(" WebuntisKurse2Atlantis | Published under the terms of GPLv3 | Stefan Bäumer " + DateTime.Now.Year + " | Version 20210208");
                Console.WriteLine("=====================================================================================================");
                Console.WriteLine(" Mit *WebuntisKurse2Atlantis* kann eine SQL-Datei erstellt werden, die die Befehle für die Neuanlage,");
                Console.WriteLine(" ein Update oder das Löschen von Kursen und Kursteilnehmern enthält. Die Datei kann dann in Atlantis ");
                Console.WriteLine(" unter *Funktionen/SQL-Anweisung ausführen* in Atlantis in die Datenbank geschrieben werden.         ");
                Console.WriteLine("=====================================================================================================");
                Console.WriteLine("");
                Global.Output = new List<string>();
                string targetPath = SetTargetPath();

                string studentgroupStudents = CheckFile(targetPath, User, "StudentgroupStudents");
                string targetSql = Path.Combine(targetPath, Zeitstempel + "_webuntiskurse2atlantis_" + User + ".SQL");

                Console.WriteLine("Aktuelles Schuljahr: " + AktSj[0] + "/" + AktSj[1]);
                Console.WriteLine("Aktuelles Halbjahr:  " + aktuellesHalbjahr[0].ToShortDateString() + "-" + aktuellesHalbjahr[1].ToShortDateString());
                Console.WriteLine("");
                Studentgroups webuntisStudentgroups = new Studentgroups(studentgroupStudents, aktuellesHalbjahr);

                Klassen atlantisKlassen = new Klassen(@"Dsn=Atlantis9;uid=DBA", AktSj[0] + "/" + AktSj[1]);
                Schuelers schuelers = new Schuelers(@"Dsn=Atlantis9;uid=DBA", AktSj[0] + "/" + AktSj[1]);
                Fachs fachs = new Fachs(@"Dsn=Atlantis9;uid=DBA");
                Kurse webuntisKurse = new Kurse(webuntisStudentgroups, schuelers, fachs, AktSj[0] + "/" + AktSj[1]);
                Kursteilnehmers webuntiskursteilnehmers = new Kursteilnehmers(webuntisStudentgroups, schuelers);

                Kurse atlantisKurse = new Kurse(@"Dsn=Atlantis9;uid=DBA", AktSj[0] + "/" + AktSj[1]);
                Kursteilnehmers atlantisKursteilnehmer = new Kursteilnehmers(@"Dsn=Atlantis9;uid=DBA", AktSj[0] + "/" + AktSj[1], schuelers, aktuellesHalbjahr);

                // Sortieren


                atlantisKurse.OrderBy(x => x.Jahrgang).ThenBy(x => x.Kurstitel);

                // CRUD

                webuntisKurse.Add(atlantisKurse);
                atlantisKurse.Delete(webuntisKurse);

                webuntiskursteilnehmers.Add(atlantisKursteilnehmer, atlantisKurse, aktuellesHalbjahr);
                webuntiskursteilnehmers.Update(atlantisKursteilnehmer, atlantisKurse);
                atlantisKursteilnehmer.Delete(webuntiskursteilnehmers);

                ErzeugeSqlDatei(new List<string>() { studentgroupStudents, targetSql });

                Console.WriteLine("");
                Console.WriteLine("Verarbeitung beendet. ENTER beendet das Programm");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }            
        }

        private static string SetTargetPath()
        {
            var pfad = @"\\fs01\SoftwarehausHeider\webuntisKurse2Atlantis\Dateien";

            try
            {
                if (Properties.Settings.Default.Pfad != "")
                {
                    pfad = Properties.Settings.Default.Pfad;
                }

                if (!Directory.Exists(pfad))
                {
                    do
                    {
                        Console.WriteLine(" Wo sollen die Dateien gespeichert werden? [ " + pfad + " ]");
                        pfad = Console.ReadLine();
                        if (pfad == "")
                        {
                            pfad = @"\\fs01\SoftwarehausHeider\webuntisNoten2Atlantis\Dateien";
                        }
                        try
                        {
                            Directory.CreateDirectory(pfad);
                            Properties.Settings.Default.Pfad = pfad;
                        }
                        catch (Exception)
                        {

                            Console.WriteLine("Der Pfad " + pfad + " kann nicht angelegt werden.");
                        }

                    } while (!Directory.Exists(pfad));
                }

                Console.WriteLine("Alle Dateien werden unter " + pfad + " abgelegt.");

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            
            return pfad;
        }

        public static string CheckFile(string targetPath, string user, string kriterium)
        {
            var sourceFile = (from f in Directory.GetFiles(@"c:\users\" + user + @"\Downloads", "*.csv", SearchOption.AllDirectories) where f.Contains(kriterium) orderby File.GetCreationTime(f) select f).LastOrDefault();

            if ((sourceFile == null || System.IO.File.GetLastWriteTime(sourceFile).Date != DateTime.Now.Date))
            {
                Console.WriteLine("");
                Console.WriteLine("  Die Datei " + kriterium + "<...>.CSV" + (sourceFile == null ? " existiert nicht im Download-Ordner" : " im Download-Ordner ist nicht von heute") + ".");
                Console.WriteLine("  Exportieren Sie die Datei frisch aus Webuntis, indem Sie als Administrator:");

                if (kriterium.Contains("StudentgroupStudents"))
                {
                    Console.WriteLine("1. sich als admin anmelden");
                    Console.WriteLine("2. auf Administration > Export klicken");
                    Console.WriteLine("3. Schülerguppen als CSV exportieren nach " + targetPath);
                    Console.WriteLine("ENTER beendet das Programm.");
                    Console.ReadKey();
                    Environment.Exit(0);
                }

                if (kriterium.Contains("ExportLessons"))
                {
                    Console.WriteLine("1. sich als admin anmelden");
                    Console.WriteLine("2. auf Administration > Export klicken");
                    Console.WriteLine("3. Unterricht als CSV exportieren nach " + targetPath);
                }

                Console.WriteLine("");
                Console.WriteLine(" ENTER beendet das Programm.");
                //Console.ReadKey();
                Environment.Exit(0);

            }
            var targetFile = Path.Combine(targetPath, Zeitstempel + "_" + kriterium + "_" + user + ".CSV");

            if (File.Exists(targetFile))
            {
                File.Delete(targetFile);
            }
            File.Copy(sourceFile, targetFile);
            
            return Path.Combine(targetFile);
        }

        public static void ErzeugeSqlDatei(List<string> files)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(files[1], true, Encoding.Default))
                {
                    foreach (var o in Global.Output)
                    {
                        writer.WriteLine(o);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            OpenFile(files);
        }

        public static void OpenFile(List<string> files)
        {
            try
            {
                Process notepadPlus = new Process();
                notepadPlus.StartInfo.FileName = "notepad++.exe";

                notepadPlus.StartInfo.Arguments = string.Format("\"{0}\" \"{1}\"", files[0], files[1]);
                notepadPlus.Start();
            }
            catch (Exception)
            {
            }
        }
    }
}
