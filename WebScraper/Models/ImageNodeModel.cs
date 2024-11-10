using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebScraper.Enums;

namespace WebScraper.Models
{
    internal class ImageNodeModel : NodeModel
    {
        public NodeType Type { get; set; } = NodeType.Image;
        public string Src { get; set; }
        public string? AltText { get; set; }
        public NodeModel? Caption { get; set; }
    }
}
