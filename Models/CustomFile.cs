namespace FileShare.Models
{
    public class CustomFile
    {
        public Guid Id { get; set; }
        public string Path { get; set; } = null!;
        public string PasswordToDel { get; set; } = null!;
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