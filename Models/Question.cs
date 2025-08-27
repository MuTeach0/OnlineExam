using System.ComponentModel.DataAnnotations;

namespace OnlineExam.Models;

public class Question
{
    public int Id { get; set; }

    [Required]
    [StringLength(500)]
    public string Title { get; set; }

    [Required]
    [StringLength(200)]
    public string ChoiceA { get; set; }

    [Required]
    [StringLength(200)]
    public string ChoiceB { get; set; }

    [Required]
    [StringLength(200)]
    public string ChoiceC { get; set; }

    [Required]
    [StringLength(200)]
    public string ChoiceD { get; set; }

    [Required]
    public string CorrectAnswer { get; set; } // A, B, C, or D

    public int ExamId { get; set; }

    // Navigation property
    public virtual Exam Exam { get; set; }
}