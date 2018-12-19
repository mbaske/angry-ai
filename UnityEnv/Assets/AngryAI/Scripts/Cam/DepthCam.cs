using UnityEngine;
using UnityEngine.Rendering;
using System.Collections.Generic;

public class DepthCam : MonoBehaviour
{
    public Texture2D Texture => tex;

    [SerializeField]
    private Material camMaterial;

    private Texture2D tex;
    private Camera cam;
    private Rect rect;
    private bool useAsync;
    private Queue<AsyncGPUReadbackRequest> requests;

    public Camera Initialize(ref Texture2D tex)
    {
        this.tex = tex;
        cam = GetComponent<Camera>();
        cam.depthTextureMode = DepthTextureMode.Depth
                             | DepthTextureMode.MotionVectors;
        cam.targetTexture = new RenderTexture(
            tex.width, tex.height, 16, RenderTextureFormat.ARGB32);

        useAsync = SystemInfo.supportsAsyncGPUReadback;
        if (useAsync)
        {
            requests = new Queue<AsyncGPUReadbackRequest>();
        }
        else
        {
            rect = new Rect(0, 0, tex.width, tex.height);
        }

        return cam;
    }

    private void Update()
    {
        if (useAsync)
        {
            bool done = false;
            while (requests.Count > 0)
            {
                AsyncGPUReadbackRequest req = requests.Peek();
                if (req.hasError)
                {
                    requests.Dequeue();
                }
                else if (req.done)
                {
                    tex.SetPixels32(req.GetData<Color32>().ToArray());
                    requests.Dequeue();
                    done = true;
                }
                else
                    break;
            }
            
            if (done)
            {
                tex.Apply();
                cam.targetTexture.DiscardContents();
            }
        }
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, camMaterial, 0);

        if (useAsync)
        {
            if (requests.Count < 2)
            {
                requests.Enqueue(AsyncGPUReadback.Request(destination));
            }
        }
        else
        {
            tex.ReadPixels(rect, 0, 0, false);
            tex.Apply();
            cam.targetTexture.DiscardContents();
        }
    }
}
