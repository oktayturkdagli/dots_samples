using UnityEngine;

namespace MoveCurvit.Scripts.Data
{
    public class LineRendererDataHolder : MonoBehaviour
    {
        [SerializeField]
        private LineRenderer lineRenderer;

        public void SetLineRendererPosition(int index, Vector3 position)
        {
            lineRenderer.SetPosition(index, position);
        }

        public void SetLineRendererPositionCount(int count)
        {
            lineRenderer.positionCount = count;
        }
    }
}