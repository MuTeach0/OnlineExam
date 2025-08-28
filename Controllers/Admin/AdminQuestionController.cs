using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineExam.Data;
using OnlineExam.Models;
using OnlineExam.ViewModels;

namespace OnlineExam.Controllers.Admin;

public class AdminQuestionController(AppDbContext context) : Controller
{
    private bool QuestionExists(int id) => context.Questions.Any(q => q.Id == id);

    // GET: AdminQuestion/Get
    public async Task<IActionResult> GetAsync(int? id = 0)
    {
        if (id is null) // this id for exam
            return NotFound();
        if (id == 0)
        {
            var questions = await context.Questions
                .Include(q => q.Exam)
                .OrderBy(q => q.Exam.Title)
                .ThenBy(q => q.Id)
                .ToListAsync();
            return View(questions);
        }
        else
        {
            var questions = await context.Questions
           .Where(q => q.ExamId == id)
           .Include(q => q.Exam)   // هنا بتعمل eager loading
           .ToListAsync();


            return View(questions);
        }

    }

    // GET: AdminQuestion/Create
    public async Task<IActionResult> Create()
    {
        ViewBag.Exams = await context.Exams.Where(e => e.IsActive).ToListAsync();
        return View(new QuestionViewModel());
    }
    // POST: AdminQuestion/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(QuestionViewModel model)
    {
        if (ModelState.IsValid)
        {
            var question = new Question
            {
                Title = model.Title,
                ChoiceA = model.ChoiceA,
                ChoiceB = model.ChoiceB,
                ChoiceC = model.ChoiceC,
                ChoiceD = model.ChoiceD,
                CorrectAnswer = model.CorrectAnswer,
                ExamId = model.ExamId
            };

            context.Add(question);
            await context.SaveChangesAsync();
            return RedirectToAction(nameof(GetAsync));
        }

        ViewBag.Exams = await context.Exams.Where(e => e.IsActive).ToListAsync();
        return View(model);
    }

    // GET: AdminQuestion/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
            return NotFound();

        var question = await context.Questions
            .Include(q => q.Exam)
            .FirstOrDefaultAsync(q => q.Id == id);

        if (question == null)
            return NotFound();

        var viewModel = new QuestionViewModel
        {
            Id = question.Id,
            Title = question.Title,
            ChoiceA = question.ChoiceA,
            ChoiceB = question.ChoiceB,
            ChoiceC = question.ChoiceC,
            ChoiceD = question.ChoiceD,
            CorrectAnswer = question.CorrectAnswer,
            ExamId = question.ExamId,
            // ExamTitle = question.Exam.Title
        };

        ViewBag.Exams = await context.Exams.Where(e => e.IsActive).ToListAsync();
        return View(viewModel);
    }
    // POST: AdminQuestion/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, QuestionViewModel model)
    {
        if (id != model.Id)
            return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                var question = await context.Questions.FindAsync(id);
                if (question == null)
                    return NotFound();

                question.Title = model.Title;
                question.ChoiceA = model.ChoiceA;
                question.ChoiceB = model.ChoiceB;
                question.ChoiceC = model.ChoiceC;
                question.ChoiceD = model.ChoiceD;
                question.CorrectAnswer = model.CorrectAnswer;
                question.ExamId = model.ExamId;

                context.Update(question);
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!QuestionExists(model.Id))
                    return NotFound();
                else
                    throw;
            }
            return RedirectToAction(nameof(GetAsync));
        }

        ViewBag.Exams = await context.Exams.Where(e => e.IsActive).ToListAsync();
        return View(model);
    }

    // POST: AdminQuestion/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var question = await context.Questions.FindAsync(id);
        if (question != null)
        {
            context.Questions.Remove(question);
            await context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(GetAsync));
    }
}