using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using VRC.Udon.Serialization.OdinSerializer.Utilities;

namespace HoshinoLabs.Sardinject.Udon {
    internal sealed class DefaultArraySerializer : UdonSharp.Serialization.Serializer {
        [InitializeOnLoadMethod]
        static void OnLoad() {
            var typeCheckSerializersField = typeof(UdonSharp.Serialization.Serializer).GetField("_typeCheckSerializers", BindingFlags.Static | BindingFlags.NonPublic);
            var typeCheckSerializers = (List<UdonSharp.Serialization.Serializer>)typeCheckSerializersField.GetValue(null);
            typeCheckSerializers.RemoveAll(x => x.GetType() == typeof(DefaultArraySerializer));
            var idx = typeCheckSerializers.FindIndex(x => x.GetType().FullName.StartsWith("UdonSharp.Serialization.ArraySerializer"));
            typeCheckSerializers.Insert(Math.Max(idx, 0), new DefaultArraySerializer());
        }

        public DefaultArraySerializer()
            : base(null) {

        }

        protected override UdonSharp.Serialization.Serializer MakeSerializer(UdonSharp.Serialization.TypeSerializationMetadata typeMetadata) {
            var type = typeof(ArraySerializer<>).MakeGenericType(typeMetadata.cSharpType.GetElementType());
            return (UdonSharp.Serialization.Serializer)Activator.CreateInstance(type, new object[] { typeMetadata });
        }

        protected override bool HandlesTypeSerialization(UdonSharp.Serialization.TypeSerializationMetadata typeMetadata) {
            VerifyTypeCheckSanity();
            if (!typeMetadata.cSharpType.IsArray) {
                return false;
            }
            var elementType = typeMetadata.cSharpType.GetElementType();
            return !elementType.IsArray && elementType.GetBaseTypes().Any(x => typeof(ISerializable).IsAssignableFrom(x));
        }

        public override void WriteWeak(UdonSharp.Serialization.IValueStorage targetObject, object sourceObject) {
            throw new NotImplementedException();
        }

        public override void ReadWeak(ref object targetObject, UdonSharp.Serialization.IValueStorage sourceObject) {
            throw new NotImplementedException();
        }

        public override Type GetUdonStorageType() {
            throw new NotImplementedException();
        }
    }
}
