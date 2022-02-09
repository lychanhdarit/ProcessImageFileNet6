using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using System.IO;
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
            string folderPath = Path.Combine(_hostingEnvironment.WebRootPath, "uploads");
            var contents = new List<string>();
            foreach (string file in Directory.EnumerateFiles(folderPath, "*"))
            {
                var fileName = file.Split('\\');
                contents.Add(fileName[fileName.Length - 1]);
            }
            ViewBag.Files = contents;
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

        //[ResponseCache(Duration = 3600 * 24 * 90, VaryByQueryKeys = new[] { "*" })]
        public ActionResult Webp(string f)
        {
            try
            {
                Byte[] b;
                var path = Path.Combine(_hostingEnvironment.WebRootPath, f);
                //--------------------------------------------------
                Image image = Image.FromFile(path);
                Graphics drawing = Graphics.FromImage(image);

                //-------------------------------
                //WebClient wc = new WebClient();
                //byte[] bytes = wc.DownloadData("https://vietnamhome.vn/uploads/website/202201/fce42e9c-a244-4b4e-a711-33fc3082cf11-logo.png");
                //MemoryStream ms = new MemoryStream(bytes); 
                //Image imgLink = Image.FromStream(ms);
                //-------------------------------
                path = Path.Combine(_hostingEnvironment.WebRootPath, "logo/logo-vn.png");
                int? width = 600; int? height = 0;
                if (width > image.Width)
                {
                    width = image.Width - 50;
                }
                Image imgLink = ResizeImage(path, width, height, "sw");// Image.FromFile(path); 


                drawing.DrawImage(imgLink, (image.Width / 2) - (imgLink.Width / 2), (image.Height / 2) - (imgLink.Height / 2));
                drawing.Save();
                drawing.Dispose();

                imgLink.Dispose();
                b = ImageToByteArray(image);
                return File(b, "image/webp");
            }
            catch
            {
                Image image = ImageEmpty("no Image", 200, 100);
                var b = ImageToByteArray(image);
                return File(b, "image/webp");
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

        public Image ResizeImage(string path, int? width, int? height, string scaleType)
        {
            try
            {
                Image imgPath = new Bitmap(path);
                int setWidth = width ?? imgPath.Width;
                int setHeight = height ?? imgPath.Height;
                switch (scaleType)
                {
                    case "sw":
                        setHeight = setWidth * imgPath.Height / imgPath.Width;
                        break;
                    case "sh":
                        setWidth = setHeight * imgPath.Width / imgPath.Height;
                        break;
                }

                Image img = new Bitmap(imgPath, setWidth, setHeight);
                Graphics drawing = Graphics.FromImage(img);
                drawing.Save();
                drawing.Dispose();
                return img;
            }
            catch (Exception e)
            {

                return ImageEmpty(e.Message, width, height);
            }
        }
        public Image ImageEmpty(string Message, int? width, int? height)
        {
            int setWidth = width ?? 200;
            var minus = (height ?? setWidth) * 0.4;
            int setHeight = (height ?? setWidth) - int.Parse(minus.ToString());
            Image img = DrawText(Message, new Font(FontFamily.GenericSansSerif, 15), Color.Gray, Color.WhiteSmoke, setWidth, setHeight);
            Graphics drawing = Graphics.FromImage(img);
            drawing.Save();
            drawing.Dispose();
            return img;
        }
        public Image DrawTextSizeText(String text, Font font, Color textColor, Color backColor)
        {
            //first, create a dummy bitmap just to get a graphics object  
            Image img = new Bitmap(100, 100);
            Graphics drawing = Graphics.FromImage(img);

            //measure the string to see how big the image needs to be  
            Graphics drawingTemp = Graphics.FromImage(img);
            SizeF textSize = drawingTemp.MeasureString(text, font);
            //free up the dummy image and old graphics object  
            img.Dispose();
            drawing.Dispose();
            int padding = 0;
            //create a new image of the right size
            int width = int.Parse((Math.Round(textSize.Width, MidpointRounding.ToEven) + padding * 2).ToString());
            var height = int.Parse((Math.Round(textSize.Height, MidpointRounding.ToEven) + padding * 2).ToString());
            img = new Bitmap(width, height);
            drawing = Graphics.FromImage(img);

            //paint the background
            drawing.Clear(backColor);
            //create a brush for the text  
            Brush textBrush = new SolidBrush(textColor);
            var tSizeWidth = textSize.Width;
            var tSizeHeight = textSize.Height;
            float y = padding;
            float x = padding;
            drawing.DrawString(text, font, textBrush, x, y);
            drawing.Save();
            textBrush.Dispose();
            drawing.Dispose();
            return img;

        }

        public Image DrawText(String text, Font font, Color textColor, Color backColor, int width, int height)
        {
            //first, create a dummy bitmap just to get a graphics object   
            Image img = new Bitmap(width, height);
            Graphics drawing = Graphics.FromImage(img);

            //measure the string to see how big the image needs to be  

            SizeF textSize = drawing.MeasureString(text, font);
            //free up the dummy image and old graphics object  
            img.Dispose();
            drawing.Dispose();

            //create a new image of the right size
            img = new Bitmap(width, height);

            drawing = Graphics.FromImage(img);

            //paint the background
            drawing.Clear(backColor);

            //create a brush for the text  
            Brush textBrush = new SolidBrush(textColor);
            var tSizeWidth = textSize.Width;
            var tSizeHeight = textSize.Height;
            float y = ((height - tSizeHeight) / 2);
            float x = ((width - tSizeWidth) / 2);
            drawing.DrawString(text, font, textBrush, x, y);

            drawing.Save();

            textBrush.Dispose();
            drawing.Dispose();

            return img;

        }
        public byte[] ImageToByteArray(System.Drawing.Image imageIn)
        {
            MemoryStream ms = new MemoryStream();
            imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            return ms.ToArray();
        }

    }
}
