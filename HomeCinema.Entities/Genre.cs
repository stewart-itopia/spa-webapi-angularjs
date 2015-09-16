using System.Collections.Generic;

namespace HomeCinema.Entities
{
    /// <summary>
    ///     HomeCinema Movie Genre
    /// </summary>
    public class Genre : IEntityBase
    {
        public Genre()
        {
            Movies = new List<Movie>();
        }

        public string Name { get; set; }
        public virtual ICollection<Movie> Movies { get; set; }
        public int ID { get; set; }
    }
}