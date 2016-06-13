using System;
using Windows.Storage.Streams;

namespace BasicUwpPlayer
{
    internal class Hearable : IHearable
    {
        public IRandomAccessStream File { get; set; }
        public string Name { get; set; }

        public Hearable(string name, IRandomAccessStream file)
        {
            this.Name = name;
            this.File = file;
        }
    }
}
