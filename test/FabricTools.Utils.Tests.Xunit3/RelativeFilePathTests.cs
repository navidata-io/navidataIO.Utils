namespace FabricTools.Utils.Tests;

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
}