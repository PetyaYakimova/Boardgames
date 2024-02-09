using Boardgames.Data.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Serialization;

namespace Boardgames.DataProcessor.ExportDto
{
    [XmlType("Creator")]
    public class ExportCreatorDto
    {
        [XmlAttribute("BoardgamesCount")]
        public int BoardgamesCount { get; set; }

        [XmlElement("CreatorName")]
        public string Name { get; set; } = null!;

        [XmlArray("Boardgames")]
        public List<ExportBoardgameDto> Boardgames { get; set; } = new List<ExportBoardgameDto>();
    }
}
