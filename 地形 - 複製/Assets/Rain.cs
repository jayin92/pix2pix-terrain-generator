using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Rain : MonoBehaviour
{
    public float g = 0.01f;
    public float erosionSpeed = 0.1f,depositionSpeed,capacity=1, repeat = 1000, drag = 0.95f,carriedInt=0.1f;
    public bool debug;
    public float GetGroundHeight(float x, float y, float[,] map)
    {

        int x_ = Mathf.FloorToInt(x), y_ = Mathf.FloorToInt(y);
        float a = map[x_, y_],
          b = map[x_ + 1, y_],
          c = map[x_, y_ + 1],
          d = map[x_ + 1, y_ + 1];
        Vector2 der = new Vector2(d + b - a - c, d + c - a - b) * 0.5f;
        return a + der.x * (x - x_) + der.y * (y - y_);
    }
    public void rain(float[,] map,float xInt=-1,float zInt=-1)
    {
        int w = map.GetLength(0), h = map.GetLength(1);

        Vector3 pos = new Vector3(-1, -1, -1);
        Vector3 vel = new Vector3(-1, -1, -1);
        float posx_, posz_;
        for (int t = 0; t < repeat;)
        {
            if (xInt == -1)//不指定雨滴的初始位置
            {
                posx_ = Random.Range(0, w - 1);
                posz_ = Random.Range(0, h - 1);
            }
            else
            {
                posx_ = xInt; posz_ = zInt;
            }
            
            float carried = carriedInt;
            pos = new Vector3(posx_, GetGroundHeight(posx_, posz_, map), posz_);
            vel = Vector3.zero;
            while (true) 
            {
                if (pos.y < 0.01) break;
                if (t < 100&&debug)
                {
                    print(pos);
                }
                vel += new Vector3(0, -g, 0);
                int x = Mathf.FloorToInt(pos.x), z = Mathf.FloorToInt(pos.z);
                float a = map[x, z],
                  b = map[x + 1, z],
                  c = map[x, z + 1],
                  d = map[x + 1, z + 1];
                Vector2 gradient = new Vector2(d + b - a - c, d + c - a - b) * 0.5f;
                Vector3 nor = Vector3.Cross(new Vector3(0, gradient.y, 1), new Vector3(1, gradient.x, 0));
                x = Mathf.FloorToInt(vel.x + pos.x); z = Mathf.FloorToInt(vel.z + pos.z);
                if ((pos.x + vel.x <= 0 || pos.x + vel.x >= w - 1 || pos.z + vel.z <= 0 || pos.z + vel.z >= h - 1)) { break; }
                float groundHeight = GetGroundHeight(pos.x + vel.x, pos.z + vel.z, map);
                if (pos.y + vel.y < groundHeight)
                {
                    float k = (groundHeight - pos.y - vel.y) / nor.y;
                    float carried_ = Mathf.Min(Mathf.Max(0, carried + erosionSpeed * vel.magnitude- depositionSpeed), capacity);
                    float erosion = carried_ - carried;
                    carried = carried_;
                    #region
                    if (vel.x > 0)
                        if (vel.y > 0)
                            map[x + 1, z + 1] -= erosion;
                        else
                            map[x + 1, z] -= erosion;
                    else
                    if (vel.y > 0)
                        map[x, z + 1] -= erosion;
                    else
                        map[x, z] -= erosion;
                    #endregion
                    vel += nor * k;//正向力
                }

                pos += vel;
                vel *= drag;
                t++;
                if (t > repeat || pos.y < 0.02 || pos.x < 0 || pos.x > w - 1 || pos.z < 0 || pos.z > h - 1) {
                    if (vel.x < 0)
                        if (vel.y < 0)
                            map[x + 1, z + 1] += carried;
                        else
                            map[x + 1, z] += carried;
                    else
                        if (vel.y < 0)
                        map[x, z + 1] += carried;
                        else
                        map[x, z] += carried;

                    break;
                }
            }

            for (int x = 0; x < w - 1; x++)
            {
                for (int z = 0; z < h - 1; z++)
                {
                    if (map[x, z] < 0)
                    {
                        map[x, z] = 0;
                    }
                }
            }
            if (xInt != -1)return;
        }

    }
    public void rain2(float[,] map, float xInt = -1, float zInt = -1)
    {
        int w = map.GetLength(0), h = map.GetLength(1);

        Vector3 pos = new Vector3(-1, -1, -1);
        Vector3 vel = new Vector3(-1, -1, -1);
        float posx_, posz_;
        for (int t = 0; t < repeat;)
        {
            if (xInt == -1)//不指定雨滴的初始位置
            {
                posx_ = Random.Range(0, w - 1);
                posz_ = Random.Range(0, h - 1);
            }
            else
            {
                posx_ = xInt; posz_ = zInt;
            }

            float carried = carriedInt;
            pos = new Vector3(posx_, GetGroundHeight(posx_, posz_, map), posz_);
            vel = Vector3.zero;
            while (true)
            {
                if (pos.y < 0.01) break;
                if (t < 100 && debug)
                {
                    print(pos);
                }
                vel *= drag;
                vel += new Vector3(0, -g, 0);
                int x = Mathf.FloorToInt(pos.x), z = Mathf.FloorToInt(pos.z);
                float a = map[x, z],
                  b = map[x + 1, z],
                  c = map[x, z + 1],
                  d = map[x + 1, z + 1];
                Vector2 gradient = new Vector2(d + b - a - c, d + c - a - b) * 0.5f;
                Vector3 nor = Vector3.Cross(new Vector3(0, gradient.y, 1), new Vector3(1, gradient.x, 0));
                x = Mathf.FloorToInt(vel.x + pos.x); z = Mathf.FloorToInt(vel.z + pos.z);
                if ((pos.x + vel.x <= 0 || pos.x + vel.x >= w - 1 || pos.z + vel.z <= 0 || pos.z + vel.z >= h - 1)) { break; }
                float groundHeight = GetGroundHeight(pos.x + vel.x, pos.z + vel.z, map);
                if (pos.y + vel.y < groundHeight)
                {
                    float k = (groundHeight - pos.y - vel.y) / nor.y;
                    float carried_ = Mathf.Min(Mathf.Max(0, carried + erosionSpeed * vel.magnitude - depositionSpeed), capacity);
                    float erosion = carried_ - carried;
                    carried = carried_;
                    #region
                    if (vel.x > 0)
                        if (vel.y > 0)
                            map[x + 1, z + 1] -= erosion;
                        else
                            map[x + 1, z] -= erosion;
                    else
                    if (vel.y > 0)
                        map[x, z + 1] -= erosion;
                    else
                        map[x, z] -= erosion;
                    #endregion
                    vel += nor * k;//正向力
                }

                pos += vel;
                
                t++;
                if (t > repeat || pos.y < 0.02 || pos.x < 0 || pos.x > w - 1 || pos.z < 0 || pos.z > h - 1)
                {
                    if (vel.x < 0)
                        if (vel.y < 0)
                            map[x + 1, z + 1] += carried;
                        else
                            map[x + 1, z] += carried;
                    else
                        if (vel.y < 0)
                        map[x, z + 1] += carried;
                    else
                        map[x, z] += carried;

                    break;
                }
            }

            for (int x = 0; x < w - 1; x++)
            {
                for (int z = 0; z < h - 1; z++)
                {
                    if (map[x, z] < 0)
                    {
                        map[x, z] = 0;
                    }
                }
            }
            if (xInt != -1) return;
        }

    }
}