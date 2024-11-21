using System.Collections.Generic;
using UdonSharp;
using UdonSharpEditor;

namespace HoshinoLabs.Sardinject.Udon {
    public sealed class DataWriter : IDataWriter {
        readonly List<string> bufferNames = new List<string>();
        readonly List<object> bufferValues = new List<object>();

        public DataWriter() {

        }

        public object GetDataDump() {
            return bufferValues
                .ToArray();
        }

        public void WriteNull(string name) {
            bufferNames.Add(name);
            bufferValues.Add(null);
        }

        public void WriteChar(string name, char value) {
            bufferNames.Add(name);
            bufferValues.Add(value);
        }

        public void WriteString(string name, string value) {
            bufferNames.Add(name);
            bufferValues.Add(value);
        }

        public void WriteReference(string name, object value) {
            bufferNames.Add(name);
            value = value != null && typeof(UdonSharpBehaviour).IsAssignableFrom(value.GetType())
                ? UdonSharpEditorUtility.GetBackingUdonBehaviour((UdonSharpBehaviour)value)
                : value;
            bufferValues.Add(value);
        }

        public void WriteSByte(string name, sbyte value) {
            bufferNames.Add(name);
            bufferValues.Add(value);
        }

        public void WriteInt16(string name, short value) {
            bufferNames.Add(name);
            bufferValues.Add(value);
        }

        public void WriteInt32(string name, int value) {
            bufferNames.Add(name);
            bufferValues.Add(value);
        }

        public void WriteInt64(string name, long value) {
            bufferNames.Add(name);
            bufferValues.Add(value);
        }

        public void WriteByte(string name, byte value) {
            bufferNames.Add(name);
            bufferValues.Add(value);
        }

        public void WriteUInt16(string name, ushort value) {
            bufferNames.Add(name);
            bufferValues.Add(value);
        }

        public void WriteUInt32(string name, uint value) {
            bufferNames.Add(name);
            bufferValues.Add(value);
        }

        public void WriteUInt64(string name, ulong value) {
            bufferNames.Add(name);
            bufferValues.Add(value);
        }

        public void WriteDecimal(string name, decimal value) {
            bufferNames.Add(name);
            bufferValues.Add(value);
        }

        public void WriteSingle(string name, float value) {
            bufferNames.Add(name);
            bufferValues.Add(value);
        }

        public void WriteDouble(string name, double value) {
            bufferNames.Add(name);
            bufferValues.Add(value);
        }

        public void WriteBoolean(string name, bool value) {
            bufferNames.Add(name);
            bufferValues.Add(value);
        }
    }
}
