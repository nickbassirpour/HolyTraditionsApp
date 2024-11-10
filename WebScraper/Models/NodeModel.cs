using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebScraper.Enums;

namespace WebScraper.Models
{
    internal class NodeModel
    {
        public NodeType Type { get; set; }
        public string Content { get; set; }
        public string? Link { get; set; }
        public bool Italic { get; set; } = false;
        public bool Bold { get; set; } = false;
        public bool Underline { get; set; } = false;
    }
}
