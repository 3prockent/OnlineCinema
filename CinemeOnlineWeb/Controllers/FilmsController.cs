#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CinemeOnlineWeb;
using Microsoft.AspNetCore.Authorization;

namespace CinemeOnlineWeb.Controllers
{
    public class FilmsController : Controller
    {
        private readonly DBOnlineCinemaContext _context;

        public FilmsController(DBOnlineCinemaContext context)
        {
            _context = context;
        }

        // GET: Films/Index
        public async Task<IActionResult> Index(int? creationTeamId)
        {
            var dBOnlineCinemaContext = _context.Films.Include(f => f.CreationTeam);
            if (creationTeamId == null) //start Page
                return View(await dBOnlineCinemaContext.ToListAsync());
                                        //return View(@"Views/JS.cshtml");
            var result = dBOnlineCinemaContext.Where(f => f.CreationTeamId == creationTeamId).ToListAsync();    //return list of films by CreatorsTeam 
            return View(await result);
        }

        // GET: Films/Details/5
        public async Task<IActionResult> Details(int? filmId)
        {
            if (filmId == null)
            {
                return NotFound();
            }

            var film = await _context.Films
                .Include(f => f.CreationTeam)
                .FirstOrDefaultAsync(m => m.FilmId == filmId);
            if (film == null)
            {
                return NotFound();
            }

            return RedirectToAction("Index", "ActorPlays", new { filmId = film.FilmId});
        }

        private List<SelectListItem> GetCreatorsTeam()
        {
            List<SelectListItem> creatorsTeam = new List<SelectListItem>();
            creatorsTeam = (from team in _context.CratorsTeams
                select new SelectListItem()
                {
                    Text = team.DirectorName,
                    Value = team.CreationTeamId.ToString()
                }).ToList();
            creatorsTeam.Insert(0, new SelectListItem() { Text = "---Enter director name---", Value = "" });
            return creatorsTeam;
        }

        // GET: Films/Create
        public IActionResult Create()
        {
            ViewBag.CreationTeamId = GetCreatorsTeam();
            return View();
        }

        // POST: Films/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Film film)
        {
            if (film.FilmName.Length is < 2)
                ModelState.AddModelError("FilmName", "Film name must be more than 3 characters");

            if (film.YearRelease is < 1888)
                ModelState.AddModelError("YearRelease", "Release year must be more than 1887(First film)");

            int curYear = DateTime.Today.Year;
            if (film.YearRelease>curYear)
                ModelState.AddModelError("YearRelease", "Release year must be less than current year");

            if (film.Duration is < 1)
                ModelState.AddModelError("Duration", "Duration must be positive number");

            if (film.Cost is < 1)
                ModelState.AddModelError("Cost", "Spent money must be positive number");

            var sameNameFilm=_context.Films.Where(f => f.FilmName == film.FilmName).Select(f=>f).ToList();
            if (sameNameFilm.Count!=0) 
            {
                ModelState.AddModelError("FilmName", "Film with such name already exist");
            }

            if (ModelState.IsValid)
            {
                _context.Add(film);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.CreationTeamId = GetCreatorsTeam();
            return View(film);
        }

        [Authorize(Roles = "admin")]
        // GET: Films/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var film = await _context.Films
                .Include(f => f.CreationTeam)
                .FirstOrDefaultAsync(m => m.FilmId == id);
            if (film == null)
            {
                return NotFound();
            }

            return View(film);
        }

        // POST: Films/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int FilmId)
        {
            var film = await _context.Films.FindAsync(FilmId);

            if (film == null)
                return NotFound();

            var filmRoles = _context.ActorPlays.Where(ap => ap.FilmId == FilmId);
            foreach (var role in filmRoles)
                _context.ActorPlays.Remove(role);
            _context.Films.Remove(film);

            await _context.SaveChangesAsync();
            return View("DeleteConfirmed");
        }

        private bool FilmExists(int id)
        {
            return _context.Films.Any(e => e.FilmId == id);
        }
    }
}
