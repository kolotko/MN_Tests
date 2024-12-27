using CodeCoverage.ClassLibrary;
using Xunit;

namespace CodeCoverage.Tests;

public class CalculatorTests
{
    private readonly Calculator _calculator;

    public CalculatorTests()
    {
        _calculator = new Calculator();
    }

    [Fact]
    public void Sum_ShouldReturnCorrectResult()
    {
        // Arrange
        int a = 5;
        int b = 3;

        // Act
        int result = _calculator.Sum(a, b);

        // Assert
        Assert.Equal(8, result);
    }

    [Fact]
    public void Sub_ShouldReturnCorrectResult()
    {
        // Arrange
        int a = 5;
        int b = 3;

        // Act
        int result = _calculator.Sub(a, b);

        // Assert
        Assert.Equal(2, result);
    }

    [Fact]
    public void Mult_ShouldReturnCorrectResult()
    {
        // Arrange
        int a = 5;
        int b = 3;

        // Act
        int result = _calculator.Mult(a, b);

        // Assert
        Assert.Equal(15, result);
    }

    [Fact]
    public void Div_ShouldReturnCorrectResult()
    {
        // Arrange
        int a = 6;
        int b = 3;

        // Act
        int result = _calculator.Div(a, b);

        // Assert
        Assert.Equal(2, result);
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