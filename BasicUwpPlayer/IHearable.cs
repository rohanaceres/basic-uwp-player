using Windows.Storage.Streams;

namespace BasicUwpPlayer
{
    internal interface IHearable
    {
        string Name { get; set; }
        IRandomAccessStream File { get; set; }
    }
}
