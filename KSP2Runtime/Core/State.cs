using System;
using System.Collections.Generic;
using KontrolSystem.TO2;

namespace KontrolSystem.KSP.Runtime.Core;

internal class State {
    internal readonly List<MainframeError> errors;
    internal readonly KontrolRegistry? registry;

    internal TimeSpan bootTime;

    internal State(KontrolRegistry? registry, TimeSpan bootTime, List<MainframeError> errors) {
        this.registry = registry;
        this.bootTime = bootTime;
        this.errors = errors;
    }
}
