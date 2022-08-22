using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using dotnet_rpg.Data;
using dotnet_rpg.Dtos.Fight;
using dotnet_rpg.Models;
using Microsoft.EntityFrameworkCore;

namespace dotnet_rpg.Services.FightService
{
    public class FightService : IFightService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public FightService(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<FightResultDto>> FightAsync(FightRequestDto request)
        {
            var response = new ServiceResponse<FightResultDto>()
            {
                Data = new FightResultDto()
            };
            try
            {
                var characters = await _context.Characters
                    .Include(c => c.Weapon)
                    .Include(c => c.Skills)
                    .Where(c => request.CharacterIds.Contains(c.Id)).ToListAsync();
                
                bool defeated = false;
                while(!defeated)
                {
                    foreach (Character attacker in characters)
                    {
                        var opponents = characters.Where(c => c.Id != attacker.Id).ToList();
                        var opponent = opponents[new Random().Next(opponents.Count)];

                        int damage = 0;
                        string attackUsed = string.Empty;

                        bool userWeapon = new Random().Next(2) == 0;
                        if(userWeapon)
                        {
                            attackUsed = attacker.Weapon.Name;
                            damage = DoWeaponAttack(attacker, opponent);
                        }
                        else
                        {
                            var skillUsed = attacker.Skills[new Random().Next(attacker.Skills.Count)];
                            attackUsed = skillUsed.Name;
                            damage = DoSkillAttack(attacker, opponent, skillUsed);
                        }
                        if(damage < 0)
                        {
                            damage = 0;
                        }
                        response.Data.Log
                            .Add($"{attacker.Name} attacked {opponent.Name}\n Attack used = {attackUsed} \n Damage = {damage}");
                        if(opponent.HitPoints <= 0)
                        {
                            defeated = true;
                            attacker.Victories++;
                            opponent.Defeats++;
                            response.Data.Log
                            .Add($"{attacker.Name} killed {opponent.Name}\n remaining HP= {attacker.HitPoints}");
                            break;
                        }
                    }
                }
                characters.ForEach(c => 
                {
                    c.Fights++;
                    
                    if (c.Id == 3)
                    {
                        c.HitPoints = 10000000;
                    }
                    else
                    {
                        c.HitPoints = 200;
                    }
                });
                await _context.SaveChangesAsync();
            }
            catch (System.Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<AttackResultDto>> SkillAttackAsync(SkillAttackDto request)
        {
            var response = new ServiceResponse<AttackResultDto>();
            try
            {
                var attacker = await _context.Characters
                    .Include(c => c.Skills)
                    .FirstOrDefaultAsync(c => c.Id == request.AttackerId);
                var opponent = await _context.Characters
                    .FirstOrDefaultAsync(c => c.Id == request.OpponentId);

                var skill = attacker.Skills.FirstOrDefault(s => s.Id == request.SkillId);
                if (skill is null)
                {
                    response.Success = false;
                    response.Message = "Skill not found!";
                    return response;
                }

                int damage = DoSkillAttack(attacker, opponent, skill);
                if (opponent.HitPoints <= 0)
                {
                    response.Message = $"{opponent.Name} has been defeated!";
                    opponent.HitPoints = 0;
                }
                await _context.SaveChangesAsync();

                response.Data = new AttackResultDto
                {
                    AttackerName = attacker.Name,
                    AttackerHp = attacker.HitPoints,
                    Opponent = opponent.Name,
                    OpponentHp = opponent.HitPoints,
                    Damage = damage
                };
            }
            catch (System.Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        private static int DoSkillAttack(Character? attacker, Character? opponent, Skill? skill)
        {
            int damage = skill.Damage + (new Random().Next(attacker.Intelligence));
            damage -= new Random().Next(opponent.Defense);

            if (damage > 0)
            {
                opponent.HitPoints -= damage;
            }

            return damage;
        }

        public async Task<ServiceResponse<AttackResultDto>> WeaponAttackAsync(WeaponAttackDto request)
        {
            var response = new ServiceResponse<AttackResultDto>();
            try
            {
                var attacker = await _context.Characters
                    .Include(c => c.Weapon)
                    .FirstOrDefaultAsync(c => c.Id == request.AttackerId);
                var opponent = await _context.Characters
                    .FirstOrDefaultAsync(c => c.Id == request.OpponentId);

                int damage = DoWeaponAttack(attacker, opponent);
                if (opponent.HitPoints <= 0)
                {
                    response.Message = $"{opponent.Name} has been defeated!";
                    opponent.HitPoints = 0;
                }
                await _context.SaveChangesAsync();

                response.Data = new AttackResultDto
                {
                    AttackerName = attacker.Name,
                    AttackerHp = attacker.HitPoints,
                    Opponent = opponent.Name,
                    OpponentHp = opponent.HitPoints,
                    Damage = damage
                };
            }
            catch (System.Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        private static int DoWeaponAttack(Character? attacker, Character? opponent)
        {
            int damage = attacker.Weapon.Damage + (new Random().Next(attacker.Strength));
            damage -= new Random().Next(opponent.Defense);

            if (damage > 0)
            {
                opponent.HitPoints -= damage;
            }

            return damage;
        }

        public async Task<ServiceResponse<List<HighScoreDto>>> GetHighScoreAsync()
        {
            var characters = await _context.Characters
                .Where(c => c.Fights > 0)
                .OrderByDescending(c => c.Victories)
                    .ThenBy(c => c.Defeats)
                .ToListAsync();
            
            var response = new ServiceResponse<List<HighScoreDto>>()
            {
                Data = characters.Select(c => _mapper.Map<HighScoreDto>(c)).ToList()
            };

            return response;
        }
    }
}