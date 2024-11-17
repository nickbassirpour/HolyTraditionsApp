using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebScraper.Validation
{
    public record ValidationFailed(string errorMessage)
    {
        public ValidationFailed() : this("Unknown error") { }
    }
}
