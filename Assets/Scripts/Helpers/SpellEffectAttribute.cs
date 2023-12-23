using System;

[AttributeUsage(AttributeTargets.Class)]
public class SpellEffectAttribute : Attribute
{
    public string Id { get; private set; }

    public SpellEffectAttribute(string id)
    {
        Id = id;
    }
}