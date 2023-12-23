using System;
using System.Collections.Generic;
using System.Reflection;
using Spells;

namespace Helpers {
    public static class SpellEffectFactory
    {
        private static Dictionary<string, Type> spellEffectTypes;

        static SpellEffectFactory()
        {
            LoadSpellEffectTypes();
        }

        private static void LoadSpellEffectTypes()
        {
            spellEffectTypes = new Dictionary<string, Type>();

            foreach (var type in Assembly.GetAssembly(typeof(SpellEffectBase)).GetTypes())
            {
                if (type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(SpellEffectBase)))
                {
                    var spellAttribute = type.GetCustomAttribute<SpellEffectAttribute>();
                    if (spellAttribute != null)
                    {
                        spellEffectTypes[spellAttribute.Id] = type;
                    }
                }
            }
        }

        public static SpellEffectBase CreateSpellEffect(string id)
        {
            if (spellEffectTypes.TryGetValue(id, out Type type))
            {
                return Activator.CreateInstance(type) as SpellEffectBase;
            }

            throw new ArgumentException($"No spell found with id '{id}'");
        }
    }

}