using System;
using System.Collections.Generic;

namespace CinemeOnlineWeb
{
    public partial class Response
    {
        public int AuthorId { get; set; }
        public string AuthorName { get; set; } = null!;
        public string ResponseText { get; set; } = null!;
        public int Rating { get; set; }
        public DateTime DataResponse { get; set; }
        public int FilmId { get; set; }

        public virtual Film Film { get; set; } = null!;
    }
}
