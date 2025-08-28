using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineExam.Data;
using OnlineExam.Models;
using OnlineExam.ViewModels;

namespace OnlineExam.Controllers.Admin;

[Authorize(Roles = "Admin")]
public class AdminExamController(AppDbContext context) : Controller
{
    private bool ExamExists(int id) => context.Exams.Any(e => e.Id == id);

    // GET: Admin/Exams
    public async Task<IActionResult> GetAll()
    {
        var exams = await context.Exams
            .Include(e => e.Questions)
            .OrderByDescending(e => e.CreatedAt)
            .ToListAsync();

        return View(exams);
    }

    // GET: Admin/CreateExam
    public IActionResult Create() => View(new ExamViewModel());
    // POST: Admin/CreateExam
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ExamViewModel model)
    {
        if (ModelState.IsValid)
        {
            var exam = new Exam
            {
                Title = model.Title,
                Description = model.Description,
                DurationMinutes = model.DurationMinutes,
                IsActive = model.IsActive,
                CreatedAt = DateTime.UtcNow
            };

            context.Add(exam);
            await context.SaveChangesAsync();
            return RedirectToAction(nameof(GetAll));
        }
        return View(model);
    }

    // GET: Admin/EditExam/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
            return NotFound();

        var exam = await context.Exams.FindAsync(id);
        if (exam == null)
            return NotFound();

        var viewModel = new ExamViewModel
        {
            Id = exam.Id,
            Title = exam.Title,
            Description = exam.Description,
            DurationMinutes = exam.DurationMinutes,
            IsActive = exam.IsActive
        };

        return View(viewModel);
    }
    // POST: Admin/EditExam/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ExamViewModel model)
    {
        if (id != model.Id)
            return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                var exam = await context.Exams.FindAsync(id);
                if (exam == null)
                    return NotFound();

                exam.Title = model.Title;
                exam.Description = model.Description;
                exam.DurationMinutes = model.DurationMinutes;
                exam.IsActive = model.IsActive;

                context.Update(exam);
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ExamExists(model.Id))
                    return NotFound();
                else
                    throw;
            }
            return RedirectToAction(nameof(GetAll));
        }
        return View(model);
    }

    // POST: Admin/DeleteExam/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var exam = await context.Exams.FindAsync(id);
        if (exam != null)
        {
            context.Exams.Remove(exam);
            await context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(GetAll));
    }
}
