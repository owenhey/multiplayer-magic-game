
using UnityEngine;
 
public class ShaderInteractor : MonoBehaviour
{
    private void Update()
    {
        // Set player position
        Shader.SetGlobalVector("_MovingPosition", transform.position);
        
        // Set player movement speed if you can have the value
        // When the value is greater than zero, surround grass 
        // will be highlighted. Set it 0 to ignore this effect!
        Shader.SetGlobalFloat("_MovingSpeedPercent", 0);
    }
}