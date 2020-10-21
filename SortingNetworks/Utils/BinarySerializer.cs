using System.Diagnostics;

namespace SortingNetworks.Utils
{
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;

    public class BinarySerializer : IBinarySerializer
    {
        public BinarySerializer(string filePath)
        {
            this.FilePath = filePath;
        }

        public string FilePath { get; }

        public void Serialize<T>(T toSerialize)
        {
            var formatter = new BinaryFormatter();
            var stream = new FileStream(this.FilePath, FileMode.Create, FileAccess.Write, FileShare.None);
            Trace.WriteLine($"Writing file at path: {stream.Name}");
            formatter.Serialize(stream, toSerialize);
            stream.Close();
        }

        public T Deserialize<T>()
        {
            var formatter = new BinaryFormatter();
            var stream = new FileStream(this.FilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            var obj = (T)formatter.Deserialize(stream);
            stream.Close();

            return obj;
        }
    }
}
