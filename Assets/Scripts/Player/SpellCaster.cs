using UnityEngine;

public class SpellCaster : MonoBehaviour
{
    public void CastSpell(Spell spell)
    {
        // Here I can instantiate a projectile prefab based on spell.shape
        Debug.Log(spell.GetPseudoCode());
        // Apply effects to target(s) based on element/modifiers.
    }
}
