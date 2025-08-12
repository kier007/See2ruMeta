using System.Collections;
using UnityEngine;
using UnityEngine.XR;
using Unity.XR.OpenXR;
using Unity.XR.OpenXR.Features.Meta;

namespace QuestCameraStreamer
{
    /// <summary>
    /// Manages Meta Quest passthrough camera functionality
    /// </summary>
    public class PassthroughManager : MonoBehaviour
    {
        [Header("Passthrough Settings")]
        [SerializeField] private bool enablePassthroughOnStart = true;
        [SerializeField] private float passthroughOpacity = 1.0f;
        
        private OVRPassthroughLayer passthroughLayer;
        private Camera mainCamera;
        private RenderTexture cameraRenderTexture;
        
        public delegate void PassthroughStatusChanged(bool isActive);
        public event PassthroughStatusChanged OnPassthroughStatusChanged;
        
        public bool IsPassthroughActive { get; private set; }
        public RenderTexture CameraTexture => cameraRenderTexture;
        
        private void Awake()
        {
            mainCamera = Camera.main;
            if (mainCamera == null)
            {
                Debug.LogError("Main camera not found!");
                return;
            }
            
            // Create render texture for streaming
            cameraRenderTexture = new RenderTexture(1920, 1080, 24, RenderTextureFormat.ARGB32);
            cameraRenderTexture.Create();
        }
        
        private void Start()
        {
            if (enablePassthroughOnStart)
            {
                StartCoroutine(InitializePassthrough());
            }
        }
        
        private IEnumerator InitializePassthrough()
        {
            // Wait for XR to initialize
            while (!XRSettings.isDeviceActive)
            {
                yield return null;
            }
            
            // Check if we're running on Quest
            if (!IsQuestDevice())
            {
                Debug.LogError("Not running on a Quest device!");
                yield break;
            }
            
            SetupPassthrough();
        }
        
        private bool IsQuestDevice()
        {
            var xrDisplaySubsystems = new List<XRDisplaySubsystem>();
            SubsystemManager.GetInstances(xrDisplaySubsystems);
            
            foreach (var subsystem in xrDisplaySubsystems)
            {
                if (subsystem.running)
                {
                    return true;
                }
            }
            
            return false;
        }
        
        private void SetupPassthrough()
        {
            // Create passthrough layer
            GameObject passthroughObject = new GameObject("PassthroughLayer");
            passthroughObject.transform.SetParent(mainCamera.transform);
            passthroughObject.transform.localPosition = Vector3.zero;
            passthroughObject.transform.localRotation = Quaternion.identity;
            
            passthroughLayer = passthroughObject.AddComponent<OVRPassthroughLayer>();
            passthroughLayer.projectionSurfaceType = OVRPassthroughLayer.ProjectionSurfaceType.Reconstructed;
            passthroughLayer.overlayType = OVROverlay.OverlayType.Underlay;
            passthroughLayer.compositionDepth = 0;
            
            // Configure camera for passthrough
            mainCamera.clearFlags = CameraClearFlags.SolidColor;
            mainCamera.backgroundColor = new Color(0, 0, 0, 0);
            mainCamera.targetTexture = cameraRenderTexture;
            
            EnablePassthrough();
        }
        
        public void EnablePassthrough()
        {
            if (passthroughLayer != null)
            {
                passthroughLayer.hidden = false;
                passthroughLayer.textureOpacity = passthroughOpacity;
                IsPassthroughActive = true;
                OnPassthroughStatusChanged?.Invoke(true);
                Debug.Log("Passthrough enabled");
            }
        }
        
        public void DisablePassthrough()
        {
            if (passthroughLayer != null)
            {
                passthroughLayer.hidden = true;
                IsPassthroughActive = false;
                OnPassthroughStatusChanged?.Invoke(false);
                Debug.Log("Passthrough disabled");
            }
        }
        
        public void SetPassthroughOpacity(float opacity)
        {
            passthroughOpacity = Mathf.Clamp01(opacity);
            if (passthroughLayer != null)
            {
                passthroughLayer.textureOpacity = passthroughOpacity;
            }
        }
        
        private void OnDestroy()
        {
            if (cameraRenderTexture != null)
            {
                cameraRenderTexture.Release();
                Destroy(cameraRenderTexture);
            }
        }
    }
}
