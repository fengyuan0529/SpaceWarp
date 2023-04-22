﻿using System;
using Newtonsoft.Json;
using SpaceWarp.API.Mods.JSON.Converters;

namespace SpaceWarp.API.Mods.JSON;

/// <summary>
/// Represents the version of the swinfo.json file specification
/// </summary>
[JsonConverter(typeof(SpecVersionConverter))]
public sealed record SpecVersion
{
    private const int DefaultMajor = 1;
    private const int DefaultMinor = 0;

    public int Major { get; } = DefaultMajor;
    public int Minor { get; } = DefaultMinor;

    /// <summary>
    /// Specification version 1.0 (SpaceWarp &lt; 1.2), used if "spec" is not specified in the swinfo.json file.
    /// </summary>
    public static SpecVersion SpecDefault => new();

    /// <summary>
    /// Specification version 1.2 (SpaceWarp 1.2.x), replaces SpaceWarp's proprietary ModID with BepInEx plugin GUID.
    /// </summary>
    public static SpecVersion SpecBepInExGuid => new(1, 2);

    /// <summary>
    ///
    /// </summary>
    /// <param name="major">Major version number</param>
    /// <param name="minor">Minor version number</param>
    public SpecVersion(int major, int minor)
    {
        Major = major;
        Minor = minor;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="version">Specification version in the format "major.minor"</param>
    /// <exception cref="InvalidSpecVersionException"></exception>
    public SpecVersion(string version = null)
    {
        if (version == null)
        {
            return;
        }

        var split = version.Split('.');
        if (split.Length != 2 || !int.TryParse(split[0], out var major) || !int.TryParse(split[1], out var minor))
        {
            throw new InvalidSpecVersionException(version);
        }

        Major = major;
        Minor = minor;
    }

    public override string ToString() => $"{Major}.{Minor}";

    public static bool operator <(SpecVersion a, SpecVersion b) => Compare(a, b) < 0;
    public static bool operator >(SpecVersion a, SpecVersion b) => Compare(a, b) > 0;
    public static bool operator <=(SpecVersion a, SpecVersion b) => Compare(a, b) <= 0;
    public static bool operator >=(SpecVersion a, SpecVersion b) => Compare(a, b) >= 0;

    private static int Compare(SpecVersion a, SpecVersion b)
    {
        if (a.Major != b.Major)
        {
            return a.Major - b.Major;
        }

        return a.Minor - b.Minor;
    }
}

public sealed class InvalidSpecVersionException : Exception
{
    public InvalidSpecVersionException(string version) : base(
        $"Invalid spec version: {version}. The correct format is \"major.minor\".")
    {
    }
}

public sealed class DeprecatedSpecPropertyException : Exception
{
    public DeprecatedSpecPropertyException(string property, SpecVersion deprecationVersion) : base(
        $"The property \"{property}\" is deprecated in the spec version {deprecationVersion} and will be removed completely in the future."
    )
    {
    }
}