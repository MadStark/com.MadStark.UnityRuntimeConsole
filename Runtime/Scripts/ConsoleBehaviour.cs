using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MadStark.RuntimeConsole
{
    public class ConsoleBehaviour : MonoBehaviour
    {
        [Header("Bindings")]
        [SerializeField] private TMP_InputField inputField;
        [SerializeField] private RectTransform window;
        [SerializeField] private RectTransform content;

        [Header("Font")]
        [SerializeField] private Color informationColor = Color.white;
        [SerializeField] private Color warningColor = Color.yellow;
        [SerializeField] private Color errorColor = Color.red;
        [SerializeField] private TMP_FontAsset font;
        [SerializeField] private Material material;
        [SerializeField] private float fontSize = 14f;

        [Header("Settings")]
        [SerializeField] private bool hideOnStart = true;
        [SerializeField] private bool hideInputFieldOnUnfocused;


        public static ConsoleBehaviour Instance { get; private set; }

        public string Text {
            get => inputField.text;
            set {
                inputField.text = value;
                inputField.caretPosition = value.Length;
            }
        }
        public bool Visible => window.gameObject.activeInHierarchy;
        public bool Focused => EventSystem.current != null && EventSystem.current.currentSelectedGameObject == inputField.gameObject;

        private readonly List<TMP_Text> logs = new List<TMP_Text>(50);


        [RuntimeInitializeOnLoadMethod]
        private static void RegisterCommands()
        {
            Console.RegisterCommandsInType(typeof(ConsoleBehaviour));
        }

        private void Awake()
        {
            Console.onLog += HandleConsoleOnLog;
            Instance = this;
        }

        private void Start()
        {
            if (hideOnStart)
                Hide();
        }

        private void OnEnable()
        {
            inputField.onSubmit.AddListener(InputFieldOnSubmit);
        }

        private void OnDisable()
        {
            inputField.onSubmit.RemoveListener(InputFieldOnSubmit);
        }

        private void OnDestroy()
        {
            Console.onLog -= HandleConsoleOnLog;
        }

        private void HandleConsoleOnLog(DateTimeOffset time, string message, MessageSeverity severity)
        {
            TMP_Text text = CreateOrReuseLogText();
            ConfigureLogTextColor(text, severity);
            text.text = $"[{time:T}]: {message}";
        }

        [ContextMenu("Show")]
        public void Show()
        {
            window.gameObject.SetActive(true);

            if (!hideInputFieldOnUnfocused)
                inputField.gameObject.SetActive(true);
        }

        [ContextMenu("Hide")]
        public void Hide()
        {
            window.gameObject.SetActive(false);
            inputField.gameObject.SetActive(false);
        }

        public void ShowAndFocus()
        {
            Show();
            Focus();
        }

        public void Focus()
        {
            Show();

            inputField.gameObject.SetActive(true);

            if (EventSystem.current != null)
                EventSystem.current.SetSelectedGameObject(inputField.gameObject);
        }

        public void Unfocus()
        {
            if (hideInputFieldOnUnfocused)
                inputField.gameObject.SetActive(false);

            if (Focused)
                EventSystem.current.SetSelectedGameObject(null);
        }

        private void InputFieldOnSubmit(string message)
        {
            if (inputField.wasCanceled || string.IsNullOrWhiteSpace(message))
            {
                Unfocus();
            }
            else
            {
                Console.Interpret(message);
                inputField.text = string.Empty;
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(inputField.gameObject);
            }
        }

        private void ConfigureLogTextColor(TMP_Text text, MessageSeverity messageSeverity)
        {
            switch (messageSeverity)
            {
                case MessageSeverity.Error:
                    text.color = errorColor;
                    break;
                case MessageSeverity.Warning:
                    text.color = warningColor;
                    break;
                default:
                    text.color = informationColor;
                    break;
            }
        }

        [ConsoleCommand("clear")]
        private static void ClearCommand()
        {
            if (Instance == null)
                return;

            foreach (TMP_Text log in Instance.logs)
                log.gameObject.SetActive(false);
        }

        private TMP_Text CreateOrReuseLogText()
        {
            GameObject textGo;
            TMP_Text text;

            if (logs.Count == logs.Capacity)
            {
                text = logs[0];
                textGo = text.gameObject;
                logs.RemoveAt(0);
                logs.Add(text);
                textGo.SetActive(true);
            }
            else
            {
                textGo = new GameObject("Log");
                textGo.transform.SetParent(content);
                text = textGo.AddComponent<TextMeshProUGUI>();
                text.fontMaterial = material;
                text.font = font;
                text.fontSize = fontSize;
                logs.Add(text);
            }

            textGo.transform.SetAsLastSibling();
            return text;
        }
    }
}
