﻿using Microsoft.Extensions.DependencyInjection;

namespace ClifferBasic;

internal static class Utility {
    private static IServiceProvider? _serviceProvider;

    internal static void SetServiceProvider(IServiceProvider provider) {
        _serviceProvider = provider;
    }

    internal static IServiceProvider GetServiceProvider() {
        if (_serviceProvider is null) {
            throw new InvalidOperationException("Service provider is not set.");
        }

        return _serviceProvider;
    }

    internal static T? GetService<T>() {
        if (_serviceProvider is null) {
            throw new InvalidOperationException("Service provider is not set.");
        }

        return _serviceProvider.GetService<T>();
    }
}
