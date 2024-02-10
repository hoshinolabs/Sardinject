using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoshinoLabs.VRC.Sardinject {
    internal sealed class Installer {
        Action<ContainerBuilder> configuration;

        public Installer(Action<ContainerBuilder> configuration) {
            this.configuration = configuration;
        }

        public void Install(ContainerBuilder builder) {
            configuration(builder);
        }
    }
}
