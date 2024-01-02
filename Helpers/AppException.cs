
namespace WebApiDataverseConnection.Helpers
{
    public class AppException : Exception
    {
        public AppException(string message, int errorCode)
        {
            Console.WriteLine(message, errorCode);
            Console.ReadKey();
        }

    }
}
