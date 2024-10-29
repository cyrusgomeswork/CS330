using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Fall2024_Assignment3_chgomes.Data;
using Fall2024_Assignment3_chgomes.Models;

namespace Fall2024_Assignment3_chgomes.Controllers
{
    public class MovieActorController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public MovieActorController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // GET: MovieActor
        public async Task<IActionResult> Index()
        {
            var movieActors = _dbContext.MovieActor.Include(ma => ma.Actor).Include(ma => ma.Movie);
            return View(await movieActors.ToListAsync());
        }

        // GET: MovieActor/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movieActor = await _dbContext.MovieActor
                .Include(ma => ma.Actor)
                .Include(ma => ma.Movie)
                .FirstOrDefaultAsync(ma => ma.Id == id);
            if (movieActor == null)
            {
                return NotFound();
            }

            return View(movieActor);
        }

        // GET: MovieActor/Create
        public IActionResult Create()
        {
            ViewData["ActorId"] = new SelectList(_dbContext.Actor, "Id", "Name");
            ViewData["MovieId"] = new SelectList(_dbContext.Movie, "Id", "Title");
            return View();
        }

        // POST: MovieActor/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,MovieId,ActorId")] MovieActor movieActor)
        {
            bool alreadyExists = await _dbContext.MovieActor
                .AnyAsync(ma => ma.MovieId == movieActor.MovieId && ma.ActorId == movieActor.ActorId);

            if (ModelState.IsValid && !alreadyExists)
            {
                _dbContext.Add(movieActor);
                await _dbContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", "Cannot add the same actor multiple times for the same movie");

            ViewData["ActorId"] = new SelectList(_dbContext.Actor, "Id", "Name", movieActor.ActorId);
            ViewData["MovieId"] = new SelectList(_dbContext.Movie, "Id", "Title", movieActor.MovieId);

            return View(movieActor);
        }

        // GET: MovieActor/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movieActor = await _dbContext.MovieActor.FindAsync(id);
            if (movieActor == null)
            {
                return NotFound();
            }
            ViewData["ActorId"] = new SelectList(_dbContext.Actor, "Id", "Name", movieActor.ActorId);
            ViewData["MovieId"] = new SelectList(_dbContext.Movie, "Id", "Title", movieActor.MovieId);
            return View(movieActor);
        }

        // POST: MovieActor/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,MovieId,ActorId")] MovieActor movieActor)
        {
            if (id != movieActor.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _dbContext.Update(movieActor);
                    await _dbContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MovieActorExists(movieActor.Id))
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
            ViewData["ActorId"] = new SelectList(_dbContext.Actor, "Id", "Name", movieActor.ActorId);
            ViewData["MovieId"] = new SelectList(_dbContext.Movie, "Id", "Title", movieActor.MovieId);
            return View(movieActor);
        }

        // GET: MovieActor/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movieActor = await _dbContext.MovieActor
                .Include(ma => ma.Actor)
                .Include(ma => ma.Movie)
                .FirstOrDefaultAsync(ma => ma.Id == id);
            if (movieActor == null)
            {
                return NotFound();
            }

            return View(movieActor);
        }

        // POST: MovieActor/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var movieActor = await _dbContext.MovieActor.FindAsync(id);
            if (movieActor != null)
            {
                _dbContext.MovieActor.Remove(movieActor);
            }

            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MovieActorExists(int id)
        {
            return _dbContext.MovieActor.Any(ma => ma.Id == id);
        }
    }
}
