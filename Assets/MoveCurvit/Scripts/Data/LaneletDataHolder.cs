using UnityEngine;
using UnityEngine.Rendering;

namespace MoveCurvit.Scripts.Data
{
    public struct LaneletDataHolder
    {
        public Mesh Mesh;
        public BatchMeshID MeshBatchID;

        public LaneletDataHolder(Mesh mesh, BatchMeshID batchID)
        {
            Mesh = mesh;
            MeshBatchID = batchID;
        }
    }
}