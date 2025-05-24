using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;

public class SpellCaster : MonoBehaviour
{
    [SerializeField] private GameObject spellPrefab;
    [SerializeField] private Transform spellSpawnPoint;
    [SerializeField] private Camera cam;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            CastTestSpell();
        }
    }

    private void CastTestSpell()
    {
        Vector3 direction;

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            direction = (hit.point - spellSpawnPoint.position).normalized;
        }
        else
        {
            direction = cam.transform.forward;
        }

        GameObject newSpell = Instantiate(spellPrefab, spellSpawnPoint.position, Quaternion.LookRotation(direction));

        var motion = newSpell.GetComponentInChildren<RFX1_TransformMotion>();
        if (motion != null)
        {
            motion.CollisionEnter += (sender, collisionInfo) =>
            {
                if (collisionInfo.Hit.collider.TryGetComponent(out Enemy enemy))
                {
                    enemy.TakeDamage(5f);
                }
            };
        }
    }

    public void CastSpell(Spell spell)
    {
        // Here I can instantiate a projectile prefab based on spell.shape
        Debug.Log(spell.GetPseudoCode());
        // Apply effects to target(s) based on element/modifiers.
    }
}
