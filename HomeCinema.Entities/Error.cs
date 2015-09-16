using System;

namespace HomeCinema.Entities
{
    public class Error : IEntityBase
    {
        public string Message { get; set; }
        public string StackTrace { get; set; }
        public DateTime DateCreated { get; set; }
        public int ID { get; set; }
    }
}