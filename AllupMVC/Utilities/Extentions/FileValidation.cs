using AllupMVC.Models;
using AllupMVC.Utilities.Enums;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.ComponentModel;
using System.Diagnostics;

namespace AllupMVC.Utilities.Extentions
{
    public static class FileValidation
    {

        public  static bool ValidateType(this IFormFile file,string type)
        {
            if (file.ContentType.Contains(type))
            {
                return true;
            }
            return false;   
        }

        public static bool ValidateSize(this IFormFile file,Filesize type,int size)
        {
            switch(type) 
            {
             case Filesize.KB:
                    return file.Length <= size * 1024;
             case Filesize.MG:
                    return file.Length <= size * 1024 * 1024;

            }
          return false;
        }

        public static string CreatePath(this string file,params string[] roots)
        {

            string path = string.Empty;
            for (int i = 0; i < roots.Length; i++)
            {
                path = Path.Combine(path, roots[i]);
            }
            path = Path.Combine(path, file);
            return path;
        }
        public static async Task<string> CreateFile(this IFormFile file,params string[] roots)
        {

            string filename = string.Concat(Guid.NewGuid().ToString(), file.FileName);
          

           using (FileStream fileStream = new(filename.CreatePath(roots), FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

          return filename;

           
        }

        public static void Delete(this string file, params string[] roots)
        {
            File.Delete(file.CreatePath(roots));
        }
    }
}
