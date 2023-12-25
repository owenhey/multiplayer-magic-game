using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ComputeShaderTest : MonoBehaviour
{
    public int resolution = 512;
    [Min(1)] public int blurStepSize;
    [Min(0)] public int blurStepCount;
    public Color clearColor = Color.white;
    [Min(0)] public float blurResistance = 1;
    [Range(0, 1)] public float fadeSpeed;
    [Range(0, 100)] public int blurPasses = 1;

    public RenderTexture myTexture;
    public ComputeShader computeShader;
    public RawImage myImage;
    public DrawingMechanic drawer;
    

    public Color color;
    public float size = 1;
    public bool clearOnStartDrawing = false;

    private int blurIndex;
    private int drawIndex;
    private int clearIndex;

    private void Awake(){
        myTexture = new RenderTexture(resolution, resolution, 24);
        myTexture.enableRandomWrite = true;
        myTexture.Create();

        drawer.OnDraw += Draw;
        drawer.OnStartDraw += HandleOnStartDraw;

        blurIndex = computeShader.FindKernel("Blur");
        drawIndex = computeShader.FindKernel("CSMain");
        clearIndex = computeShader.FindKernel("Clear");

        myImage.texture = myTexture;
        myImage.enabled = true;
        
        Clear();
    }

    private void Update(){
        if(Input.GetKeyDown(KeyCode.Space)){
            Clear();
        }
    }

    private void FixedUpdate(){
        Blur();
    }

    // Start is called before the first frame update
    void Draw(Vector2 point)
    {
        point *= resolution;

        computeShader.SetTexture(0, "Result", myTexture);

        computeShader.SetFloats("Color", new float[4]{color.r, color.g, color.b, color.a});
        computeShader.SetFloats("Resolution", new float[2]{myTexture.width, myTexture.height});
        computeShader.SetFloat("Size", size);
        computeShader.SetFloats("Point", new float[2]{point.x, point.y});

        computeShader.Dispatch(drawIndex, myTexture.width / 8, myTexture.height / 8, 1);
    }

    void Blur(){
        computeShader.SetTexture(blurIndex, "Result", myTexture);
        computeShader.SetInt("BlurSteps", blurStepCount);
        computeShader.SetInt("BlurStepSize", blurStepSize);
        computeShader.SetInt("NumBlurs", blurPasses);
        computeShader.SetFloat("BlurResistance", blurResistance);
        computeShader.SetFloat("FadeSpeed", 1 - fadeSpeed);
        computeShader.Dispatch(blurIndex, myTexture.width / 8, myTexture.height / 8, 1);
    }

    void HandleOnStartDraw(){
        if(clearOnStartDrawing) Clear();
    }

    public void Clear(){
        computeShader.SetTexture(clearIndex, "Result", myTexture);
        computeShader.SetFloats("Color", new float[4]{clearColor.r, clearColor.g, clearColor.b, clearColor.a});
        computeShader.Dispatch(clearIndex, myTexture.width / 8, myTexture.height / 8, 1);
    }
}
