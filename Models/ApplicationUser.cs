
using Microsoft.AspNetCore.Identity;

namespace HobbyHarbour.Models
{
    public class ApplicationUser : IdentityUser
    {
        // You can add your custom properties here.
        public string FirstName { get; set; }
        public string LastName { get; set; }
        // Other custom properties
    }

}

