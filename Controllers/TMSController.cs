using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMSApp.Infrastructure.Repository;
using TMSApp.Application.DTO;
using System.IO;

namespace TMSApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TMSController : ControllerBase
    {
        private readonly ITMSQueryRepository _tmsRepository;

        public TMSController(ITMSQueryRepository tmsRepository)
        {

            _tmsRepository = tmsRepository;

        }
        [HttpGet]
        public async Task<IEnumerable<TMSDto>> GetTMs()
        {
            return await _tmsRepository.Get();

        }
        [HttpGet("{id}")]
        public async Task<ActionResult<TMSDto>> GetTms(int id)
        {

            return await _tmsRepository.Get(id);
        }

        [HttpPost]
        public async Task<ActionResult<TMSDto>> PostTMS([FromBody] TMSDto tMSDto)
        {

            var newTms = await _tmsRepository.Create(tMSDto);
            return CreatedAtAction(nameof(GetTms), new { id = newTms.TaskID }, newTms);

        }
        [HttpPut]
        public async Task<ActionResult> PutTMS(int id, [FromBody] TMSDto tMSDto)
        {

            if (id != tMSDto.TaskID)
            {

                return BadRequest();
            }
            await _tmsRepository.Update(tMSDto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var taskDelete = await _tmsRepository.Get(id);
            if (taskDelete == null)
                return NotFound();

            await _tmsRepository.Delete(taskDelete.TaskID);
            return NotFound();
        
        }

        [HttpPost]
        [Route("documents/upload")]
        public async Task<IActionResult> OnPostInsertCodeAsync(IFormFile upload)
        {

            TMSDto tMSDto = new TMSDto();
            if (upload.FileName.EndsWith(".csv"))
            {
                using (var sreader = new StreamReader(upload.OpenReadStream()))
                {
                   
                    string[] headers = sreader.ReadLine().Split(',');     //Title
                    while (!sreader.EndOfStream)                          //get all the content in rows 
                    {
                        string[] rows = sreader.ReadLine().Split(',');
                        tMSDto.TaskID = int.Parse(rows[0].ToString());
                        tMSDto.TaskName = rows[1].ToString();
                        tMSDto.Assignee = rows[2].ToString();
                        tMSDto.TimeSpent = float .Parse(rows[3].ToString());
                        tMSDto.TaskGroup = int.Parse(rows[4].ToString());
                        tMSDto.TaskStatus = int.Parse(rows[5].ToString());
                        var newTms = await _tmsRepository.Create(tMSDto);
                    }
                }
            }

            //var newTms = await _tmsRepository.Create(tMSDto);
            //return CreatedAtAction(nameof(GetTms), new { id = newTms.TaskID }, newTms);
            return NoContent();
        }

    }
}
