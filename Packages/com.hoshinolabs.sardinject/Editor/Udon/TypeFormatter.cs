using System;
using System.Reflection;
using VRC.Udon.Serialization.OdinSerializer;

namespace HoshinoLabs.Sardinject.Udon {
    internal sealed class TypeFormatter<T> : MinimalBaseFormatter<T> {
        readonly TypeFormatter typeFormatter = new TypeFormatter();
        readonly MethodInfo readMethod;
        readonly MethodInfo writeMethod;
        readonly MethodInfo getUninitializedObjectMethod;

        public TypeFormatter(MethodInfo readMethod, MethodInfo writeMethod, MethodInfo getUninitializedObjectMethod) {
            this.readMethod = readMethod;
            this.writeMethod = writeMethod;
            this.getUninitializedObjectMethod = getUninitializedObjectMethod;
        }

        protected override void Read(ref T value, VRC.Udon.Serialization.OdinSerializer.IDataReader reader) {
            var parameters = new object[] { value, reader };
            readMethod.Invoke(typeFormatter, parameters);
            value = (T)parameters[0];
        }

        protected override void Write(ref T value, VRC.Udon.Serialization.OdinSerializer.IDataWriter writer) {
            var parameters = new object[] { value, writer };
            writeMethod.Invoke(typeFormatter, parameters);
            value = (T)parameters[0];
        }

        protected override T GetUninitializedObject() {
            return (T)getUninitializedObjectMethod.Invoke(typeFormatter, Array.Empty<object>());
        }
    }
}
