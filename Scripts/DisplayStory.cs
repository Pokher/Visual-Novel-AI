using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DisplayStory : MonoBehaviour
{
    [SerializeField] private GameObject generatorGameObject;
    [SerializeField] private ApplyStoryGenerator applyStoryGenerator;
    [SerializeField] private StorySO storySO;

    private bool isStoryReady = false;
    private bool isImageReady = false;
    private List<string> storyBlocks = new List<string>();

    public void IsStoryReady(bool x)
    {
        isStoryReady = x;
        Debug.Log("Is Story Ready: " + x);
    }

    public void AreImagesReady(bool x)
    {
        isImageReady = x;
        Debug.Log("Is Image Ready: " + x);
    }

    public bool GetStoryStatus()
    {
        return isStoryReady;
    }
    
    public bool GetImageStatus()
    {
        return isImageReady;
    }

    private void Update()
    {
        if(isStoryReady && isImageReady)
        {
            isStoryReady = isImageReady = false;
            storyBlocks=applyStoryGenerator.GetStoryBlocks();
            storySO.StoryBlocks=storyBlocks;
            storySO.storyBlockCount=storyBlocks.Count;
            generatorGameObject.SetActive(false);
            SceneManager.LoadScene(1);
        }
    }
}
