using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Threading.Tasks;
using System;

namespace Techniki_Internetowe_Proj.Controllers
{
    public class RecipeController : Controller
    {
        private readonly IWebHostEnvironment _env;

        public RecipeController(IWebHostEnvironment env)
        {
            _env = env;
        }

        // Obsługa uploadu zdjęcia dla nowego przepisu
        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile imageFile)
        {
            // Walidacja obecności pliku
            if (imageFile == null || imageFile.Length == 0)
            {
                return BadRequest("Nie wybrano pliku.");
            }

            // Generowanie unikalnej nazwy i przygotowanie ścieżki zapisu w wwwroot/images
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
            var uploadsFolder = Path.Combine(_env.WebRootPath, "images");
            var filePath = Path.Combine(uploadsFolder, fileName);

            // Asynchroniczny zapis pliku na serwerze
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            // Zwracamy ścieżkę do zapisanego pliku, żeby frontend mógł jej użyć
            return Ok(new { imageUrl = "/images/" + fileName });
        }
    }
}