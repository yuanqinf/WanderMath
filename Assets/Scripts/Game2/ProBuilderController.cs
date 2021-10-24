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
    [SerializeField]
    private ProBuilderMesh ramp;
    private Dictionary<int, Edge> topEdges = new Dictionary<int, Edge>();
    private int[] edgeNums = { 4, 5, 6, 7 };

    void Start()
    {
        //CreateSingleQuad();
        //CreateSingleCube();
        //ramp = Instantiate(ramp, new Vector3(0, 0, 0), Quaternion.identity);
        // TODO: change location & size accordingly to be instantiated center
        ramp = Instantiate(ramp, new Vector3(0, 0, 0), ramp.transform.rotation);
        //ramp.transform.localScale += new Vector3(0, 3.5f, 0);
        foreach (Face f in ramp.faces)
        {
            foreach (Edge e in f.edges)
            {
                foreach(int num in edgeNums)
                {
                    if (e.a == num)
                    {
                        topEdges.Add(num, e);
                    }
                }
            }
        }

    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit raycastHit))
            {
                if (raycastHit.transform.tag == "ramp")
                {
                    MoveCubeUp(raycastHit.transform.name);
                    raycastHit.transform.localPosition += new Vector3(0.1f, 0, 0);
                    //if (isDragging && raycastHit.transform.position != startObj.transform.position)
                    //{
                    //    connectedDots.Add(startObj);
                    //    connectedDots.Add(raycastHit.transform.gameObject);
                    //    isSnapping = true;
                    //    curedgeCount += 1;
                    //}
                    //startObj = raycastHit.transform.gameObject;
                }

            }
        }
    }

    private void MoveCubeUp(string edge)
    {
        Debug.Log(edge);
        var movingEdge = Enumerable.Repeat(topEdges[Int32.Parse(edge)], 1);
        ramp.TranslateVertices(movingEdge, new Vector3(0.1f, 0, 0));
    }

    private void CreateSingleCube()
    {

        ramp = ShapeGenerator.GenerateCube(PivotLocation.Center, new Vector3(0.1f, 1, 1));
        Debug.Log("edge count: " + ramp.edgeCount);
        Debug.Log("face count: " + ramp.faceCount);
        var selectedEdges = ramp.selectedEdges;
        foreach (Edge e in selectedEdges)
        {
            Debug.Log("selected edge location: " + e.a + " " + e.b);

        }
        ramp.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
        //cube.transform.position = new Vector3(0f, 1f, 0f);
        ramp.SetMaterial(ramp.faces, quadMaterial);
        Transform trs = ramp.transform;
        int count = 1;
        int faceCount = 1;
        foreach (Face f in ramp.faces)
        {
            foreach (int i in f.distinctIndexes)
            {
                Debug.Log("face count: " + faceCount + " indexPt: " + trs.TransformPoint(ramp.positions[i]));

            }
            foreach (Edge e in f.edges) {
                if (e.b == 7)
                {
                    var movingEdge = Enumerable.Repeat(e, 1);
                    ramp.TranslateVertices(movingEdge, new Vector3(1, 0, 0));
                }

                Debug.Log("edge location: " + e.a + " " + e.b);
                var posA = trs.TransformPoint(ramp.positions[e.a]);
                var posB = trs.TransformPoint(ramp.positions[e.b]);
                Debug.Log("edge " + count + " pos: " + posA + " " + posB);
                count++;
            }
            faceCount++;
        }
        var test = ramp.selectedEdges;
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
