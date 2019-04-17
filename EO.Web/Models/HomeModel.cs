using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EO.Core.Data;

namespace EO.Web.Models
{
    public class HomeModel
    {
        [RegularExpression("^[0-9]{5}$", ErrorMessage = "Invalid zip code")]
        public string ZipCode { get; set; }
        public string ProviderId { get; set; }
        public Dictionary<string, string> Ids { get; set; }
        public List<Rates> Rates { get; set; }
    }
}
