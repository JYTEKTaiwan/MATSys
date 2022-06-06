namespace MATSys.Commands
{
    public interface ICommand
    {
        string MethodName { get; set; }

        //object Execute(IDevice instance);
        string? ConvertResultToString(object obj);

        object[] GetParameters();

        string SimplifiedString();
    }
}