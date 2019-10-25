using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tweetbook.Contracts.Responses
{
    public class AuthFailedResponse
    {
        public IEnumerable<string> ErrorMessages { get; set; }
    }
}
