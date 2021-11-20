using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class BezierCurve : MonoBehaviour
{
    public GameObject       start;
    public GameObject       startControl;
    public GameObject       end;
    public GameObject       endControl;
    
    public int              resolution;
    public float            width;

    public Color            color;

    public ComputeShader    bezierCompute;
    public Shader           drawShader;

    private ComputeBuffer   vertexBuffer;
    private ComputeBuffer   indexBuffer;
    private ComputeBuffer   argBuffer;  
    private Material        material;
    private Bounds          bounds;

    private CommandBuffer   commandBuffer;

    void Start()
    {
        vertexBuffer = new ComputeBuffer(resolution * 2, sizeof(float) * 4, ComputeBufferType.Structured);
        indexBuffer = new ComputeBuffer(resolution * 6, sizeof(int), ComputeBufferType.Structured);
        argBuffer = new ComputeBuffer(4, sizeof(int), ComputeBufferType.IndirectArguments);
       
        material = new Material(drawShader);
        material.SetPass(0);
        material.SetBuffer("vertexBuffer", vertexBuffer);
        material.SetBuffer("indexBuffer", indexBuffer);
        material.SetColor("color", color);
        material.EnableKeyword("PROCEDURAL_INSTANCING_ON");

        bounds = new Bounds();

        commandBuffer = new CommandBuffer();
        commandBuffer.name = "Draw Bezier";
        commandBuffer.DrawProceduralIndirect(Matrix4x4.identity, material, 0, MeshTopology.Triangles, argBuffer);
        Camera.main.AddCommandBuffer(CameraEvent.AfterForwardOpaque, commandBuffer);

        RecalculateCurveMesh();
    }

    void RecalculateCurveMesh()
    {
        int kernal = bezierCompute.FindKernel("CalculateCurveMesh");

        bezierCompute.SetBuffer(kernal, "vertices", vertexBuffer);
        bezierCompute.SetBuffer(kernal, "indices", indexBuffer);
        bezierCompute.SetBuffer(kernal, "args", argBuffer);

        bezierCompute.SetVector("point0", start.transform.position);
        bezierCompute.SetVector("point1", startControl.transform.position);
        bezierCompute.SetVector("point2", endControl.transform.position);
        bezierCompute.SetVector("point3", end.transform.position);

        bezierCompute.SetInt("resolution", resolution);
        bezierCompute.SetFloat("width", width);

        uint threadX = 0;
        uint threadY = 0;
        uint threadZ = 0;
        bezierCompute.GetKernelThreadGroupSizes(kernal, out threadX, out threadY, out threadZ);

        bezierCompute.Dispatch(kernal, (int)threadX, 1, 1);

        bounds.min = start.transform.position;
        bounds.max = end.transform.position;
    }

    void Update()
    {
        RecalculateCurveMesh();
    }

    void OnDestroy()
    {
        vertexBuffer.Release();
        indexBuffer.Release();
        argBuffer.Release();
    }
}
