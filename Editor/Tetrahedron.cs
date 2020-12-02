using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Tetrahedron : ScriptableWizard
{
    public Vector3 size = new Vector3(1,1,1);
    [MenuItem("GameObject/3D Object/Tetrahedron")]
    static void ShowWizard()
    {
        ScriptableWizard.DisplayWizard<Tetrahedron>("Create Tetraheron", "Create");
    }

    void OnWizardCreate()
    {
        var mesh = new Mesh();
        
        Vector3 p0=new Vector3(0,0,0);
        Vector3 p1=new Vector3(1,0,0);
        Vector3 p2=new Vector3(0.5f,0,Mathf.Sqrt(0.75f));
        Vector3 p3=new Vector3(0.5f,Mathf.Sqrt(0.75f),Mathf.Sqrt(0.75f)/3);

        p0.Scale(size);
        p1.Scale(size);
        p2.Scale(size);
        p3.Scale(size);

        mesh.vertices = new Vector3[] {p0, p1, p2, p3};

        mesh.triangles = new int[]
        {
            0, 1, 2,
            0, 2, 3,
            2, 1, 3,
            0, 3, 1
        };
        
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        
        var gameObject = new GameObject("Tetrahedron");
        var meshFilter = gameObject.AddComponent<MeshFilter>();
        meshFilter.mesh = mesh;

        var meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.material= new Material(Shader.Find("Standard"));
        
    }

    void OnWizardUpdate()
    {
        if (this.size.x <= 0 ||
            this.size.y <= 0 ||
            this.size.z <= 0)
        {

            this.isValid = false;
            this.errorString = "Size cannot be less than zero";
        }else
        {
            this.errorString = null;
            this.isValid = true;
        }
    }
}
