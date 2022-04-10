using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CinemeOnlineWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChartController : ControllerBase
    {
        private readonly DBOnlineCinemaContext _context;

        public ChartController(DBOnlineCinemaContext context)
        {
            _context = context;
        }

        [HttpGet("Spent")]
        public JsonResult SpentResult()
        {
            var spentData = new List<object>();
            var films = _context.Films;
            spentData.Add(new []{"Film", "Spent"});
            foreach (var film in films)
            {
                spentData.Add(new object[]{film.FilmName, film.Cost});
            }

            return new JsonResult(spentData);
        }
        [HttpGet("Duration")]
        public JsonResult DurationResult()
        {
            var durationData = new List<object>();
            var films = _context.Films;
            durationData.Add(new[] { "Film", "Duration" });
            foreach (var film in films)
            {
                durationData.Add(new object[] { film.FilmName, film.Duration});
            }

            return new JsonResult(durationData);
        }

        [HttpGet("Role")]
        public JsonResult RoleResult()
        {
            var actorPlayData = new List<object>();
            var films = _context.Films;
            actorPlayData.Add(new[] { "Film", "Roles" });
            foreach (var film in films)
            {
                int rolesCount = _context.ActorPlays.Count(ap => ap.FilmId == film.FilmId);
                actorPlayData.Add(new object[] { film.FilmName, rolesCount});
            }

            return new JsonResult(actorPlayData);
        }

    }
}
