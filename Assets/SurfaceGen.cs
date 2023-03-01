using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public struct QuadTree
{
    public QuadTree(bool isLeaf, int noChildren=4 )
    {
        this.isLeaf = isLeaf;
        children = new QuadTree[noChildren];
        points = new int[]{-1, -1, -1, -1};

    }
    public QuadTree[] children;
    public bool isLeaf;
    public int[] points;
    
    public void addNextPoint(int pointPos)
    {
        for (int i = 0; i < points.Length; i++)
        {
            Debug.Log($"lenght: {points.Length}, points: {points}");
            if (points[i] < 0)
            {
                points[i] = pointPos;
                return;
            }
        }
    }

}

public class SurfaceGen : MonoBehaviour
{
    enum Face {Front=2, Back=3 , Top=5, Bottom=7 ,Left=11, Right=13 };

    [Serializable]
    public struct Point
    {
        public Point(Vector3 _pos,  uint _faces)
        {
            Pos = _pos;
            Faces = _faces;
        }
        public Vector3 Pos;
        public uint Faces;
    }

    private QuadTree rootQuadTree;
    

    public int subLevel = 1;
    // Start is called before the first frame update
    public List<Point> points = new List<Point>();
    private Dictionary<Face, List<Vector3>> _faces;
    

    void Start()
    {
        rootQuadTree = new QuadTree(false, 6);
        for (int i = 0; i < rootQuadTree.children.Length; i++)
        {
            rootQuadTree.children[i] = new QuadTree(true);
        }
        
        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                for (int k = -1; k < 2; k++)
                {
                    if (i == 0 || j == 0 || k == 0)
                    {
                        continue;
                    }


                    
                    Vector3 p = new Vector3(i, j, k);
                    
                    uint face_flag = 1;
                    int size = points.Count;
                    if (i > 0)
                    {
                        face_flag *= (int)Face.Front;
                        rootQuadTree.children[0].addNextPoint(size);
                    }
                    else
                    {
                        face_flag *= (int)Face.Back;
                        rootQuadTree.children[1].addNextPoint(size);
                    }

                    if (j > 0)
                    {
                        face_flag *= (int)Face.Top;
                        rootQuadTree.children[2].addNextPoint(size);
                    }
                    else
                    {
                        face_flag *= (int)Face.Bottom;
                        rootQuadTree.children[3].addNextPoint(size);
                    }

                    if (k > 0)
                    {
                        face_flag *= (int)Face.Left;
                        rootQuadTree.children[4].addNextPoint(size);
                    }
                    else
                    {
                        face_flag += (int)Face.Right;
                        rootQuadTree.children[5].addNextPoint(size);
                    }

                    points.Add(new Point(p, face_flag));
                }
            }
            
        }
        
        Subdivide(rootQuadTree);

    }

    void Subdivide(QuadTree tree)
    {
        if (tree.isLeaf)
        {
            tree.children = new QuadTree[4];
            Vector3[] vecs = new Vector3[4];
            
            vecs[0] = points[tree.points[0]].Pos;
            vecs[1] = points[tree.points[1]].Pos;
            vecs[2] = points[tree.points[2]].Pos;
            vecs[3]  = points[tree.points[3]].Pos;

            float minX = float.MaxValue;
            float minY = float.MaxValue;
            float minZ = float.MaxValue;
            float maxX = float.MinValue;
            float maxY = float.MinValue;
            float maxZ = float.MinValue;
            
            foreach (var pt in vecs)
            {
                if (pt.x < minX)
                {
                    minX = pt.x;
                }
                
                if (pt.x > maxX)
                {
                    maxX = pt.x;
                }
                
                if (pt.y < minY)
                {
                    minY = pt.y;
                }
                
                if (pt.y > maxY)
                {
                    maxY = pt.y;
                }
                
                if (pt.z < minZ)
                {
                    minZ = pt.z;
                }
                
                if (pt.z > maxZ)
                {
                    maxZ = pt.z;
                }
            } //end foreach

            Vector3 n0 = new Vector3((minX + maxX) / 2, (minY + maxY) / 2, (minZ + maxZ) / 2);

            
            points.Add(new Point(n0, 0));
            
            
            for(int i =0; i < tree.children.Length; i++)
            {
                tree.children[i] = new QuadTree(true, 4);
                
                // tree.children[i].points;
            }
            return;
        }

        for (int i = 0; i < tree.children.Length; i++)
        {
            Subdivide(tree.children[i]);
        }
    }

    private void OnDrawGizmos()
    {
        foreach (Point point in points)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(point.Pos, .1f);

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
