using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PclLocalizer.Console
{
    class ResourceContainer
    {
        public string Culture { get; set; }
        public Dictionary<string,string> Resource { get; set; }

        public ResourceContainer(string item, string separator)
        {
            var lines = File.ReadAllLines(item);
            Culture = lines[0].Split(new[] { separator }, StringSplitOptions.None).ToList()[1];

            Resource = new Dictionary<string, string>();
            for (var i = 1; i < lines.Length; i++)
            {
                Resource.Add(lines[i].Split(new[] {separator}, StringSplitOptions.None).ToList()[0],
                    lines[i].Split(new[] {separator}, StringSplitOptions.None).ToList()[1]);
            }
        }
    }
}
