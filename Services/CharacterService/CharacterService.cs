using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dotnet_rpg.Models;

namespace dotnet_rpg.Services.CharacterService
{
    public class CharacterService : ICharacterService
    {
        private static List<Character> characters = new List<Character>{
            new Character(),
            new Character { Name = "Murder",
                            Id = 1,
                            Class = RpgClass.Barbarian,
                            Intelligence = 500,
                            Strength = 10000}
            };
        public async Task<List<Character>> AddCharacterAsync(Character newCharacter)
        {
            characters.Add(newCharacter);
            return characters;
        }

        public async Task<List<Character>> GetAllCharactersAsync()
        {
            return characters;
        }

        public async Task<Character> GetCharacterByIdAsync(int id)
        {
            return characters.FirstOrDefault(c => c.Id == id);
        }
    }
}