using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils
{
    public static class FileHelpers
    {
        public static async Task<byte[]> ReadFileAsync(string path) 
        {
            using var fs = new FileStream(path, FileMode.Open);

            using var bs = new BufferedStream(fs, 4096);

            var bytes = new byte[fs.Length];

            await bs.ReadAsync(bytes, 0, bytes.Length);

            return bytes;

        }
    }
}
