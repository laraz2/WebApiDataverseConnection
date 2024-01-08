
namespace WebApiDataverseConnection.Helpers
{
    public class AppException : Exception
    {
        //private Func<int> getHashCode;
        //private string message;
        private readonly string? message;
        private readonly Func<int> ?getHashCode;
        //public AppException(string message, int errorCode)
        //{
        //    Console.WriteLine(message, errorCode);
        //    Console.ReadKey();
        //}
            public AppException(string? message, Func<int> getHashCode) : base(message)
            {
                if (message != null)
                {
                    this.message = message;
                    this.getHashCode = getHashCode;
                    Console.WriteLine(message, getHashCode);
                    Console.ReadKey();
                }
                
            }
        

    }
}
