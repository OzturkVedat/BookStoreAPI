namespace BookStoreBackend.Models.ResultModels
{
    public class SuccessResult
    {
        public bool Success => true;    // always true
        public string Message {  get; set; }
    }
    public class SuccessDataResult <T>
    {
        public bool Success => true; 
        public string Message { get; set; }
        public T Data { get; set; }
    }
}
