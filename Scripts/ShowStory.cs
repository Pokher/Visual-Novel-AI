using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShowStory : MonoBehaviour
{
    [SerializeField] private Image currentImage;
    [SerializeField] private TextMeshProUGUI currentStoryBlock;
    [SerializeField] private StorySO StorySO;

    private List<string> imageLocations;
    private List<string> storyBlocks;
    private int slideCount;
    private int currentSlideIndex = 0;

    private string imagesLocation = @"C:\Users\walke\Unity Projects\AI-generated Visual Novel\Assets\Models\Image model\Generated Images";

    private void Start()
    {
        GetImageLocations();
        slideCount = StorySO.storyBlockCount;
        storyBlocks = StorySO.StoryBlocks;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return) && slideCount > currentSlideIndex)
        {
            if(!currentImage.enabled)
            {
                currentImage.enabled = true;
            }
            currentStoryBlock.text=storyBlocks[currentSlideIndex];
            LoadCurrentImage(imageLocations[currentSlideIndex]);
            currentSlideIndex++;
        }
    }

    private void GetImageLocations()
    {
        if(Directory.Exists(imagesLocation))
        {
            var files = Directory.GetFiles(imagesLocation, "*.*", SearchOption.TopDirectoryOnly).Where(file => file.EndsWith(".png") || file.EndsWith(".jpg")).ToList();
            files.Sort((file1, file2) => CompareByDateTime(file1, file2));
            imageLocations = files;
        }
        else
        {
            Debug.LogError("Images not found");
        }
    }

    private int CompareByDateTime(string filePath1, string filePath2)
    {
        string fileName1 = Path.GetFileNameWithoutExtension(filePath1);
        string fileName2 = Path.GetFileNameWithoutExtension(filePath2);

        string dateTimeString1 = fileName1.Substring("generated_image_".Length);
        string dateTimeString2 = fileName2.Substring("generated_image_".Length);

        // Parse the date and time part of the filename into DateTime objects
        DateTime dateTime1 = DateTime.ParseExact(dateTimeString1, "yyyyMMdd_HHmmss", null);
        DateTime dateTime2 = DateTime.ParseExact(dateTimeString2, "yyyyMMdd_HHmmss", null);

        // Compare the DateTime objects
        return dateTime1.CompareTo(dateTime2);
    }

    private void LoadCurrentImage(string location)
    {
        byte[] fileData = File.ReadAllBytes(location);
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(fileData);

        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        currentImage.sprite = sprite;
    }
}
