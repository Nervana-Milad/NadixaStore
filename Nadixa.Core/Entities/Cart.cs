using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nadixa.Core.Entities
{
    public class Cart
    {

        public int Id { get; set; }

        public string UserId { get; set; }  // if using Identity
        public AppUser User { get; set; }


        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();


    }
}
