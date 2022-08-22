using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dotnet_rpg.Dtos.Fight;
using dotnet_rpg.Models;

namespace dotnet_rpg.Services.FightService
{
    public interface IFightService
    {
        Task<ServiceResponse<AttackResultDto>> WeaponAttackAsync(WeaponAttackDto request);
        Task<ServiceResponse<AttackResultDto>> SkillAttackAsync(SkillAttackDto request);
        Task<ServiceResponse<FightResultDto>> FightAsync(FightRequestDto request);
        Task<ServiceResponse<List<HighScoreDto>>> GetHighScoreAsync();
    }
}