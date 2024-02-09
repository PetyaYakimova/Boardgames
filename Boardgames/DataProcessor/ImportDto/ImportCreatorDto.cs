using Boardgames.Data.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Serialization;

namespace Boardgames.DataProcessor.ImportDto
{
    [XmlType("Creator")]
    public class ImportCreatorDto
    {
        [Required]
        [MaxLength(7)]
        [MinLength(2)]
        [XmlElement("FirstName")]
        public string FirstName { get; set; } = null!;

        [Required]
        [MaxLength(7)]
        [MinLength(2)]
        [XmlElement("LastName")]
        public string LastName { get; set; } = null!;

        [XmlArray("Boardgames")]
        [NotMapped]
        public List<ImportBoardgameDto> Boardgames { get; set; } = new List<ImportBoardgameDto>();
    }
}
