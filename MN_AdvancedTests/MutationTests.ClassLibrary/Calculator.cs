namespace MutationTests.ClassLibrary;

public class Calculator
{
    public int Sum(int a, int b) => a + b;

    public int Sub(int a, int b) => a - b;

    public int Mult(int a, int b) => a * b;

    public int Div(int a, int b)
    {
        if (b == 0)
            throw new DivideByZeroException("Division by zero is not allowed.");
        return a / b;
    }
}