using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dotnet_rpg.Dtos.Fight;
using dotnet_rpg.Models;
using dotnet_rpg.Services.FightService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_rpg.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class FightController : ControllerBase   
    {
        private readonly IFightService _fightService;

        public FightController(IFightService fightService)
        {
            _fightService = fightService;
        }
        [HttpPost("WeaponAttack")]
        public async Task<ActionResult<ServiceResponse<AttackResultDto>>> WeaponAttack(WeaponAttackDto request)
        {
            return Ok(await _fightService.WeaponAttackAsync(request));
        }
        [HttpPost("SkillAttack")]
        public async Task<ActionResult<ServiceResponse<AttackResultDto>>> SkillAttack(SkillAttackDto request)
        {
            return Ok(await _fightService.SkillAttackAsync(request));
        }
        [HttpPost("Fight")]
        public async Task<ActionResult<ServiceResponse<FightResultDto>>> Fight(FightRequestDto request)
        {
            return Ok(await _fightService.FightAsync(request));
        }
        [HttpGet("GetHighScore")]
        public async Task<ActionResult<ServiceResponse<List<HighScoreDto>>>> GetHighScore()
        {
            return Ok(await _fightService.GetHighScoreAsync());
        }
    }
}