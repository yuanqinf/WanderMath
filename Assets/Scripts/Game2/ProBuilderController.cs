using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;

public class ProBuilderController : MonoBehaviour
{
    [SerializeField]
    private Material quadMaterial;

    private ProBuilderMesh quad;
    private ProBuilderMesh cube;

    void Start()
    {
        //CreateSingleQuad();
        CreateSingleCube();
    }

    private void CreateSingleCube()
    {

        cube = ShapeGenerator.GenerateCube(PivotLocation.Center, new Vector3(0.01f, 1, 1));
        Debug.Log("edge count: " + cube.edgeCount);
        Debug.Log("face count: " + cube.faceCount);
        Debug.Log("selected edge: " + cube.selectedEdges.ToString());
        cube.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
        cube.transform.position = new Vector3(1.2f, 1.1f, 1.3f);
        cube.SetMaterial(cube.faces, quadMaterial);
        foreach (Face f in cube.faces)
        {
            Debug.Log(f.IsQuad());
            foreach (Edge e in f.edges) {
                if (e.a == 5)
                {
                    var movingEdge = Enumerable.Repeat(e, 1);
                    cube.TranslateVertices(movingEdge, new Vector3(1, 0, 0));
                    //cube.TranslateVertices(f.edges, new Vector3(10, 0, 0));
                }
                
                Debug.Log("edge location: " + e.a + " " + e.b);
            }
        }
        var test = cube.selectedEdges;
        quad.Refresh();
    }

    private void EditingPlane()
    {
        //if (Input.)
    }

    // create a list of verticies & faces
    // TODO: enhance this logic to use start points from drawing
    private void CreateSingleQuad()
    {
        quad = ProBuilderMesh.Create(
            new Vector3[] {
                new Vector3(0f, 0f, 0f),
                new Vector3(1f, 0f, 0f),
                new Vector3(0f, 1f, 0f),
                new Vector3(1f, 1f, 0f)
            },
            new Face[] { new Face(new int[] { 0, 1, 2, 1, 3, 2 } )
        });
        quad.SetMaterial(quad.faces, quadMaterial);
        quad.Refresh();
        quad.ToMesh();
    }

    private void CreateDoubleSideQuad()
    {

    }
}
