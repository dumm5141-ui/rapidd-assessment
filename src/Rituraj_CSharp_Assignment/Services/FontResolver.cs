using System;
using System.IO;
using System.Reflection;
using SkiaSharp;

namespace Rituraj_CSharp_Assignment.Services;

/// <summary>
/// Ensures robust cross-platform font resolution by leveraging embedded open-source fonts
/// when host system fonts are absent or minimal (such as headless Linux environments).
/// </summary>
public static class FontResolver
{
    private static SKTypeface? _cachedTypeface;
    private static readonly object _lock = new();

    public static SKTypeface GetDefaultTypeface()
    {
        if (_cachedTypeface != null)
        {
            return _cachedTypeface;
        }

        lock (_lock)
        {
            if (_cachedTypeface != null)
            {
                return _cachedTypeface;
            }

            // 1. Try to load embedded DejaVuSans resource
            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                string resourceName = "Rituraj_CSharp_Assignment.Resources.DejaVuSans.ttf";

                using var stream = assembly.GetManifestResourceStream(resourceName);
                if (stream != null)
                {
                    _cachedTypeface = SKTypeface.FromStream(stream);
                    if (_cachedTypeface != null)
                    {
                        return _cachedTypeface;
                    }
                }
            }
            catch
            {
                // Fall through to system fonts
            }

            // 2. Try standard system fonts
            string[] preferredFonts = { "Segoe UI", "Arial", "DejaVu Sans", "Helvetica", "Roboto" };
            foreach (var fontName in preferredFonts)
            {
                var tf = SKTypeface.FromFamilyName(fontName);
                if (tf != null && !string.IsNullOrEmpty(tf.FamilyName))
                {
                    _cachedTypeface = tf;
                    return _cachedTypeface;
                }
            }

            // 3. Fallback to platform default
            _cachedTypeface = SKTypeface.Default;
            return _cachedTypeface;
        }
    }
}
