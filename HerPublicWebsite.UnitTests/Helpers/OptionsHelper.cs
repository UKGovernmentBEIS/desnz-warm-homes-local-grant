using Microsoft.Extensions.Options;

namespace Tests.Helpers;

public static class OptionsHelper
{
    public static IOptions<T> AsOptions<T>(this T value) where T : class
    {
        return new TestOptions<T>(value);
    }

    private class TestOptions<T> : IOptions<T> where T : class
    {
        public TestOptions(T value)
        {
            Value = value;
        }

        public T Value { get; }
    }
}