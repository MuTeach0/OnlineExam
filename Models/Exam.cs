using System.ComponentModel.DataAnnotations;

namespace OnlineExam.Models;

public class Exam
{
    public int Id { get; set; }

    [Required]
    [StringLength(200)]
    public string Title { get; set; }

    [StringLength(1000)]
    public string Description { get; set; }

    public int DurationMinutes { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual List<Question> Questions { get; set; } = [];
    public virtual List<ExamResult> ExamResults { get; set; } = [];
}