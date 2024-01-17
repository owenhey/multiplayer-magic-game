using System.Collections;
using System.Collections.Generic;
using Drawing;
using UnityEngine;
using UnityEngine.UI;

public class DrawingComputeShader : MonoBehaviour {
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
    
    
    private static readonly int _fadeSpeed = Shader.PropertyToID("FadeSpeed");
    private static readonly int _blurResistance = Shader.PropertyToID("BlurResistance");
    private static readonly int _numBlurs = Shader.PropertyToID("NumBlurs");
    private static readonly int _blurStepSize = Shader.PropertyToID("BlurStepSize");
    private static readonly int _blurSteps = Shader.PropertyToID("BlurSteps");
    private static readonly int _result = Shader.PropertyToID("Result");
    private static readonly int _point = Shader.PropertyToID("Point");
    private static readonly int _size = Shader.PropertyToID("Size");
    private static readonly int _resolution = Shader.PropertyToID("Resolution");
    private static readonly int _color = Shader.PropertyToID("Color");

    public void Init() {
        myTexture = new RenderTexture(resolution, resolution, 24);
        myTexture.enableRandomWrite = true;
        myTexture.Create();
        
        DrawingManager.OnTranslatedDraw += Draw;
        DrawingManager.OnTranslatedStartDraw += HandleOnStartDraw;

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

        computeShader.SetTexture(0, _result, myTexture);

        computeShader.SetFloats(_color, new float[4]{color.r, color.g, color.b, color.a});
        computeShader.SetFloats(_resolution, new float[2]{myTexture.width, myTexture.height});
        computeShader.SetFloat(_size, size);
        computeShader.SetFloats(_point, new float[2]{point.x, point.y});

        computeShader.Dispatch(drawIndex, myTexture.width / 8, myTexture.height / 8, 1);
    }

    void Blur(){
        computeShader.SetTexture(blurIndex, _result, myTexture);
        computeShader.SetInt(_blurSteps, blurStepCount);
        computeShader.SetInt(_blurStepSize, blurStepSize);
        computeShader.SetInt(_numBlurs, blurPasses);
        computeShader.SetFloat(_blurResistance, blurResistance);
        computeShader.SetFloat(_fadeSpeed, 1 - fadeSpeed);
        computeShader.Dispatch(blurIndex, myTexture.width / 8, myTexture.height / 8, 1);
    }

    void HandleOnStartDraw(){
        if(clearOnStartDrawing) Clear();
    }

    public void Clear(){
        computeShader.SetTexture(clearIndex, _result, myTexture);
        computeShader.SetFloats(_color, new float[4]{clearColor.r, clearColor.g, clearColor.b, clearColor.a});
        computeShader.Dispatch(clearIndex, myTexture.width / 8, myTexture.height / 8, 1);
    }
}
