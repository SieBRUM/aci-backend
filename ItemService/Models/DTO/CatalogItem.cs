using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductService.Models.DTO
{
    public class CatalogItem
    {
        /// <summary>
        /// The id of the product
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// The name of the product
        /// </summary>
        public string Name { get; set; }

        public int CatalogNumber { get; set; }

        public string Description { get; set; }
        /// <summary>
        /// Whether the product has to be approved before it can be rented
        /// </summary>
        public bool RequiresApproval { get; set; }
        /// <summary>
        /// The availability status of the product
        /// </summary>
        public ProductState Status { get; set; }

        public List<string> Images { get; set; }

        public int ImageIndex { get; set; }

        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }

        public Category Category { get; set; }
    }
}
