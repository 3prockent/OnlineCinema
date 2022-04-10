using System;
using System.Collections.Generic;

namespace CinemeOnlineWeb
{
    public partial class CratorsTeam
    {
        public CratorsTeam()
        {
            Films = new HashSet<Film>();
        }

        public int CreationTeamId { get; set; }
        public string DirectorName { get; set; } = null!;
        public int QuantityPeople { get; set; }

        public virtual ICollection<Film> Films { get; set; }
    }
}
