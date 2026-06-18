using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Threading.Tasks;
using System;
using Techniki_Internetowe_Proj.ViewModels;

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
        public async Task<IActionResult> UploadImage(RecipeCreateViewModel model)
        {
            // 1. Backendowa tarcza ochronna
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IFormFile imageFile = model.ImageFile;

            // 2. Sprawdzenie czy plik istnieje
            if (imageFile != null && imageFile.Length > 0)
            {
                // Generowanie unikalnej nazwy i przygotowanie ścieżki zapisu w wwwroot/images
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                var uploadsFolder = Path.Combine(_env.WebRootPath, "images");

                // Upewnij się, że folder images istnieje
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var filePath = Path.Combine(uploadsFolder, fileName);

                // Asynchroniczny zapis pliku na serwerze
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                // Zwracamy ścieżkę do zapisanego pliku
                return Ok(new { imageUrl = "/images/" + fileName });
            }

            return BadRequest("Nie wybrano pliku lub plik jest pusty.");
        }
    }
}