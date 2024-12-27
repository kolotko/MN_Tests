using MutationTests.ClassLibrary;
using Xunit;

namespace MutationTests.Tests;

public class CalculatorTests
{
    private readonly Calculator _calculator;

    public CalculatorTests()
    {
        _calculator = new Calculator();
    }

    [Theory]
    [InlineData(5, 5, 10)]
    public void Sum_ShouldReturnCorrectResult(int a, int b, int expected)
    {
        // Act
        int result = _calculator.Sum(a, b);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(1, 1, 0)]
    public void Sub_ShouldReturnCorrectResult(int a, int b, int expected)
    {
        // Act
        int result = _calculator.Sub(a, b);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(1, 1, 1)]
    public void Mult_ShouldReturnCorrectResult(int a, int b, int expected)
    {
        // Act
        int result = _calculator.Mult(a, b);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(1, 1, 1)]
    public void Div_ShouldReturnCorrectResult(int a, int b, int expected)
    {
        // Act
        int result = _calculator.Div(a, b);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Div_ShouldThrowDivideByZeroException_WhenDividingByZero()
    {
        // Arrange
        int a = 6;
        int b = 0;

        // Act & Assert
        Assert.Throws<DivideByZeroException>(() => _calculator.Div(a, b));
    }
}