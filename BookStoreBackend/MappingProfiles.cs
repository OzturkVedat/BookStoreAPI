using AutoMapper;
using BookStoreBackend.Models;
using BookStoreBackend.Models.ViewModels;

namespace BookStoreBackend
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<AuthorModel, AuthorFullNameDto>();
            CreateMap<BookModel, BookViewModel>();

            CreateMap<AuthorFullNameDto, AuthorModel>();
            CreateMap<BookViewModel, BookModel>();

        }
    }
}
