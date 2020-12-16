using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsController : MonoBehaviour
{
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private List<Image> terrainSizeButtons;
    [SerializeField] private List<Image> terrainStyleButtons;
    [SerializeField] private TerrainGenerator terrainGenerator;

    [SerializeField] private GameObject slider;
    [SerializeField] private GameObject sliderBackground;
    [SerializeField] private GameObject sliderText;

    private int size = 5;
    private bool standardTerrainStyle = true;
    private bool locked = false;

    private void Start()
    {
        CloseSettings();
        terrainSizeButtons[0].color = Color.green;
        terrainStyleButtons[0].color = Color.green;

        sliderBackground.SetActive(false);
        sliderText.SetActive(false);
    }

    public void OpenSettings()
    {
        if (!locked)
        {
            settingsPanel.SetActive(true);
        }
    }

    public void CloseSettings()
    {
        if (!locked)
        {
            settingsPanel.SetActive(false);
        }
    }

    public void SetTerainSize(int element)
    {
        foreach (var button in terrainSizeButtons)
        {
            button.color = Color.white;
        }

        terrainSizeButtons[element].color = Color.green;
        size = (element + 1) * 5;
    }

    public void SetTerrainStyle(int element)
    {
        foreach (var button in terrainStyleButtons)
        {
            button.color = Color.white;
        }

        terrainStyleButtons[element].color = Color.green;
        standardTerrainStyle = !Convert.ToBoolean(element);
    }   

    public void CreateTerrain()
    {
        locked = true;
        slider.GetComponent<Image>().fillAmount = 0f;
        sliderBackground.SetActive(true);
        sliderText.SetActive(true);

        if (standardTerrainStyle)
        {
            terrainGenerator.CreateTerrain(size, 5, 3);
        }
        else
        {
            terrainGenerator.CreateTerrain(size, 1, 20);
        }
    }

    public void OnTerrainPartGenerated(float part)
    {
        slider.GetComponent<Image>().fillAmount = part;
    }

    public void OnTerrainGenerated()
    {
        slider.GetComponent<Image>().fillAmount = 1f;
        sliderBackground.SetActive(false);
        sliderText.SetActive(false);
        locked = false;
    }
    
    public void Exit()
    {
        Application.Quit();
    }
}
