namespace OnlineExam.ViewModels;

public class EditUserViewModel
{
    public string Id { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool EmailConfirmed { get; set; }

    public List<int> SelectedAvailableExamIds { get; set; } = [];
    public List<ExamCheckboxVM> AvailableExams { get; set; } = [];

    // الامتحانات اللي اتحلت فعلاً
    public List<SolvedExamVM> SolvedExams { get; set; } = [];
}

public class ExamCheckboxVM
{
    public int ExamId { get; set; }
    public string Title { get; set; } = string.Empty;
    public bool IsSelected { get; set; }
}

public class SolvedExamVM
{
    public int ExamId { get; set; }
    public string Title { get; set; } = string.Empty;
    public decimal Score { get; set; }
    public DateTime CompletedAt { get; set; }
    public bool IsPassed { get; set; }
}
