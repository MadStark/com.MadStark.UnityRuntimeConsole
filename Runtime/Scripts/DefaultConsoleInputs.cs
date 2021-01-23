using UnityEngine;

namespace MadStark.RuntimeConsole
{
    [DefaultExecutionOrder(-1000)]
    public class DefaultConsoleInputs : MonoBehaviour
    {
        [SerializeField] private ConsoleBehaviour console;
        [SerializeField] private KeyCode toggleKey = KeyCode.Tilde;
        [SerializeField] private KeyCode commandShortcut = KeyCode.Slash;


        private void Awake()
        {
            if (console == null)
                enabled = false;
        }

        private void Update()
        {
            if (Input.GetKeyDown(toggleKey) && !console.Focused)
            {
                if (console.Visible)
                    console.Hide();
                else
                    console.ShowAndFocus();
            }
            else if (Input.GetKeyDown(commandShortcut) && !console.Focused)
            {
                console.ShowAndFocus();
                console.Text = Console.kCommandPrefix.ToString();
            }
        }

        private void Reset()
        {
            console = GetComponent<ConsoleBehaviour>();
        }
    }
}
