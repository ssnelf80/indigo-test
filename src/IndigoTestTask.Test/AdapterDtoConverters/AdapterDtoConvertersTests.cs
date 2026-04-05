using IndigoTestTask.Adapters.Sources.BaseTickConverter;
using IndigoTestTask.Adapters.Sources.Converters;
using IndigoTestTask.Adapters.Sources.Dtos;

namespace IndigoTestTask.Test.AdapterDtoConverters;

public class AdapterDtoConvertersTests
{
    private readonly AliceDomainTickConverter _aliceConverter = new();
    private readonly BobDomainTickConverter _bobConverter = new();
    private readonly ChloeDomainTickConverter _chloeConverter = new();
    
    [Fact]
    public void AliceAdapterConverter_ShouldSuccess()
    {
        // Arrange
        var aliceDto = new AliceSourceDto
        {
            Id = "id",
            Price = "2.0",
            Volume = "1.0"
        };

        // Act
        var tick = _aliceConverter.ToDomainModel(aliceDto);
        // Assert
        Assert.NotNull(tick);
        Assert.Equal("id", tick.Ticker);
        Assert.Equal(1.0m, tick.Volume);
        Assert.Equal(2.0m, tick.Price);
    }
    
    [Fact]
    public void AliceAdapterConverter_ShouldThrow()
    {
        // Arrange
        var aliceDto = new AliceSourceDto
        {
            Id = "id",
            Price = "abc",
            Volume = "def"
        };

        // Act
        // Assert
        Assert.Throws<DomainConverterException>(() => _aliceConverter.ToDomainModel(aliceDto));
    }
    
    [Fact]
    public void AliceAdapterConverter_ShouldThrowIfNull()
    {
        // Arrange
        // Act
        // Assert
        Assert.Throws<DomainConverterException>(() => _aliceConverter.ToDomainModel(null!));
    }
    
    [Fact]
    public void BobAdapterConverter_ShouldSuccess()
    {
        // Arrange
        var dto = new BobSourceDto
        {
            Ticker = "id",
            TotalPrice = 10.0m,
            Count = 5
        };

        // Act
        var tick = _bobConverter.ToDomainModel(dto);
        // Assert
        Assert.NotNull(tick);
        Assert.Equal("id", tick.Ticker);
        Assert.Equal(50.0m, tick.Volume);
        Assert.Equal(10.0m, tick.Price);
    }
    
    [Fact]
    public void BobAdapterConverter_ShouldThrow()
    {
        // Arrange
        var dto = new BobSourceDto
        {
            Ticker = null,
            TotalPrice = 10,
            Count = 5
        };

        // Act
        // Assert
        Assert.Throws<DomainConverterException>(() => _bobConverter.ToDomainModel(dto));
    }
    
    [Fact]
    public void BobAdapterConverter_ShouldThrowIfNull()
    {
        // Arrange
        // Act
        // Assert
        Assert.Throws<DomainConverterException>(() => _bobConverter.ToDomainModel(null!));
    }
    
    [Fact]
    public void ChloeAdapterConverter_ShouldSuccess()
    {
        // Arrange
        var dto = new ChloeSourceDto
        {
            Ticker = "id",
            Price = 10.0m,
            Volume = 50.0m,
            Timestamp = DateTimeOffset.UtcNow
        };

        // Act
        var tick = _chloeConverter.ToDomainModel(dto);
        // Assert
        Assert.NotNull(tick);
        Assert.Equal("id", tick.Ticker);
        Assert.Equal(50.0m, tick.Volume);
        Assert.Equal(10.0m, tick.Price);
    }
    
    [Fact]
    public void ChloeAdapterConverter_ShouldThrow()
    {
        // Arrange
        var dto = new ChloeSourceDto
        {
            Ticker = "id",
            Price = 10.0m,
            Volume = 50.0m,
            Timestamp = default
        };

        // Act
        // Assert
        Assert.Throws<DomainConverterException>(() => _chloeConverter.ToDomainModel(dto));
    }
    
    [Fact]
    public void ChloeAdapterConverter_ShouldThrowIfNull()
    {
        // Arrange
        // Act
        // Assert
        Assert.Throws<DomainConverterException>(() => _chloeConverter.ToDomainModel(null!));
    }
    
}