using System;

namespace HoshinoLabs.Sardinject.Udon {
    public interface IDataReader {
        bool ReadNull();
        bool ReadChar(out char value);
        bool ReadString(out string value);
        bool ReadReference(out object value);
        bool ReadSByte(out sbyte value);
        bool ReadInt16(out short value);
        bool ReadInt32(out int value);
        bool ReadInt64(out long value);
        bool ReadByte(out byte value);
        bool ReadUInt16(out ushort value);
        bool ReadUInt32(out uint value);
        bool ReadUInt64(out ulong value);
        bool ReadDecimal(out decimal value);
        bool ReadSingle(out float value);
        bool ReadDouble(out double value);
        bool ReadBoolean(out bool value);
        void SkipEntry();
    }
}
