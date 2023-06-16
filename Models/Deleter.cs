using System.Transactions;

namespace FileShare.Models
{
    public class Deleter
    {
        private readonly DatabaseContext _context;

        public Deleter(DatabaseContext context)
        {
            _context = context;
        }
        public async Task DeleteFileAsync(CustomFile fileToDelete)
        {
            using (var scope = new TransactionScope())
            {
                try
                {
                    File.Delete(fileToDelete.Path);
                    _context.Files.Remove(fileToDelete);
                    await _context.SaveChangesAsync();
                    scope.Complete();
                }
                catch (Exception)
                {
                    scope.Dispose();
                }
            }
        }

        public void DeleteObsoleteData()
        {
            var obsoleteData = _context.Files.Where(file => file.CreatedAt < file.DelTime);
            obsoleteData.Select(file => DeleteFileAsync(file));
        }
    }
}