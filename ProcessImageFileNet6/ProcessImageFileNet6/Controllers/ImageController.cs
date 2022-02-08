using Microsoft.AspNetCore.Mvc;

namespace ProcessImageFileNet6.Controllers
{
    public class ImageController : Controller
    {
        private IWebHostEnvironment _hostingEnvironment;
        public ImageController(IWebHostEnvironment environment)
        {
            _hostingEnvironment = environment;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> UploadFile(List<IFormFile> files)
        {
            try
            {
                string folderUpload = "uploads/";
                string filename = "";
                string uploadPath = Path.Combine(_hostingEnvironment.WebRootPath, folderUpload);
                if (files != null)
                {
                    foreach (var file in files)
                    {
                        if (file != null)
                        {
                            filename += file.FileName + " | ";

                            string fileNameResult = SaveFile(file, uploadPath);
                        }
                    }
                }
                return Json(new { msg = filename, code = 1 });
            }
            catch (Exception ex)
            {
                return Json(new { msg = ex.Message, code = 1 });
            }
        }
        public string SaveFile(IFormFile file, string uploadPath)
        {
            if (file != null)
            {
                string filenameNew = Guid.NewGuid() + "-" + file.FileName;
                string filePath = Path.Combine(uploadPath, filenameNew);
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                } 
                var stream = new FileStream(filePath, FileMode.Create);
                file.CopyTo(stream);
                stream.Close();
                return filenameNew;
            }
            return "";
        }
    }
}
