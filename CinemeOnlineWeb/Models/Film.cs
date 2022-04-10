using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CinemeOnlineWeb
{
    public partial class Film
    {
        public Film()
        {
            ActorPlays = new HashSet<ActorPlay>();
            FilmAwards = new HashSet<FilmAward>();
            Responses = new HashSet<Response>();
        }
        
        public int FilmId { get; set; }

        [Display (Name = "Name")]
        public string FilmName { get; set; } = null!;

        [Display (Name = "Release year")]
        public int YearRelease { get; set; }

        [Display(Name ="Duration")]
        public int Duration { get; set; }

        [Display(Name ="Spent")]
        public int? Cost { get; set; }
        public int CreationTeamId { get; set; }

        [Display(Name = "Director")]
        public virtual CratorsTeam CreationTeam { get; set; } = null!;
        public virtual ICollection<ActorPlay> ActorPlays { get; set; }
        public virtual ICollection<FilmAward> FilmAwards { get; set; }
        public virtual ICollection<Response> Responses { get; set; }
    }
}
