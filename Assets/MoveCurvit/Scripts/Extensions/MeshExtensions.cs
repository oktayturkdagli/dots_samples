using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace MoveCurvit.Scripts.Extensions
{
    public static class MeshExtensions
    {
        public static Mesh BuildMeshForLanelet(NativeList<float3> leftVertices, NativeList<float3> rightVertices)
        {
            //Find bigger way
            bool isEqual = leftVertices.Length == rightVertices.Length;

            // Combine in one array
            // Line left is assigned to even numbers
            // Line right is assigned to odd numbers
            Vector3[] vertices;
            int smallCounter = -1;
            if (isEqual)
            {
                vertices = new Vector3[leftVertices.Length * 2];
                for (var i = 0; i < leftVertices.Length; i++)
                    vertices[i * 2] = leftVertices[i];
            
                for (var i = 0; i < rightVertices.Length; i++)
                    vertices[smallCounter += 2] = rightVertices[i];
            }
            else
            {
                NativeList<float3> bigger;
                NativeList<float3> smaller;

                if (leftVertices.Length >= rightVertices.Length)
                {
                    bigger = leftVertices;
                    smaller = rightVertices;
                }
                else
                {
                    bigger = rightVertices;
                    smaller = leftVertices;
                }
            
                vertices = new Vector3[bigger.Length * 2];
                for (int i = 0; i < bigger.Length; i++)
                    vertices[i * 2] = bigger[i];
            
                for (int i = 0; i < smaller.Length; i++)
                    vertices[smallCounter += 2] = smaller[i];
            }
        
            int[] triangles = CalculateTrianglesForLanelet(leftVertices.Length + rightVertices.Length, isEqual, vertices);
            return CreateMesh(vertices, triangles);
        }

        public static void SetMeshVertices(NativeList<float3> leftVertices, NativeList<float3> rightVertices, Mesh mesh)
        {
            bool isEqual = leftVertices.Length == rightVertices.Length;

            // Combine in one array
            // Line left is assigned to even numbers
            // Line right is assigned to odd numbers
            NativeArray<float3> vertices;
            int smallCounter = -1;
            if (isEqual)
            {
                vertices = new NativeArray<float3>(leftVertices.Length * 2, Allocator.Temp);
                for (var i = 0; i < leftVertices.Length; i++)
                    vertices[i * 2] = leftVertices[i];
            
                for (var i = 0; i < rightVertices.Length; i++)
                    vertices[smallCounter += 2] = rightVertices[i];
            }
            else
            {
                NativeList<float3> bigger;
                NativeList<float3> smaller;

                if (leftVertices.Length >= rightVertices.Length)
                {
                    bigger = leftVertices;
                    smaller = rightVertices;
                }
                else
                {
                    bigger = rightVertices;
                    smaller = leftVertices;
                }
            
                vertices = new NativeArray<float3>(bigger.Length * 2, Allocator.Temp);
                for (int i = 0; i < bigger.Length; i++)
                    vertices[i * 2] = bigger[i];
            
                for (int i = 0; i < smaller.Length; i++)
                    vertices[smallCounter += 2] = smaller[i];
            }
        
            mesh.SetVertices(vertices);
        }
    
        private static Mesh CreateMesh(Vector3[] vertices, int[] triangles)
        {
            Vector2[] uvs = new Vector2[vertices.Length];
            for (int i = 0; i < uvs.Length; i++)
            {
                uvs[i] = new Vector2(vertices[i].x, vertices[i].z);
            }
        
            var mesh = new Mesh
            {
                vertices = vertices,
                triangles = triangles,
                uv = uvs
            };
        
            mesh.RecalculateNormals();
            return mesh;
        }
    
        private static int[] CalculateTrianglesForLanelet(int sumOfVerticesCount, bool isEqual, Vector3[] vertices)
        {
            // Find triangles count
            int[] triangles = new int[(sumOfVerticesCount - 2) * 3];
            bool isUsingEqualityAlgorithm = true;
            bool isTurnLeft = false;
        
            int firstVertex = 0; // A[0]
            int secondVertex = 2; // A[1]
            int thirdVertex = 1; // B[0]

            // Detect triangles
            if (isEqual)
            {
                // First triangle
                triangles[0] = firstVertex;
                triangles[1] = secondVertex;
                triangles[2] = thirdVertex;
            
                // Second and others
                for (var i = 3; i < triangles.Length; i += 3)
                {
                    firstVertex += 1;
                    if (i % 2 == 0)
                        secondVertex += 2;
                    else
                        thirdVertex += 2;
                
                    triangles[i] = firstVertex;
                    triangles[i + 1] = secondVertex;
                    triangles[i + 2] = thirdVertex;
                }
            }
            else
            {
                // First triangle
                triangles[0] = firstVertex;
                triangles[1] = secondVertex;
                triangles[2] = thirdVertex;
            
                // Second and others
                for (var i = 3; i < triangles.Length; i += 3)
                {
                    if (isUsingEqualityAlgorithm)
                    {
                        isTurnLeft = !isTurnLeft;
                    
                        // First algorithm (same with equality algorithm)
                        firstVertex += 1;
                        if (i % 2 == 0)
                            secondVertex += 2;
                        else
                            thirdVertex += 2;
                    }
                
                    // If there is no a node change the algorithm 
                    if (isUsingEqualityAlgorithm && vertices[firstVertex] == Vector3.zero || vertices[secondVertex] == Vector3.zero || vertices[thirdVertex] == Vector3.zero)
                    {
                        // Reset
                        if (i % 2 == 0)
                            secondVertex -= 2;
                        else
                            thirdVertex -= 2;
                    
                        // Assign default values
                        if (isTurnLeft)
                        {
                            firstVertex = thirdVertex;
                            thirdVertex = secondVertex;
                            secondVertex -= 2;
                        }
                        else
                        {
                            firstVertex = secondVertex;
                            secondVertex = thirdVertex;
                            thirdVertex = thirdVertex - 2;
                        }
                    
                        isUsingEqualityAlgorithm = false;
                    }
                
                    if (!isUsingEqualityAlgorithm)
                    {
                        // Second Algorithm
                        secondVertex += 2;
                        thirdVertex += 2;
                    }
                
                    triangles[i] = firstVertex;
                    triangles[i + 1] = secondVertex;
                    triangles[i + 2] = thirdVertex;
                }
            }
        
            return triangles;
        }

        public static Mesh CreateSimpleQuad(float width, float height, string name = "procedural_mesh")
        {
            Mesh mesh = new Mesh();
            mesh.name = name;

            Vector3[] vertices = new Vector3[4]
            {
                new Vector3(0, 0, 0),
                new Vector3(width, 0, 0),
                new Vector3(0, 0, height),
                new Vector3(width, 0, height)
            };
            mesh.vertices = vertices;

            int[] tris = new int[6]
            {
                0, 2, 1,
                2, 3, 1
            };
            mesh.triangles = tris;

            Vector3[] normals = new Vector3[4]
            {
                -Vector3.forward,
                -Vector3.forward,
                -Vector3.forward,
                -Vector3.forward
            };
            mesh.normals = normals;

            Vector2[] uv = new Vector2[4]
            {
                new Vector2(0, 0),
                new Vector2(1, 0),
                new Vector2(0, 1),
                new Vector2(1, 1)
            };
            mesh.uv = uv;

            return mesh;
        }
    }
}