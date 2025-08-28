using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineExam.Data;
using OnlineExam.Models;
using OnlineExam.ViewModels;

namespace OnlineExam.Controllers.User;


[Authorize]
public class UserExamController(AppDbContext context, UserManager<Users> userManager) : Controller
{
    // GET: UserExam/AvailableExams
    public async Task<IActionResult> AvailableExams()
    {
        var user = await userManager.GetUserAsync(User);
        if (user == null) return RedirectToAction("Login", "Account");

        var availableExams = await context.UserAvailableExams
            .Where(ue => ue.UserId == user.Id)
            .Select(ue => new AvailableExamViewModel
            {
                Id = ue.Exam.Id,
                Title = ue.Exam.Title,
                Description = ue.Exam.Description,
                DurationMinutes = ue.Exam.DurationMinutes,
                QuestionCount = ue.Exam.Questions.Count,
                HasTaken = ue.Exam.ExamResults.Any(er => er.UserId == user.Id)
            })
            .ToListAsync();

        return View(availableExams);
    }

    // GET: Exam/TakeExam/5
    public async Task<IActionResult> TakeExam(int? id)
    {
        if (id == null)
            return NotFound();

        var userId = User.Identity?.Name;
        if (string.IsNullOrEmpty(userId))
            return RedirectToAction("Login", "Account");

        // Check if user has already taken this exam
        var existingResult = await context.ExamResults
            .FirstOrDefaultAsync(er => er.ExamId == id && er.UserId == userId);

        if (existingResult != null)
            return RedirectToAction(nameof(ExamResult), new { id = existingResult.Id });

        var exam = await context.Exams
            .Include(e => e.Questions)
            .FirstOrDefaultAsync(e => e.Id == id && e.IsActive);

        if (exam is null)
            return NotFound();

        if (exam.Questions.Count == 0)
        {
            TempData["Error"] = "This exam has no questions.";
            return RedirectToAction(nameof(AvailableExams));
        }

        var viewModel = new TakeExamViewModel
        {
            ExamId = exam.Id,
            ExamTitle = exam.Title,
            ExamDescription = exam.Description ?? string.Empty,
            DurationMinutes = exam.DurationMinutes,
            Questions = [.. exam.Questions.Select(q => new ExamQuestionViewModel
            {
                QuestionId = q.Id,
                Title = q.Title,
                ChoiceA = q.ChoiceA,
                ChoiceB = q.ChoiceB,
                ChoiceC = q.ChoiceC,
                ChoiceD = q.ChoiceD
            })]
        };

        return View(viewModel);
    }

    // POST: Exam/SubmitExam
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SubmitExam(int examId, List<string> answers)
    {
        var userId = userManager.GetUserId(User);
        if (string.IsNullOrEmpty(userId))
            return RedirectToAction("Login", "Account");

        // Check if user has already taken this exam
        var existingResult = await context.ExamResults
            .FirstOrDefaultAsync(er => er.ExamId == examId && er.UserId == userId);

        if (existingResult != null)
            return RedirectToAction(nameof(ExamResult), new { id = existingResult.Id });

        var exam = await context.Exams
            .Include(e => e.Questions)
            .FirstOrDefaultAsync(e => e.Id == examId && e.IsActive);

        if (exam == null)
            return NotFound();

        var questions = exam.Questions.ToList();
        var correctAnswers = 0;
        var totalQuestions = questions.Count;

        // Calculate score
        for (int i = 0; i < questions.Count && i < answers.Count; i++)
            if (answers[i] == questions[i].CorrectAnswer)
                correctAnswers++;

        var score = totalQuestions > 0 ? (decimal)correctAnswers / totalQuestions * 100 : 0;
        var isPassed = score >= 60;

        var examResult = new ExamResult
        {
            UserId = userId,
            ExamId = examId,
            TotalQuestions = totalQuestions,
            CorrectAnswers = correctAnswers,
            Score = score,
            IsPassed = isPassed,
            StartedAt = DateTime.UtcNow.AddMinutes(-exam.DurationMinutes), // Approximate start time
            CompletedAt = DateTime.UtcNow
        };

        context.Add(examResult);
        await context.SaveChangesAsync();

        return RedirectToAction(nameof(ExamResult), new { id = examResult.Id });
    }

    // GET: Exam/ExamResult/5
    public async Task<IActionResult> ExamResult(int? id)
    {
        if (id is null)
            return NotFound();

        //var userId = User.Identity?.Name;
        var userId = userManager.GetUserId(User);

        if (string.IsNullOrEmpty(userId))
            return RedirectToAction("Login", "Account");

        var examResult = await context.ExamResults
            .Include(er => er.Exam)
            .FirstOrDefaultAsync(er => er.Id == id && er.UserId == userId);

        if (examResult is null)
            return NotFound();

        var viewModel = new ExamResultViewModel
        {
            ResultId = examResult.Id, // ← هنا
            ExamId = examResult.ExamId,
            ExamTitle = examResult.Exam?.Title ?? "Unknown Exam",
            TotalQuestions = examResult.TotalQuestions,
            CorrectAnswers = examResult.CorrectAnswers,
            IncorrectAnswers = examResult.TotalQuestions - examResult.CorrectAnswers,
            Score = examResult.Score,
            IsPassed = examResult.IsPassed,
            StartedAt = examResult.StartedAt,
            CompletedAt = examResult.CompletedAt,
            Duration = examResult.CompletedAt - examResult.StartedAt
        };

        return View(viewModel);
    }

    // GET: Exam/MyResults
    public async Task<IActionResult> MyResults()
    {
        //var userId = User.Identity?.Name;
        var userId = userManager.GetUserId(User);

        if (string.IsNullOrEmpty(userId))
            return RedirectToAction("Login", "Account");

        var results = await context.ExamResults
            .Include(er => er.Exam)
            .Where(er => er.UserId == userId)
            .OrderByDescending(er => er.CompletedAt)
            .ToListAsync();

        var viewModels = results.Select(er => new ExamResultViewModel
        {
            ResultId = er.Id, // ← هنا
            ExamId = er.ExamId,
            ExamTitle = er.Exam?.Title ?? "Unknown Exam",
            TotalQuestions = er.TotalQuestions,
            CorrectAnswers = er.CorrectAnswers,
            IncorrectAnswers = er.TotalQuestions - er.CorrectAnswers,
            Score = er.Score,
            IsPassed = er.IsPassed,
            StartedAt = er.StartedAt,
            CompletedAt = er.CompletedAt,
            Duration = er.CompletedAt - er.StartedAt
        }).ToList();

        return View(viewModels);
    }
}