using System.Collections.Generic;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.KSP.Runtime.Core;

internal class REPLState {
    internal readonly IAnyFuture? futureResult;
    internal readonly List<MainframeError> errors;

    public REPLState(IAnyFuture? futureResult, List<MainframeError> errors) {
        this.futureResult = futureResult;
        this.errors = errors;
    }
}
