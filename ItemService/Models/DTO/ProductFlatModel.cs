using System;

namespace ProductService.Models.DTO
{
    public class ProductFlatModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
    }
}
