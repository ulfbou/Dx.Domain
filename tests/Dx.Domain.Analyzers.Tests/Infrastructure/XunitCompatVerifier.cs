using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Microsoft.CodeAnalysis.Testing;

using Xunit;

namespace Dx.Domain.Analyzers.Tests.Infrastructure
{
    internal sealed class XunitCompatVerifier : IVerifier
    {
        public static readonly XunitCompatVerifier Instance = new();

        public void Equal<T>(T expected, T actual, string? message = null)
        {
            if (string.IsNullOrWhiteSpace(message))
                Assert.Equal(expected, actual);
            else
                Assert.True(Equals(expected, actual), message);
        }

        public void True(bool condition, string? message = null)
        {
            if (string.IsNullOrWhiteSpace(message))
                Assert.True(condition);
            else
                Assert.True(condition, message);
        }

        public void False(bool condition, string? message = null)
        {
            if (string.IsNullOrWhiteSpace(message))
                Assert.False(condition);
            else
                Assert.False(condition, message);
        }

        public void NotNull(object? value, string? message = null)
        {
            if (string.IsNullOrWhiteSpace(message))
                Assert.NotNull(value);
            else
                Assert.True(value is not null, message);
        }

        public void Null(object? value, string? message = null)
        {
            if (string.IsNullOrWhiteSpace(message))
                Assert.Null(value);
            else
                Assert.True(value is null, message);
        }

        public void Empty<T>(string message, IEnumerable<T> collection)
        {
            ArgumentNullException.ThrowIfNull(collection);

            if (string.IsNullOrWhiteSpace(message))
                Assert.Empty(collection);
            else
                Assert.True(!collection.Any(), message);
        }

        public void NotEmpty<T>(string message, IEnumerable<T> collection)
        {
            ArgumentNullException.ThrowIfNull(collection);

            if (string.IsNullOrWhiteSpace(message))
                Assert.NotEmpty(collection);
            else
                Assert.True(collection.Any(), message);
        }

        public void SequenceEqual<T>(
            IEnumerable<T> expected,
            IEnumerable<T> actual,
            IEqualityComparer<T>? comparer,
            string? message)
        {
            ArgumentNullException.ThrowIfNull(expected);
            ArgumentNullException.ThrowIfNull(actual);

            var matches = comparer is null
                ? expected.SequenceEqual(actual)
                : expected.SequenceEqual(actual, comparer);

            if (string.IsNullOrWhiteSpace(message))
                Assert.True(matches);
            else
                Assert.True(matches, message);
        }

        public void LanguageIsSupported(string language)
        {
            if (string.IsNullOrWhiteSpace(language))
                throw new ArgumentException("Language must be specified.", nameof(language));
        }

        public IVerifier PushContext(string message)
        {
            _ = message;
            return this;
        }

        [DoesNotReturn]
        public void Fail(string? message = null)
        {
            if (string.IsNullOrWhiteSpace(message))
                Assert.True(false);
            else
                Assert.Fail(message);
        }

    }
}
