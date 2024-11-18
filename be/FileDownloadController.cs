using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.IO.Compression;

namespace FileUploadDownloadApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowAll")] // Enable CORS
    public class FileDownloadController : ControllerBase
    {
        // Endpoint to zip and download the folder
        [HttpGet("download-zip")]
        public IActionResult DownloadZippedFolder()
        {
            string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "crud"); // Path to the crud folder
            string zipFilePath = Path.Combine(Path.GetTempPath(), "crud.zip");  // Temporary path to store the zip file

            if (!Directory.Exists(folderPath))
            {
                return NotFound("The specified folder does not exist.");
            }

            // Create zip file
            if (System.IO.File.Exists(zipFilePath))
            {
                System.IO.File.Delete(zipFilePath); // Delete existing zip if it exists
            }

            ZipFile.CreateFromDirectory(folderPath, zipFilePath); // Create zip from folder

            // Read the zip file as bytes
            byte[] fileBytes = System.IO.File.ReadAllBytes(zipFilePath);

            // Optionally delete the zip file after reading it
            System.IO.File.Delete(zipFilePath);

            // Return the zip file for download
            return File(fileBytes, "application/zip", "Crud.zip");
        }

        // Endpoint to upload files to the crud folder
        [HttpPost("upload")]
        public IActionResult UploadFiles(List<IFormFile> files)
        {
            if (files == null || files.Count == 0)
            {
                return BadRequest("No files uploaded.");
            }

            try
            {
                string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "crud"); // Path to crud folder

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath); // Create crud folder if it doesn't exist
                }

                foreach (var file in files)
                {
                    string filePath = Path.Combine(folderPath, file.FileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(stream); // Save the file
                    }
                }

                return Ok("Files uploaded successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while uploading the files: " + ex.Message);
            }
        }
    }
}
