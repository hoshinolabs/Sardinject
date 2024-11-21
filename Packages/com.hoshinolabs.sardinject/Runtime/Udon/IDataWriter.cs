using System;

namespace HoshinoLabs.Sardinject.Udon {
    public interface IDataWriter {
        void WriteNull(string name);
        void WriteChar(string name, char value);
        void WriteString(string name, string value);
        void WriteReference(string name, object value);
        void WriteSByte(string name, sbyte value);
        void WriteInt16(string name, short value);
        void WriteInt32(string name, int value);
        void WriteInt64(string name, long value);
        void WriteByte(string name, byte value);
        void WriteUInt16(string name, ushort value);
        void WriteUInt32(string name, uint value);
        void WriteUInt64(string name, ulong value);
        void WriteDecimal(string name, decimal value);
        void WriteSingle(string name, float value);
        void WriteDouble(string name, double value);
        void WriteBoolean(string name, bool value);
    }
}
