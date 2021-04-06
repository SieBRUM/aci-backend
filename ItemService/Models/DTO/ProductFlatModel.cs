using System;

namespace ProductService.Models.DTO
{
    public class ProductFlatModel
    {
        public int Id { get; set; }
        public int CatalogNumber { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsAvailable { get; set; }
        public DateTime? ArchivedSince { get; set; }
        public Category Category { get; set; }
        public string Image { get; set; }
    }
}
