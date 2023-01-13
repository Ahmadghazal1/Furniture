using System.Collections.Generic;

namespace Furniture.Models.ViewModel
{
    public class DataVM
    {
        public IEnumerable<CategoryProject> categoryProjects { get; set; }
        public IEnumerable<Testimonial> testimonials { get; set; }
        public IEnumerable<ProductProject> productProjects { get; set; }
        public IEnumerable<AboutU> AboutUs { get; set; }
        public IEnumerable<UserAccount> userAccounts { get; set; }
        public IEnumerable<HomePaje> homePajes { get; set; }
    

    }
}
