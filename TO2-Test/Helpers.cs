using Xunit;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace KontrolSystem.TO2.Test {
    public static class Helpers {
        public static void ShouldDeepEqual(
            object expected,
            object actual,
            string[] ignore,
            string fieldPath = "Field ",
            BindingFlags bindingFlags = BindingFlags.NonPublic |
                                        BindingFlags.Public |
                                        BindingFlags.Instance) {
            if (expected == null && actual == null)
                return;

            Assert.NotNull(expected);
            Assert.NotNull(actual);

            var type = expected.GetType();
            Assert.IsAssignableFrom(type, actual);

            foreach (var field in type.GetFields(bindingFlags)) {
                if (ignore?.Contains(field.Name) ?? false) continue;

                var fieldType = field.FieldType;
                var expectedValue = field.GetValue(expected);
                var actualValue = field.GetValue(actual);

                if (fieldType.IsValueType || fieldType == typeof(string)) {
                    Assert.True(expectedValue != null ? expectedValue.Equals(actualValue) : actualValue == null,
                        fieldPath + field.Name);
                }

                if (fieldType.IsClass || fieldType.IsInterface) {
                    if (fieldType.IsGenericType && fieldType.GetGenericTypeDefinition() == typeof(List<>)) {
                        List<object> expectedList = (expectedValue as IEnumerable<object>).ToList();
                        List<object> actualList = (actualValue as IEnumerable<object>).ToList();

                        Assert.True(expectedList.Count == actualList.Count, fieldPath + field.Name + ".Count");
                        for (int i = 0; i < expectedList.Count; i++) {
                            ShouldDeepEqual(expectedList[i], actualList[i], ignore, fieldPath + field.Name + $"[{i}].");
                        }
                    } else {
                        ShouldDeepEqual(expectedValue, actualValue, ignore, fieldPath + field.Name + ".");
                    }
                }
            }
        }
    }
}
