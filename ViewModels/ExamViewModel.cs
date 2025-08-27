using System.ComponentModel.DataAnnotations;

namespace OnlineExam.ViewModels;

public class ExamViewModel
{
    public int Id { get; set; }

    [Required]
    [StringLength(200)]
    [Display(Name = "Exam Title")]
    public string Title { get; set; }

    [StringLength(1000)]
    [Display(Name = "Description")]
    public string Description { get; set; }

    [Required]
    [Range(1, 300)]
    [Display(Name = "Duration (minutes)")]
    public int DurationMinutes { get; set; }

    [Display(Name = "Active")]
    public bool IsActive { get; set; } = true;
}