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
    public class CratorsTeamsController : Controller
    {
        private readonly DBOnlineCinemaContext _context;

        public CratorsTeamsController(DBOnlineCinemaContext context)
        {
            _context = context;
        }

        // GET: CratorsTeams
        public async Task<IActionResult> Index(int? creatorsTeamId)
        {
            if (creatorsTeamId == null)
                return View(await _context.CratorsTeams.ToListAsync());
            else
            {
                var creators = _context.CratorsTeams.Where(ct => ct.CreationTeamId == creatorsTeamId).ToListAsync();
                return View(await creators);
            }
               


        }

        // GET: CratorsTeams/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cratorsTeam = await _context.CratorsTeams
                .FirstOrDefaultAsync(ct => ct.CreationTeamId == id);
            if (cratorsTeam == null)
            {
                return NotFound();
            }
            ViewBag.quantityFilms = _context.Films.Count(f => f.CreationTeamId == cratorsTeam.CreationTeamId);
            return View(cratorsTeam);
        }

        // GET: CratorsTeams/Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CreationTeamId,DirectorName,QuantityPeople")] CratorsTeam cratorsTeam)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cratorsTeam);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(cratorsTeam);
        }

        // GET: CratorsTeams/Edit/5
        [Authorize(Roles = "admin, editor")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cratorsTeam = await _context.CratorsTeams.FindAsync(id);
            if (cratorsTeam == null)
            {
                return NotFound();
            }
            return View(cratorsTeam);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CreationTeamId,DirectorName,QuantityPeople")] CratorsTeam cratorsTeam)
        {
            if (id != cratorsTeam.CreationTeamId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cratorsTeam);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CratorsTeamExists(cratorsTeam.CreationTeamId))
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
            return View(cratorsTeam);
        }


        #region //// GET: CratorsTeams/Delete/5 
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cratorsTeam = await _context.CratorsTeams
                .FirstOrDefaultAsync(m => m.CreationTeamId == id);
            if (cratorsTeam == null)
            {
                return NotFound();
            }
            ViewBag.filmsCount = _context.Films.Where(f => f.CreationTeamId == id).Count();
           
            return View(cratorsTeam);
        }

        // POST: CratorsTeams/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cratorsTeam = await _context.CratorsTeams.FindAsync(id);
            _context.CratorsTeams.Remove(cratorsTeam);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        #endregion

        private bool CratorsTeamExists(int id)
        {
            return _context.CratorsTeams.Any(e => e.CreationTeamId == id);
        }
    }
}
