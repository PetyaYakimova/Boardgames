namespace Boardgames.DataProcessor
{
    using AutoMapper;
    using Boardgames.Data;
    using Boardgames.Data.Models;
    using Boardgames.DataProcessor.ExportDto;
    using Boardgames.Utilities;
    using Microsoft.EntityFrameworkCore;
    using Newtonsoft.Json;

    public class Serializer
    {
        public static string ExportCreatorsWithTheirBoardgames(BoardgamesContext context)
        {
            Mapper mapper = new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<BoardgamesProfile>();
            }));
            XmlHelper xmlHelper = new XmlHelper();

            List<Creator> creators = context.Creators
                .Include(c=>c.Boardgames)
                .ToList()
                .Where(c => c.Boardgames.Any())
                .OrderByDescending(c => c.Boardgames.Count)
                .ThenBy(c => $"{c.FirstName} {c.LastName}")
                .ToList();

            foreach (Creator creator in creators)
            {
                creator.Boardgames = creator.Boardgames
                    .OrderBy(b => b.Name)
                    .ToList();
            }

            List<ExportCreatorDto> exportCreators = mapper.Map<List<ExportCreatorDto>>(creators);

            return xmlHelper.Serialize<List<ExportCreatorDto>>(exportCreators, "Creators");
        }

        public static string ExportSellersWithMostBoardgames(BoardgamesContext context, int year, double rating)
        {
            var sellers = context.Sellers
                .Include(s => s.BoardgamesSellers)
                .ThenInclude(bs => bs.Boardgame)
                .AsNoTracking()
                .Where(s => s.BoardgamesSellers.Any(bs => bs.Boardgame.YearPublished >= year && bs.Boardgame.Rating <= rating))
                .Select(s => new
                {
                    s.Name,
                    s.Website,
                    Boardgames = s.BoardgamesSellers
                        .Where(bs => bs.Boardgame.YearPublished >= year && bs.Boardgame.Rating <= rating)
                        .Select(b => new
                        {
                            Name = b.Boardgame.Name,
                            Rating = b.Boardgame.Rating,
                            Mechanics = b.Boardgame.Mechanics,
                            Category = b.Boardgame.CategoryType.ToString()
                        })
                        .OrderByDescending(b => b.Rating)
                        .ThenBy(b => b.Name)
                        .ToList()
                })
                .OrderByDescending(s => s.Boardgames.Count)
                .ThenBy(s => s.Name)
                .Take(5)
                .ToList();

            return JsonConvert.SerializeObject(sellers, new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            });
        }
    }
}