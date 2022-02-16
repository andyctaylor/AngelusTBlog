﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AngelusTBlog.Data;
using AngelusTBlog.Models;

namespace AngelusTBlog.Controllers
{
    public class CommentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CommentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Comments all Comments: GET: Comments/Details/5
        public async Task<IActionResult> Index()
        {
            var allComments = await _context.Comments.ToListAsync();
            return View(allComments);
        }

        // GET: Comments all the Original Comments
        public async Task<IActionResult> OriginalComments()
        {
            var originalComments = _context.Comments.ToListAsync();
            return View("Index", await originalComments);
        }

        // GET: Comments all the Moderated Comments
        public async Task<IActionResult> ModeratedIndex()
        {
            var moderatedIndex = _context.Comments.Where(c => c.Moderated != null).ToListAsync();
            return View("Index", await moderatedIndex);
        }

        // GET: Getting all of the soft Deleted Comments
        //public async Task<IActionResult> DeletedIndex()
        //{
        //    Soft Delete Boolen 
        //}
        

        // GET: Comments/Create
        //public IActionResult Create()
        //{
        //    ViewData["AuthorID"] = new SelectList(_context.Users, "Id", "Id");
        //    ViewData["ModeratorID"] = new SelectList(_context.Users, "Id", "Id");
        //    return View();
        //}

        // POST: Comments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,PostId,AuthorID,ModeratorID,CommentBody,Creaed,Updated,Moderated,Deleted,ModeratedCommentBody,ModerationType")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                // Recoarding the created date.
                comment.Created = DateTime.Now;

                _context.Add(comment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AuthorID"] = new SelectList(_context.Users, "Id", "Id", comment.AuthorID);
            ViewData["ModeratorID"] = new SelectList(_context.Users, "Id", "Id", comment.ModeratorID);
            ViewData["PostId"] = new SelectList(_context.Users, "Id", "Summary", comment.PostId);
            return View(comment);
        }

        // GET: Comments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                return NotFound();
            }
            ViewData["AuthorID"] = new SelectList(_context.Users, "Id", "Id", comment.AuthorID);
            ViewData["ModeratorID"] = new SelectList(_context.Users, "Id", "Id", comment.ModeratorID);
            return View(comment);
        }

        // POST: Comments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,PostId,AuthorID,ModeratorID,CommentBody,Creaed,Updated,Moderated,Deleted,ModeratedCommentBody,ModerationType")] Comment comment)
        {
            if (id != comment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(comment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CommentExists(comment.Id))
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
            ViewData["AuthorID"] = new SelectList(_context.Users, "Id", "Id", comment.AuthorID);
            ViewData["ModeratorID"] = new SelectList(_context.Users, "Id", "Id", comment.ModeratorID);
            return View(comment);
        }

        // GET: Comments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await _context.Comments
                .Include(c => c.Author)
                .Include(c => c.Moderator)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (comment == null)
            {
                return NotFound();
            }

            return View(comment);
        }

        // POST: Comments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CommentExists(int id)
        {
            return _context.Comments.Any(e => e.Id == id);
        }
    }
}
