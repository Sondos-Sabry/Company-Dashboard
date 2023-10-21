using Demo.BLL.Repositories;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;

namespace Demo.PL.Helpers
{
    public class DocumentSettings
    {
        // file on server as static file in wwwroot
        // save in database path of file on server
        //path on my pc => D:\.Net Projects\Asp .net mvc\Company Solution\Demo.PL\wwwroot\Files\Images\
        public static  string UploadFile(IFormFile file , string folderName)//IFormFile => file upload from html form 
        {
            
            // 1. Get located folder path 
            //string folderPath = Directory.GetCurrentDirectory() +@"\wwwroot\Files\" + folderName;
            string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Files", folderName);

            // 2. Get File Name and make it Unique
            //   string fileName = $"{Guid.NewGuid()}{file.FileName}"; 
            string fileName = $"{Guid.NewGuid()}{file.FileName}";

            //3. Get file path
            string filePath = Path.Combine(folderPath, fileName);

            // 4. Save my file in file path => As Stream {data per time}

           using var fileStream = new FileStream( filePath , FileMode.Create);

           file.CopyTo(fileStream); //copy the content of uploaded file to target stream

            return fileName; //not file path .. لانه هيتكرر مع الكل 

            
        }
   
        public static void DeleteFile( string fileName , string folderName)
        {
            if(fileName != null && folderName != null)
            {
                 string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Files" , folderName , fileName);
                if (File.Exists(filePath))
                  File.Delete(filePath);
            }

           

        }
    
    
    }


}
