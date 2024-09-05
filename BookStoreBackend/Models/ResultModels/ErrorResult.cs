using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace BookStoreBackend.Models.ResultModels
{
    public class ErrorResult
    {
        public bool Result => false;
        public string Message { get; set; }
    }
    public class ErrorDataResult
    {
        public bool Result => false;
        public string Message { get; set; }
        public IEnumerable<string> Errors { get; set; }
    }
    public static class ModelStateExtensions        // for listing server-side errors
    {
        public static IEnumerable<string> GetErrors(this ModelStateDictionary modelState)
        {
            return modelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));
        }
    }
}
