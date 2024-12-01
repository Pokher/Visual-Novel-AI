using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.IO;
using UnityEngine.WSA;
using UnityEngine.UI;

public class AcceptUserPrompt : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI userInput;
    [SerializeField] private TextMeshProUGUI userInputCharacterCounter;
    [SerializeField] private GameObject generatorGameObject;
    [SerializeField] private TextMeshProUGUI loadingScreen;
    [SerializeField] private Button enterButton;

    private string imagesLocation = @"C:\Users\walke\Unity Projects\AI-generated Visual Novel\Assets\Models\Image model\Generated Images";
    private string textLocation = @"C:\Users\walke\Unity Projects\AI-generated Visual Novel\Assets\Models\Generated Stories";

    /*
    Deep in the heart of the ancient forest of Eldergrove, the light of the fading sun filters through dense, towering trees. A lone adventurer, cloaked in a dark emerald cape, walks along a narrow path that winds around roots and fallen leaves. They clutch an old map with shaking hands, following a trail that has been hidden for centuries. It’s said that at the end lies the lost city of Aurelia, glittering with treasures and ancient wisdom. The air is thick with mystery, and each step echoes with anticipation. Strange sounds fill the air, and shadows seem to dance among the trees as if alive. As the adventurer ventures deeper, they wonder: will they be the first in hundreds of years to uncover the secrets of the lost city?
    */

    private void Start()
    {
        userInput.enableWordWrapping = true;
        DeleteOldStory(imagesLocation, textLocation);
    }

    public void UserInputInfo()
    {
        if(userInput.text.Length- 1 == 0)
        {
            Debug.Log("No user inout");
        }
        else if(userInput.text.Length - 1 < 350)
        {
            Debug.Log("User prompt not long enough");
        }
        else if(userInput.text.Length - 1 >= 350)
        {
            Debug.Log("User input has " + userInput.text.Length + " characters");
            Debug.Log("User input: " + userInput.text);
            generatorGameObject.SetActive(true);
            userInputCharacterCounter.gameObject.SetActive(false);
            loadingScreen.gameObject.SetActive(true);
            enterButton.gameObject.SetActive(false);
        }
        
    }

    private void Update()
    {
        userInputCharacterCounter.text = (userInput.text.Length - 1).ToString() + " /350";
    }

    private void DeleteOldStory(string images, string text)
    {
        string[] imageFiles = Directory.GetFiles(images);
        string[] textFiles = Directory.GetFiles(text);

        if(imageFiles.Length>0)
        {
            foreach(string imageFile in imageFiles)
            {
                File.Delete(imageFile);
                Debug.Log("Deleted image: " +  imageFile);
            }
        }
        else
        {
            Debug.Log("Image folder already empty");
        }

        if(textFiles.Length>0)
        {
            foreach(string textFile in textFiles)
            {
                File.Delete(textFile);
                Debug.Log("Deleted story file: " +  textFile);
            }
        }
        else
        {
            Debug.Log("Text folder empty");
        }
    }
}
