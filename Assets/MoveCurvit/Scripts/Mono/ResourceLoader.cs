using UnityEngine;

namespace MoveCurvit.Scripts.Mono
{
    public class ResourceLoader : MonoBehaviour
    {
        private void Awake()
        {
            //Load Materials
            Resources.LoadAsync("Prefabs/Line");
            Resources.LoadAsync("Materials/Mat_Lanelet");
            Resources.LoadAsync("Materials/Mat_Line_Arrow_Bidirectional_Dashed");
            Resources.LoadAsync("Materials/Mat_Line_Arrow_Bidirectional_Solid");
            Resources.LoadAsync("Materials/Mat_Line_Arrow_Common_Dashed");
            Resources.LoadAsync("Materials/Mat_Line_Arrow_Common_Solid");
            Resources.LoadAsync("Materials/Mat_Node");

            //Load Prefabs
            Resources.Load("Prefabs/Line");
            Resources.Load("Prefabs/WayLineRenderer");
        
            //Load Mesh Assets
            Resources.GetBuiltinResource<Mesh>("Sphere.fbx");
            Resources.GetBuiltinResource<Mesh>("Plane.fbx");
        }
    }
}