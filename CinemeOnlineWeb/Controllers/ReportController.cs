using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using ClosedXML;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Office2010.PowerPoint;
using Microsoft.CodeAnalysis;

namespace CinemeOnlineWeb.Controllers
{
    public class ReportController : Controller
    {
        #region constants
        private const string REPORT_NAME = "Cinema – Report";
        private const string REPORT_FORMAT = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

        private const string FILM = "Film Name";
        private const string ACTOR_1 = "Actor 1";
        private const string ACTOR_2 = "Actor 2";
        private const string ACTOR_3 = "Actor 3";
        private const string DIRECTOR = "Director";
       
        private const string FILM_CELL = "A1";
        private const string ACTOR_1_CELL = "B1";
        private const string ACTOR_2_CELL = "C1";
        private const string ACTOR_3_CELL = "D1";
        private const string DIRECTOR_CELL = "E1";
    



        private const int FILM_IND = 1;
        private const int YEAR_IND = 2;
        private const int DUR_IND = 3;
        private const int SPENT_IND = 4;
        private const int DIRECTOR_IND = 5;


        private const string ERR_NULL = "UNKNOWN ERROR";
        private const string ERR_CRIT_NULL = "Records on the specified filters are missing";
        private const string ERR_DATA_NULL = "File doesn't contain data";
        private const string ERR_PATH_NULL = "File path incorrect";
        private const string ERR_DIRR_NULL = "Not found director in a file";
        private const string ERR_FILM_NULL = "Not found Film in a file";
        private const string ERR_FILM_DUPL = "Film already exist in DB";


        #endregion

        private readonly DBOnlineCinemaContext _context;

        public ReportController(DBOnlineCinemaContext context)
        {
            _context = context;
        }

        public IActionResult Index(string errorMessage)
        {
            if (errorMessage == null)
                ViewBag.Error = ERR_NULL;
            else
                ViewBag.Error = errorMessage;
            return View();
        }

        //Post: Report/Import
        public async Task<IActionResult> Import(IFormFile fileExcel)
        {
            if (fileExcel == null)
                return RedirectToAction("Index", new { errorMessage = ERR_PATH_NULL });
            using (var excelStream = new FileStream(fileExcel.FileName, FileMode.Create))
            {
                await fileExcel.CopyToAsync(excelStream);
                if(excelStream.Length ==0)
                    return RedirectToAction("Index", new { errorMessage = ERR_DATA_NULL });
                using (var workbook = new XLWorkbook(excelStream))
                {
                    if (!ParseReport(workbook, out string error))
                        return RedirectToAction("Index", new { errorMessage = error });
                }
            }
            return RedirectToAction("Index", "Films");
        }

        private bool ParseReport(IXLWorkbook workbook,out string error)
        {
            error="";
            int rowCnt = 1;
            var sheet = workbook.Worksheet(1);
            foreach (var row in sheet.RowsUsed())
            {
                string rowError = "";
                if (!ParseRow(row, out rowError))
                {
                    //string filmName = row.Cell(FILM_IND).Value.ToString();
                    //string directorName = row.Cell(DIRECTOR_IND).Value.ToString();
                    error += $"(row {rowCnt}):{rowError}\n"; 
                }
                rowCnt++;

            }
            if (String.IsNullOrWhiteSpace(error))
                return true;
            return false;
        }
        
        private bool ParseRow(IXLRow row, out string rowError)
        {
            rowError = "";
            CratorsTeam team;
            if (ParseTeam(row, out team, out rowError))
            {
                ParseFilm(row, team.CreationTeamId, out rowError);
            }
            if (string.IsNullOrWhiteSpace(rowError))
                return true;
            return false;
            
        }

        private void ParseFilm(IXLRow row, int teamId, out string filmError)
        {
            string filmName = row.Cell(FILM_IND).Value.ToString();
            if (!string.IsNullOrEmpty(filmName))
            {
                var film = _context.Films.FirstOrDefault(f => f.FilmName.Equals(filmName) && f.CreationTeamId==teamId) ;
                if (film == null)
                {
                    filmError = "";
                    int year = 0;
                    int duration = 0;
                    int spent = 0;
                    if(ParseCriteria(row, out year, out duration, out spent,out filmError))
                    {
                        createFilm(filmName, year, duration, spent, teamId);
                        return;
                    }
                    return;
                }
                else
                {
                    filmError = ERR_FILM_DUPL;
                    return;
                }
            }
            filmError = ERR_FILM_NULL;
        }

        private Film createFilm(string name, int year, int duration, int spent, int teamId)
        {
            Film film = new Film();
            film.CreationTeamId = teamId;
            film.FilmName = name; 
            film.YearRelease= year; 
            film.Duration = duration; 
            film.Cost= spent;
            _context.Films.Add(film);
            _context.SaveChanges();
            return film;
        }
        private bool ParseCriteria(IXLRow row, out int year, out int duration, out int spent, out string criteriaError) 
        {
            year = 0;
            duration = 0;
            spent = 0;
            if (!Int32.TryParse(row.Cell(YEAR_IND).Value.ToString(), out year))
            {
                criteriaError = "cannot parse year";
                return false;
            }
            if (!Int32.TryParse(row.Cell(DUR_IND).Value.ToString(), out duration))
            {
                criteriaError = "cannot parse duration";
                return false;
            }
            Int32.TryParse(row.Cell(SPENT_IND).Value.ToString(), out spent);
            if (IsCriteriaCorrect(year, duration, spent, out criteriaError))
                return true;
            return false;

        }

        private bool IsCriteriaCorrect(int year, int duration, int spent, out string criteriaError)
        {
            criteriaError = "";
            if (year <= 1888 || year >= DateTime.Today.Year)
            {
                criteriaError = $"year must be between 1888 and {DateTime.Today.Year}";
                return false;
            }
            if(duration <= 0)
            {
                criteriaError = "duration must be positive number";
                return false;
            }
            if (spent < 0)
            {
                criteriaError = "number of spent money must be more than 0";
                return false;
            }
            return true;
        }

        private bool ParseTeam(IXLRow row, out CratorsTeam team, out string error)
        {
            team = null;
            error = "";

            string directorName = row.Cell(DIRECTOR_IND).Value.ToString();
            if (directorName.Length > 0)
            {
                team = _context.CratorsTeams.FirstOrDefault(ct => ct.DirectorName.Equals(directorName));
                if (team == null)
                {
                    team = CreateTeam(directorName);
                }
                return true;
            }
            error = ERR_DIRR_NULL;
            return false;
        }
        private CratorsTeam CreateTeam(string directorName)
        {
            var team = new CratorsTeam();
            team = new CratorsTeam();
            team.DirectorName = directorName;
            team.QuantityPeople = 0;
            _context.Add(team);
            _context.SaveChanges();
            return team;
        }
        // POST: Report/Export
        public async Task<IActionResult> Export(string criteria)
        {
            List<Film> exportList = GetFilmsByCriteria(criteria);
            if (exportList.Count == 0)
                return RedirectToAction("Index", new{errorMessage = ERR_CRIT_NULL});
            using (var workbook = new XLWorkbook(XLEventTracking.Disabled))
            {
                var worksheet = workbook.Worksheets.Add(REPORT_NAME);
                FillWorksheet(worksheet, exportList);
                return await DownloadExcel(workbook);
            }
        }

        private async Task<IActionResult> DownloadExcel(IXLWorkbook workbook)
        {
            using (var stream = new MemoryStream())
            {
                workbook.SaveAs(stream);
                await stream.FlushAsync();

                return new FileContentResult(stream.ToArray(), REPORT_FORMAT)
                    { FileDownloadName = $"Cinema_{ DateTime.Now.ToString()}.xlsx" };

            }
        }
        private void FillWorksheet(IXLWorksheet worksheet, List<Film> filmList)
        {
            worksheet.Cell(FILM_CELL).Value = FILM;
            worksheet.Cell(ACTOR_1_CELL).Value = ACTOR_1;
            worksheet.Cell(ACTOR_2_CELL).Value = ACTOR_2;
            worksheet.Cell(ACTOR_3_CELL).Value = ACTOR_3;
            worksheet.Cell(DIRECTOR_CELL).Value = DIRECTOR;
            worksheet.Row(1).Style.Font.Bold = true;

            for (int i = 0; i < filmList.Count; i++)
            {
                var filmName = _context.Films.Find(filmList[i].FilmId).FilmName;
                var actorsName = (from f in _context.Films
                    join ap in _context.ActorPlays on f.FilmId equals ap.FilmId
                    join a in _context.Actors on ap.ActorId equals a.ActorId
                    where f.FilmId == filmList[i].FilmId
                    select a.ActorName).ToList();
                var directorName = (from f in _context.Films
                    join d in _context.CratorsTeams on f.CreationTeamId equals d.CreationTeamId
                    where f.FilmId == filmList[i].FilmId
                    select d.DirectorName).ToList()[0];

                int nameCounter = 1;
                worksheet.Cell(i + 2, FILM_IND).Value = filmName;
                foreach (var actorName in actorsName)
                {
                    if (nameCounter < 4)    //max 3 names
                        worksheet.Cell(i + 2, nameCounter + 1).Value = actorName;
                    nameCounter++;
                }
                worksheet.Cell(i + 2, DIRECTOR_IND).Value = directorName;
            }
        }

        private List<Film> GetFilmsByCriteria(string criteria)
        {
            if (criteria == null)
            {
                return _context.Films.ToList();
            }
            return _context.Films.Where(film => film.FilmName.Contains(criteria)).ToList();
        }


    }
}
