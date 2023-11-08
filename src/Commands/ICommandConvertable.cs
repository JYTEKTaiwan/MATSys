namespace MATSys.Commands;
public interface ICommandConvertable
{
    MATSys.Commands.ICommand ConvertToCommand();
}