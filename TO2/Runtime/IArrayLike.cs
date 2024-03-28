
using System.Collections.Generic;

namespace KontrolSystem.TO2.Runtime;

public interface IArrayLike {
    long Length { get; }
}

public interface IArrayLike<out T> : IArrayLike, IEnumerable<T> {

    T GetElement(long index);
}
