using DotNet5CRUD.Data;
using DotNet5CRUD.Models;
using DotNet5CRUD.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NToastNotify;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DotNet5CRUD.Controllers
{

    public class MoviesController : Controller
    {
        private readonly ApplicationDbContext ctx;
        private long _maxAllowedPosterSize = 1048576;
        private readonly IToastNotification toastNotification;
        public MoviesController(ApplicationDbContext _ctx, IToastNotification _toastNotification)
        {
            ctx = _ctx;
            toastNotification = _toastNotification;
        }
        // GET: MoviesController
        public async Task<ActionResult> Index()
        {
            var movies = await ctx.Movies.OrderByDescending(m=>m.Rate).ToListAsync();
            return View(movies);
        }

        // GET: MoviesController/Create
        public async Task<ActionResult> Create()
        {
            var model = new MovieFormViewModel
            {
                Genres=await ctx.Genres.OrderBy(m=>m.Name).ToListAsync()
            };
            return View(model);
        }

        // POST: MoviesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult>Create(MovieFormViewModel model)
        {
            if(!ModelState.IsValid)
            {
                model.Genres = await ctx.Genres.OrderBy(m => m.Name).ToListAsync();
                return View(model);
            }
            var files = Request.Form.Files;

            if (!files.Any())
            {
                model.Genres = await ctx.Genres.OrderBy(m => m.Name).ToListAsync();
                ModelState.AddModelError("Poster", "Please select movie poster!");
                return View(model);
            }

            var poster = files.FirstOrDefault();


            if (poster.Length > _maxAllowedPosterSize)
            {
                model.Genres = await ctx.Genres.OrderBy(m => m.Name).ToListAsync();
                ModelState.AddModelError("Poster", "Poster cannot be more than 1 MB!");
                return View(model);
            }

            using var dataStream = new MemoryStream();

            await poster.CopyToAsync(dataStream);

            var movies = new Movie
            {
                Title = model.Title,
                GenreId = model.GenreId,
                Year = model.Year,
                Rate = model.Rate,
                Storyline = model.Storyline,
                Poster = dataStream.ToArray()
            };

            ctx.Movies.Add(movies);
            ctx.SaveChanges();
            toastNotification.AddSuccessToastMessage("Movie Added successfully");
            return RedirectToAction(nameof(Index));
        }

        // GET: MoviesController/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return BadRequest();

            var movie = await ctx.Movies.FindAsync(id);

            if (movie == null)
                return NotFound();

            var viewModel = new MovieFormViewModel
            {
                Id = movie.Id,
                Title = movie.Title,
                GenreId = movie.GenreId,
                Rate = movie.Rate,
                Year = movie.Year,
                Storyline = movie.Storyline,
                Poster = movie.Poster,
                Genres = await ctx.Genres.OrderBy(m => m.Name).ToListAsync()
            };

            return View("Create", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(MovieFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Genres = await ctx.Genres.OrderBy(m => m.Name).ToListAsync();
                return View("Create", model);
            }

            var movie = await ctx.Movies.FindAsync(model.Id);

            if (movie == null)
                return NotFound();

            var files = Request.Form.Files;

            if (files.Any())
            {
                var poster = files.FirstOrDefault();

                using var dataStream = new MemoryStream();

                await poster.CopyToAsync(dataStream);

                model.Poster = dataStream.ToArray();

               

                if (poster.Length > _maxAllowedPosterSize)
                {
                    model.Genres = await ctx.Genres.OrderBy(m => m.Name).ToListAsync();
                    ModelState.AddModelError("Poster", "Poster cannot be more than 1 MB!");
                    return View("Create", model);
                }

                movie.Poster = model.Poster;
            }

            movie.Title = model.Title;
            movie.GenreId = model.GenreId;
            movie.Year = model.Year;
            movie.Rate = model.Rate;
            movie.Storyline = model.Storyline;

            ctx.SaveChanges();

            toastNotification.AddSuccessToastMessage("Movie updated successfully");
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return BadRequest();

            var movie = await ctx.Movies.Include(m => m.Genre).SingleOrDefaultAsync(m => m.Id == id);

            if (movie == null)
                return NotFound();

            return View(movie);
        }

        public async Task<IActionResult>Delete(int? id)
        {
            if (id == null)
                return BadRequest();

            var movie = await ctx.Movies.FindAsync(id);

            if (movie == null)
                return NotFound();

            ctx.Movies.Remove(movie);
            ctx.SaveChanges();

            return RedirectToAction("Index");
        }


    }

        
        
    }

