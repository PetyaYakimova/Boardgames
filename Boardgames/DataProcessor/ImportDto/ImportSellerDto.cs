using Boardgames.Data.Models;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Boardgames.DataProcessor.ImportDto
{
    public class ImportSellerDto
    {
        [JsonProperty("Name")]
        [Required]
        [MaxLength(20)]
        [MinLength(5)]
        public string Name { get; set; } = null!;

        [JsonProperty("Address")]
        [Required]
        [MaxLength(30)]
        [MinLength(2)]
        public string Address { get; set; } = null!;

        [JsonProperty("Country")]
        [Required]
        public string Country { get; set; } = null!;

        [JsonProperty("Website")]
        [Required]
        [RegularExpression("www\\.[a-zA-Z\\d-]{1,}\\.com")]
        public string Website { get; set; } = null!;

        [JsonProperty("Boardgames")]
        [NotMapped]
        public List<int> Boardgames { get; set; } = new List<int>();
    }
}
