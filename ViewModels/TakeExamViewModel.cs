namespace OnlineExam.ViewModels;

public class TakeExamViewModel
{
    public int ExamId { get; set; }
    public string ExamTitle { get; set; }
    public string ExamDescription { get; set; }
    public int DurationMinutes { get; set; }
    public List<ExamQuestionViewModel> Questions { get; set; } = [];
}

public class ExamQuestionViewModel
{
    public int QuestionId { get; set; }
    public string Title { get; set; }
    public string ChoiceA { get; set; }
    public string ChoiceB { get; set; }
    public string ChoiceC { get; set; }
    public string ChoiceD { get; set; }
    public string SelectedAnswer { get; set; }
}