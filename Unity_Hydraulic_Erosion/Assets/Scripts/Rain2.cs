using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Rain2 : MonoBehaviour {
    public float KRain = 1, KCapacity = 1, A = 1, Kdmax = 1, KS = 1, KD = 1, RMin = 0.5f, KH = 1, KEvaporation = 0.1f,KMove=10,drag=0.1f,maxV=0.01f;
    public int deafultIters;
    public float deltaT;

    int w, h;
    float[,] b,s,s2,r, d1;
    public float[,] d;
    float[,] c;
    float[,,] f;
    Vector3[,] v;
    int[,] sea;
    Texture2D a ;
    public void Init(float[,] b)
    {

        this.b = b;
        w = b.GetLength(0); h = b.GetLength(1);
        s = new float[w, h];
        s2 = new float[w, h];
        r = new float[w, h];
        sea = new int[w,h];
        for (int x = 0; x < w; x++)
        {
            for (int y = 0; y < h; y++)
            {
                r[x, y] = 1;
                if (b[x, y] == 0) { sea[x, y] = 1; }
            }
        }
        d = new float[w, h];
        d1 = new float[w, h];
        c = new float[w, h];
        v = new Vector3[w, h];
        f = new float[w, h, 4];//x,y,(R,L,T,B)
    }

    public void Rain(int iters = -1)
    {
        if (iters == -1) iters =deafultIters;
        for (int i = 0; i < iters; i++)
        {
            //rain
            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    d[x, y] += KRain * deltaT;
                }
            }

            //calculate flux
            for (int x = 0; x < w; x++)
            { 
                for (int y = 0; y < h; y++)
                {

                    f[x, y, 0] = x == w - 1 ? 0 : Mathf.Max(0, f[x, y, 0] + deltaT * A * (b[x, y] + d[x, y] - b[x + 1, y] - d[x + 1, y]));
                    f[x, y, 1] = x == 0 ? 0 : Mathf.Max(0, f[x, y, 1] + deltaT * A * (b[x, y] + d[x, y] - b[x - 1, y] - d[x - 1, y]));
                    f[x, y, 2] = y == h - 1 ? 0 : Mathf.Max(0, f[x, y, 2] + deltaT * A * (b[x, y] + d[x, y] - b[x, y + 1] - d[x , y + 1]));
                    f[x, y, 3] = y == 0 ? 0 : Mathf.Max(0, f[x, y, 3] + deltaT * A * (b[x, y] + d[x, y] - b[x, y - 1] - d[x, y - 1]));
                    float K = (f[x, y, 0] + f[x, y, 1] + f[x, y, 2] + f[x, y, 3])<0.0000001?1: Mathf.Min(1, d[x, y] / (f[x, y, 0] + f[x, y, 1] + f[x, y, 2] + f[x, y, 3]) / deltaT);
                    f[x, y, 0] *= K;
                    f[x, y, 1] *= K;
                    f[x, y, 2] *= K;
                    f[x, y, 3] *= K;
                }
            }

            //flow
            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    d1[x, y] = d[x, y];
                    d[x, y] +=  (x == 0 ? 0 : deltaT*f[x - 1, y, 0]);
                    d[x, y] +=  (x == w - 1 ? 0 : deltaT *f[x + 1, y, 1]);
                    d[x, y] +=  (y == 0 ? 0 : deltaT *f[x, y - 1, 2]);
                    d[x, y] +=  (y == h - 1 ? 0 : deltaT *f[x, y + 1, 3]);
                    d[x, y] -= deltaT *(f[x, y, 0] + f[x, y, 1] + f[x, y, 2] + f[x, y, 3]);

                }
            }
            
            //calculate capacity
            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    Vector3 v_ = new Vector3(x == 0 ? 0 : f[x - 1, y, 0]
                    -(( x == w - 1) ? 0 : f[x + 1, y, 1])
                    + f[x, y, 0]
                    - f[x, y, 1],
                    y == 0 ? 0 : f[x, y - 1, 2]
                    -(( y == h - 1) ? 0 : f[x, y + 1, 3])
                    + f[x, y, 2]
                    - f[x, y, 3], 0);
                    v[x, y] = v_;
                    Vector3 g = new Vector3(x == 0||x==w-1 ? 0 :( b[x - 1, y] -b[x + 1, y])
                    ,
                    y == 0|| y == h - 1? 0 :( b[x, y - 1]-b[x, y + 1]));
                    c[x, y] =Mathf.Max(0, KCapacity /** (-Vector3.Dot(v_,Vector3.Normalize( Vector3.Cross(new Vector3(0, 1, g.y), new Vector3(1, 0, g.x))))) */*Mathf.Min(maxV, v_.magnitude) * Mathf.Min(Kdmax, d1[x, y]));
                }
            }
            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    if (s[x, y] <= c[x, y])
                    {
                        float a = deltaT * r[x, y] * KS * (c[x, y] - s[x, y]);
                        b[x, y] -= a;
                        s[x, y] += a;
                        d[x, y] += a;
                    }
                    else
                    {
                        float a = deltaT * KD * (s[x, y] - c[x, y]);
                        b[x, y] += a;
                        s[x, y] -= a;
                        d[x, y] -= a;
                    }
                    r[x, y] = Mathf.Max(RMin, r[x, y] - deltaT * KS * KH * (s[x, y] - c[x, y]));
                }
            }

            
            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    float xs = x + v[x, y].x * deltaT*KMove, ys = y + v[x, y].y * deltaT*KMove;
                    int fxs = (int)xs, fys = (int)ys;
                    float dx = xs - fxs, dy = ys - fys;
                    s2[Mathf.Min(Mathf.Max(0, fxs), w - 1), Mathf.Min(Mathf.Max(0, fys), h - 1)] += s[x, y] * (1 - dx) * (1 - dy);
                    s2[Mathf.Min(Mathf.Max(0, fxs+1), w - 1), Mathf.Min(Mathf.Max(0, fys), h - 1)] += s[x, y] * dx * (1 - dy);
                    s2[Mathf.Min(Mathf.Max(0, fxs), w - 1), Mathf.Min(Mathf.Max(0, fys+1), h - 1)] += s[x, y] * (1 - dx) * dy;
                    s2[Mathf.Min(Mathf.Max(0, fxs+1), w - 1), Mathf.Min(Mathf.Max(0, fys+1), h - 1)] += s[x, y] * dx*dy;
                }
            }


                    for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    s[x, y] = s2[x, y];
                    s2[x, y] = 0;
                    d[x, y] *= (1.0f - KEvaporation );
                    for (int j = 0; j < 4; j++)
                    {
                        f[x, y, j] *= (1.0f - drag);
                    }
                    /*
                    if (sea[x, y] == 1)
                    {
                        d[x, y] = 0;
                        b[x, y] = 0;
                    }*/

                }
            }
        }
    }
 }