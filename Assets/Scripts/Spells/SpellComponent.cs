using UnityEngine;

public enum ComponentType { Element, Shape, Modifier }
public enum ElementType { Fire, Ice, Lightning }
public enum ShapeType { Bolt, AOE, Beam }
public enum ModifierType { Piercing, Bouncing, DOT }

[CreateAssetMenu(fileName = "SpellComponent", menuName = "Scriptable Objects/Spell/Component")]
public class SpellComponent : ScriptableObject
{
    public ComponentType componentType;
    public string label;
}
