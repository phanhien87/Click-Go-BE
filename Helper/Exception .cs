namespace Click_Go.Helper
{
    public class AppException : Exception
    {
        public AppException(string message) : base(message) { }
    }
    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message) { }
    }
}
