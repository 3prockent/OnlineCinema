#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CinemeOnlineWeb;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.AspNetCore.Authorization;

namespace CinemeOnlineWeb.Controllers
{
    public class ActorPlaysController : Controller
    {
        private readonly DBOnlineCinemaContext _context;

        public ActorPlaysController(DBOnlineCinemaContext context)
        {
            _context = context;
        }

        private List<SelectListItem> GetActors()
        {
            List<SelectListItem> actors = new List<SelectListItem>();
            actors = (from actor in _context.Actors
                select new SelectListItem()
                {
                    Text = actor.ActorName,
                    Value = actor.ActorId.ToString()
                }).ToList();
            actors.Insert(0, new SelectListItem(){Text = "---Enter actor name---", Value= ""});
            return actors;
        }

        // GET: ActorPlays
        public async Task<IActionResult> Index(int? filmId)
        {
            ViewBag.filmId = filmId;
            ViewBag.FilmName = _context.Films.First(f => f.FilmId == filmId).FilmName;
            var dBOnlineCinemaContext = _context.ActorPlays
                .Where(ap => ap.FilmId == filmId)
                .Include(a => a.Actor)
                .Include(a => a.Film);

            return View(await dBOnlineCinemaContext.ToListAsync());
        }

        // GET: ActorPlays/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var actorPlay = await _context.ActorPlays
                .Include(a => a.Actor)
                .Include(a => a.Film)
                .FirstOrDefaultAsync(m => m.ActorPlayId == id);
            if (actorPlay == null)
            {
                return NotFound();
            }
            return RedirectToAction("Details", "Actors", new { actorId = id });
        }

        // GET: ActorPlays/Create
        public IActionResult Create(int? filmId)
        {
            ViewBag.ActorId=GetActors();
            ViewBag.filmId = filmId;
            ViewBag.filmName = _context.Films.First(f => f.FilmId == filmId).FilmName;
            return View();
        }

        // POST: ActorPlays/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ActorPlay actorPlay)
        {
            if (actorPlay.Salary is <= 0)
                ModelState.AddModelError("Salary","Salary must be positive number");

            if (actorPlay.QuantityScenes is < 0)
                ModelState.AddModelError("QuantityScenes", "Quantity of Scenes must be positive number");
            
            Film film = _context.Films.Find(actorPlay.FilmId);
            int filmYear=film.YearRelease;

            Actor actor = _context.Actors.Find(actorPlay.ActorId);
            int actorBirthYear = actor.BirthDate.Year;

            if(actorBirthYear>filmYear)
                ModelState.AddModelError("ActorId", "Actor can't be younger than film release year");

            if (ModelState.IsValid)
            {
                _context.Add(actorPlay);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", new {filmId=actorPlay.FilmId});
            }
            ViewBag.ActorId = GetActors();
            ViewBag.filmName = _context.Films.First(f => f.FilmId == actorPlay.FilmId).FilmName;
            ViewBag.filmId = actorPlay.FilmId;
            return View(actorPlay);
        }
        [Authorize(Roles="admin, editor")]
        // GET: ActorPlays/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var actorPlay = await _context.ActorPlays.FindAsync(id);
            if (actorPlay == null)
            {
                return NotFound();
            }
            ViewData["ActorId"] = new SelectList(_context.Actors, "ActorId", "ActorId", actorPlay.ActorId);
            ViewData["FilmId"] = new SelectList(_context.Films, "FilmId", "FilmId", actorPlay.FilmId);
            return View(actorPlay);
        }

        // POST: ActorPlays/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ActorPlay actorPlay)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(actorPlay);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ActorPlayExists(actorPlay.ActorPlayId))
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
            ViewData["ActorId"] = new SelectList(_context.Actors, "ActorId", "ActorId", actorPlay.ActorId);
            ViewData["FilmId"] = new SelectList(_context.Films, "FilmId", "FilmId", actorPlay.FilmId);
            return View(actorPlay);
        }

        // GET: ActorPlays/Delete/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var actorPlay = await _context.ActorPlays
                .Include(a => a.Actor)
                .Include(a => a.Film)
                .FirstOrDefaultAsync(m => m.ActorPlayId == id);
            if (actorPlay == null)
            {
                return NotFound();
            }

            ViewBag.filmId = id;

            return View(actorPlay);
        }

        // POST: ActorPlays/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var actorPlay = await _context.ActorPlays.FindAsync(id);
            _context.ActorPlays.Remove(actorPlay);
            await _context.SaveChangesAsync();
            ViewBag.filmId = actorPlay.FilmId;
            return View("DeleteConfirmed", new {filmId=actorPlay.FilmId});
        }

        private bool ActorPlayExists(int id)
        {
            return _context.ActorPlays.Any(e => e.ActorPlayId == id);
        }
    }
 
}
