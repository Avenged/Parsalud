namespace Parsalud.Client;

public interface IModelBase<out TSelf, out TRequest, in TDto>
    where TRequest : class
{
    TSelf FromDto(TDto dto);
    TRequest ToRequest();
}