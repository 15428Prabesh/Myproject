using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class MediaHelper
    {
        public static string NewFileName(string fileName, string strBaseLocation)
        {
            string fileLocation = Path.Combine(strBaseLocation, fileName);
            if (File.Exists(fileLocation))
            {
                string fileNameOnly = Path.GetFileNameWithoutExtension(fileName);
                string fileExtension = Path.GetExtension(fileName);
                fileName = RenameFile(fileNameOnly);
                fileName = fileName + fileExtension;
                fileLocation = Path.Combine(strBaseLocation, fileName);
                if (File.Exists(fileLocation))
                {
                    fileName = NewFileName(fileName, strBaseLocation);
                }
            }
            return fileName;
        }
        public static string RenameFile(string fileName)
        {
            string[] names = fileName.Split('_');
            int count = 0;
            int length = names.Length;
            if (length > 1)
            {
                int.TryParse(names[length - 1], out count);
                count++;
                names[length - 1] = count.ToString();
                fileName = string.Join("_", names);
            }
            else
            {
                count++;
                fileName = fileName + "_" + count;
            }

            return fileName;
        }
    }
}
