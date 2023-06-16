namespace FileShare.Models
{
    public class CustomFile
    {
        public Guid Id { get; set; }
        public string Path { get; set; } = null!;
        public byte[] Salt { get; set; } = null!;
        public byte[] PasswordToDel { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime DelTime { get; set; }
    }

    public class FilePublicViewDto
    {
        public Guid Id { get; set; }
        public string Path { get; set; } = null!;
    }

    public class FileDeleteDto
    {
        public Guid Id { get; set; }
        public string PasswordToDel { get; set; } = null!;
    }

}