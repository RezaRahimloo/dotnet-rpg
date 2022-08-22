using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dotnet_rpg.Data;
using dotnet_rpg.Dtos.Fight;
using dotnet_rpg.Models;
using Microsoft.EntityFrameworkCore;

namespace dotnet_rpg.Services.FightService
{
    public class FightService : IFightService
    {
        private readonly DataContext _context;
        public FightService(DataContext context)
        {
            _context = context;
            
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
                if(skill is null)
                {
                    response.Success = false;
                    response.Message = "Skill not found!";
                    return response;
                }

                int damage = skill.Damage + (new Random().Next(attacker.Intelligence));
                damage -= new Random().Next(opponent.Defense);

                if(damage > 0)
                {
                    opponent.HitPoints -= damage;
                }
                if(opponent.HitPoints <= 0)
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
                
                int damage = attacker.Weapon.Damage + (new Random().Next(attacker.Strength));
                damage -= new Random().Next(opponent.Defense);

                if(damage > 0)
                {
                    opponent.HitPoints -= damage;
                }
                if(opponent.HitPoints <= 0)
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
    }
}