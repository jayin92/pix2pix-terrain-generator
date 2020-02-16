using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rain3 : MonoBehaviour
{
    public ctrl c;
    public ComputeShader shader;
    ComputeBuffer HeightMap, b1; float[] Flat; ComputeBuffer WaterMap,d1,f,s,s1,e;
    public int numIterations = 10;
    public float KRain = 1, KCapacity = 1, A = 1, Kdmax = 1, KS = 1, KD = 1, RMin = 0.5f, KH = 1, KEvaporation = 0.1f, KMove = 10, drag = 0.1f, maxV = 0.01f,deltaT=0.1f;
    public int w = 256, h = 256;
    Vector4[] f_data;
    int[] kernelId = new int[6];
    public void Init()
    {
        kernelId = new int[6];
        if (HeightMap != null)
        {
            Realease();
        }
        
        w = c.w;
        h = c.h;
        HeightMap = new ComputeBuffer(w * h, sizeof(float));
        WaterMap= new ComputeBuffer(w * h, sizeof(float));
        f = new ComputeBuffer(w * h, sizeof(float) * 4);
        s= new ComputeBuffer(w * h, sizeof(float));
        b1 = new ComputeBuffer(w * h, sizeof(float));
        d1 = new ComputeBuffer(w * h, sizeof(float));
        s1 = new ComputeBuffer(w * h, sizeof(float));
        e = new ComputeBuffer(w * h, sizeof(float)*4);
        Flat = new float[w * h];
        s.SetData(Flat);
        s1.SetData(Flat);
        
        for (int x = 0; x < w; x++)
        {
            for (int y = 0; y < h; y++)
            {
                Flat[x + y * w] = c.heightMap[x, y];
            }
        }
        HeightMap.SetData(Flat);
        b1.SetData(Flat);
        for (int x = 0; x < w; x++)
        {
            for (int y = 0; y < h; y++)
            {
                Flat[x + y * w] = c.waterMap[x,y];
            }
        }
        WaterMap.SetData(Flat);

        f_data = new Vector4[w * h];
        for (int i = 0; i < w * h; i++) f_data[i] = new Vector4();
        f.SetData(f_data);
        e.SetData(f_data);

        kernelId[0] = shader.FindKernel("Rain");
        kernelId[1] = shader.FindKernel("CalculateFlux");
        kernelId[2] = shader.FindKernel("Flow");
        kernelId[3] = shader.FindKernel("d1_d");
        kernelId[4] = shader.FindKernel("Erode");
        kernelId[5] = shader.FindKernel("Deposit");

        foreach (int i in kernelId) {
            shader.SetBuffer(kernelId[i], "f", f);
            shader.SetBuffer(kernelId[i], "s", s);
            shader.SetBuffer(kernelId[i], "b", HeightMap);
            shader.SetBuffer(kernelId[i], "b1", b1);
            shader.SetBuffer(kernelId[i], "d1", d1);
            shader.SetBuffer(kernelId[i], "s1", s1);
            shader.SetBuffer(kernelId[i], "d", WaterMap);
            shader.SetBuffer(kernelId[i], "e", e);
        }


        setParams();
        shader.SetInt("w", w);
        shader.SetInt("h", h);
    }
    public void Rain()
    {
        int gs = 32;
        for (int i = 0; i < numIterations; i++)
        {
            shader.Dispatch(kernelId[0], w /gs, h / gs, 1);
            shader.Dispatch(kernelId[1], w / gs, h / gs, 1);
            shader.Dispatch(kernelId[2], w / gs, h / gs, 1);
            shader.Dispatch(kernelId[3], w / gs, h / gs, 1);
            shader.Dispatch(kernelId[4], w / gs, h / gs, 1);
            shader.Dispatch(kernelId[5], w / gs, h / gs, 1);
        }
        HeightMap.GetData(Flat);
        for (int x = 0; x < w; x++)
        {
            for (int y = 0; y < h; y++)
            {
                 c.heightMap[x, y] =Flat[x + y * w];
            }
        }
        WaterMap.GetData(Flat);
        c.waterMap = new float[w, h];
        for (int x = 0; x < w; x++)
        {
            for (int y = 0; y < h; y++)
            {
                c.waterMap[x, y] = Flat[x + y * w];
            }
        }
        c.Display();
    }
    public void setParams()
    {
        shader.SetFloat("KRain", KRain);
        shader.SetFloat("KCapacity", KCapacity);
        shader.SetFloat("A", A);
        shader.SetFloat("Kdmax", Kdmax);
        shader.SetFloat(" RMin", RMin);
        shader.SetFloat("KH", KH);
        shader.SetFloat("KS", KS);
        shader.SetFloat("KD", KD);
        shader.SetFloat("KEvaporation", KEvaporation);
        shader.SetFloat("KMove", KMove);
        shader.SetFloat("drag", drag);
        shader.SetFloat("maxV", maxV);
        shader.SetFloat("deltaT", deltaT);
    }
    public void Realease()
    {
        HeightMap.Dispose();
        WaterMap.Dispose();
        f.Dispose();
        b1.Dispose();
        d1.Dispose();
        s.Dispose();
    }
}
