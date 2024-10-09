using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Context;
using WebApplication1.Models;
using Newtonsoft.Json;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LostAndFoundController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly string _picturesFolder = "pictures";
        private static int _imageCounter = 1;

        public LostAndFoundController(ApplicationDbContext context)
        {
            _context = context;

            if (!Directory.Exists(_picturesFolder))
            {
                Directory.CreateDirectory(_picturesFolder);
            }
        }

        // GET: api/lostandfound/lost
        [HttpGet("lost")]
        public async Task<ActionResult<IEnumerable<LostAndFoundModels>>> GetLostItems()
        {
            // 获取所有 "lost" 类型的失物
            var lostItems = await _context.LostAndFoundItems.Where(item => item.LostOrFound == "lost").ToListAsync();

            if (lostItems == null || !lostItems.Any())
            {
                return NotFound(); // 如果没有找到任何信息，返回404
            }

            // 转码图片并返回
            foreach (var item in lostItems)
            {
                if (!string.IsNullOrEmpty(item.image))
                {
                    var imagePath = Path.Combine(_picturesFolder, Path.GetFileName(item.image));
                    if (System.IO.File.Exists(imagePath))
                    {
                        byte[] imageBytes = await System.IO.File.ReadAllBytesAsync(imagePath);
                        item.image = $"data:image/jpeg;base64,{Convert.ToBase64String(imageBytes)}";
                    }
                }
            }

            return Ok(lostItems); // 返回200和lost物品列表
        }

        // GET: api/lostandfound/found
        [HttpGet("found")]
        public async Task<ActionResult<IEnumerable<LostAndFoundModels>>> GetFoundItems()
        {
            // 获取所有 "found" 类型的招领
            var foundItems = await _context.LostAndFoundItems.Where(item => item.LostOrFound == "found").ToListAsync();

            if (foundItems == null || !foundItems.Any())
            {
                return NotFound(); // 如果没有找到任何信息，返回404
            }

            // 转码图片并返回
            foreach (var item in foundItems)
            {
                if (!string.IsNullOrEmpty(item.image))
                {
                    var imagePath = Path.Combine(_picturesFolder, Path.GetFileName(item.image));
                    if (System.IO.File.Exists(imagePath))
                    {
                        byte[] imageBytes = await System.IO.File.ReadAllBytesAsync(imagePath);
                        item.image = $"data:image/jpeg;base64,{Convert.ToBase64String(imageBytes)}";
                    }
                }
            }

            return Ok(foundItems); // 返回200和found物品列表
        }

        // POST: api/lostandfound
        [HttpPost]
        public async Task<IActionResult> PostLostAndFound([FromForm] IFormFile image, [FromForm] string lostAndFoundJson)
        {
            if (image == null || string.IsNullOrEmpty(lostAndFoundJson))
            {
                return BadRequest("Missing image or lostAndFound data");
            }

            LostAndFoundModels lostAndFound;
            try
            {
                lostAndFound = JsonConvert.DeserializeObject<LostAndFoundModels>(lostAndFoundJson);
            }
            catch (Newtonsoft.Json.JsonException ex)
            {
                return BadRequest("Invalid lostAndFound JSON data");
            }

            // 生成唯一的文件名
            string fileName = await GenerateUniqueFileName(image.FileName);
            string filePath = Path.Combine(_picturesFolder, fileName);

            // 保存图片
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }

            // 更新 LostAndFoundModels 对象
            lostAndFound.image = filePath;

            _context.LostAndFoundItems.Add(lostAndFound);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Lost and found item created successfully" });
        }

        private async Task<string> GenerateUniqueFileName(string originalFileName)
        {
            string extension = Path.GetExtension(originalFileName);
            string newFileName;
            do
            {
                newFileName = $"{_imageCounter:D5}{extension}";
                _imageCounter++;
            } while (await _context.LostAndFoundItems.AnyAsync(l => l.image == Path.Combine(_picturesFolder, newFileName)));

            return newFileName;
        }

        // PUT: api/lostandfound/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLostAndFound(int id, LostAndFoundModels lostAndFound)
        {
            if (id != lostAndFound.Id)
            {
                return BadRequest();
            }

            _context.Entry(lostAndFound).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LostAndFoundExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/lostandfound/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLostAndFound(int id)
        {
            var lostAndFound = await _context.LostAndFoundItems.FindAsync(id);
            if (lostAndFound == null)
            {
                return NotFound();
            }

            _context.LostAndFoundItems.Remove(lostAndFound);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool LostAndFoundExists(int id)
        {
            return _context.LostAndFoundItems.Any(e => e.Id == id);
        }
    }
}
