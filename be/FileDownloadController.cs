using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.IO.Compression;

[Route("api/[controller]")]
[ApiController]
[EnableCors("AllowAll")] // Allow CORS
public class FileDownloadController : ControllerBase
{
    // Endpoint to zip and download the folder
    [HttpGet("download-zip")]
    public IActionResult DownloadZippedFolder()
    {
        string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "crud"); // Folder to be zipped
        string zipFilePath = Path.Combine(Path.GetTempPath(), "crud.zip"); // Temporary path to store the zip file

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath); // Ensure the folder exists
            // Add placeholder content if needed, e.g., create a dummy file
            System.IO.File.WriteAllText(Path.Combine(folderPath, "placeholder.txt"), "This is a placeholder file.");
        }

        if (System.IO.File.Exists(zipFilePath))
        {
            System.IO.File.Delete(zipFilePath); // Delete existing zip if any
        }

        ZipFile.CreateFromDirectory(folderPath, zipFilePath); // Create zip file from folder

        byte[] fileBytes = System.IO.File.ReadAllBytes(zipFilePath); // Read zip as byte array
        System.IO.File.Delete(zipFilePath); // Optionally delete the zip file after sending

        return File(fileBytes, "application/zip", "Crud.zip"); // Return zip file for download
    }

    // Endpoint to upload a file
    [HttpPost("upload")]
    public IActionResult UploadFiles(List<IFormFile> files)
    {
        if (files == null || files.Count == 0)
        {
            return BadRequest("No files uploaded.");
        }

        try
        {
            string uploadFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads");

            if (!Directory.Exists(uploadFolderPath))
            {
                Directory.CreateDirectory(uploadFolderPath); // Ensure uploads folder exists
            }

            foreach (var file in files)
            {
                var filePath = Path.Combine(uploadFolderPath, file.FileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(stream);
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
