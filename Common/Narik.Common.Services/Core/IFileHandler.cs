using System.IO;

namespace Narik.Common.Services.Core
{
    public interface IFileHandler
    {
        string Store(Stream  report);

        string Store(byte[] data);

        Stream GetDataAsStream(string key);
        byte[] GetDataAsByteArray(string key);

        void Remove(string key );
    }
}
