namespace SortingNetworks.Utils
{
    public interface IBinarySerializer
    {
        string FilePath { get; }

        void Serialize<T>(T toSerialize);

        T Deserialize<T>();
    }
}