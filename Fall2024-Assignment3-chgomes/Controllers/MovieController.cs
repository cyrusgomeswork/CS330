using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Fall2024_Assignment3_chgomes.Data;
using Fall2024_Assignment3_chgomes.Models;
using System.Numerics;
using Fall2024_Assignment3_chgomes.Services;

namespace Fall2024_Assignment3_chgomes.Controllers
{
    public class MovieController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly OpenAIService _openAIService;

        public MovieController(ApplicationDbContext context, OpenAIService openAIService)
        {
            _context = context;
            _openAIService = openAIService;
        }

        // GET: Movie
        public async Task<IActionResult> Index()
        {
            return View(await _context.Movie.ToListAsync());
        }

        // GET: Movie/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Load movie with related actors through MovieActor join table
            var movie = await _context.Movie
                .Include(m => m.MovieActors)
                .ThenInclude(ma => ma.Actor)  // Include actors through the join table
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movie == null)
            {
                return NotFound();
            }

            // Fetch AI-generated reviews with sentiment
            var reviewsWithSentiment = await _openAIService.GetMovieReviews(movie.Title, movie.Year);

            // Calculate the average sentiment
            double averageSentiment = reviewsWithSentiment.Any() ? reviewsWithSentiment.Average(r => r.Sentiment) : 0.0;

            // Create the ViewModel and pass the data
            var viewModel = new MovieDetailsView
            {
                Movie = movie,
                ReviewsWithSentiment = reviewsWithSentiment,
                AverageSentiment = averageSentiment,
                Actors = movie.MovieActors.Select(ma => ma.Actor).ToList() 
            };

            // Return the view with the ViewModel
            return View(viewModel);
        }

        // GET: Movie/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Movie/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Genre,Year,Imdb,Poster")] Movie movie, IFormFile Poster)
        {
            if (ModelState.IsValid)
            {
                if (Poster != null && Poster.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await Poster.CopyToAsync(memoryStream);
                        movie.Poster = memoryStream.ToArray();
                    }
                }
                _context.Add(movie);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(movie);
        }

        // GET: Movie/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie.FindAsync(id);
            if (movie == null)
            {
                return NotFound();
            }
            return View(movie);
        }

        // POST: Movie/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Genre,Year,Imdb,Poster")] Movie movie, IFormFile Poster)
        {
            if (id != movie.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (Poster != null && Poster.Length > 0)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await Poster.CopyToAsync(memoryStream);
                            movie.Poster = memoryStream.ToArray();
                        }
                    }
                    _context.Update(movie);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MovieExists(movie.Id))
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
            return View(movie);
        }

        // GET: Movie/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        // POST: Movie/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var movie = await _context.Movie.FindAsync(id);
            if (movie != null)
            {
                _context.Movie.Remove(movie);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MovieExists(int id)
        {
            return _context.Movie.Any(e => e.Id == id);
        }
    }
    public class MoviesController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly OpenAIService _aiService;

        public MoviesController(ApplicationDbContext dbContext, OpenAIService aiService)
        {
            _dbContext = dbContext;
            _aiService = aiService;
        }

        // GET: Movies
        public async Task<IActionResult> Index()
        {
            return View(await _dbContext.Movie.ToListAsync());
        }

        // GET: Movies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Load movie with related actors through MovieActor join table
            var movie = await _dbContext.Movie
                .Include(m => m.MovieActors)
                .ThenInclude(ma => ma.Actor)  // Include actors through the join table
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movie == null)
            {
                return NotFound();
            }

            // Fetch AI-generated reviews with sentiment
            var reviewsWithSentiment = await _aiService.GetMovieReviews(movie.Title, movie.Year);

            // Calculate the average sentiment
            double averageSentiment = reviewsWithSentiment.Any() ? reviewsWithSentiment.Average(r => r.Sentiment) : 0.0;

            // Create the ViewModel and pass the data
            var viewModel = new MovieDetailsView
            {
                Movie = movie,
                ReviewsWithSentiment = reviewsWithSentiment,
                AverageSentiment = averageSentiment,
                Actors = movie.MovieActors.Select(ma => ma.Actor).ToList()  // Add actors to the ViewModel
            };

            // Return the view with the ViewModel
            return View(viewModel);
        }

        // GET: Movies/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Movies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Genre,Year,Imdb,Poster")] Movie movie, IFormFile Poster)
        {
            if (ModelState.IsValid)
            {
                if (Poster != null && Poster.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await Poster.CopyToAsync(memoryStream);
                        movie.Poster = memoryStream.ToArray();
                    }
                }
                _dbContext.Add(movie);
                await _dbContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(movie);
        }

        // GET: Movies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _dbContext.Movie.FindAsync(id);
            if (movie == null)
            {
                return NotFound();
            }
            return View(movie);
        }

        // POST: Movies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Genre,Year,Imdb,Poster")] Movie movie, IFormFile Poster)
        {
            if (id != movie.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (Poster != null && Poster.Length > 0)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await Poster.CopyToAsync(memoryStream);
                            movie.Poster = memoryStream.ToArray();
                        }
                    }
                    _dbContext.Update(movie);
                    await _dbContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MovieExists(movie.Id))
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
            return View(movie);
        }

        // GET: Movies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _dbContext.Movie
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        // POST: Movies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var movie = await _dbContext.Movie.FindAsync(id);
            if (movie != null)
            {
                _dbContext.Movie.Remove(movie);
            }

            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MovieExists(int id)
        {
            return _dbContext.Movie.Any(e => e.Id == id);
        }
    }
}
