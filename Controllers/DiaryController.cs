using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DearDiaryLogs.Models;

namespace DearDiaryLogs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiaryController : ControllerBase
    {
        private readonly DearDiaryLogsContext _context;

        public DiaryController(DearDiaryLogsContext context)
        {
            _context = context;
        }

        // GET: api/Diary
        [HttpGet]
        public IEnumerable<DiaryLog> GetDiaryLog()
        {
            return _context.DiaryLog;
        }

        // GET: api/Diary/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDiaryLog([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var diaryLog = await _context.DiaryLog.FindAsync(id);

            if (diaryLog == null)
            {
                return NotFound();
            }

            return Ok(diaryLog);
        }

        // PUT: api/Diary/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDiaryLog([FromRoute] int id, [FromBody] DiaryLog diaryLog)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != diaryLog.Id)
            {
                return BadRequest();
            }

            _context.Entry(diaryLog).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DiaryLogExists(id))
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

        // POST: api/Diary
        [HttpPost]
        public async Task<IActionResult> PostDiaryLog([FromBody] DiaryLog diaryLog)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.DiaryLog.Add(diaryLog);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDiaryLog", new { id = diaryLog.Id }, diaryLog);
        }

        // DELETE: api/Diary/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDiaryLog([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var diaryLog = await _context.DiaryLog.FindAsync(id);
            if (diaryLog == null)
            {
                return NotFound();
            }

            _context.DiaryLog.Remove(diaryLog);
            await _context.SaveChangesAsync();

            return Ok(diaryLog);
        }

        private bool DiaryLogExists(int id)
        {
            return _context.DiaryLog.Any(e => e.Id == id);
        }
    }
}