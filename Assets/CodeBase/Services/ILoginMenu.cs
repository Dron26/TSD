namespace CodeBase.Services
{
    public interface ILoginMenu: IService
    {
        bool CheckCredentials(string username, string password);
        void DisplayWarning(string message);
    }
}