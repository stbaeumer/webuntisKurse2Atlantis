namespace webuntisKurse2Atlantis
{
    public class ExportLesson
    {
        public int LessonId { get; internal set; }
        public int LessonNumber { get; internal set; }
        public string Subject { get; internal set; }
        public string Teacher { get; internal set; }
        public string Klassen { get; internal set; }
        public string Studentgroup { get; internal set; }
        public string Periods { get; internal set; }
        public string Startdate { get; internal set; }
        public string EndDate { get; internal set; }
        public string Room { get; internal set; }
        public string Foreignkey { get; internal set; }
    }
}