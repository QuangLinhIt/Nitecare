using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using nitecare.BaseModel;
using nitecare.Model;
using PagedList.Core;

namespace nitecare.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminFeedbacksController : Controller
    {
        private readonly nitecareContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        public AdminFeedbacksController(nitecareContext context, IWebHostEnvironment hostEnvironment)
        {
            _hostEnvironment = hostEnvironment;
            _context = context;
        }

        // GET: Admin/AdminFeedbacks
        public IActionResult Index(int? page)
        {
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = 20;
            var ListFeedback = _context.Feedbacks
                .AsNoTracking()
                .Include(x => x.User)
                .OrderByDescending(x => x.FeedbackId);
            PagedList<Feedback> models = new PagedList<Feedback>(ListFeedback, pageNumber, pageSize);
            ViewBag.CurrentPage = pageNumber;
            return View(models);
        }

        // GET: Admin/AdminFeedbacks/Create
        public IActionResult Create()
        {
            ViewData["NguoiDung"] = new SelectList(_context.Users, "UserId", "UserName");
            return View();
        }

        // POST: Admin/AdminFeedbacks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FeedbackVm feedbackVm)
        {
            if (ModelState.IsValid)
            {
                ViewData["NguoiDung"] = new SelectList(_context.Users, "UserId", "UserName",feedbackVm.UserId);
                var feedback = new Feedback();
                feedback.FeedbackId = feedbackVm.FeedbackId;
                feedback.UserId = feedbackVm.UserId;
                feedback.FeedbackContent = feedbackVm.FeedbackContent;
                feedback.FeedbackAvatar = ProcessUploadedAvatar(feedbackVm);
                feedback.FeedbackImage = ProcessUploadedFile(feedbackVm);
                _context.Add(feedback);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(feedbackVm);
        }

        // GET: Admin/AdminFeedbacks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            ViewData["NguoiDung"] = new SelectList(_context.Users, "UserId", "UserName");
            var feedback = await _context.Feedbacks.FindAsync(id);
            var feedbackVm = new FeedbackVm();
            feedbackVm.FeedbackId = feedback.FeedbackId;
            feedbackVm.UserId = feedback.UserId;
            feedbackVm.FeedbackContent = feedback.FeedbackContent;
            feedbackVm.AvatarName = feedback.FeedbackAvatar;
            feedbackVm.ImageName = feedback.FeedbackImage;

            if (feedback == null)
            {
                return NotFound();
            }
            return View(feedbackVm);
        }

        // POST: Admin/AdminFeedbacks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, FeedbackVm feedbackVm)
        {
            if (id != feedbackVm.FeedbackId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    ViewData["NguoiDung"] = new SelectList(_context.Users, "UserId", "UserName",feedbackVm.UserId);
                    var feedback = _context.Feedbacks.FirstOrDefault(x => x.FeedbackId == feedbackVm.FeedbackId);
                    if (feedbackVm.ImageFile != null)
                    {
                        string filePath = Path.Combine(_hostEnvironment.WebRootPath, "feedback/image", feedback.FeedbackImage);
                        System.IO.File.Delete(filePath);
                        feedback.FeedbackImage = ProcessUploadedFile(feedbackVm);
                    }
                    if (feedbackVm.AvatarFile != null)
                    {
                        string avatarPath = Path.Combine(_hostEnvironment.WebRootPath, "feedback/avatar", feedback.FeedbackAvatar);
                        System.IO.File.Delete(avatarPath);
                        feedback.FeedbackAvatar = ProcessUploadedAvatar(feedbackVm);
                    }
                    feedback.FeedbackId = feedbackVm.FeedbackId;
                    feedback.UserId = feedbackVm.UserId;
                    feedback.FeedbackContent = feedbackVm.FeedbackContent;
                    _context.Update(feedback);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FeedbackExists(feedbackVm.FeedbackId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(feedbackVm);
        }

        // GET: Admin/AdminFeedbacks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var feedback = await _context.Feedbacks
                .FirstOrDefaultAsync(m => m.FeedbackId == id);
            if (feedback == null)
            {
                return NotFound();
            }

            return View(feedback);
        }

        // POST: Admin/AdminFeedbacks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var feedback = await _context.Feedbacks.FindAsync(id);
            var CurrentImage = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/feedback/image/", feedback.FeedbackImage);
            var CurrentAvatar = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/feedback/avatar/", feedback.FeedbackAvatar);
            _context.Feedbacks.Remove(feedback);
            if (await _context.SaveChangesAsync() > 0)
            {
                if (System.IO.File.Exists(CurrentImage))
                {
                    System.IO.File.Delete(CurrentImage);
                }
                if (System.IO.File.Exists(CurrentAvatar))
                {
                    System.IO.File.Delete(CurrentAvatar);
                }
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FeedbackExists(int id)
        {
            return _context.Feedbacks.Any(e => e.FeedbackId == id);
        }
        private string ProcessUploadedFile(FeedbackVm feedbackVm)
        {

            if (feedbackVm.ImageFile != null)
            {
                string uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "feedback/image");
                feedbackVm.ImageName = Guid.NewGuid().ToString() + "_" + feedbackVm.ImageFile.FileName;
                string filePath = Path.Combine(uploadsFolder, feedbackVm.ImageName);
                using var fileStream = new FileStream(filePath, FileMode.Create);
                feedbackVm.ImageFile.CopyTo(fileStream);
            }
            return feedbackVm.ImageName;
        }

        private string ProcessUploadedAvatar(FeedbackVm feedbackVm)
        {
            if (feedbackVm.AvatarFile != null)
            {
                string uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "feedback/avatar");
                feedbackVm.AvatarName = Guid.NewGuid().ToString() + "_" + feedbackVm.AvatarFile.FileName;
                string filePath = Path.Combine(uploadsFolder, feedbackVm.AvatarName);
                using var fileStream = new FileStream(filePath, FileMode.Create);
                feedbackVm.AvatarFile.CopyTo(fileStream);
            }
            return feedbackVm.AvatarName;
        }
    }
}
