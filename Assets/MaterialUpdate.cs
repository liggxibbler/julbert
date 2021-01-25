using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialUpdate : MonoBehaviour
{
    public float stepLength = 1;
    float lastStepTime = 0;
    float nextStepTime;

    public Material material;
    public Vector2 position;
    public float scale;
    public List<Vector2> points;
    int sample = 0;

    private void Awake()
    {
        nextStepTime = stepLength;
        points = new List<Vector2>();
        float scale = 1;
        var corners = new Vector2[] { new Vector2(-.5f, -.5f) * scale, new Vector2(-.5f, .5f) * scale, new Vector2(.5f, .5f) * scale, new Vector2(.5f, -.5f) * scale };
        Hilbert(corners, 5, points);
        Debug.Log(points.Count);
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.UpArrow))
            scale *= 1.01f;
        else if (Input.GetKey(KeyCode.DownArrow))
            scale /= 1.01f;

        if (Input.GetKey(KeyCode.LeftArrow))
            stepLength *= 1.01f;
        else if (Input.GetKey(KeyCode.RightArrow))
            stepLength /= 1.01f;

        if (Input.GetKeyUp(KeyCode.Escape))
            Application.Quit();


        float aspect = (float)Screen.width / Screen.height;
        float scalex = scale;
        float scaley = scale;
        if (aspect > 1)
            scalex = scalex * aspect;
        else
            scaley = scaley / aspect;

        var diff = Input.mousePosition - .5f * new Vector3(Screen.width, Screen.height, 0);
        position = new Vector2(diff.x / Screen.width, diff.y/Screen.height);

        //material.SetVector("_Position", new Vector4(position.x, position.y, scalex, scaley));
        int nextSample = (sample + 1) % points.Count;
        Vector2 lerp = Vector2.Lerp(points[sample], points[nextSample], (Time.time - lastStepTime) / stepLength);
        material.SetVector("_Position", new Vector4(lerp.x, lerp.y, scalex, scaley));

        if (Time.time > nextStepTime)
        {
            lastStepTime = Time.time;
            nextStepTime = lastStepTime + stepLength;
            sample += 1;
            sample = sample % points.Count;
        }
    }

    void Hilbert(Vector2[] corners, int depth, List<Vector2> points)
    {
        if (depth == 0)
        {
            points.AddRange(corners);
        }
        else
        {
            Vector2 c = (corners[2] + corners[0]) * .5f;

            var p0 = (corners[0] - c) * .5f;
            var p1 = (corners[1] - c) * .5f;
            var p2 = (corners[2] - c) * .5f;
            var p3 = (corners[3] - c) * .5f;

            Hilbert(new Vector2[] { corners[0] + p0, corners[0] + p3, corners[0] + p2, corners[0] + p1 }, depth - 1, points);
            Hilbert(new Vector2[] { corners[1] + p0, corners[1] + p1, corners[1] + p2, corners[1] + p3 }, depth - 1, points);
            Hilbert(new Vector2[] { corners[2] + p0, corners[2] + p1, corners[2] + p2, corners[2] + p3 }, depth - 1, points);
            Hilbert(new Vector2[] { corners[3] + p2, corners[3] + p1, corners[3] + p0, corners[3] + p3 }, depth - 1, points);
        }
    }
}
