namespace User.Identity.Helpers;

public class AppSettings
{
    public string Secret { get; set; }

    // refresh token time to live (in days), inactive tokens are
    // automatically deleted from the database after this time
    public int RefreshTokenTTL { get; set; }
    public int JwtTokenTTL { get; set; }
    public string SmtpHost { get; set; }
    public int SmtpPort { get; set; }
    public string SmtpUsername { get; set; }
    public string SmtpPassword { get; set; }
    public string SenderEmail { get; set; }
    public string SenderName { get; set; }
    public string CcTo { get; set; }
    public string BccTo { get; set; }

}