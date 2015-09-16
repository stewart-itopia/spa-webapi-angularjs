﻿using System;

namespace HomeCinema.Entities
{
    /// <summary>
    ///     HomeCinema Rental Info
    /// </summary>
    public class Rental : IEntityBase
    {
        public int CustomerId { get; set; }
        public int StockId { get; set; }
        public virtual Stock Stock { get; set; }
        public DateTime RentalDate { get; set; }
        public DateTime? ReturnedDate { get; set; }
        public string Status { get; set; }
        public int ID { get; set; }
    }
}