using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Fall2024_Assignment3_chgomes.Data;
using Fall2024_Assignment3_chgomes.Models;
using Fall2024_Assignment3_chgomes.Services;

namespace Fall2024_Assignment3_chgomes.Controllers
{
    public class ActorController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly OpenAIService _aiService;

        public ActorController(ApplicationDbContext dbContext, OpenAIService aiService)
        {
            _dbContext = dbContext;
            _aiService = aiService;
        }

        // GET: Actor
        public async Task<IActionResult> Index()
        {
            return View(await _dbContext.Actor.ToListAsync());
        }

        // GET: Actor/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var actor = await _dbContext.Actor
               .Include(a => a.MovieActors)
               .ThenInclude(ma => ma.Movie)
               .FirstOrDefaultAsync(a => a.Id == id);

            if (actor == null)
            {
                return NotFound();
            }

            var tweetsWithSentiment = await _aiService.GenerateTweets(actor.Name);
            double averageSentiment = tweetsWithSentiment.Any() ? tweetsWithSentiment.Average(t => t.Sentiment) : 0.0;

            var viewModel = new ActorDetailsView
            {
                Actor = actor,
                TweetWithSentiment = tweetsWithSentiment,
                AverageSentiment = averageSentiment,
                Movies = actor.MovieActors.Select(ma => ma.Movie).ToList()
            };

            return View(viewModel);
        }

        // GET: Actor/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Actor/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Gender,Age,Imdb")] Actor actor, IFormFile photo)
        {
            if (ModelState.IsValid)
            {
                if (photo != null && photo.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await photo.CopyToAsync(memoryStream);
                        actor.Photo = memoryStream.ToArray();
                    }
                }

                _dbContext.Add(actor);
                await _dbContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(actor);
        }

        // GET: Actor/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var actor = await _dbContext.Actor.FindAsync(id);
            if (actor == null)
            {
                return NotFound();
            }
            return View(actor);
        }

        // POST: Actor/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Gender,Age,Imdb,Photo")] Actor actor, IFormFile photo)
        {
            if (id != actor.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (photo != null && photo.Length > 0)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await photo.CopyToAsync(memoryStream);
                            actor.Photo = memoryStream.ToArray();
                        }
                    }
                    _dbContext.Update(actor);
                    await _dbContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ActorExists(actor.Id))
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
            return View(actor);
        }

        // GET: Actor/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var actor = await _dbContext.Actor
                .FirstOrDefaultAsync(m => m.Id == id);
            if (actor == null)
            {
                return NotFound();
            }

            return View(actor);
        }

        // POST: Actor/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var actor = await _dbContext.Actor.FindAsync(id);
            if (actor != null)
            {
                _dbContext.Actor.Remove(actor);
            }

            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ActorExists(int id)
        {
            return _dbContext.Actor.Any(e => e.Id == id);
        }
    }
}
