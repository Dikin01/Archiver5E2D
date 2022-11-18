using Archiver5E2D.CompressionAlgorithms;

namespace Tests.CompressorsTests;

public class RlePrefixTests
{
    //TODO: Поменять название тестов
    [Fact]
    public void Test1()
    {
        var bytes = new byte[] { 2, 1, 1, 1, 0, 1, 0 };
        var algo = new RlePrefix();

        var result = algo.Compress(2, bytes);

        result.Should().Equal(2, 0, 1, 1, 1, 0, 1, 0);
    }
    
    [Fact]
    public void Test2()
    {
        var bytes = new byte[] { 2, 1, 1, 1, 1, 0, 1, 0 };
        var algo = new RlePrefix();

        var result = algo.Compress(2, bytes);

        result.Should().Equal(2, 0, 2, 3, 1, 0, 1, 0);
    }
    
    [Fact]
    public void Test3()
    {
        var bytes = new byte[] { 2, 2, 2, 2, 1, 0, 1, 0 };
        var algo = new RlePrefix();

        var result = algo.Compress(2, bytes);

        result.Should().Equal(2, 3, 2, 1, 0, 1, 0);
    }
    
}