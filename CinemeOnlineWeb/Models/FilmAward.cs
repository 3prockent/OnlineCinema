using System;
using System.Collections.Generic;

namespace CinemeOnlineWeb
{
    public partial class FilmAward
    {
        public int FilmAwardsId { get; set; }
        public int FilmId { get; set; }
        public int AwardId { get; set; }

        public virtual Award Award { get; set; } = null!;
        public virtual Film Film { get; set; } = null!;
    }
}
