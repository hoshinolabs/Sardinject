namespace HoshinoLabs.Sardinject.Udon {
    public interface ISerializable {
        void Serialize(IDataWriter writer);
        void Deserialize(IDataReader reader);
    }
}
