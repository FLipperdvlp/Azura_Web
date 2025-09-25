using System.ComponentModel.DataAnnotations;

namespace student.Models
{
    public class Student
    {
        public int Id { get; set; }

        [Required, StringLength(128)]
        public string FirstName { get; set; } = string.Empty;

        [Required, StringLength(128)]
        public string LastName { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        public DateTime? EnrollmentDate { get; set; }

        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

        public string FullName => $"{LastName} {FirstName}";
    }
}