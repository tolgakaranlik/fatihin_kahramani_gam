using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellCaster : MonoBehaviour
{
    GameObject[] Spells;

    public void CastSpell(int index, Vector3 startPos, GameObject target)
    {
        var currentInstance = Instantiate(Spells[index]);
        var targetScript = currentInstance.GetComponent<RFX1_AnimatorEvents>();
        if (targetScript != null) targetScript.Target = target;

        var targetScript2 = currentInstance.GetComponent<RFX1_Target>();
        if (targetScript2 != null) targetScript2.Target = target;

        //var transformMotion = currentInstance.GetComponentInChildren<RFX1_TransformMotion>();

        var animator = currentInstance.GetComponent<RFX1_AnimatorEvents>();
        if (animator != null)
        {
            animator.Speed = 4;
        }
    }
}
