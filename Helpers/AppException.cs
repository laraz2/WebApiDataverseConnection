using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
