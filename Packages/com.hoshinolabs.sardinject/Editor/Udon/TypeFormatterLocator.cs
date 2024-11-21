using HoshinoLabs.Sardinject.Udon;
using System;
using System.Reflection;
using VRC.Udon.Serialization.OdinSerializer;

[assembly: RegisterFormatterLocator(typeof(TypeFormatterLocator), 0)]
namespace HoshinoLabs.Sardinject.Udon {
    public class TypeFormatterLocator : IFormatterLocator {
        public bool TryGetFormatter(Type type, FormatterLocationStep step, ISerializationPolicy policy, out VRC.Udon.Serialization.OdinSerializer.IFormatter formatter) {
            formatter = null;
            if (!typeof(Type).IsAssignableFrom(type)) {
                return false;
            }

            var readMethod = typeof(TypeFormatter).GetMethod("Read", BindingFlags.Instance | BindingFlags.NonPublic);
            if (readMethod == null) {
                return false;
            }
            var writeMethod = typeof(TypeFormatter).GetMethod("Write", BindingFlags.Instance | BindingFlags.NonPublic);
            if (writeMethod == null) {
                return false;
            }
            var getUninitializedObjectMethod = typeof(TypeFormatter).GetMethod("GetUninitializedObject", BindingFlags.Instance | BindingFlags.NonPublic);
            if (getUninitializedObjectMethod == null) {
                return false;
            }

            var _type = typeof(TypeFormatter<>).MakeGenericType(type);
            var args = new object[] { readMethod, writeMethod, getUninitializedObjectMethod };
            formatter = (IFormatter)Activator.CreateInstance(_type, args);

            return true;
        }
    }
}
