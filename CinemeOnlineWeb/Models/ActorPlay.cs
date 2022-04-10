using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CinemeOnlineWeb
{
    public partial class ActorPlay
    {
        public int ActorPlayId { get; set; }
        [Required]
        public int FilmId { get; set; }
        [Required]
        public int ActorId { get; set; }
        [StringLength(30, MinimumLength = 2, ErrorMessage = "Role must be between 2 and 30 characters")]
        [Required]
        public string Role { get; set; } = null!;
        public int? Salary { get ; set; }
        [Display (Name = "Quantity scenes")]
        public int? QuantityScenes { get; set; }

        public virtual Actor? Actor { get; set; } = null!;
        public virtual Film? Film { get; set; } = null!;
    }
}
