using System;
using UdonSharp.Serialization;

namespace HoshinoLabs.Sardinject.Udon {
    public class Serializer<T> : UdonSharp.Serialization.Serializer<T> where T : ISerializable {
        public Serializer(TypeSerializationMetadata typeMetadata)
            : base(typeMetadata) {

        }

        protected override Serializer MakeSerializer(TypeSerializationMetadata typeMetadata) {
            VerifyTypeCheckSanity();
            return new Serializer<T>(typeMetadata);
        }

        protected override bool HandlesTypeSerialization(TypeSerializationMetadata typeMetadata) {
            VerifyTypeCheckSanity();
            return typeMetadata.cSharpType == typeof(T);
        }

        public override void Write(IValueStorage targetObject, in T sourceObject) {
            VerifySerializationSanity();
            if (targetObject == null) {
                Logger.LogError($"Field for {typeof(T)} does not exist");
                return;
            }

            if (targetObject.Value != null && targetObject.Value.GetType() == typeof(object[])) {
                return;
            }

            if (sourceObject == null) {
                targetObject.Value = null;
                return;
            }

            var writer = new DataWriter();
            sourceObject.Serialize(writer);
            var _sourceObject = writer.GetDataDump();
            if (_sourceObject == null) {
                targetObject.Value = null;
                return;
            }
            var behaviourSerializer = CreatePooled(_sourceObject.GetType());
            behaviourSerializer.WriteWeak(targetObject, _sourceObject);
        }

        public override void Read(ref T targetObject, IValueStorage sourceObject) {
            VerifySerializationSanity();
        }

        public override Type GetUdonStorageType() {
            return typeof(object[]);
        }
    }
}
