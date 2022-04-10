using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CinemeOnlineWeb
{
    public partial class Actor
    {
        public Actor()
        {
            ActorPlays = new HashSet<ActorPlay>();
        }

        public int ActorId { get; set; }
        public string ActorName { get; set; } = null!;
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }

        public virtual ICollection<ActorPlay> ActorPlays { get; set; }
    }
}
