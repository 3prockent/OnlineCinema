using System;
using System.Collections.Generic;

namespace CinemeOnlineWeb
{
    public partial class Award
    {
        public Award()
        {
            FilmAwards = new HashSet<FilmAward>();
        }

        public int AwardId { get; set; }
        public string Name { get; set; } = null!;

        public virtual ICollection<FilmAward> FilmAwards { get; set; }
    }
}
