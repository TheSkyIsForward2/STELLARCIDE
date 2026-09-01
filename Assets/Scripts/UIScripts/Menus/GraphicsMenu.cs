using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GraphicsMenu : MonoBehaviour
{
    // reference to options menu parent
    public OptionsMenu optionsMenu;
    // TODO add references to panel
    // List of graphics options
    private List<Resolution> _resolutions = new List<Resolution>();

    private void Awake()
    {
        foreach (Resolution resolution in Screen.resolutions)
        {
            _resolutions.Add(resolution);
        }
    }

    private void Start()
    {
        if (PlayerPrefs.GetString("Resolution") != null)
        {
            
        }
    }

    // return to options menu
    public void Return()
    {
        optionsMenu.ExitGraphics();
    }
    
    // TODO grab the graphics and change them!
    
    
}
