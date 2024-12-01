using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Diagnostics;
using System.Threading.Tasks;
using System.IO;

public class ApplyGeneratedImage : MonoBehaviour
{
    [SerializeField] private bool generateImage;
    [SerializeField] private DisplayStory imageChecker;
    [SerializeField] private int imageGenerationDelay;

    private string condaPath = @"C:\Users\walke\Anaconda3\Scripts\activate.bat";
    private string condaEnv = "base";
    private string pythonScript = @"C:\Users\walke\Unity Projects\AI-generated Visual Novel\Assets\Scripts\Python Codes\ImageGenerator.py";

    private Process process;
    private List<string> imagePrompts = new List<string>();
    private ApplyStoryGenerator applyGeneratedStory;

    private Image backgroundImage;

    private bool isStoryGenerated = false;
    private void Start()
    {
        applyGeneratedStory = GetComponent<ApplyStoryGenerator>();
        backgroundImage = GetComponent<Image>();
        imageGenerationDelay *= 1000;
    }

    private async void Update()
    {
        if (generateImage && isStoryGenerated && imageChecker.GetStoryStatus())
        {
            generateImage = false;
            imagePrompts = applyGeneratedStory.GetImagePrompts();
            foreach (string prompt in imagePrompts)
            {
                await RunPythonProcessAsync(prompt);
                await Task.Delay(imageGenerationDelay);
            }
            imageChecker.AreImagesReady(true);
        }
    }

    private async Task RunPythonProcessAsync(string prompt)
    {
        await Task.Run(() =>
        {
            ProcessStartInfo start = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/C \"{condaPath} && conda activate {condaEnv} && python \"{pythonScript}\" \"{prompt.Replace("\"", "\\\"")}\"\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,  // Capture Python script output
                RedirectStandardError = true,
                CreateNoWindow = true,  // Hide command window
                WindowStyle = ProcessWindowStyle.Hidden
            };

            // Start the process and track progress
            using (Process process = Process.Start(start))
            {
                // Read standard output
                Task<string> outputTask = process.StandardOutput.ReadToEndAsync();
                Task<string> errorTask = process.StandardError.ReadToEndAsync();

                // Wait for the process to complete
                process.WaitForExit();

                // Log output and errors
                string output = outputTask.Result;
                string error = errorTask.Result;

                if (!string.IsNullOrEmpty(output))
                {
                    string[] outputLines = output.Split('\n');
                    foreach (string line in outputLines)
                    {
                        if (line.Contains("/50"))
                        {
                            UnityEngine.Debug.Log($"Python Progress: {line}");
                        }
                        else
                        {
                            UnityEngine.Debug.Log($"Python Output: {line}");
                        }
                    }
                }

                if (!string.IsNullOrEmpty(error))
                {
                    UnityEngine.Debug.LogError($"Python Errors: {error}");
                }

                UnityEngine.Debug.Log("Python script finished execution.");
            }
        });
    }

    public void AllowImageGeneration()
    {
        isStoryGenerated = true;
    }
}
