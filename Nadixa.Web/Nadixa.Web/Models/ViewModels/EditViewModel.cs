using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nadixa.Core.Entities;

namespace Nadixa.Web.Models.ViewModels
{
    public class EditViewModel
    {
        public Product Product { get; set; } = new Product();
        [ValidateNever]
        public IEnumerable<SelectListItem> Categories { get; set; }
        [ValidateNever]
        public IFormFile MainImageUrl { get; set; }

        public bool IsFeatured { get; set; }
        public bool HasDiscount => Product.OldPrice.HasValue && Product.OldPrice > Product.Price;

        public List<string> ImageUrls { get; set; } = new List<string>();
        //public List<ColorViewModel> Colors { get; set; } = new List<ColorViewModel>();

    }
}
