using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebScraper.Models
{
    internal class ImageNodeModel : NodeModel
    {
        public string? AltText { get; set; }
        public NodeModel? Caption { get; set; }
    }
}
