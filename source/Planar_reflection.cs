using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.Universal.Internal;

[ExecuteAlways]
public class Planar_reflection : MonoBehaviour
{

    private GameObject _reflectionGo;
    private Camera _reflectionCam;
    private Skybox _Skybox;
    private RenderTexture _camTex;
    public float farClipPlane = 1000f;
    private readonly int _PlanarReflectionTextureId = Shader.PropertyToID("_PlanarReflectionTexture");



    private void OnEnable()
    {
        RenderPipelineManager.beginCameraRendering += render;
    }


    private void OnDisable()
    {
        RenderPipelineManager.beginCameraRendering -= render;
        CleaupCamera();
    }

    private void OnDestroy()
    {
        RenderPipelineManager.beginCameraRendering -= render;
    }
    private void InitCam()
    {
        _reflectionGo = new GameObject("", typeof(Camera), typeof(Skybox));
        _reflectionGo.name = "PRCamera";
        _reflectionGo.hideFlags = HideFlags.HideAndDontSave;
        _reflectionCam = _reflectionGo.GetComponent<Camera>();
        _Skybox = _reflectionGo.GetComponent<Skybox>();
        _Skybox.enabled = false;
        _Skybox.material = null;

    }

    private void CleaupCamera()
    {
        CleanupRenderTex();
        if (_reflectionCam == null)
        {
            return;
        }
        if (Application.isEditor)
        {
            DestroyImmediate(_reflectionGo);
        }
        else
        {
            Destroy(_reflectionGo);
        }
    }
    private void CleanupRenderTex()
    {
        if(_camTex != null)
        {
            _camTex.Release();
            DestroyImmediate(_camTex);
            _camTex = null;
        }
        
    }

    private bool CheckCamera(Camera cam)
    {
        if (cam.cameraType == CameraType.Reflection)
        {
            return true;
        }

        return false;
    }
    private void render(ScriptableRenderContext context, Camera cam)
    {
        if (CheckCamera(cam))
        {
            return;
        }
        else if (_reflectionCam == null || _reflectionCam.Equals(null))
        {
            InitCam();
        }

        Vector3 normal = GetNormal();
        UpdateProbeSettings(cam);
        CreateRenderTex(cam);
        UpdateCameraTransform(cam, normal);
        ObliqueProjection(normal);
        UniversalRenderPipeline.RenderSingleCamera(context, _reflectionCam);
        Shader.SetGlobalTexture(_PlanarReflectionTextureId, _camTex);
        //Debug.Log(cam.name);
    }

    private void UpdateProbeSettings (Camera cam)
    {
        _reflectionCam.CopyFrom(cam);
        _reflectionCam.clearFlags = CameraClearFlags.SolidColor;
        _reflectionCam.backgroundColor = Color.black;
        _reflectionCam.useOcclusionCulling = false;
        _reflectionCam.cullingMask = cam.cullingMask & ~(1 << LayerMask.NameToLayer("NOT_Reflect_Object"));
        _reflectionCam.enabled = false;
        _reflectionCam.cameraType = CameraType.Reflection;
        _reflectionCam.usePhysicalProperties = false;
        _reflectionCam.farClipPlane = farClipPlane;
        _Skybox.material = null;
        _Skybox.enabled = false ;



    }
    private void CreateRenderTex(Camera cam)
    {
        int width = cam.pixelWidth;
        int height = cam.pixelHeight;
        if (_camTex == null || _camTex.width != width || _camTex.height != height)
        {
            if (_camTex != null)
            {
                _camTex.Release();
                DestroyImmediate(_camTex);
                _camTex = null;
            }

            _camTex = new RenderTexture(width, height, 24, RenderTextureFormat.ARGBHalf);
            _camTex.Create();
            _reflectionCam.targetTexture = _camTex;
        }
        else
        {
            _reflectionCam.targetTexture = _camTex;
        }
    }

    private Vector3 GetNormal()
    {
        return transform.up;
    }
    
    private void UpdateCameraTransform(Camera cam, Vector3 normal)
    {
        Vector3 proj = normal * Vector3.Dot(normal, cam.transform.position - transform.position);
        _reflectionCam.transform.position = cam.transform.position - 2 * proj;

        Vector3 illusionForward = Vector3.Reflect(cam.transform.forward, normal);
        Vector3 illusionUp = Vector3.Reflect(cam.transform.up, normal);
        _reflectionCam.transform.LookAt(_reflectionCam.transform.position + illusionForward, illusionUp);
    }

    private void ObliqueProjection (Vector3 normal)
    {
        Matrix4x4 viewMat = _reflectionCam.worldToCameraMatrix;
        Vector3 viewPosition = viewMat.MultiplyPoint(transform.position);
        Vector3 viewNormal = viewMat.MultiplyVector(normal);
        Vector4 plane = new Vector4(viewNormal.x, viewNormal.y, viewNormal.z, -Vector3.Dot(viewPosition, viewNormal));
        _reflectionCam.projectionMatrix = _reflectionCam.CalculateObliqueMatrix(plane);
    }
}
