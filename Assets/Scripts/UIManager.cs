using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Slider widthSlider;   
    [SerializeField] private Slider heightSlider;

    [SerializeField] private TextMeshProUGUI widthAmount;
    [SerializeField] private TextMeshProUGUI heightAmount;

    [SerializeField] private Toggle showGenerationToggle;

    private void Start()
    {
        OnWidthValueChange();
        OnHeightValueChange();

        //adds listeners to the sliders and calls function every time the value is changed
        widthSlider.onValueChanged.AddListener(delegate { OnWidthValueChange(); });
        heightSlider.onValueChanged.AddListener(delegate { OnHeightValueChange(); });
    }

    //Changes text value to slider value so you can see the maze size in the UI
    private void OnWidthValueChange()
    {
        widthAmount.text = widthSlider.value.ToString();
    }

    private void OnHeightValueChange()
    {
        heightAmount.text = heightSlider.value.ToString();
    }

    //called when you click the GenerateMaze button.
    public void GenerateMaze()
    {
        int width = (int)widthSlider.value;
        int height = (int)heightSlider.value;

        //tells the RandomMazeGenerator to start generating a maze.
        RandomMazeGenerator.instance.CreateMazeGrid(width, height, showGenerationToggle.isOn);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
