namespace Zimra.ApiClient.Helpers
{
    public class FileParameter(Stream data, string fileName, string contentType)
    {
        public FileParameter(Stream data)
            : this(data, null, null)
        {
        }

        public FileParameter(Stream data, string fileName)
            : this(data, fileName, null)
        {
        }

        public Stream Data { get; private set; } = data;

        public string FileName { get; private set; } = fileName;

        public string ContentType { get; private set; } = contentType;
    }

}
