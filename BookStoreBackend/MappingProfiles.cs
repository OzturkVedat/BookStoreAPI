using AutoMapper;
using BookStoreBackend.Models;
using BookStoreBackend.Models.ViewModels;

namespace BookStoreBackend
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<AuthorModel, AuthorViewModel>().ReverseMap();   // for two-way mapping
            CreateMap<BookModel, BookViewModel>().ReverseMap();

        }
    }
}
