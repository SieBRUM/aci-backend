using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductService.Models.DTO
{
    public class CatalogItemsWithCategory
    {
        public List<CatalogItem> catalogItems { get; set; }
        public string categoryName { get; set; }
    }
}
