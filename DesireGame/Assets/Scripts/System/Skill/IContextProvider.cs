using Client;

public interface IContextProvider
{
    InputParameter InputParameter { get; }
    BuffParameter BuffParameter { get; }
}