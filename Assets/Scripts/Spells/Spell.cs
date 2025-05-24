using UnityEngine;
using System.Collections.Generic;

public class Spell
{
    public ElementType element;
    public ShapeType shape;
    public List<ModifierType> modifiers = new List<ModifierType>();

    public Spell(ElementType _element, ShapeType _shape, List<ModifierType> _mods)
    {
        this.element = _element;
        this.shape = _shape;
        this.modifiers = _mods;
    }

    public string GetPseudoCode()
    {
        string mods = string.Join(", ", modifiers);
        return $"CastSpell(\"{element}{shape}\") {{\n Element = {element};\n Shape = {shape};\n Modifiers = [{mods}];\n}}";
    }
}
