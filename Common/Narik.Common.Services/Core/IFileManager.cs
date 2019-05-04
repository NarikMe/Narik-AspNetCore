using System.Collections.Generic;

namespace Narik.Common.Services.Core
{

    public class FileSpec
    {
        public FileSpec()
        {
            
        }

        public FileSpec(string key, string pathFormat,bool isImage=true)
        {
            Key = key;
            PathFormat = pathFormat;
            IsImage = isImage;
        }

        public string Key { get; set; }
        public string PathFormat { get; set; }

        public bool IsImage { get; set; }

    }
    public interface IFileManager
    {
        void AddFileSpecs(List<FileSpec> fileSpecs);

        bool SaveFile(string data, string fileType, params object[] parameters);
        bool SaveFile(string data, object fileType, params object[] parameters);
        string GetFile(string fileType, params object[] parameters);
        string GetFile(object fileType, params object[] parameters);
        void RemovFile(string fileType, params object[] parameters);
        void RemovFile(object fileType, params object[] parameters);

        string GetFileIgnoreExt(object fileType, params object[] parameters);
        void RemovFileIgnoreExt(object fileType, params object[] parameters);
    }
}
