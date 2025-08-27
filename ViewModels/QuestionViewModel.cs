using System.ComponentModel.DataAnnotations;

namespace OnlineExam.ViewModels;

public class QuestionViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Question is required.")]
    [StringLength(500)]
    [Display(Name = "Question")]
    public string Title { get; set; }

    [Required(ErrorMessage = "Choice A is required.")]
    [StringLength(200)]
    [Display(Name = "Choice A")]
    public string ChoiceA { get; set; }

    [Required(ErrorMessage = "Choice B is required.")]
    [StringLength(200)]
    [Display(Name = "Choice B")]
    public string ChoiceB { get; set; }

    [Required(ErrorMessage = "Choice C is required.")]
    [StringLength(200)]
    [Display(Name = "Choice C")]
    public string ChoiceC { get; set; }

    [Required(ErrorMessage = "Choice D is required.")]
    [StringLength(200)]
    [Display(Name = "Choice D")]
    public string ChoiceD { get; set; }

    [Required(ErrorMessage = "Correct answer is required.")]
    [RegularExpression("A|B|C|D", ErrorMessage = "Correct answer must be A, B, C, or D.")]
    [Display(Name = "Correct Answer")]
    public string CorrectAnswer { get; set; }

    [Required(ErrorMessage = "Exam selection is required.")]
    public int ExamId { get; set; }

    //[Display(Name = "Exam")]
    //public string ExamTitle { get; set; } = string.Empty;
}
