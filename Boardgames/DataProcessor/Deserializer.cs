namespace Boardgames.DataProcessor
{
    using System.ComponentModel.DataAnnotations;
    using AutoMapper;
    using System.Text;
    using Boardgames.Data;
    using Boardgames.Utilities;
    using Boardgames.DataProcessor.ImportDto;
    using Boardgames.Data.Models;
    using Newtonsoft.Json;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedCreator
            = "Successfully imported creator – {0} {1} with {2} boardgames.";

        private const string SuccessfullyImportedSeller
            = "Successfully imported seller - {0} with {1} boardgames.";

        public static string ImportCreators(BoardgamesContext context, string xmlString)
        {
            XmlHelper xmlHelper = new XmlHelper();
            Mapper mapper = new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<BoardgamesProfile>();
            }));
            StringBuilder sb = new StringBuilder();

            ImportCreatorDto[] creators = xmlHelper.Deserialize<ImportCreatorDto[]>(xmlString, "Creators");

            List<Creator> validCreators = new List<Creator>();

            foreach (ImportCreatorDto creator in creators)
            {
                if (!IsValid(creator))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Creator validCreator = mapper.Map<Creator>(creator);
                List<Boardgame> validBoardgames = new List<Boardgame>();

                foreach (ImportBoardgameDto boardgame in creator.Boardgames)
                {
                    if (!IsValid(boardgame))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    Boardgame validBoardgame = mapper.Map<Boardgame>(boardgame);
                    validBoardgames.Add(validBoardgame);
                }

                validCreator.Boardgames = validBoardgames;
                validCreators.Add(validCreator);

                sb.AppendLine(string.Format(SuccessfullyImportedCreator, validCreator.FirstName, validCreator.LastName, validCreator.Boardgames.Count));
            }

            context.Creators.AddRange(validCreators);
            context.SaveChanges();

            return sb.ToString().Trim();
        }

        public static string ImportSellers(BoardgamesContext context, string jsonString)
        {
            Mapper mapper = new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<BoardgamesProfile>();
            }));
            StringBuilder sb = new StringBuilder();

            List<int> validBoardgamesIds = context.Boardgames.Select(b => b.Id).ToList();

            ImportSellerDto[] sellers = JsonConvert.DeserializeObject<ImportSellerDto[]>(jsonString);

            List<Seller> validSellers = new List<Seller>();

            foreach (ImportSellerDto seller in sellers)
            {
                if (!IsValid(seller))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Seller validSeller = mapper.Map<Seller>(seller);
                seller.Boardgames = seller.Boardgames.Distinct().ToList();

                foreach (int boardgameId in seller.Boardgames)
                {
                    if (!validBoardgamesIds.Contains(boardgameId))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    BoardgameSeller boardgameSeller = new BoardgameSeller();
                    boardgameSeller.Seller = validSeller;
                    boardgameSeller.BoardgameId = boardgameId;

                    validSeller.BoardgamesSellers.Add(boardgameSeller);
                }

                validSellers.Add(validSeller);
                sb.AppendLine(string.Format(SuccessfullyImportedSeller, validSeller.Name, validSeller.BoardgamesSellers.Count));
            }

            context.Sellers.AddRange(validSellers);
            context.SaveChanges();

            return sb.ToString().Trim();
        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}
