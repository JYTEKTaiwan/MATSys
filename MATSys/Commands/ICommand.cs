namespace MATSys.Commands
{
    
    public interface ICommand
    {
        string MethodName { get; set; }
        string? ConvertResultToString(object obj);

        object[]? GetParameters();

        string SimplifiedString();
    }
}