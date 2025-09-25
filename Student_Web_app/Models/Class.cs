using System.ComponentModel.DataAnnotations;

namespace student.Models
{
    public class Class
    {
        public int Id { get; set; }

        [Required, StringLength(128)]
        public string Title { get; set; } = string.Empty;

        [StringLength(32)]
        public string? Code { get; set; }

        [Range(0, 60)]
        public int Credits { get; set; }

        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    }
}