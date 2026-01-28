using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class SwallowSector : MonoBehaviour
{
    public float radius = 2f;
    public float angle = 60f;
    public float swallowCoolTime = 2f;
    public int segments = 32;
    public Mesh mesh;
    public Renderer rd;
    public Color originalColor;
    public Color swallowColor;
    public Color currentColor;
    public bool isVisible;

    void Update()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dir = mousePosition - (Vector2)transform.position;
        float targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, targetAngle);
    }

    void LateUpdate()
    {
        UpdateSectorMesh(); 
    }

    private void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        rd = GetComponent<Renderer>();
        isVisible = false;
        // rd.material.color = originalColor;
    }

    public void SwitchSectorColor()
    {
        StartCoroutine(SwallowSectorCoroutine());
    }
    
    public IEnumerator SwallowSectorCoroutine()
    {
        // rd.material.color = swallowColor;
        currentColor = swallowColor;
        
        yield return new WaitForSeconds(swallowCoolTime);
        // rd.material.color = originalColor;
        currentColor = originalColor;
    }

    public void InitSector(float radius, float angle, float coolTime)
    {
        this.radius = radius;
        this.angle = angle;
        this.swallowCoolTime = coolTime;
        // rd.material.color = originalColor;
        currentColor = originalColor;
        isVisible = true;

        // if (rd.material == null)
        // {
        //     Material vertexColorMaterial = new Material(Shader.Find("Sprites/Default"));
        //     rd.material = vertexColorMaterial;
        // }
    }

    private void UpdateSectorMesh()
    {
        Vector3[] vertices = new Vector3[segments + 2];
        int[] triangles = new int[segments * 3];
        vertices[0] = Vector3.zero;
        float halfAngle = angle * 0.5f;
        float angleStep = angle / segments;
        float currentAngle = -halfAngle;
        Color[] colors = new Color[segments + 2];

        colors[0] = currentColor;
        for(int i = 0; i <= segments; i++)
        {
            float rad = currentAngle * Mathf.Deg2Rad;
            float x = radius * Mathf.Cos(rad);
            float y = radius * Mathf.Sin(rad);
            vertices[i + 1] = new Vector3(x, y, 0);
            colors[i + 1] = currentColor;
            currentAngle += angleStep;

        }
        for(int i=0; i < segments; i++)
        {
            triangles[i*3] = 0;
            triangles[i*3 + 1] = i + 1;
            triangles[i*3 + 2] = i + 2;
        }
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.colors = colors;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }
}
