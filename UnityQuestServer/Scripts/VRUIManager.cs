using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace QuestCameraStreamer
{
    /// <summary>
    /// Manages VR UI for controlling streaming in Meta Quest
    /// </summary>
    public class VRUIManager : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Canvas vrCanvas;
        [SerializeField] private TextMeshProUGUI ipAddressText;
        [SerializeField] private TextMeshProUGUI statusText;
        [SerializeField] private Button startStreamingButton;
        [SerializeField] private Button stopStreamingButton;
        [SerializeField] private GameObject streamingIndicator;
        [SerializeField] private Slider opacitySlider;
        [SerializeField] private TextMeshProUGUI opacityValueText;
        
        [Header("UI Settings")]
        [SerializeField] private float canvasDistance = 2f;
        [SerializeField] private float canvasScale = 0.002f;
        
        [Header("References")]
        [SerializeField] private StreamingServer streamingServer;
        [SerializeField] private PassthroughManager passthroughManager;
        
        private Transform cameraTransform;
        private bool isUIVisible = true;
        
        private void Awake()
        {
            if (streamingServer == null)
            {
                streamingServer = FindObjectOfType<StreamingServer>();
            }
            
            if (passthroughManager == null)
            {
                passthroughManager = FindObjectOfType<PassthroughManager>();
            }
            
            cameraTransform = Camera.main.transform;
            
            SetupUI();
        }
        
        private void SetupUI()
        {
            // Create VR Canvas if not assigned
            if (vrCanvas == null)
            {
                CreateVRCanvas();
            }
            
            // Configure canvas for VR
            vrCanvas.renderMode = RenderMode.WorldSpace;
            vrCanvas.transform.localScale = Vector3.one * canvasScale;
            
            // Position canvas in front of camera
            PositionCanvas();
            
            // Setup button listeners
            if (startStreamingButton != null)
            {
                startStreamingButton.onClick.AddListener(OnStartStreamingClicked);
            }
            
            if (stopStreamingButton != null)
            {
                stopStreamingButton.onClick.AddListener(OnStopStreamingClicked);
            }
            
            if (opacitySlider != null)
            {
                opacitySlider.onValueChanged.AddListener(OnOpacityChanged);
                opacitySlider.value = 1f;
            }
            
            // Subscribe to events
            if (streamingServer != null)
            {
                streamingServer.OnStreamingStatusChanged += OnStreamingStatusChanged;
            }
            
            if (passthroughManager != null)
            {
                passthroughManager.OnPassthroughStatusChanged += OnPassthroughStatusChanged;
            }
            
            UpdateUI();
        }
        
        private void CreateVRCanvas()
        {
            GameObject canvasObject = new GameObject("VR UI Canvas");
            vrCanvas = canvasObject.AddComponent<Canvas>();
            canvasObject.AddComponent<CanvasScaler>();
            canvasObject.AddComponent<GraphicRaycaster>();
            
            // Create background panel
            GameObject panel = new GameObject("Panel");
            panel.transform.SetParent(vrCanvas.transform, false);
            RectTransform panelRect = panel.AddComponent<RectTransform>();
            panelRect.sizeDelta = new Vector2(800, 600);
            panelRect.anchoredPosition = Vector2.zero;
            
            Image panelImage = panel.AddComponent<Image>();
            panelImage.color = new Color(0.1f, 0.1f, 0.1f, 0.9f);
            
            // Create title
            GameObject titleObject = new GameObject("Title");
            titleObject.transform.SetParent(panel.transform, false);
            TextMeshProUGUI title = titleObject.AddComponent<TextMeshProUGUI>();
            title.text = "Quest Camera Streamer";
            title.fontSize = 48;
            title.alignment = TextAlignmentOptions.Center;
            RectTransform titleRect = title.GetComponent<RectTransform>();
            titleRect.sizeDelta = new Vector2(700, 60);
            titleRect.anchoredPosition = new Vector2(0, 250);
            
            // Create IP address display
            GameObject ipObject = new GameObject("IP Address");
            ipObject.transform.SetParent(panel.transform, false);
            ipAddressText = ipObject.AddComponent<TextMeshProUGUI>();
            ipAddressText.fontSize = 36;
            ipAddressText.alignment = TextAlignmentOptions.Center;
            RectTransform ipRect = ipAddressText.GetComponent<RectTransform>();
            ipRect.sizeDelta = new Vector2(700, 50);
            ipRect.anchoredPosition = new Vector2(0, 150);
            
            // Create status text
            GameObject statusObject = new GameObject("Status");
            statusObject.transform.SetParent(panel.transform, false);
            statusText = statusObject.AddComponent<TextMeshProUGUI>();
            statusText.fontSize = 32;
            statusText.alignment = TextAlignmentOptions.Center;
            RectTransform statusRect = statusText.GetComponent<RectTransform>();
            statusRect.sizeDelta = new Vector2(700, 50);
            statusRect.anchoredPosition = new Vector2(0, 50);
            
            // Create Start button
            GameObject startButton = new GameObject("Start Button");
            startButton.transform.SetParent(panel.transform, false);
            startStreamingButton = startButton.AddComponent<Button>();
            Image startButtonImage = startButton.AddComponent<Image>();
            startButtonImage.color = new Color(0.2f, 0.7f, 0.2f);
            RectTransform startRect = startButton.GetComponent<RectTransform>();
            startRect.sizeDelta = new Vector2(250, 80);
            startRect.anchoredPosition = new Vector2(-150, -50);
            
            GameObject startText = new GameObject("Text");
            startText.transform.SetParent(startButton.transform, false);
            TextMeshProUGUI startLabel = startText.AddComponent<TextMeshProUGUI>();
            startLabel.text = "Start Streaming";
            startLabel.fontSize = 28;
            startLabel.alignment = TextAlignmentOptions.Center;
            startLabel.color = Color.white;
            RectTransform startLabelRect = startLabel.GetComponent<RectTransform>();
            startLabelRect.sizeDelta = new Vector2(250, 80);
            
            // Create Stop button
            GameObject stopButton = new GameObject("Stop Button");
            stopButton.transform.SetParent(panel.transform, false);
            stopStreamingButton = stopButton.AddComponent<Button>();
            Image stopButtonImage = stopButton.AddComponent<Image>();
            stopButtonImage.color = new Color(0.7f, 0.2f, 0.2f);
            RectTransform stopRect = stopButton.GetComponent<RectTransform>();
            stopRect.sizeDelta = new Vector2(250, 80);
            stopRect.anchoredPosition = new Vector2(150, -50);
            
            GameObject stopText = new GameObject("Text");
            stopText.transform.SetParent(stopButton.transform, false);
            TextMeshProUGUI stopLabel = stopText.AddComponent<TextMeshProUGUI>();
            stopLabel.text = "Stop Streaming";
            stopLabel.fontSize = 28;
            stopLabel.alignment = TextAlignmentOptions.Center;
            stopLabel.color = Color.white;
            RectTransform stopLabelRect = stopLabel.GetComponent<RectTransform>();
            stopLabelRect.sizeDelta = new Vector2(250, 80);
            
            // Create opacity slider
            GameObject sliderObject = new GameObject("Opacity Slider");
            sliderObject.transform.SetParent(panel.transform, false);
            opacitySlider = sliderObject.AddComponent<Slider>();
            RectTransform sliderRect = sliderObject.GetComponent<RectTransform>();
            sliderRect.sizeDelta = new Vector2(500, 30);
            sliderRect.anchoredPosition = new Vector2(0, -150);
            
            // Create opacity label
            GameObject opacityLabel = new GameObject("Opacity Label");
            opacityLabel.transform.SetParent(panel.transform, false);
            TextMeshProUGUI opacityText = opacityLabel.AddComponent<TextMeshProUGUI>();
            opacityText.text = "Passthrough Opacity:";
            opacityText.fontSize = 24;
            opacityText.alignment = TextAlignmentOptions.Center;
            RectTransform opacityLabelRect = opacityText.GetComponent<RectTransform>();
            opacityLabelRect.sizeDelta = new Vector2(200, 30);
            opacityLabelRect.anchoredPosition = new Vector2(-200, -200);
            
            // Create opacity value text
            GameObject opacityValue = new GameObject("Opacity Value");
            opacityValue.transform.SetParent(panel.transform, false);
            opacityValueText = opacityValue.AddComponent<TextMeshProUGUI>();
            opacityValueText.fontSize = 24;
            opacityValueText.alignment = TextAlignmentOptions.Center;
            RectTransform opacityValueRect = opacityValueText.GetComponent<RectTransform>();
            opacityValueRect.sizeDelta = new Vector2(100, 30);
            opacityValueRect.anchoredPosition = new Vector2(200, -200);
            
            // Create streaming indicator
            streamingIndicator = new GameObject("Streaming Indicator");
            streamingIndicator.transform.SetParent(panel.transform, false);
            Image indicatorImage = streamingIndicator.AddComponent<Image>();
            indicatorImage.color = Color.red;
            RectTransform indicatorRect = streamingIndicator.GetComponent<RectTransform>();
            indicatorRect.sizeDelta = new Vector2(30, 30);
            indicatorRect.anchoredPosition = new Vector2(350, 250);
        }
        
        private void PositionCanvas()
        {
            if (vrCanvas != null && cameraTransform != null)
            {
                Vector3 canvasPosition = cameraTransform.position + cameraTransform.forward * canvasDistance;
                vrCanvas.transform.position = canvasPosition;
                vrCanvas.transform.rotation = Quaternion.LookRotation(
                    vrCanvas.transform.position - cameraTransform.position
                );
            }
        }
        
        private void Update()
        {
            // Toggle UI visibility with controller button (e.g., Menu button)
            if (OVRInput.GetDown(OVRInput.Button.Start))
            {
                ToggleUIVisibility();
            }
            
            // Keep canvas in front of user when visible
            if (isUIVisible && vrCanvas != null)
            {
                PositionCanvas();
            }
            
            // Update streaming indicator animation
            if (streamingIndicator != null && streamingServer != null && streamingServer.IsStreaming)
            {
                float alpha = Mathf.PingPong(Time.time, 1f);
                Image indicator = streamingIndicator.GetComponent<Image>();
                if (indicator != null)
                {
                    Color color = indicator.color;
                    color.a = alpha;
                    indicator.color = color;
                }
            }
        }
        
        private void OnStartStreamingClicked()
        {
            if (streamingServer != null)
            {
                streamingServer.StartStreaming();
            }
        }
        
        private void OnStopStreamingClicked()
        {
            if (streamingServer != null)
            {
                streamingServer.StopStreaming();
            }
        }
        
        private void OnOpacityChanged(float value)
        {
            if (passthroughManager != null)
            {
                passthroughManager.SetPassthroughOpacity(value);
            }
            
            if (opacityValueText != null)
            {
                opacityValueText.text = $"{(value * 100):F0}%";
            }
        }
        
        private void OnStreamingStatusChanged(bool isStreaming, string ipAddress)
        {
            UpdateUI();
            
            if (ipAddressText != null)
            {
                ipAddressText.text = $"IP Address: {ipAddress}:8080";
            }
            
            if (statusText != null)
            {
                statusText.text = isStreaming ? "Status: Streaming" : "Status: Idle";
                statusText.color = isStreaming ? Color.green : Color.yellow;
            }
            
            if (streamingIndicator != null)
            {
                streamingIndicator.SetActive(isStreaming);
            }
        }
        
        private void OnPassthroughStatusChanged(bool isActive)
        {
            if (statusText != null)
            {
                string passthroughStatus = isActive ? "Passthrough Active" : "Passthrough Inactive";
                statusText.text = $"{statusText.text} | {passthroughStatus}";
            }
        }
        
        private void UpdateUI()
        {
            bool isStreaming = streamingServer != null && streamingServer.IsStreaming;
            
            if (startStreamingButton != null)
            {
                startStreamingButton.interactable = !isStreaming;
            }
            
            if (stopStreamingButton != null)
            {
                stopStreamingButton.interactable = isStreaming;
            }
        }
        
        private void ToggleUIVisibility()
        {
            isUIVisible = !isUIVisible;
            if (vrCanvas != null)
            {
                vrCanvas.gameObject.SetActive(isUIVisible);
            }
        }
        
        private void OnDestroy()
        {
            if (streamingServer != null)
            {
                streamingServer.OnStreamingStatusChanged -= OnStreamingStatusChanged;
            }
            
            if (passthroughManager != null)
            {
                passthroughManager.OnPassthroughStatusChanged -= OnPassthroughStatusChanged;
            }
        }
    }
}
