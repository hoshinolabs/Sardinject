using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoshinoLabs.VRC.Sardinject {
    public interface IContainerBuilder {
        event Action<Container> OnBuild;

        RegistrationBuilder Register(RegistrationBuilder registrationBuilder);
    }
}
