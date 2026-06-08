using System;

namespace Shop.Domain.Entities
{
    public class Phone
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Subtitle { get; set; }
        public string Storage { get; set; }
        public string Ram { get; set; }
        public int? Year { get; set; }
        public string Tags { get; set; } // comma separated tags
        public string Screen { get; set; }
        public string Camera { get; set; }
        public string Battery { get; set; }
        public decimal? PriceMin { get; set; }
        public decimal? PriceMax { get; set; }
        public string ImageUrl { get; set; }
    }
}
