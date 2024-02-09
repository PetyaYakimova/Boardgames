namespace Boardgames
{
    using AutoMapper;
    using Boardgames.Data.Models;
    using Boardgames.Data.Models.Enums;
    using Boardgames.DataProcessor.ExportDto;
    using Boardgames.DataProcessor.ImportDto;

    public class BoardgamesProfile : Profile
    {
        // DO NOT CHANGE OR RENAME THIS CLASS!
        public BoardgamesProfile()
        {
            this.CreateMap<ImportBoardgameDto, Boardgame>()
                .ForMember(d => d.CategoryType, opt => opt.MapFrom(s => (CategoryType)s.CategoryType));
            this.CreateMap<Boardgame, ExportBoardgameDto>();

            this.CreateMap<ImportCreatorDto, Creator>();
            this.CreateMap<Creator, ExportCreatorDto>()
                .ForMember(d => d.BoardgamesCount, opt => opt.MapFrom(s => s.Boardgames.Count))
                .ForMember(d => d.Name, opt => opt.MapFrom(s => $"{s.FirstName} {s.LastName}"))
                .ForMember(d => d.Boardgames, opt => opt.MapFrom(s => s.Boardgames));

            this.CreateMap<ImportSellerDto, Seller>();
        }
    }
}