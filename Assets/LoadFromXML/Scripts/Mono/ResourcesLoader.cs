using UnityEngine;

namespace LoadFromXML.Scripts.Mono
{
    public class ResourcesLoader : MonoBehaviour
    {
        private void Awake()
        {
            Resources.LoadAsync("Prefabs/Line");
            Resources.LoadAsync("Materials/Mat_Lanelet");
            Resources.LoadAsync("Materials/Mat_Line_Arrow_Bidirectional_Dashed");
            Resources.LoadAsync("Materials/Mat_Line_Arrow_Bidirectional_Solid");
            Resources.LoadAsync("Materials/Mat_Line_Arrow_Common_Dashed");
            Resources.LoadAsync("Materials/Mat_Line_Arrow_Common_Solid");
        }
    }
}