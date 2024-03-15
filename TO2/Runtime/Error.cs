using System;

namespace KontrolSystem.TO2.Runtime;

public class Error(string message) {
    public string message = message;
    
    public string Message => message;
    
    public override string ToString() => message;
}
