namespace student.Models
{
    public class Enrollment
    {
        public int StudentId { get; set; }
        public Student Student { get; set; } = default!;

        public int ClassId { get; set; }
        public Class Class { get; set; } = default!;
    }
}