using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;

public class ApplyStoryGenerator : MonoBehaviour
{
    [SerializeField] private bool generateStory;
    [SerializeField] private TextMeshProUGUI userInputText;
    [SerializeField] private int storyLength;
    [SerializeField] private int chunkLength;
    [SerializeField] private DisplayStory storyChecker;
    [SerializeField] private TMP_InputField userInputField;

    private string generatedStory;
    private List<string> storyChunks = new List<string>();
    private List<string> imagePrompts = new List<string>();
    private string condaPath = @"C:\Users\walke\Anaconda3\Scripts\activate.bat";
    private string condaEnv = "base";
    private string pythonScript = @"C:\Users\walke\Unity Projects\AI-generated Visual Novel\Assets\Scripts\Python Codes\StoryGenerator.py";
    private string storyFilePath = @"C:\Users\walke\Unity Projects\AI-generated Visual Novel\Assets\Models\Generated Stories\story2.txt";
    private ApplyGeneratedImage applyGeneratedImage;
    private string userPrompt;

    private bool isGenerating = false;
    private async void Start()
    {
        userPrompt = userInputText.text;
        userInputField.gameObject.SetActive(false);
        applyGeneratedImage = GetComponent<ApplyGeneratedImage>();
        UnityEngine.Debug.Log("Story generator recieved prompt: " + userPrompt);
        if(generateStory && !isGenerating)
        {
            generateStory = false;
            isGenerating = true;
            await RunPythonProcessAsync(userPrompt, storyLength);
            generatedStory = LoadStoryFromFile(storyFilePath);
            DisplayChunks(generatedStory);
            storyChecker.IsStoryReady(true);
            StartCoroutine(AllowImageGeneration());
        }
    }

    private async Task RunPythonProcessAsync(string prompt, int storyLength)
    {
        await Task.Run(() =>
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/C \"{condaPath} && conda activate {condaEnv} && python \"{pythonScript}\" \"{userPrompt}\" {storyLength}\"",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
     
            using (Process process = Process.Start(startInfo))
            {
                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();  // Wait for the process to complete
            }
            isGenerating = true;
        }
        );
    }

    private string LoadStoryFromFile(string path)
    {
        if(File.Exists(path))
        {
            return File.ReadAllText(path, Encoding.UTF8);
        }
        return string.Empty;
    }

    private void DisplayChunks(string story)
    {
        int start = 0;
        while(start < story.Length)
        {
            int length = Math.Min(start + chunkLength, story.Length);
            int periodIndex = story.IndexOf('.', length);
            if(periodIndex!=-1)
            {
                length = periodIndex + 1;
            }
            else
            {
                length = story.Length;
            }
            string chunk = story.Substring(start, length - start).Trim();

            if (chunk.Length <= 1 || chunk == "'")
            {
                // Skip this chunk and move to the next part of the story
                start = length;
                continue;
            }

            storyChunks.Add(chunk);

            string currentImagePrompt = ExtractPrompt(chunk);

            imagePrompts.Add(currentImagePrompt);

            UnityEngine.Debug.Log("The chunk is: " + chunk);
            UnityEngine.Debug.Log("The image prompt is: " + currentImagePrompt);
            start = length;
        }
    }

    private string ExtractPrompt(string chunk)
    {
        // Regex to capture phrases ending with nouns or adjective-noun pairs
        string pattern = @"(\b\w+\s+\w+|\b\w+)\b(?=\s+\w+\b)";
        MatchCollection matches = Regex.Matches(chunk, pattern);

        if (matches.Count > 0)
        {
            // Pick the first 3-7 word phrase
            foreach (Match match in matches)
            {
                string candidate = match.Value;
                int wordCount = candidate.Split(' ').Length;

                if (wordCount >= 3 && wordCount <= 7)
                {
                    return candidate; // Return the first 3-7 word phrase found
                }
            }
        }

        // If no suitable phrase is found, return the first sentence
        int sentenceEnd = chunk.IndexOf('.') + 1;
        return sentenceEnd > 0 ? chunk.Substring(0, sentenceEnd) : chunk;
    }

    public List<string> GetImagePrompts()
    {
        return imagePrompts;
    }

    private IEnumerator AllowImageGeneration()
    {
        yield return new WaitForSeconds(20f);
        applyGeneratedImage.AllowImageGeneration();
    }

    public List<string> GetStoryBlocks()
    {
        return storyChunks;
    }
}
