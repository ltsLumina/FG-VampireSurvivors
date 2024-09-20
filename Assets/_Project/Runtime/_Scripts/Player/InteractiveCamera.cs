using System;
using System.Linq;
using Cinemachine;
using UnityEngine;

/// <summary>
///   Modifies various camera settings depending on the player's actions or the game state.
/// <example> Zooms out the camera based on the amount of enemies on screen. </example> 
/// </summary>
public class InteractiveCamera : CinemachineExtension
{
    protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
    {
        if (stage == CinemachineCore.Stage.Body)
        {
            // Total amount of active enemies. Not necessarily the amount of enemies on screen.
            //var totalEnemies = FindObjectsOfType<Enemy>().Count(enemy => enemy.Health > 0);
            
            // Zoom out the camera based on the amount of enemies on screen.
            // if (totalEnemies > 0)
            // {
            //     // Zoom out the camera based on the amount of enemies on screen.
            //     //state.Lens.FieldOfView = Mathf.Lerp(state.Lens.FieldOfView, 60 * totalEnemies, deltaTime);
            // }
        }
    }
}
