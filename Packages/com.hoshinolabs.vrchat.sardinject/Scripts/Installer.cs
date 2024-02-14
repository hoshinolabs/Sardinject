using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoshinoLabs.VRC.Sardinject {
    internal sealed class Installer {
        Action<IContainerBuilder> configuration;

        public Installer(Action<IContainerBuilder> configuration) {
            this.configuration = configuration;
        }

        public void Install(IContainerBuilder builder) {
            configuration(builder);
        }
    }
}
