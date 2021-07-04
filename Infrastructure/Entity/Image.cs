using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Entity
{
    public class Image
    {
        public int imageId { get; set; }
        public string contentType { get; set; }
        public string path { get; set; }
    }
}
