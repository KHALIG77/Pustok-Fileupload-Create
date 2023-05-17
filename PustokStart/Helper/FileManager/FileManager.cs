using PustokStart.Models;
using System.IO;

namespace PustokStart.Helper.FileManager
{
    public static class FileManager
    {
        public static string Save(string rootPath,string folder,IFormFile file)
        {
            string newPath=(Guid.NewGuid().ToString()+file.FileName);
            newPath = newPath.Substring(newPath.Length - 20);
            string path = Path.Combine(rootPath,folder, newPath);
            using (FileStream stream = new FileStream(path, FileMode.Create))
            {
                file.CopyTo(stream);
            }
            return newPath;
        }
    }
}
