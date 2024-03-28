
using System;

namespace KontrolSystem.TO2.Runtime;

public interface IArrayLike<T> {
    Type ElementType { get; }

    long Length { get; }

    T GetElement(long index);
}

public class Testy : IArrayLike<long> {
    public Type ElementType { get; }
    public long Length { get; }
    public long GetElement(long index) {
        throw new NotImplementedException();
    }
}
