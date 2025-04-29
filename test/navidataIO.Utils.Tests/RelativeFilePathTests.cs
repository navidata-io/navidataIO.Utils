// Copyright (c) 2024 navidata.io Corp
// LICENSE-SPDX: <LGPL-3.0-only>

namespace navidataIO.Utils.Tests;

using Xunit;

public class RelativeFilePathTests
{
    // ReSharper disable StringLiteralTypo

    [Theory]
    [InlineData("", 0)]
    [InlineData("C:/Temp/tmp1klltc.tmp/output/version.json", 4)]
    [InlineData("Temp/tmp1klltc.tmp/output/version.json", 4)]
    [InlineData("/Temp/tmp1klltc.tmp/output/version.json", 4)]
    [InlineData("file://C:/Temp/tmp1klltc.tmp/output/version.json", 4)]
    [InlineData(@"C:\Temp\tmp1klltc.tmp\output\version.json", 4)]
    [InlineData(@"Temp\tmp1klltc.tmp\output\version.json", 4)]
    public void Implicit_from_string(string path, int expectedSegmentsCount)
    {
        RelativeFilePath relativePath = path;
        Assert.NotEqual(relativePath, default(RelativeFilePath));
        Assert.Equal(expectedSegmentsCount, relativePath.Segments.Length);
    }

    [Theory]
    [InlineData(@"C:\root\with spaces\foo", @"C:\root\with spaces\", '/', "foo")]
    [InlineData(@"C:\root\with spaces\foo\bar", @"C:\root\with spaces\", '/', "foo/bar")]
    [InlineData(@"C:\root\with spaces\foo\bar", @"C:\root\with spaces", '/', "with spaces/foo/bar")]
    public void Create_from_string(string fullPath, string basePath, char separator, string expectedResult)
    {
        var relPath = RelativeFilePath.Create(fullPath, basePath, separator);
        Assert.Equal(expectedResult, relPath);
    }
}