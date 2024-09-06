using UnityEngine;

/// <summary>
/// Handles playing effects and auras 
/// </summary>
public static class EffectPlayer
{
   public static void PlayEffect(ParticleSystem effect)
   {
      effect.gameObject.SetActive(true);
   }
}
