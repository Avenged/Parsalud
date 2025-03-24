namespace Parsalud;

public class EmailSettings
{
    public string HostName { get; set; } = string.Empty;
    public string Host { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string EmailFrom { get; set; } = string.Empty;
    public string EmailTo { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public int Port { get; set; }
    public bool EnableSsl { get; set; }
    public bool UseDefaultCredentials { get; set; }
    public string Template { get; set; } = string.Empty;
}
