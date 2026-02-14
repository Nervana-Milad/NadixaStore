using Nadixa.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nadixa.Core.Entities
{
    public class ProductColor : BaseEntity
    {
        public string Name { get; set; } = string.Empty; // اسم اللون (Red)
        public string HexCode { get; set; } = string.Empty; // كود اللون (#FF0000) عشان الـ CSS

        // ربط بالمنتج
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
    }
}
